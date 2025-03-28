using StrateZone_Service.BusinessModels;
using StrateZone_Service.CustomModels.RequestModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrateZone_Service.Implements
{
    public class TournamentService : ITournamentService
    {
        private readonly ITournamentService _tournamentService;

        public TournamentService(ITournamentService tournamentService)
        {
            _tournamentService = tournamentService;
        }


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
    }
}
