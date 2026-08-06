using InventoryManagementAPI.Models.DTOs.Category;
using InventoryManagementAPI.Models.Entities;
using InventoryManagementAPI.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryRepository _repository;

        public CategoriesController(ICategoryRepository repository)
        {
            this._repository = repository;
        }

        [HttpGet]

        public async Task<ActionResult<IReadOnlyList<CategoryDto>>> GetAll()
        {
            var categories = await _repository.GetAllWithProductsAsync();
            
            var result = categories.Select(c => new CategoryDto
            {
                Id              = c.Id,
                Name            = c.Name,
                Description     = c.Description,
                ProductCount    = c.Products.Count,
                CreatedAtUtc    = c.CreatedAtUtc,
                UpdatedAtUtc    = c.UpdatedAtUtc
            }).ToList();

            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<CategoryDto>> Create(CreateCategoryDto dto)
        {
            if (await _repository.NameExistsAsync(dto.Name))
            {
                return Conflict($"A category named '{dto.Name}' already exists.");
            }

            var category = new Category
            {
                Name        = dto.Name,
                Description = dto.Description
            };

            await _repository.AddAsync(category);
            await _repository.SaveChangesAsync();

            var result = new CategoryDto
            {
                Id              = category.Id,
                Name            = category.Name,
                Description     = category.Description,
                ProductCount    = 0,
                CreatedAtUtc    = category.CreatedAtUtc,
                UpdatedAtUtc    = category.UpdatedAtUtc
            };

            return CreatedAtAction(nameof(GetAll), new { id = result.Id }, result);
        }
    }
}
