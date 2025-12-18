using Microsoft.EntityFrameworkCore;
using YusurIntegration.Models;

namespace YusurIntegration.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Order> Orders { get; set; }
        public DbSet<Activity> Activities { get; set; }
        public DbSet<TradeDrugOption> TradeDrugOptions { get; set; }
        public DbSet<OrderStatusHistory> OrderStatusHistory { get; set; }
        public DbSet<WebhookLog> WebhookLogs { get; set; }
        public DbSet<PendingMessage> PendingMessages { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Pharmacies> Pharmacies { get; set; }
        public DbSet<PharmacyGroups> PharmacyGroups { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Order>().HasKey(o => o.Id);
            modelBuilder.Entity<Activity>().HasKey(a => a.Id);
            modelBuilder.Entity<TradeDrugOption>().HasKey(t => t.Id);
            modelBuilder.Entity<OrderStatusHistory>().HasKey(s => s.Id);
            modelBuilder.Entity<WebhookLog>().HasKey(w => w.Id);
            modelBuilder.Entity<Activity>().HasOne(a => a.Order).WithMany(o => o.Activities).HasForeignKey(a => a.OrderId);
            modelBuilder.Entity<TradeDrugOption>().HasOne(t => t.Activity).WithMany(a => a.TradeDrugOptions).HasForeignKey(t => t.ActivityId);
            modelBuilder.Entity<PendingMessage>().HasKey(p => p.Id);
            modelBuilder.Entity<PendingMessage>().HasIndex(p => p.MessageId).IsUnique();

            modelBuilder.Entity<User>().HasKey(u => u.Id);
            modelBuilder.Entity<User>().HasIndex(u => u.Username).IsUnique();

            modelBuilder.Entity<Pharmacies>().HasKey(p => p.Id);
            modelBuilder.Entity<PharmacyGroups>().HasKey(pg => pg.Id);

        }
    }
}
