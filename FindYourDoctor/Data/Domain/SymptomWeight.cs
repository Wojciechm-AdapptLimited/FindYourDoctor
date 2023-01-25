using System;
using System.Collections.Generic;

namespace FindYourDoctor.Data.Domain;

public partial class SymptomWeight
{
    public string DiseaseIcd { get; set; } = null!;

    public int SymptomId { get; set; }

    public int Weight { get; set; }

    public virtual Disease DiseaseIcdNavigation { get; set; } = null!;

    public virtual Symptom Symptom { get; set; } = null!;
}
