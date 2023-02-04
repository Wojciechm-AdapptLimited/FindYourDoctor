namespace FindYourDoctor.Data.Domain;

public class Symptom
{
    public int SymptomId { get; set; }

    public string Name { get; set; } = null!;

    public virtual IEnumerable<SymptomWeight> SymptomWeights { get; } = new List<SymptomWeight>();
}
