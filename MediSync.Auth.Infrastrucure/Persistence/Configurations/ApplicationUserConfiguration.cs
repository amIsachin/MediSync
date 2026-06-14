using MediSync.Auth.Infrastrucure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MediSync.Auth.Infrastrucure.Persistence.Configurations;

public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.Property(r => r.Role).HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(s => s.Status).HasConversion<string>()
            .HasMaxLength(20);
    }
}
