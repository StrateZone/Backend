using System;
using System.Collections.Generic;

namespace StrateZone_Repository.Entities;

public partial class Tag
{
    public int TagId { get; set; }

    public string? TagName { get; set; }

    public virtual ICollection<ProductTag> ProductTags { get; set; } = new List<ProductTag>();

    public virtual ICollection<ThreadsTag> ThreadsTags { get; set; } = new List<ThreadsTag>();
}
