using System;
using System.Collections.Generic;

namespace FindYourDoctor.Data.Domain;

public partial class DiseaseHistory
{
    public DateOnly IllnessDate { get; set; }

    public int PatientId { get; set; }

    public string DiseaseIcd { get; set; } = null!;

    public virtual Disease DiseaseIcdNavigation { get; set; } = null!;

    public virtual Patient Patient { get; set; } = null!;
}
