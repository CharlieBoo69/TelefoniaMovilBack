using Microsoft.EntityFrameworkCore;
using TelefoniaMovilBackend.Models; // Asegúrate de importar el espacio de nombres donde está tu clase Plan

namespace TelefoniaMovilBackend.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Define un DbSet para cada modelo que quieres mapear en la base de datos
        public DbSet<Plan> Planes { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Suscripcion> Suscripciones { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configura las relaciones
            modelBuilder.Entity<Usuario>()
                .HasMany(u => u.Suscripciones)
                .WithOne(s => s.Usuario)
                .HasForeignKey(s => s.UsuarioId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Suscripcion>()
                .HasOne(s => s.Plan)
                .WithMany()
                .HasForeignKey(s => s.PlanId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configuración de las tablas
            modelBuilder.Entity<Plan>().ToTable("planes");
            modelBuilder.Entity<Usuario>().ToTable("usuarios");
            modelBuilder.Entity<Suscripcion>().ToTable("suscripciones");

            // Configuración de tipos de datos
            modelBuilder.Entity<Plan>().Property(p => p.Nombre).HasColumnType("nvarchar(max)");
            modelBuilder.Entity<Plan>().Property(p => p.Operadora).HasColumnType("nvarchar(max)");
            modelBuilder.Entity<Plan>().Property(p => p.BeneficiosAdicionales).HasColumnType("nvarchar(max)");

            modelBuilder.Entity<Usuario>().Property(u => u.Nombre).HasColumnType("nvarchar(max)");
            modelBuilder.Entity<Usuario>().Property(u => u.Email).HasColumnType("nvarchar(max)");
            modelBuilder.Entity<Usuario>().Property(u => u.Telefono).HasColumnType("nvarchar(max)");
            modelBuilder.Entity<Usuario>().Property(u => u.Password).HasColumnType("nvarchar(max)");

            modelBuilder.Entity<Suscripcion>().Property(s => s.NumeroTelefono).HasColumnType("nvarchar(max)");

            // Especifica la precisión y escala de la propiedad 'Costo' en la entidad 'Plan'
            modelBuilder.Entity<Plan>().Property(p => p.Costo).HasPrecision(18, 2);
        }
    }
}
