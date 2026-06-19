namespace Bunsei.Mcp.Har.Models;

/// <summary>
/// HAR 文件加载后的基础信息。
/// </summary>
public sealed class HarLoadResult
{
    /// <summary>
    /// 是否已成功加载。
    /// </summary>
    public bool Loaded { get; set; }

    /// <summary>
    /// 已加载的 HAR 文件路径。
    /// </summary>
    public string FilePath { get; set; } = string.Empty;

    /// <summary>
    /// HAR 中的请求数量。
    /// </summary>
    public int RequestCount { get; set; }

    /// <summary>
    /// HAR 中的页面数量。
    /// </summary>
    public int PageCount { get; set; }

    /// <summary>
    /// 最早请求的开始时间。
    /// </summary>
    public DateTimeOffset? StartedDateTime { get; set; }

    /// <summary>
    /// HAR 中出现过的 Host 列表。
    /// </summary>
    public IReadOnlyList<string> Hosts { get; set; } = [];
}
