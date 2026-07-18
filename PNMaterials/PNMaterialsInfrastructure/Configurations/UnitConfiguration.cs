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
            new Unit { Id = 1, Name = "штука" },
            new Unit { Id = 2, Name = "кВт" },
            new Unit { Id = 3, Name = "кг" },
            new Unit { Id = 4, Name = "миллиметр" },
            new Unit { Id = 5, Name = "метр" },
            new Unit { Id = 6, Name = "литр" },
            new Unit { Id = 7, Name = "мм2" },
            new Unit { Id = 8, Name = "м2" },
            new Unit { Id = 9, Name = "м3" },
            new Unit { Id = 10, Name = "л/мин" },
            new Unit { Id = 11, Name = "м3/час" }
        );
    }
}