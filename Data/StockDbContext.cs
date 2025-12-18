using Microsoft.EntityFrameworkCore;
using YusurIntegration.Models;
namespace YusurIntegration.Data
{
    public class StockDbContext : DbContext
    {
        public StockDbContext(DbContextOptions<StockDbContext> opt) : base(opt) { }
        public DbSet<StockTable> StockTable { get; set; }
        public DbSet<ApprovedDrug> ApprovedDrugs { get; set; }
        public DbSet<WasfatyDrugs> WasfatyDrugs { get; set; }

        protected override void OnModelCreating(ModelBuilder b)
        {
            b.Entity<StockTable>().HasKey(s => s.Id);
            b.Entity<StockTable>().HasIndex(s => new { s.ItemNo, s.BranchLicense, s.GenericCode }).IsUnique();
            b.Entity<ApprovedDrug>().HasKey(a => a.Id);
            b.Entity<WasfatyDrugs>().HasKey(a => new { a.DrugId});


        }

    }
}
