using Microsoft.EntityFrameworkCore;

namespace FindYourDoctor.Pages;

public partial class ManageDoctors
{
    private List<DoctorTableObject> _doctors = new();
    private string? _searchString;
    private string _searchType = "Username";

    private class DoctorTableObject
    {
        private bool _showSpecializations;
        private bool _showClinics;
        
        public int Id { get; init; }

        public string UserName { get; init; } = string.Empty;

        public string Name { get; init; } = string.Empty;

        public string Surname { get; init; } = string.Empty;

        public string PwzNumber { get; init; } = string.Empty;

        public List<SpecializationTableObject> Specializations { get; init; } = new();

        public List<ClinicTableObject> Clinics { get; init; } = new();
        
        public bool ShowSpecializations
        {
            get => _showSpecializations;
            set
            {
                _showSpecializations = value;
                _showClinics &= !value;
            }
        }
    
        public bool ShowClinics
        {
            get => _showClinics;
            set
            {
                _showClinics = value;
                _showSpecializations &= !value;
            }
        } 
    }
    
    private class SpecializationTableObject
    {
        public int Id { get; init; }

        public string Name { get; init; } = string.Empty;
    }
    
    private class ClinicTableObject
    {
        public int Id { get; init; }

        public string Name { get; init; } = string.Empty;

        public string Voivodeship { get; init; } = string.Empty;

        public string FullAddress { get; init; } = string.Empty;
    }
    
    protected override void OnInitialized()
    {
        base.OnInitialized();
        _doctors = DoctorPatientService.Doctors
            .Include(x => x.User)
            .Include(x => x.Specializations)
            .Include(x => x.Clinics)
            .Select(x => new DoctorTableObject
            {
                Id = x.UserId, UserName = x.User.UserName ?? string.Empty, Name = x.Name, Surname = x.Surname,
                PwzNumber = x.PwzNumber, 
                Specializations = x.Specializations
                    .Select(y => new SpecializationTableObject 
                    { 
                        Id = y.SpecializationId, Name = y.Name 
                    })
                    .ToList(),
                Clinics = x.Clinics
                    .Select(y => new ClinicTableObject
                    {
                        Id = y.ClinicId, Name = y.Name, Voivodeship = y.Voivodeship, FullAddress = y.FullAddress
                    })
                    .ToList()
            })
            .OrderBy(x => x.Id)
            .ToList();
    }

    private bool FilterDoctors(DoctorTableObject doctor)
    {
        if (string.IsNullOrWhiteSpace(_searchString))
            return true;
        
        return _searchType switch
        {
            "Username" when doctor.UserName.Contains(_searchString, StringComparison.CurrentCultureIgnoreCase) => true,
            "Name" when doctor.Name.Contains(_searchString, StringComparison.CurrentCultureIgnoreCase) => true,
            "Surname" when doctor.Surname.Contains(_searchString, StringComparison.CurrentCultureIgnoreCase) => true,
            "PWZ Number" when doctor.PwzNumber.Contains(_searchString, StringComparison.CurrentCultureIgnoreCase) => true,
            _ => false
        };
    }
}