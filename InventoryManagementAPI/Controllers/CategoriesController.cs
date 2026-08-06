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

        [HttpGet("{id}")]
        public async Task<ActionResult<CategoryDto>> GetById(int id)
        {
            var category = await _repository.GetByIdWithProductsAsync(id);

            if (category is null)
            {
                return NotFound($"Category with id {id} was not found.");
            }

            var result = new CategoryDto
            {
                Id              = category.Id,
                Name            = category.Name,
                Description     = category.Description,
                ProductCount    = category.Products.Count,
                CreatedAtUtc    = category.CreatedAtUtc,
                UpdatedAtUtc    = category.UpdatedAtUtc
            };

            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<CategoryDto>> Update(int id, UpdateCategoryDto dto)
        {
            var category = await _repository.GetByIdWithProductsAsync(id);

            if (category is null)
            {
                return NotFound($"Category with id {id} was not found.");
            }

            category.Name           = dto.Name;
            category.Description    = dto.Description;
            category.UpdatedAtUtc   = DateTime.UtcNow;

            await _repository.SaveChangesAsync();

            var result = new CategoryDto
            {
                Id              = category.Id,
                Name            = category.Name,
                Description     = category.Description,
                ProductCount    = category.Products.Count,
                CreatedAtUtc    = category.CreatedAtUtc,
                UpdatedAtUtc    = category.UpdatedAtUtc
            };

            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var category = await _repository.GetByIdWithProductsAsync(id);

            if (category is null)
            {
                return NotFound($"Category with id {id} was not found.");
            }

            if (category.Products.Any())
            {
                return Conflict("Cannot delete a category that still has products assigned to it.");
            }

            await _repository.RemoveAsync(category);
            await _repository.SaveChangesAsync();

            return NoContent();
        }
    }
}
