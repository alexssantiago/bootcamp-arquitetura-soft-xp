using FluentResults;
using Microsoft.Extensions.Logging;
using XPE.ArquiteturaSoftware.DesafioFinal.Application.Interfaces;
using XPE.ArquiteturaSoftware.DesafioFinal.Application.Mappers;
using XPE.ArquiteturaSoftware.DesafioFinal.Application.Requests;
using XPE.ArquiteturaSoftware.DesafioFinal.Application.Responses;
using XPE.ArquiteturaSoftware.DesafioFinal.Domain.Models;
using XPE.ArquiteturaSoftware.DesafioFinal.Infra.Data.Interfaces;

namespace XPE.ArquiteturaSoftware.DesafioFinal.Application.Services;

public sealed class ProductService(ILogger<ProductService> logger,
                                   IProductRepository repository) : IProductService
{
    public async Task<Result<CreateProductResponse>> CreateAsync(CreateProductRequest request)
    {
        try
        {
            var product = request.MapToModel();

            var productId = await repository.CreateAsync(product);
            if (productId > 0)
                return Result.Ok(new CreateProductResponse(productId));

            return Result
                    .Fail<CreateProductResponse>("Product was not created.")
                    .WithError(new Error("Insert returned invalid id (<= 0)."));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "[{ServiceName}][{MethodName}] Error: {ErrorMessage}",
                nameof(ProductService), nameof(CreateAsync), ex.Message);

            return Result
                .Fail<CreateProductResponse>(new Error("Unexpected error while creating product.").CausedBy(ex));
        }
    }

    public async Task<Result<ProductResponse>> GetByIdAsync(int id)
    {
        try
        {
            var product = await repository.GetByIdAsync(id);
            if (product is null)
                return Result.Fail<ProductResponse>(new Error("Not found").WithMetadata("statusCode", 404));

            var resp = new ProductResponse(product.Id, product.Name, product.Description, product.Price, product.Active, product.CreatedAt, product.UpdatedAt);
            return Result.Ok(resp);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "[{ServiceName}][{MethodName}] Error: {ErrorMessage}",
                nameof(ProductService), nameof(GetByIdAsync), ex.Message);
            return Result.Fail<ProductResponse>(new Error("Unexpected error while getting product.").CausedBy(ex));
        }
    }

    public async Task<Result<IEnumerable<ProductResponse>>> GetAllAsync()
    {
        try
        {
            var list = await repository.GetAllAsync();
            var resp = list.Select(p => new ProductResponse(p.Id, p.Name, p.Description, p.Price, p.Active, p.CreatedAt, p.UpdatedAt));
            return Result.Ok(resp);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "[{ServiceName}][{MethodName}] Error: {ErrorMessage}",
                nameof(ProductService), nameof(GetAllAsync), ex.Message);
            return Result.Fail<IEnumerable<ProductResponse>>(new Error("Unexpected error while listing products.").CausedBy(ex));
        }
    }

    public async Task<Result<IEnumerable<ProductResponse>>> FindByNameAsync(string name)
    {
        try
        {
            var list = await repository.FindByNameAsync(name);
            var resp = list.Select(p => new ProductResponse(p.Id, p.Name, p.Description, p.Price, p.Active, p.CreatedAt, p.UpdatedAt));
            return Result.Ok(resp);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "[{ServiceName}][{MethodName}] Error: {ErrorMessage}",
                nameof(ProductService), nameof(FindByNameAsync), ex.Message);
            return Result.Fail<IEnumerable<ProductResponse>>(new Error("Unexpected error while finding products.").CausedBy(ex));
        }
    }

    public async Task<Result<int>> CountAsync()
    {
        try
        {
            var total = await repository.CountAsync();
            return Result.Ok(total);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "[{ServiceName}][{MethodName}] Error: {ErrorMessage}",
                nameof(ProductService), nameof(CountAsync), ex.Message);
            return Result.Fail<int>(new Error("Unexpected error while counting products.").CausedBy(ex));
        }
    }

    public async Task<Result> UpdateAsync(int id, UpdateProductRequest request)
    {
        try
        {
            var updated = new Product(request.Name, request.Description, request.Price);
            updated.SetActive(request.Active);

            var ok = await repository.UpdateAsync(id, updated);
            return ok ? Result.Ok() : Result.Fail(new Error("Not found").WithMetadata("statusCode", 404));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "[{ServiceName}][{MethodName}] Error: {ErrorMessage}",
                nameof(ProductService), nameof(UpdateAsync), ex.Message);
            return Result.Fail(new Error("Unexpected error while updating product.").CausedBy(ex));
        }
    }

    public async Task<Result> DeleteAsync(int id)
    {
        try
        {
            var ok = await repository.DeleteAsync(id);
            return ok ? Result.Ok() : Result.Fail(new Error("Not found").WithMetadata("statusCode", 404));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "[{ServiceName}][{MethodName}] Error: {ErrorMessage}",
                nameof(ProductService), nameof(DeleteAsync), ex.Message);
            return Result.Fail(new Error("Unexpected error while deleting product.").CausedBy(ex));
        }
    }
}