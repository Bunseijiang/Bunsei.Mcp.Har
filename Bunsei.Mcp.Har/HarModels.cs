using System.Text.Json.Serialization;

namespace Bunsei.Mcp.Har;

/// <summary>
/// HAR 文件根对象。
/// </summary>
public sealed class HarArchive
{
    [JsonPropertyName("log")]
    public HarLog Log { get; set; } = new();
}

/// <summary>
/// HAR log 节点，包含版本、创建者、页面和请求条目。
/// </summary>
public sealed class HarLog
{
    [JsonPropertyName("version")]
    public string? Version { get; set; }

    [JsonPropertyName("creator")]
    public HarCreator? Creator { get; set; }

    [JsonPropertyName("browser")]
    public HarBrowser? Browser { get; set; }

    [JsonPropertyName("pages")]
    public List<HarPage>? Pages { get; set; }

    [JsonPropertyName("entries")]
    public List<HarEntry> Entries { get; set; } = [];

    [JsonPropertyName("comment")]
    public string? Comment { get; set; }
}

/// <summary>
/// 生成 HAR 文件的工具信息。
/// </summary>
public sealed class HarCreator
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("version")]
    public string? Version { get; set; }

    [JsonPropertyName("comment")]
    public string? Comment { get; set; }
}

/// <summary>
/// 采集 HAR 文件的浏览器信息。
/// </summary>
public sealed class HarBrowser
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("version")]
    public string? Version { get; set; }

    [JsonPropertyName("comment")]
    public string? Comment { get; set; }
}

/// <summary>
/// HAR 页面信息。
/// </summary>
public sealed class HarPage
{
    [JsonPropertyName("startedDateTime")]
    public DateTimeOffset StartedDateTime { get; set; }

    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("title")]
    public string? Title { get; set; }

    [JsonPropertyName("pageTimings")]
    public HarPageTimings? PageTimings { get; set; }

    [JsonPropertyName("comment")]
    public string? Comment { get; set; }
}

/// <summary>
/// 页面级加载时间信息。
/// </summary>
public sealed class HarPageTimings
{
    [JsonPropertyName("onContentLoad")]
    public double? OnContentLoad { get; set; }

    [JsonPropertyName("onLoad")]
    public double? OnLoad { get; set; }

    [JsonPropertyName("comment")]
    public string? Comment { get; set; }
}

/// <summary>
/// HAR 中的单个网络请求条目。
/// </summary>
public sealed class HarEntry
{
    [JsonPropertyName("pageref")]
    public string? PageRef { get; set; }

    [JsonPropertyName("startedDateTime")]
    public DateTimeOffset StartedDateTime { get; set; }

    [JsonPropertyName("time")]
    public double Time { get; set; }

    [JsonPropertyName("request")]
    public HarRequest Request { get; set; } = new();

    [JsonPropertyName("response")]
    public HarResponse Response { get; set; } = new();

    [JsonPropertyName("cache")]
    public HarCache? Cache { get; set; }

    [JsonPropertyName("timings")]
    public HarTimings Timings { get; set; } = new();

    [JsonPropertyName("serverIPAddress")]
    public string? ServerIPAddress { get; set; }

    [JsonPropertyName("connection")]
    public string? Connection { get; set; }

    [JsonPropertyName("comment")]
    public string? Comment { get; set; }
}

/// <summary>
/// HAR 请求对象。
/// </summary>
public sealed class HarRequest
{
    [JsonPropertyName("method")]
    public string? Method { get; set; }

    [JsonPropertyName("url")]
    public string? Url { get; set; }

    [JsonPropertyName("httpVersion")]
    public string? HttpVersion { get; set; }

    [JsonPropertyName("cookies")]
    public List<HarCookie> Cookies { get; set; } = [];

    [JsonPropertyName("headers")]
    public List<HarNameValue> Headers { get; set; } = [];

    [JsonPropertyName("queryString")]
    public List<HarNameValue> QueryString { get; set; } = [];

    [JsonPropertyName("postData")]
    public HarPostData? PostData { get; set; }

    [JsonPropertyName("headersSize")]
    public long HeadersSize { get; set; }

    [JsonPropertyName("bodySize")]
    public long BodySize { get; set; }

    [JsonPropertyName("comment")]
    public string? Comment { get; set; }
}

/// <summary>
/// HAR 响应对象。
/// </summary>
public sealed class HarResponse
{
    [JsonPropertyName("status")]
    public int Status { get; set; }

    [JsonPropertyName("statusText")]
    public string? StatusText { get; set; }

    [JsonPropertyName("httpVersion")]
    public string? HttpVersion { get; set; }

