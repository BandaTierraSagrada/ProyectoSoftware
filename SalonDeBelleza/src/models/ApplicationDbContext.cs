using Microsoft.EntityFrameworkCore;

namespace SalonDeBelleza.src.models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Usuario> Usuarios { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configurar el enum para Loginstatus y Rol
            modelBuilder.Entity<Usuario>()
                .Property(u => u.Loginstatus)
                .HasConversion<string>();

            modelBuilder.Entity<Usuario>()
                .Property(u => u.Rol)
                .HasConversion<string>();
        }
    }
}