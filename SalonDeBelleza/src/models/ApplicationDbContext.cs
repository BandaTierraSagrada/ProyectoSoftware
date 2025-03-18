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
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Usuario>().ToTable("Usuarios");

            modelBuilder.Entity<ColaboradorInfo>()
                .HasOne(c => c.Usuario)
                .WithMany()
                .HasForeignKey(c => c.UserID);
        }
    }
}
