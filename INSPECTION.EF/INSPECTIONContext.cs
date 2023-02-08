using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using INSPECTION.EF.Entidades;

namespace INSPECTION.EF
{
    public partial class INSPECTIONContext : DbContext
    {
        public INSPECTIONContext()
        {
        }

        public INSPECTIONContext(DbContextOptions<INSPECTIONContext> options)
            : base(options)
        {
        }

        public virtual DbSet<ColourBonu> ColourBonus { get; set; }
        public virtual DbSet<Customer> Customers { get; set; }
        public virtual DbSet<Cut> Cuts { get; set; }
        public virtual DbSet<Department> Departments { get; set; }
        public virtual DbSet<Failure> Failures { get; set; }
        public virtual DbSet<InspectedPiece> InspectedPieces { get; set; }
        public virtual DbSet<Log> Logs { get; set; }
        public virtual DbSet<PieceAnalysis> PieceAnalyses { get; set; }
        public virtual DbSet<PieceFailsSummary> PieceFailsSummaries { get; set; }
        public virtual DbSet<PieceFailure> PieceFailures { get; set; }
        public virtual DbSet<PieceSummary> PieceSummaries { get; set; }
        public virtual DbSet<Reporte> Reportes { get; set; }
        public virtual DbSet<ReporteFalla> ReporteFallas { get; set; }
        public virtual DbSet<Scann> Scanns { get; set; }
        public virtual DbSet<TmsPiece> TmsPieces { get; set; }
        public virtual DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {

            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "Modern_Spanish_CI_AS");

            modelBuilder.Entity<ColourBonu>(entity =>
            {
                entity.HasKey(e => e.CodeColourBonus)
                    .HasName("PK11")
                    .IsClustered(false);

                entity.ToTable("COLOUR_BONUS");

                entity.Property(e => e.CodeColourBonus)
                    .HasMaxLength(30)
                    .HasColumnName("code_colour_bonus");

                entity.Property(e => e.Quantity).HasColumnName("quantity");
            });

            modelBuilder.Entity<Customer>(entity =>
            {
                entity.HasKey(e => e.CustomerName);

                entity.ToTable("CUSTOMER");

                entity.Property(e => e.CustomerName)
                    .HasMaxLength(50)
                    .HasColumnName("customer_name");

                entity.Property(e => e.CustomerFilter)
                    .HasMaxLength(250)
                    .HasColumnName("customer_filter")
                    .HasDefaultValueSql("(N'NONE')");

                entity.Property(e => e.CustomerMsg)
                    .HasColumnType("ntext")
                    .HasColumnName("customer_msg");

                entity.Property(e => e.CustomerShowmsg).HasColumnName("customer_showmsg");
            });

            modelBuilder.Entity<Cut>(entity =>
            {
                entity.HasKey(e => e.CodeCut)
                    .HasName("PK8")
                    .IsClustered(false);

                entity.ToTable("CUT");

                entity.Property(e => e.CodeCut).HasColumnName("code_cut");

                entity.Property(e => e.Cause)
                    .HasMaxLength(50)
                    .HasColumnName("cause");

                entity.Property(e => e.CodeTmsPiece)
                    .IsRequired()
                    .HasMaxLength(10)
                    .HasColumnName("code_tms_piece");

                entity.Property(e => e.FinalMeter).HasColumnName("final_meter");

                entity.Property(e => e.InitMeter).HasColumnName("init_meter");

                entity.Property(e => e.Quality)
                    .IsRequired()
                    .HasMaxLength(1)
                    .HasColumnName("quality")
                    .HasDefaultValueSql("(N'A')");

                entity.Property(e => e.Width).HasColumnName("width");

                entity.HasOne(d => d.CodeTmsPieceNavigation)
                    .WithMany(p => p.Cuts)
                    .HasForeignKey(d => d.CodeTmsPiece)
                    .HasConstraintName("RefINSPECTED_PIECE9");
            });

            modelBuilder.Entity<Department>(entity =>
            {
                entity.HasKey(e => e.CodeDepartment)
                    .HasName("PK17")
                    .IsClustered(false);

                entity.ToTable("Department");

                entity.HasIndex(e => e.Description, "indx_description_department")
                    .IsUnique();

                entity.Property(e => e.CodeDepartment)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("code_department");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(30)
                    .HasColumnName("description");
            });

