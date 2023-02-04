namespace FindYourDoctor.Data.Domain;

public class User
{
    public int Id { get; set; }
    
    public string? UserName { get; set; }
    
    public string? NormalizedUserName { get; set; }
    
    public string? Email { get; set; }
    
    public string? NormalizedEmail { get; set; }
    
    public string? PasswordHash { get; set; }
    
    public DateTime RegistrationTime { get; set; }

    public DateTime LastLoginTime { get; set; }

    public string? ConcurrencyStamp { get; set; }

    public int AccountType { get; set; }

    public virtual Account AccountTypeNavigation { get; set; } = null!;

    public virtual Doctor? Doctor { get; set; }

    public virtual Patient? Patient { get; set; }
}
