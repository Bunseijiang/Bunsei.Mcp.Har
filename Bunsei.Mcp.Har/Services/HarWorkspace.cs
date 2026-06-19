using Bunsei.Mcp.Har.Models;

namespace Bunsei.Mcp.Har.Services;

/// <summary>
/// 保存当前 MCP 会话中已加载的 HAR 文件及其分析上下文。
/// </summary>
public sealed class HarWorkspace(IHarAnalysisService harAnalysisService) : IHarWorkspace
{
    public HarArchive? CurrentArchive { get; private set; }

    public string? CurrentFilePath { get; private set; }

    public HarLoadResult? CurrentLoadResult { get; private set; }

    public async Task<HarLoadResult> LoadAsync(string filePath, CancellationToken cancellationToken = default)
    {
        var (archive, result) = await harAnalysisService.LoadWithResultAsync(filePath, cancellationToken);
        CurrentArchive = archive;
        CurrentFilePath = filePath;
        CurrentLoadResult = result;
        return result;
    }

    public HarArchive GetRequiredArchive()
    {
        return CurrentArchive ?? throw new InvalidOperationException("尚未加载 HAR 文件，请先调用 load_har_file。");
    }
}
