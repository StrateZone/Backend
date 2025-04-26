using AutoMapper;
using StrateZone_Repository.Entities;
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
    public class ThreadsTagService : IThreadsTagService
    {
        private readonly IThreadsTagRepository _threadsTagRepository;
        private readonly IMapper _mapper;

        public ThreadsTagService(IThreadsTagRepository threadsTagRepository, IMapper mapper)
        {
            _threadsTagRepository = threadsTagRepository;
            _mapper = mapper;
        }

        public async Task<ThreadsTagModel> CreateThreadsTagAsync(ThreadsTagModel threadsTag)
        {
            try
            {
                var mapped = _mapper.Map<ThreadsTag>(threadsTag);
                var result = await _threadsTagRepository.CreateThreadsTagAsync(mapped);

                return _mapper.Map<ThreadsTagModel>(result);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<List<ThreadsTagModel>> CreateThreadsTagsAsync(List<ThreadsTagModel> threadsTags)
        {
            try
            {
                var mapped = _mapper.Map<List<ThreadsTag>>(threadsTags);
                var result = await _threadsTagRepository.CreateThreadsTagsAsync(mapped);

                return _mapper.Map<List<ThreadsTagModel>>(result);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<List<ThreadsTagModel>> CreateThreadsTagsAsync(HashSet<int> TagIds, int threadId)
        {
            try
            {
                List<ThreadsTagModel> list = new();

                foreach (var tagId in TagIds)
                {
                    ThreadsTagModel m = new()
                    {
                        TagId = tagId,
                        ThreadId = threadId
                    };

                    list.Add(m);
                }

                return await CreateThreadsTagsAsync(list);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<List<ThreadsTagModel>> UpdateThreadsTagsAsync(HashSet<int> TagIds, int threadId)
        {
            try
            {
                List<ThreadsTagModel> list = new();

                foreach (var tagId in TagIds)
                {
                    ThreadsTagModel m = new()
                    {
                        TagId = tagId,
                        ThreadId = threadId
                    };

                    list.Add(m);
                }

                return await UpdateThreadsTagsAsync(list, threadId);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<ThreadsTagModel> DeleteThreadsTagAsync(int id)
        {
            try
            {
                var result = await _threadsTagRepository.DeleteThreadsTagAsync(id);
                return _mapper.Map<ThreadsTagModel>(result);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<ThreadsTagModel> UpdateThreadsTagAsync(ThreadsTagModel threadsTag, int id)
        {
            try
            {
                var mapped = _mapper.Map<ThreadsTag>(threadsTag);
                var result = await _threadsTagRepository.UpdateThreadsTagAsync(mapped, id);
                return _mapper.Map<ThreadsTagModel>(result);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<List<ThreadsTagModel>> UpdateThreadsTagsAsync(List<ThreadsTagModel> threadsTags, int threadId)
        {
            try
            {
                var mapped = _mapper.Map<List<ThreadsTag>>(threadsTags);
                var result = await _threadsTagRepository.UpdateThreadsTagsAsync(mapped, threadId);
                return _mapper.Map<List<ThreadsTagModel>>(result);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
    }
}
