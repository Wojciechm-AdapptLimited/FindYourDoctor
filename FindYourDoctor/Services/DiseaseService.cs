using FindYourDoctor.Data;
using FindYourDoctor.Data.Domain;
using Microsoft.EntityFrameworkCore;

namespace FindYourDoctor.Services;

public class DiseaseService
{
    private readonly FindYourDoctorDbContext _context;
    
    public IQueryable<Specialization> Specializations => _context.Set<Specialization>();

    public IQueryable<Disease> Diseases => _context.Set<Disease>();

    public IQueryable<Symptom> Symptoms => _context.Set<Symptom>();
    
    public IQueryable<SymptomWeight> SymptomWeights => _context.Set<SymptomWeight>();

    public DiseaseService(FindYourDoctorDbContext context)
    {
        _context = context;
    }

    public List<Symptom> Search(string searchString)
    {
        return _context.Symptoms
            .Where(s => s.Name.Contains(searchString))
            .ToList();
    }

    public IEnumerable<Disease> GetPossibleDiseases(IEnumerable<Symptom> symptoms)
    {
        var diseases = _context.Diseases.Include(d => d.Specialization).ThenInclude(s => s.Doctors).ToList();

        diseases = symptoms
            .Select(symptom => _context.Diseases.FromSql($"SELECT * FROM public.get_diseases_for_symptom({symptom.SymptomId})").ToList())
            .Aggregate(diseases, (current, diseasesForSymptom) => current.IntersectBy(diseasesForSymptom, d => d).ToList());

        return diseases;
    }

    public List<Doctor> GetRecommendedDoctors(IEnumerable<Symptom> symptoms)
    {
        var diseases = GetPossibleDiseases(symptoms);
        return diseases.SelectMany(d => d.Specialization.Doctors).ToList();
    }
}