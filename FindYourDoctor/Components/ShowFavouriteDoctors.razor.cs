using FindYourDoctor.Data.Domain;
using Microsoft.EntityFrameworkCore;
using MudBlazor;

namespace FindYourDoctor.Components;

public partial class ShowFavouriteDoctors
{
    private List<DoctorTableObject> _favouriteDoctors = new();
    private Patient? _patient;
    private string? _searchString;

    private class DoctorTableObject
    {
        public int DoctorId { get; set; }
        
        public string DoctorName { get; init; } = string.Empty;
        
        public string PwzNumber { get; init; } = string.Empty;
    }

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        
        var authenticationState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
        var user = await UserManager.GetUserAsync(authenticationState.User);

        if (user == null)
            return;

        _patient = DoctorPatientService.Patients
            .Include(x => x.FavouriteDoctors)
            .SingleOrDefault(x => x.UserId == user.Id);
        
        SetFavouriteDoctors();
    }
    
    private void SetFavouriteDoctors()
    {
        if (_patient == null)
            return;

        _favouriteDoctors = _patient.FavouriteDoctors
            .Select(x => new DoctorTableObject
            {
                DoctorId = x.UserId, DoctorName = x.Name + " " + x.Surname, PwzNumber = x.PwzNumber
            })
            .ToList();
    }
    
    private bool FilterDoctors(DoctorTableObject doctor)
    {
        return string.IsNullOrWhiteSpace(_searchString) || doctor.DoctorName.Contains(_searchString);
    }
    
    private async void DeleteFavouriteDoctor(int doctorId)
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

        var logicDialog = await DialogService.ShowAsync<InfoDialog>("Delete clinic", parameters, options);

        var result = await logicDialog.Result;

        if (result.Canceled)
            return;
        
        DoctorPatientService.RemoveFavouriteDoctor(_patient.UserId, doctorId);
        
        SetFavouriteDoctors();

        StateHasChanged();
    }
}