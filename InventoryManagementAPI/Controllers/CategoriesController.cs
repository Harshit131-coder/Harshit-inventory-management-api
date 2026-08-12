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
        private readonly ILogger<CategoriesController> _logger;
        public CategoriesController(ICategoryRepository repository, ILogger<CategoriesController> logger)
        {
            this._repository = repository;
            this._logger = logger;
        }

        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<PagedResult<CategoryDto>>), StatusCodes.Status200OK)]
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
        [ProducesResponseType(typeof(ApiResponse<CategoryDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
        public async Task<ActionResult<ApiResponse<CategoryDto>>> Create(CreateCategoryDto dto)
        {
            if (await _repository.NameExistsAsync(dto.Name))
            {
                _logger.LogWarning(Logs.Category.DuplicateName, dto.Name);
                throw new ConflictException(Messages.Category.NameAlreadyExists);
            }

            var category = dto.ToEntity();
            await _repository.AddAsync(category);
            await _repository.SaveChangesAsync();

            _logger.LogInformation(Logs.Category.Created, category.Id);

            return CreatedAtAction(nameof(GetById), 
                new { id = category.Id }, 
                ApiResponse<CategoryDto>.SuccessResponse(category.ToDto(), Messages.Category.Created));
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<CategoryDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<CategoryDto>>> GetById(int id)
        {
            var category = await _repository.GetByIdWithProductsAsync(id);

            if (category is null)
            {
                _logger.LogWarning(Logs.NotFound, "Category", id);
                throw new NotFoundException("Category", id);
            }

            _logger.LogInformation(Logs.Category.Retrieved, id);
            return Ok(ApiResponse<CategoryDto>.SuccessResponse(category.ToDto(), Messages.Category.Retrieved));
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ApiResponse<CategoryDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
        public async Task<ActionResult<ApiResponse<CategoryDto>>> Update(int id, UpdateCategoryDto dto)
        {
            var category = await _repository.GetByIdWithProductsAsync(id);

            if (category is null)
            {
                _logger.LogWarning(Logs.NotFound, "Category", id);
                throw new NotFoundException("Category", id);
            }

            category.ApplyUpdate(dto);
            await _repository.SaveChangesAsync();

            _logger.LogInformation(Logs.Category.Updated, category.Id);
            return Ok(ApiResponse<CategoryDto>.SuccessResponse(category.ToDto(), Messages.Category.Updated));
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Delete(int id)
        {
            var category = await _repository.GetByIdWithProductsAsync(id);

            if (category is null)
            {
                _logger.LogWarning(Logs.NotFound, "Category", id);
                throw new NotFoundException("Category", id);
            }

            if (category.Products.Any())
            {
                _logger.LogWarning(Logs.Category.DeleteBlocked, id); 
                throw new ConflictException(Messages.Category.HasProducts);
            }

            await _repository.RemoveAsync(category);
            await _repository.SaveChangesAsync();

            _logger.LogInformation(Logs.Category.Deleted, category.Id);
            return NoContent();
        }
    }
}
