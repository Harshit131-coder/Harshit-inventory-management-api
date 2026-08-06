using InventoryManagementAPI.Models.Entities;

namespace InventoryManagementAPI.Repositories.Interfaces
{
    public interface ICategoryRepository
    {
        Task<Category?> GetByIdAsync(int id);
        Task<IReadOnlyList<Category>> GetAllAsync();
        Task<IReadOnlyList<Category>> GetAllWithProductsAsync();
        Task<bool> NameExistsAsync(string name);
        Task AddAsync(Category category);
        Task SaveChangesAsync();
    }
}
