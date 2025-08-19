using XPE.ArquiteturaSoftware.DesafioFinal.Application.Requests;
using XPE.ArquiteturaSoftware.DesafioFinal.Application.Responses;

namespace XPE.ArquiteturaSoftware.DesafioFinal.Application.Interfaces;

public interface IProductService
{
    Task<CreateProductResponse> CreateAsync(CreateProductRequest request);
}