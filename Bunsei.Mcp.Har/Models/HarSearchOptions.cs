namespace Bunsei.Mcp.Har.Models;

/// <summary>
/// HAR 请求搜索条件。
/// </summary>
public sealed class HarSearchOptions
{
    /// <summary>
    /// 关键字，会匹配 URL、状态文本、请求/响应头、查询字符串和正文。
    /// </summary>
    public string? Keyword { get; set; }

    /// <summary>
    /// HTTP 方法过滤条件。
    /// </summary>
    public string? Method { get; set; }

    /// <summary>
    /// Host 过滤条件。
    /// </summary>
    public string? Host { get; set; }

    /// <summary>
    /// 精确 HTTP 状态码过滤条件。
    /// </summary>
    public int? Status { get; set; }

    /// <summary>
    /// 最小 HTTP 状态码过滤条件。
    /// </summary>
    public int? MinStatus { get; set; }

    /// <summary>
    /// 最大 HTTP 状态码过滤条件。
    /// </summary>
    public int? MaxStatus { get; set; }

    /// <summary>
    /// 响应 MIME 类型过滤条件。
    /// </summary>
    public string? MimeType { get; set; }

    /// <summary>
    /// 搜索结果分页偏移量。
    /// </summary>
    public int Offset { get; set; }

    /// <summary>
    /// 搜索结果分页返回数量上限。
    /// </summary>
    public int Limit { get; set; } = 50;
}