    [JsonPropertyName("cookies")]
    public List<HarCookie> Cookies { get; set; } = [];

    [JsonPropertyName("headers")]
    public List<HarNameValue> Headers { get; set; } = [];

    [JsonPropertyName("content")]
    public HarContent Content { get; set; } = new();

    [JsonPropertyName("redirectURL")]
    public string? RedirectUrl { get; set; }

    [JsonPropertyName("headersSize")]
    public long HeadersSize { get; set; }

    [JsonPropertyName("bodySize")]
    public long BodySize { get; set; }

    [JsonPropertyName("comment")]
    public string? Comment { get; set; }
}

/// <summary>
/// HTTP Cookie 信息。
/// </summary>
public sealed class HarCookie
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("value")]
    public string? Value { get; set; }

    [JsonPropertyName("path")]
    public string? Path { get; set; }

    [JsonPropertyName("domain")]
    public string? Domain { get; set; }

    [JsonPropertyName("expires")]
    public DateTimeOffset? Expires { get; set; }

    [JsonPropertyName("httpOnly")]
    public bool? HttpOnly { get; set; }

    [JsonPropertyName("secure")]
    public bool? Secure { get; set; }

    [JsonPropertyName("sameSite")]
    public string? SameSite { get; set; }

    [JsonPropertyName("comment")]
    public string? Comment { get; set; }
}

/// <summary>
/// HAR 中通用的名称和值结构，用于请求头、响应头和查询字符串。
/// </summary>
public sealed class HarNameValue
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("value")]
    public string? Value { get; set; }

    [JsonPropertyName("comment")]
    public string? Comment { get; set; }
}

/// <summary>
/// 请求提交数据。
/// </summary>
public sealed class HarPostData
{
    [JsonPropertyName("mimeType")]
    public string? MimeType { get; set; }

    [JsonPropertyName("params")]
    public List<HarPostParam>? Params { get; set; }

    [JsonPropertyName("text")]
    public string? Text { get; set; }

    [JsonPropertyName("comment")]
    public string? Comment { get; set; }
}

/// <summary>
/// 表单提交参数。
/// </summary>
public sealed class HarPostParam
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("value")]
    public string? Value { get; set; }

    [JsonPropertyName("fileName")]
    public string? FileName { get; set; }

    [JsonPropertyName("contentType")]
    public string? ContentType { get; set; }

    [JsonPropertyName("comment")]
    public string? Comment { get; set; }
}

/// <summary>
/// 响应内容信息。
/// </summary>
public sealed class HarContent
{
    [JsonPropertyName("size")]
    public long Size { get; set; }

    [JsonPropertyName("compression")]
    public long? Compression { get; set; }

    [JsonPropertyName("mimeType")]
    public string? MimeType { get; set; }

    [JsonPropertyName("text")]
    public string? Text { get; set; }

    [JsonPropertyName("encoding")]
    public string? Encoding { get; set; }

    [JsonPropertyName("comment")]
    public string? Comment { get; set; }
}

/// <summary>
/// HTTP 缓存信息。
/// </summary>
public sealed class HarCache
{
    [JsonPropertyName("beforeRequest")]
    public HarCacheState? BeforeRequest { get; set; }

    [JsonPropertyName("afterRequest")]
    public HarCacheState? AfterRequest { get; set; }

    [JsonPropertyName("comment")]
    public string? Comment { get; set; }
}

/// <summary>
/// 单次请求前后缓存状态。
/// </summary>
public sealed class HarCacheState
{
    [JsonPropertyName("expires")]
    public DateTimeOffset? Expires { get; set; }

    [JsonPropertyName("lastAccess")]
    public DateTimeOffset? LastAccess { get; set; }

    [JsonPropertyName("eTag")]
    public string? ETag { get; set; }

    [JsonPropertyName("hitCount")]
    public int HitCount { get; set; }

    [JsonPropertyName("comment")]
    public string? Comment { get; set; }
}

/// <summary>
/// 请求各阶段耗时信息，单位通常为毫秒。
/// </summary>
public sealed class HarTimings
{
    [JsonPropertyName("blocked")]
    public double? Blocked { get; set; }

    [JsonPropertyName("dns")]
    public double? Dns { get; set; }

    [JsonPropertyName("connect")]
    public double? Connect { get; set; }

    [JsonPropertyName("send")]
    public double Send { get; set; }

    [JsonPropertyName("wait")]
    public double Wait { get; set; }

    [JsonPropertyName("receive")]
    public double Receive { get; set; }

    [JsonPropertyName("ssl")]
    public double? Ssl { get; set; }

    [JsonPropertyName("comment")]
    public string? Comment { get; set; }
}
