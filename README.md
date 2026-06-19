这是一个基于 ASP.NET Core 和 MCP C# SDK 构建的 HAR（HTTP Archive）分析 MCP Server。它可以加载、检索和分析 `.har` 文件，并通过 MCP 工具把网络请求摘要、时间线、请求/响应正文分块等能力提供给支持 MCP 的客户端（如 Visual Studio、VS Code 或其他 AI 客户端）。

## 功能特性

- 扫描指定目录下的 HAR 文件。
- 加载 HAR 文件到当前会话工作区。
- 获取当前 HAR 文件的基础信息和统计摘要。
- 按原始顺序分页列出请求。
- 按关键字、HTTP 方法、Host、状态码范围、MIME 类型搜索请求。
- 获取单个请求的完整结构化详情。
- 生成请求时间线，辅助定位慢请求和异常请求。
- 分块读取请求体和响应体，支持 `Text`、`Base64`、`Hex` 输出格式。

## 技术栈

- .NET 10
- ASP.NET Core
- ModelContextProtocol.AspNetCore 1.2.0
- Streamable HTTP MCP Transport（同时启用 Legacy SSE）

## 项目结构

```text
Bunsei.Mcp.Har/
├─ Mcp/                 # MCP 工具定义
├─ Models/              # 工具返回模型与查询模型
├─ Services/            # HAR 加载、扫描与分析服务
├─ Configs.json         # HAR 扫描与 Kestrel 配置
├─ HarModels.cs         # HAR 数据结构模型
└─ Program.cs           # 应用入口与 MCP Server 注册
```

## 配置说明

默认配置位于 `Bunsei.Mcp.Har/Configs.json`：

```json
{
  "Har": {
    "ScanDirectory": ".",
    "SearchPattern": "*.har",
    "Recursive": false,
    "MaxFiles": 500
  },
  "Kestrel": {
    "Endpoints": {
      "Http": {
        "Url": "http://localhost:3001"
      }
    }
  }
}
```

### Har 配置项

| 配置项 | 说明 | 默认值 |
| --- | --- | --- |
| `ScanDirectory` | 默认扫描目录，`ListAvailableHarFiles` 未传入目录时使用 | `.` |
| `SearchPattern` | 文件搜索模式 | `*.har` |
| `Recursive` | 是否递归扫描子目录 | `false` |
| `MaxFiles` | 最大返回文件数量 | `500` |

### 服务端点

运行后根路径会返回服务信息，主要端点如下：

| 端点 | 说明 |
| --- | --- |
| `/` | 服务基础信息 |
| `/mcp` | MCP Streamable HTTP 端点 |
| `/sse` | Legacy SSE 端点 |
| `/message` | Legacy SSE Message 端点 |

> 注意：`Configs.json` 中配置的 Kestrel 地址为 `http://localhost:3001`。开发配置 `launchSettings.json` 中还包含 `http://localhost:6075` 和 `https://localhost:5166`，实际监听地址取决于启动方式。

## 本地运行

在仓库根目录执行：

```powershell
dotnet run --project .\Bunsei.Mcp.Har\Bunsei.Mcp.Har.csproj
```

启动后可访问：

```text
http://localhost:3001/
```

如果通过 Visual Studio 的 `http` 或 `https` Profile 启动，请使用 `launchSettings.json` 中对应的地址。

## MCP 客户端配置示例

以 HTTP MCP Server 方式连接：

```json
{
  "servers": {
    "Bunsei.Mcp.Har": {
      "type": "http",
      "url": "http://localhost:3001/mcp"
    }
  }
}
```

如果使用 Visual Studio/VS Code 的开发 Profile 启动，也可以按实际端口调整为：

```json
{
  "servers": {
    "Bunsei.Mcp.Har": {
      "type": "http",
      "url": "http://localhost:6075/mcp"
    }
  }
}
```

## 可用 MCP 工具

| 工具 | 说明 |
| --- | --- |
| `ListAvailableHarFiles` | 列出配置目录或指定目录中的 HAR 文件 |
| `LoadHarFileAsync` | 加载指定 HAR 文件到当前工作区 |
| `GetCurrentHarInfo` | 获取当前已加载 HAR 文件的基础信息 |
| `GetHarSummary` | 获取 HAR 统计摘要、分布、慢请求和失败请求 |
| `ListHarRequests` | 按原始顺序分页列出请求摘要 |
| `GetHarRequestFullDetail` | 按请求索引获取完整 HAR Entry 详情 |
| `SearchHarRequests` | 按关键字、方法、Host、状态码或 MIME 类型搜索请求 |
| `GetHarTimeline` | 获取请求时间线 |
| `GetHarRequestBodyChunk` | 分块读取请求体 |
| `GetHarResponseBodyChunk` | 分块读取响应体 |

## 使用示例

在 MCP 客户端中可以按以下流程使用：

1. “列出当前目录下可用的 HAR 文件。”
2. “加载这个 HAR 文件：`C:\path\to\example.har`。”
3. “总结当前 HAR 文件，列出最慢的 20 个请求。”
4. “搜索状态码为 500 的请求。”
5. “读取第 12 个请求的响应体前 4096 字节，按 Text 输出。”

## 发布

项目已配置为自包含、单文件、Native AOT 发布。默认支持以下 Runtime Identifier：

- `win-x64`
- `win-arm64`
- `osx-arm64`
- `linux-x64`
- `linux-arm64`
- `linux-musl-x64`

发布示例：

```powershell
dotnet publish .\Bunsei.Mcp.Har\Bunsei.Mcp.Har.csproj -c Release -r win-x64
```

如需支持其他平台，请在项目文件的 `<RuntimeIdentifiers>` 中添加对应 RID。

## 常见问题

### 尚未加载 HAR 文件

除文件扫描和加载工具外，大多数分析工具都依赖当前工作区中已加载的 HAR 文件。如果出现“尚未加载 HAR 文件”的错误，请先调用加载 HAR 文件工具。

### HTTPS 本地证书问题

部分客户端连接本地 HTTPS 开发证书时可能失败。开发阶段建议优先使用 HTTP 地址，例如：

```text
http://localhost:3001/mcp
```

## 相关链接

- [Model Context Protocol 官方文档](https://modelcontextprotocol.io/)
- [MCP 协议规范](https://modelcontextprotocol.io/specification/)
- [MCP C# SDK](https://modelcontextprotocol.github.io/csharp-sdk)
- [ModelContextProtocol.AspNetCore NuGet 包](https://www.nuget.org/packages/ModelContextProtocol.AspNetCore)
