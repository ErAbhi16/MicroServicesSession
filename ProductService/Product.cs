namespace ProductService
{
    public class Product
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int Price { get; set; }
    }

    public static class ProductItems
    {
        public static List<Product> _products = new List<Product>
        {
            new Product{ ProductId = 1, ProductName = "Shirt", Price = 1200 },
            new Product{ ProductId = 2, ProductName = "Jean", Price = 1000},
            new Product{ ProductId = 3, ProductName = "Jacket", Price = 1500}
        };
    }
}
