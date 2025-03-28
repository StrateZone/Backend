using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrateZone_Repository.Interfaces
{
    public interface ITournamentRepository
    {
        Task<Tournament> CreateTournamentAsync(Tournament tournament);
        Task<List<Tournament>> GetAllAsync();
        Task<Tournament> GetByIdAsync(int id);
        Task<Tournament> UpdateTournamentAsync(int id, Tournament tournament);
        Task<Tournament> DeleteTournamentAsync(int id);
    }
}
