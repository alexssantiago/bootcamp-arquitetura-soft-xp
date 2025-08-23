using FluentResults;
using Microsoft.AspNetCore.Mvc;
using XPE.ArquiteturaSoftware.DesafioFinal.Application.Interfaces;
using XPE.ArquiteturaSoftware.DesafioFinal.Application.Responses;

namespace XPE.ArquiteturaSoftware.DesafioFinal.Api.Controllers;

/// <summary>
/// Health check endpoints to validate external dependencies (e.g., database).
/// </summary>
[ApiController]
[Route("api/[controller]")]
public sealed class HealthController(IHealthService service) : ControllerBase
{
    /// <summary>
    /// Checks connectivity with the MySQL database.
    /// </summary>
    [HttpGet("db")]
    [ProducesResponseType(typeof(HealthCheckResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(HealthCheckResponse), StatusCodes.Status503ServiceUnavailable)]
    public async Task<IActionResult> CheckDatabaseAsync(CancellationToken ct)
    {
        Result<HealthCheckResponse> result = await service.CheckDatabaseAsync(ct);
        return result.IsSuccess
            ? Ok(result.Value)
            : StatusCode(StatusCodes.Status503ServiceUnavailable, result.Value);
    }

    /// <summary>
    /// Checks connectivity with the Redis cache.
    /// </summary>
    [HttpGet("redis")]
    [ProducesResponseType(typeof(HealthCheckResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(HealthCheckResponse), StatusCodes.Status503ServiceUnavailable)]
    public async Task<IActionResult> CheckRedisAsync(CancellationToken ct)
    {
        Result<HealthCheckResponse> result = await service.CheckRedisAsync(ct);
        return result.IsSuccess
            ? Ok(result.Value)
            : StatusCode(StatusCodes.Status503ServiceUnavailable, result.Value);
    }
}