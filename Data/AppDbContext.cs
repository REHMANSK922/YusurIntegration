using Microsoft.EntityFrameworkCore;
using YusurIntegration.Models;

namespace YusurIntegration.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Order> Orders { get; set; }
        public DbSet<Activity> Activities { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<TradeDrug> TradeDrugs { get; set; }
        public DbSet<ShippingAddress> ShippingAddress { get; set; }




        public DbSet<OrderStatusHistory> OrderStatusHistory { get; set; }
        public DbSet<WebhookLog> WebhookLogs { get; set; }
        public DbSet<PendingMessage> PendingMessages { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Pharmacies> Pharmacies { get; set; }
        public DbSet<PharmacyGroups> PharmacyGroups { get; set; }

        public DbSet<ApiToken> ApiTokens { get; set; }


        public DbSet<YusurUsers> YusurUsers { get; set; }


        public DbSet<StockTable> StockTable { get; set; }
        public DbSet<ApprovedDrug> ApprovedDrugs { get; set; }
        public DbSet<WasfatyDrugs> WasfatyDrugs { get; set; }





        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasKey(e => e.OrderId); // Auto-generated PK

                // Make OrderId unique if it's a business identifier
                //entity.HasIndex(e => e.OrderId).IsUnique();



                entity.HasOne(e => e.Patient)
                    .WithOne(e => e.Order)
                    .HasForeignKey<Patient>(e => e.OrderId) // Changed to avoid confusion
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.ShippingAddress)
                    .WithOne(e => e.Order)
                    .HasForeignKey<ShippingAddress>(e => e.OrderId) // Changed to avoid confusion
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(e => e.Activities)
                    .WithOne(e => e.Order)
                    .HasForeignKey(e => e.OrderId) // Changed to avoid confusion
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Activity Configuration
            modelBuilder.Entity<Activity>(entity =>
            {
                entity.HasKey(e => e.Id); // Auto-generated PK

                // Make ActivityId unique if it's a business identifier
                ////entity.HasIndex(e => e.ActivityId).IsUnique();

                entity.HasIndex(e => new { e.OrderId, e.ActivityId })
                .IsUnique();


                entity.HasMany(e => e.TradeDrugs)
                    .WithOne(e => e.Activity)
                    .HasForeignKey(e => e.ActivityForeignId) // Changed to avoid confusion
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Patient Configuration
            modelBuilder.Entity<Patient>(entity =>
            {
                entity.HasKey(e => e.Id);
            });

            // ShippingAddress Configuration
            modelBuilder.Entity<ShippingAddress>(entity =>
            {
                entity.HasKey(e => e.Id);
            });

            // TradeDrug Configuration
            modelBuilder.Entity<TradeDrug>(entity =>
            {

                entity.HasKey(e => e.Id);
            });





            // after adding Id in all tables to avoid duplicate key issues

            // Order Configuration
            //modelBuilder.Entity<Order>(entity =>
            //{
            //    entity.HasKey(e => e.OrderId);

            //    entity.HasOne(e => e.Patient)
            //        .WithOne(e => e.Order)
            //        .HasForeignKey<Patient>(e => e.OrderId)
            //        .OnDelete(DeleteBehavior.Cascade);

            //    entity.HasOne(e => e.ShippingAddress)
            //        .WithOne(e => e.Order)
            //        .HasForeignKey<ShippingAddress>(e => e.OrderId)
            //        .OnDelete(DeleteBehavior.Cascade);

            //    entity.HasMany(e => e.Activities)
            //        .WithOne(e => e.Order)
            //        .HasForeignKey(e => e.OrderId)
            //        .OnDelete(DeleteBehavior.Cascade);
            //});

            //// Activity Configuration
            //modelBuilder.Entity<Activity>(entity =>
            //{
            //    entity.HasKey(e => e.ActivityId);

            //    entity.HasMany(e => e.TradeDrugs)
            //        .WithOne(e => e.Activity)
            //        .HasForeignKey(e => e.ActivityId)
            //        .OnDelete(DeleteBehavior.Cascade);
            //});

            //// Patient Configuration
            //modelBuilder.Entity<Patient>(entity =>
            //{
            //    entity.HasKey(e => e.OrderId);
            //});

            //// ShippingAddress Configuration
            //modelBuilder.Entity<ShippingAddress>(entity =>
            //{
            //    entity.HasKey(e => e.OrderId);
            //    entity.OwnsOne(e => e.Coordinates);
            //});

            //// TradeDrug Configuration
            //modelBuilder.Entity<TradeDrug>(entity =>
            //{
            //    entity.HasKey(e => e.Id);
            //});


            //modelBuilder.Entity<Order>().HasKey(o => o.Id);
            //modelBuilder.Entity<Activity>().HasKey(a => a.Id);
            //modelBuilder.Entity<TradeDrugs>().HasKey(t => t.Id);
            //modelBuilder.Entity<OrderStatusHistory>().HasKey(s => s.Id);
            //modelBuilder.Entity<WebhookLog>().HasKey(w => w.Id);
            //modelBuilder.Entity<Activity>().HasOne(a => a.Order).WithMany(o => o.Activities).HasForeignKey(a => a.OrderId);
            //modelBuilder.Entity<TradeDrugs>().HasOne(t => t.Activity).WithMany(a => a.TradeDrugs).HasForeignKey(t => t.ActivityId);






            /*

                        // 1. Order -> Patient (1:1)
                        modelBuilder.Entity<Order>()
                            .HasOne(o => o.Patient)
                            .WithOne(p => p.Order)
                            .HasForeignKey<Patient>(p => p.OrderId)
                            .OnDelete(DeleteBehavior.Cascade);

                        // 2. Order -> ShippingAddress (1:1)
                        modelBuilder.Entity<Order>()
                            .HasOne(o => o.ShippingAddress)
                            .WithOne(s => s.Order)
                            .HasForeignKey<ShippingAddress>(s => s.OrderId)
                            .OnDelete(DeleteBehavior.Cascade);


                        // Configure Order -> Activities (1:Many)
                        modelBuilder.Entity<Activity>()
                            .HasOne(a => a.Order)  // Configure from Activity side
                            .WithMany(o => o.Activities)
                            .HasForeignKey(a => a.OrderId)
                            .OnDelete(DeleteBehavior.Cascade);


                        // Configure Activity -> TradeDrugs (1:Many)
                        modelBuilder.Entity<TradeDrugs>()
                            .HasOne(t => t.Activity)  // Configure from TradeDrugs side
                            .WithMany(a => a.TradeDrugs)
                            .HasForeignKey(t => t.ActivityId)
                            .OnDelete(DeleteBehavior.Cascade);



                        ////// 3. Order -> Activities (1:Many)
                        ////modelBuilder.Entity<Order>()

                        ////    .HasMany(o => o.Activities)
                        ////    .WithOne(a => a.Order) // Activity doesn't need a virtual Order property unless you want it
                        ////    .HasForeignKey(a => a.OrderId)
                        ////    .OnDelete(DeleteBehavior.Cascade);

                        //// 4. Activity -> TradeDrugs (1:Many)

                        //modelBuilder.Entity<Activity>()
                        //    .HasMany(a => a.TradeDrugs)
                        //    .WithOne(t => t.Activity)
                        //    .HasForeignKey(t => t.ActivityId)
                        //    .OnDelete(DeleteBehavior.Cascade);

                        // 5. Owned Type for Coordinates
                        modelBuilder.Entity<ShippingAddress>()
                            .OwnsOne(s => s.Coordinates);

                        modelBuilder.Entity<Activity>()
                   .HasIndex(a => a.OrderId)
                   .IsUnique(false);  // This is a regular non-unique index




                        //// 1. One-to-One: Order -> Patient
                        //modelBuilder.Entity<Order>()
                        //    .HasOne(o => o.Patient)
                        //    .WithOne()
                        //    .HasForeignKey<Patient>(p => p.OrderId)
                        //    .OnDelete(DeleteBehavior.Cascade);


                        //// 2. One-to-Many: Order -> Activity
                        //modelBuilder.Entity<Activity>()
                        //    .HasOne(a => a.Order)
                        //    .WithMany(o => o.Activities)
                        //    .HasForeignKey(a => a.OrderId);

                        ////modelBuilder.Entity<Order>()
                        ////.HasMany(o => o.Activities)
                        ////.WithOne()
                        ////.HasForeignKey(a => a.OrderId)
                        ////.OnDelete(DeleteBehavior.Cascade);





                        //// 3. One-to-One: Order -> ShippingAddress
                        //modelBuilder.Entity<Order>()
                        //    .HasOne(o => o.ShippingAddress)
                        //    .WithOne()
                        //    .HasForeignKey<ShippingAddress>(s => s.OrderId);

                        //// 4. Owned Type Configuration (Coordinates inside ShippingAddress)
                        //modelBuilder.Entity<ShippingAddress>()
                        //    .OwnsOne(s => s.Coordinates);




                        /*

                        // 5. Firebird Specific: Guid handling
                        // Many Firebird EF providers require Guids to be stored as strings 
                        // or specialized binary types to avoid index issues.
                        modelBuilder.Entity<Activity>()
                            .Property(a => a.Id)
                            .HasDefaultValueSql("UUID_TO_CHAR(GEN_UUID())"); // If using Firebird 4.0+

                        */


            // 6. Limits on String Lengths
            // Firebird has a limit on index sizes. It's good practice to set max lengths.

            //modelBuilder.Entity<Order>().Property(o => o.OrderId).HasMaxLength(50);
            //modelBuilder.Entity<Patient>().Property(p => p.nationalId).HasMaxLength(20);    





            modelBuilder.Entity<PendingMessage>().HasKey(p => p.Id);
            modelBuilder.Entity<PendingMessage>().HasIndex(p => p.MessageId).IsUnique();




            modelBuilder.Entity<User>().HasKey(u => u.Id);
            modelBuilder.Entity<User>().HasIndex(u => u.Username).IsUnique();

            modelBuilder.Entity<Pharmacies>().HasKey(p => p.Id);
            modelBuilder.Entity<PharmacyGroups>().HasKey(pg => pg.Id);


            modelBuilder.Entity<ApiToken>().HasKey(p => p.Id);


            modelBuilder.Entity<YusurUsers>().HasKey(p => p.Id);



            modelBuilder.Entity<StockTable>().HasKey(s => s.Id);
            modelBuilder.Entity<StockTable>().HasIndex(s => new { s.ItemNo, s.BranchLicense, s.GenericCode }).IsUnique();

            modelBuilder.Entity<ApprovedDrug>().HasKey(a => a.Id);
            modelBuilder.Entity<WasfatyDrugs>().HasKey(a => new { a.DrugId });



            //modelBuilder.Entity<ApiToken>(entity =>
            //{
            //    entity.ToTable("ApiToken");
            //    entity.HasKey(e => e.Id);
            //    entity.Property(e => e.Id).HasColumnName("ID");
            //    entity.Property(e => e.TokenType).HasColumnName("TokenType").HasMaxLength(50);
            //    entity.Property(e => e.AccessToken).HasColumnName("ACCESS_TOKEN").HasMaxLength(2000);
            //    entity.Property(e => e.Username).HasColumnName("USERNAME").HasMaxLength(100);
            //    entity.Property(e => e.CreatedDate).HasColumnName("CREATED_DATE");
            //    entity.Property(e => e.ExpiresAt).HasColumnName("EXPIRES_AT");
            //    entity.Property(e => e.IsValid).HasColumnName("IS_VALID");
            //    entity.Property(e => e.LastUsed).HasColumnName("LAST_USED");

            //    entity.HasIndex(e => e.TokenType).HasDatabaseName("IDX_API_TOKENS_TYPE");
            //});



        }
    }
}
