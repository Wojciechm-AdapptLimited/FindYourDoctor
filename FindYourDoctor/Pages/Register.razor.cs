using System.Net.Mail;
using System.Text.RegularExpressions;
using FindYourDoctor.Data.Domain;
using FindYourDoctor.Services;
using Microsoft.AspNetCore.Identity;
using MudBlazor;

namespace FindYourDoctor.Pages;

public partial class Register
{
    private bool _success;
    private bool _passwordHidden = true;
    private bool _confirmPasswordHidden = true;
    private MudForm? _form;
    private InputType _passwordType = InputType.Password;
    private InputType _confirmPasswordType = InputType.Password;
    private string _passwordIcon = Icons.Material.Filled.VisibilityOff;
    private string _confirmPasswordIcon = Icons.Material.Filled.VisibilityOff;
    private IUserEmailStore<User>? _emailStore;
    private IUserRoleStore<User>? _userRoleStore;
    
    [GeneratedRegex("[A-Z]")]
    private static partial Regex CapitalCharacters();

    [GeneratedRegex("[a-z]")]
    private static partial Regex LowerCaseCharacters();
    
    [GeneratedRegex("[0-9]")]
    private static partial Regex Digits();

    [GeneratedRegex("^[a-zA-Z0-9]+$")]
    private static partial Regex ValidCharacters();

    private string? UserName { get; set; }

    private string? Email { get; set; }

    private string? Password { get; set; }
    
    private string? ConfirmPassword { get; set; }
    
    private string? AccountType { get; set; }

    protected override void OnInitialized()
    {
        base.OnInitialized();
        if (UserManager.SupportsUserEmail) _emailStore = (IUserEmailStore<User>)UserStore;
        if (UserManager.SupportsUserRole) _userRoleStore = (IUserRoleStore<User>)UserStore;
    }

    private IEnumerable<string> ValidateUserName(string un)
    {
        if (string.IsNullOrWhiteSpace(un))
        {
            yield return "Username is required";
            yield break;
        }

        if (!ValidCharacters().IsMatch(un))
        {
            yield return "Username must only be made of alphanumeric characters";
            yield break;
        }

        if (UserManager.Users.Any(u => u.UserName == un))
            yield return "User with that name already exists";
    }
    
    private IEnumerable<string> ValidateEmail(string em)
    {
        if (string.IsNullOrWhiteSpace(em))
        {
            yield return "Email is required";
            yield break;
        }

        string? message = null;

        try
        {
            _ = new MailAddress(em);
        }
        catch (FormatException)
        {
            message = "Enter valid email address";
        }

        if (!string.IsNullOrWhiteSpace(message))
        {
            yield return message;
            yield break;
        }
        
        if (UserManager.Users.Any(u => u.Email == em))
            yield return "User with that email already exists";
    }
    
    private static IEnumerable<string> ValidatePassword(string pw)
    {
        if (string.IsNullOrWhiteSpace(pw))
        {
            yield return "Password is required";
            yield break;
        }

        if (pw.Length < 8)
            yield return "Password must be at least of length 8";
        if (!CapitalCharacters().IsMatch(pw))
            yield return "Password must contain at least one capital letter";
        if (!LowerCaseCharacters().IsMatch(pw))
            yield return "Password must contain at least one lowercase letter";
        if (!Digits().IsMatch(pw))
            yield return "Password must contain at least one digit";
    }

    private string? ValidateConfirmPassword(string cpw)
    {
        return Password != cpw ? "Passwords don't match!" : null;
    }

    private void ShowPassword()
    {
        if (_passwordHidden)
        {
            _passwordHidden = false;
            _passwordType = InputType.Text;
            _passwordIcon = Icons.Material.Filled.Visibility;
        }
        else
        {
            _passwordHidden = true;
            _passwordType = InputType.Password;
            _passwordIcon = Icons.Material.Filled.VisibilityOff;
        }
    }
    
    private void ShowConfirmPassword()
    {
        if (_confirmPasswordHidden)
        {
            _confirmPasswordHidden = false;
            _confirmPasswordType = InputType.Text;
            _confirmPasswordIcon = Icons.Material.Filled.Visibility;
        }
        else
        {
            _confirmPasswordHidden = true;
            _confirmPasswordType = InputType.Password;
            _confirmPasswordIcon = Icons.Material.Filled.VisibilityOff;
        }
    }

    private async void Submit()
    {
        if (_emailStore == null || _userRoleStore == null || UserName == null || Password == null || AccountType == null) 
            return;

        _form?.Validate();

        if (!_success) 
            return;

        var user = Activator.CreateInstance<User>();

        await UserStore.SetUserNameAsync(user, UserName, CancellationToken.None);
        await _emailStore.SetEmailAsync(user, Email, CancellationToken.None);
        await _userRoleStore.AddToRoleAsync(user, AccountType, CancellationToken.None);
        var result = await UserManager.CreateAsync(user, Password);

        if (!result.Succeeded)
            return;
        
        var key = Guid.NewGuid();
        CookieMiddleware.Logins[key] = new LoginInfo { UserName = UserName, Password = Password };
        NavigationManager.NavigateTo($"/login_user?key={key}", true);
    }
}