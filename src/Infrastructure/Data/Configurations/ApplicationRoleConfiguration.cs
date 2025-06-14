using HelloDoctorApi.Domain.Entities.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HelloDoctorApi.Infrastructure.Data.Configurations;

public class ApplicationRoleConfiguration : IEntityTypeConfiguration<ApplicationRole>
{
    public void Configure(EntityTypeBuilder<ApplicationRole> builder)
    {
        builder.ToTable(name: "AspNetRoles");
        
        builder.Property(r => r.IsDeleted)
            .HasDefaultValue(false);

        builder.Property(r => r.DeletedAt)
            .IsRequired(false);
    }
}