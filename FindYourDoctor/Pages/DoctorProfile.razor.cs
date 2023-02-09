using FindYourDoctor.Components;
using FindYourDoctor.Data.Domain;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using MudBlazor;

namespace FindYourDoctor.Pages;

public partial class DoctorProfile
{
    private int _doctorId = -1;
    private bool _isFavourite;
    private string _name = string.Empty;
    private string _pwz = string.Empty;
    private int _stars;
    private DateTime _lastLoginTime;
    private Patient? _patient;
    private List<OpinionTableObject> _opinions = new();
    private List<string> _specializations = new();
    private List<string> _clinics = new();

    private string ButtonText => _isFavourite ? "Remove from favourites" : "Add to favourites";
    
    private class OpinionTableObject
    {
        public string PatientUserName { get; init; } = string.Empty;
        public DateTime IssueDate { get; init; }
        public int Stars { get; init; }
        public string Description { get; init; } = string.Empty;
    }

    protected override void OnInitialized()
    {
        GetDoctor();
        GetPatientAsync();
        SetOpinions();
    }

    private void GetDoctor()
    {
        var uri = NavigationManager.ToAbsoluteUri(NavigationManager.Uri);
        if (QueryHelpers.ParseQuery(uri.Query).TryGetValue("id", out var vs))
        {
            _doctorId = int.Parse(vs.ToString());
        }

        var doctor = DoctorPatientService.Doctors
            .Include(x => x.User)
            .Include(x => x.Clinics)
            .Include(x => x.Specializations)
            .SingleOrDefault(x => x.UserId == _doctorId);
        
        if (doctor == null) return;

        _name = doctor.Name + " " + doctor.Surname;
        _pwz = doctor.PwzNumber;
        _stars = DoctorPatientService.Reviews.SingleOrDefault(x => x.UserId == _doctorId)?.DoctorsRating ?? 0;
        _lastLoginTime = doctor.User.LastLoginTime;
        _specializations = doctor.Specializations.Select(x => x.Name).ToList();
        _clinics = doctor.Clinics.Select(x => $"{x.Name} {x.FullAddress} {x.Voivodeship} {x.PhoneNumber}").ToList();
    }

    private async void GetPatientAsync()
    {
        var authenticationState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
        var user = await UserManager.GetUserAsync(authenticationState.User);

        if (user == null)
            return;

        _patient = DoctorPatientService.Patients
            .Include(x => x.FavouriteDoctors)
            .SingleOrDefault(x => x.UserId == user.Id);
        
        if (_patient == null)
            return;

        _isFavourite = _patient.FavouriteDoctors.Any(x => x.UserId == _doctorId);

        await InvokeAsync(StateHasChanged);
    }

    private void SetOpinions()
    {
        _opinions = DoctorPatientService.Opinions
            .Include(x => x.Patient)
            .ThenInclude(x => x.User)
            .Where(x => x.DoctorId == _doctorId)
            .Select(x => new OpinionTableObject
            {
                PatientUserName = x.Patient.User.UserName ?? "[DELETED]", Description = x.Description ?? string.Empty,
                Stars = x.Stars, IssueDate = x.IssueDate
            }).ToList();
    }

    private void AddOrRemoveToFavourite()
    {
        if (_patient == null)
            return;

        if (_isFavourite)
        {
            DoctorPatientService.RemoveFavouriteDoctor(_patient.UserId, _doctorId);
        }
        else
        {
            DoctorPatientService.AddFavouriteDoctor(_patient.UserId, _doctorId);
        }

        _isFavourite = !_isFavourite;
        StateHasChanged();
    }

    private async void AddOpinion()
    {
        if (_patient == null)
            return;
        
        var parameters = new DialogParameters
        {
            { "DoctorId", _doctorId },
            { "IssueDate", null }
        };

        var options = new DialogOptions
        {
            DisableBackdropClick = true
        };

        var logicDialog = await DialogService.ShowAsync<EditOpinion>($"Review {_name}", parameters, options);

        _ = await logicDialog.Result;
        
        SetOpinions();
        
        _stars = DoctorPatientService.Reviews.SingleOrDefault(x => x.UserId == _doctorId)?.DoctorsRating ?? 0;
        
        StateHasChanged();
    }
}