namespace Bunsei.Mcp.Har.Models;

/// <summary>
/// 单个 HAR 请求的完整结构化详情。
/// </summary>
public sealed class HarRequestDetail
{
    /// <summary>
    /// 请求在 HAR entries 中的零基索引。
    /// </summary>
    public int Index { get; set; }

    /// <summary>
    /// 请求所属页面引用。
    /// </summary>
    public string? PageRef { get; set; }

    /// <summary>
    /// 请求开始时间。
    /// </summary>
    public DateTimeOffset StartedDateTime { get; set; }

    /// <summary>
    /// 请求总耗时，单位为毫秒。
    /// </summary>
    public double TimeMilliseconds { get; set; }

    /// <summary>
    /// HAR 原始请求对象。
    /// </summary>
    public HarRequest Request { get; set; } = new();

    /// <summary>
    /// HAR 原始响应对象。
    /// </summary>
    public HarResponse Response { get; set; } = new();

    /// <summary>
    /// 缓存信息。
    /// </summary>
    public HarCache? Cache { get; set; }

    /// <summary>
    /// 请求各阶段耗时信息。
    /// </summary>
    public HarTimings Timings { get; set; } = new();

    /// <summary>
    /// 服务器 IP 地址。
    /// </summary>
    public string? ServerIPAddress { get; set; }

    /// <summary>
    /// 连接标识。
    /// </summary>
    public string? Connection { get; set; }

    /// <summary>
    /// HAR Entry 注释。
    /// </summary>
    public string? Comment { get; set; }
}
