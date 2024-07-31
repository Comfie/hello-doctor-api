using ApiBaseTemplate.Domain.Entities.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ApiBaseTemplate.Infrastructure.Data.Configurations;

public class SuperAdministratorConfiguration : IEntityTypeConfiguration<SuperAdministrator>
{
    public void Configure(EntityTypeBuilder<SuperAdministrator> builder)
    {
        builder.HasOne(accountAdmin => accountAdmin.User)
            .WithOne(account => account.SuperAdministrator)
            .HasForeignKey<SuperAdministrator>(accountAdmin => accountAdmin.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}