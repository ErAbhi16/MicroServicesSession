using Polly;
using Polly.CircuitBreaker;
using Polly.Retry;
using Polly.Wrap;

namespace CartService
{
    public class CartBusiness : ICartBusiness
    {
        private readonly IHttpClientFactory _httpClientFactory;

        private static readonly Random _jitter = new Random();

        private static readonly AsyncRetryPolicy<HttpResponseMessage> transientErrorRetryPolicy =
            Policy.HandleResult<HttpResponseMessage>(message => (int)message.StatusCode == 429 || (int)message.StatusCode >= 500)
            .WaitAndRetryAsync(2, retryAttempt =>
            {
                Console.WriteLine($"Retrying because of transient error . Attempt {retryAttempt}");
                return TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)) + TimeSpan.FromMilliseconds(_jitter.Next(0, 1000));
                // firstattempt after 2.000-2.999 2nd attempt after 4.0-4.999
            });

        private static readonly AsyncCircuitBreakerPolicy<HttpResponseMessage> circuitBreakerPolicy =
            Policy.HandleResult<HttpResponseMessage>(message => (int)message.StatusCode == 500)
            .CircuitBreakerAsync(2, TimeSpan.FromSeconds(30));

        private static readonly AsyncCircuitBreakerPolicy<HttpResponseMessage> advancedCircuitBreakerPolicy =
            Policy.HandleResult<HttpResponseMessage>(message => (int)message.StatusCode == 500)
            .AdvancedCircuitBreakerAsync(0.5,TimeSpan.FromMinutes(1),10,TimeSpan.FromMinutes(1));

        private static readonly AsyncPolicyWrap<HttpResponseMessage> resilientPolicy = circuitBreakerPolicy.WrapAsync(transientErrorRetryPolicy);

        public CartBusiness(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        public async Task<bool> GetProductAsync(int id)
        {
            if (circuitBreakerPolicy.CircuitState == CircuitState.Open)
            {
                throw new Exception("Service is unavailable");
            }

            var httpClient = _httpClientFactory.CreateClient("ProductClient");
            
            //HttpResponseMessage response = await transientErrorRetryPolicy.ExecuteAsync(() => httpClient.GetAsync($"{id}"));
            
            HttpResponseMessage response = await circuitBreakerPolicy.ExecuteAsync(() => transientErrorRetryPolicy.ExecuteAsync(() => httpClient.GetAsync($"{id}")));
            
            //HttpResponseMessage response = await resilientPolicy.ExecuteAsync(() => httpClient.GetAsync($"{id}"));

            // Check if the response is successful (HTTP status code in the 200-299 range)
            if (response.IsSuccessStatusCode)
            {
                // Read the response content as a string
                string responseBody = await response.Content.ReadAsStringAsync();

                // Output the response body
                Console.WriteLine($"Response body: {responseBody}");
                return true;
            }
            else
            {
                // If the response is not successful, output the HTTP status code
                throw new Exception("Service is unavailable");
            }
            
            
        }
    }
}