            modelBuilder.Entity<Failure>(entity =>
            {
                entity.HasKey(e => e.CodeFailure)
                    .HasName("PK15")
                    .IsClustered(false);

                entity.ToTable("FAILURE");

                entity.HasIndex(e => e.Description, "ind_description_failure")
                    .IsUnique();

                entity.Property(e => e.CodeFailure).HasColumnName("code_failure");

                entity.Property(e => e.CodeDepartment).HasColumnName("code_department");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(30)
                    .HasColumnName("description");

                entity.HasOne(d => d.CodeDepartmentNavigation)
                    .WithMany(p => p.Failures)
                    .HasForeignKey(d => d.CodeDepartment)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("RefDepartment29");
            });

            modelBuilder.Entity<InspectedPiece>(entity =>
            {
                entity.HasKey(e => e.CodeTmsPiece)
                    .HasName("PK10")
                    .IsClustered(false);

                entity.ToTable("INSPECTED_PIECE");

                entity.HasIndex(e => e.InspectionDate, "INDXDATEPIECE");

                entity.HasIndex(e => e.InspectionDate, "INDX_INSPDATE");

                entity.Property(e => e.CodeTmsPiece)
                    .HasMaxLength(10)
                    .HasColumnName("code_tms_piece");

                entity.Property(e => e.Authorized).HasColumnName("authorized");

                entity.Property(e => e.Authorizedby)
                    .HasMaxLength(10)
                    .HasColumnName("authorizedby");

                entity.Property(e => e.Creason)
                    .HasMaxLength(250)
                    .HasColumnName("creason");

                entity.Property(e => e.Dispatch).HasColumnName("dispatch");

                entity.Property(e => e.FinalMeters).HasColumnName("final_meters");

                entity.Property(e => e.FinalWeigth).HasColumnName("final_weigth");

                entity.Property(e => e.FinalWidth).HasColumnName("final_width");

                entity.Property(e => e.Inspectedby)
                    .IsRequired()
                    .HasMaxLength(10)
                    .HasColumnName("inspectedby");

                entity.Property(e => e.InspectionDate)
                    .HasColumnType("smalldatetime")
                    .HasColumnName("inspection_date");

                entity.Property(e => e.MFails).HasColumnName("m_fails");

                entity.Property(e => e.MGreen).HasColumnName("m_green");

                entity.Property(e => e.MWhite).HasColumnName("m_white");

                entity.Property(e => e.MYellow).HasColumnName("m_yellow");

                entity.Property(e => e.PieceQuality)
                    .IsRequired()
                    .HasMaxLength(2)
                    .HasColumnName("piece_quality")
                    .HasDefaultValueSql("('A')");

                entity.Property(e => e.Reinspected).HasColumnName("reinspected");

                entity.Property(e => e.ReinspectedDate)
                    .HasColumnType("smalldatetime")
                    .HasColumnName("reinspected_date");

                entity.Property(e => e.Reinspectedby)
                    .HasMaxLength(10)
                    .HasColumnName("reinspectedby");

                entity.HasOne(d => d.CodeTmsPieceNavigation)
                    .WithOne(p => p.InspectedPiece)
                    .HasForeignKey<InspectedPiece>(d => d.CodeTmsPiece)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("RefTMS_PIECE8");
            });

            modelBuilder.Entity<Log>(entity =>
            {
                entity.HasKey(e => e.CodeLog)
                    .HasName("PK9")
                    .IsClustered(false);

                entity.ToTable("LOG");

                entity.Property(e => e.CodeLog).HasColumnName("code_log");

                entity.Property(e => e.Datetime)
                    .HasColumnType("datetime")
                    .HasColumnName("datetime");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasColumnName("description");

                entity.Property(e => e.Username)
                    .IsRequired()
                    .HasMaxLength(30)
                    .HasColumnName("username");
            });

