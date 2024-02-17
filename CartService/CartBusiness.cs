namespace CartService
{
    public class CartBusiness : ICartBusiness
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public CartBusiness(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        public void GetProduct(int id)
        {
            var httpClient = _httpClientFactory.CreateClient();
            httpClient.GetAsync(httpClient.BaseAddress).Wait();
        }
    }
}
