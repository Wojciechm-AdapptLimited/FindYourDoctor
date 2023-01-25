using FindYourDoctor.Data.Domain;
using Microsoft.EntityFrameworkCore;

namespace FindYourDoctor.Data;

public partial class FindYourDoctorDbContext : DbContext
{
    public FindYourDoctorDbContext()
    {
    }

    public FindYourDoctorDbContext(DbContextOptions<FindYourDoctorDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Account> Accounts { get; set; }

    public virtual DbSet<Clinic> Clinics { get; set; }

    public virtual DbSet<Disease> Diseases { get; set; }

    public virtual DbSet<DiseaseHistory> DiseaseHistories { get; set; }

    public virtual DbSet<Doctor> Doctors { get; set; }

    public virtual DbSet<Opinion> Opinions { get; set; }

    public virtual DbSet<Patient> Patients { get; set; }

    public virtual DbSet<ShowReview> ShowReviews { get; set; }

    public virtual DbSet<ShowReviewsWithSpecialization> ShowReviewsWithSpecializations { get; set; }

    public virtual DbSet<Specialization> Specializations { get; set; }

    public virtual DbSet<Symptom> Symptoms { get; set; }

    public virtual DbSet<SymptomWeight> SymptomWeights { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("account_pkey");

            entity.ToTable("account");

            entity.HasIndex(e => e.Name, "account_name_key").IsUnique();
            entity.HasIndex(e => e.NormalizedName, "account_norm_name_key").IsUnique();

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("account_id");
            entity.Property(e => e.Name).HasColumnName("name");
            entity.Property(e => e.ConcurrencyStamp).HasColumnName("concurrency_stamp");
            entity.Property(e => e.NormalizedName).HasColumnName("normalized_name");
        });

        modelBuilder.Entity<Clinic>(entity =>
        {
            entity.HasKey(e => e.ClinicId).HasName("clinic_pkey");

            entity.ToTable("clinic");

            entity.HasIndex(e => e.Name, "clinic_name_key").IsUnique();

            entity.Property(e => e.ClinicId)
                .UseIdentityAlwaysColumn()
                .HasColumnName("clinic_id");
            entity.Property(e => e.FullAddress).HasColumnName("full_address");
            entity.Property(e => e.Name).HasColumnName("name");
            entity.Property(e => e.PhoneNumber).HasColumnName("phone_number");
            entity.Property(e => e.Voivodeship).HasColumnName("voivodeship");

            entity.HasMany(d => d.Doctors).WithMany(p => p.Clinics)
                .UsingEntity<Dictionary<string, object>>(
                    "ClinicDoctor",
                    r => r.HasOne<Doctor>().WithMany()
                        .HasForeignKey("DoctorId")
                        .HasConstraintName("clinic_doctor_doctor_id_fkey"),
                    l => l.HasOne<Clinic>().WithMany()
                        .HasForeignKey("ClinicId")
                        .HasConstraintName("clinic_doctor_clinic_id_fkey"),
                    j =>
                    {
                        j.HasKey("ClinicId", "DoctorId").HasName("clinic_doctor_pkey");
                        j.ToTable("clinic_doctor");
                    });
        });

