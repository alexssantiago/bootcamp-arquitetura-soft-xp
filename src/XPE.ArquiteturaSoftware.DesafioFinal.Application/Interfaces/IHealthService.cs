using CSharpFunctionalExtensions;
using XPE.ArquiteturaSoftware.DesafioFinal.Application.Responses;

namespace XPE.ArquiteturaSoftware.DesafioFinal.Application.Interfaces;

public interface IHealthService
{
    Task<Result<HealthCheckResponse>> CheckDatabaseAsync(CancellationToken ct);
    Task<Result<HealthCheckResponse>> CheckRedisAsync(CancellationToken ct);
}