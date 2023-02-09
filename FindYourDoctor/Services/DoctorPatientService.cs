using FindYourDoctor.Data;
using FindYourDoctor.Data.Domain;
using Microsoft.EntityFrameworkCore;

namespace FindYourDoctor.Services;

public class DoctorPatientService
{
    private readonly FindYourDoctorDbContext _context;

    public IQueryable<Doctor> Doctors => _context.Set<Doctor>();

    public IQueryable<Patient> Patients => _context.Set<Patient>();

    public IQueryable<Clinic> Clinics => _context.Set<Clinic>();

    public IQueryable<Opinion> Opinions => _context.Set<Opinion>();

    public IQueryable<ShowReview> Reviews => _context.Set<ShowReview>();

    public IQueryable<DiseaseHistory> DiseaseHistory => _context.Set<DiseaseHistory>();

    public DoctorPatientService(FindYourDoctorDbContext context)
    {
        _context = context;
    }

    public void InsertDoctor(Doctor doctor)
    {
        _context.Add(doctor);
        _context.SaveChanges();
    }

    public void UpdateDoctor(int doctorId, string name, string surname, string pwzNumber, List<Clinic> clinics, 
        List<Specialization> specializations)
    {
        var doctor = Doctors.SingleOrDefault(x => x.UserId == doctorId);
        
        if (doctor == null) return;

        doctor.Name = name;
        doctor.Surname = surname;
        doctor.PwzNumber = pwzNumber;
        doctor.Clinics = clinics;
        doctor.Specializations = specializations;

        _context.SaveChanges();
    }

    public void InsertPatient(Patient patient)
    {
        _context.Add(patient);
        _context.SaveChanges();
    }
    
    public void UpdatePatient(int patientId, string name, string surname, string insuranceNumber)
    {
        var patient = Patients.SingleOrDefault(x => x.UserId == patientId);
        
        if (patient == null) return;

        patient.Name = name;
        patient.Surname = surname;
        patient.InsuranceNumber = insuranceNumber;
        
        _context.SaveChanges();
    }
    
    public void InsertClinic(Clinic clinic)
    {
        _context.Add(clinic);
        _context.SaveChanges();
    }
    
    public void UpdateClinic(int clinicId, string name, string voivodeship, string fullAddress, string phoneNumber)
    {
        var clinic = Clinics.SingleOrDefault(x => x.ClinicId == clinicId);

        if (clinic == null) return;

        clinic.Name = name;
        clinic.Voivodeship = voivodeship;
        clinic.FullAddress = fullAddress;
        clinic.PhoneNumber = phoneNumber;
        
        _context.SaveChanges();
    }

    public void RemoveClinic(int clinicId)
    {
        var clinic = Clinics.SingleOrDefault(x => x.ClinicId == clinicId);

        if (clinic == null) return;

        _context.Remove(clinic);
        _context.SaveChanges();
    }

    public void InsertOpinion(Opinion opinion)
    {
        _context.Add(opinion);
        _context.SaveChanges();
    }
    
    public void UpdateOpinion(int patientId, int doctorId, DateTime issueDate, int stars, string? description)
    {
        var opinion = Opinions.SingleOrDefault(x =>
            x.PatientId == patientId && x.DoctorId == doctorId && x.IssueDate == issueDate);

        if (opinion == null) return;

        opinion.Stars = stars;
        opinion.Description = description;
        
        _context.SaveChanges();
    }
    
    public void RemoveOpinion(int patientId, int doctorId, DateTime issueDate)
    {
        var opinion = Opinions.SingleOrDefault(x =>
            x.PatientId == patientId && x.DoctorId == doctorId && x.IssueDate == issueDate);

        if (opinion == null) return;

        _context.Remove(opinion);
        _context.SaveChanges();
    }
    
    public void InsertDiseaseHistory(DiseaseHistory diseaseHistory)
    {
        _context.Add(diseaseHistory);
        _context.SaveChanges();
    }
    
    public void RemoveDiseaseHistory(int patientId, string diseaseIcd, DateOnly illnessDate)
    {
        var diseaseHistory = DiseaseHistory.SingleOrDefault(x =>
            x.PatientId == patientId && x.DiseaseIcd == diseaseIcd && x.IllnessDate == illnessDate);

        if (diseaseHistory == null) return;

        _context.Remove(diseaseHistory);
        _context.SaveChanges();
    }

    public void AddFavouriteDoctor(int patientId, int doctorId)
    {
        var patient = Patients.Include(x => x.FavouriteDoctors).SingleOrDefault(x => x.UserId == patientId);

        var doctor = Doctors.SingleOrDefault(x => x.UserId == doctorId);
        
        if (doctor == null) return;
        
        patient?.FavouriteDoctors.Add(doctor);
        _context.SaveChanges();
    }

    public void RemoveFavouriteDoctor(int patientId, int doctorId)
    {
        var patient = Patients.Include(x => x.FavouriteDoctors).SingleOrDefault(x => x.UserId == patientId);

        var doctor = patient?.FavouriteDoctors.SingleOrDefault(x => x.UserId == doctorId);
        
        if (doctor == null) return;

        patient?.FavouriteDoctors.Remove(doctor);
        _context.SaveChanges();
    }
}