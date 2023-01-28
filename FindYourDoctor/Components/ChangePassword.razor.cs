using System.Text.RegularExpressions;
using FindYourDoctor.Data.Domain;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using MudBlazor;

namespace FindYourDoctor.Components;

public partial class ChangePassword
{
    private MudForm? _form;
    private bool _success;

    [Parameter] 
    public EventCallback Refresh { get; set; }
    private string? CurrentPassword { get; set; }
    private string? Password { get; set; }
    private string? ConfirmPassword { get; set; }
    
    [GeneratedRegex("[A-Z]")]
    private static partial Regex CapitalCharacters();

    [GeneratedRegex("[a-z]")]
    private static partial Regex LowerCaseCharacters();
    
    [GeneratedRegex("[0-9]")]
    private static partial Regex Digits();
    private async Task<string?> ValidateCurrentPassword(string cpw)
    {
        var authenticationState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
        var user = await UserManager.GetUserAsync(authenticationState.User);

        if (user == null || CurrentPassword == null)
        {
            return "Password is required";
        }

        var checkPasswordAsync = await UserManager.CheckPasswordAsync(user, cpw);

        return !checkPasswordAsync ? "Invalid password" : null;
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
        return Password != cpw ? "Password don't match!" : null;
    }

    private async void Submit()
    {
        if (Password == null || CurrentPassword == null)
            return;

        _form?.Validate();

        if (!_success)
            return;

        var authenticationState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
        var user = await UserManager.GetUserAsync(authenticationState.User);
        
        if (user == null)
            return;

        await UserManager.ChangePasswordAsync(user, CurrentPassword, Password);
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