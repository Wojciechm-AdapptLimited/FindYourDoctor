namespace FindYourDoctor.Data.Domain;

public class Doctor
{
    public int UserId { get; set; }

    public string PwzNumber { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string Surname { get; set; } = null!;

    public virtual IEnumerable<Opinion> Opinions { get; } = new List<Opinion>();

    public virtual User User { get; set; } = null!;

    public virtual ICollection<Clinic> Clinics { get; set; } = new List<Clinic>();

    public virtual IEnumerable<Patient> Patients { get; } = new List<Patient>();

    public virtual ICollection<Specialization> Specializations { get; set; } = new List<Specialization>();
}
