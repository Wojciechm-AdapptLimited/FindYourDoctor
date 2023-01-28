namespace FindYourDoctor.Pages;

public partial class ManageAccount
{
    private bool _changeEmailShown;
    private bool _changePasswordShown;
    private bool _changeUserNameShown;

    private void ShowChangeEmail()
    {
        _changeEmailShown = true;
        _changePasswordShown = false;
        _changeUserNameShown = false;
    }

    private void ShowChangePassword()
    {
        _changeEmailShown = false;
        _changePasswordShown = true;
        _changeUserNameShown = false;
    }
    
    private void ShowChangeUserName()
    {
        _changeEmailShown = false;
        _changePasswordShown = false;
        _changeUserNameShown = true;
    }

    private void Refresh()
    {
        _changeEmailShown = false;
        _changePasswordShown = false;
        _changeUserNameShown = false;
        StateHasChanged();
    }
}