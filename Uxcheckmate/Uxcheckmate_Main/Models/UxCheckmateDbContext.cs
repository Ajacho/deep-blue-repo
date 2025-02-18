using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Uxcheckmate_Main.Models;

public partial class UxCheckmateDbContext : DbContext
{
    public UxCheckmateDbContext()
    {
    }

    public UxCheckmateDbContext(DbContextOptions<UxCheckmateDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Report> Reports { get; set; }

    public virtual DbSet<ReportCategory> ReportCategories { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<UserAccount> UserAccounts { get; set; }

    /**
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Name=DBConnection");
    */
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder){
        if (!optionsBuilder.IsConfigured){
            optionsBuilder.UseSqlServer("Name=DBConnection");
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Report>(entity =>
        {
            entity.HasKey(e => e.ReportId).HasName("PK__Reports__D5BD48E55537F96D");

            entity.Property(e => e.ReportId).HasColumnName("ReportID");

            entity.Property(e => e.CategoryId).HasColumnName("CategoryID");
            


            entity.Property(e => e.Date).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Category).WithMany(p => p.Reports)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Reports_CategoryID");

            entity.HasOne(d => d.User).WithMany(p => p.Reports)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_Reports_UserID");
        });

        modelBuilder.Entity<ReportCategory>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("PK__ReportCa__19093A2BAE98F34D");

            entity.HasIndex(e => e.Name, "UQ__ReportCa__737584F6260F4374").IsUnique();

            entity.Property(e => e.CategoryId).HasColumnName("CategoryID");
            entity.Property(e => e.Name).HasMaxLength(255);
            entity.Property(e => e.OpenAiprompt).HasColumnName("OpenAIPrompt");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__Roles__8AFACE3A9F76FFA0");

            entity.HasIndex(e => e.Name, "UQ__Roles__737584F6A7C2AD59").IsUnique();

            entity.Property(e => e.RoleId).HasColumnName("RoleID");
            entity.Property(e => e.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<UserAccount>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__UserAcco__1788CCACA92BB876");

            entity.HasIndex(e => e.Email, "UQ__UserAcco__A9D10534985556DE").IsUnique();

            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.RoleId).HasColumnName("RoleID");

            entity.HasOne(d => d.Role).WithMany(p => p.UserAccounts)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserAccounts_RoleID");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
