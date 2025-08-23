namespace XPE.ArquiteturaSoftware.DesafioFinal.Application.Requests;

public sealed record CreateProductRequest
(
    string Name, 
    string Description, 
    decimal Price
);