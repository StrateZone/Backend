using System;
using System.Collections.Generic;

namespace StrateZone_Repository.Entities;

public partial class GameType
{
    public int TypeId { get; set; }

    public Parameters.PostgreEnums.GameType TypeName { get; set; }

    public virtual ICollection<GameExtension> GameExtensions { get; set; } = new List<GameExtension>();

    public virtual ICollection<Price> Prices { get; set; } = new List<Price>();
}