            modelBuilder.Entity<PieceAnalysis>(entity =>
            {
                entity.HasKey(e => new { e.CodeTmsPiece, e.Design, e.Quality, e.Shade });

                entity.ToTable("PIECE_ANALYSIS");

                entity.Property(e => e.CodeTmsPiece)
                    .HasMaxLength(10)
                    .HasColumnName("code_tms_piece");

                entity.Property(e => e.Design)
                    .HasMaxLength(10)
                    .HasColumnName("design");

                entity.Property(e => e.Quality)
                    .HasMaxLength(15)
                    .HasColumnName("quality");

                entity.Property(e => e.Shade)
                    .HasMaxLength(10)
                    .HasColumnName("shade");

                entity.Property(e => e.DateCreated)
                    .HasColumnType("datetime")
                    .HasColumnName("date_created");

                entity.Property(e => e.DeltaE)
                    .HasColumnType("decimal(12, 2)")
                    .HasColumnName("delta_e");

                entity.Property(e => e.DryRub)
                    .HasColumnType("decimal(12, 2)")
                    .HasColumnName("dry_rub");

                entity.Property(e => e.MartindaleAbrasion)
                    .HasColumnType("decimal(12, 2)")
                    .HasColumnName("martindale_abrasion");

                entity.Property(e => e.MartindaleAbrasionBroken).HasColumnName("martindale_abrasion_broken");

                entity.Property(e => e.Pilling)
                    .HasColumnType("decimal(12, 2)")
                    .HasColumnName("pilling");

                entity.Property(e => e.Pm2)
                    .HasColumnType("decimal(12, 2)")
                    .HasColumnName("pm2");

                entity.Property(e => e.WarpDensity)
                    .HasColumnType("decimal(12, 2)")
                    .HasColumnName("warp_density");

                entity.Property(e => e.WarpDimensional)
                    .HasColumnType("decimal(12, 2)")
                    .HasColumnName("warp_dimensional");

                entity.Property(e => e.WarpResistance)
                    .HasColumnType("decimal(12, 2)")
                    .HasColumnName("warp_resistance");

                entity.Property(e => e.WeftDensity)
                    .HasColumnType("decimal(12, 2)")
                    .HasColumnName("weft_density");

                entity.Property(e => e.WeftDimensional)
                    .HasColumnType("decimal(12, 2)")
                    .HasColumnName("weft_dimensional");

                entity.Property(e => e.WeftResistance)
                    .HasColumnType("decimal(12, 2)")
                    .HasColumnName("weft_resistance");

                entity.Property(e => e.WetRub)
                    .HasColumnType("decimal(12, 2)")
                    .HasColumnName("wet_rub");

                entity.Property(e => e.Width)
                    .HasColumnType("decimal(12, 2)")
                    .HasColumnName("width");
            });

            modelBuilder.Entity<PieceFailsSummary>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("PKNUM54")
                    .IsClustered(false);

                entity.ToTable("PIECE_FAILS_SUMMARY");

                entity.HasIndex(e => e.CodeFailure, "INDX_CODE01");

                entity.HasIndex(e => e.ColourBonus, "INDX_COLOR01");

                entity.HasIndex(e => e.Mapping, "INDX_MAP01");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Bonus).HasColumnName("bonus");

                entity.Property(e => e.CodeFailure).HasColumnName("code_failure");

                entity.Property(e => e.CodeTmsPiece)
                    .IsRequired()
                    .HasMaxLength(10)
                    .HasColumnName("code_tms_piece");

                entity.Property(e => e.ColourBonus)
                    .IsRequired()
                    .HasMaxLength(2)
                    .HasColumnName("colour_bonus");

                entity.Property(e => e.Mapping)
                    .IsRequired()
                    .HasMaxLength(2)
                    .HasColumnName("mapping");

                entity.Property(e => e.Meter).HasColumnName("meter");

                entity.Property(e => e.PieceNumber).HasColumnName("piece_number");

