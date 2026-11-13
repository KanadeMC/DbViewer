using System;
using System.Collections.Generic;

namespace zalupa.DataModel;

public partial class Hall
{
    public int Id { get; set; }

    public int Hallcount { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Session> Sessions { get; set; } = new List<Session>();
}
