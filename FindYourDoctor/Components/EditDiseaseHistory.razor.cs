using FindYourDoctor.Data.Domain;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using MudBlazor;

namespace FindYourDoctor.Components;

public partial class EditDiseaseHistory
{
    private bool _success;
    private MudForm? _form;
    private string _disease = string.Empty;
    private List<string> _diseases = new();
    private DateTime? _illnessDate;

    [CascadingParameter] 
    public MudDialogInstance? MudDialog { get; set; }

    protected override void OnInitialized()
    {
        base.OnInitialized();

        _diseases = DiseaseService.Diseases.Select(x => x.Name).ToList();
    }

    private async void SubmitAsync()
    {
        if (string.IsNullOrWhiteSpace(_disease) || _illnessDate == null || _illnessDate > DateTime.Today)
            return;
        
        _form?.Validate();
        
        if (!_success)
            return;
        
        var authenticationState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
        var user = await UserManager.GetUserAsync(authenticationState.User);

        if (user == null)
            return;

        var patient = DoctorPatientService.Patients
            .Include(x => x.DiseaseHistories)
            .SingleOrDefault(x => x.UserId == user.Id);

        var disease = DiseaseService.Diseases.SingleOrDefault(x => x.Name == _disease);
        
        if (patient == null || disease == null)
            return;

        var history = new DiseaseHistory
        {
            PatientId = patient.UserId, DiseaseIcd = disease.Icd, IllnessDate = DateOnly.FromDateTime((DateTime)_illnessDate)
        };
        
        DoctorPatientService.InsertDiseaseHistory(history);
        
        MudDialog?.Close(DialogResult.Ok(true));
    }

    private void Cancel() => MudDialog?.Cancel();
}