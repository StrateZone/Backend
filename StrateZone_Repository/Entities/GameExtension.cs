using System;
using System.Collections.Generic;

namespace StrateZone_Repository.Entities;

public partial class GameExtension
{
    public int ExtensionId { get; set; }

    public int? TypeId { get; set; }

    public virtual ICollection<Table> Tables { get; set; } = new List<Table>();

    public virtual GameType? Type { get; set; }
}
