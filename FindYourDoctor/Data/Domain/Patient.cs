using System;
using System.Collections.Generic;

namespace FindYourDoctor.Data.Domain;

public partial class Patient
{
    public int UserId { get; set; }

    public string InsuranceNumber { get; set; } = null!;

    public virtual ICollection<DiseaseHistory> DiseaseHistories { get; } = new List<DiseaseHistory>();

    public virtual ICollection<Opinion> Opinions { get; } = new List<Opinion>();

    public virtual User User { get; set; } = null!;

    public virtual ICollection<Doctor> Doctors { get; } = new List<Doctor>();
}
