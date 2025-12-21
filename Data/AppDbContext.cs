using Microsoft.EntityFrameworkCore;
using YusurIntegration.Models;

namespace YusurIntegration.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Order> Orders { get; set; }

        public DbSet<Patient> Patients { get; set; }
        public DbSet<Activity> Activities { get; set; }
        public DbSet<TradeDrugs> TradeDrugOptions { get; set; }
        public DbSet<ShippingAddress> ShippingAddress { get; set; }

        public DbSet<OrderStatusHistory> OrderStatusHistory { get; set; }
        public DbSet<WebhookLog> WebhookLogs { get; set; }
        public DbSet<PendingMessage> PendingMessages { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Pharmacies> Pharmacies { get; set; }
        public DbSet<PharmacyGroups> PharmacyGroups { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<Order>().HasKey(o => o.Id);
            //modelBuilder.Entity<Activity>().HasKey(a => a.Id);
            //modelBuilder.Entity<TradeDrugs>().HasKey(t => t.Id);
            //modelBuilder.Entity<OrderStatusHistory>().HasKey(s => s.Id);
            //modelBuilder.Entity<WebhookLog>().HasKey(w => w.Id);
            //modelBuilder.Entity<Activity>().HasOne(a => a.Order).WithMany(o => o.Activities).HasForeignKey(a => a.OrderId);
            //modelBuilder.Entity<TradeDrugs>().HasOne(t => t.Activity).WithMany(a => a.TradeDrugs).HasForeignKey(t => t.ActivityId);



            // 1. One-to-One: Order -> Patient
            modelBuilder.Entity<Order>()
                .HasOne(o => o.Patient)
                .WithOne()
                .HasForeignKey<Patient>(p => p.OrderId)
                .OnDelete(DeleteBehavior.Cascade);


            // 2. One-to-Many: Order -> Activity
            modelBuilder.Entity<Activity>()
                .HasOne(a => a.Order)
                .WithMany(o => o.Activities)
                .HasForeignKey(a => a.OrderId);

            // 3. One-to-One: Order -> ShippingAddress
            modelBuilder.Entity<Order>()
                .HasOne(o => o.ShippingAddress)
                .WithOne()
                .HasForeignKey<ShippingAddress>(s => s.OrderId);

            // 4. Owned Type Configuration (Coordinates inside ShippingAddress)
            modelBuilder.Entity<ShippingAddress>()
                .OwnsOne(s => s.Coordinates);

            // 5. Firebird Specific: Guid handling
            // Many Firebird EF providers require Guids to be stored as strings 
            // or specialized binary types to avoid index issues.
            modelBuilder.Entity<Activity>()
                .Property(a => a.Id)
                .HasDefaultValueSql("UUID_TO_CHAR(GEN_UUID())"); // If using Firebird 4.0+

            // 6. Limits on String Lengths
            // Firebird has a limit on index sizes. It's good practice to set max lengths.
            modelBuilder.Entity<Order>().Property(o => o.OrderId).HasMaxLength(50);
            modelBuilder.Entity<Patient>().Property(p => p.nationalId).HasMaxLength(20);    



            modelBuilder.Entity<PendingMessage>().HasKey(p => p.Id);
            modelBuilder.Entity<PendingMessage>().HasIndex(p => p.MessageId).IsUnique();

            modelBuilder.Entity<User>().HasKey(u => u.Id);
            modelBuilder.Entity<User>().HasIndex(u => u.Username).IsUnique();

            modelBuilder.Entity<Pharmacies>().HasKey(p => p.Id);
            modelBuilder.Entity<PharmacyGroups>().HasKey(pg => pg.Id);

        }
    }
}
