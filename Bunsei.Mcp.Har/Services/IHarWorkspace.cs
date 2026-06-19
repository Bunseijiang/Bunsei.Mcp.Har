using Bunsei.Mcp.Har.Models;

namespace Bunsei.Mcp.Har.Services;

/// <summary>
/// 表示 MCP Server 当前会话中的 HAR 工作区状态。
/// </summary>
public interface IHarWorkspace
{
    /// <summary>
    /// 当前已加载的 HAR 档案；未加载时为 null。
    /// </summary>
    HarArchive? CurrentArchive { get; }

    /// <summary>
    /// 当前已加载 HAR 文件的路径。
    /// </summary>
    string? CurrentFilePath { get; }

    /// <summary>
    /// 最近一次加载 HAR 文件得到的基础信息。
    /// </summary>
    HarLoadResult? CurrentLoadResult { get; }

    /// <summary>
    /// 加载指定 HAR 文件并更新当前工作区状态。
    /// </summary>
    Task<HarLoadResult> LoadAsync(string filePath, CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取当前已加载的 HAR 档案；未加载时抛出异常。
    /// </summary>
    HarArchive GetRequiredArchive();
}
