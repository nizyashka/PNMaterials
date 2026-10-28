using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PNMaterialsInfrastructure.Identity;

namespace PNMaterialsInfrastructure.Configurations;

public class PNUserConfiguration : IEntityTypeConfiguration<PNUser>
{
    public void Configure(EntityTypeBuilder<PNUser> builder)
    {
        builder.Property(u => u.FirstName)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(u => u.LastName)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(u => u.MiddleName)
            .HasMaxLength(50);
    }
}
