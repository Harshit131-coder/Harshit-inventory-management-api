namespace InventoryManagementAPI.Resources
{
    public static class Logs
    {
        public const string NotFound = "{EntityName} with id {Id} was not found.";

        public static class Category
        {
            public const string DuplicateName   = "Category save rejected — name '{CategoryName}' already exists.";
            public const string Retrieved       = "Category {CategoryId} retrieved.";
            public const string Created         = "Category {CategoryId} created.";
            public const string Updated         = "Category {CategoryId} updated.";
            public const string Deleted         = "Category {CategoryId} deleted.";
            public const string DeleteBlocked   = "Delete blocked for category {CategoryId}: still has products assigned.";
        }

        public static class Product
        {
            public const string InvalidCategory = "Product save rejected - CategoryId {CategoryId} does not exist.";
            public const string Retrieved       = "Product {ProductId} retrieved.";
            public const string DuplicateSku    = "Product save rejected — SKU {Sku} already exists.";
            public const string Created         = "Product {ProductId} created.";
            public const string Updated         = "Product {ProductId} updated.";
            public const string Deleted         = "Product {ProductId} deleted.";
        }
    }
}
