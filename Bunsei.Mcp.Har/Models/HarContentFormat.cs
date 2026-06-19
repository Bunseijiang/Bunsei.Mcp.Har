namespace Bunsei.Mcp.Har.Models;

/// <summary>
/// 请求体或响应体分块内容的输出格式。
/// </summary>
public enum HarContentFormat
{
    /// <summary>
    /// 按文本方式输出。
    /// </summary>
    Text,

    /// <summary>
    /// 按 Base64 字符串输出。
    /// </summary>
    Base64,

    /// <summary>
    /// 按十六进制字节序列输出。
    /// </summary>
    Hex
}
