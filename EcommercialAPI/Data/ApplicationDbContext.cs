using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Text.Json;
using EcommercialAPI.Data.Entities;
namespace EcommercialAPI.Data
{
    public partial class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext()
        {
        }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
        public virtual DbSet<Products> Products { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.ConfigureWarnings(warnings => warnings.Ignore(RelationalEventId.PendingModelChangesWarning));
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Products>(entity =>
            {
                modelBuilder.Entity<Products>()
                .HasKey(p => p.Id);
            });
















            modelBuilder.Entity<Products>().HasData(
                new Products
                {
                    Id = "1",
                    Name = "S24 Ultra",
                    Brand = "Samsung",
                    Description ="Nothing in my mind to descript this phone",
                    ReleaseDate =DateOnly.Parse("2026-01-01"),
                    Price = 20000000,
                    Quantity =10,
                    Status = 1
                }
            );
        }
    }
}
