using XPE.ArquiteturaSoftware.DesafioFinal.Domain.Models;
using XPE.ArquiteturaSoftware.DesafioFinal.Infra.Data.Interfaces;

namespace XPE.ArquiteturaSoftware.DesafioFinal.Infra.Data.Repositories;

public sealed class ProductRepository : IProductRepository
{
    public async Task<int> CreateAsync(Product model)
    {
        throw new NotImplementedException();
    }
}