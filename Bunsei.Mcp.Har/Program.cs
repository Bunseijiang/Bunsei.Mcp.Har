using Bunsei.Mcp.Har.Serialization;
using Bunsei.Mcp.Har.Services;
using Bunsei.Mcp.Har.Tools;
using Bunsei.Mcp.Har.Models;
using ModelContextProtocol;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("Configs.json", optional: false, reloadOnChange: true);

var mcpJsonOptions = new JsonSerializerOptions(McpJsonUtilities.DefaultOptions);
mcpJsonOptions.TypeInfoResolverChain.Insert(0, HarJsonSerializerContext.Default);

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.TypeInfoResolverChain.Insert(0, HarJsonSerializerContext.Default);
});

builder.Services
    .Configure<HarCatalogOptions>(builder.Configuration.GetSection("Har"))
    .AddSingleton<IHarCatalogService, HarCatalogService>()
    .AddSingleton<IHarAnalysisService, HarAnalysisService>()
    .AddSingleton<IHarWorkspace, HarWorkspace>()
    .AddCors(item =>
    {
        item.AddDefaultPolicy(configure =>
        {
            configure.AllowAnyHeader().AllowAnyOrigin().AllowAnyMethod();
        });
    })
    .AddMcpServer()
    .WithHttpTransport(options =>
    {
        options.EnableLegacySse = true;
    })
    .WithTools<HarMcpTools>(mcpJsonOptions);


var app = builder.Build();

app.UseCors();

app.MapGet("/", () => Results.Ok(new ServerInfoResult
{
    Name = "HAR MCP Server",
    Transport = "Streamable HTTP",
    Endpoint = "/mcp",
    LegacySseEndpoint = "/sse",
    LegacyMessageEndpoint = "/message"
}));

app.MapMcp("/mcp");

await app.RunAsync();
