using FindYourDoctor.Data.Domain;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace FindYourDoctor.Components;

public partial class EditSpecialization
{
    private bool _success;
    private MudForm? _form;
    private string _name = string.Empty;
    
    [CascadingParameter] 
    public MudDialogInstance? MudDialog { get; set; }
    
    [Parameter] 
    public int? SpecializationId{ get; set; }

    protected override void OnInitialized()
    {
        base.OnInitialized();

        if (SpecializationId != null)
        {
            _name = DiseaseService.Specializations.Single(x => x.SpecializationId == SpecializationId).Name;
        }
    }
    
    private IEnumerable<string> ValidateName(string n)
    {
        if (string.IsNullOrWhiteSpace(n))
        {
            yield return "Specialization's name is required";
            yield break;
        }

        if (DiseaseService.Specializations.Any(u => u.Name == n))
            yield return "Specialization with that name already exists";
    }

    private void Submit()
    {
        if (string.IsNullOrWhiteSpace(_name))
            return;
        
        _form?.Validate();
        
        if (!_success)
            return;

        if (SpecializationId == null)
        {
            var specialization = new Specialization
            {
                Name = _name
            };
            
            DiseaseService.InsertSpecialization(specialization);
        }
        else
        {
            
            DiseaseService.UpdateSpecialization((int)SpecializationId, _name);
        }
        
        MudDialog?.Close(DialogResult.Ok(true));
    }

    private void Cancel() => MudDialog?.Cancel();
}