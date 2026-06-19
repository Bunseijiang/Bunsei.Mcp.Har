namespace Bunsei.Mcp.Har.Models;

/// <summary>
/// 请求体或响应体的分块读取结果。
/// </summary>
public sealed class HarContentChunk
{
    /// <summary>
    /// 请求在 HAR entries 中的零基索引。
    /// </summary>
    public int RequestIndex { get; set; }

    /// <summary>
    /// 内容来源，通常为 request 或 response。
    /// </summary>
    public string Source { get; set; } = string.Empty;

    /// <summary>
    /// 内容输出格式。
    /// </summary>
    public HarContentFormat Format { get; set; }

    /// <summary>
    /// 文本输出时使用的编码名称。
    /// </summary>
    public string? TextEncodingName { get; set; }

    /// <summary>
    /// 本次读取的字节偏移量。
    /// </summary>
    public int Offset { get; set; }

    /// <summary>
    /// 本次实际返回的字节长度。
    /// </summary>
    public int Length { get; set; }

    /// <summary>
    /// 完整内容的总字节长度。
    /// </summary>
    public int TotalLength { get; set; }

    /// <summary>
    /// 是否已经读取到内容末尾。
    /// </summary>
    public bool EndOfContent { get; set; }

    /// <summary>
    /// 按指定格式编码后的内容片段。
    /// </summary>
    public string Content { get; set; } = string.Empty;
}
