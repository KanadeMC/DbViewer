using System;
using System.Collections.Generic;

namespace zalupa.DataModel;

public partial class Ticketdiscount
{
    public int Id { get; set; }

    public int? Discountgroupid { get; set; }

    public decimal Discountpercent { get; set; }

    public virtual Discountgroup? Discountgroup { get; set; }
}
