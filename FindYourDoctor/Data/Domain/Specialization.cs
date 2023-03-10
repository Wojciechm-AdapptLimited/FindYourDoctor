namespace FindYourDoctor.Data.Domain;

public class Specialization
{
    public int SpecializationId { get; set; }

    public string Name { get; set; } = null!;

    public virtual IEnumerable<Disease> Diseases { get; } = new List<Disease>();

    public virtual ICollection<Doctor> Doctors { get; } = new List<Doctor>();
}
