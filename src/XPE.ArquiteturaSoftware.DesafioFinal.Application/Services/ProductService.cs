using CSharpFunctionalExtensions;
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
                return Result.Success(new CreateProductResponse(productId));

            return Result.Failure<CreateProductResponse>(
                "Product was not created. Insert returned invalid id (<= 0).");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "[{ServiceName}][{MethodName}] Error: {ErrorMessage}",
                nameof(ProductService), nameof(CreateAsync), ex.Message);

            return Result.Failure<CreateProductResponse>(
                "Unexpected error while creating product.");
        }
    }

    public async Task<Result<ProductResponse>> GetByIdAsync(int id)
    {
        try
        {
            var product = await repository.GetByIdAsync(id);
            if (product is null)
                return Result.Failure<ProductResponse>("Not found");

            var resp = new ProductResponse(
                product.Id, product.Name, product.Description,
                product.Price, product.Active, product.CreatedAt, product.UpdatedAt);

            return Result.Success(resp);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "[{ServiceName}][{MethodName}] Error: {ErrorMessage}",
                nameof(ProductService), nameof(GetByIdAsync), ex.Message);

            return Result.Failure<ProductResponse>(
                "Unexpected error while getting product.");
        }
    }

    public async Task<Result<IEnumerable<ProductResponse>>> GetAllAsync()
    {
        try
        {
            var list = await repository.GetAllAsync();
            var resp = list.Select(p => new ProductResponse(
                p.Id, p.Name, p.Description, p.Price, p.Active, p.CreatedAt, p.UpdatedAt));

            return Result.Success(resp);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "[{ServiceName}][{MethodName}] Error: {ErrorMessage}",
                nameof(ProductService), nameof(GetAllAsync), ex.Message);

            return Result.Failure<IEnumerable<ProductResponse>>(
                "Unexpected error while listing products.");
        }
    }

    public async Task<Result<IEnumerable<ProductResponse>>> FindByNameAsync(string name)
    {
        try
        {
            var list = await repository.FindByNameAsync(name);
            var resp = list.Select(p => new ProductResponse(
                p.Id, p.Name, p.Description, p.Price, p.Active, p.CreatedAt, p.UpdatedAt));

            return Result.Success(resp);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "[{ServiceName}][{MethodName}] Error: {ErrorMessage}",
                nameof(ProductService), nameof(FindByNameAsync), ex.Message);

            return Result.Failure<IEnumerable<ProductResponse>>(
                "Unexpected error while finding products.");
        }
    }

    public async Task<Result<int>> CountAsync()
    {
        try
        {
            var total = await repository.CountAsync();
            return Result.Success(total);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "[{ServiceName}][{MethodName}] Error: {ErrorMessage}",
                nameof(ProductService), nameof(CountAsync), ex.Message);

            return Result.Failure<int>("Unexpected error while counting products.");
        }
    }

    public async Task<Result> UpdateAsync(int id, UpdateProductRequest request)
    {
        try
        {
            var updated = new Product(request.Name, request.Description, request.Price);
            updated.SetActive(request.Active);

            var ok = await repository.UpdateAsync(id, updated);
            return ok ? Result.Success() : Result.Failure("Not found");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "[{ServiceName}][{MethodName}] Error: {ErrorMessage}",
                nameof(ProductService), nameof(UpdateAsync), ex.Message);

            return Result.Failure("Unexpected error while updating product.");
        }
    }

    public async Task<Result> DeleteAsync(int id)
    {
        try
        {
            var ok = await repository.DeleteAsync(id);
            return ok ? Result.Success() : Result.Failure("Not found");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "[{ServiceName}][{MethodName}] Error: {ErrorMessage}",
                nameof(ProductService), nameof(DeleteAsync), ex.Message);

            return Result.Failure("Unexpected error while deleting product.");
        }
    }
}