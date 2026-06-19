namespace Bunsei.Mcp.Har.Models;

/// <summary>
/// 单个 HAR 请求的摘要信息。
/// </summary>
public sealed class HarRequestSummary
{
    /// <summary>
    /// 请求在 HAR entries 中的零基索引。
    /// </summary>
    public int Index { get; set; }

    /// <summary>
    /// HTTP 方法。
    /// </summary>
    public string? Method { get; set; }

    /// <summary>
    /// 请求 URL。
    /// </summary>
    public string? Url { get; set; }

    /// <summary>
    /// 请求 URL 中的 Host。
    /// </summary>
    public string? Host { get; set; }

    /// <summary>
    /// HTTP 响应状态码。
    /// </summary>
    public int Status { get; set; }

    /// <summary>
    /// HTTP 响应状态文本。
    /// </summary>
    public string? StatusText { get; set; }

    /// <summary>
    /// 请求总耗时，单位为毫秒。
    /// </summary>
    public double TimeMilliseconds { get; set; }

    /// <summary>
    /// 响应 MIME 类型。
    /// </summary>
    public string? MimeType { get; set; }

    /// <summary>
    /// 请求体大小，单位为字节。
    /// </summary>
    public long RequestBodySize { get; set; }

    /// <summary>
    /// 响应体大小，单位为字节。
    /// </summary>
    public long ResponseBodySize { get; set; }

    /// <summary>
    /// 请求开始时间。
    /// </summary>
    public DateTimeOffset StartedDateTime { get; set; }
}
