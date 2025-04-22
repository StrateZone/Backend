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
        private readonly IMapper _mapper;

        private static readonly string[] InappropriateKeywords = new string[]
        {
            "vcl", "vl", "vđ", "dm", "đm", "dmm", "cmm", "ml", "cl", "cc", "vãi", "vãi l", "vãi đái",
            "địt", "lồn", "cặc", "buồi", "bướm", "nứng", "bú lol", "bú l", "đụ", "đéo", "lol", "dái", "cu", "vú", "bú", "liếm",
            "xếp hình", "sex", "fuck", "f u", "fuk", "f*ck", "fucking", "shit", "bitch", "wtf", "đĩ", "điếm", "cave", "phò",
            "v*l", "đ*o", "c*l", "đ.m", "d.m", "l*on", "b*oi", "f.u", "fuk", "c@", "b@", "l@", "n@g", "s.x", "s.xh", "đ*x", "v@i",
            "cmn", "dmtt", "vcd", "vlcl", "ch0", "ngu vl", "ngu vc", "ngu vkl", "óc", "óc heo",
            "súc vật", "óc chó", "óc lợn", "đần", "ngu", "thằng điên",
            "con điên", "vô học", "thất học", "mất dạy", "thiểu năng", "tâm thần",

            "dcs", "đcs", "hcm", "bkc", "bắc kì", "nam kì", "trung kì",
            "phản động", "biểu tình", "đa đảng", "đa nguyên", "dân chủ", "tự do ngôn luận", "chống phá", "chế độ", "cách mạng",
            "lật đổ", "đảo chính", "thế lực thù địch", "việt tân", "vnch", "cờ vàng", "cộng sản", "đảng", "nhà nước", "bộ công an",
            "cảnh sát", "bạo quyền", "trấn áp", "đấu tố", "tôn giáo", "thiên chúa", "phật giáo", "hồi giáo", "thần học", "chúa trời", "thiên đường",

            "giết", "ám sát", "khủng bố", "chém", "đánh", "nổ bom", "tấn công", "hành quyết",
            "thuốc nổ", "vũ khí", "chất nổ", "đập phá", "cướp", "hiếp", "hiếp dâm", "lạm dụng", "bạo lực",
            "buôn người", "buôn ma túy", "mua bán nội tạng", "dâm ô", "lừa đảo", "tống tiền", "tự sát"
        };

        public CommentService(ICommentRepository commentRepository, INotificationService notificationService, IMapper mapper, IUserService userService)
        {
            _commentRepository = commentRepository;
            _notificationService = notificationService;
            _mapper = mapper;
            _userService = userService;
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
                if (IsContentInappropriate(request.Content))
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

                user.ContributionPoints += 2;
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
                if (IsContentInappropriate(comment.Content))
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

        public static bool IsContentInappropriate(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return false;

            string[] normalizedInput = input.ToLowerInvariant().Split(" ");

            foreach (var keyword in InappropriateKeywords)
            {
                if (normalizedInput.Contains(keyword))
                    return true;
            }

            return false;
        }
    }
}
