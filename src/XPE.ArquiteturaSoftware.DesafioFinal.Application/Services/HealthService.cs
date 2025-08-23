using CSharpFunctionalExtensions;
using Dapper;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System.Data;
using System.Diagnostics;
using XPE.ArquiteturaSoftware.DesafioFinal.Application.Interfaces;
using XPE.ArquiteturaSoftware.DesafioFinal.Application.Responses;

namespace XPE.ArquiteturaSoftware.DesafioFinal.Application.Services;

public sealed class HealthService(
    ILogger<HealthService> logger,
    IDbConnection connection,
    IDistributedCache cache) : IHealthService
{
    public async Task<Result<HealthCheckResponse>> CheckDatabaseAsync(CancellationToken ct)
    {
        try
        {
            var result = await connection.ExecuteScalarAsync<int>(
                new CommandDefinition("SELECT 1", cancellationToken: ct));

            return Result.Success(new HealthCheckResponse(
                Status: "healthy",
                Dependency: "mysql",
                Result: result
            ));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "[{Service}][{Method}] Database unavailable: {Message}",
                nameof(HealthService), nameof(CheckDatabaseAsync), ex.Message);

            return Result.Failure<HealthCheckResponse>(ex.Message);
        }
    }

    public async Task<Result<HealthCheckResponse>> CheckRedisAsync(CancellationToken ct)
    {
        var sw = Stopwatch.StartNew();
        const string key = "health:redis:ping";
        var payload = Guid.NewGuid().ToString("N");

        try
        {
            var opts = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(5)
            };

            await cache.SetStringAsync(key, payload, opts, ct);
            var roundtrip = await cache.GetStringAsync(key, ct);
            sw.Stop();

            if (roundtrip == payload)
            {
                return Result.Success(new HealthCheckResponse(
                    Status: "healthy",
                    Dependency: "redis",
                    RoundtripMs: sw.ElapsedMilliseconds
                ));
            }

            return Result.Failure<HealthCheckResponse>("Round-trip value mismatch.");
        }
        catch (Exception ex)
        {
            sw.Stop();
            logger.LogError(ex, "[{Service}][{Method}] Redis unavailable: {Message}",
                nameof(HealthService), nameof(CheckRedisAsync), ex.Message);

            return Result.Failure<HealthCheckResponse>(ex.Message);
        }
    }
}
