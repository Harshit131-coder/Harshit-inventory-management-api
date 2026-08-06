using InventoryManagementAPI.Models.Entities;

namespace InventoryManagementAPI.Repositories.Interfaces;

public interface IProductRepository
{
    Task<Product?> GetByIdAsync(int id);
    Task<Product?> GetByIdWithCategoryAsync(int id);
    Task<IReadOnlyList<Product>> GetAllWithCategoryAsync();
    Task<bool> SkuExistsAsync(string sku);
    Task<bool> CategoryExistsAsync(int categoryId);
    Task AddAsync(Product product);
    Task SaveChangesAsync();
}