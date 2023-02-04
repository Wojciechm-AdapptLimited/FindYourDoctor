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
    
    public void InsertSpecialization(Specialization specialization)
    {
        _context.Add(specialization);
        _context.SaveChanges();
    }
    
    public void UpdateSpecialization(int specializationId, string newName)
    {
        var specialization = Specializations.SingleOrDefault(x => x.SpecializationId == specializationId);
        
        if (specialization == null) return;

        specialization.Name = newName;
        
        _context.SaveChanges();
    }
    
    public void RemoveSpecialization(int specializationId)
    {
        var specialization = Specializations.SingleOrDefault(x => x.SpecializationId == specializationId);
        
        if (specialization == null) return;
        
        _context.Remove(specialization);
        _context.SaveChanges();
    }
    
    public void InsertSymptom(Symptom symptom)
    {
        _context.Add(symptom);
        _context.SaveChanges();
    }
    
    public void UpdateSymptom(int symptomId, string newName)
    {
        var symptom = Symptoms.SingleOrDefault(x => x.SymptomId == symptomId);
        
        if (symptom == null) return;

        symptom.Name = newName;
        
        _context.SaveChanges();
    }
    
    public void RemoveSymptom(int symptomId)
    {
        var symptom = Symptoms.SingleOrDefault(x => x.SymptomId == symptomId);
        
        if (symptom == null) return;

        _context.Remove(symptom);
        _context.SaveChanges();
    }
    
    public void InsertDisease(Disease disease)
    {
        _context.Add(disease);
        _context.SaveChanges();
    }
    
    public void UpdateDisease(string diseaseIcd, string name, string specializationName, List<SymptomWeight> symptomWeightsToAdd)
    {
        var disease = Diseases.SingleOrDefault(x => x.Icd == diseaseIcd);
        var specialization = Specializations.SingleOrDefault(x => x.Name == specializationName);
        
        if (disease == null || specialization == null) return;

        disease.Name = name;
        disease.Specialization = specialization;
        disease.SymptomWeights = symptomWeightsToAdd;
        
        _context.SaveChanges();
    }
    
    public void RemoveDisease(string diseaseIcd)
    {
        var disease = Diseases.SingleOrDefault(x => x.Icd == diseaseIcd);

        if (disease == null) return;
        
        _context.Remove(disease);
        _context.SaveChanges();
    }
}