using FindYourDoctor.Components;
using MudBlazor;

namespace FindYourDoctor.Pages;

public partial class ManageSpecializations
{
    private List<SpecializationTableObject> _specializations = new();
    private string? _searchString;

    private class SpecializationTableObject
    {
        public int Id { get; init; }
        public string Name { get; init; } = string.Empty;
    }


    protected override void OnInitialized()
    {
        base.OnInitialized();
        SetSpecializations();
    }

    private void SetSpecializations()
    {
        _specializations = DiseaseService.Specializations
            .Select(x => new SpecializationTableObject
            {
                Id = x.SpecializationId, Name = x.Name
            })
            .OrderBy(x => x.Id)
            .ToList();
    }

    private async void AddSpecialization()
    {
        var parameters = new DialogParameters
        {
            { "SpecializationId", null }
        };

        var options = new DialogOptions
        {
            DisableBackdropClick = true
        };

        var logicDialog = await DialogService.ShowAsync<EditSpecialization>("Add specialization", parameters, options);

        _ = await logicDialog.Result;
        
        SetSpecializations();
        
        StateHasChanged();
    }
    
    private async void EditSpecialization(int specializationId)
    {
        var parameters = new DialogParameters
        {
            { "SpecializationId", specializationId }
        };

        var options = new DialogOptions
        {
            DisableBackdropClick = true
        };

        var logicDialog = await DialogService.ShowAsync<EditSpecialization>("Edit specialization", parameters, options);

        _ = await logicDialog.Result;
        
        SetSpecializations();
        
        StateHasChanged();
    }
    
    private async void DeleteSpecialization(int specializationId)
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

        var logicDialog = await DialogService.ShowAsync<InfoDialog>("Delete specialization", parameters, options);

        var result = await logicDialog.Result;

        if (result.Canceled)
            return;
        
        DiseaseService.RemoveSpecialization(specializationId);
        
        SetSpecializations();

        StateHasChanged();
    }
    
    private bool FilterSpecializations(SpecializationTableObject specialization) => 
        string.IsNullOrWhiteSpace(_searchString) || specialization.Name.Contains(_searchString, StringComparison.CurrentCultureIgnoreCase);
}