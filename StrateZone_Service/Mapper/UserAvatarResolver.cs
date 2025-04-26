using AutoMapper;
using StrateZone_Repository.Entities;
using StrateZone_Service.BusinessModels;
using StrateZone_Service.CustomModels.ResponseModels;
using StrateZone_Service.Interfaces;

namespace StrateZone_Service.Mapper
{
    public class UserResponseAvatarResolver : IValueResolver<User, UserResponse, string>
    {
        private readonly IImageService _imageService;

        public UserResponseAvatarResolver(IImageService imageService)
        {
            _imageService = imageService;
        }

        public string? Resolve(User source, UserResponse destination, string destMember, ResolutionContext context)
        {
            if (source.AvatarUrl != null) return source.AvatarUrl;

            var result = _imageService.GetUserAvatarAsync(source.UserId).Result;
            return result?.Url;
        }
    }

    public class UserAvatarResolver : IValueResolver<User, UserModel, string>
    {
        private readonly IImageService _imageService;

        public UserAvatarResolver(IImageService imageService)
        {
            _imageService = imageService;
        }

        public string? Resolve(User source, UserModel destination, string destMember, ResolutionContext context)
        {
            if (source.AvatarUrl != null) return source.AvatarUrl;

            var result = _imageService.GetUserAvatarAsync(source.UserId).Result;
            return result?.Url;
        }
    }
}
