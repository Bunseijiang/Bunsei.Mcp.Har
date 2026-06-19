namespace Bunsei.Mcp.Har.Services;

/// <summary>
/// HAR 文件目录扫描配置。
/// </summary>
public sealed class HarCatalogOptions
{
    /// <summary>
    /// 默认扫描目录。
    /// </summary>
    public string ScanDirectory { get; set; } = ".";

    /// <summary>
    /// 文件搜索模式，例如 *.har。
    /// </summary>
    public string SearchPattern { get; set; } = "*.har";

    /// <summary>
    /// 是否递归扫描子目录。
    /// </summary>
    public bool Recursive { get; set; }

    /// <summary>
    /// 单次扫描最多纳入排序和分页的文件数量。
    /// </summary>
    public int MaxFiles { get; set; } = 500;
}
