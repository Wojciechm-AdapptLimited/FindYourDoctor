using FindYourDoctor.Components;
using Microsoft.EntityFrameworkCore;
using MudBlazor;

namespace FindYourDoctor.Pages;

public partial class ManageDiseases
{
    private List<DiseaseTableObject> _diseases = new();
    private string? _searchString;
    private string _searchType = "Name";

    private class DiseaseTableObject
    {
        public string Icd { get; init; } = string.Empty;
        public string Name { get; init; } = string.Empty;
        public string SpecializationName { get; init; } = string.Empty;
        public List<SymptomWeightTableObject> SymptomWeights { get; init; } = new();
        public bool ShowSymptoms { get; set; }
    }
    
    private class SymptomWeightTableObject
    {
        public string SymptomName { get; init; } = string.Empty;
        public int Weight { get; init; }
    }
    
    
    protected override void OnInitialized()
    {
        base.OnInitialized();
        SetDiseases();
    }

    private void SetDiseases()
    {
        _diseases = DiseaseService.Diseases
            .Include(x => x.Specialization)
            .Include(x => x.SymptomWeights)
            .ThenInclude(x => x.Symptom)
            .Select(x => new DiseaseTableObject
            {
                Icd = x.Icd, Name = x.Name, SpecializationName = x.Specialization.Name,
                SymptomWeights = x.SymptomWeights
                    .Select(y => new SymptomWeightTableObject
                    {
                        SymptomName = y.Symptom.Name, Weight = y.Weight
                    })
                    .ToList()
            })
            .OrderBy(x => x.Icd)
            .ToList();
    }

    private async void AddDisease()
    {
        var parameters = new DialogParameters
        {
            { "DiseaseIcd", null }
        };

        var options = new DialogOptions
        {
            DisableBackdropClick = true
        };

        var logicDialog = await DialogService.ShowAsync<EditDisease>("Add disease", parameters, options);

        _ = await logicDialog.Result;
        
        SetDiseases();
        
        StateHasChanged();
    }
    
    private async void EditDisease(string diseaseIcd)
    {
        var parameters = new DialogParameters
        {
            { "DiseaseIcd", diseaseIcd }
        };

        var options = new DialogOptions
        {
            DisableBackdropClick = true
        };

        var logicDialog = await DialogService.ShowAsync<EditDisease>("Edit disease", parameters, options);

        _ = await logicDialog.Result;
        
        SetDiseases();
        
        StateHasChanged();
    }
    
    private async void DeleteDisease(string diseaseIcd)
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

        var logicDialog = await DialogService.ShowAsync<InfoDialog>("Delete disease", parameters, options);

        var result = await logicDialog.Result;

        if (result.Canceled)
            return;
        
        DiseaseService.RemoveDisease(diseaseIcd);
        
        SetDiseases();

        StateHasChanged();
    }
    
    private bool FilterDiseases(DiseaseTableObject disease)
    {
        if (string.IsNullOrWhiteSpace(_searchString))
            return true;
        
        return _searchType switch
        {
            "Name" when disease.Name.Contains(_searchString, StringComparison.CurrentCultureIgnoreCase) => true,
            "Icd" when disease.Icd.Contains(_searchString, StringComparison.CurrentCultureIgnoreCase) => true,
            "Specialization Name" when disease.SpecializationName.Contains(_searchString, StringComparison.CurrentCultureIgnoreCase) => true,
            _ => false
        };
    }
}