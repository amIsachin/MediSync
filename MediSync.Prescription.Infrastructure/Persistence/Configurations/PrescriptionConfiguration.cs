using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MediSync.Prescription.Infrastructure.Persistence.Configurations;

public class PrescriptionConfiguration : IEntityTypeConfiguration<MediSync.Prescription.Domain.Aggregates.Prescription>
{
    public void Configure(EntityTypeBuilder<Domain.Aggregates.Prescription> builder)
    {
        builder.ToTable("Prescriptions");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.PatientId)
            .IsRequired();

        builder.Property(p => p.DoctorId)
            .IsRequired();

        builder.Property(p => p.Status)
            .IsRequired()
            .HasMaxLength(20)
            .HasConversion<string>();

        builder.Property(p => p.PrescribedAt).IsRequired();
        builder.Property(p => p.ExpiresAt).IsRequired();
        builder.Property(p => p.Notes).HasMaxLength(500);
        builder.Property(p => p.SupersededById);

        // Value Object: DrugInfo
        builder.OwnsOne(p => p.Drug, drug =>
        {
            drug.Property(d => d.DrugName)
                .HasColumnName("DrugName")
                .HasMaxLength(200)
                .IsRequired();

            drug.Property(d => d.Dosage)
                .HasColumnName("Dosage")
                .HasMaxLength(50)
                .IsRequired();

            drug.Property(d => d.GenericName)
                .HasColumnName("GenericName")
                .HasMaxLength(200);
        });

        // Value Object: DosageInstruction
        builder.OwnsOne(p => p.Dosage, dosage =>
        {
            dosage.Property(d => d.Frequency)
                .HasColumnName("Frequency")
                .HasConversion<string>()
                .HasMaxLength(30)
                .IsRequired();

            dosage.Property(d => d.Route)
                .HasColumnName("RouteOfAdministration")
                .HasConversion<string>()
                .HasMaxLength(30)
                .IsRequired();

            dosage.Property(d => d.DurationDays)
                .HasColumnName("DurationDays")
                .IsRequired();

            dosage.Property(d => d.SpecialInstructions)
                .HasColumnName("SpecialInstructions")
                .HasMaxLength(500);
        });

        // Indexes
        builder.HasIndex(p => p.PatientId)
            .HasDatabaseName("IX_Prescriptions_PatientId");

        builder.HasIndex(p => p.DoctorId)
            .HasDatabaseName("IX_Prescriptions_DoctorId");

        builder.HasIndex(p => p.Status)
            .HasDatabaseName("IX_Prescriptions_Status");
    }
}
