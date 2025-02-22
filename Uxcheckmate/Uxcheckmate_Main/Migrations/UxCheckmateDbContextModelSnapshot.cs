﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Uxcheckmate_Main.Models;

#nullable disable

namespace Uxcheckmate_Main.Migrations
{
    [DbContext(typeof(UxCheckmateDbContext))]
    partial class UxCheckmateDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.2")
                .HasAnnotation("Proxies:ChangeTracking", false)
                .HasAnnotation("Proxies:CheckEquality", false)
                .HasAnnotation("Proxies:LazyLoading", true)
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Uxcheckmate_Main.Models.AccessibilityCategory", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("ID");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(128)
                        .IsUnicode(false)
                        .HasColumnType("varchar(128)");

                    b.HasKey("Id")
                        .HasName("PK__Accessib__3214EC274D88B479");

                    b.ToTable("AccessibilityCategory");
                });

            modelBuilder.Entity("Uxcheckmate_Main.Models.AccessibilityIssue", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("ID");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("CategoryId")
                        .HasColumnType("int")
                        .HasColumnName("CategoryID");

                    b.Property<string>("Message")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("ReportId")
                        .HasColumnType("int")
                        .HasColumnName("ReportID");

                    b.Property<string>("Selector")
                        .IsRequired()
                        .HasMaxLength(128)
                        .IsUnicode(false)
                        .HasColumnType("varchar(128)");

                    b.Property<int>("Severity")
                        .HasColumnType("int");

                    b.HasKey("Id")
                        .HasName("PK__Accessib__3214EC2759D39646");

                    b.HasIndex("CategoryId");

                    b.HasIndex("ReportId");

                    b.ToTable("AccessibilityIssue");
                });

            modelBuilder.Entity("Uxcheckmate_Main.Models.DesignCategory", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("ID");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(128)
                        .IsUnicode(false)
                        .HasColumnType("varchar(128)");

                    b.Property<string>("ScanMethod")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id")
                        .HasName("PK__DesignCa__3214EC27E2402227");

                    b.ToTable("DesignCategory");
                });

            modelBuilder.Entity("Uxcheckmate_Main.Models.DesignIssue", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("ID");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("CategoryId")
                        .HasColumnType("int")
                        .HasColumnName("CategoryID");

                    b.Property<string>("Message")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("ReportId")
                        .HasColumnType("int")
                        .HasColumnName("ReportID");

                    b.Property<int>("Severity")
                        .HasColumnType("int");

                    b.HasKey("Id")
                        .HasName("PK__DesignIs__3214EC277C5F35C0");

                    b.HasIndex("CategoryId");

                    b.HasIndex("ReportId");

                    b.ToTable("DesignIssue");
                });

            modelBuilder.Entity("Uxcheckmate_Main.Models.Report", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("ID");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateOnly>("Date")
                        .HasColumnType("date");

                    b.Property<string>("Url")
                        .IsRequired()
                        .HasMaxLength(128)
                        .IsUnicode(false)
                        .HasColumnType("varchar(128)")
                        .HasColumnName("URL");

                    b.HasKey("Id")
                        .HasName("PK__Report__3214EC27DC95E762");

                    b.ToTable("Report");
                });

            modelBuilder.Entity("Uxcheckmate_Main.Models.AccessibilityIssue", b =>
                {
                    b.HasOne("Uxcheckmate_Main.Models.AccessibilityCategory", "Category")
                        .WithMany("AccessibilityIssues")
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("FK_AccessibilityIssue_CategoryID");

                    b.HasOne("Uxcheckmate_Main.Models.Report", "Report")
                        .WithMany("AccessibilityIssues")
                        .HasForeignKey("ReportId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("FK_AccessibilityIssue_ReportID");

                    b.Navigation("Category");

                    b.Navigation("Report");
                });

            modelBuilder.Entity("Uxcheckmate_Main.Models.DesignIssue", b =>
                {
                    b.HasOne("Uxcheckmate_Main.Models.DesignCategory", "Category")
                        .WithMany("DesignIssues")
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("FK_DesignIssue_CategoryID");

                    b.HasOne("Uxcheckmate_Main.Models.Report", "Report")
                        .WithMany("DesignIssues")
                        .HasForeignKey("ReportId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("FK_DesignIssue_ReportID");

                    b.Navigation("Category");

                    b.Navigation("Report");
                });

            modelBuilder.Entity("Uxcheckmate_Main.Models.AccessibilityCategory", b =>
                {
                    b.Navigation("AccessibilityIssues");
                });

            modelBuilder.Entity("Uxcheckmate_Main.Models.DesignCategory", b =>
                {
                    b.Navigation("DesignIssues");
                });

            modelBuilder.Entity("Uxcheckmate_Main.Models.Report", b =>
                {
                    b.Navigation("AccessibilityIssues");

                    b.Navigation("DesignIssues");
                });
#pragma warning restore 612, 618
        }
    }
}
