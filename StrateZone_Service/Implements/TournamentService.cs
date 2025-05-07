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
    public class TournamentService : ITournamentService
    {
        private readonly ITournamentRepository _tournamentRepository;

        public TournamentService(ITournamentRepository tournamentRepository)
        {
            _tournamentRepository = tournamentRepository;
        }

        /*
        public Task<Tournament> DeleteTournamentAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<List<Tournament>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Tournament> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<TournamentModel> CreateTournamentAsync(CreateTournamentRequest tournamentRequest)
        {
            throw new NotImplementedException();
        }

        public Task<Tournament> UpdateTournamentAsync(int id, Tournament tournament)
        {
            throw new NotImplementedException();
        }
        */
    }
}
