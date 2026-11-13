using System;
using System.Collections.Generic;

namespace zalupa.DataModel;

public partial class Ticket
{
    public int Id { get; set; }

    public int? Clientid { get; set; }

    public int? Sessionid { get; set; }

    public int Seat { get; set; }

    public virtual Client? Client { get; set; }

    public virtual Session? Session { get; set; }
}
