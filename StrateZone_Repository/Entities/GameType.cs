using System;
using System.Collections.Generic;

namespace StrateZone_Repository.Entities;

public partial class GameType
{
    public int TypeId { get; set; }

    public string TypeName { get; set; }

    public virtual Image? Image { get; set; }

    public virtual ICollection<Table> Tables { get; set; } = new List<Table>();

    public virtual ICollection<Price> Prices { get; set; } = new List<Price>();
}
