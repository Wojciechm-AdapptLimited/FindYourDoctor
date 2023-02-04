using FindYourDoctor.Data.Domain;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace FindYourDoctor.Components;

public partial class EditAccount
{
    private bool _success;
    private MudForm? _form;
    private string _name = string.Empty;
    
    [CascadingParameter] 
    public MudDialogInstance? MudDialog { get; set; }
    
    [Parameter] 
    public int? AccountId{ get; set; }

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        if (AccountId == null) return;
        
        var account = await RoleManager.FindByIdAsync(AccountId.ToString() ?? string.Empty);
        
        if (account == null) return;
        
        _name = await RoleManager.GetRoleNameAsync(account) ?? string.Empty;
    }
    
    private IEnumerable<string> ValidateName(string n)
    {
        if (string.IsNullOrWhiteSpace(n))
        {
            yield return "Account Type's name is required";
            yield break;
        }

        if (RoleManager.Roles.Any(u => u.Name == n))
            yield return "Account Type with that name already exists";
    }

    private async void Submit()
    {
        if (string.IsNullOrWhiteSpace(_name))
            return;
        
        _form?.Validate();
        
        if (!_success)
            return;

        if (AccountId == null)
        {
            await RoleManager.CreateAsync(new Account { Name = _name });
        }
        else
        {
            var account = await RoleManager.FindByIdAsync(AccountId.ToString() ?? string.Empty);
        
            if (account == null) return;

            await RoleManager.SetRoleNameAsync(account, _name);
            
            var result = await RoleManager.UpdateAsync(account);
        
            if (!result.Succeeded)
                return;
        }
        
        MudDialog?.Close(DialogResult.Ok(true));
    }

    private void Cancel() => MudDialog?.Cancel();
}