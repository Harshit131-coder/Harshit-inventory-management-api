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

    public async Task<IReadOnlyList<Category>> GetAllAsync() =>
        await _db.Categories.ToListAsync();

    public async Task<bool> NameExistsAsync(string name) =>
        await _db.Categories.AnyAsync(c => c.Name == name);

    public async Task AddAsync(Category category) =>
        await _db.Categories.AddAsync(category);

    public async Task SaveChangesAsync() =>
        await _db.SaveChangesAsync();
}