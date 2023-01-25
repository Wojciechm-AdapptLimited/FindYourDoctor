using MudBlazor;

namespace FindYourDoctor.Shared;

public partial class MainLayout
{
    private bool _drawerOpen = true;
    private bool _isDarkTheme = true;
    private readonly MudTheme _theme = new();

    private void DrawerToggle()
    {
        _drawerOpen = !_drawerOpen;
    }
}