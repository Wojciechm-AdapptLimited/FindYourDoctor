using FindYourDoctor.Data.Domain;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using MudBlazor;

namespace FindYourDoctor.Components;

public partial class EditDisease
{
    private bool _success;
    private bool _icdReadOnly;
    private MudForm? _form;
    private string _searchString = string.Empty;
    private string _name = string.Empty;
    private string _specialization = string.Empty;
    private List<SymptomWeightTableObject> _symptomWeights = new();
    private SymptomWeightTableObject _selectedSymptom= new();
    private readonly SymptomWeightTableObject _symptomBeforeEdit = new();
    private int _symptomWeight;
    private string _symptomName = string.Empty;

    private List<string> _specializations = new();
    private List<string> _symptoms = new();
    
    [CascadingParameter] 
    public MudDialogInstance? MudDialog { get; set; }
    
    [Parameter] 
    public string? DiseaseIcd{ get; set; }
    
    private class SymptomWeightTableObject
    {
        public string SymptomName { get; set; } = string.Empty;
        public int Weight { get; set; }
    }

    protected override void OnInitialized()
    {
        base.OnInitialized();
        _specializations = DiseaseService.Specializations.Select(x => x.Name).ToList();
        _symptoms = DiseaseService.Symptoms.Select(x => x.Name).ToList();

        if (DiseaseIcd == null) return;

        var disease = DiseaseService.Diseases
            .Include(x => x.Specialization)
            .Include(x => x.SymptomWeights)
            .SingleOrDefault(x => x.Icd == DiseaseIcd);
        
        if (disease == null) return;

        _icdReadOnly = true;
        _name = disease.Name;
        _specialization = disease.Specialization.Name;
        _symptomWeights = DiseaseService.SymptomWeights
            .Include(x => x.Symptom)
            .Where(x => x.DiseaseIcd == DiseaseIcd)
            .Select(x => new SymptomWeightTableObject
            {
                SymptomName = x.Symptom.Name, Weight = x.Weight
            })
            .ToList();
    }
    
    private IEnumerable<string> ValidateIcd(string icd)
    {
        if (string.IsNullOrWhiteSpace(icd))
        {
            yield return "Disease's icd is required";
            yield break;
        }

        if (_icdReadOnly == false && DiseaseService.Diseases.Any(u => u.Icd == icd))
            yield return "Disease with that icd already exists";
    }
    
    private IEnumerable<string> ValidateName(string n)
    {
        if (string.IsNullOrWhiteSpace(n))
        {
            yield return "Disease's name is required";
            yield break;
        }

        if (DiseaseService.Diseases.Any(u => u.Name == n && u.Icd != DiseaseIcd))
            yield return "Disease with that name already exists";
    }

    private bool FilterSymptomWeights(SymptomWeightTableObject symptomWeight) =>
        string.IsNullOrWhiteSpace(_searchString) || symptomWeight.SymptomName.Contains(_searchString);
    
    private void BackupSymptom(object symptomObj)
    {
        var symptom = (SymptomWeightTableObject)symptomObj;
        _symptomBeforeEdit.SymptomName = symptom.SymptomName;
        _symptomBeforeEdit.Weight = symptom.Weight;
    }

    private void ResetSymptom(object symptomObj)
    {
        var symptom = (SymptomWeightTableObject)symptomObj;
        symptom.SymptomName = _symptomBeforeEdit.SymptomName;
        symptom.Weight = _symptomBeforeEdit.Weight;
    }

    private void UpdateSymptom(object symptomObj)
    {
        var symptom = (SymptomWeightTableObject)symptomObj;
        _symptomWeights.First(x => x.SymptomName == symptom.SymptomName).Weight = symptom.Weight;
    }

    private void RemoveSymptom(SymptomWeightTableObject symptom)
    {
        _symptomWeights.Remove(symptom);
        _symptoms.Add(symptom.SymptomName);
        StateHasChanged();
    }

    private void AddSymptom()
    {
        if (string.IsNullOrEmpty(_symptomName))
            return;

        var symptom = new SymptomWeightTableObject()
        {
            SymptomName = _symptomName,
            Weight = _symptomWeight,
        };

        _symptomWeights.Add(symptom);
        _symptoms.Remove(symptom.SymptomName);
        StateHasChanged();
    }
    
    private void Submit()
    {
        if (string.IsNullOrWhiteSpace(_name) || string.IsNullOrWhiteSpace(DiseaseIcd) || string.IsNullOrWhiteSpace(_specialization))
            return;
        
        _form?.Validate();
        
        if (!_success)
            return;

        var symptomWeights = _symptomWeights.Select(x => new SymptomWeight
        {
            DiseaseIcd = DiseaseIcd, 
            Weight = x.Weight, 
            SymptomId = DiseaseService.Symptoms.Single(y => y.Name == x.SymptomName).SymptomId
        }).ToList();

        var specialization = DiseaseService.Specializations.Single(x => x.Name == _specialization);

        if (!DiseaseService.Diseases.Any(x => x.Icd == DiseaseIcd))
        {
            var disease = new Disease
            {
                Icd = DiseaseIcd,
                Name = _name,
                Specialization = specialization,
                SymptomWeights = symptomWeights
            };
            
            DiseaseService.InsertDisease(disease);
        }
        else
        {
            DiseaseService.UpdateDisease(DiseaseIcd, _name, _specialization, symptomWeights);
        }
        
        MudDialog?.Close(DialogResult.Ok(true));
    }

    private void Cancel() => MudDialog?.Cancel();
}