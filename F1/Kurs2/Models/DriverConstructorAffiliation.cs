using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kurs2;

public partial class DriverConstructorAffiliation
{
    public uint Id { get; set; }

    public uint DriverId { get; set; }

    public uint ConstructorId { get; set; }

    public DateOnly StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    public virtual Constructor Constructor { get; set; } = null!;

    public virtual Driver Driver { get; set; } = null!;

    [NotMapped]
    public DateTime StartDateTime
    {
        get => StartDate.ToDateTime(TimeOnly.MinValue);
        set => StartDate = DateOnly.FromDateTime(value);
    }

    [NotMapped]
    public DateTime? EndDateTime
    {
        get => EndDate.HasValue? EndDate.Value.ToDateTime(TimeOnly.MinValue) : null;
        set => EndDate = value.HasValue? DateOnly.FromDateTime(value.Value) : null;
    }

    public static void Add(uint driverId, uint constructorId, DateTime startDate) 
    {
        var context = new Formula12025Context();

        context.DriverConstructorAffiliations.Add(
            new DriverConstructorAffiliation
            {
                DriverId = driverId,
                ConstructorId = constructorId,
                StartDate = DateOnly.FromDateTime(startDate)
            });
        context.SaveChanges();
    }
}
