using Microsoft.AspNetCore.Http;

namespace StrateZone_Service.Interfaces
{
    public interface ICloudinaryService
    {
        Task<string> UploadImageAsync(IFormFile file);
        Task<string> UploadImageAsync(IFormFile file, int width, int height);
    }
}