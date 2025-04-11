using AutoMapper;
using StrateZone_Repository.Entities;
using StrateZone_Repository.Interfaces;
using StrateZone_Service.BusinessModels;
using StrateZone_Service.CustomModels.RequestModels;
using StrateZone_Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrateZone_Service.Implements
{
    public class LikeService : ILikeService
    {
        private readonly ILikeRepository _likeRepository;
        private readonly IMapper _mapper;

        public LikeService(ILikeRepository likeRepository, IMapper mapper)
        {
            _likeRepository = likeRepository;
            _mapper = mapper;
        }

        public async Task<LikeModel> CreateLike(LikeRequest request)
        {
            try
            {
                if (request.ThreadId == null && request.CommentId == null)
                    throw new ArgumentException("Either ThreadId or CommentId must be provided.");

                if (request.ThreadId != null && request.CommentId != null)
                    throw new ArgumentException("A like can only have either ThreadId or CommentId.");

                LikeModel model = new()
                {
                    CommentId = request.CommentId,
                    ThreadId = request.ThreadId,
                    UserId = request.UserId,
                };

                var like = _mapper.Map<Like>(model);
                var result = await _likeRepository.CreateLike(like);

                return _mapper.Map<LikeModel>(result);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<LikeModel> DeleteLike(int id)
        {
            try
            {
                var result = await _likeRepository.DeleteLike(id);

                return _mapper.Map<LikeModel>(result);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
    }
}
