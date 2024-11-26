using Microsoft.EntityFrameworkCore;
using TelefoniaMovilBackend.Models;

namespace TelefoniaMovilBackend.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Define los DbSet para mapear las tablas en la base de datos
        public DbSet<Plan> Planes { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Suscripcion> Suscripciones { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configura las relaciones entre las entidades
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

            // Configura la precisión de 'Costo' en la entidad 'Plan'
            modelBuilder.Entity<Plan>().Property(p => p.Costo).HasPrecision(18, 2);

            // Configura los nombres de las tablas
            modelBuilder.Entity<Plan>().ToTable("planes");
            modelBuilder.Entity<Usuario>().ToTable("usuarios");
            modelBuilder.Entity<Suscripcion>().ToTable("suscripciones");
        }
    }
}
