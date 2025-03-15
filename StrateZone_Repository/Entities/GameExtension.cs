using System;
using System.Collections.Generic;

namespace StrateZone_Repository.Entities;

public partial class GameExtension
{
    public int ExtensionId { get; set; }

    public int? TypeId { get; set; }

    public short NumberOfPlayers { get; set; }

    public Parameters.PostgreEnums.GameExtensionEnum ExtensionName { get; set; }

    public virtual ICollection<TablesAppointment> TablesAppointments { get; set; } = new List<TablesAppointment>();

    public virtual GameType? Type { get; set; }
}
