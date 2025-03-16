using AutoMapper;
using Microsoft.Extensions.Logging;
using StrateZone_Repository.Entities;
using StrateZone_Repository.Interfaces;
using StrateZone_Service.BusinessModels;
using StrateZone_Service.CustomModels.RequestModels;
using StrateZone_Service.Interfaces;

namespace StrateZone_Service.Implements
{
    public class ImageService : IImageService
    {
        private readonly IImageRepository _imageRepository;
        private readonly ICloudinaryService _cloudinaryService;
        private readonly IMapper _mapper;

        public ImageService(IImageRepository imageRepository, ICloudinaryService cloudinaryService, IMapper mapper)
        {
            _imageRepository = imageRepository;
            _cloudinaryService = cloudinaryService;
            _mapper = mapper;
        }

        public async Task<ImageModel> CreateImageAsync(ImageRequest imageRequest)
        {
            try
            {
                var imgUrl = imageRequest.Height.HasValue && imageRequest.Width.HasValue
                    ? await _cloudinaryService.UploadImageAsync(imageRequest.ImageFile, (int)imageRequest.Width, (int)imageRequest.Height)
                    : await _cloudinaryService.UploadImageAsync(imageRequest.ImageFile);

                ImageModel imageModel = new ImageModel()
                {
                    CreatedAt = DateTime.Now,
                    Url = imgUrl
                };

                switch (imageRequest.Type)
                {
                    case ImageType.avatar:
                        imageModel.UserId = imageRequest.EntityId;
                        break;
                    case ImageType.game_type:
                        imageModel.GameTypeId = imageRequest.EntityId;
                        break;
                    case ImageType.product:
                        imageModel.ProductId = imageRequest.EntityId;
                        break;
                    case ImageType.thread:
                        imageModel.ThreadId = imageRequest.EntityId;
                        break;
                    case ImageType.event_thumbnail:
                        imageModel.EventId = imageRequest.EntityId;
                        break;
                    case ImageType.tournament_thumbnail:
                        imageModel.TournamentId = imageRequest.EntityId;
                        break;
                }

                var image = _mapper.Map<Image>(imageModel);
                var result = await _imageRepository.CreateImageAsync(image);

                return _mapper.Map<ImageModel>(result);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<ImageModel> DeleteImageAsync(int id)
        {
            try
            {
                var result = await _imageRepository.DeleteImageAsync(id);
                return _mapper.Map<ImageModel>(result);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<ImageModel> GetEventThumbnailAsync(int eventId)
        {
            try
            {
                var result = await _imageRepository.GetEventThumbnailAsync(eventId);
                return _mapper.Map<ImageModel>(result);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<ImageModel> GetGametypeThumbnail(int gametypeId)
        {
            try
            {
                var result = await _imageRepository.GetGametypeThumbnailAsync(gametypeId);
                return _mapper.Map<ImageModel>(result);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<ImageModel>> GetProductImagesAsync(int productId)
        {
            try
            {
                var result = await _imageRepository.GetProductImagesAsync(productId);
                return _mapper.Map<List<ImageModel>>(result);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<ImageModel>> GetThreadImagesAsync(int threadId)
        {
            try
            {
                var result = await _imageRepository.GetThreadImagesAsync(threadId);
                return _mapper.Map<List<ImageModel>>(result);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<ImageModel> GetTournamentThumbnailAsync(int tournamentId)
        {
            try
            {
                var result = await _imageRepository.GetTournamentThumbnailAsync(tournamentId);
                return _mapper.Map<ImageModel>(result);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<ImageModel> GetUserAvatarAsync(int userId)
        {
            try
            {
                var result = await _imageRepository.GetUserAvatarAsync(userId);
                return _mapper.Map<ImageModel>(result);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<ImageModel> UpdateImageAsync(ImageRequest image, int id)
        {
            throw new NotImplementedException();
        }
    }
}
