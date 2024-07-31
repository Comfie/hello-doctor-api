using ApiBaseTemplate.Domain.Entities.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ApiBaseTemplate.Infrastructure.Data.Configurations;

public class SystemAdministratorConfiguration : IEntityTypeConfiguration<SystemAdministrator>
{
    public void Configure(EntityTypeBuilder<SystemAdministrator> builder)
    {
        builder
            .HasOne(o => o.User)
            .WithOne(account => account.SystemAdministrator)
            .HasForeignKey<SystemAdministrator>(o => o.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}