using FindYourDoctor.Data.Domain;
using Microsoft.EntityFrameworkCore;
using MudBlazor;

namespace FindYourDoctor.Components;

public partial class ShowOpinions
{
    private List<OpinionTableObject> _opinions = new();
    private Patient? _patient;
    private string? _searchString;

    private class OpinionTableObject
    {
        public int DoctorId { get; init; }
        public string DoctorName { get; init; } = string.Empty;
        public DateTime IssueDate { get; init; }
        public int Stars { get; init; }
        public string Description { get; init; } = string.Empty;
    }

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        
        var authenticationState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
        var user = await UserManager.GetUserAsync(authenticationState.User);

        if (user == null)
            return;

        _patient = DoctorPatientService.Patients
            .SingleOrDefault(x => x.UserId == user.Id);
        
        SetOpinions();
    }
    
    private void SetOpinions()
    {
        if (_patient == null)
            return;

        _opinions = DoctorPatientService.Opinions
            .Include(x => x.Doctor)
            .Where(x => x.PatientId == _patient.UserId)
            .Select(x => new OpinionTableObject
            {
                DoctorId = x.DoctorId, DoctorName = x.Doctor.Name + " " + x.Doctor.Surname, IssueDate = x.IssueDate, Stars = x.Stars, 
                Description = x.Description ?? string.Empty
            })
            .OrderByDescending(x => x.IssueDate)
            .ToList();
    }
    
    private bool FilterOpinions(OpinionTableObject opinion)
    {
        return string.IsNullOrWhiteSpace(_searchString) || opinion.DoctorName.Contains(_searchString);
    }
    
    private async void EditOpinion(int doctorId, DateTime issueDate)
    {
        if (_patient == null)
            return;
        
        var parameters = new DialogParameters
        {
            { "DoctorId", doctorId },
            { "IssueDate", issueDate }
        };

        var options = new DialogOptions
        {
            DisableBackdropClick = true
        };

        var logicDialog = await DialogService.ShowAsync<EditOpinion>("Edit review", parameters, options);

        _ = await logicDialog.Result;
        
        SetOpinions();
        
        StateHasChanged();
    }
    
    private async void DeleteOpinion(int doctorId, DateTime issueDate)
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

        var logicDialog = await DialogService.ShowAsync<InfoDialog>("Delete Opinion", parameters, options);

        var result = await logicDialog.Result;

        if (result.Canceled)
            return;
        
        DoctorPatientService.RemoveOpinion(_patient.UserId, doctorId, issueDate);
        
        SetOpinions();

        StateHasChanged();
    }
}