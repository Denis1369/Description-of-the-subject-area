using System;
using System.Collections.Generic;

namespace Kurs2;

public partial class Driver
{
    public uint Id { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public DateOnly? DateOfBirth { get; set; }

    public string? Nationality { get; set; }

    public string? Url { get; set; }

    public virtual ICollection<DriverConstructorAffiliation> DriverConstructorAffiliations { get; set; } = new List<DriverConstructorAffiliation>();

    public virtual ICollection<RaceResult> RaceResults { get; set; } = new List<RaceResult>();
}
