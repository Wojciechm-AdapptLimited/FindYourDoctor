using FindYourDoctor.Components;
using MudBlazor;

namespace FindYourDoctor.Pages;

public partial class ManageSymptoms
{
    private List<SymptomTableObject> _symptoms = new();
    private string? _searchString;

    private class SymptomTableObject
    {
        public int Id { get; init; }
        public string Name { get; init; } = string.Empty;
    }
    
    
    protected override void OnInitialized()
    {
        base.OnInitialized();
        SetSymptoms();
    }

    private void SetSymptoms()
    {
        _symptoms = DiseaseService.Symptoms
            .Select(x => new SymptomTableObject
            {
                Id = x.SymptomId, Name = x.Name
            })
            .OrderBy(x => x.Id)
            .ToList();
    }

    private async void AddSymptom()
    {
        var parameters = new DialogParameters
        {
            { "SymptomId", null }
        };

        var options = new DialogOptions
        {
            DisableBackdropClick = true
        };

        var logicDialog = await DialogService.ShowAsync<EditSymptom>("Add symptom", parameters, options);

        _ = await logicDialog.Result;
        
        SetSymptoms();
        
        StateHasChanged();
    }
    
    private async void EditSymptom(int symptomId)
    {
        var parameters = new DialogParameters
        {
            { "SymptomId", symptomId }
        };

        var options = new DialogOptions
        {
            DisableBackdropClick = true
        };

        var logicDialog = await DialogService.ShowAsync<EditSymptom>("Edit symptom", parameters, options);

        _ = await logicDialog.Result;
        
        SetSymptoms();
        
        StateHasChanged();
    }
    
    private async void DeleteSymptom(int symptomId)
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

        var logicDialog = await DialogService.ShowAsync<InfoDialog>("Delete symptom", parameters, options);

        var result = await logicDialog.Result;

        if (result.Canceled)
            return;
        
        DiseaseService.RemoveSymptom(symptomId);
        
        SetSymptoms();

        StateHasChanged();
    }
    
    private bool FilterSymptoms(SymptomTableObject symptom) => 
        string.IsNullOrWhiteSpace(_searchString) || symptom.Name.Contains(_searchString, StringComparison.CurrentCultureIgnoreCase);
}