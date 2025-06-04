using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace LicenseManager.Models;

public partial class LicenseManagerContext : DbContext
{
    public LicenseManagerContext()
    {
    }

    public LicenseManagerContext(DbContextOptions<LicenseManagerContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Application> Applications { get; set; }

    public virtual DbSet<Client> Clients { get; set; }

    public virtual DbSet<ClientApplicationLicensedFeature> ClientApplicationLicensedFeatures { get; set; }

    public virtual DbSet<ClientApplicationMapping> ClientApplicationMappings { get; set; }

    public virtual DbSet<Feature> Features { get; set; }

    public virtual DbSet<FeatureGroup> FeatureGroups { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        //empty
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Application>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Applicat__3214EC07BAD0BE80");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<Client>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Clients__3214EC0776FDC569");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<ClientApplicationLicensedFeature>(entity =>
        {
            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Features).IsUnicode(false);
            entity.Property(e => e.FkClientApplicationMappingId).HasColumnName("FK_ClientApplicationMappingId");

            entity.HasOne(d => d.FkClientApplicationMapping).WithMany(p => p.ClientApplicationLicensedFeatures)
                .HasForeignKey(d => d.FkClientApplicationMappingId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ClientApplicationLicensedFeatures_ClientApplicationMapping");
        });

        modelBuilder.Entity<ClientApplicationMapping>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ClientAp__3214EC074B5C0135");

            entity.ToTable("ClientApplicationMapping");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.EndDate).HasColumnType("datetime");
            entity.Property(e => e.FkApplicationId).HasColumnName("FK_ApplicationId");
            entity.Property(e => e.FkClientId).HasColumnName("FK_ClientId");
            entity.Property(e => e.LicenseId).HasMaxLength(100);
            entity.Property(e => e.StartDate).HasColumnType("datetime");

            entity.HasOne(d => d.FkApplication).WithMany(p => p.ClientApplicationMappings)
                .HasForeignKey(d => d.FkApplicationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ClientApplicationMapping_Applications");

            entity.HasOne(d => d.FkClient).WithMany(p => p.ClientApplicationMappings)
                .HasForeignKey(d => d.FkClientId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ClientApplicationMapping_Clients");
        });

        modelBuilder.Entity<Feature>(entity =>
        {
            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.FkFeatureGroupId).HasColumnName("FK_FeatureGroupId");
            entity.Property(e => e.Name).HasMaxLength(100);

            entity.HasOne(d => d.FkFeatureGroup).WithMany(p => p.Features)
                .HasForeignKey(d => d.FkFeatureGroupId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Features_FeatureGroups");
        });

        modelBuilder.Entity<FeatureGroup>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__FeatureG__3214EC07AF331C17");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.Name).HasMaxLength(100);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
