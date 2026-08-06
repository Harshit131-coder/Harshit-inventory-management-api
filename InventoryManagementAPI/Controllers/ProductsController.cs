using InventoryManagementAPI.Models.DTOs.Product;
using InventoryManagementAPI.Repositories.Interfaces;
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
        public async Task<ActionResult<IReadOnlyList<ProductDto>>> GetAll()
        {
            var products = await _repository.GetAllWithCategoryAsync();
            var result = products.Select(p => new ProductDto
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
            }).ToList();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProductDto>> GetById(int id)
        {
            var product = await _repository.GetByIdWithCategoryAsync(id);

            if(product == null)
            {
                return NotFound($"Product with ID {id} not found.");
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

            return Ok(result);
        }
    }
}
