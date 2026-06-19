namespace Bunsei.Mcp.Har.Models;

/// <summary>
/// MCP Server 根端点返回的服务信息。
/// </summary>
public sealed class ServerInfoResult
{
    /// <summary>
    /// 服务名称。
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// MCP 传输方式。
    /// </summary>
    public string Transport { get; set; } = string.Empty;

    /// <summary>
    /// Streamable HTTP MCP 端点。
    /// </summary>
    public string Endpoint { get; set; } = string.Empty;

    /// <summary>
    /// 旧版 SSE 端点。
    /// </summary>
    public string LegacySseEndpoint { get; set; } = string.Empty;

    /// <summary>
    /// 旧版 SSE 消息端点。
    /// </summary>
    public string LegacyMessageEndpoint { get; set; } = string.Empty;
}
