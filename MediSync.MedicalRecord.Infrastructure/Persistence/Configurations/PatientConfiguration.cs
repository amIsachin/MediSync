using MediSync.MedicalRecord.Domain.Aggregates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MediSync.MedicalRecord.Infrastructure.Persistence.Configurations;

public class PatientConfiguration : IEntityTypeConfiguration<Patient>
{
    public void Configure(EntityTypeBuilder<Patient> builder)
    {
        // Table name
        builder.ToTable("Patients");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.UserId)
            .IsRequired();

        builder.Property(e => e.Email)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(p => p.PhoneNumber)
            .HasMaxLength(15);

        builder.Property(b => b.BloodGroup)
            .HasConversion<string>()
            .HasMaxLength(15)
            .IsRequired();

        builder.Property(p => p.Gender)
           .HasConversion<string>()
           .HasMaxLength(20)
           .IsRequired();

        builder.Property(p => p.Status)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(p => p.CreatedAt).IsRequired();
        builder.Property(p => p.UpdatedAt).IsRequired(false);

        // Value Object: FullName (owned entity)
        // Stored as columns inside Patients table — no separate table
        builder.OwnsOne(p => p.FullName, fn =>
        {
            fn.Property(f => f.FirstName)
                .HasColumnName("FirstName")
                .HasMaxLength(100)
                .IsRequired();

            fn.Property(f => f.LastName)
               .HasColumnName("LastName")
               .HasMaxLength(100)
               .IsRequired();
        });

        builder.OwnsOne(p => p.DateOfBirth, dob =>
        {
            dob.Property(d => d.Value)
                .HasColumnName("DateOfBirth")
                .IsRequired();
        });

        builder.HasIndex(p => p.UserId)
            .IsUnique()
            .HasDatabaseName("IX_Patients_UserId");

        builder.HasIndex(p => p.Email)
            .IsUnique()
            .HasDatabaseName("IX_Patients_Email");

        // Owned Collection: Allergies
        builder.OwnsMany(p => p.Allergies, allergy =>
        {
            allergy.ToTable("Allergies");

            // Tell EF Core to use private field _allergies
            allergy.UsePropertyAccessMode(PropertyAccessMode.Field);

            allergy.WithOwner().HasForeignKey("PatientId"); // Shadow property for foreign key

            allergy.HasKey("Id"); // Shadow property for primary key

            allergy.Property(a => a.Substance)
                .HasMaxLength(200)
                .IsRequired();

            allergy.Property(a => a.Severity)
                .HasConversion<string>()
                .HasMaxLength(20)
                .IsRequired();

            allergy.Property(a => a.Notes)
                .HasMaxLength(500);

            allergy.Property(a => a.RecordedBy).IsRequired();
            allergy.Property(a => a.RecordedAt).IsRequired();
        });

        // Owned Collection: Diagnoses
        builder.OwnsMany(p => p.Diagnoses, diagnosis =>
        {
            diagnosis.ToTable("Diagnoses");

            diagnosis.UsePropertyAccessMode(PropertyAccessMode.Field);

            diagnosis.WithOwner().HasForeignKey("PatientId");

            diagnosis.HasKey(d => d.Id);

            // IcdCode is a Value Object inside Diagnosis
            // Stored as a single column in Diagnoses table
            diagnosis.OwnsOne(d => d.IcdCode, icd =>
            {
                icd.Property(i => i.Value)
                    .HasColumnName("IcdCode")
                    .HasMaxLength(10)
                    .IsRequired();
            });

            diagnosis.Property(d => d.Description)
                .HasMaxLength(500)
                .IsRequired();

            diagnosis.Property(d => d.DoctorId).IsRequired();
            diagnosis.Property(d => d.DiagnosedAt).IsRequired();
            diagnosis.Property(d => d.IsActive).IsRequired();
        });

        // Owned Collection: Encounters
        builder.OwnsMany(p => p.Encounters, encounter =>
        {
            encounter.ToTable("Encounters");

            encounter.UsePropertyAccessMode(PropertyAccessMode.Field);

            encounter.WithOwner().HasForeignKey("PatientId");

            encounter.HasKey(e => e.Id);

            encounter.Property(e => e.DoctorId).IsRequired();
            encounter.Property(e => e.VisitDate).IsRequired();

            encounter.Property(e => e.Type)
                .HasConversion<string>()
                .HasMaxLength(30)
                .IsRequired();

            encounter.Property(e => e.ChiefComplaint)
                .HasMaxLength(500)
                .IsRequired();

            encounter.Property(e => e.Notes)
                .HasMaxLength(1000);

            encounter.Property(e => e.Facility)
                .HasMaxLength(200);
        });

        builder.Navigation(p => p.Allergies)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.Navigation(p => p.Diagnoses)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.Navigation(p => p.Encounters)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

    }
}
