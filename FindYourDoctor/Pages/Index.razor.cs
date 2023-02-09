using FindYourDoctor.Data.Domain;
using MudBlazor;

namespace FindYourDoctor.Pages;

public partial class Index
{
    private readonly string _selectedSymptom = string.Empty;
    private string _text = string.Empty;
    private List<string> _symptomNames = new();
    private readonly HashSet<string> _selectedSymptoms = new();
    private List<Doctor> _recommendedDoctors = new();

    private string SelectedSymptom
    {
        get => _selectedSymptom;
        set
        {
            _selectedSymptoms.Add(value);
            _symptomNames.Remove(value);
            _text = string.Empty;
            _recommendedDoctors = DiseaseService.GetRecommendedDoctors(_selectedSymptoms);
            StateHasChanged();
        } 
    }

    protected override void OnInitialized()
    {
        base.OnInitialized();
        _symptomNames = DiseaseService.Symptoms.Select(x => x.Name).ToList();
    }

    protected override void OnAfterRender(bool firstRender)
    {
        if (!firstRender)
            return;

        if (NavigationManager.Uri.ToLower().Contains("login_failed")) 
            Snackbar.Add("Login failed", Severity.Error);
    }
    
    private async Task<IEnumerable<string>> SearchForSymptom(string value)
    {
        await Task.Delay(5);
        
        return string.IsNullOrEmpty(value)
            ? Array.Empty<string>()
            : _symptomNames.Where(x => x.Contains(value, StringComparison.InvariantCultureIgnoreCase));
    }

    private void CloseChip(MudChip chip)
    {
        _selectedSymptoms.Remove(chip.Text);
        _symptomNames.Add(chip.Text);
        _recommendedDoctors = DiseaseService.GetRecommendedDoctors(_selectedSymptoms);
    }

    private int GetDoctorsRating(int doctorId)
    {
        var review = DoctorPatientService.Reviews.Single(x => x.UserId == doctorId);
        return review.DoctorsRating ?? 0;
    }
    
    private void ViewDoctorProfile(int doctorId)
    {
        NavigationManager.NavigateTo($"/doctor-profile?id={doctorId}");
    }
}