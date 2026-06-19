using Bunsei.Mcp.Har.Models;

namespace Bunsei.Mcp.Har.Services;

/// <summary>
/// 提供 HAR 文件加载、检索、统计分析和内容读取能力。
/// </summary>
public interface IHarAnalysisService
{
    /// <summary>
    /// 从磁盘读取并反序列化 HAR 文件。
    /// </summary>
    Task<HarArchive> LoadAsync(string filePath, CancellationToken cancellationToken = default);

    /// <summary>
    /// 加载 HAR 文件，并同时返回文件基础信息。
    /// </summary>
    Task<(HarArchive Archive, HarLoadResult Result)> LoadWithResultAsync(string filePath, CancellationToken cancellationToken = default);

    /// <summary>
    /// 对 HAR 档案进行汇总分析，包括状态码、Host、MIME 类型分布和问题检测。
    /// </summary>
    HarAnalysisResult Analyze(HarArchive archive, HarAnalysisOptions? options = null);

    /// <summary>
    /// 基于已加载的 HAR 档案生成加载结果信息。
    /// </summary>
    HarLoadResult GetLoadResult(HarArchive archive, string filePath = "");

    /// <summary>
    /// 按原始顺序分页列出 HAR 请求摘要。
    /// </summary>
    IReadOnlyList<HarRequestSummary> ListRequests(HarArchive archive, int offset = 0, int limit = 50, HarAnalysisOptions? options = null);

    /// <summary>
    /// 根据请求索引获取完整 HAR Entry 详情。
    /// </summary>
    HarRequestDetail GetRequestDetail(HarArchive archive, int requestIndex);

    /// <summary>
    /// 按关键字、HTTP 方法、Host、状态码和 MIME 类型搜索请求。
    /// </summary>
    IReadOnlyList<HarRequestSummary> SearchRequests(HarArchive archive, HarSearchOptions options, HarAnalysisOptions? analysisOptions = null);

    /// <summary>
    /// 获取请求时间线，用于观察请求开始时间和持续时间。
    /// </summary>
    IReadOnlyList<HarTimelineItem> GetTimeline(HarArchive archive, int offset = 0, int limit = 100, HarAnalysisOptions? options = null);

    /// <summary>
    /// 分块读取指定请求的请求体。
    /// </summary>
    HarContentChunk GetRequestBodyChunk(HarArchive archive, int requestIndex, int offset, int length, HarContentFormat format = HarContentFormat.Text, string textEncodingName = "utf-8");

    /// <summary>
    /// 分块读取指定请求的响应体。
    /// </summary>
    HarContentChunk GetResponseBodyChunk(HarArchive archive, int requestIndex, int offset, int length, HarContentFormat format = HarContentFormat.Text, string textEncodingName = "utf-8");

    /// <summary>
    /// 获取失败请求列表，状态码为 0 或大于等于 400 时视为失败。
    /// </summary>
    IReadOnlyList<HarRequestSummary> GetFailedRequests(HarArchive archive, HarAnalysisOptions? options = null);

    /// <summary>
    /// 获取超过指定耗时阈值的慢请求列表。
    /// </summary>
    IReadOnlyList<HarRequestSummary> GetSlowRequests(HarArchive archive, double? minimumMilliseconds = null, HarAnalysisOptions? options = null);

    /// <summary>
    /// 检测 HAR 中的服务端错误、客户端错误、慢请求、大响应、慢 DNS、慢 SSL 和重复请求等问题。
    /// </summary>
    IReadOnlyList<HarIssue> DetectIssues(HarArchive archive, HarAnalysisOptions? options = null);
}
