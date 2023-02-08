using DEVTOOLS.EF.Entidades;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DEVTOOLS.EF
{
    public partial class DEVTOOLSContext : DbContext
    {
        public DEVTOOLSContext()
        {
        }
        public DEVTOOLSContext(DbContextOptions<DEVTOOLSContext> options): base(options)
        {
        }

        public virtual DbSet<Project> Project { get; set; }
        public virtual DbSet<Entity> Entity { get; set; }
        public virtual DbSet<Column> Column { get; set; }
        public virtual DbSet<EntityTemplate> EntityTemplate { get; set; }
        public virtual DbSet<ColumnTemplate> ColumnTemplate { get; set; }
        public virtual DbSet<ProjectTemplate> ProjectTemplate { get; set; }
        public virtual DbSet<DatabaseSource> DatabaseSource { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server=192.168.24.11;Database=DEVTOOLS;User Id=sa; Password=Cr0ssv1ll3");
            }
        }
    }
}
