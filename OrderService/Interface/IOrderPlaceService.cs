namespace OrderService.Interface
{
    public interface IOrderPlaceService
    {
        void PublishOrder(int cartID);
    }
}
