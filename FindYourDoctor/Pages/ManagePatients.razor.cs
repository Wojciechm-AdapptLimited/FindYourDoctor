using Microsoft.EntityFrameworkCore;

namespace FindYourDoctor.Pages;

public partial class ManagePatients
{
    private List<PatientTableObject> _patients = new();
    private string? _searchString;
    private string _searchType = "Username";

    private class PatientTableObject
    {
        private bool _showOpinions;
        private bool _showFavouriteDoctors;
        private bool _showDiseaseHistory;
        
        public int Id { get; init; }

        public string UserName { get; init; } = string.Empty;

        public string Name { get; init; } = string.Empty;

        public string Surname { get; init; } = string.Empty;

        public string InsuranceNumber { get; init; } = string.Empty;

        public List<DoctorTableObject> FavouriteDoctors { get; init; } = new();

        public List<OpinionTableObject> Opinions { get; init; } = new();
        
        public List<DiseaseHistoryTableObject> DiseaseHistory { get; init; } = new();
        
        public bool ShowOpinions
        {
            get => _showOpinions;
            set
            {
                _showOpinions = value;
                _showFavouriteDoctors &= !value;
                _showDiseaseHistory &= !value;
            }
        }
    
        public bool ShowFavouriteDoctors
        {
            get => _showFavouriteDoctors;
            set
            {
                _showFavouriteDoctors = value;
                _showOpinions &= !value;
                _showDiseaseHistory &= !value;
            }
        }

        public bool ShowDiseaseHistory
        {
            get => _showDiseaseHistory;
            set
            {
                _showDiseaseHistory = value;
                _showFavouriteDoctors &= !value;
                _showOpinions &= !value;
            }
        }
    }

    private class DoctorTableObject
    {
        public string DoctorName { get; init; } = string.Empty;
        
        public string PwzNumber { get; init; } = string.Empty;
    }

    private class DiseaseHistoryTableObject
    {
        public string Icd { get; init; } = string.Empty;
        public string DiseaseName { get; init; } = string.Empty;
        public DateOnly IllnessTime { get; init; }
    }
    
    private class OpinionTableObject
    {
        public string DoctorName { get; init; } = string.Empty;
        public DateTime IssueDate { get; init; }
        public int Stars { get; init; }
        public string Description { get; init; } = string.Empty;
    }
    
    protected override void OnInitialized()
    {
        base.OnInitialized();
        _patients = DoctorPatientService.Patients
            .Include(x => x.User)
            .Include(x => x.FavouriteDoctors)
            .Include(x => x.Opinions)
            .ThenInclude(x => x.Doctor)
            .Include(x => x.DiseaseHistories)
            .ThenInclude(x => x.Disease)
            .Select(x => new PatientTableObject
            {
                Id = x.UserId, UserName = x.User.UserName ?? string.Empty, 
                Name = x.Name ?? string.Empty, Surname = x.Surname ?? string.Empty,
                InsuranceNumber = x.InsuranceNumber ?? string.Empty, 
                FavouriteDoctors = x.FavouriteDoctors
                    .Select(y => new DoctorTableObject
                    { 
                        DoctorName = y.Name + " " + y.Surname, PwzNumber = y.PwzNumber
                    })
                    .ToList(),
                Opinions = x.Opinions
                    .Select(y => new OpinionTableObject
                    {
                        DoctorName = y.Doctor.Name + " " + y.Doctor.Surname, IssueDate = y.IssueDate, Stars = y.Stars,
                        Description = y.Description ?? string.Empty
                    })
                    .ToList(),
                DiseaseHistory = x.DiseaseHistories
                    .Select(y => new DiseaseHistoryTableObject
                    {
                        Icd = y.DiseaseIcd, DiseaseName = y.Disease.Name, IllnessTime = y.IllnessDate
                    })
                    .ToList()
            })
            .OrderBy(x => x.Id)
            .ToList();
    }

    private bool FilterPatients(PatientTableObject patient)
    {
        if (string.IsNullOrWhiteSpace(_searchString))
            return true;
        
        return _searchType switch
        {
            "Username" when patient.UserName.Contains(_searchString, StringComparison.CurrentCultureIgnoreCase) => true,
            "Name" when patient.Name.Contains(_searchString, StringComparison.CurrentCultureIgnoreCase) => true,
            "Surname" when patient.Surname.Contains(_searchString, StringComparison.CurrentCultureIgnoreCase) => true,
            "PWZ Number" when patient.InsuranceNumber.Contains(_searchString, StringComparison.CurrentCultureIgnoreCase) => true,
            _ => false
        };
    }
}