using XPE.ArquiteturaSoftware.DesafioFinal.Application.Requests;
using XPE.ArquiteturaSoftware.DesafioFinal.Application.Responses;
using XPE.ArquiteturaSoftware.DesafioFinal.Domain.Models;

namespace XPE.ArquiteturaSoftware.DesafioFinal.Application.Mappers;

public static class ProductMapper
{
    public static Product MapToModel(this CreateProductRequest request)
        => new(request.Name, request.Description, request.Price);

    public static Product MapToModel(this UpdateProductRequest request)
    {
        var model = new Product(request.Name, request.Description, request.Price);
        model.SetActive(request.Active);
        return model;
    }

    public static ProductResponse ToResponse(this Product p)
        => new(
            Id: p.Id,
            Name: p.Name,
            Description: p.Description,
            Price: p.Price,
            Active: p.Active,
            CreatedAt: p.CreatedAt,
            UpdatedAt: p.UpdatedAt
        );

    public static IEnumerable<ProductResponse> ToResponse(this IEnumerable<Product> items)
        => items.Select(p => p.ToResponse());
}