namespace FindYourDoctor.Data.Domain;

public class Clinic
{
    public int ClinicId { get; set; }

    public string Name { get; set; } = null!;

    public string Voivodeship { get; set; } = null!;

    public string FullAddress { get; set; } = null!;

    public string? PhoneNumber { get; set; }

    public virtual IEnumerable<Doctor> Doctors { get; } = new List<Doctor>();
}
