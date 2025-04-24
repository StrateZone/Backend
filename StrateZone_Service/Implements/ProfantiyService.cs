using StrateZone_Repository.Pagination;
using StrateZone_Repository.Entities;
using StrateZone_Repository.Interfaces;
using StrateZone_Repository.Parameters;
using StrateZone_Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrateZone_Service.Implements
{
    public class ProfanityService : IProfanityService
    {
        private readonly IProfanityRepository _repository;

        public ProfanityService(IProfanityRepository repository)
        {
            _repository = repository;
        }

        public async Task<Profanity> AddAsync(string word)
        {
            var profanity = new Profanity { Word = word };
            return await _repository.AddAsync(profanity);
        }

        public async Task<bool> CheckContain(string content)
        {
            return await _repository.CheckContain(content);
        }

        public async Task DeleteAsync(int id)
        {
            await _repository.DeleteAsync(id);
        }

        public async Task<PagedList<Profanity>> GetAllAsync(TablesAppointmentParameters parameters, string? searchValue)
        {
            var result = await _repository.GetAllAsync(parameters, searchValue);
            return new PagedList<Profanity>(result, result.TotalCount, result.CurrentPage, result.PageSize);
        }

        public async Task<Profanity?> GetByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }
    }
}
