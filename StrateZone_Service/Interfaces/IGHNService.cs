namespace StrateZone_Service.Interfaces
{
    public interface IGHNService
    {
        Task<string> GetServicesAsync();
        Task<string> CreateOrderAsync(object orderData);
        Task<string> GetProvincesAsync();
        Task<string> UpdateOrderAsync(object orderData);
        Task<string> CalculaExpectedDeliveryTime();
    }
}