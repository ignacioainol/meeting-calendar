using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using REPOSITORY.EF.Entidades;

namespace REPOSITORY.EF
{
    public partial class REPOSITORYContext : DbContext
    {
        public REPOSITORYContext()
        {
        }

        public REPOSITORYContext(DbContextOptions<REPOSITORYContext> options)
            : base(options)
        {
        }   

        public virtual DbSet<Query> Query { get; set; }
        public virtual DbSet<Share> Share { get; set; }
        public virtual DbSet<Parameters> Parameters { get; set; }
  

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                //optionsBuilder.UseSqlServer("Server=192.168.24.11;Database=INVENTORYR;User Id=sa; Password=Cr0ssv1ll3");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Query>(entity =>
            {
                entity.HasKey(e => e.ID_Query)
                    .IsClustered(false);
                entity.ToTable("QUERY");
            });

            modelBuilder.Entity<Share>(entity =>
            {
                entity.HasKey(e => e.ID_Share)
                    .IsClustered(false);
                entity.ToTable("SHARE");

                entity.HasOne(d => d.Query)
                     .WithMany(p => p.Shares)
                     .HasForeignKey(d => d.ID_Query)
                     .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<Parameters>(entity =>
            {
                entity.HasKey(e => e.ID_Paramater)
                    .IsClustered(false);
                entity.ToTable("PARAMETERS");

                entity.HasOne(d => d.Query)
                     .WithMany(p => p.Parameters)
                     .HasForeignKey(d => d.ID_Query)
                     .OnDelete(DeleteBehavior.ClientSetNull);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
