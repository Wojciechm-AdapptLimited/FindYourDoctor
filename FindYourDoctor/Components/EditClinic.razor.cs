using FindYourDoctor.Data.Domain;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace FindYourDoctor.Components;

public partial class EditClinic
{
    private bool _success;
    private MudForm? _form;
    private string _name = string.Empty;
    private string _voivodeship = string.Empty;
    private string _fullAddress = string.Empty;
    private string _phoneNumber = string.Empty;

    private readonly List<string> _voivodeships = new()
    {
        "dolnośląskie", "kujawsko-pomorskie", "lubelskie", "lubuskie", "łódzkie", "małopolskie",
        "mazowieckie", "opolskie", "podkarpackie", "podlaskie", "pomorskie", "śląskie", "świętokrzyskie",
        "warmińsko-mazurskie", "wielkopolskie", "zachodniopomorskie"
    };
    
    [CascadingParameter] 
    public MudDialogInstance? MudDialog { get; set; }
    
    [Parameter] 
    public int? ClinicId{ get; set; }

    protected override void OnInitialized()
    {
        base.OnInitialized();
        
        if (ClinicId == null) return;

        var clinic = DoctorPatientService.Clinics.SingleOrDefault(x => x.ClinicId == ClinicId);
        
        if (clinic == null) return;

        _name = clinic.Name;
        _voivodeship = clinic.Voivodeship;
        _fullAddress = clinic.FullAddress;
        _phoneNumber = clinic.PhoneNumber ?? string.Empty;
    }
    
    private IEnumerable<string> ValidateName(string n)
    {
        if (string.IsNullOrWhiteSpace(n))
        {
            yield return "Clinic's name is required";
            yield break;
        }

        if (DoctorPatientService.Clinics.Any(u => u.Name == n))
            yield return "Clinic with that name already exists";
    }

    private void Submit()
    {
        if (string.IsNullOrWhiteSpace(_name) || string.IsNullOrWhiteSpace(_voivodeship) || string.IsNullOrWhiteSpace(_fullAddress) || string.IsNullOrWhiteSpace(_phoneNumber))
            return;
        
        _form?.Validate();
        
        if (!_success)
            return;

        if (ClinicId == null)
        {
            var clinic = new Clinic
            {
                Name = _name,
                Voivodeship = _voivodeship,
                FullAddress = _fullAddress,
                PhoneNumber = _phoneNumber
            };
            
            DoctorPatientService.InsertClinic(clinic);
        }
        else
        {
            DoctorPatientService.UpdateClinic((int)ClinicId, _name, _voivodeship, _fullAddress, _phoneNumber);
        }
        
        MudDialog?.Close(DialogResult.Ok(true));
    }

    private void Cancel() => MudDialog?.Cancel();
}