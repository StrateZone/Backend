using System;
using System.Collections.Generic;
using static StrateZone_Repository.Parameters.PostgreEnums;

namespace StrateZone_Repository.Entities;

public partial class Payment
{
    public int Id { get; set; }

    public int? UserId { get; set; }

    public PaymentType PaymentType { get; set; } 

    public PaymentStatus PaymentStatus { get; set; }

    public int? OrderId { get; set; }

    public int? TablesAppointmentId { get; set; }

    public int? CourseId { get; set; }

    public string? Description { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual User? User { get; set; }

    public virtual Order? Order { get; set; }

    public virtual TablesAppointment? TablesAppointment { get; set; }

    public virtual Course? Course { get; set; }
}
