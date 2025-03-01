using System;
using System.Collections.Generic;

namespace StrateZone_Repository.Entities;

public partial class ThreadsTag
{
    public int Id { get; set; }

    public int? ThreadId { get; set; }

    public int? TagId { get; set; }

    public virtual Tag? Tag { get; set; }

    public virtual Thread? Thread { get; set; }
}
