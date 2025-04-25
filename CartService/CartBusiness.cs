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
            Policy.HandleResult<HttpResponseMessage>(message =>
                (int)message.StatusCode == 429 || (int)message.StatusCode >= 500)
                    .WaitAndRetryAsync(
                        retryCount: 2,
                        sleepDurationProvider: retryAttempt =>
                        {
                            // Exponential backoff: 2^retryAttempt seconds + random jitter (0–999 ms)
                            var backoff = TimeSpan.FromSeconds(Math.Pow(2, retryAttempt));
                            var jitter = TimeSpan.FromMilliseconds(_jitter.Next(0, 1000));
                            var totalDelay = backoff + jitter;

                            Console.WriteLine($"[Retry #{retryAttempt}] Waiting {totalDelay.TotalSeconds:F2} seconds before retry due to transient error.");
                            return totalDelay;
                        });


        // Circuit Breaker: break after 2 consecutive 500 errors, for 30 seconds
        private static readonly AsyncCircuitBreakerPolicy<HttpResponseMessage> circuitBreakerPolicy =
            Policy.HandleResult<HttpResponseMessage>(r => (int)r.StatusCode == 500)
                    .CircuitBreakerAsync(
                        handledEventsAllowedBeforeBreaking: 2,
                        durationOfBreak: TimeSpan.FromSeconds(30),
                        onBreak: (result, breakDelay) =>
                        {
                            Console.WriteLine($"Circuit broken due to {result.Result?.StatusCode}. Breaking for {breakDelay.TotalSeconds} seconds.");
                        },
                        onReset: () => Console.WriteLine("Circuit closed again. Back to normal."),
                        onHalfOpen: () => Console.WriteLine("Circuit in half-open state. Trying a test call.")
                    );


        private static readonly AsyncCircuitBreakerPolicy<HttpResponseMessage> advancedCircuitBreakerPolicy =
            Policy.HandleResult<HttpResponseMessage>(message => (int)message.StatusCode == 500)
                    .AdvancedCircuitBreakerAsync(
                        failureThreshold: 0.5, // 50% failure rate
                        samplingDuration: TimeSpan.FromSeconds(60), // Sample over 1 minute
                        minimumThroughput: 10, // At least 10 calls in the window to trigger evaluation
                        durationOfBreak: TimeSpan.FromSeconds(30) // How long to break
                    );


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
