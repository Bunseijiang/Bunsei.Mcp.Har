namespace Bunsei.Mcp.Har.Models;

/// <summary>
/// 扫描到的单个 HAR 文件信息。
/// </summary>
public sealed class HarFileItem
{
    /// <summary>
    /// 文件在扫描结果中的零基索引。
    /// </summary>
    public int Index { get; set; }

    /// <summary>
    /// 文件名。
    /// </summary>
    public string FileName { get; set; } = string.Empty;

    /// <summary>
    /// 文件完整路径。
    /// </summary>
    public string FullPath { get; set; } = string.Empty;

    /// <summary>
    /// 文件所在目录。
    /// </summary>
    public string Directory { get; set; } = string.Empty;

    /// <summary>
    /// 文件大小，单位为字节。
    /// </summary>
    public long SizeBytes { get; set; }

    /// <summary>
    /// 文件最后修改时间，使用 UTC 时间。
    /// </summary>
    public DateTimeOffset LastModified { get; set; }
}
