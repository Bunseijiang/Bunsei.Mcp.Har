using Bunsei.Mcp.Har.Models;

namespace Bunsei.Mcp.Har.Services;

/// <summary>
/// 提供 HAR 文件目录扫描能力。
/// </summary>
public interface IHarCatalogService
{
    /// <summary>
    /// 列出指定目录或默认配置目录中的 HAR 文件。
    /// </summary>
    HarFileListResult ListHarFiles(string? scanDirectory = null, int offset = 0, int limit = 50);
}
