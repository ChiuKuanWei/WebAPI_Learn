using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace WebAPI_Test.Models;

public partial class WebApiTestDbContext : DbContext
{
    public WebApiTestDbContext()
    {
    }

    public WebApiTestDbContext(DbContextOptions<WebApiTestDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Information> Information { get; set; }

    public virtual DbSet<Status> Statuses { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Information>(entity =>
        {
            entity.ToTable("information");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.ColAddress)
                .HasMaxLength(50)
                .HasColumnName("Col_Address");
            entity.Property(e => e.ColAge).HasColumnName("Col_Age");
            entity.Property(e => e.ColName)
                .HasMaxLength(50)
                .HasColumnName("Col_Name");
            entity.Property(e => e.ColProfession)
                .HasMaxLength(50)
                .HasColumnName("Col_Profession");
            entity.Property(e => e.DateTime).HasColumnType("datetime");
        });

        modelBuilder.Entity<Status>(entity =>
        {
            entity
                //.HasNoKey()
                .ToTable("Status");

            entity.Property(e => e.ColAge).HasColumnName("Col_Age");
            entity.Property(e => e.ColNote)
                .HasMaxLength(50)
                .HasColumnName("Col_Note");
            entity.Property(e => e.ColPosition)
                .HasMaxLength(50)
                .HasColumnName("Col_Position");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
