namespace CartService
{
    public class CartDatabase
    {
        public static List<CartDetails> cartdb = new List<CartDetails> {
            new CartDetails{ CustomerID = 2,Amount =1200,ProductsID = 1},
            new CartDetails{ CustomerID = 2,Amount =1000,ProductsID = 2},
            new CartDetails{ CustomerID = 2,Amount =1100,ProductsID = 3},
        };
    }
}
