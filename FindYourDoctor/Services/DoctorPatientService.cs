using FindYourDoctor.Data;
using FindYourDoctor.Data.Domain;

namespace FindYourDoctor.Services;

public class DoctorPatientService
{
    private readonly FindYourDoctorDbContext _context;

    public IQueryable<Doctor> Doctors => _context.Set<Doctor>();

    public IQueryable<Patient> Patients => _context.Set<Patient>();

    public IQueryable<Clinic> Clinics => _context.Set<Clinic>();

    public DoctorPatientService(FindYourDoctorDbContext context)
    {
        _context = context;
    }

    public void InsertDoctor(Doctor doctor)
    {
        _context.Add(doctor);
        _context.SaveChanges();
    }

    public void UpdateDoctor(Doctor doctor)
    {
        _context.Update(doctor);
        _context.SaveChanges();
    }

    public void InsertPatient(Patient patient)
    {
        _context.Add(patient);
        _context.SaveChanges();
    }
    
    public void UpdatePatient(Patient patient)
    {
        _context.Update(patient);
        _context.SaveChanges();
    }
}