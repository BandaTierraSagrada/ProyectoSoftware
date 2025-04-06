using Microsoft.EntityFrameworkCore;
using SalonDeBelleza.src.models;

namespace SalonDeBelleza.src.config
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Cita> Citas { get; set; }
        public DbSet<ColaboradorInfo> Colaboradores { get; set; }
        public DbSet<Notificacion> Notificaciones { get; set; }
        public DbSet<Producto> Productos { get; set; }
        public DbSet<PreferenciaNotificacion> PreferenciasNotificaciones { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Usuario>().ToTable("Usuarios");

            modelBuilder.Entity<ColaboradorInfo>()
                .HasOne(c => c.Usuario)
                .WithMany()
                .HasForeignKey(c => c.UserID);

            modelBuilder.Entity<Notificacion>()
            .HasOne(n => n.Usuario)
            .WithMany(u => u.Notificaciones)
            .HasForeignKey(n => n.UserID)
            .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PreferenciaNotificacion>()
                .HasOne(p => p.Usuario)
                .WithMany(u => u.PreferenciasNotificaciones)
                .HasForeignKey(p => p.UserID)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
