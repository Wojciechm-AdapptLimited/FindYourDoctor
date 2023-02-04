namespace FindYourDoctor.Data.Domain;

public class SymptomWeight
{
    public string DiseaseIcd { get; set; } = null!;

    public int SymptomId { get; set; }

    public int Weight { get; set; }

    public virtual Disease Disease { get; set; } = null!;

    public virtual Symptom Symptom { get; set; } = null!;
}
