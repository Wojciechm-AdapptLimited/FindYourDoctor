using FindYourDoctor.Components;
using Microsoft.EntityFrameworkCore;
using MudBlazor;

namespace FindYourDoctor.Pages;

public partial class ManageOpinions
{
    private List<OpinionTableObject> _opinions = new();
    private string? _searchString;
    private string _searchType = "Patient Username";

    private class OpinionTableObject
    {
        public int PatientId { get; init; }
        public int DoctorsId { get; init; }
        public string PatientUserName { get; init; } = string.Empty;
        public string DoctorName { get; init; } = string.Empty;
        public DateTime IssueDate { get; init; }
        public int Stars { get; init; }
        public string Description { get; init; } = string.Empty;
    }
    
    
    protected override void OnInitialized()
    {
        base.OnInitialized();
        SetOpinions();
    }

    private void SetOpinions()
    {
        _opinions = DoctorPatientService.Opinions
            .Include(x => x.Patient)
            .ThenInclude(x => x.User)
            .Include(x => x.Doctor)
            .Select(x => new OpinionTableObject
            {
                PatientId = x.PatientId,
                DoctorsId = x.DoctorId,
                PatientUserName = x.Patient.User.UserName ?? string.Empty,
                DoctorName = x.Doctor.Name + " " + x.Doctor.Surname,
                IssueDate = x.IssueDate,
                Stars = x.Stars,
                Description = x.Description ?? string.Empty
            })
            .ToList();
    }
    
    private async void DeleteOpinion(int patientId, int doctorId, DateTime issueDate)
    {
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

        var logicDialog = await DialogService.ShowAsync<InfoDialog>("Delete symptom", parameters, options);

        var result = await logicDialog.Result;

        if (result.Canceled)
            return;
        
        DoctorPatientService.RemoveOpinion(patientId, doctorId, issueDate);
        
        SetOpinions();

        StateHasChanged();
    }
    
    private bool FilterOpinions(OpinionTableObject opinion)
    {
        if (string.IsNullOrWhiteSpace(_searchString))
            return true;
        
        return _searchType switch
        {
            "Patient Username" when opinion.PatientUserName.Contains(_searchString, StringComparison.CurrentCultureIgnoreCase) => true,
            "Doctor Name" when opinion.DoctorName.Contains(_searchString, StringComparison.CurrentCultureIgnoreCase) => true,
            _ => false
        };
    }
}