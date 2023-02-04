using FindYourDoctor.Components;
using MudBlazor;

namespace FindYourDoctor.Pages;

public partial class ManageAccounts
{
    private List<AccountTableObject> _accounts = new();
    private string? _searchString;

    private class AccountTableObject
    {
        public int Id { get; init; }
        public string Name { get; init; } = string.Empty;
    }


    protected override void OnInitialized()
    {
        base.OnInitialized();
        SetAccounts();
    }

    private void SetAccounts()
    {
        _accounts = RoleManager.Roles
            .Select(x => new AccountTableObject
            {
                Id = x.Id, Name = x.Name ?? string.Empty
            })
            .OrderBy(x => x.Id)
            .ToList();
    }

    private async void AddAccount()
    {
        var parameters = new DialogParameters
        {
            { "AccountId", null }
        };

        var options = new DialogOptions
        {
            DisableBackdropClick = true
        };

        var logicDialog = await DialogService.ShowAsync<EditAccount>("Add account type", parameters, options);

        _ = await logicDialog.Result;
        
        SetAccounts();
        
        StateHasChanged();
    }
    
    private async void EditAccount(int accountId)
    {
        var parameters = new DialogParameters
        {
            { "AccountId", accountId }
        };

        var options = new DialogOptions
        {
            DisableBackdropClick = true
        };

        var logicDialog = await DialogService.ShowAsync<EditAccount>("Edit account type", parameters, options);

        _ = await logicDialog.Result;
        
        SetAccounts();
        
        StateHasChanged();
    }
    
    private async void DeleteAccount(int accountId)
    {
        var parameters = new DialogParameters
        {
            { "ContentText", "Do you really want to delete these records? This process cannot be undone." },
            { "ButtonText", "Delete" },
            { "Color", Color.Error }
        };

        var options = new DialogOptions
        {
            DisableBackdropClick = true
        };

        var logicDialog = await DialogService.ShowAsync<InfoDialog>("Delete account type", parameters, options);

        var result = await logicDialog.Result;

        if (result.Canceled)
            return;

        var account = await RoleManager.FindByIdAsync(accountId.ToString());
        
        if (account == null) 
            return;
        
        await RoleManager.DeleteAsync(account);
        
        SetAccounts();

        StateHasChanged();
    }
    
    private bool FilterSpecializations(AccountTableObject specialization) => 
        string.IsNullOrWhiteSpace(_searchString) || specialization.Name.Contains(_searchString, StringComparison.CurrentCultureIgnoreCase);
}