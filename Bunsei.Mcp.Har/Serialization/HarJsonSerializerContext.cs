using Bunsei.Mcp.Har.Models;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Bunsei.Mcp.Har.Serialization;

/// <summary>
/// Native AOT 环境下使用的 System.Text.Json 源生成上下文。
/// </summary>
[JsonSourceGenerationOptions(
    PropertyNameCaseInsensitive = true,
    ReadCommentHandling = JsonCommentHandling.Skip,
    AllowTrailingCommas = true)]
[JsonSerializable(typeof(HarArchive))]
[JsonSerializable(typeof(HarAnalysisResult))]
[JsonSerializable(typeof(HarContentChunk))]
[JsonSerializable(typeof(HarContentFormat))]
[JsonSerializable(typeof(HarFileItem))]
[JsonSerializable(typeof(HarFileListResult))]
[JsonSerializable(typeof(HarIssue))]
[JsonSerializable(typeof(HarLoadResult))]
[JsonSerializable(typeof(HarRequestDetail))]
[JsonSerializable(typeof(HarRequestSummary))]
[JsonSerializable(typeof(HarSearchOptions))]
[JsonSerializable(typeof(HarTimelineItem))]
[JsonSerializable(typeof(ServerInfoResult))]
[JsonSerializable(typeof(IReadOnlyList<HarFileItem>))]
[JsonSerializable(typeof(IReadOnlyList<HarIssue>))]
[JsonSerializable(typeof(IReadOnlyList<HarRequestSummary>))]
[JsonSerializable(typeof(IReadOnlyList<HarTimelineItem>))]
[JsonSerializable(typeof(Dictionary<int, int>))]
[JsonSerializable(typeof(Dictionary<string, int>))]
internal sealed partial class HarJsonSerializerContext : JsonSerializerContext
{
}
