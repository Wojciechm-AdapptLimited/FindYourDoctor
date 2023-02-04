using FindYourDoctor.Data.Domain;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace FindYourDoctor.Components;

public partial class EditPatientProfile
{
    private bool _success;
    private MudForm? _form;
    private string _name = string.Empty;
    private string _surname = string.Empty;
    private string _insuranceNumber = string.Empty;
    private Patient? _patient;

    [Parameter] 
    public EventCallback Refresh { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        
        var authenticationState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
        var user = await UserManager.GetUserAsync(authenticationState.User);

        if (user == null)
            return;

        _patient = DoctorPatientService.Patients
            .SingleOrDefault(x => x.UserId == user.Id);

        if (_patient == null)
            return;

        _name = _patient.Name ?? string.Empty;
        _surname = _patient.Surname ?? string.Empty;
        _insuranceNumber = _patient.InsuranceNumber ?? string.Empty;
    }
    
    private async void Submit()
    {
        var authenticationState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
        var user = await UserManager.GetUserAsync(authenticationState.User);

        if (user == null)
            return;

        _form?.Validate();

        if (!_success)
            return;
        
        if (_patient == null)
        {
            _patient = new Patient()
            {
                UserId = user.Id,
                InsuranceNumber = _insuranceNumber
            };
            
            DoctorPatientService.InsertPatient(_patient);
        }
        else
        {
            DoctorPatientService.UpdatePatient(_patient.UserId, _name, _surname, _insuranceNumber);
        }

        _ = Refresh.InvokeAsync();
    }
}