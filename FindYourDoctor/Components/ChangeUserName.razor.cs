using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace FindYourDoctor.Components;

public partial class ChangeUserName
{
    private MudForm? _form;
    private bool _success;
    
    [Parameter] 
    public EventCallback Refresh { get; set; }
    private string? UserName { get; set; }
    private string? ConfirmUserName { get; set; }
    [GeneratedRegex("^[a-zA-Z0-9]+$")]
    private static partial Regex ValidCharacters();

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
    
    private string? ValidateConfirmUserName(string cun)
    {
        return UserName != cun ? "Usernames don't match!" : null;
    }

    private async void Submit()
    {
        if (UserName == null || ConfirmUserName == null)
            return;

        _form?.Validate();

        if (!_success)
            return;

        var authenticationState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
        var user = await UserManager.GetUserAsync(authenticationState.User);
        
        if (user == null)
            return;

        await UserStore.SetUserNameAsync(user, UserName, CancellationToken.None);
        var result = await UserManager.UpdateAsync(user);
        
        if (!result.Succeeded)
            return;

        _ = Refresh.InvokeAsync();
    }

    private void Cancel()
    {
        Refresh.InvokeAsync();
    }
}