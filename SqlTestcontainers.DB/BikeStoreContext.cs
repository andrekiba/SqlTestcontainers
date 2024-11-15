using Microsoft.EntityFrameworkCore;

namespace SqlTestcontainers.DB;

public class BikeStoreContext : DbContext
{
    public DbSet<Bike> Bikes { get; set; }
    public DbSet<BikeLike> BikeLikes { get; set; }
    public DbSet<BikeReview> BikeReviews { get; set; }

    public BikeStoreContext() { }
    public BikeStoreContext(DbContextOptions<BikeStoreContext> options) : base(options) { }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) { }
}