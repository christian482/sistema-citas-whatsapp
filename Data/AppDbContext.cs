using Microsoft.EntityFrameworkCore;
using citas.Models;

namespace citas.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Business> Businesses { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Appointment> Appointments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            // Configuración compatible con MySQL y SQL Server
            // Configurar longitud máxima de strings para compatibilidad con índices MySQL
            modelBuilder.Entity<Business>(entity =>
            {
                entity.Property(e => e.Name).HasMaxLength(255).IsRequired();
                entity.Property(e => e.WorkingHours).HasMaxLength(100).IsRequired();
                entity.Property(e => e.WorkingDays).HasMaxLength(100).IsRequired();
            });

            modelBuilder.Entity<Service>(entity =>
            {
                entity.Property(e => e.Name).HasMaxLength(255).IsRequired();
            });

            modelBuilder.Entity<Doctor>(entity =>
            {
                entity.Property(e => e.Name).HasMaxLength(255).IsRequired();
                entity.Property(e => e.Specialty).HasMaxLength(255).IsRequired();
            });

            modelBuilder.Entity<Client>(entity =>
            {
                entity.Property(e => e.Name).HasMaxLength(255).IsRequired();
                entity.Property(e => e.Phone).HasMaxLength(50).IsRequired();
                entity.Property(e => e.Email).HasMaxLength(255).IsRequired();
                
                // Índices únicos
                entity.HasIndex(e => e.Phone).IsUnique().HasDatabaseName("IX_Clients_Phone");
                entity.HasIndex(e => e.Email).IsUnique().HasDatabaseName("IX_Clients_Email");
            });

            modelBuilder.Entity<Appointment>(entity =>
            {
                // Configurar fechas
                entity.Property(e => e.Date).IsRequired();
                entity.Property(e => e.IsCancelled).HasDefaultValue(false);
                
                // Índices para optimización
                entity.HasIndex(e => e.Date).HasDatabaseName("IX_Appointments_Date");
                entity.HasIndex(e => new { e.Date, e.BusinessId }).HasDatabaseName("IX_Appointments_Date_Business");
                entity.HasIndex(e => new { e.DoctorId, e.Date }).HasDatabaseName("IX_Appointments_Doctor_Date");
            });

            // Configurar relaciones - compatible con MySQL y SQL Server
            // Evitar cascadas múltiples que pueden causar problemas
            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Client)
                .WithMany(c => c.Appointments)
                .HasForeignKey(a => a.ClientId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Service)
                .WithMany()
                .HasForeignKey(a => a.ServiceId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Doctor)
                .WithMany(d => d.Appointments)
                .HasForeignKey(a => a.DoctorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Business)
                .WithMany()
                .HasForeignKey(a => a.BusinessId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Service>()
                .HasOne(s => s.Business)
                .WithMany(b => b.Services)
                .HasForeignKey(s => s.BusinessId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
