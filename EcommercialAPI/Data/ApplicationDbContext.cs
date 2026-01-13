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
        public virtual DbSet<Carts> Carts { get; set; } = null!;
        public virtual DbSet<CartDetails> CartDetails { get; set; } = null!;
        public virtual DbSet<Orders> Orders { get; set; } = null!;
        public virtual DbSet<OrderDetails> OrderDetails { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.ConfigureWarnings(warnings => warnings.Ignore(RelationalEventId.PendingModelChangesWarning));
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Products>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.Property(p => p.Id).ValueGeneratedOnAdd();
            });
            modelBuilder.Entity<Users>(entity =>
            {
                entity.HasKey(p => p.Id);
            });
            modelBuilder.Entity<RefreshToken>(entity =>
            {
                entity.HasKey(p => p.Id);
            });
            modelBuilder.Entity<Carts>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.Property(p=>p.Id).ValueGeneratedOnAdd();
                entity.HasOne(p => p.User)
                      .WithOne(c => c.Cart)
                      .HasForeignKey<Carts>(c => c.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
            modelBuilder.Entity<CartDetails>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.Property(p=> p.Id).ValueGeneratedOnAdd();
                entity.HasOne(cd => cd.Cart)
                      .WithMany(c => c.CartDetails)
                      .HasForeignKey(cd => cd.CartId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
            modelBuilder.Entity<Orders>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.Property(p => p.Id).ValueGeneratedOnAdd();
                entity.HasOne(p => p.User)
                      .WithMany(c => c.Orders)
                      .HasForeignKey(c => c.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
            modelBuilder.Entity<OrderDetails>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.Property(p => p.Id).ValueGeneratedOnAdd();
                entity.HasOne(cd => cd.Order)
                      .WithMany(c => c.OrderDetails)
                      .HasForeignKey(cd => cd.OrderId)
                      .OnDelete(DeleteBehavior.Cascade);
            });













            modelBuilder.Entity<Products>().HasData(
                new Products
                {
                    Id = 1,
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
