namespace Bunsei.Mcp.Har.Models;

/// <summary>
/// HAR 分析过程中检测到的潜在问题。
/// </summary>
public sealed class HarIssue
{
    /// <summary>
    /// 问题类型，例如 ServerError、SlowRequest 或 DuplicateRequest。
    /// </summary>
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// 问题严重程度，例如 High、Medium 或 Low。
    /// </summary>
    public string Severity { get; set; } = string.Empty;

    /// <summary>
    /// 面向用户的问题说明。
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// 与问题关联的请求摘要。
    /// </summary>
    public HarRequestSummary? Request { get; set; }
}
