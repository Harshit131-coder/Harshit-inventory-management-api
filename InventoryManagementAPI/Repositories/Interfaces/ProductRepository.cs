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
        _db = db;
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
    public async Task<bool> SkuExistsAsync(string sku) =>
        await _db.Products
            .AnyAsync(p => p.Sku == sku);
    public async Task<bool> CategoryExistsAsync(int categoryId) =>
        await _db.Categories
            .AnyAsync(c => c.Id == categoryId);

    public async Task AddAsync(Product product) =>
        await _db.Products
            .AddAsync(product);
    public async Task SaveChangesAsync() =>
        await _db.SaveChangesAsync();
}