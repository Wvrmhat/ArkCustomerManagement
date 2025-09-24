using System;
using System.Collections.Generic;
using ArkCustomerManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace ArkCustomerManagement.Data;

public partial class ArkCmsDbContext : DbContext
{
    public ArkCmsDbContext()
    {
    }

    public ArkCmsDbContext(DbContextOptions<ArkCmsDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Customer> Customers { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=arkCMS;Trusted_Connection=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Customer>(entity =>
        {
            entity.ToTable("Customer");

            entity.HasKey(e => e.CustomerId);
            entity.Property(e => e.CustomerId)
                .HasColumnName("CustomerID");
            entity.Property(e => e.Address)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.ContactPersonEmail)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.ContactPersonName)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Vat).HasColumnName("VAT");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
