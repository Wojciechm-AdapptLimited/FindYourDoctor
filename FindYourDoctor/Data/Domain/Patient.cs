namespace FindYourDoctor.Data.Domain;

public class Patient
{
    public int UserId { get; set; }
    
    public string? Name { get; set; }
    
    public string? Surname { get; set; }

    public string? InsuranceNumber { get; set; }

    public virtual ICollection<DiseaseHistory> DiseaseHistories { get; set; } = new List<DiseaseHistory>();

    public virtual ICollection<Opinion> Opinions { get; set; } = new List<Opinion>();

    public virtual User User { get; set; } = null!;

    public virtual ICollection<Doctor> FavouriteDoctors { get; set; } = new List<Doctor>();
}
