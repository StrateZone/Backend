using AutoMapper;
using StrateZone_Repository.Entities;
using StrateZone_Repository.Implements;
using StrateZone_Repository.Interfaces;
using StrateZone_Repository.Parameters;
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
    public class TagService : ITagService
    {
        private readonly ITagRepository _tagRepository;
        private readonly IMapper _mapper;

        public TagService(ITagRepository tagRepository, IMapper mapper)
        {
            _tagRepository = tagRepository;
            _mapper = mapper;
        }

        public async Task<TagModel> CreateTagAsync(TagModel tagModel)
        {
            try
            {
                var tag = _mapper.Map<Tag>(tagModel);
                var result = await _tagRepository.CreateTagAsync(tag);
                return _mapper.Map<TagModel>(result);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<TagModel> DeleteTagAsync(int id)
        {
            try
            {
                var result = await _tagRepository.DeleteTagAsync(id);
                return _mapper.Map<TagModel>(result);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<TagModel> GetTagByIdAsync(int id)
        {
            try
            {
                var result = await _tagRepository.GetTagByIdAsync(id);
                return _mapper.Map<TagModel>(result);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<TagModel>> GetTagsAsync()
        {
            try
            {
                var result = await _tagRepository.GetTagsAsync();
                return _mapper.Map<List<TagModel>>(result);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<TagModel>> GetThreadTagsAsync()
        {
            try
            {
                var result = await _tagRepository.GetThreadTagsAsync();
                return _mapper.Map<List<TagModel>>(result);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<TagModel>> GetProductTagsAsync()
        {
            try
            {
                var result = await _tagRepository.GetProductTagsAsync();
                return _mapper.Map<List<TagModel>>(result);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<TagModel>> SearchTagsAsync(string content)
        {
            try
            {
                var result = await _tagRepository.SearchTagsAsync(content);
                return _mapper.Map<List<TagModel>>(result);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<TagModel> AdminActivateTagAsync(int id)
        {
            try
            {
                var toHide = await _tagRepository.GetTagByIdAsync(id)
                            ?? throw new Exception("Thread with this ID does not exist");
                var tagModel = _mapper.Map<TagModel>(toHide);
                if (tagModel.Status != PostgreEnums.TagStatus.hided.ToString())
                    throw new Exception($"This thread is already {tagModel.Status}");

                tagModel.Status = PostgreEnums.TagStatus.active.ToString();
                var result = await UpdateTagAsync(tagModel, tagModel.TagId);

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }


        public async Task<TagModel> AdminHideTagAsync(int id)
        {
            try
            {
                var toHide = await _tagRepository.GetTagByIdAsync(id)
                            ?? throw new Exception("Thread with this ID does not exist");
                var tagModel = _mapper.Map<TagModel>(toHide);
                if (tagModel.Status != PostgreEnums.TagStatus.active.ToString())
                    throw new Exception($"This thread is already {tagModel.Status}");

                tagModel.Status = PostgreEnums.TagStatus.hided.ToString();
                var result = await UpdateTagAsync(tagModel, tagModel.TagId);

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<TagModel> UpdateTagAsync(TagModel tagModel, int id)
        {
            try
            {
                var tag = _mapper.Map<Tag>(tagModel);
                var result = await _tagRepository.UpdateTagAsync(tag, id);

                return _mapper.Map<TagModel>(result);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
    }
}
