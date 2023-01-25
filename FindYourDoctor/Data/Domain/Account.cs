namespace FindYourDoctor.Data.Domain;

public class Account
{
    public int Id { get; set; }

    public string? Name { get; set; }
    
    public string? NormalizedName { get; set; }

    public string? ConcurrencyStamp { get; set; }

    public virtual ICollection<User> Users { get; } = new List<User>();
}
