using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PNMaterialsDomain.Entities;

namespace PNMaterialsInfrastructure.Configurations;

public class MaterialGroupConfiguration : IEntityTypeConfiguration<MaterialGroup>
{
    public void Configure(EntityTypeBuilder<MaterialGroup> builder)
    {
        builder.ToTable("MaterialGroups");
        builder.HasKey(g => g.Id);

        builder.Property(g => g.Name)
            .IsRequired()
            .HasMaxLength(40);

        builder.Property(g => g.Description)
            .HasMaxLength(200);
    }
}