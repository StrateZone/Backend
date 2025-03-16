using StrateZone_Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static StrateZone_Repository.Parameters.PostgreEnums;

public class Tournament
{
    public int TournamentId { get; set; }

    public int? UserId { get; set; }

    public string? Name { get; set; }

    public string? Description { get; set; }

    public Ranking TargetedRanking { get; set; } 

    public int? MaxParticipants {  get; set; }

    public DateOnly? StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    public EventStatus Status { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Image? Image { get; set; }

    public virtual User? User { get; set; }

    public virtual ICollection<TournamentsParticipants> TournamentsParticipants { get; set; } = new List<TournamentsParticipants>();
}
