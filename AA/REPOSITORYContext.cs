using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using AA.EF.Entidades;

namespace AA.EF
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

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
