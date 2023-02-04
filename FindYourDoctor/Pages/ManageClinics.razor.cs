using FindYourDoctor.Components;
using MudBlazor;

namespace FindYourDoctor.Pages;

public partial class ManageClinics
{
    private List<ClinicTableObject> _clinics = new();
    private string? _searchString;
    private string _searchType = "Name";

    private class ClinicTableObject
    {
        public int Id { get; init; }
        public string Name { get; init; } = string.Empty;
        public string Voivodeship { get; init; } = string.Empty;
        public string FullAddress { get; init; } = string.Empty;
        public string PhoneNumber { get; init; } = string.Empty;
    }

    protected override void OnInitialized()
    {
        base.OnInitialized();
        SetClinics();
    }
    
    private void SetClinics()
    {
        _clinics = DoctorPatientService.Clinics
            .Select(x => new ClinicTableObject
            {
                Id = x.ClinicId, Name = x.Name, Voivodeship = x.Voivodeship, FullAddress = x.FullAddress,
                PhoneNumber = x.PhoneNumber ?? string.Empty
            })
            .OrderBy(x => x.Id)
            .ToList();
    }

    private bool FilterClinics(ClinicTableObject clinic)
    {
        if (string.IsNullOrWhiteSpace(_searchString))
            return true;
        
        return _searchType switch
        {
            "Name" when clinic.Name.Contains(_searchString, StringComparison.CurrentCultureIgnoreCase) => true,
            "Voivodeship" when clinic.Voivodeship.Contains(_searchString, StringComparison.CurrentCultureIgnoreCase) => true,
            "Full Address" when clinic.FullAddress.Contains(_searchString, StringComparison.CurrentCultureIgnoreCase) => true,
            _ => false
        };
    }

    private async void AddClinic()
    {
        var parameters = new DialogParameters
        {
            { "ClinicId", null }
        };

        var options = new DialogOptions
        {
            DisableBackdropClick = true
        };

        var logicDialog = await DialogService.ShowAsync<EditClinic>("Add clinic", parameters, options);

        _ = await logicDialog.Result;
        
        SetClinics();
        
        StateHasChanged();
    }
    
    private async void EditClinic(int clinicId)
    {
        var parameters = new DialogParameters
        {
            { "ClinicId", clinicId }
        };

        var options = new DialogOptions
        {
            DisableBackdropClick = true
        };

        var logicDialog = await DialogService.ShowAsync<EditClinic>("Edit clinic", parameters, options);

        _ = await logicDialog.Result;
        
        SetClinics();
        
        StateHasChanged();
    }
    
    private async void DeleteClinic(int clinicId)
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

        var logicDialog = await DialogService.ShowAsync<InfoDialog>("Delete clinic", parameters, options);

        var result = await logicDialog.Result;

        if (result.Canceled)
            return;
        
        DoctorPatientService.RemoveClinic(clinicId);
        
        SetClinics();

        StateHasChanged();
    }
}