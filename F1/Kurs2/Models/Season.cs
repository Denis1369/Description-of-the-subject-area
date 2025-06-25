using System;
using System.Collections.Generic;

namespace Kurs2;

public partial class Season
{
    public uint Id { get; set; }

    public int Year { get; set; }

    public virtual ICollection<Race> Races { get; set; } = new List<Race>();
}
