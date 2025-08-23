using System.Text;
using System.Text.Json;

namespace XPE.ArquiteturaSoftware.DesafioFinal.Tests.Extensions;

public static class CacheExtensions
{
    private static readonly JsonSerializerOptions JsonOpts = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public static byte[] ToBytes<T>(T value) => JsonSerializer.SerializeToUtf8Bytes(value, JsonOpts);
    public static byte[] Bytes(string s) => Encoding.UTF8.GetBytes(s);
}