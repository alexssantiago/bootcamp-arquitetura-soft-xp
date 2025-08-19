using XPE.ArquiteturaSoftware.DesafioFinal.Domain.Models;

namespace XPE.ArquiteturaSoftware.DesafioFinal.Infra.Data.Interfaces;

public interface IProductRepository
{
    Task<int> CreateAsync(Product model);
}