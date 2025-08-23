namespace XPE.ArquiteturaSoftware.DesafioFinal.Application.Responses;

public sealed record ProductResponse
(
    int Id,
    string Name,
    string Description,
    decimal Price,
    bool Active,
    DateTime CreatedAt,
    DateTime? UpdatedAt
);
