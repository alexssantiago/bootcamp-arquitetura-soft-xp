using CSharpFunctionalExtensions;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using XPE.ArquiteturaSoftware.DesafioFinal.Application.Caching;
using XPE.ArquiteturaSoftware.DesafioFinal.Application.Interfaces;
using XPE.ArquiteturaSoftware.DesafioFinal.Application.Mappers;
using XPE.ArquiteturaSoftware.DesafioFinal.Application.Requests;
using XPE.ArquiteturaSoftware.DesafioFinal.Application.Responses;
using XPE.ArquiteturaSoftware.DesafioFinal.Infra.Data.Interfaces;

namespace XPE.ArquiteturaSoftware.DesafioFinal.Application.Services;

public sealed class ProductService(
    ILogger<ProductService> logger,
    IProductRepository repository,
    IDistributedCache cache) : IProductService
{
    private static readonly TimeSpan TtlById = TimeSpan.FromMinutes(5);
    private static readonly TimeSpan TtlList = TimeSpan.FromSeconds(30);
    private static readonly TimeSpan TtlCount = TimeSpan.FromSeconds(30);
    private static readonly TimeSpan TtlByName = TimeSpan.FromSeconds(30);

    public async Task<Result<CreateProductResponse>> CreateAsync(CreateProductRequest request)
    {
        try
        {
            var product = request.MapToModel();

            var productId = await repository.CreateAsync(product);
            if (productId <= 0)
                return Result.Failure<CreateProductResponse>("Product was not created. Insert returned invalid id (<= 0).");

            await ProductCache.BumpVersionAsync(cache, CancellationToken.None);

            return Result.Success(new CreateProductResponse(productId));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "[{Service}][{Method}] {Msg}", nameof(ProductService), nameof(CreateAsync), ex.Message);
            return Result.Failure<CreateProductResponse>("Unexpected error while creating product.");
        }
    }

    public async Task<Result<ProductResponse>> GetByIdAsync(int id)
    {
        try
        {
            var v = await ProductCache.GetVersionAsync(cache, CancellationToken.None);
            var key = ProductCache.KeyProductById(v, id);

            var cached = await ProductCache.GetAsync<ProductResponse>(cache, key, CancellationToken.None);
            if (cached is not null) return Result.Success(cached);

            var entity = await repository.GetByIdAsync(id);
            if (entity is null) return Result.Failure<ProductResponse>("Not found");

            var resp = entity.ToResponse();
            await ProductCache.SetAsync(cache, key, resp, TtlById, CancellationToken.None);
            return Result.Success(resp);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "[{Service}][{Method}] {Msg}", nameof(ProductService), nameof(GetByIdAsync), ex.Message);
            return Result.Failure<ProductResponse>("Unexpected error while getting product.");
        }
    }

    public async Task<Result<IEnumerable<ProductResponse>>> GetAllAsync()
    {
        try
        {
            var v = await ProductCache.GetVersionAsync(cache, CancellationToken.None);
            var key = ProductCache.KeyAll(v);

            var cached = await ProductCache.GetAsync<IEnumerable<ProductResponse>>(cache, key, CancellationToken.None);
            if (cached is not null) return Result.Success(cached);

            var list = await repository.GetAllAsync();
            var resp = list.ToResponse();

            await ProductCache.SetAsync(cache, key, resp, TtlList, CancellationToken.None);
            return Result.Success(resp);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "[{Service}][{Method}] {Msg}", nameof(ProductService), nameof(GetAllAsync), ex.Message);
            return Result.Failure<IEnumerable<ProductResponse>>("Unexpected error while listing products.");
        }
    }

    public async Task<Result<IEnumerable<ProductResponse>>> FindByNameAsync(string name)
    {
        try
        {
            var v = await ProductCache.GetVersionAsync(cache, CancellationToken.None);
            var key = ProductCache.KeyByName(v, name);

            var cached = await ProductCache.GetAsync<IEnumerable<ProductResponse>>(cache, key, CancellationToken.None);
            if (cached is not null) return Result.Success(cached);

            var list = await repository.FindByNameAsync(name);
            var resp = list.ToResponse();

            await ProductCache.SetAsync(cache, key, resp, TtlByName, CancellationToken.None);
            return Result.Success(resp);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "[{Service}][{Method}] {Msg}", nameof(ProductService), nameof(FindByNameAsync), ex.Message);
            return Result.Failure<IEnumerable<ProductResponse>>("Unexpected error while finding products.");
        }
    }

    public async Task<Result<int>> CountAsync()
    {
        try
        {
            var v = await ProductCache.GetVersionAsync(cache, CancellationToken.None);
            var key = ProductCache.KeyCount(v);

            var cached = await ProductCache.GetAsync<int?>(cache, key, CancellationToken.None);
            if (cached.HasValue) return Result.Success(cached.Value);

            var total = await repository.CountAsync();
            await ProductCache.SetAsync(cache, key, total, TtlCount, CancellationToken.None);
            return Result.Success(total);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "[{Service}][{Method}] {Msg}", nameof(ProductService), nameof(CountAsync), ex.Message);
            return Result.Failure<int>("Unexpected error while counting products.");
        }
    }

    public async Task<Result> UpdateAsync(int id, UpdateProductRequest request)
    {
        try
        {
            var updated = request.MapToModel();
            var ok = await repository.UpdateAsync(id, updated);
            if (!ok) return Result.Failure("Not found");

            await ProductCache.BumpVersionAsync(cache, CancellationToken.None);

            var v = await ProductCache.GetVersionAsync(cache, CancellationToken.None);
            await ProductCache.RemoveAsync(cache, ProductCache.KeyProductById(v, id), CancellationToken.None);

            return Result.Success();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "[{Service}][{Method}] {Msg}", nameof(ProductService), nameof(UpdateAsync), ex.Message);
            return Result.Failure("Unexpected error while updating product.");
        }
    }

    public async Task<Result> DeleteAsync(int id)
    {
        try
        {
            var ok = await repository.DeleteAsync(id);
            if (!ok) return Result.Failure("Not found");

            await ProductCache.BumpVersionAsync(cache, CancellationToken.None);

            var v = await ProductCache.GetVersionAsync(cache, CancellationToken.None);
            await ProductCache.RemoveAsync(cache, ProductCache.KeyProductById(v, id), CancellationToken.None);

            return Result.Success();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "[{Service}][{Method}] {Msg}", nameof(ProductService), nameof(DeleteAsync), ex.Message);
            return Result.Failure("Unexpected error while deleting product.");
        }
    }
}