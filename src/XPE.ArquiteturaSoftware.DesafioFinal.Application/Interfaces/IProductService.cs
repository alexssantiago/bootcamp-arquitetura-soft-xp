using CSharpFunctionalExtensions;
using XPE.ArquiteturaSoftware.DesafioFinal.Application.Requests;
using XPE.ArquiteturaSoftware.DesafioFinal.Application.Responses;

namespace XPE.ArquiteturaSoftware.DesafioFinal.Application.Interfaces;

public interface IProductService
{
    Task<Result<CreateProductResponse>> CreateAsync(CreateProductRequest request);
    Task<Result<ProductResponse>> GetByIdAsync(int id);
    Task<Result<IEnumerable<ProductResponse>>> GetAllAsync();
    Task<Result<IEnumerable<ProductResponse>>> FindByNameAsync(string name);
    Task<Result<int>> CountAsync();
    Task<Result> UpdateAsync(int id, UpdateProductRequest request);
    Task<Result> DeleteAsync(int id);
}