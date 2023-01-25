using System;
using System.Collections.Generic;

namespace FindYourDoctor.Data.Domain;

public partial class Clinic
{
    public int ClinicId { get; set; }

    public string Name { get; set; } = null!;

    public string Voivodeship { get; set; } = null!;

    public string FullAddress { get; set; } = null!;

    public string? PhoneNumber { get; set; }

    public virtual ICollection<Doctor> Doctors { get; } = new List<Doctor>();
}
