using Dapper;
using FluentResults;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System.Data;
using System.Diagnostics;
using XPE.ArquiteturaSoftware.DesafioFinal.Application.Interfaces;
using XPE.ArquiteturaSoftware.DesafioFinal.Application.Responses;

namespace XPE.ArquiteturaSoftware.DesafioFinal.Application.Services;

public sealed class HealthService(ILogger<HealthService> logger,
                                  IDbConnection connection,
                                  IDistributedCache cache) : IHealthService
{
    public async Task<Result<HealthCheckResponse>> CheckDatabaseAsync(CancellationToken ct)
    {
        try
        {
            var result = await connection.ExecuteScalarAsync<int>(
                new CommandDefinition("SELECT 1", cancellationToken: ct));

            return Result.Ok(new HealthCheckResponse(
                Status: "healthy",
                Dependency: "mysql",
                Result: result
            ));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "[{Service}][{Method}] Database unavailable: {Message}",
                nameof(HealthService), nameof(CheckDatabaseAsync), ex.Message);

            var response = new HealthCheckResponse(
                Status: "unhealthy",
                Dependency: "mysql",
                Error: ex.Message
            );

            var error = new Error("Database unavailable")
                .CausedBy(ex)
                .WithMetadata("response", response);

            return Result.Fail<HealthCheckResponse>(error);
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
                return Result.Ok(new HealthCheckResponse(
                    Status: "healthy",
                    Dependency: "redis",
                    RoundtripMs: sw.ElapsedMilliseconds
                ));
            }

            var mismatch = new HealthCheckResponse(
                Status: "unhealthy",
                Dependency: "redis",
                Error: "Round-trip value mismatch."
            );

            return Result.Fail<HealthCheckResponse>(
                new Error("Round-trip value mismatch.")
                    .WithMetadata("response", mismatch));
        }
        catch (Exception ex)
        {
            sw.Stop();
            logger.LogError(ex, "[{Service}][{Method}] Redis unavailable: {Message}",
                nameof(HealthService), nameof(CheckRedisAsync), ex.Message);

            var response = new HealthCheckResponse(
                Status: "unhealthy",
                Dependency: "redis",
                Error: ex.Message
            );

            return Result.Fail<HealthCheckResponse>(
                new Error("Redis unavailable")
                    .CausedBy(ex)
                    .WithMetadata("response", response));
        }
    }
}