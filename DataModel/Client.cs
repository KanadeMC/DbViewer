using System;
using System.Collections.Generic;

namespace zalupa.DataModel;

public partial class Client
{
    public int Id { get; set; }

    public string Firstname { get; set; } = null!;

    public string Secondname { get; set; } = null!;

    public string? Patronymic { get; set; }

    public int Age { get; set; }

    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}
