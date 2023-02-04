using Microsoft.EntityFrameworkCore;

namespace FindYourDoctor.Pages;

public partial class ManageUsers
{
    private List<UserTableObject> _users = new();
    private string? _searchString;
    private string _searchType = "Username";

    private class UserTableObject
    {
        public int Id { get; init; }
        public string UserName { get; init; } = string.Empty;
        public string Email { get; init; } = string.Empty;
        public string Account { get; init; } = string.Empty;
        public DateTime LastLoginDate { get; init; }
        public DateTime RegisterDate { get; init; }
    }
    
    
    protected override void OnInitialized()
    {
        base.OnInitialized();
        _users = UserManager.Users
            .Include(x => x.AccountTypeNavigation)
            .Select(x => new UserTableObject
            {
                Id = x.Id, UserName = x.UserName ?? string.Empty, Email = x.Email ?? string.Empty, Account = x.AccountTypeNavigation.Name ?? string.Empty,
                RegisterDate = x.RegistrationTime, LastLoginDate = x.LastLoginTime
            })
            .OrderBy(x => x.Id)
            .ToList();
    }

    private bool FilterUsers(UserTableObject user)
    {
        if (string.IsNullOrWhiteSpace(_searchString))
            return true;
        
        return _searchType switch
        {
            "Username" when user.UserName.Contains(_searchString, StringComparison.CurrentCultureIgnoreCase) => true,
            "Email" when user.Email.Contains(_searchString, StringComparison.CurrentCultureIgnoreCase) => true,
            "Account Type" when user.Account.Contains(_searchString, StringComparison.CurrentCultureIgnoreCase) => true,
            _ => false
        };
    }
}