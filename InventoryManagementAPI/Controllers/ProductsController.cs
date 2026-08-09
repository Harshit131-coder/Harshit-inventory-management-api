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
                Items       = products.Select(p => p.ToDto()).ToList(),
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

            return Ok(ApiResponse<ProductDto>.SuccessResponse(product.ToDto(), Messages.Product.Retrieved));
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

            var product = dto.ToEntity();
            await _repository.AddAsync(product);
            await _repository.SaveChangesAsync();

            var created = await _repository.GetByIdWithCategoryAsync(product.Id);

          
            return CreatedAtAction(nameof(GetById), 
                new { id = product.Id },
                ApiResponse<ProductDto>.SuccessResponse(created!.ToDto(), Messages.Product.Created));
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


            product.ApplyUpdate(dto);
            await _repository.SaveChangesAsync();

            var updated = await _repository.GetByIdWithCategoryAsync(id);


            return Ok(ApiResponse<ProductDto>.SuccessResponse(updated!.ToDto(), Messages.Product.Updated));
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
