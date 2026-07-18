using Microsoft.EntityFrameworkCore;
using PNMaterialsDomain.Entities;

namespace PNMaterialsInfrastructure;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Unit> Units => Set<Unit>();
    public DbSet<MaterialGroup> MaterialGroups => Set<MaterialGroup>();
    public DbSet<Material> Materials => Set<Material>();
    public DbSet<PurchaseRequest> PurchaseRequests => Set<PurchaseRequest>();
    public DbSet<PurchaseRequestItem> PurchaseRequestItems => Set<PurchaseRequestItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}