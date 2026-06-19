using Bunsei.Mcp.Har.Models;
using Microsoft.Extensions.Options;

namespace Bunsei.Mcp.Har.Services;

/// <summary>
/// 基于配置目录扫描 HAR 文件，并按修改时间倒序返回分页结果。
/// </summary>
public sealed class HarCatalogService(IOptions<HarCatalogOptions> options) : IHarCatalogService
{
    public HarFileListResult ListHarFiles(string? scanDirectory = null, int offset = 0, int limit = 50)
    {
        if (offset < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(offset), offset, "偏移量不能小于 0。");
        }

        if (limit < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(limit), limit, "数量不能小于 0。");
        }

        var catalogOptions = options.Value;
        var directory = string.IsNullOrWhiteSpace(scanDirectory) ? catalogOptions.ScanDirectory : scanDirectory;
        var fullDirectory = Path.GetFullPath(directory);

        if (!Directory.Exists(fullDirectory))
        {
            throw new DirectoryNotFoundException($"HAR 扫描目录不存在：{fullDirectory}");
        }

        var searchOption = catalogOptions.Recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
        var files = Directory
            .EnumerateFiles(fullDirectory, catalogOptions.SearchPattern, searchOption)
            .Select(path => new FileInfo(path))
            .OrderByDescending(file => file.LastWriteTimeUtc)
            .ThenBy(file => file.FullName, StringComparer.OrdinalIgnoreCase)
            .Take(catalogOptions.MaxFiles)
            .Select((file, index) => new HarFileItem
            {
                Index = index,
                FileName = file.Name,
                FullPath = file.FullName,
                Directory = file.DirectoryName ?? string.Empty,
                SizeBytes = file.Length,
                LastModified = file.LastWriteTimeUtc
            })
            .ToList();

        return new HarFileListResult
        {
            ScanDirectory = fullDirectory,
            SearchPattern = catalogOptions.SearchPattern,
            Recursive = catalogOptions.Recursive,
            Offset = offset,
            Limit = limit,
            TotalCount = files.Count,
            Files = files.Skip(offset).Take(limit).ToList()
        };
    }
}
