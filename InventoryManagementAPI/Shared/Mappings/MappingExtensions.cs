using InventoryManagementAPI.Models.DTOs.Category;
using InventoryManagementAPI.Models.DTOs.Product;
using InventoryManagementAPI.Models.Entities;

namespace InventoryManagementAPI.Shared.Mappings
{
    public static class MappingExtensions
    {
        public static CategoryDto ToDto(this Category category) => new()
        {
            Id              = category.Id,
            Name            = category.Name,
            Description     = category.Description,
            ProductCount    = category.Products.Count,
            CreatedAtUtc    = category.CreatedAtUtc,
            UpdatedAtUtc    = category.UpdatedAtUtc
        };

        public static Category ToEntity(this CreateCategoryDto dto) => new()
        {
            Name            = dto.Name,
            Description     = dto.Description
        };

        public static void ApplyUpdate(this Category category, UpdateCategoryDto dto)
        {
            category.Name           = dto.Name;
            category.Description    = dto.Description;
            category.UpdatedAtUtc   = DateTime.UtcNow;
        }
        public static ProductDto ToDto(this Product product) => new()
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

        public static Product ToEntity(this CreateProductDto dto) => new()
        {
            Name = dto.Name,
            Sku = dto.Sku,
            Description = dto.Description,
            Price = dto.Price,
            QuantityInStock = dto.QuantityInStock,
            CategoryId = dto.CategoryId
        };

        public static void ApplyUpdate(this Product product, UpdateProductDto dto)
        {
            product.Name = dto.Name;
            product.Sku = dto.Sku;
            product.Description = dto.Description;
            product.Price = dto.Price;
            product.QuantityInStock = dto.QuantityInStock;
            product.CategoryId = dto.CategoryId;
            product.UpdatedAtUtc = DateTime.UtcNow;
        }
    }   
}
