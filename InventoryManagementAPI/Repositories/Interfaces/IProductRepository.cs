using InventoryManagementAPI.Models.Entities;

namespace InventoryManagementAPI.Repositories.Interfaces;

public interface IProductRepository
{
    Task<Product?> GetByIdAsync(int id);
    Task<Product?> GetByIdWithCategoryAsync(int id);
    Task<IReadOnlyList<Product>> GetAllWithCategoryAsync();
    Task<bool> SkuExistsAsync(string sku, int? excludeId = null);
    Task<bool> CategoryExistsAsync(int categoryId);
    Task<(IReadOnlyList<Product> Items, int TotalCount) > GetPagedWithCategoryAsync(int pageNumber, int pageSize, string? search, int? categoryId);
    Task AddAsync(Product product);
    Task SaveChangesAsync();
    Task RemoveAsync(Product product);
}