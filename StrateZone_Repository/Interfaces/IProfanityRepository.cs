using MealHunt_Repositories.Pagination;
using StrateZone_Repository.Entities;
using StrateZone_Repository.Parameters;

namespace StrateZone_Repository.Interfaces
{
    public interface IProfanityRepository
    {
        Task<Profanity> AddAsync(Profanity profanity);
        Task<bool> CheckContain(string content);
        Task DeleteAsync(int id);
        Task<PagedList<Profanity>> GetAllAsync(TablesAppointmentParameters parameters, string? searchValue);
        Task<Profanity?> GetByIdAsync(int id);
    }
}