using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using INVENTORY.EF.Entidades;

namespace INVENTORY.EF
{
    public partial class INVENTORYRContext : DbContext
    {
        public INVENTORYRContext()
        {
        }

        public INVENTORYRContext(DbContextOptions<INVENTORYRContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Department> Department { get; set; }
        public virtual DbSet<Inventory> Inventory { get; set; }
        public virtual DbSet<InventoryStatus> InventoryStatus { get; set; }
        public virtual DbSet<PhysicalInventory> PhysicalInventory { get; set; }
        public virtual DbSet<PhysicalInventoryHist> PhysicalInventoryHist { get; set; }
        public virtual DbSet<TmsPiece> TmsPiece { get; set; }
        public virtual DbSet<TmsPieceHist> TmsPieceHist { get; set; }
        public virtual DbSet<UserInventory> UserInventory { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                //optionsBuilder.UseSqlServer("Server=192.168.24.11;Database=INVENTORYR;User Id=sa; Password=Cr0ssv1ll3");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Department>(entity =>
            {
                entity.HasKey(e => e.DepartmentId)
                    .HasName("PK3")
                    .IsClustered(false);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.UserId)
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Department)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("RefUserInventory7");
            });

            modelBuilder.Entity<Inventory>(entity =>
            {
                entity.HasKey(e => e.InventoryId)
                    .HasName("PK1")
                    .IsClustered(false);

                entity.Property(e => e.CloseOwner)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.CreatorOwner)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.EndDate).HasColumnType("datetime");

                entity.Property(e => e.StartDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<InventoryStatus>(entity =>
            {
                entity.HasKey(e => e.StatusId)
                    .IsClustered(false);

                entity.Property(e => e.StatusName)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<PhysicalInventory>(entity =>
            {
                entity.HasKey(e => new { e.PieceNo, e.InventoryId, e.DepartmentId })
                    .HasName("PK4")
                    .IsClustered(false);

                entity.Property(e => e.PieceNo)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.Location)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.LoomNo)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.RackNo)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.ScanDate).HasColumnType("datetime");

                entity.HasOne(d => d.Department)
                    .WithMany(p => p.PhysicalInventory)
                    .HasForeignKey(d => d.DepartmentId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("RefDepartment8");

                entity.HasOne(d => d.Inventory)
                    .WithMany(p => p.PhysicalInventory)
                    .HasForeignKey(d => d.InventoryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("RefInventory7");
            });

            modelBuilder.Entity<PhysicalInventoryHist>(entity =>
            {
                entity.HasKey(e => new { e.PieceNo, e.InventoryId, e.DepartmentId })
                    .HasName("PK6")
                    .IsClustered(false);

                entity.Property(e => e.PieceNo)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.Location)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.LoomNo)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.RackNo)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.ScanDate).HasColumnType("datetime");

                entity.HasOne(d => d.Department)
                    .WithMany(p => p.PhysicalInventoryHist)
                    .HasForeignKey(d => d.DepartmentId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("RefDepartment11");

                entity.HasOne(d => d.Inventory)
                    .WithMany(p => p.PhysicalInventoryHist)
                    .HasForeignKey(d => d.InventoryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("RefInventory10");
            });

            modelBuilder.Entity<TmsPiece>(entity =>
            {
                entity.HasKey(e => e.PieceNo)
                    .HasName("PK7")
                    .IsClustered(false);

                entity.Property(e => e.PieceNo)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Design)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Location)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.LoomNo)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Notes)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Quality)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.RackNo)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ScanDate).HasColumnType("datetime");

                entity.Property(e => e.WarpShade)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.WeftShade)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.Department)
                    .WithMany(p => p.TmsPiece)
                    .HasForeignKey(d => d.DepartmentId)
                    .HasConstraintName("RefDepartment6");

                entity.HasOne(d => d.Inventory)
                    .WithMany(p => p.TmsPiece)
                    .HasForeignKey(d => d.InventoryId)
                    .HasConstraintName("RefInventory17");

                entity.HasOne(d => d.Status)
                    .WithMany(p => p.TmsPiece)
                    .HasForeignKey(d => d.StatusId)
                    .HasConstraintName("RefInventoryStatus");
            });

            modelBuilder.Entity<TmsPieceHist>(entity =>
            {
                entity.HasKey(e => new { e.PieceNo, e.InventoryId, e.DepartmentId })
                    .HasName("PK8")
                    .IsClustered(false);

                entity.Property(e => e.PieceNo)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Design)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Location)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.LoomNo)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Notes)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Quality)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.RackNo)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ScanDate).HasColumnType("datetime");

                entity.Property(e => e.WarpShade)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.WeftShade)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.Department)
                    .WithMany(p => p.TmsPieceHist)
                    .HasForeignKey(d => d.DepartmentId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("RefDepartment9");

                entity.HasOne(d => d.Inventory)
                    .WithMany(p => p.TmsPieceHist)
                    .HasForeignKey(d => d.InventoryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("RefInventory8");
            });

            modelBuilder.Entity<UserInventory>(entity =>
            {
                entity.HasKey(e => e.UserId)
                    .HasName("PK_User")
                    .IsClustered(false);

                entity.Property(e => e.UserId)
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.Name)
                    .HasMaxLength(80)
                    .IsUnicode(false);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