                entity.HasOne(d => d.PieceSummary)
                    .WithMany(p => p.PieceFailsSummaries)
                    .HasForeignKey(d => new { d.PieceNumber, d.CodeTmsPiece })
                    .HasConstraintName("RefPIECE_SUMMARY7");
            });

            modelBuilder.Entity<PieceFailure>(entity =>
            {
                entity.HasKey(e => e.CodePieceFailure)
                    .HasName("PK1")
                    .IsClustered(false);

                entity.ToTable("PIECE_FAILURE");

                entity.Property(e => e.CodePieceFailure).HasColumnName("code_piece_failure");

                entity.Property(e => e.BonusQuantity).HasColumnName("bonus_quantity");

                entity.Property(e => e.CodeFailure).HasColumnName("code_failure");

                entity.Property(e => e.CodeTmsPiece)
                    .IsRequired()
                    .HasMaxLength(10)
                    .HasColumnName("code_tms_piece");

                entity.Property(e => e.ColourBonus)
                    .IsRequired()
                    .HasMaxLength(2)
                    .HasColumnName("colour_bonus");

                entity.Property(e => e.DateFailure)
                    .HasColumnType("smalldatetime")
                    .HasColumnName("date_failure");

                entity.Property(e => e.InitMeter).HasColumnName("init_meter");

                entity.Property(e => e.Mapping)
                    .IsRequired()
                    .HasMaxLength(2)
                    .HasColumnName("mapping");

                entity.HasOne(d => d.CodeFailureNavigation)
                    .WithMany(p => p.PieceFailures)
                    .HasForeignKey(d => d.CodeFailure)
                    .HasConstraintName("RefFAILURE28");

                entity.HasOne(d => d.CodeTmsPieceNavigation)
                    .WithMany(p => p.PieceFailures)
                    .HasForeignKey(d => d.CodeTmsPiece)
                    .HasConstraintName("RefINSPECTED_PIECE10");
            });

            modelBuilder.Entity<PieceSummary>(entity =>
            {
                entity.HasKey(e => new { e.PieceNumber, e.CodeTmsPiece })
                    .HasName("PKPieceNum01")
                    .IsClustered(false);

                entity.ToTable("PIECE_SUMMARY");

                entity.HasIndex(e => e.CodeTmsPiece, "INDXTMSPIECE01");

                entity.Property(e => e.PieceNumber).HasColumnName("piece_number");

                entity.Property(e => e.CodeTmsPiece)
                    .HasMaxLength(10)
                    .HasColumnName("code_tms_piece");

                entity.Property(e => e.Bonus).HasColumnName("bonus");

                entity.Property(e => e.Creason)
                    .HasMaxLength(50)
                    .HasColumnName("creason");

                entity.Property(e => e.Green).HasColumnName("green");

                entity.Property(e => e.Meters).HasColumnName("meters");

                entity.Property(e => e.NetMeters).HasColumnName("net_meters");

                entity.Property(e => e.Quality)
                    .IsRequired()
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("quality")
                    .HasDefaultValueSql("('A')")
                    .IsFixedLength(true);

                entity.Property(e => e.White).HasColumnName("white");

                entity.Property(e => e.Yellow).HasColumnName("yellow");

                entity.HasOne(d => d.CodeTmsPieceNavigation)
                    .WithMany(p => p.PieceSummaries)
                    .HasForeignKey(d => d.CodeTmsPiece)
                    .HasConstraintName("RefINSPECTED_PIECE6");
            });

            modelBuilder.Entity<Reporte>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("Reporte");

                entity.Property(e => e.Bonus).HasColumnName("bonus");

                entity.Property(e => e.CategoryName)
                    .HasMaxLength(30)
                    .HasColumnName("category_name");

                entity.Property(e => e.CodeTmsPiece)
                    .IsRequired()
                    .HasMaxLength(10)
                    .HasColumnName("code_tms_piece");

                entity.Property(e => e.Creason)
                    .HasMaxLength(50)
                    .HasColumnName("creason");

                entity.Property(e => e.Design)
                    .HasMaxLength(10)
                    .HasColumnName("design");

                entity.Property(e => e.Green).HasColumnName("green");

                entity.Property(e => e.InspectionDate)
                    .HasColumnType("smalldatetime")
                    .HasColumnName("inspection_date");

                entity.Property(e => e.Meters).HasColumnName("meters");

                entity.Property(e => e.NameCustomer)
                    .HasMaxLength(50)
                    .HasColumnName("name_customer");

                entity.Property(e => e.NetMeters).HasColumnName("net_meters");

                entity.Property(e => e.PieceNumber).HasColumnName("piece_number");

                entity.Property(e => e.Quality)
                    .IsRequired()
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("quality")
                    .IsFixedLength(true);

                entity.Property(e => e.Shade)
                    .HasMaxLength(10)
                    .HasColumnName("shade");

                entity.Property(e => e.White).HasColumnName("white");

                entity.Property(e => e.Yellow).HasColumnName("yellow");
            });

            modelBuilder.Entity<ReporteFalla>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("ReporteFallas");

                entity.Property(e => e.Bonus).HasColumnName("bonus");

                entity.Property(e => e.CategoryName)
                    .IsRequired()
                    .HasMaxLength(30)
                    .HasColumnName("category_name");

                entity.Property(e => e.CodeFailure).HasColumnName("code_failure");

                entity.Property(e => e.CodeTmsPiece)
                    .IsRequired()
                    .HasMaxLength(10)
                    .HasColumnName("code_tms_piece");

                entity.Property(e => e.ColourBonus)
                    .IsRequired()
                    .HasMaxLength(2)
                    .HasColumnName("colour_bonus");

                entity.Property(e => e.Department)
                    .IsRequired()
                    .HasMaxLength(30)
                    .HasColumnName("department");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(30)
                    .HasColumnName("description");

                entity.Property(e => e.InspectionDate)
                    .HasColumnType("smalldatetime")
                    .HasColumnName("inspection_date");

                entity.Property(e => e.Mapping)
                    .IsRequired()
                    .HasMaxLength(2)
                    .HasColumnName("mapping");

                entity.Property(e => e.Meter).HasColumnName("meter");

                entity.Property(e => e.PieceNumber).HasColumnName("piece_number");

                entity.Property(e => e.Quality)
                    .IsRequired()
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("quality")
                    .IsFixedLength(true);
            });

            modelBuilder.Entity<Scann>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("SCANN");

                entity.Property(e => e.CodeTmsPiece)
                    .IsRequired()
                    .HasMaxLength(10)
                    .HasColumnName("code_tms_piece");

                entity.Property(e => e.FinalMeters).HasColumnName("final_meters");

                entity.Property(e => e.FinalWeigth).HasColumnName("final_weigth");

                entity.Property(e => e.FinalWidth).HasColumnName("final_width");

                entity.Property(e => e.Inspectedby)
                    .HasMaxLength(10)
                    .HasColumnName("inspectedby");

                entity.Property(e => e.MetersC).HasColumnName("Meters_C");

                entity.Property(e => e.PieceQuality)
                    .IsRequired()
                    .HasMaxLength(2)
                    .HasColumnName("piece_quality");
            });

            modelBuilder.Entity<TmsPiece>(entity =>
            {
                entity.HasKey(e => e.CodeTmsPiece)
                    .HasName("PK7")
                    .IsClustered(false);

                entity.ToTable("TMS_PIECE");

                entity.Property(e => e.CodeTmsPiece)
                    .HasMaxLength(10)
                    .HasColumnName("code_tms_piece");

                entity.Property(e => e.CategoryName)
                    .IsRequired()
                    .HasMaxLength(30)
                    .HasColumnName("category_name");

                entity.Property(e => e.CodeCustomer)
                    .IsRequired()
                    .HasMaxLength(10)
                    .HasColumnName("code_customer");

                entity.Property(e => e.CodeMender)
                    .IsRequired()
                    .HasMaxLength(5)
                    .HasColumnName("code_mender");

                entity.Property(e => e.DateScann)
                    .HasColumnType("datetime")
                    .HasColumnName("date_scann");

                entity.Property(e => e.Design)
                    .IsRequired()
                    .HasMaxLength(10)
                    .HasColumnName("design");

                entity.Property(e => e.EcruMeters).HasColumnName("ecru_meters");

                entity.Property(e => e.EcruWeigth).HasColumnName("ecru_weigth");

                entity.Property(e => e.EcruWidth).HasColumnName("ecru_width");

                entity.Property(e => e.LoomDate)
                    .HasColumnType("datetime")
                    .HasColumnName("loom_date");

                entity.Property(e => e.LoomNumber)
                    .HasMaxLength(5)
                    .HasColumnName("loom_number");

                entity.Property(e => e.NameCustomer)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("name_customer");

                entity.Property(e => e.NameMender)
                    .HasMaxLength(50)
                    .HasColumnName("name_mender");

                entity.Property(e => e.Quality)
                    .IsRequired()
                    .HasMaxLength(15)
                    .HasColumnName("quality");

                entity.Property(e => e.Shade)
                    .IsRequired()
                    .HasMaxLength(10)
                    .HasColumnName("shade");

                entity.Property(e => e.Sp)
                    .HasColumnType("numeric(10, 0)")
                    .HasColumnName("sp");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.CodeUser)
                    .HasName("pk_users")
                    .IsClustered(false);

                entity.ToTable("USERS");

                entity.HasIndex(e => e.CodeUserTms, "indx_user_unique")
                    .IsUnique();

                entity.Property(e => e.CodeUser)
                    .HasMaxLength(10)
                    .HasColumnName("code_user");

                entity.Property(e => e.CodeUserTms)
                    .IsRequired()
                    .HasMaxLength(10)
                    .HasColumnName("code_user_tms");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
