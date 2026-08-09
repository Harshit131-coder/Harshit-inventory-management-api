using InventoryManagementAPI.Shared.Exceptions;
using InventoryManagementAPI.Models.DTOs.Category;
using InventoryManagementAPI.Models.DTOs.Common;
using InventoryManagementAPI.Models.Entities;
using InventoryManagementAPI.Repositories.Interfaces;
using InventoryManagementAPI.Resources;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

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

        public async Task<ActionResult<ApiResponse<PagedResult<CategoryDto>>>> GetAll(
            int pageNumber = 1, int pageSize = 10, string? search = null)
        {
            var (categories, totalCount) = await _repository.GetPagedWithProductsAsync(pageNumber, pageSize, search);

            var result = new PagedResult<CategoryDto>
            {
                Items = categories.Select(c => new CategoryDto
                {
                    Id              = c.Id,
                    Name            = c.Name,
                    Description     = c.Description,
                    ProductCount    = c.Products.Count,
                    CreatedAtUtc    = c.CreatedAtUtc,
                    UpdatedAtUtc    = c.UpdatedAtUtc
                }).ToList(),
                PageNumber  = pageNumber,
                PageSize    = pageSize,
                TotalCount  = totalCount
            };

             return Ok(ApiResponse<PagedResult<CategoryDto>>.SuccessResponse(result, Messages.Category.Retrieved));
        }


        [HttpPost]
        public async Task<ActionResult<ApiResponse<CategoryDto>>> Create(CreateCategoryDto dto)
        {
            if (await _repository.NameExistsAsync(dto.Name))
            {
                throw new ConflictException(Messages.Category.NameAlreadyExists);
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

            return CreatedAtAction(nameof(GetAll), 
                new { id = result.Id }, 
                ApiResponse<CategoryDto>.SuccessResponse(result, Messages.Category.Created));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<CategoryDto>>> GetById(int id)
        {
            var category = await _repository.GetByIdWithProductsAsync(id);

            if (category is null)
            {
                throw new NotFoundException("Category", id);
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

            return Ok(ApiResponse<CategoryDto>.SuccessResponse(result, Messages.Category.Retrieved));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<CategoryDto>>> Update(int id, UpdateCategoryDto dto)
        {
            var category = await _repository.GetByIdWithProductsAsync(id);

            if (category is null)
            {
                throw new NotFoundException("Category", id);
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

            return Ok(ApiResponse<CategoryDto>.SuccessResponse(result, Messages.Category.Updated));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var category = await _repository.GetByIdWithProductsAsync(id);

            if (category is null)
            {
                throw new NotFoundException("Category", id);
            }

            if (category.Products.Any())
            {    
                throw new ConflictException(Messages.Category.HasProducts);
            }

            await _repository.RemoveAsync(category);
            await _repository.SaveChangesAsync();

            return NoContent();
        }
    }
}
