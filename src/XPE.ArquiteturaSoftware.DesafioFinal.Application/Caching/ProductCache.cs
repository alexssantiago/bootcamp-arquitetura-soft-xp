using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace XPE.ArquiteturaSoftware.DesafioFinal.Application.Caching;

public static class ProductCache
{
    private const string VersionKey = "products:version";
    private static readonly JsonSerializerOptions JsonOpts = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

    public static async Task<string> GetVersionAsync(IDistributedCache cache, CancellationToken ct)
    {
        var v = await cache.GetStringAsync(VersionKey, ct);
        if (!string.IsNullOrEmpty(v)) return v!;
        var token = Guid.NewGuid().ToString("N");
        await cache.SetStringAsync(VersionKey, token, new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(12) }, ct);
        return token;
    }

    public static Task BumpVersionAsync(IDistributedCache cache, CancellationToken ct)
        => cache.SetStringAsync(VersionKey, Guid.NewGuid().ToString("N"),
            new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(12) }, ct);

    public static string KeyProductById(string v, int id) => $"products:v:{v}:id:{id}";
    public static string KeyAll(string v) => $"products:v:{v}:all";
    public static string KeyCount(string v) => $"products:v:{v}:count";
    public static string KeyByName(string v, string name) => $"products:v:{v}:name:{Normalize(name)}";

    private static string Normalize(string s) => s.Trim().ToLowerInvariant();

    public static async Task<T?> GetAsync<T>(IDistributedCache cache, string key, CancellationToken ct)
    {
        var bytes = await cache.GetAsync(key, ct);
        if (bytes is null || bytes.Length == 0) return default;
        return JsonSerializer.Deserialize<T>(bytes, JsonOpts);
    }

    public static Task SetAsync<T>(IDistributedCache cache, string key, T value, TimeSpan ttl, CancellationToken ct)
    {
        var bytes = JsonSerializer.SerializeToUtf8Bytes(value, JsonOpts);
        return cache.SetAsync(key, bytes, new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = ttl }, ct);
    }

    public static Task RemoveAsync(IDistributedCache cache, string key, CancellationToken ct)
        => cache.RemoveAsync(key, ct);
}