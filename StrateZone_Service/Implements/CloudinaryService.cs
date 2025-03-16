using CloudinaryDotNet.Actions;
using CloudinaryDotNet;
using Microsoft.AspNetCore.Http;
using System;
using StrateZone_Service.Interfaces;

namespace StrateZone_Service.Implements
{
    public class CloudinaryService : ICloudinaryService
    {
        private readonly Cloudinary _cloudinary;

        public CloudinaryService(Cloudinary cloudinary)
        {
            _cloudinary = cloudinary;
        }

        public async Task<string> UploadImageAsync(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                    throw new ArgumentException("Invalid file");

                const long maxFileSize = 10 * 1024 * 1024;

                if (file.Length > maxFileSize)
                    throw new ArgumentException("File size can not exceed 10mb.");

                using var stream = file.OpenReadStream();
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(file.FileName, stream)
                };

                var uploadResult = await _cloudinary.UploadAsync(uploadParams);
                return uploadResult.SecureUrl.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<string> UploadImageAsync(IFormFile file, int width, int height)
        {
            try
            {
                if (width <= 0 || height <= 0)
                    return await UploadImageAsync(file);
                
                if (file == null || file.Length == 0)
                    throw new ArgumentException("Invalid file");

                const long maxFileSize = 10 * 1024 * 1024;

                if (file.Length > maxFileSize)
                    throw new ArgumentException("File size can not exceed 10mb.");

                using var stream = file.OpenReadStream();
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(file.FileName, stream),
                    Transformation = new Transformation().Crop("fill").Gravity("auto").Width(width).Height(height)
                };

                var uploadResult = await _cloudinary.UploadAsync(uploadParams);
                return uploadResult.SecureUrl.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
    }
}
