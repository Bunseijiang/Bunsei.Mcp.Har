using Bunsei.Mcp.Har.Models;
using Bunsei.Mcp.Har.Serialization;
using System.Text;
using System.Text.Json;

namespace Bunsei.Mcp.Har.Services;

/// <summary>
/// 基于 HAR 数据结构实现加载、检索、统计和问题检测逻辑。
/// </summary>
public sealed class HarAnalysisService : IHarAnalysisService
{
    public async Task<HarArchive> LoadAsync(string filePath, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);

        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException("HAR 文件不存在。", filePath);
        }

        await using var stream = File.OpenRead(filePath);
        var archive = await JsonSerializer.DeserializeAsync(stream, HarJsonSerializerContext.Default.HarArchive, cancellationToken);

        if (archive?.Log is null)
        {
            throw new InvalidDataException("HAR 文件格式无效：缺少 log 节点。");
        }

        archive.Log.Entries ??= [];
        return archive;
    }

    public async Task<(HarArchive Archive, HarLoadResult Result)> LoadWithResultAsync(string filePath, CancellationToken cancellationToken = default)
    {
        var archive = await LoadAsync(filePath, cancellationToken);
        return (archive, GetLoadResult(archive, filePath));
    }

    public HarAnalysisResult Analyze(HarArchive archive, HarAnalysisOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(archive);

        options ??= new HarAnalysisOptions();
        var entries = GetEntries(archive);
        var summaries = entries.Select(entry => ToSummary(entry, options)).ToList();
        var failedRequests = summaries.Where(IsFailed).Take(options.MaxReturnedRequests).ToList();
        var slowRequests = summaries
            .Where(summary => summary.TimeMilliseconds >= options.SlowRequestThresholdMilliseconds)
            .OrderByDescending(summary => summary.TimeMilliseconds)
            .Take(options.MaxReturnedRequests)
            .ToList();

        return new HarAnalysisResult
        {
            TotalRequests = summaries.Count,
            SuccessfulRequests = summaries.Count(summary => !IsFailed(summary)),
            FailedRequests = summaries.Count(IsFailed),
            TotalTimeMilliseconds = summaries.Sum(summary => summary.TimeMilliseconds),
            AverageTimeMilliseconds = summaries.Count == 0 ? 0 : summaries.Average(summary => summary.TimeMilliseconds),
            SlowestRequest = summaries.OrderByDescending(summary => summary.TimeMilliseconds).FirstOrDefault(),
            StatusCodeDistribution = summaries
                .GroupBy(summary => summary.Status)
                .OrderBy(group => group.Key)
                .ToDictionary(group => group.Key, group => group.Count()),
            HostDistribution = summaries
                .Where(summary => !string.IsNullOrWhiteSpace(summary.Host))
                .GroupBy(summary => summary.Host!, StringComparer.OrdinalIgnoreCase)
                .OrderByDescending(group => group.Count())
                .ToDictionary(group => group.Key, group => group.Count(), StringComparer.OrdinalIgnoreCase),
            MimeTypeDistribution = summaries
                .Where(summary => !string.IsNullOrWhiteSpace(summary.MimeType))
                .GroupBy(summary => summary.MimeType!, StringComparer.OrdinalIgnoreCase)
                .OrderByDescending(group => group.Count())
                .ToDictionary(group => group.Key, group => group.Count(), StringComparer.OrdinalIgnoreCase),
            SlowRequests = slowRequests,
            FailedRequestDetails = failedRequests,
            Issues = DetectIssues(archive, options)
        };
    }

    public HarLoadResult GetLoadResult(HarArchive archive, string filePath = "")
    {
        ArgumentNullException.ThrowIfNull(archive);

        var entries = GetEntries(archive);

        return new HarLoadResult
        {
            Loaded = true,
            FilePath = filePath,
            RequestCount = entries.Count,
            PageCount = archive.Log.Pages?.Count ?? 0,
            StartedDateTime = entries.Count == 0 ? null : entries.Min(entry => entry.StartedDateTime),
            Hosts = entries
                .Select(entry => TryGetHost(entry.Request.Url))
                .Where(host => !string.IsNullOrWhiteSpace(host))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .Order(StringComparer.OrdinalIgnoreCase)
                .ToList()!
        };
    }

    public IReadOnlyList<HarRequestSummary> ListRequests(HarArchive archive, int offset = 0, int limit = 50, HarAnalysisOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(archive);
        ValidatePaging(offset, limit);

        options ??= new HarAnalysisOptions();

        return GetEntries(archive)
            .Select((entry, index) => ToSummary(entry, index, options))
            .Skip(offset)
            .Take(limit)
            .ToList();
    }

    public HarRequestDetail GetRequestDetail(HarArchive archive, int requestIndex)
    {
        ArgumentNullException.ThrowIfNull(archive);

        var entry = GetEntryByIndex(archive, requestIndex);

        return new HarRequestDetail
        {
            Index = requestIndex,
            PageRef = entry.PageRef,
            StartedDateTime = entry.StartedDateTime,
            TimeMilliseconds = entry.Time,
            Request = entry.Request,
            Response = entry.Response,
            Cache = entry.Cache,
            Timings = entry.Timings,
            ServerIPAddress = entry.ServerIPAddress,
            Connection = entry.Connection,
            Comment = entry.Comment
        };
    }

    public IReadOnlyList<HarRequestSummary> SearchRequests(HarArchive archive, HarSearchOptions options, HarAnalysisOptions? analysisOptions = null)
    {
        ArgumentNullException.ThrowIfNull(archive);
        ArgumentNullException.ThrowIfNull(options);
        ValidatePaging(options.Offset, options.Limit);

        analysisOptions ??= new HarAnalysisOptions();

        return GetEntries(archive)
            .Select((entry, index) => new { Entry = entry, Summary = ToSummary(entry, index, analysisOptions) })
            .Where(item => MatchesSearch(item.Entry, item.Summary, options))
            .Select(item => item.Summary)
            .Skip(options.Offset)
            .Take(options.Limit)
            .ToList();
    }

    public IReadOnlyList<HarTimelineItem> GetTimeline(HarArchive archive, int offset = 0, int limit = 100, HarAnalysisOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(archive);
        ValidatePaging(offset, limit);

        options ??= new HarAnalysisOptions();
        var entries = GetEntries(archive);

        if (entries.Count == 0)
        {
            return [];
        }

        var firstStart = entries.Min(entry => entry.StartedDateTime);

        return entries
            .Select((entry, index) => new HarTimelineItem
            {
                Index = index,
                StartedDateTime = entry.StartedDateTime,
                RelativeStartMilliseconds = (entry.StartedDateTime - firstStart).TotalMilliseconds,
                DurationMilliseconds = entry.Time,
                Method = entry.Request.Method,
                Url = options.RedactSensitiveData ? RedactUrl(entry.Request.Url) : entry.Request.Url,
                Status = entry.Response.Status,
                MimeType = entry.Response.Content.MimeType
            })
            .Skip(offset)
            .Take(limit)
            .ToList();
    }

    public HarContentChunk GetRequestBodyChunk(HarArchive archive, int requestIndex, int offset, int length, HarContentFormat format = HarContentFormat.Text, string textEncodingName = "utf-8")
    {
        ArgumentNullException.ThrowIfNull(archive);

        var entry = GetEntryByIndex(archive, requestIndex);
        var text = entry.Request.PostData?.Text ?? string.Empty;

        return CreateContentChunk(requestIndex, "request", text, null, offset, length, format, textEncodingName);
    }

    public HarContentChunk GetResponseBodyChunk(HarArchive archive, int requestIndex, int offset, int length, HarContentFormat format = HarContentFormat.Text, string textEncodingName = "utf-8")
    {
        ArgumentNullException.ThrowIfNull(archive);

        var entry = GetEntryByIndex(archive, requestIndex);
        var content = entry.Response.Content;
        var text = content.Text ?? string.Empty;

        return CreateContentChunk(requestIndex, "response", text, content.Encoding, offset, length, format, textEncodingName);
    }

    public IReadOnlyList<HarRequestSummary> GetFailedRequests(HarArchive archive, HarAnalysisOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(archive);

        options ??= new HarAnalysisOptions();
        return GetEntries(archive)
            .Select((entry, index) => ToSummary(entry, index, options))
            .Where(IsFailed)
            .OrderByDescending(summary => summary.TimeMilliseconds)
            .Take(options.MaxReturnedRequests)
            .ToList();
    }

    public IReadOnlyList<HarRequestSummary> GetSlowRequests(HarArchive archive, double? minimumMilliseconds = null, HarAnalysisOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(archive);

        options ??= new HarAnalysisOptions();
        var threshold = minimumMilliseconds ?? options.SlowRequestThresholdMilliseconds;

        return GetEntries(archive)
            .Select((entry, index) => ToSummary(entry, index, options))
            .Where(summary => summary.TimeMilliseconds >= threshold)
            .OrderByDescending(summary => summary.TimeMilliseconds)
            .Take(options.MaxReturnedRequests)
            .ToList();
    }

    public IReadOnlyList<HarIssue> DetectIssues(HarArchive archive, HarAnalysisOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(archive);

        options ??= new HarAnalysisOptions();
        var issues = new List<HarIssue>();

        foreach (var entry in GetEntries(archive))
        {
            var summary = ToSummary(entry, options);

            if (summary.Status >= 500)
            {
                issues.Add(CreateIssue("ServerError", "High", $"服务端错误：HTTP {summary.Status}。", summary));
            }
            else if (summary.Status >= 400)
            {
                issues.Add(CreateIssue("ClientError", "Medium", $"客户端错误：HTTP {summary.Status}。", summary));
            }
            else if (summary.Status == 0)
            {
                issues.Add(CreateIssue("NoResponse", "High", "请求没有有效 HTTP 响应状态。", summary));
            }

            if (summary.TimeMilliseconds >= options.SlowRequestThresholdMilliseconds)
            {
                issues.Add(CreateIssue("SlowRequest", "Medium", $"请求耗时 {summary.TimeMilliseconds:F0}ms，超过阈值 {options.SlowRequestThresholdMilliseconds:F0}ms。", summary));
            }

            if (summary.ResponseBodySize >= options.LargeResponseThresholdBytes)
            {
                issues.Add(CreateIssue("LargeResponse", "Low", $"响应体大小 {summary.ResponseBodySize} 字节，超过阈值 {options.LargeResponseThresholdBytes} 字节。", summary));
            }

            if (entry.Timings.Dns is >= 0 && entry.Timings.Dns >= options.DnsThresholdMilliseconds)
            {
                issues.Add(CreateIssue("SlowDns", "Medium", $"DNS 耗时 {entry.Timings.Dns:F0}ms，超过阈值 {options.DnsThresholdMilliseconds:F0}ms。", summary));
            }

            if (entry.Timings.Ssl is >= 0 && entry.Timings.Ssl >= options.SslThresholdMilliseconds)
            {
                issues.Add(CreateIssue("SlowSsl", "Medium", $"SSL 握手耗时 {entry.Timings.Ssl:F0}ms，超过阈值 {options.SslThresholdMilliseconds:F0}ms。", summary));
            }
        }

        AddDuplicateRequestIssues(issues, archive, options);

        return issues.Take(options.MaxReturnedRequests).ToList();
    }

    private static IReadOnlyList<HarEntry> GetEntries(HarArchive archive)
    {
        return archive.Log?.Entries ?? [];
    }

    private static HarEntry GetEntryByIndex(HarArchive archive, int requestIndex)
    {
        var entries = GetEntries(archive);

        if (requestIndex < 0 || requestIndex >= entries.Count)
        {
            throw new ArgumentOutOfRangeException(nameof(requestIndex), requestIndex, $"请求索引必须在 0 到 {entries.Count - 1} 之间。");
        }

        return entries[requestIndex];
    }

    private static void ValidatePaging(int offset, int limit)
    {
        if (offset < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(offset), offset, "偏移量不能小于 0。");
        }

        if (limit < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(limit), limit, "数量不能小于 0。");
        }
    }

    private static HarContentChunk CreateContentChunk(int requestIndex, string source, string text, string? encoding, int offset, int length, HarContentFormat format, string textEncodingName)
    {
        if (offset < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(offset), offset, "偏移量不能小于 0。");
        }

        if (length < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(length), length, "长度不能小于 0。");
        }

        var bytes = GetContentBytes(text, encoding);
        var totalLength = bytes.Length;
        var availableLength = offset >= totalLength ? 0 : Math.Min(length, totalLength - offset);
        var chunk = availableLength == 0 ? [] : bytes.AsSpan(offset, availableLength).ToArray();

        return new HarContentChunk
        {
            RequestIndex = requestIndex,
            Source = source,
            Format = format,
            TextEncodingName = format == HarContentFormat.Text ? textEncodingName : null,
            Offset = offset,
            Length = chunk.Length,
            TotalLength = totalLength,
            EndOfContent = offset + chunk.Length >= totalLength,
            Content = FormatContent(chunk, format, textEncodingName)
        };
    }

    private static byte[] GetContentBytes(string text, string? encoding)
    {
        if (string.Equals(encoding, "base64", StringComparison.OrdinalIgnoreCase))
        {
            return Convert.FromBase64String(text);
        }

        return Encoding.UTF8.GetBytes(text);
    }

    private static string FormatContent(byte[] bytes, HarContentFormat format, string textEncodingName)
    {
        return format switch
        {
            HarContentFormat.Text => GetTextEncoding(textEncodingName).GetString(bytes),
            HarContentFormat.Base64 => Convert.ToBase64String(bytes),
            HarContentFormat.Hex => string.Join(' ', bytes.Select(value => $"0x{value:X2}")),
            _ => throw new ArgumentOutOfRangeException(nameof(format), format, "不支持的内容格式。")
        };
    }

    private static Encoding GetTextEncoding(string textEncodingName)
    {
        if (string.IsNullOrWhiteSpace(textEncodingName))
        {
            return Encoding.UTF8;
        }

        return Encoding.GetEncoding(textEncodingName);
    }

    private static HarRequestSummary ToSummary(HarEntry entry, HarAnalysisOptions options)
    {
        return ToSummary(entry, -1, options);
    }

    private static HarRequestSummary ToSummary(HarEntry entry, int index, HarAnalysisOptions options)
    {
        var url = options.RedactSensitiveData ? RedactUrl(entry.Request.Url) : entry.Request.Url;

        return new HarRequestSummary
        {
            Index = index,
            Method = entry.Request.Method,
            Url = url,
            Host = TryGetHost(entry.Request.Url),
            Status = entry.Response.Status,
            StatusText = entry.Response.StatusText,
            TimeMilliseconds = entry.Time,
            MimeType = entry.Response.Content.MimeType,
            RequestBodySize = entry.Request.BodySize,
            ResponseBodySize = entry.Response.BodySize >= 0 ? entry.Response.BodySize : entry.Response.Content.Size,
            StartedDateTime = entry.StartedDateTime
        };
    }

    private static bool MatchesSearch(HarEntry entry, HarRequestSummary summary, HarSearchOptions options)
    {
        if (!string.IsNullOrWhiteSpace(options.Method) && !string.Equals(summary.Method, options.Method, StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        if (!string.IsNullOrWhiteSpace(options.Host) && !string.Equals(summary.Host, options.Host, StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        if (options.Status.HasValue && summary.Status != options.Status.Value)
        {
            return false;
        }

        if (options.MinStatus.HasValue && summary.Status < options.MinStatus.Value)
        {
            return false;
        }

        if (options.MaxStatus.HasValue && summary.Status > options.MaxStatus.Value)
        {
            return false;
        }

        if (!string.IsNullOrWhiteSpace(options.MimeType) && !string.Equals(summary.MimeType, options.MimeType, StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        if (!string.IsNullOrWhiteSpace(options.Keyword) && !ContainsKeyword(entry, summary, options.Keyword))
        {
            return false;
        }

        return true;
    }

    private static bool ContainsKeyword(HarEntry entry, HarRequestSummary summary, string keyword)
    {
        return Contains(summary.Url, keyword)
            || Contains(summary.StatusText, keyword)
            || Contains(entry.Request.PostData?.Text, keyword)
            || Contains(entry.Response.Content.Text, keyword)
            || entry.Request.Headers.Any(header => Contains(header.Name, keyword) || Contains(header.Value, keyword))
            || entry.Response.Headers.Any(header => Contains(header.Name, keyword) || Contains(header.Value, keyword))
            || entry.Request.QueryString.Any(item => Contains(item.Name, keyword) || Contains(item.Value, keyword));
    }

    private static bool Contains(string? value, string keyword)
    {
        return value?.Contains(keyword, StringComparison.OrdinalIgnoreCase) == true;
    }

    private static bool IsFailed(HarRequestSummary summary)
    {
        return summary.Status == 0 || summary.Status >= 400;
    }

    private static HarIssue CreateIssue(string type, string severity, string message, HarRequestSummary request)
    {
        return new HarIssue
        {
            Type = type,
            Severity = severity,
            Message = message,
            Request = request
        };
    }

    private static void AddDuplicateRequestIssues(List<HarIssue> issues, HarArchive archive, HarAnalysisOptions options)
    {
        var duplicatedRequests = GetEntries(archive)
            .GroupBy(entry => new
            {
                Method = entry.Request.Method ?? string.Empty,
                Url = options.RedactSensitiveData ? RedactUrl(entry.Request.Url) ?? string.Empty : entry.Request.Url ?? string.Empty
            })
            .Where(group => group.Key.Url.Length > 0 && group.Count() > 1)
            .OrderByDescending(group => group.Count())
            .Take(options.MaxReturnedRequests);

        foreach (var group in duplicatedRequests)
        {
            var first = group.First();
            var summary = ToSummary(first, options);
            issues.Add(CreateIssue("DuplicateRequest", "Low", $"发现重复请求 {group.Count()} 次。", summary));
        }
    }

    private static string? TryGetHost(string? url)
    {
        if (Uri.TryCreate(url, UriKind.Absolute, out var uri))
        {
            return uri.Host;
        }

        return null;
    }

    private static string? RedactUrl(string? url)
    {
        if (string.IsNullOrWhiteSpace(url) || !Uri.TryCreate(url, UriKind.Absolute, out var uri))
        {
            return url;
        }

        // 仅隐藏常见敏感查询参数的值，保留参数名便于定位请求来源。
        var sensitiveKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "access_token",
            "refresh_token",
            "id_token",
            "token",
            "api_key",
            "apikey",
            "key",
            "password",
            "pwd",
            "secret",
            "authorization"
        };

        if (string.IsNullOrEmpty(uri.Query))
        {
            return url;
        }

        var query = uri.Query.TrimStart('?').Split('&', StringSplitOptions.RemoveEmptyEntries);
        var redactedQuery = query.Select(item =>
        {
            var separatorIndex = item.IndexOf('=', StringComparison.Ordinal);
            var name = separatorIndex >= 0 ? item[..separatorIndex] : item;
            var value = separatorIndex >= 0 ? item[(separatorIndex + 1)..] : string.Empty;

            return sensitiveKeys.Contains(Uri.UnescapeDataString(name))
                ? $"{name}=***"
                : separatorIndex >= 0 ? $"{name}={value}" : name;
        });

        var builder = new UriBuilder(uri)
        {
            Query = string.Join('&', redactedQuery)
        };

        return builder.Uri.ToString();
    }
}
