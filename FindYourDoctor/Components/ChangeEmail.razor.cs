using System.Net.Mail;
using FindYourDoctor.Data.Domain;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using MudBlazor;

namespace FindYourDoctor.Components;

public partial class ChangeEmail
{
    private MudForm? _form;
    private bool _success;
    private IUserEmailStore<User>? _emailStore;
    
    [Parameter] 
    public EventCallback Refresh { get; set; }
    private string? Email { get; set; }
    private string? ConfirmEmail { get; set; }

    protected override void OnInitialized()
    {
        base.OnInitialized();
        if (UserManager.SupportsUserEmail) _emailStore = (IUserEmailStore<User>)UserStore;
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
    
    private string? ValidateConfirmEmail(string cem)
    {
        return Email != cem ? "Emails don't match!" : null;
    }

    private async void Submit()
    {
        if (_emailStore == null || Email == null || ConfirmEmail == null)
            return;

        _form?.Validate();

        if (!_success)
            return;

        var authenticationState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
        var user = await UserManager.GetUserAsync(authenticationState.User);
        
        if (user == null)
            return;

        await _emailStore.SetEmailAsync(user, Email, CancellationToken.None);
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