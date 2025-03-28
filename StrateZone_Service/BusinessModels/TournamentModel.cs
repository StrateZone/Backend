using StrateZone_Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static StrateZone_Repository.Parameters.PostgreEnums;

namespace StrateZone_Service.BusinessModels
{
    public class TournamentModel
    {
        public int TournamentId { get; set; }

        public int? UserId { get; set; }

        public string? Name { get; set; }

        public string? Description { get; set; }

        public Ranking TargetedRanking { get; set; }

        public int? MaxParticipants { get; set; }

        public DateOnly? StartDate { get; set; }

        public DateOnly? EndDate { get; set; }

        public TournamentStatus Status { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public virtual ImageModel? Image { get; set; }

        public virtual UserModel? User { get; set; }

        public virtual ICollection<TournamentsParticipantsModel> TournamentsParticipants { get; set; } = new List<TournamentsParticipantsModel>();
    }
}
