using FindYourDoctor.Services;
using MudBlazor;

namespace FindYourDoctor.Pages;

public partial class Login
{
    private bool _success;
    private bool _passwordHidden = true;
    private bool _isPersistent;
    private MudForm? _form;
    private InputType _passwordType = InputType.Password;
    private string _passwordIcon = Icons.Material.Filled.VisibilityOff;
    
    private string? UserName { get; set; }
    
    private string? Password { get; set; }
    
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
    
    private async void Submit()
    {
        if (UserName == null || Password == null) 
            return;

        _form?.Validate();

        if (!_success) 
            return;
        
        Snackbar.Clear();
        Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
        
        var user = await UserManager.FindByNameAsync(UserName);

        if (user == null)
        {
            Snackbar.Add("Invalid username. User not found", Severity.Error);
            StateHasChanged();
            return;
        }

        var result = await SignInManager.CheckPasswordSignInAsync(user, Password, false);

        if (!result.Succeeded)
        {
            Snackbar.Add("Invalid password. Try again", Severity.Error);
            StateHasChanged();
            return;
        }
        
        user.LastLoginTime = DateTime.Now.ToUniversalTime();
        await UserManager.UpdateAsync(user);

        var key = Guid.NewGuid();
        CookieMiddleware.Logins[key] = new LoginInfo { UserName = UserName, Password = Password, IsPersistent = _isPersistent};
        NavigationManager.NavigateTo($"/login_user?key={key}", true);
    }
}