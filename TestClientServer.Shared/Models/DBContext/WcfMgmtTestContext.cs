using Microsoft.EntityFrameworkCore;
using TestClientServer.Shared.Models.Server;

namespace TestClientServer.Shared.Models.DBContext;
public partial class WcfMgmtTestContext : DbContext
{
    public WcfMgmtTestContext()
    {
    }

    public WcfMgmtTestContext(DbContextOptions<WcfMgmtTestContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AvailableSignalPorts2> AvailableSignalPorts2s { get; set; }

    public virtual DbSet<WcfMgmtEquipment> WcfMgmtEquipments { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Data Source=SQL_Dev1901;Initial Catalog=wcfMgmt_test;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True;Integrated security=false;User ID=wcf_app;Password=wcfapp;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AvailableSignalPorts2>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.ToTable("availableSignalPorts2");
            entity.Property(e => e.AvailableOnts).HasColumnName("available_onts");
            entity.Property(e => e.AvailableTails).HasColumnName("available_tails");
            entity.Property(e => e.Fdh)
                .HasMaxLength(50)
                .HasColumnName("fdh");
            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnName("id");
            entity.Property(e => e.Lt).HasColumnName("lt");
            entity.Property(e => e.NextAvailableOnt).HasColumnName("next_available_ont");
            entity.Property(e => e.NextAvailableTail).HasColumnName("next_available_tail");
            entity.Property(e => e.NextOntEquid)
                .HasMaxLength(50)
                .HasColumnName("next_ont_equid");
            entity.Property(e => e.NextTailEquid)
                .HasMaxLength(50)
                .HasColumnName("next_tail_equid");
            entity.Property(e => e.Olt).HasColumnName("olt");
            entity.Property(e => e.OntCount).HasColumnName("ont_count");
            entity.Property(e => e.Pon).HasColumnName("pon");
            entity.Property(e => e.PonPath)
                .HasMaxLength(255)
                .HasColumnName("pon_path");
            entity.Property(e => e.Splitter)
                .HasMaxLength(50)
                .HasColumnName("splitter");
            entity.Property(e => e.SplitterCard)
                .HasMaxLength(255)
                .HasColumnName("splitter_card");
            entity.Property(e => e.SplitterInstalled).HasColumnName("splitter_installed");
            entity.Property(e => e.SplitterTailCount).HasColumnName("splitter_tail_count");
            entity.Property(e => e.Town)
                .HasMaxLength(255)
                .HasColumnName("town");
        });

        modelBuilder.Entity<WcfMgmtEquipment>(entity =>
        {
            entity.ToTable("wcfMgmtEquipments", tb => tb.HasTrigger("calculateAvailablePorts"));
            entity.HasKey(x => x.EquId);
            entity.ToTable("wcfMgmtEquipments");
            entity.Property(e => e.AssetTag)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("assetTag");
            entity.Property(e => e.ContainerId)
                .HasMaxLength(255)
                .HasColumnName("containerID");
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("createdBy");
            entity.Property(e => e.CreatedDate)
                .HasColumnType("datetime")
                .HasColumnName("createdDate");
            entity.Property(e => e.EquClass)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("equClass");
            entity.Property(e => e.EquId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("equID");
            entity.Property(e => e.Fdh)
                .HasMaxLength(50)
                .HasColumnName("fdh");
            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnName("id");
            entity.Property(e => e.InstallDate)
                .HasColumnType("datetime")
                .HasColumnName("installDate");
            entity.Property(e => e.LocationId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("locationID");
            entity.Property(e => e.Lt).HasColumnName("lt");
            entity.Property(e => e.Mac)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("mac");
            entity.Property(e => e.Manufacturer)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("manufacturer");
            entity.Property(e => e.Model)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("model");
            entity.Property(e => e.ModifiedBy)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("modifiedBy");
            entity.Property(e => e.ModifiedDate)
                .HasColumnType("datetime")
                .HasColumnName("modifiedDate");
            entity.Property(e => e.Olt).HasColumnName("olt");
            entity.Property(e => e.Ont).HasColumnName("ont");
            entity.Property(e => e.Pon).HasColumnName("pon");
            entity.Property(e => e.RemovalDate)
                .HasColumnType("datetime")
                .HasColumnName("removalDate");
            entity.Property(e => e.RemovedBy)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("removedBy");
            entity.Property(e => e.Serial)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("serial");
            entity.Property(e => e.SplitterCard)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("splitterCard");
            entity.Property(e => e.SplitterTail).HasColumnName("splitterTail");
            entity.Property(e => e.Ssid24)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("ssid24");
            entity.Property(e => e.Ssid5g)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("ssid5g");
            entity.Property(e => e.SsidPassword)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("ssidPassword");
            entity.Property(e => e.Town)
                .HasMaxLength(50)
                .HasColumnName("town");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
