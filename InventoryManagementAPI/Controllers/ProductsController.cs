using InventoryManagementAPI.Shared.Exceptions;
using InventoryManagementAPI.Models.DTOs.Common;
using InventoryManagementAPI.Models.DTOs.Product;
using InventoryManagementAPI.Models.Entities;
using InventoryManagementAPI.Repositories.Interfaces;
using InventoryManagementAPI.Resources;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using InventoryManagementAPI.Shared.Mappings;

namespace InventoryManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductRepository _repository;
        private readonly ILogger<ProductsController> _logger;
        public ProductsController(IProductRepository repository, ILogger<ProductsController> logger)
        {
            this._repository = repository;
            this._logger = logger;
        }


        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<PagedResult<ProductDto>>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse<PagedResult<ProductDto>>>> GetAll(
            int pageNumber = 1, int pageSize = 10, string ? search = null, int? categoryId = null)
        {
            var (products, totalCount) = await _repository.GetPagedWithCategoryAsync(
                                                pageNumber, pageSize, search, categoryId);

            var result = new PagedResult<ProductDto>
            {
                Items       = products.Select(p => p.ToDto()).ToList(),
                PageNumber  = pageNumber,
                PageSize    = pageSize,
                TotalCount  = totalCount
            };
           
            return Ok(ApiResponse<PagedResult<ProductDto>>.SuccessResponse(result, Messages.Product.Retrieved));
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<ProductDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<ProductDto>>> GetById(int id)
        {
            var product = await _repository.GetByIdWithCategoryAsync(id);

            if(product == null)
            {
                _logger.LogWarning(Logs.NotFound, "Product", id);
                throw new NotFoundException("Product", id);
            }

            _logger.LogInformation(Logs.Product.Retrieved, id);
            return Ok(ApiResponse<ProductDto>.SuccessResponse(product.ToDto(), Messages.Product.Retrieved));
        }

        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<ProductDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
        public async Task<ActionResult<ApiResponse<ProductDto>>> Create(CreateProductDto dto)
        {
            if (!await _repository.CategoryExistsAsync(dto.CategoryId))
            {
                _logger.LogWarning(Logs.Product.InvalidCategory, dto.CategoryId);
                throw new BadRequestException(Messages.Product.InvalidCategory);
            }

            if (await _repository.SkuExistsAsync(dto.Sku))
            {
                _logger.LogWarning(Logs.Product.DuplicateSku, dto.Sku);
                throw new ConflictException(Messages.Product.SkuAlreadyExists);
            }

            var product = dto.ToEntity();
            await _repository.AddAsync(product);
            await _repository.SaveChangesAsync();

            var created = await _repository.GetByIdWithCategoryAsync(product.Id);

            _logger.LogInformation(Logs.Product.Created, product.Id);
            return CreatedAtAction(nameof(GetById), 
                new { id = product.Id },
                ApiResponse<ProductDto>.SuccessResponse(created!.ToDto(), Messages.Product.Created));
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ApiResponse<ProductDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
        public async Task<ActionResult<ApiResponse<ProductDto>>> Update(int id, UpdateProductDto dto)
        {
            var product = await _repository.GetByIdAsync(id);

            if (product is null)
            {
                _logger.LogWarning(Logs.NotFound, "Product", id);
                throw new NotFoundException("Product", id);
            }

            if (!await _repository.CategoryExistsAsync(dto.CategoryId))
            {
                _logger.LogWarning(Logs.Product.InvalidCategory, dto.CategoryId);
                throw new BadRequestException(Messages.Product.InvalidCategory);
            }

            if (await _repository.SkuExistsAsync(dto.Sku, excludeId: id))
            {
                _logger.LogWarning(Logs.Product.DuplicateSku, dto.Sku);
                throw new ConflictException(Messages.Product.SkuAlreadyExists);
            }


            product.ApplyUpdate(dto);
            await _repository.SaveChangesAsync();

            var updated = await _repository.GetByIdWithCategoryAsync(id);

            _logger.LogInformation(Logs.Product.Updated, product.Id);
            return Ok(ApiResponse<ProductDto>.SuccessResponse(updated!.ToDto(), Messages.Product.Updated));
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _repository.GetByIdAsync(id);

            if (product is null)
            {
                _logger.LogWarning(Logs.NotFound, "Product", id);
                throw new NotFoundException("Product", id);
            }

            await _repository.RemoveAsync(product);
            await _repository.SaveChangesAsync();

            _logger.LogInformation(Logs.Product.Deleted, product.Id);
            return NoContent();
        }
    }
}
