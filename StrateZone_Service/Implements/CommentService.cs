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
    public class CommentService : ICommentService
    {
        private readonly ICommentRepository _commentRepository;
        private readonly IUserService _userService;
        private readonly INotificationService _notificationService;
        private readonly IProfanityService _profanityService;
        private readonly IMapper _mapper;

        public CommentService(ICommentRepository commentRepository, INotificationService notificationService, IMapper mapper, IUserService userService, IProfanityService profanityService)
        {
            _commentRepository = commentRepository;
            _notificationService = notificationService;
            _mapper = mapper;
            _userService = userService;
            _profanityService = profanityService;
        }

        public async Task<CommentModel> DeleteCommentAsync(int id)
        {
            try
            {
                var result = await _commentRepository.DeleteCommentAsync(id);

                return _mapper.Map<CommentModel>(result);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<CommentModel> GetCommentById(int id)
        {
            try
            {
                var result = await _commentRepository.GetCommentById(id);

                return _mapper.Map<CommentModel>(result);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<List<CommentModel>> GetCommentsByThreadIdAsync(int id)
        {
            try
            {
                var result = await _commentRepository.GetCommentsByThreadIdAsync(id);

                return _mapper.Map<List<CommentModel>>(result);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<List<CommentModel>> GetCommentsByUserIdAsync(int userId)
        {
            try
            {
                var result = await _commentRepository.GetCommentsByUserIdAsync(userId);

                return _mapper.Map<List<CommentModel>>(result);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<CommentModel> PostCommentAsync(CommentRequest request)
        {
            try
            {
                if (await _profanityService.CheckContain(request.Content))
                    throw new Exception("Comment contains inapproriate content, unable to comment.");

                var user = await _userService.GetUserByIdAsync(request.UserId)
                        ?? throw new Exception("User with this ID does not exist");

                CommentModel model = new()
                {
                    UserId = request.UserId,
                    ThreadId = request.ThreadId,
                    ReplyTo = request.ReplyTo,
                    Rating = 0,
                    Content = request.Content,
                    CreatedAt = DateTime.SpecifyKind(DateTime.UtcNow.AddHours(7), DateTimeKind.Unspecified)
                };

                var comment = _mapper.Map<Comment>(model);
                var result = await _commentRepository.PostCommentAsync(comment);

                user.ContributionPoints += 5;
                await _userService.UpdateUserAsync(_mapper.Map<UserModel>(user), user.UserId);

                return _mapper.Map<CommentModel>(result);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<CommentModel> UpdateCommentAsync(CommentModel comment, int id)
        {
            try 
            { 
                if (await _profanityService.CheckContain(comment.Content))
                    throw new Exception("Updated comment contains inapproriate content, unable to comment.");

                var mappedComment = _mapper.Map<Comment>(comment);
                var result = await _commentRepository.UpdateCommentAsync(mappedComment, id);
            
                return _mapper.Map<CommentModel>(result);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
    }
}
