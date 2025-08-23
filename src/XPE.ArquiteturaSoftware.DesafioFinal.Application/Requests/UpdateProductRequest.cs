namespace XPE.ArquiteturaSoftware.DesafioFinal.Application.Requests;

public sealed record UpdateProductRequest
(
    string Name,
    string Description,
    decimal Price,
    bool Active
);