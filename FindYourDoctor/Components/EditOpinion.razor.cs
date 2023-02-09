using FindYourDoctor.Data.Domain;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using MudBlazor;

namespace FindYourDoctor.Components;

public partial class EditOpinion
{
    private bool _success;
    private MudForm? _form;
    private int _stars;
    private int? _hoveredStars;
    private string _description = string.Empty;
    private Patient? _patient;

    [CascadingParameter] 
    public MudDialogInstance? MudDialog { get; set; }
    
    [Parameter]
    public int DoctorId { get; set; }
    
    [Parameter]
    public DateTime? IssueDate { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        
        var authenticationState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
        var user = await UserManager.GetUserAsync(authenticationState.User);

        if (user == null)
            return;

        _patient = DoctorPatientService.Patients
            .Include(x => x.DiseaseHistories)
            .SingleOrDefault(x => x.UserId == user.Id);

        if (_patient == null || IssueDate == null) return;

        var opinion = DoctorPatientService.Opinions
            .SingleOrDefault(x => x.PatientId == _patient.UserId && x.DoctorId == DoctorId && x.IssueDate == IssueDate);
        
        if (opinion == null) return;

        _stars = opinion.Stars;
        _description = opinion.Description ?? string.Empty;
    }
    
    private void HandleHoveredValueChanged(int? val) => _hoveredStars = val;
    
    private string LabelText => (_hoveredStars ?? _stars) switch
    {
        1 => "Very bad",
        2 => "Bad",
        3 => "Sufficient",
        4 => "Good",
        5 => "Awesome!",
        _ => "Rate your experience!"
    };

    private void Submit()
    {
        if (_patient == null || _stars is > 5 or < 1)
            return;
        
        _form?.Validate();
        
        if (!_success)
            return;

        var description = string.IsNullOrWhiteSpace(_description) ? null : _description;

        if (IssueDate == null)
        {
            var opinion = new Opinion
            {
                PatientId = _patient.UserId, DoctorId = DoctorId, Stars = _stars, Description = description
            };
            
            DoctorPatientService.InsertOpinion(opinion);
        }
        else
        {
            DoctorPatientService.
                UpdateOpinion(_patient.UserId, (int)DoctorId, (DateTime)IssueDate, _stars, description);
        }

        MudDialog?.Close(DialogResult.Ok(true));
    }

    private void Cancel() => MudDialog?.Cancel();
}