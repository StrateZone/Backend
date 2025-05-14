using StrateZone_Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static StrateZone_Repository.Parameters.PostgreEnums;

public partial class Appointmentrequest
{
    public int Id { get; set; }

    public int FromUser { get; set; }

    public int ToUser { get; set; }

    public int TableId { get; set; }

    public int? AppointmentId { get; set; }

    public decimal? TotalPrice { get; set; }

    public bool IsPaid { get; set; }

    public RequestStatus Status { get; set; }

    public DateTime? StartTime { get; set; }

    public  DateTime? EndTime { get; set; }

    public DateTime? ExpireAt { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual User FromUserNavigation { get; set; } = null!;

    public virtual User ToUserNavigation { get; set; } = null!;

    public virtual Table Table { get; set; } = null!;

    public virtual Appointment? Appointment { get; set; }
}
