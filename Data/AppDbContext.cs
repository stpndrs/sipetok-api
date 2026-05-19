using Microsoft.EntityFrameworkCore;
using sipetok_api.Models;

namespace sipetok_api.Data
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Tenant> Tenants { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<EggCategory> EggCategories { get; set; }
        public DbSet<Egg> Eggs { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<TransactionDetail> TransactionDetails { get; set; }
        public DbSet<Operational> Operationals { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 1. Relasi User -> Tenant (One-to-One atau One-to-Many)
            modelBuilder.Entity<Tenant>()
                .HasOne(t => t.User)
                .WithMany()
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Restrict); // Jangan hapus User jika Tenant dihapus

            // 2. Relasi User -> Customer
            modelBuilder.Entity<Customer>()
                .HasOne(c => c.User)
                .WithMany()
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // 3. Relasi Tenant -> Egg (Stok Telur)
            modelBuilder.Entity<Egg>()
                .HasOne(e => e.Tenant)
                .WithMany()
                .HasForeignKey(e => e.TenantId)
                .OnDelete(DeleteBehavior.Cascade); // Jika Tenant tutup, hapus stok telurnya

            // 4. Relasi EggCategory -> Egg
            modelBuilder.Entity<Egg>()
                .HasOne(e => e.Category)
                .WithMany()
                .HasForeignKey(e => e.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            // 5. Relasi Tenant -> Transaction
            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.Tenant)
                .WithMany()
                .HasForeignKey(t => t.TenantId)
                .OnDelete(DeleteBehavior.Restrict);

            // 7. Relasi Transaction -> TransactionDetail (Master-Detail)
            modelBuilder.Entity<TransactionDetail>()
                .HasOne(td => td.Transaction)
                .WithMany(t => t.Details)
                .HasForeignKey(td => td.TransactionId)
                .OnDelete(DeleteBehavior.Cascade); // Jika Transaksi dihapus, detailnya wajib hapus

            // 8. Relasi Tenant -> Operational
            modelBuilder.Entity<Operational>()
                .HasOne(o => o.Tenant)
                .WithMany()
                .HasForeignKey(o => o.TenantId)
                .OnDelete(DeleteBehavior.Cascade);

            // // Mengatur Role menjadi tipe data ENUM di MySQL
            // modelBuilder.Entity<User>()
            //     .Property(u => u.role)
            //     .HasConversion<string>() // Tetap perlu konversi ke string agar C# bisa memetakan nama enum-nya
            //     .HasColumnType("enum('ADMIN', 'TENANT', 'CUSTOMER')");

            // // Mengatur Status menjadi tipe data ENUM di MySQL
            // modelBuilder.Entity<User>()
            //     .Property(u => u.status)
            //     .HasConversion<string>()
            //     .HasColumnType("enum('ACTIVE', 'INACTIVE')");

            // Konfigurasi tambahan untuk tipe data Decimal (Uang)
            modelBuilder.Entity<EggCategory>().Property(e => e.Price).HasPrecision(18, 2);
            modelBuilder.Entity<Transaction>().Property(t => t.PaymentAmount).HasPrecision(18, 2);
            modelBuilder.Entity<Transaction>().Property(t => t.TotalPrice).HasPrecision(18, 2);
            modelBuilder.Entity<TransactionDetail>().Property(td => td.Subtotal).HasPrecision(18, 2);

            modelBuilder.Entity<User>().HasQueryFilter(u => u.DeletedAt == null);
            modelBuilder.Entity<Tenant>().HasQueryFilter(t => t.DeletedAt == null);
            modelBuilder.Entity<Customer>().HasQueryFilter(c => c.DeletedAt == null);
            modelBuilder.Entity<Egg>().HasQueryFilter(e => e.DeletedAt == null);
            modelBuilder.Entity<EggCategory>().HasQueryFilter(ec => ec.DeletedAt == null);
            modelBuilder.Entity<Transaction>().HasQueryFilter(t => t.DeletedAt == null);
            modelBuilder.Entity<TransactionDetail>().HasQueryFilter(td => td.DeletedAt == null);
            modelBuilder.Entity<Operational>().HasQueryFilter(o => o.DeletedAt == null);
        }
        // public override int SaveChanges()
        // {
        //     var entries = ChangeTracker.Entries()
        //         .Where(e => e.Entity is BaseEntity && (e.State == EntityState.Added || e.State == EntityState.Modified));

        //     foreach (var entityEntry in entries)
        //     {
        //         ((BaseEntity)entityEntry.Entity).updated_at = DateTime.Now;

        //         if (entityEntry.State == EntityState.Added)
        //         {
        //             ((BaseEntity)entityEntry.Entity).created_at = DateTime.Now;
        //         }
        //     }
        //     return base.SaveChanges();
        // }
        // public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        // {
        //     var entries = ChangeTracker.Entries()
        //         .Where(e => e.Entity is BaseEntity && (e.State == EntityState.Added || e.State == EntityState.Modified));

        //     foreach (var entityEntry in entries)
        //     {
        //         ((BaseEntity)entityEntry.Entity).updated_at = DateTime.Now;

        //         if (entityEntry.State == EntityState.Added)
        //         {
        //             ((BaseEntity)entityEntry.Entity).created_at = DateTime.Now;
        //         }
        //     }
        //     return base.SaveChangesAsync(cancellationToken);
        // }
    }
}