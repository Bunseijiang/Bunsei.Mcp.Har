namespace Bunsei.Mcp.Har.Models;

/// <summary>
/// HAR 文件目录扫描结果。
/// </summary>
public sealed class HarFileListResult
{
    /// <summary>
    /// 实际扫描目录的完整路径。
    /// </summary>
    public string ScanDirectory { get; set; } = string.Empty;

    /// <summary>
    /// 使用的文件搜索模式。
    /// </summary>
    public string SearchPattern { get; set; } = string.Empty;

    /// <summary>
    /// 是否递归扫描子目录。
    /// </summary>
    public bool Recursive { get; set; }

    /// <summary>
    /// 分页偏移量。
    /// </summary>
    public int Offset { get; set; }

    /// <summary>
    /// 分页返回数量上限。
    /// </summary>
    public int Limit { get; set; }

    /// <summary>
    /// 扫描命中的文件总数。
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// 当前页的 HAR 文件列表。
    /// </summary>
    public IReadOnlyList<HarFileItem> Files { get; set; } = [];
}
