using FindYourDoctor.Data.Domain;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace FindYourDoctor.Components;

public partial class EditSymptom
{
    private bool _success;
    private MudForm? _form;
    private string _name = string.Empty;
    
    [CascadingParameter] 
    public MudDialogInstance? MudDialog { get; set; }
    
    [Parameter] 
    public int? SymptomId{ get; set; }

    protected override void OnInitialized()
    {
        base.OnInitialized();

        if (SymptomId != null)
        {
            _name = DiseaseService.Symptoms.Single(x => x.SymptomId == SymptomId).Name;
        }
    }
    
    private IEnumerable<string> ValidateName(string n)
    {
        if (string.IsNullOrWhiteSpace(n))
        {
            yield return "Symptom's name is required";
            yield break;
        }

        if (DiseaseService.Symptoms.Any(u => u.Name == n))
            yield return "Symptom with that name already exists";
    }

    private void Submit()
    {
        if (string.IsNullOrWhiteSpace(_name))
            return;
        
        _form?.Validate();
        
        if (!_success)
            return;

        if (SymptomId == null)
        {
            var symptom = new Symptom
            {
                Name = _name
            };
            
            DiseaseService.InsertSymptom(symptom);
        }
        else
        {
            DiseaseService.UpdateSymptom((int)SymptomId, _name);
        }
        
        MudDialog?.Close(DialogResult.Ok(true));
    }

    private void Cancel() => MudDialog?.Cancel();
}