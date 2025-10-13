using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Scaffolding.Internal;

namespace Kurs2;

public partial class Formula12025Context : DbContext
{
    public Formula12025Context()
    {
    }

    public Formula12025Context(DbContextOptions<Formula12025Context> options)
        : base(options)
    {
    }

    public virtual DbSet<Circuit> Circuits { get; set; }

    public virtual DbSet<Constructor> Constructors { get; set; }

    public virtual DbSet<Driver> Drivers { get; set; }

    public virtual DbSet<DriverConstructorAffiliation> DriverConstructorAffiliations { get; set; }

    public virtual DbSet<Race> Races { get; set; }

    public virtual DbSet<RaceResult> RaceResults { get; set; }

    public virtual DbSet<Season> Seasons { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseMySql("server=localhost;user=root;password=root;database=formula1_2025", Microsoft.EntityFrameworkCore.ServerVersion.Parse("8.0.40-mysql"));

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_0900_ai_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<Circuit>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("circuits");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CircuitName)
                .HasMaxLength(255)
                .HasColumnName("circuit_name");
            entity.Property(e => e.Country)
                .HasMaxLength(100)
                .HasColumnName("country");
            entity.Property(e => e.Location)
                .HasMaxLength(255)
                .HasColumnName("location");
        });

        modelBuilder.Entity<Constructor>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("constructors");

            entity.HasIndex(e => e.ConstructorName, "idx_constructor_name");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ConstructorName).HasColumnName("constructor_name");
            entity.Property(e => e.DirectorLastName)
                .HasMaxLength(65)
                .HasColumnName("director_last_name");
            entity.Property(e => e.DirectorName)
                .HasMaxLength(65)
                .HasColumnName("director_name");
            entity.Property(e => e.Nationality)
                .HasMaxLength(100)
                .HasColumnName("nationality");
        });

        modelBuilder.Entity<Driver>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("drivers");

            entity.HasIndex(e => e.LastName, "idx_driver_lastname");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.DateOfBirth).HasColumnName("date_of_birth");
            entity.Property(e => e.FirstName)
                .HasMaxLength(100)
                .HasColumnName("first_name");
            entity.Property(e => e.LastName)
                .HasMaxLength(100)
                .HasColumnName("last_name");
            entity.Property(e => e.Nationality)
                .HasMaxLength(100)
                .HasColumnName("nationality");
        });

        modelBuilder.Entity<DriverConstructorAffiliation>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("driver_constructor_affiliations");

            entity.HasIndex(e => e.ConstructorId, "fk_aff_constructor");

            entity.HasIndex(e => new { e.DriverId, e.ConstructorId, e.StartDate }, "ux_affiliation").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ConstructorId).HasColumnName("constructor_id");
            entity.Property(e => e.DriverId).HasColumnName("driver_id");
            entity.Property(e => e.EndDate).HasColumnName("end_date");
            entity.Property(e => e.StartDate).HasColumnName("start_date");

            entity.HasOne(d => d.Constructor).WithMany(p => p.DriverConstructorAffiliations)
                .HasForeignKey(d => d.ConstructorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_aff_constructor");

            entity.HasOne(d => d.Driver).WithMany(p => p.DriverConstructorAffiliations)
                .HasForeignKey(d => d.DriverId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_aff_driver");
        });

        modelBuilder.Entity<Race>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("races");

            entity.HasIndex(e => e.CircuitId, "fk_races_circuits");

            entity.HasIndex(e => e.SeasonId, "fk_races_seasons");

            entity.HasIndex(e => e.RaceDate, "idx_race_date");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CircuitId).HasColumnName("circuit_id");
            entity.Property(e => e.RaceDate).HasColumnName("race_date");
            entity.Property(e => e.RaceName)
                .HasMaxLength(255)
                .HasColumnName("race_name");
            entity.Property(e => e.RaceStatus)
                .HasColumnType("enum('scheduled','completed','cancelled','postponed')")
                .HasColumnName("race_status");
            entity.Property(e => e.Round).HasColumnName("round");
            entity.Property(e => e.SeasonId).HasColumnName("season_id");

            entity.HasOne(d => d.Circuit).WithMany(p => p.Races)
                .HasForeignKey(d => d.CircuitId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_races_circuits");

            entity.HasOne(d => d.Season).WithMany(p => p.Races)
                .HasForeignKey(d => d.SeasonId)
                .HasConstraintName("fk_races_seasons");
        });

        modelBuilder.Entity<RaceResult>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("race_results");

            entity.HasIndex(e => e.ConstructorId, "fk_rr_constructor");

            entity.HasIndex(e => e.DriverId, "fk_rr_driver");

            entity.HasIndex(e => e.RaceId, "idx_results_race");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ConstructorId).HasColumnName("constructor_id");
            entity.Property(e => e.DriverId).HasColumnName("driver_id");
            entity.Property(e => e.FinishPosition)
                .HasMaxLength(3)
                .HasColumnName("finish_position");
            entity.Property(e => e.PointsAwarded)
                .HasPrecision(5, 2)
                .HasColumnName("points_awarded");
            entity.Property(e => e.RaceId).HasColumnName("race_id");
            entity.Property(e => e.StartPosition)
                .HasMaxLength(3)
                .HasColumnName("start_position");

            entity.HasOne(d => d.Constructor).WithMany(p => p.RaceResults)
                .HasForeignKey(d => d.ConstructorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_rr_constructor");

            entity.HasOne(d => d.Driver).WithMany(p => p.RaceResults)
                .HasForeignKey(d => d.DriverId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_rr_driver");

            entity.HasOne(d => d.Race).WithMany(p => p.RaceResults)
                .HasForeignKey(d => d.RaceId)
                .HasConstraintName("fk_rr_race");
        });

        modelBuilder.Entity<Season>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("seasons");

            entity.HasIndex(e => e.Year, "year").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Year).HasColumnName("year");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
