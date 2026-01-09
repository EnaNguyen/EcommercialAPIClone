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
        public virtual DbSet<Users> Users { get; set; } = null!;
        public virtual DbSet<RefreshToken> RefreshTokens { get; set; } = null!;
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
            modelBuilder.Entity<Users>(entity =>
            {
                modelBuilder.Entity<Users>().HasKey(p => p.Id);
            });
            modelBuilder.Entity<RefreshToken>(entity =>
            {
                modelBuilder.Entity<RefreshToken>().HasKey(p => p.Id);
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
            modelBuilder.Entity<Users>().HasData(
                new Users
                {
                    Id = "AD000001",
                    FullName = "Nguyễn Quang Quý",
                    Username = "Admin",
                    Password = "8d969eef6ecad3c29a3a629280e686cf0c3f5d5a86aff3ca12020c923adc6c92",
                    Role = "Admin",
                    Email = "nguyenquangquyX@gmail.com",
                    Phone = "0973713274",
                    DayOfBirth = DateOnly.Parse("1999-05-19"),
                    Gender = 1,
                    Status = 2,
                    Img = "",
                    TwoFA = true,
                    CurrentOtpCode=null,
                    OtpExpiryTime = null,
                }
            );
        }
    }
}
