using FindYourDoctor.Data.Domain;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace FindYourDoctor.Components;

public partial class EditPatientProfile
{
    private bool _success;
    private MudForm? _form;
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

        _insuranceNumber = _patient.InsuranceNumber;
    }
    
    private async void Submit()
    {
        if (string.IsNullOrWhiteSpace(_insuranceNumber))
            return;
        
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

        _patient.InsuranceNumber = _insuranceNumber;
        DoctorPatientService.UpdatePatient(_patient);

        _ = Refresh.InvokeAsync();
    }
}