using InventoryManagementAPI.Data;
using InventoryManagementAPI.Models.Entities;
using InventoryManagementAPI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagementAPI.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly AppDbContext _db;

    public ProductRepository(AppDbContext db)
    {
       this._db = db;
    }

    public async Task<Product?> GetByIdAsync(int id) =>
        await _db.Products.FindAsync(id);
    public async Task<Product?> GetByIdWithCategoryAsync(int id) =>
        await _db.Products.Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.Id == id);
    public async Task<IReadOnlyList<Product>> GetAllWithCategoryAsync() =>
        await _db.Products
            .Include(p => p.Category)
            .ToListAsync();
    public async Task<bool> SkuExistsAsync(string sku, int? excludeId = null) =>
        await _db.Products.AnyAsync(p => p.Sku == sku   && 
                                    (excludeId == null  || 
                                    p.Id != excludeId));
    public async Task<bool> CategoryExistsAsync(int categoryId) =>
        await _db.Categories
            .AnyAsync(c => c.Id == categoryId);
    public async Task<(IReadOnlyList<Product> Items, int TotalCount)> GetPagedWithCategoryAsync(
        int pageNumber, int pageSize, string? search, int? categoryId)
    {
        IQueryable<Product> query = _db.Products.Include(p => p.Category);

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(p => p.Name.Contains(search));
        }

        if (categoryId is not null)
        {
            query = query.Where(p => p.CategoryId == categoryId);
        }

        var totalCount = await query.CountAsync();

        var items = await query
            .Include(p => p.Category)
            .OrderBy(p => p.Name)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }
    public async Task AddAsync(Product product) =>
        await _db.Products
            .AddAsync(product);
    public async Task SaveChangesAsync() =>
        await _db.SaveChangesAsync();
    public Task RemoveAsync(Product product)
    {
        _db.Products.Remove(product);
        return Task.CompletedTask;
    }
}