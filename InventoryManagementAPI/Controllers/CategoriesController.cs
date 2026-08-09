using InventoryManagementAPI.Shared.Exceptions;
using InventoryManagementAPI.Models.DTOs.Category;
using InventoryManagementAPI.Models.DTOs.Common;
using InventoryManagementAPI.Models.Entities;
using InventoryManagementAPI.Repositories.Interfaces;
using InventoryManagementAPI.Resources;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using InventoryManagementAPI.Shared.Mappings;

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
                Items       = categories.Select(c => c.ToDto()).ToList(),
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

            var category = dto.ToEntity();
            await _repository.AddAsync(category);
            await _repository.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAll), 
                new { id = category.Id }, 
                ApiResponse<CategoryDto>.SuccessResponse(category.ToDto(), Messages.Category.Created));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<CategoryDto>>> GetById(int id)
        {
            var category = await _repository.GetByIdWithProductsAsync(id);

            if (category is null)
            {
                throw new NotFoundException("Category", id);
            }

            return Ok(ApiResponse<CategoryDto>.SuccessResponse(category.ToDto(), Messages.Category.Retrieved));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<CategoryDto>>> Update(int id, UpdateCategoryDto dto)
        {
            var category = await _repository.GetByIdWithProductsAsync(id);

            if (category is null)
            {
                throw new NotFoundException("Category", id);
            }

            category.ApplyUpdate(dto);
            await _repository.SaveChangesAsync();


            return Ok(ApiResponse<CategoryDto>.SuccessResponse(category.ToDto(), Messages.Category.Updated));
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
