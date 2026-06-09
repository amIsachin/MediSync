using MediSync.Auth.Domain.Aggregates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MediSync.Auth.Infrastrucure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        // Map to "Users" table — explicit, not relying on convention
        builder.ToTable("Users");

        // Primary key
        builder.HasKey(u => u.Id);

        builder.Property(u => u.FirstName)
            .HasMaxLength(100) // prevents unbounded string columns
            .IsRequired();     // NOT NULL in database

        builder.Property(u => u.LastName)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(u => u.Email)
            .HasMaxLength(256) // RFC 5321 max email length
            .IsRequired();

        // Store Role as STRING — "Doctor" not 2
        // Decision made earlier — self-documenting data, safe against enum reordering
        builder.Property(u => u.Role)
            .HasConversion<string>()  // enum → string conversion
            .HasMaxLength(50)
            .IsRequired();

        // Store Status as STRING for same reasons
        builder.Property(u => u.Status)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(u => u.CreatedAt).IsRequired();
        builder.Property(u => u.UpdatedAt).IsRequired(false); // nullable — OK

        // Unique index on Email — database enforces uniqueness
        // Even if application code has a bug, database prevents duplicates
        builder.HasIndex(u => u.Email)
            .IsUnique()
            .HasDatabaseName("IX_Users_Email");
    }
}
