using FindYourDoctor.Data.Domain;
using Microsoft.EntityFrameworkCore;
using MudBlazor;

namespace FindYourDoctor.Components;

public partial class ShowDiseaseHistory
{
    private List<DiseaseHistoryTableObject> _diseaseHistory = new();
    private Patient? _patient;
    private string? _searchString;

    private class DiseaseHistoryTableObject
    {
        public string Icd { get; init; } = string.Empty;
        public string DiseaseName { get; init; } = string.Empty;
        public DateOnly IllnessTime { get; init; }
    }

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        
        var authenticationState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
        var user = await UserManager.GetUserAsync(authenticationState.User);

        if (user == null)
            return;

        _patient = DoctorPatientService.Patients
            .Include(x => x.DiseaseHistories)
            .ThenInclude(x => x.Disease)
            .SingleOrDefault(x => x.UserId == user.Id);
        
        SetDiseaseHistory();
    }
    
    private void SetDiseaseHistory()
    {
        if (_patient == null)
            return;

        _diseaseHistory = DoctorPatientService.DiseaseHistory
            .Include(x => x.Disease)
            .Where(x => x.PatientId == _patient.UserId)
            .Select(x => new DiseaseHistoryTableObject
            {
                Icd = x.DiseaseIcd, DiseaseName = x.Disease.Name, IllnessTime = x.IllnessDate
            })
            .OrderByDescending(x => x.IllnessTime)
            .ToList();
    }
    
    private bool FilterDiseaseHistory(DiseaseHistoryTableObject diseaseHistory)
    {
        return string.IsNullOrWhiteSpace(_searchString) || diseaseHistory.DiseaseName.Contains(_searchString);
    }

    private async void AddDiseaseHistory()
    {
        var options = new DialogOptions
        {
            DisableBackdropClick = true
        };

        var logicDialog = await DialogService.ShowAsync<EditDiseaseHistory>("Add Disease History", options);

        _ = await logicDialog.Result;
        
        SetDiseaseHistory();
        
        StateHasChanged();
    }
    
    private async void DeleteDiseaseHistory(string diseaseIcd, DateOnly illnessDate)
    {
        if (_patient == null)
            return;

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

        var logicDialog = await DialogService.ShowAsync<InfoDialog>("Delete Entry", parameters, options);

        var result = await logicDialog.Result;

        if (result.Canceled)
            return;
        
        DoctorPatientService.RemoveDiseaseHistory(_patient.UserId, diseaseIcd, illnessDate);
        
        SetDiseaseHistory();

        StateHasChanged();
    }
}