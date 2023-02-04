using FindYourDoctor.Data;
using FindYourDoctor.Data.Domain;

namespace FindYourDoctor.Services;

public class DoctorPatientService
{
    private readonly FindYourDoctorDbContext _context;

    public IQueryable<Doctor> Doctors => _context.Set<Doctor>();

    public IQueryable<Patient> Patients => _context.Set<Patient>();

    public IQueryable<Clinic> Clinics => _context.Set<Clinic>();

    public IQueryable<Opinion> Opinions => _context.Set<Opinion>();

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
    
    public void RemoveOpinion(int patientId, int doctorId, DateTime issueDate)
    {
        var opinion = Opinions.SingleOrDefault(x =>
            x.PatientId == patientId && x.DoctorId == doctorId && x.IssueDate == issueDate);

        if (opinion == null) return;

        _context.Remove(opinion);
        _context.SaveChanges();
    }
}