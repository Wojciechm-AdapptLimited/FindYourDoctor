namespace FindYourDoctor.Data.Domain;

public class Disease
{
    public string Icd { get; init; } = null!;

    public int SpecializationId { get; set; }

    public string Name { get; set; } = null!;

    public virtual IEnumerable<DiseaseHistory> DiseaseHistories { get; } = new List<DiseaseHistory>();

    public virtual Specialization Specialization { get; set; } = null!;

    public virtual ICollection<SymptomWeight> SymptomWeights { get; set; } = new List<SymptomWeight>();
    
    public override bool Equals(object? obj)
    {
        if ((obj == null) || !(GetType() == obj.GetType()))
        {
            return false;
        }

        return Icd == ((Disease)obj).Icd;
    }

    public override int GetHashCode()
    {
        return Icd.GetHashCode();
    }

    public override string ToString()
    {
        return Name;
    }
}