        modelBuilder.Entity<Disease>(entity =>
        {
            entity.HasKey(e => e.Icd).HasName("disease_pkey");

            entity.ToTable("disease");

            entity.Property(e => e.Icd).HasColumnName("icd");
            entity.Property(e => e.Name).HasColumnName("name");
            entity.Property(e => e.SpecializationId).HasColumnName("specialization_id");

            entity.HasOne(d => d.Specialization).WithMany(p => p.Diseases)
                .HasForeignKey(d => d.SpecializationId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("disease_specialization_id_fkey");
        });

        modelBuilder.Entity<DiseaseHistory>(entity =>
        {
            entity.HasKey(e => new { e.IllnessDate, e.PatientId, e.DiseaseIcd }).HasName("disease_history_pkey");

            entity.ToTable("disease_history");

            entity.Property(e => e.IllnessDate).HasColumnName("illness_date");
            entity.Property(e => e.PatientId).HasColumnName("patient_id");
            entity.Property(e => e.DiseaseIcd).HasColumnName("disease_icd");

            entity.HasOne(d => d.DiseaseIcdNavigation).WithMany(p => p.DiseaseHistories)
                .HasForeignKey(d => d.DiseaseIcd)
                .HasConstraintName("disease_history_disease_icd_fkey");

            entity.HasOne(d => d.Patient).WithMany(p => p.DiseaseHistories)
                .HasForeignKey(d => d.PatientId)
                .HasConstraintName("disease_history_patient_id_fkey");
        });

        modelBuilder.Entity<Doctor>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("doctor_pkey");

            entity.ToTable("doctor");

            entity.HasIndex(e => e.PwzNumber, "doctor_pwz_number_key").IsUnique();

            entity.Property(e => e.UserId)
                .ValueGeneratedNever()
                .HasColumnName("user_id");
            entity.Property(e => e.Name).HasColumnName("name");
            entity.Property(e => e.PwzNumber).HasColumnName("pwz_number");
            entity.Property(e => e.Surname).HasColumnName("surname");

            entity.HasOne(d => d.User).WithOne(p => p.Doctor)
                .HasForeignKey<Doctor>(d => d.UserId)
                .HasConstraintName("doctor_user_id_fkey");

            entity.HasMany(d => d.Specializations).WithMany(p => p.Doctors)
                .UsingEntity<Dictionary<string, object>>(
                    "DoctorSpecialization",
                    r => r.HasOne<Specialization>().WithMany()
                        .HasForeignKey("SpecializationId")
                        .HasConstraintName("doctor_specialization_specialization_id_fkey"),
                    l => l.HasOne<Doctor>().WithMany()
                        .HasForeignKey("DoctorId")
                        .HasConstraintName("doctor_specialization_doctor_id_fkey"),
                    j =>
                    {
                        j.HasKey("DoctorId", "SpecializationId").HasName("doctor_specialization_pkey");
                        j.ToTable("doctor_specialization");
                    });
        });

        modelBuilder.Entity<Opinion>(entity =>
        {
            entity.HasKey(e => new { e.IssueDate, e.PatientId, e.DoctorId }).HasName("opinion_pkey");

            entity.ToTable("opinion");

            entity.Property(e => e.IssueDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("issue_date");
            entity.Property(e => e.PatientId).HasColumnName("patient_id");
            entity.Property(e => e.DoctorId).HasColumnName("doctor_id");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Stars).HasColumnName("stars");

            entity.HasOne(d => d.Doctor).WithMany(p => p.Opinions)
                .HasForeignKey(d => d.DoctorId)
                .HasConstraintName("opinion_doctor_id_fkey");

            entity.HasOne(d => d.Patient).WithMany(p => p.Opinions)
                .HasForeignKey(d => d.PatientId)
                .HasConstraintName("opinion_patient_id_fkey");
        });

        modelBuilder.Entity<Patient>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("patient_pkey");

            entity.ToTable("patient");

            entity.Property(e => e.UserId)
                .ValueGeneratedNever()
                .HasColumnName("user_id");
            entity.Property(e => e.InsuranceNumber).HasColumnName("insurance_number");

            entity.HasOne(d => d.User).WithOne(p => p.Patient)
                .HasForeignKey<Patient>(d => d.UserId)
                .HasConstraintName("patient_user_id_fkey");

            entity.HasMany(d => d.Doctors).WithMany(p => p.Patients)
                .UsingEntity<Dictionary<string, object>>(
                    "FavouriteDoctor",
                    r => r.HasOne<Doctor>().WithMany()
                        .HasForeignKey("DoctorId")
                        .HasConstraintName("favourite_doctors_doctor_id_fkey"),
                    l => l.HasOne<Patient>().WithMany()
                        .HasForeignKey("PatientId")
                        .HasConstraintName("favourite_doctors_patient_id_fkey"),
                    j =>
                    {
                        j.HasKey("PatientId", "DoctorId").HasName("favourite_doctors_pkey");
                        j.ToTable("favourite_doctors");
                    });
        });

        modelBuilder.Entity<ShowReview>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("show_reviews");

            entity.Property(e => e.DoctorsRating).HasColumnName("doctors_rating");
            entity.Property(e => e.Name).HasColumnName("name");
            entity.Property(e => e.Surname).HasColumnName("surname");
            entity.Property(e => e.UserId).HasColumnName("user_id");
        });

        modelBuilder.Entity<ShowReviewsWithSpecialization>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("show_reviews_with_specialization");

            entity.Property(e => e.DoctorsRating).HasColumnName("doctors_rating");
            entity.Property(e => e.Name).HasColumnName("name");
            entity.Property(e => e.Specialization).HasColumnName("specialization");
            entity.Property(e => e.Surname).HasColumnName("surname");
            entity.Property(e => e.UserId).HasColumnName("user_id");
        });

        modelBuilder.Entity<Specialization>(entity =>
        {
            entity.HasKey(e => e.SpecializationId).HasName("specialization_pkey");

            entity.ToTable("specialization");

            entity.HasIndex(e => e.Name, "specialization_name_key").IsUnique();

            entity.Property(e => e.SpecializationId)
                .UseIdentityAlwaysColumn()
                .HasColumnName("specialization_id");
            entity.Property(e => e.Name).HasColumnName("name");
        });

        modelBuilder.Entity<Symptom>(entity =>
        {
            entity.HasKey(e => e.SymptomId).HasName("symptom_pkey");

            entity.ToTable("symptom");

            entity.HasIndex(e => e.Name, "symptom_name_key").IsUnique();

            entity.Property(e => e.SymptomId)
                .UseIdentityAlwaysColumn()
                .HasColumnName("symptom_id");
            entity.Property(e => e.Name).HasColumnName("name");
        });

        modelBuilder.Entity<SymptomWeight>(entity =>
        {
            entity.HasKey(e => new { e.DiseaseIcd, e.SymptomId }).HasName("symptom_weight_pk");

            entity.ToTable("symptom_weight");

            entity.Property(e => e.DiseaseIcd).HasColumnName("disease_icd");
            entity.Property(e => e.SymptomId).HasColumnName("symptom_id");
            entity.Property(e => e.Weight).HasColumnName("weight");

            entity.HasOne(d => d.DiseaseIcdNavigation).WithMany(p => p.SymptomWeights)
                .HasForeignKey(d => d.DiseaseIcd)
                .HasConstraintName("symptom_weight_disease_icd_fkey");

            entity.HasOne(d => d.Symptom).WithMany(p => p.SymptomWeights)
                .HasForeignKey(d => d.SymptomId)
                .HasConstraintName("symptom_weight_symptom_id_fkey");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("user_pkey");

            entity.ToTable("user");

            entity.HasIndex(e => e.Email, "user_email_key").IsUnique();

            entity.HasIndex(e => e.UserName, "user_user_name_key").IsUnique();
            
            entity.HasIndex(e => e.NormalizedUserName, "user_norm_user_name_key").IsUnique();
            
            entity.HasIndex(e => e.NormalizedEmail, "user_norm_email_key").IsUnique();

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("user_id");
            entity.Property(e => e.AccountType).HasColumnName("account_type");
            entity.Property(e => e.Email).HasColumnName("email");
            entity.Property(e => e.LastLoginTime)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("last_login_time");
            entity.Property(e => e.PasswordHash).HasColumnName("password");
            entity.Property(e => e.RegistrationTime)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("registration_time");
            entity.Property(e => e.UserName).HasColumnName("user_name");
            entity.Property(e => e.ConcurrencyStamp).HasColumnName("concurrency_stamp");
            entity.Property(e => e.NormalizedUserName).HasColumnName("normalized_user_name");
            entity.Property(e => e.NormalizedEmail).HasColumnName("normalized_email");

            entity.HasOne(d => d.AccountTypeNavigation).WithMany(p => p.Users)
                .HasForeignKey(d => d.AccountType)
                .HasConstraintName("user_account_account_id_fk");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
