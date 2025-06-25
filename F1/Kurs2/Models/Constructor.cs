using System;
using System.Collections.Generic;

namespace Kurs2;

public partial class Constructor
{
    public uint Id { get; set; }

    public string ConstructorName { get; set; } = null!;

    public string? Nationality { get; set; }

    public string? DirectorLastName { get; set; }

    public string? DirectorName { get; set; }

    public string? Url { get; set; }

    public virtual ICollection<DriverConstructorAffiliation> DriverConstructorAffiliations { get; set; } = new List<DriverConstructorAffiliation>();

    public virtual ICollection<RaceResult> RaceResults { get; set; } = new List<RaceResult>();
}
