namespace XPE.ArquiteturaSoftware.DesafioFinal.Application.Responses;

public sealed record HealthCheckResponse
(
    string Status,
    string Dependency,
    int? Result = null,
    long? RoundtripMs = null,
    string? Error = null
);