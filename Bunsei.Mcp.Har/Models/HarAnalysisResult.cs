namespace Bunsei.Mcp.Har.Models;

/// <summary>
/// HAR 文件的整体分析结果。
/// </summary>
public sealed class HarAnalysisResult
{
    /// <summary>
    /// 请求总数。
    /// </summary>
    public int TotalRequests { get; set; }

    /// <summary>
    /// 成功请求数量。
    /// </summary>
    public int SuccessfulRequests { get; set; }

    /// <summary>
    /// 失败请求数量。
    /// </summary>
    public int FailedRequests { get; set; }

    /// <summary>
    /// 所有请求耗时总和，单位为毫秒。
    /// </summary>
    public double TotalTimeMilliseconds { get; set; }

    /// <summary>
    /// 平均请求耗时，单位为毫秒。
    /// </summary>
    public double AverageTimeMilliseconds { get; set; }

    /// <summary>
    /// 耗时最长的请求摘要。
    /// </summary>
    public HarRequestSummary? SlowestRequest { get; set; }

    /// <summary>
    /// HTTP 状态码分布，键为状态码，值为请求数量。
    /// </summary>
    public Dictionary<int, int> StatusCodeDistribution { get; set; } = [];

    /// <summary>
    /// Host 分布，键为主机名，值为请求数量。
    /// </summary>
    public Dictionary<string, int> HostDistribution { get; set; } = [];

    /// <summary>
    /// 响应 MIME 类型分布，键为 MIME 类型，值为请求数量。
    /// </summary>
    public Dictionary<string, int> MimeTypeDistribution { get; set; } = [];

    /// <summary>
    /// 超过慢请求阈值的请求摘要列表。
    /// </summary>
    public IReadOnlyList<HarRequestSummary> SlowRequests { get; set; } = [];

    /// <summary>
    /// 失败请求摘要列表。
    /// </summary>
    public IReadOnlyList<HarRequestSummary> FailedRequestDetails { get; set; } = [];

    /// <summary>
    /// 自动检测到的潜在问题列表。
    /// </summary>
    public IReadOnlyList<HarIssue> Issues { get; set; } = [];
}
