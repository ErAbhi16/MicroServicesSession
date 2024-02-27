using System;

namespace ProductService
{
    public class ProductBusiness : IProductBusiness
    {
        private DateTime _recoveryTime = DateTime.UtcNow;
        private static readonly Random _random = new Random();
        public void GetProductById(int id)
        {
            Console.WriteLine($"Recovery Time {_recoveryTime}");
            Console.WriteLine($"Current Time {DateTime.UtcNow}");
            if (_recoveryTime > DateTime.UtcNow)
            {
                throw new Exception("Something went wrong");
            }

            if (_recoveryTime < DateTime.UtcNow && _random.Next(1, 4) == 1)
            {
                throw new Exception("Something went wrong");
            }
        }
    }
}
