using Microsoft.EntityFrameworkCore;
using SOFTLAND.EF.Entidades;

namespace SOFTLAND.EF
{
    public class SOFTLANDContext : DbContext
    {
        public SOFTLANDContext()
        {
        }

        public SOFTLANDContext(DbContextOptions<SOFTLANDContext> options)
            : base(options)
        {
        }

        public DbSet<Personal> sw_personal { get; set; }
        public DbSet<Ccostoper> sw_ccostoper { get; set; }
        public DbSet<Cwtccos> cwtccos { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Personal>(entity =>
            {
                entity.HasKey(e => e.ficha);
            });

            modelBuilder.Entity<Ccostoper>(entity =>
            {
                entity.HasKey(e => e.ficha);

                entity.HasOne(d => d.personal)
                    .WithMany(p => p.ccostopers)
                    .HasForeignKey(d => d.ficha);

                entity.HasOne(d => d.cwtcco)
                    .WithMany(p => p.ccostopers)
                    .HasForeignKey(d => d.codiCC);
            });

            modelBuilder.Entity<Cwtccos>(entity =>
            {
                entity.HasKey(e => e.CodiCC);
            });
        }
    }
}
