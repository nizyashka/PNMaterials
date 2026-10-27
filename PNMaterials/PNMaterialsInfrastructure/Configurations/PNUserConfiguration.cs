using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PNMaterialsInfrastructure.Identity;

namespace PNMaterialsInfrastructure.Configurations;

public class PNUserConfiguration : IEntityTypeConfiguration<PNUser>
{
    public void Configure(EntityTypeBuilder<PNUser> builder)
    {
        builder.Property(m => m.FirstName)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(m => m.LastName)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(m => m.MiddleName)
            .HasMaxLength(50);
    }
}
