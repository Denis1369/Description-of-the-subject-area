using System;
using System.Collections.Generic;

namespace Kurs2;

public partial class Circuit
{
    public uint Id { get; set; }

    public string CircuitName { get; set; } = null!;

    public string? Location { get; set; }

    public string? Country { get; set; }

    public virtual ICollection<Race> Races { get; set; } = new List<Race>();
}
