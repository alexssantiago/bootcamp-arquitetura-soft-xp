using Microsoft.Extensions.Logging;
using XPE.ArquiteturaSoftware.DesafioFinal.Application.Interfaces;
using XPE.ArquiteturaSoftware.DesafioFinal.Application.Mappers;
using XPE.ArquiteturaSoftware.DesafioFinal.Application.Requests;
using XPE.ArquiteturaSoftware.DesafioFinal.Application.Responses;
using XPE.ArquiteturaSoftware.DesafioFinal.Infra.Data.Interfaces;

namespace XPE.ArquiteturaSoftware.DesafioFinal.Application.Services;

public sealed class ProductService(ILogger<ProductService> logger,
                                   IProductRepository repository) : IProductService
{
    public async Task<CreateProductResponse> CreateAsync(CreateProductRequest request)
    {
        try
        {
            var product = request.MapToModel();
            var productId = await repository.CreateAsync(product);

            return new CreateProductResponse(product.Id);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "[{ServiceName}][{MethodName}] Error: {ErrorMessage}",
                nameof(ProductService), nameof(CreateAsync), ex.Message);
            throw;
        }
    }
}