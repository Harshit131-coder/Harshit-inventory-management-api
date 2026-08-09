using InventoryManagementAPI.Shared.Exceptions;
using InventoryManagementAPI.Models.DTOs.Common;
using InventoryManagementAPI.Models.DTOs.Product;
using InventoryManagementAPI.Models.Entities;
using InventoryManagementAPI.Repositories.Interfaces;
using InventoryManagementAPI.Resources;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductRepository _repository;
        public ProductsController(IProductRepository repository)
        {
            this._repository = repository;
        }


        [HttpGet]
        public async Task<ActionResult<ApiResponse<PagedResult<ProductDto>>>> GetAll(
            int pageNumber = 1, int pageSize = 10, string ? search = null, int? categoryId = null)
        {
            var (products, totalCount) = await _repository.GetPagedWithCategoryAsync(pageNumber, pageSize, search, categoryId);

            var result = new PagedResult<ProductDto>
            {
                Items = products.Select(p => new ProductDto
                {
                    Id              = p.Id,
                    Name            = p.Name,
                    Sku             = p.Sku,
                    Description     = p.Description,
                    Price           = p.Price,
                    QuantityInStock = p.QuantityInStock,
                    CategoryId      = p.CategoryId,
                    CategoryName    = p.Category?.Name ?? string.Empty,
                    CreatedAtUtc    = p.CreatedAtUtc,
                    UpdatedAtUtc    = p.UpdatedAtUtc
                }).ToList(),
                PageNumber  = pageNumber,
                PageSize    = pageSize,
                TotalCount  = totalCount
            };
           
            return Ok(ApiResponse<PagedResult<ProductDto>>.SuccessResponse(result, Messages.Product.Retrieved));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<ProductDto>>> GetById(int id)
        {
            var product = await _repository.GetByIdWithCategoryAsync(id);

            if(product == null)
            {
                throw new NotFoundException("Product", id);
            }

            var result = new ProductDto
            {
                Id              = product.Id,
                Name            = product.Name,
                Sku             = product.Sku,
                Description     = product.Description,
                Price           = product.Price,
                QuantityInStock = product.QuantityInStock,
                CategoryId      = product.CategoryId,
                CategoryName    = product.Category?.Name ?? string.Empty,
                CreatedAtUtc    = product.CreatedAtUtc,
                UpdatedAtUtc    = product.UpdatedAtUtc
            };

            return Ok(ApiResponse<ProductDto>.SuccessResponse(result, Messages.Product.Retrieved));
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<ProductDto>>> Create(CreateProductDto dto)
        {
            if (!await _repository.CategoryExistsAsync(dto.CategoryId))
            {
                throw new BadRequestException(Messages.Product.InvalidCategory);
            }

            if (await _repository.SkuExistsAsync(dto.Sku))
            {
                throw new ConflictException(Messages.Product.SkuAlreadyExists);
            }

            var product = new Product
            {
                Name            = dto.Name,
                Sku             = dto.Sku,
                Description     = dto.Description,
                Price           = dto.Price,
                QuantityInStock = dto.QuantityInStock,
                CategoryId      = dto.CategoryId
            };

            await _repository.AddAsync(product);
            await _repository.SaveChangesAsync();

            var created = await _repository.GetByIdWithCategoryAsync(product.Id);

            var result = new ProductDto
            {
                Id              = created!.Id,
                Name            = created.Name,
                Sku             = created.Sku,
                Description     = created.Description,
                Price           = created.Price,
                QuantityInStock = created.QuantityInStock,
                CategoryId      = created.CategoryId,
                CategoryName    = created.Category?.Name ?? string.Empty,
                CreatedAtUtc    = created.CreatedAtUtc,
                UpdatedAtUtc    = created.UpdatedAtUtc
            };

            return CreatedAtAction(nameof(GetById), 
                new { id = result.Id },
                ApiResponse<ProductDto>.SuccessResponse(result, Messages.Product.Created));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<ProductDto>>> Update(int id, UpdateProductDto dto)
        {
            var product = await _repository.GetByIdAsync(id);

            if (product is null)
            {
                throw new NotFoundException("Product", id);
            }

            if (!await _repository.CategoryExistsAsync(dto.CategoryId))
            {
                throw new BadRequestException(Messages.Product.InvalidCategory);
            }

            if (await _repository.SkuExistsAsync(dto.Sku, excludeId: id))
            {
                  throw new ConflictException(Messages.Product.SkuAlreadyExists);
            }

            product.Name            = dto.Name;
            product.Sku             = dto.Sku;
            product.Description     = dto.Description;
            product.Price           = dto.Price;
            product.QuantityInStock = dto.QuantityInStock;
            product.CategoryId      = dto.CategoryId;
            product.UpdatedAtUtc    = DateTime.UtcNow;

            await _repository.SaveChangesAsync();

            var updated = await _repository.GetByIdWithCategoryAsync(id);

            var result = new ProductDto
            {
                Id              = updated!.Id,
                Name            = updated.Name,
                Sku             = updated.Sku,
                Description     = updated.Description,
                Price           = updated.Price,
                QuantityInStock = updated.QuantityInStock,
                CategoryId      = updated.CategoryId,
                CategoryName    = updated.Category?.Name ?? string.Empty,
                CreatedAtUtc    = updated.CreatedAtUtc,
                UpdatedAtUtc    = updated.UpdatedAtUtc
            };

            return Ok(ApiResponse<ProductDto>.SuccessResponse(result, Messages.Product.Updated));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _repository.GetByIdAsync(id);

            if (product is null)
            {
                throw new NotFoundException("Product", id);
            }

            await _repository.RemoveAsync(product);
            await _repository.SaveChangesAsync();

            return NoContent();
        }
    }
}
