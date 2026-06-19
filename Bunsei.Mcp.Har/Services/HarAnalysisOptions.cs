namespace Bunsei.Mcp.Har.Services;

/// <summary>
/// HAR 分析过程中的阈值与输出控制选项。
/// </summary>
public sealed class HarAnalysisOptions
{
    /// <summary>
    /// 是否在输出 URL 时隐藏 token、password、secret 等敏感查询参数。
    /// </summary>
    public bool RedactSensitiveData { get; set; }

    /// <summary>
    /// 判断慢请求的耗时阈值，单位为毫秒。
    /// </summary>
    public double SlowRequestThresholdMilliseconds { get; set; } = 1000;

    /// <summary>
    /// 判断 DNS 解析过慢的阈值，单位为毫秒。
    /// </summary>
    public double DnsThresholdMilliseconds { get; set; } = 300;

    /// <summary>
    /// 判断 SSL 握手过慢的阈值，单位为毫秒。
    /// </summary>
    public double SslThresholdMilliseconds { get; set; } = 500;

    /// <summary>
    /// 判断大响应体的字节数阈值。
    /// </summary>
    public long LargeResponseThresholdBytes { get; set; } = 1024 * 1024;

    /// <summary>
    /// 分析结果中最多返回的请求或问题数量。
    /// </summary>
    public int MaxReturnedRequests { get; set; } = 50;
}
