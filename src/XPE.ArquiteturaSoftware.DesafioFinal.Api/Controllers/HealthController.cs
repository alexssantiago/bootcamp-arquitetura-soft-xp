using Microsoft.AspNetCore.Mvc;
using XPE.ArquiteturaSoftware.DesafioFinal.Api.Extensions;
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
    /// <summary>Checks connectivity with the MySQL database.</summary>
    [HttpGet("db")]
    [ProducesResponseType(typeof(HealthCheckResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(HealthCheckResponse), StatusCodes.Status503ServiceUnavailable)]
    public async Task<IActionResult> CheckDatabaseAsync(CancellationToken ct)
        => (await service.CheckDatabaseAsync(ct)).ToActionResult(this);

    /// <summary>Checks connectivity with the Redis cache.</summary>
    [HttpGet("redis")]
    [ProducesResponseType(typeof(HealthCheckResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(HealthCheckResponse), StatusCodes.Status503ServiceUnavailable)]
    public async Task<IActionResult> CheckRedisAsync(CancellationToken ct)
        => (await service.CheckRedisAsync(ct)).ToActionResult(this);
}