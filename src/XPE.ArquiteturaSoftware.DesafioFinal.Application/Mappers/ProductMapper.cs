using XPE.ArquiteturaSoftware.DesafioFinal.Application.Requests;
using XPE.ArquiteturaSoftware.DesafioFinal.Domain.Models;

namespace XPE.ArquiteturaSoftware.DesafioFinal.Application.Mappers;

public static class ProductMapper
{
    public static Product MapToModel(this CreateProductRequest request)
        => new(request.Name, request.Description, request.Price);
}