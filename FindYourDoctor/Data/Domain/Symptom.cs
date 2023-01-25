using System;
using System.Collections.Generic;

namespace FindYourDoctor.Data.Domain;

public partial class Symptom
{
    public int SymptomId { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<SymptomWeight> SymptomWeights { get; } = new List<SymptomWeight>();
}
