using XPE.ArquiteturaSoftware.DesafioFinal.Domain.Models;

namespace XPE.ArquiteturaSoftware.DesafioFinal.Infra.Data.Interfaces;

public interface IProductRepository
{
    Task<int> CreateAsync(Product model);
    Task<Product?> GetByIdAsync(int id);
    Task<IEnumerable<Product>> GetAllAsync();
    Task<IEnumerable<Product>> FindByNameAsync(string name);
    Task<int> CountAsync();
    Task<bool> UpdateAsync(int id, Product updated);
    Task<bool> DeleteAsync(int id);
}