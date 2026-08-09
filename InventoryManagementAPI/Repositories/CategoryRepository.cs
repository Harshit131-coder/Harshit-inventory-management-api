using InventoryManagementAPI.Data;
using InventoryManagementAPI.Models.Entities;
using InventoryManagementAPI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagementAPI.Repositories;

public class CategoryRepository : ICategoryRepository
{
    private readonly AppDbContext _db;

    public CategoryRepository(AppDbContext db)
    {
        this._db = db;
    }

    public async Task<Category?> GetByIdAsync(int id) =>
        await _db.Categories.FindAsync(id);

    public async Task<Category?> GetByIdWithProductsAsync(int id) =>
    await _db.Categories
            .Include(c => c.Products)
            .FirstOrDefaultAsync(c => c.Id == id);

    public async Task<IReadOnlyList<Category>> GetAllAsync() =>
        await _db.Categories.ToListAsync();

    public async Task<IReadOnlyList<Category>> GetAllWithProductsAsync() =>
        await _db.Categories
            .Include(c => c.Products)
            .ToListAsync();

    public async Task<bool> NameExistsAsync(string name) =>
        await _db.Categories.AnyAsync(c => c.Name == name);

    public async Task<(IReadOnlyList<Category> Items, int TotalCount)> GetPagedWithProductsAsync(
        int pageNumber, int pageSize, string? search)
    {
        IQueryable<Category> query = _db.Categories.Include(c => c.Products);
        
        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(c => c.Name.Contains(search));
        }

        var totalCount = await query.CountAsync();

        var items = await query
            .Include(c => c.Products)
            .OrderBy(c => c.Name)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task AddAsync(Category category) =>
        await _db.Categories.AddAsync(category);

    public Task RemoveAsync(Category category)
    {
        _db.Categories.Remove(category);
        return Task.CompletedTask;
    }

    public async Task SaveChangesAsync() =>
        await _db.SaveChangesAsync();
}