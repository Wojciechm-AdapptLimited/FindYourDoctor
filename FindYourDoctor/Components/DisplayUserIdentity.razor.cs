using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;

namespace FindYourDoctor.Components;

public partial class DisplayUserIdentity
{
    private string? UserName { get; set; }
    
    private string? Email { get; set; }
    
    private DateTime LastLoginTime { get; set; }
    
    private DateTime RegistrationTime { get; set; }
    
    
    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        var authenticationState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
        var user = await UserManager.GetUserAsync(authenticationState.User);

        if (user != null)
        {
            UserName = user.UserName;
            Email = user.Email;
            LastLoginTime = user.LastLoginTime;
            RegistrationTime = user.RegistrationTime;
        }
    }
}