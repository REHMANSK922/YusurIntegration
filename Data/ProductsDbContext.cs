using Microsoft.EntityFrameworkCore;
using YusurIntegration.Models;

namespace YusurIntegration.Data
{
    public class ProductsDbContext : DbContext
    {
        public ProductsDbContext(DbContextOptions<ProductsDbContext> opt) : base(opt) { }
       
        public DbSet<StockTable> StockTable { get; set; }
        public DbSet<ApprovedDrug> ApprovedItems { get; set; }

        protected override void OnModelCreating(ModelBuilder b)
        {
            b.Entity<StockTable>().HasKey(s => s.Id);
            b.Entity<StockTable>().HasIndex(s => new { s.ItemNo, s.BranchLicense, s.GenericCode }).IsUnique();
            b.Entity<ApprovedDrug>().HasKey(a => a.Id);
            b.Entity<ApprovedDrug>().HasIndex(a => new { a.ItemNo, a.GenericCode }).IsUnique();
        }

    }
}

