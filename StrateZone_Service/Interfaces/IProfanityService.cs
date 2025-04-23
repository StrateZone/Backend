using MealHunt_Repositories.Pagination;
using StrateZone_Repository.Entities;
using StrateZone_Repository.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrateZone_Service.Interfaces
{
    public interface IProfanityService
    {
        Task<Profanity> AddAsync(string word);
        Task<bool> CheckContain(string content);
        Task DeleteAsync(int id);
        Task<PagedList<Profanity>> GetAllAsync(TablesAppointmentParameters parameters, string? searchValue);
        Task<Profanity?> GetByIdAsync(int id);
    }
}
