namespace Bunsei.Mcp.Har.Models;

/// <summary>
/// HAR 请求时间线中的单个请求项。
/// </summary>
public sealed class HarTimelineItem
{
    /// <summary>
    /// 请求在 HAR entries 中的零基索引。
    /// </summary>
    public int Index { get; set; }

    /// <summary>
    /// 请求开始时间。
    /// </summary>
    public DateTimeOffset StartedDateTime { get; set; }

    /// <summary>
    /// 相对于首个请求开始时间的偏移量，单位为毫秒。
    /// </summary>
    public double RelativeStartMilliseconds { get; set; }

    /// <summary>
    /// 请求持续时间，单位为毫秒。
    /// </summary>
    public double DurationMilliseconds { get; set; }

    /// <summary>
    /// HTTP 方法。
    /// </summary>
    public string? Method { get; set; }

    /// <summary>
    /// 请求 URL。
    /// </summary>
    public string? Url { get; set; }

    /// <summary>
    /// HTTP 响应状态码。
    /// </summary>
    public int Status { get; set; }

    /// <summary>
    /// 响应 MIME 类型。
    /// </summary>
    public string? MimeType { get; set; }
}
