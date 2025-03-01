namespace StrateZone_Service.Interfaces
{
    public interface IGHNService
    {
        Task<string> CreateOrderAsync(object orderData);
        Task<string> GetProvincesAsync();
    }
}