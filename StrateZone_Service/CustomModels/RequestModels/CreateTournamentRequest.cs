using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static StrateZone_Repository.Parameters.PostgreEnums;

namespace StrateZone_Service.CustomModels.RequestModels
{
    public class CreateTournamentRequest
    {
        public string? Name { get; set; }

        public string? Description { get; set; }

        public Ranking TargetedRanking { get; set; }

        public int? MaxParticipants { get; set; }

        public DateOnly? StartDate { get; set; }

        public DateOnly? EndDate { get; set; }
    }
}
