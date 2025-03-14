using StrateZone_Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class TournamentsParticipants
{
    public int? Id { get; set; }

    public int? TournamentId { get; set; }

    public int? ParticipantId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Tournament? Tournament {  get; set; }

    public virtual User? Participant {  get; set; }
}
