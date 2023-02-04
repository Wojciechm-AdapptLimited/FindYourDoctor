using FindYourDoctor.Data.Domain;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using MudBlazor;
using NuGet.Packaging;

namespace FindYourDoctor.Components;

public partial class EditDoctorProfile
{
    private bool _success;
    private MudForm? _form;
    private string _valueClinics = "Nothing selected";
    private string _valueSpecializations = "Nothing selected";
    private string _name = string.Empty;
    private string _surname = string.Empty;
    private string _pwz = string.Empty;
    private Doctor? _doctor;
    private List<string> _clinics = new();
    private IEnumerable<string> _selectedClinics = new HashSet<string>();
    private List<string> _specializations = new();
    private IEnumerable<string> _selectedSpecializations = new HashSet<string>();

    [Parameter] public EventCallback Refresh { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        _specializations = DiseaseService.Specializations.Select(x => x.Name).ToList();
        _clinics = DoctorPatientService.Clinics.Select(x => x.Name).ToList();
        
        var authenticationState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
        var user = await UserManager.GetUserAsync(authenticationState.User);

        if (user == null)
            return;

        _doctor = DoctorPatientService.Doctors
            .Include(x => x.Clinics)
            .Include(x => x.Specializations)
            .SingleOrDefault(x => x.UserId == user.Id);

        if (_doctor == null)
            return;

        _name = _doctor.Name;
        _surname = _doctor.Surname;
        _pwz = _doctor.PwzNumber;
        _selectedClinics = _doctor.Clinics.Select(x => x.Name).ToHashSet();
        _selectedSpecializations = _doctor.Specializations.Select(x => x.Name).ToHashSet();
    }

    private string? ValidatePwz(string pwz)
    {
        if (string.IsNullOrWhiteSpace(pwz))
        {
            return "PWZ Number is required";
        }

        return DoctorPatientService.Doctors.Any(x => x.PwzNumber == pwz)
            ? "PWZ already registered in the system"
            : null;
    }

    private async void Submit()
    {
        if (string.IsNullOrWhiteSpace(_name) || string.IsNullOrWhiteSpace(_surname) || !_selectedClinics.Any() ||
            !_selectedSpecializations.Any())
            return;

        var authenticationState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
        var user = await UserManager.GetUserAsync(authenticationState.User);

        if (user == null)
            return;

        _form?.Validate();

        if (!_success)
            return;

        var clinics = DoctorPatientService.Clinics.Where(x => _selectedClinics.Contains(x.Name));
        var specializations = DiseaseService.Specializations.Where(x => _selectedSpecializations.Contains(x.Name));

        if (_doctor == null)
        {
            _doctor = new Doctor
            {
                UserId = user.Id,
                Name = _name,
                Surname = _surname,
                PwzNumber = _pwz
            };

            _doctor.Clinics.AddRange(clinics);
            _doctor.Specializations.AddRange(specializations);
            DoctorPatientService.InsertDoctor(_doctor);
        }
        else
        {
            DoctorPatientService.UpdateDoctor(_doctor.UserId, _name, _surname, _pwz, clinics.ToList(), specializations.ToList());
        }

        _ = Refresh.InvokeAsync();
    }
}