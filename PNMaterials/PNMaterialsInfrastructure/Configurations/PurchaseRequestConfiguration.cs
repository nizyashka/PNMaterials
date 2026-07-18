using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PNMaterialsDomain.Entities;

namespace PNMaterialsInfrastructure.Configurations;

public class PurchaseRequestConfiguration : IEntityTypeConfiguration<PurchaseRequest>
{
    public void Configure(EntityTypeBuilder<PurchaseRequest> builder)
    {
        builder.ToTable("PurchaseRequests");
        builder.HasKey(r => r.Id);

        builder.Property(r => r.Number)
            .IsRequired()
            .HasMaxLength(10);

        builder.HasIndex(r => r.Number)
            .IsUnique();

        builder.Property(r => r.CreatedAt).IsRequired();
        builder.Property(r => r.DeliveryDate).IsRequired();
        builder.Property(r => r.Status).IsRequired();

        builder.HasMany(r => r.Items)
            .WithOne(i => i.Request)
            .HasForeignKey(i => i.RequestId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}