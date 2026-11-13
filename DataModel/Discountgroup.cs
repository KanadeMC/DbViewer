using System;
using System.Collections.Generic;

namespace zalupa.DataModel;

public partial class Discountgroup
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public decimal Agemin { get; set; }

    public decimal Agemax { get; set; }

    public virtual ICollection<Ticketdiscount> Ticketdiscounts { get; set; } = new List<Ticketdiscount>();
}
