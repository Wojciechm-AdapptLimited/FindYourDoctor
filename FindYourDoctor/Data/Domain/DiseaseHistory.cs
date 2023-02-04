namespace FindYourDoctor.Data.Domain;

public class DiseaseHistory
{
    public DateOnly IllnessDate { get; set; }

    public int PatientId { get; set; }

    public string DiseaseIcd { get; set; } = null!;

    public virtual Disease Disease { get; set; } = null!;

    public virtual Patient Patient { get; set; } = null!;
}
