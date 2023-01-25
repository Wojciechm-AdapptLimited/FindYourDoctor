using System;
using System.Collections.Generic;

namespace FindYourDoctor.Data.Domain;

public partial class Doctor
{
    public int UserId { get; set; }

    public string PwzNumber { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string Surname { get; set; } = null!;

    public virtual ICollection<Opinion> Opinions { get; } = new List<Opinion>();

    public virtual User User { get; set; } = null!;

    public virtual ICollection<Clinic> Clinics { get; } = new List<Clinic>();

    public virtual ICollection<Patient> Patients { get; } = new List<Patient>();

    public virtual ICollection<Specialization> Specializations { get; } = new List<Specialization>();
}
