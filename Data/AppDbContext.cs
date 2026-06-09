using Microsoft.EntityFrameworkCore;
using sipetok_api.Models;

namespace sipetok_api.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // 1. Daftarkan semua Model menjadi DbSet agar menjadi tabel di Database
        public DbSet<User> Users { get; set; }
        public DbSet<Tenant> Tenants { get; set; }
        public DbSet<EggCategory> EggCategories { get; set; }
        public DbSet<Egg> Eggs { get; set; }
        public DbSet<Operational> Operationals { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<TransactionDetail> TransactionDetails { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 2. Konfigurasi Global Query Filter untuk Soft Delete (BaseEntity)
            // Ini otomatis menyaring data yang DeletedAt != null agar tidak ikut ke-load
            modelBuilder.Entity<User>().HasQueryFilter(e => e.DeletedAt == null);
            modelBuilder.Entity<Tenant>().HasQueryFilter(e => e.DeletedAt == null);
            modelBuilder.Entity<EggCategory>().HasQueryFilter(e => e.DeletedAt == null);
            modelBuilder.Entity<Egg>().HasQueryFilter(e => e.DeletedAt == null);
            modelBuilder.Entity<Operational>().HasQueryFilter(e => e.DeletedAt == null);
            modelBuilder.Entity<Transaction>().HasQueryFilter(e => e.DeletedAt == null);
            modelBuilder.Entity<TransactionDetail>().HasQueryFilter(e => e.DeletedAt == null);

            // 3. Konfigurasi Relasi & Presisi Data (Fluent API)

            // EggCategory -> Price menggunakan decimal (set presisi di DB agar aman)
            modelBuilder.Entity<EggCategory>()
                .Property(ec => ec.Price)
                .HasPrecision(18, 2);

            // Transaction -> Price & Payment Amount menggunakan decimal
            modelBuilder.Entity<Transaction>()
                .Property(t => t.TotalPrice)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Transaction>()
                .Property(t => t.PaymentAmount)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Transaction>()
                .HasOne(ec => ec.Tenant)
                .WithMany()
                .HasForeignKey(ec => ec.TenantId)
                .OnDelete(DeleteBehavior.Restrict);

            // TransactionDetail -> PriceAtPurchase & Subtotal menggunakan decimal
            modelBuilder.Entity<TransactionDetail>()
                .Property(td => td.PriceAtPurchase)
                .HasPrecision(18, 2);

            modelBuilder.Entity<TransactionDetail>()
                .Property(td => td.Subtotal)
                .HasPrecision(18, 2);

            // Mencegah Cascade Delete melingkar jika diperlukan (Opsional tapi aman)
            // Contoh: Jika Tenant dihapus, kita bisa atur apakah EggCategory ikut terhapus otomatis atau tidak
            modelBuilder.Entity<EggCategory>()
                .HasOne(ec => ec.Tenant)
                .WithMany()
                .HasForeignKey(ec => ec.TenantId)
                .OnDelete(DeleteBehavior.Restrict);
        }

        // 4. Otomatisasi isi CreatedAt dan UpdatedAt sebelum simpan data
        public override int SaveChanges()
        {
            ProcessInterceptors();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            ProcessInterceptors();
            return base.SaveChangesAsync(cancellationToken);
        }

        private void ProcessInterceptors()
        {
            var entries = ChangeTracker.Entries()
                .Where(e => e.Entity is BaseEntity && (e.State == EntityState.Added || e.State == EntityState.Modified));

            foreach (var entry in entries)
            {
                var entity = (BaseEntity)entry.Entity;

                if (entry.State == EntityState.Added)
                {
                    entity.CreatedAt = DateTime.Now;
                }
                else if (entry.State == EntityState.Modified)
                {
                    entity.UpdateTimestamps();
                }
            }
        }
    }
}