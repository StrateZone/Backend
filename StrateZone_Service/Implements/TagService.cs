using AutoMapper;
using StrateZone_Repository.Entities;
using StrateZone_Repository.Implements;
using StrateZone_Repository.Interfaces;
using StrateZone_Service.BusinessModels;
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
    }
}
