namespace InventoryManagementAPI.Resources
{
    public class Messages
    {
        public static class Category
        {
            public const string Created             = "Category created successfully.";
            public const string Updated             = "Category updated successfully.";
            public const string Deleted             = "Category deleted successfully.";
            public const string Retrieved           = "Category(ies) retrieved successfully.";
            public const string NameAlreadyExists   = "A category with this name already exists.";
            public const string HasProducts         = "Cannot delete a category that still has products assigned to it.";
        }

        public static class Product
        {
            public const string Created             = "Product created successfully.";
            public const string Updated             = "Product updated successfully.";
            public const string Deleted             = "Product deleted successfully.";
            public const string Retrieved           = "Product(s) retrieved successfully.";
            public const string SkuAlreadyExists    = "A product with this SKU already exists.";
            public const string InvalidCategory     = "The specified category does not exist.";
        }
    }
}
