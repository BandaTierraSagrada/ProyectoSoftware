﻿using Microsoft.EntityFrameworkCore;
using SalonDeBelleza.src.models;

namespace SalonDeBelleza.src.models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

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

            // Configurar valores por defecto para created_at y updated_at
            modelBuilder.Entity<Usuario>()
                .Property(u => u.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            modelBuilder.Entity<Usuario>()
                .Property(u => u.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP");
        }
    }
}