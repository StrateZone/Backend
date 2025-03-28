using StrateZone_Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrateZone_Service.BusinessModels
{
    public class TournamentsParticipantsModel
    {
        public int? Id { get; set; }

        public int? TournamentId { get; set; }

        public int? ParticipantId { get; set; }

        public DateTime? CreatedAt { get; set; }

        public virtual TournamentModel? Tournament { get; set; }

        public virtual UserModel? Participant { get; set; }
    }
}
