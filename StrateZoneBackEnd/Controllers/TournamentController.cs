using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StrateZone_Service.BusinessModels;
using StrateZone_Service.CustomModels.RequestModels;
using StrateZone_Service.Implements;
using StrateZone_Service.Interfaces;

namespace StrateZone_APIs.Controllers
{
    [Route("api/tournament")]
    [ApiController]
    public class TournamentController : ControllerBase
    {
        private readonly ITournamentService _tournamentService;

        public TournamentController(ITournamentService tournamentService)
        {
            _tournamentService = tournamentService;
        }

        [HttpPost("create-tournament")]
        public async Task<TournamentModel> CreateTournament([FromBody] CreateTournamentRequest createTounamentRequest, [FromForm] ImageRequest imageRequest /*temp cuz IFormFile doesnt work*/) 
        {
            try
            {
                var result = await _tournamentService.CreateTournamentAsync(createTounamentRequest);
                return result;
            }catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
