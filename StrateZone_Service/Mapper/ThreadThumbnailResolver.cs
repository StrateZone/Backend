using AutoMapper;
using StrateZone_Repository.DTO;
using StrateZone_Repository.Entities;
using StrateZone_Service.BusinessModels;
using StrateZone_Service.CustomModels.ResponseModels;
using StrateZone_Service.Interfaces;
using Thread = StrateZone_Repository.Entities.Thread;

namespace StrateZone_Service.Mapper
{
    public class ThreadThumbnailResolver : IValueResolver<Thread, ThreadModel, string>
    {
        private readonly IImageService _imageService;

        public ThreadThumbnailResolver(IImageService imageService)
        {
            _imageService = imageService;
        }

        public string? Resolve(Thread source, ThreadModel destination, string destMember, ResolutionContext context)
        {
            if (source.ThumbnailUrl != null) return source.ThumbnailUrl;    

            // Ensure the method is synchronous (use .Result or .GetAwaiter().GetResult())
            var result = _imageService.GetThreadImagesAsync(source.ThreadId).Result;
            return result?.Url;
        }
    }

    public class ThreadDTOThumbnailResolver : IValueResolver<ThreadDTO, ThreadModel, string>
    {
        private readonly IImageService _imageService;

        public ThreadDTOThumbnailResolver(IImageService imageService)
        {
            _imageService = imageService;
        }

        public string? Resolve(ThreadDTO source, ThreadModel destination, string destMember, ResolutionContext context)
        {
            if (source.ThumbnailUrl != null) return source.ThumbnailUrl;

            // Ensure the method is synchronous (use .Result or .GetAwaiter().GetResult())
            var result = _imageService.GetThreadImagesAsync(source.ThreadId).Result;
            return result?.Url;
        }
    }
}
