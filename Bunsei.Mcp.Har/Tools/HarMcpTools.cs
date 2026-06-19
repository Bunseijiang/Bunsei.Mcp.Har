using Bunsei.Mcp.Har.Models;
using Bunsei.Mcp.Har.Services;
using ModelContextProtocol.Server;
using System.ComponentModel;

namespace Bunsei.Mcp.Har.Tools;

/// <summary>
/// 暴露给 MCP 客户端调用的 HAR 分析工具集合。
/// </summary>
[McpServerToolType]
public sealed class HarMcpTools(IHarWorkspace workspace, IHarAnalysisService harAnalysisService, IHarCatalogService harCatalogService)
{
    [McpServerTool]
    [Description("List available HAR files from the configured scan directory or an explicitly provided directory.")]
    public HarFileListResult ListAvailableHarFiles(
        [Description("Optional directory to scan. If omitted, uses Har:ScanDirectory from Configs.json.")] string? scanDirectory = null,
        [Description("Zero-based result offset.")] int offset = 0,
        [Description("Maximum number of files to return.")] int limit = 50)
    {
        return harCatalogService.ListHarFiles(scanDirectory, offset, limit);
    }

    [McpServerTool]
    [Description("Load a HAR file into the current workspace and return basic file facts.")]
    public Task<HarLoadResult> LoadHarFileAsync(
        [Description("Path to the HAR file.")] string filePath,
        CancellationToken cancellationToken = default)
    {
        return workspace.LoadAsync(filePath, cancellationToken);
    }

    [McpServerTool]
    [Description("Get basic facts for the currently loaded HAR file.")]
    public HarLoadResult GetCurrentHarInfo()
    {
        return workspace.CurrentLoadResult ?? harAnalysisService.GetLoadResult(workspace.GetRequiredArchive(), workspace.CurrentFilePath ?? string.Empty);
    }

    [McpServerTool]
    [Description("Get a factual summary of the currently loaded HAR file, including distributions and top slow or failed requests.")]
    public HarAnalysisResult GetHarSummary(
        [Description("Maximum number of returned request items.")] int maxReturnedRequests = 50,
        [Description("Slow request threshold in milliseconds.")] double slowRequestThresholdMilliseconds = 1000)
    {
        var archive = workspace.GetRequiredArchive();
        return harAnalysisService.Analyze(archive, new HarAnalysisOptions
        {
            MaxReturnedRequests = maxReturnedRequests,
            SlowRequestThresholdMilliseconds = slowRequestThresholdMilliseconds
        });
    }

    [McpServerTool]
    [Description("List HAR requests in original sequence with pagination.")]
    public IReadOnlyList<HarRequestSummary> ListHarRequests(
        [Description("Zero-based offset.")] int offset = 0,
        [Description("Maximum number of requests to return.")] int limit = 50)
    {
        return harAnalysisService.ListRequests(workspace.GetRequiredArchive(), offset, limit);
    }

    [McpServerTool]
    [Description("Get the full structured HAR entry data for a request by zero-based index.")]
    public HarRequestDetail GetHarRequestFullDetail(
        [Description("Zero-based request index.")] int requestIndex)
    {
        return harAnalysisService.GetRequestDetail(workspace.GetRequiredArchive(), requestIndex);
    }

    [McpServerTool]
    [Description("Search HAR requests by keyword, method, host, status range, or MIME type.")]
    public IReadOnlyList<HarRequestSummary> SearchHarRequests(
        [Description("Keyword searched in URL, status text, headers, query string, request body, and response body.")] string? keyword = null,
        [Description("HTTP method filter, such as GET or POST.")] string? method = null,
        [Description("Host filter.")] string? host = null,
        [Description("Exact HTTP status code filter.")] int? status = null,
        [Description("Minimum HTTP status code filter.")] int? minStatus = null,
        [Description("Maximum HTTP status code filter.")] int? maxStatus = null,
        [Description("Response MIME type filter.")] string? mimeType = null,
        [Description("Zero-based result offset.")] int offset = 0,
        [Description("Maximum number of results to return.")] int limit = 50)
    {
        return harAnalysisService.SearchRequests(workspace.GetRequiredArchive(), new HarSearchOptions
        {
            Keyword = keyword,
            Method = method,
            Host = host,
            Status = status,
            MinStatus = minStatus,
            MaxStatus = maxStatus,
            MimeType = mimeType,
            Offset = offset,
            Limit = limit
        });
    }

    [McpServerTool]
    [Description("Get the HAR request timeline in original order with relative start time and duration.")]
    public IReadOnlyList<HarTimelineItem> GetHarTimeline(
        [Description("Zero-based offset.")] int offset = 0,
        [Description("Maximum number of timeline items to return.")] int limit = 100)
    {
        return harAnalysisService.GetTimeline(workspace.GetRequiredArchive(), offset, limit);
    }

    [McpServerTool]
    [Description("Read a chunk from a request body by byte offset and length. Format can be Text, Base64, or Hex.")]
    public HarContentChunk GetHarRequestBodyChunk(
        [Description("Zero-based request index.")] int requestIndex,
        [Description("Byte offset.")] int offset,
        [Description("Byte length.")] int length,
        [Description("Output format: Text, Base64, or Hex.")] HarContentFormat format = HarContentFormat.Text,
        [Description("Text decoding name used only when format is Text.")] string textEncodingName = "utf-8")
    {
        return harAnalysisService.GetRequestBodyChunk(workspace.GetRequiredArchive(), requestIndex, offset, length, format, textEncodingName);
    }

    [McpServerTool]
    [Description("Read a chunk from a response body by byte offset and length. Format can be Text, Base64, or Hex.")]
    public HarContentChunk GetHarResponseBodyChunk(
        [Description("Zero-based request index.")] int requestIndex,
        [Description("Byte offset.")] int offset,
        [Description("Byte length.")] int length,
        [Description("Output format: Text, Base64, or Hex.")] HarContentFormat format = HarContentFormat.Text,
        [Description("Text decoding name used only when format is Text.")] string textEncodingName = "utf-8")
    {
        return harAnalysisService.GetResponseBodyChunk(workspace.GetRequiredArchive(), requestIndex, offset, length, format, textEncodingName);
    }
}
