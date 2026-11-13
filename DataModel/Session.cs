using System;
using System.Collections.Generic;

namespace zalupa.DataModel;

public partial class Session
{
    public int Id { get; set; }

    public int? Filmid { get; set; }

    public int? Hallid { get; set; }

    public decimal Baseticketcost { get; set; }

    public virtual Film? Film { get; set; }

    public virtual Hall? Hall { get; set; }

    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}
