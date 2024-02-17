namespace CartService
{
    public interface ICartBusiness
    {
        void GetProduct(int id);
    }

    public class Product
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int Price { get; set; }
    }
}
