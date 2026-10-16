using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PNMaterialsDomain.Entities;

namespace PNMaterialsInfrastructure.Configurations;

public class UnitConfiguration : IEntityTypeConfiguration<Unit>
{
    public void Configure(EntityTypeBuilder<Unit> builder)
    {
        builder.ToTable("Units");
        builder.HasKey(u => u.Id);

        builder.Property(u => u.Name)
            .IsRequired()
            .HasMaxLength(20);

        builder.HasData(
            new Unit { Id = 1, Name = "штука", AllowsFractions = false },
            new Unit { Id = 2, Name = "кВт", AllowsFractions = true },
            new Unit { Id = 3, Name = "кг", AllowsFractions = true },
            new Unit { Id = 4, Name = "миллиметр", AllowsFractions = false },
            new Unit { Id = 5, Name = "метр", AllowsFractions = true },
            new Unit { Id = 6, Name = "литр", AllowsFractions = true },
            new Unit { Id = 7, Name = "мм2", AllowsFractions = true },
            new Unit { Id = 8, Name = "м2", AllowsFractions = true },
            new Unit { Id = 9, Name = "м3", AllowsFractions = true },
            new Unit { Id = 10, Name = "л/мин", AllowsFractions = true },
            new Unit { Id = 11, Name = "м3/час", AllowsFractions = true }
        );
    }
}