using MudBlazor;

namespace FindYourDoctor.Pages;

public partial class Index
{
    protected override void OnAfterRender(bool firstRender)
    {
        if (!firstRender)
        {
            return;
        }
        
        if (NavigationManager.Uri.ToLower().Contains("loginfailed"))
        {
            Snackbar.Add("Login failed", Severity.Error);
        }
    }
}