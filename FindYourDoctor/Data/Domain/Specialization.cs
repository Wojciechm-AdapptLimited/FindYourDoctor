using System;
using System.Collections.Generic;

namespace FindYourDoctor.Data.Domain;

public partial class Specialization
{
    public int SpecializationId { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Disease> Diseases { get; } = new List<Disease>();

    public virtual ICollection<Doctor> Doctors { get; } = new List<Doctor>();
}
