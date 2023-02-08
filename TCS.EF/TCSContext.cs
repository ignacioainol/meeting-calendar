using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using TCS.EF.Entidades;

namespace TCS.EF
{
    public partial class TCSContext : DbContext
    {
        public TCSContext()
        {
        }

        public TCSContext(DbContextOptions<TCSContext> options)
            : base(options)
        {
        }
        public virtual DbSet<Meeting> Meetings { get; set; }
        public virtual DbSet<Meetingparticipant> Meetingparticipants { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Beam> Beam { get; set; }
        public virtual DbSet<BeamPos> BeamPos { get; set; }
        public virtual DbSet<CalendarP> CalendarP { get; set; }
        public virtual DbSet<Covid> Covid { get; set; }
        public virtual DbSet<CurrentProd> CurrentProd { get; set; }
        public virtual DbSet<CurrentProdHistory> CurrentProdHistory { get; set; }
        public virtual DbSet<CurrentShift> CurrentShift { get; set; }
        public virtual DbSet<CurrentShiftSpeed> CurrentShiftSpeed { get; set; }
        public virtual DbSet<CurrentShiftStop> CurrentShiftStop { get; set; }
        public virtual DbSet<CurrentStop> CurrentStop { get; set; }
        public virtual DbSet<DayCalendar> DayCalendar { get; set; }
        public virtual DbSet<Design> Design { get; set; }
        public virtual DbSet<Enroll> Enroll { get; set; }
        public virtual DbSet<Files> Files { get; set; }
        public virtual DbSet<Folders> Folders { get; set; }
        public virtual DbSet<DesignYarn> DesignYarn { get; set; }
        public virtual DbSet<PegPlan> PegPlan { get; set; }
        public virtual DbSet<PegPlanDraft> PegPlanDraft { get; set; }
        public virtual DbSet<PegPlanAudit> PegPlanAudit { get; set; }
        public virtual DbSet<Job> Job { get; set; }
        public virtual DbSet<JobPos> JobPos { get; set; }
        public virtual DbSet<LinkType> LinkType { get; set; }
        public virtual DbSet<Loom> Loom { get; set; }
        public virtual DbSet<LoomGroup> LoomGroup { get; set; }
        public virtual DbSet<LoomPos> LoomPos { get; set; }
        public virtual DbSet<LoomState> LoomState { get; set; }
        public virtual DbSet<LoomType> LoomType { get; set; }
        public virtual DbSet<Machine> Machine { get; set; }
        public virtual DbSet<Module> Modules { get; set; }
        public virtual DbSet<PbAuthorizedUser> PbAuthorizedUser { get; set; }
        public virtual DbSet<PbStatus> PbStatus { get; set; }
        public virtual DbSet<PbViewerFilter> PbViewerFilter { get; set; }
        public virtual DbSet<PbViewerFilterData> PbViewerFilterData { get; set; }
        public virtual DbSet<Permission> Permissions { get; set; }
        public virtual DbSet<Piece> Piece { get; set; }
        public virtual DbSet<Process> Process { get; set; }
        public virtual DbSet<Processmachine> Processmachine { get; set; }
        public virtual DbSet<ProductionHistory> ProductionHistory { get; set; }
        public virtual DbSet<ResumeTable> ResumeTable { get; set; }
        public virtual DbSet<Route> Route { get; set; }
        public virtual DbSet<Routeitem> Routeitem { get; set; }
        public virtual DbSet<Shift> Shift { get; set; }
        public virtual DbSet<ShiftHistory> ShiftHistory { get; set; }
        public virtual DbSet<ShiftPattern> ShiftPattern { get; set; }
        public virtual DbSet<ShiftStatus> ShiftStatus { get; set; }
        public virtual DbSet<Draft> Draft { get; set; }
        public virtual DbSet<State> State { get; set; }
        public virtual DbSet<StopHistory> StopHistory { get; set; }
        public virtual DbSet<StopKnottDchange> StopKnottDchange { get; set; }
        public virtual DbSet<StopType> StopType { get; set; }
        public virtual DbSet<Ticket> Ticket { get; set; }
        public virtual DbSet<Tracking> Tracking { get; set; }
        public virtual DbSet<UserPermission> UserPermissions { get; set; }
    
        public virtual DbSet<WeaverAssignation> WeaverAssignation { get; set; }
        public virtual DbSet<Worker> Worker { get; set; }
        public virtual DbSet<WorkerGroup> WorkerGroup { get; set; }
        public virtual DbSet<WorkerPosition> WorkerPosition { get; set; }
        public virtual DbSet<Yarn> Yarn { get; set; }

        public virtual DbSet<PerformanceEvaluation> PerformanceEvaluation { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server=192.168.24.11;Database=TCS;User Id=sa; Password=Cr0ssv1ll3");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Beam>(entity =>
            {
                entity.HasKey(e => e.BeamId)
                    .HasName("PK10")
                    .IsClustered(false);

                entity.ToTable("BEAM");

                entity.Property(e => e.BeamId).HasColumnName("BeamID");
            });

            modelBuilder.Entity<BeamPos>(entity =>
            {
                entity.HasKey(e => new { e.BeamId, e.Position })
                    .HasName("PK16")
                    .IsClustered(false);

                entity.ToTable("BEAM_POS");

                entity.Property(e => e.BeamId).HasColumnName("BeamID");

                entity.Property(e => e.JobNumber)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.HasOne(d => d.Beam)
                    .WithMany(p => p.BeamPos)
                    .HasForeignKey(d => d.BeamId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_BEAM_POS_BeamID");

                entity.HasOne(d => d.JobNumberNavigation)
                    .WithMany(p => p.BeamPos)
                    .HasForeignKey(d => d.JobNumber)
                    .HasConstraintName("RefJOB28");
            });

            modelBuilder.Entity<CalendarP>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.CalendarDate).HasColumnType("datetime");

                entity.Property(e => e.CalendarId).HasColumnName("CalendarID");
            });


            modelBuilder.Entity<Covid>(entity =>
            {
                entity.HasKey(e => e.CovidId);

                entity.ToTable("COVID", "dbo");

                entity.Property(u => u.CovidId)
                      .ValueGeneratedOnAdd();

                entity.HasOne(d => d.User)
                   .WithMany(p => p.Covids)
                   .HasForeignKey(d => d.UserId);

               
            });


            modelBuilder.Entity<CurrentProd>(entity =>
            {
                entity.HasKey(e => e.LoomId)
                    .HasName("PKCURPROD5445")
                    .IsClustered(false);

                entity.ToTable("CURRENT_PROD");

                entity.Property(e => e.LoomId)
                    .HasColumnName("LoomID")
                    .ValueGeneratedNever();

                entity.Property(e => e.ActualEff).HasColumnType("decimal(10, 2)");

                entity.Property(e => e.Article)
                    .HasMaxLength(25)
                    .IsUnicode(false);

                entity.Property(e => e.Date).HasColumnType("datetime");

                entity.Property(e => e.Ppc)
                    .HasColumnName("PPC")
                    .HasColumnType("decimal(18, 2)");

                entity.Property(e => e.Shift)
                    .HasMaxLength(25)
                    .IsUnicode(false);

                entity.Property(e => e.Status)
                    .HasMaxLength(25)
                    .IsUnicode(false);

                entity.Property(e => e.StdEff).HasColumnType("decimal(10, 2)");

                entity.Property(e => e.ToWarpDate).HasColumnType("date");

                entity.Property(e => e.WarpingDate).HasColumnType("date");

                entity.Property(e => e.WeaverId).HasColumnName("WeaverID");

                entity.Property(e => e.WeavingDate).HasColumnType("date");

                entity.Property(e => e.YarnStoreDate).HasColumnType("date");

                entity.HasOne(d => d.Loom)
                    .WithOne(p => p.CurrentProd)
                    .HasForeignKey<CurrentProd>(d => d.LoomId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKLOOMCURPDO34");
            });

            modelBuilder.Entity<CurrentProdHistory>(entity =>
            {
                entity.HasKey(e => new { e.LoomId, e.Hora })
                    .HasName("PKCURPRODHIST454")
                    .IsClustered(false);

                entity.ToTable("CURRENT_PROD_HISTORY");

                entity.Property(e => e.LoomId).HasColumnName("LoomID");

                entity.Property(e => e.ActualEff).HasColumnType("decimal(10, 2)");

                entity.Property(e => e.Article)
                    .HasMaxLength(25)
                    .IsUnicode(false);

                entity.Property(e => e.Date).HasColumnType("datetime");

                entity.Property(e => e.Ppc)
                    .HasColumnName("PPC")
                    .HasColumnType("decimal(18, 2)");

                entity.Property(e => e.Shift)
                    .HasMaxLength(25)
                    .IsUnicode(false);

                entity.Property(e => e.Status)
                    .HasMaxLength(25)
                    .IsUnicode(false);

                entity.Property(e => e.StdEff).HasColumnType("decimal(10, 2)");

                entity.Property(e => e.WeaverId).HasColumnName("WeaverID");

                entity.HasOne(d => d.Loom)
                    .WithMany(p => p.CurrentProdHistory)
                    .HasForeignKey(d => d.LoomId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKCURPRODHISTLOOM45");
            });

            modelBuilder.Entity<CurrentShift>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("CURRENT_SHIFT");

                entity.Property(e => e.Date).HasColumnType("datetime");

                entity.Property(e => e.Shift)
                    .HasMaxLength(10)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<CurrentShiftSpeed>(entity =>
            {
                entity.HasKey(e => new { e.LoomId, e.DateAndTime });

                entity.ToTable("CURRENT_SHIFT_SPEED");

                entity.Property(e => e.LoomId).HasColumnName("LoomID");

                entity.Property(e => e.DateAndTime).HasColumnType("datetime");

                entity.HasOne(d => d.Loom)
                    .WithMany(p => p.CurrentShiftSpeed)
                    .HasForeignKey(d => d.LoomId)
                    .HasConstraintName("FK_CURRENT_SHIFT_SPEED_LoomID");
            });

            modelBuilder.Entity<CurrentShiftStop>(entity =>
            {
                entity.HasKey(e => new { e.LoomId, e.ShiftStopId })
                    .HasName("PK5")
                    .IsClustered(false);

                entity.ToTable("CURRENT_SHIFT_STOP");

                entity.Property(e => e.LoomId).HasColumnName("LoomID");

                entity.Property(e => e.ShiftStopId)
                    .HasColumnName("ShiftStopID")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.StartHour).HasColumnType("datetime");

                entity.Property(e => e.StopHour).HasColumnType("datetime");

                entity.Property(e => e.StopTypeId).HasColumnName("StopTypeID");

                entity.HasOne(d => d.Loom)
                    .WithMany(p => p.CurrentShiftStop)
                    .HasForeignKey(d => d.LoomId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("RefLOOM27");

                entity.HasOne(d => d.StopType)
                    .WithMany(p => p.CurrentShiftStop)
                    .HasForeignKey(d => d.StopTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("RefSTOP_TYPE26");
            });

            modelBuilder.Entity<CurrentStop>(entity =>
            {
                entity.HasKey(e => e.LoomId)
                    .HasName("PK1")
                    .IsClustered(false);

                entity.ToTable("CURRENT_STOP");

                entity.Property(e => e.LoomId)
                    .HasColumnName("LoomID")
                    .ValueGeneratedNever();

                entity.Property(e => e.Date).HasColumnType("datetime");

                entity.Property(e => e.StopTypeId).HasColumnName("StopTypeID");

                entity.HasOne(d => d.Loom)
                    .WithOne(p => p.CurrentStop)
                    .HasForeignKey<CurrentStop>(d => d.LoomId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("RefLOOM24");

                entity.HasOne(d => d.StopType)
                    .WithMany(p => p.CurrentStop)
                    .HasForeignKey(d => d.StopTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("RefSTOP_TYPE25");
            });

            modelBuilder.Entity<DayCalendar>(entity =>
            {
                entity.HasKey(e => e.Date)
                    .HasName("PK35")
                    .IsClustered(false);

                entity.ToTable("DAY_CALENDAR");

                entity.Property(e => e.Date).HasColumnType("date");
            });

            modelBuilder.Entity<Enroll>(entity =>
            {
                entity.HasKey(e => e.EnrollId);
                entity.ToTable("ENROLL", "app");
            });

            modelBuilder.Entity<Files>(entity =>
            {
                entity.HasKey(e => e.FileId);

                entity.ToTable("FILES", "sig");

                entity.Property(u => u.FileId)
                      .ValueGeneratedOnAdd();
                entity.HasOne(d => d.Folders)
                  .WithMany(p => p.Files)
                  .HasForeignKey(d => d.FolderId);
                 

            });
            modelBuilder.Entity<Folders>(entity =>
            {
                entity.HasKey(e => e.FolderId);

                entity.ToTable("FOLDERS", "sig");

                entity.Property(u => u.FolderId)
                      .ValueGeneratedOnAdd();


            });

            modelBuilder.Entity<Meeting>(entity =>
            {
                entity.ToTable("MEETINGS", "sig");

                entity.Property(e => e.Description).HasColumnType("text");

                entity.Property(e => e.EndDate).HasColumnType("date");

                entity.Property(e => e.StartDate).HasColumnType("date");

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Meetings)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_MEETINGS_OWNER_USERS");
            });

            modelBuilder.Entity<Meetingparticipant>(entity =>
            {
                entity.HasKey(e => new { e.MeetingId, e.UserId });

                entity.ToTable("MEETINGPARTICIPANTS", "sig");

                entity.Property(e => e.Justification).HasColumnType("text");

                entity.HasOne(d => d.Meeting)
                    .WithMany(p => p.Meetingparticipants)
                    .HasForeignKey(d => d.MeetingId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_MEETINGPARTICIPANTS_MEETINGS");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Meetingparticipants)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_MEETINGPARTICIPANTS_USERS");
            });


            modelBuilder.Entity<PegPlan>(entity =>
            {
                entity.HasKey(e => e.PegPlanId)
                    .HasName("PK_PEGPLAN");

                entity.Property(u => u.PegPlanId)
                      .ValueGeneratedOnAdd();
                entity.ToTable("PEGPLAN", "design");

                entity.HasIndex(e => e.Code)
                    .HasName("UQ__DRAFTS")
                    .IsUnique();

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.Description)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.DraftPlan)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Drafts)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_DRAFTS_USERS");
                entity.HasOne(d => d.User)
                   .WithMany(p => p.Drafts)
                   .HasForeignKey(d => d.AuthorizedBy);
            });


            modelBuilder.Entity<PegPlanAudit>(entity =>
            {
                entity.HasKey(e => e.PegPlanAuditId);

                entity.ToTable("PEGPLANAUDIT", "design");

                entity.Property(u => u.PegPlanAuditId)
                      .ValueGeneratedOnAdd();

                entity.HasOne(d => d.Draft)
                   .WithMany(p => p.DraftsAudit)
                   .HasForeignKey(d => d.PegPlanId);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.DraftsAudit)
                    .HasForeignKey(d => d.UserId);
            });

            modelBuilder.Entity<PegPlanDraft>(entity =>
            {
                entity.HasKey(e => new  { e.PegPlanId, e.DraftId});

                entity.ToTable("PegPlanDraft", "design");
                entity.HasOne(d => d.PegPlan)
                   .WithMany(p => p.DraftSleys)
                   .HasForeignKey(d => d.PegPlanId);

                entity.HasOne(d => d.Draft)
                   .WithMany(p => p.DraftSleys)
                   .HasForeignKey(d => d.DraftId);

            });


            modelBuilder.Entity<Job>(entity =>
            {
                entity.HasKey(e => e.JobNumber)
                    .HasName("PK9")
                    .IsClustered(false);

                entity.ToTable("JOB");

                entity.Property(e => e.JobNumber)
                    .HasMaxLength(10)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<JobPos>(entity =>
            {
                entity.HasKey(e => new { e.JobNumber, e.Position })
                    .HasName("PK14")
                    .IsClustered(false);

                entity.ToTable("JOB_POS");

                entity.Property(e => e.JobNumber)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.TicketId)
                    .HasColumnName("TicketID")
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.HasOne(d => d.JobNumberNavigation)
                    .WithMany(p => p.JobPos)
                    .HasForeignKey(d => d.JobNumber)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("RefJOB23");

                entity.HasOne(d => d.Ticket)
                    .WithMany(p => p.JobPos)
                    .HasForeignKey(d => d.TicketId)
                    .HasConstraintName("RefTICKET31");
            });

            modelBuilder.Entity<LinkType>(entity =>
            {
                entity.HasKey(e => e.LinkId)
                    .HasName("PK17")
                    .IsClustered(false);

                entity.ToTable("LINK_TYPE");

                entity.Property(e => e.LinkId).HasColumnName("LinkID");

                entity.Property(e => e.Description)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Loom>(entity =>
            {
                entity.HasKey(e => e.LoomId)
                    .HasName("PK8")
                    .IsClustered(false);

                entity.ToTable("LOOM");

                entity.Property(e => e.LoomId).HasColumnName("LoomID");

                entity.Property(e => e.IsCalculated).HasDefaultValueSql("((1))");

                entity.Property(e => e.LastTimeOnline).HasColumnType("datetime");

                entity.Property(e => e.LoomGroupId).HasColumnName("LoomGroupID");

                entity.Property(e => e.LoomIp)
                    .HasColumnName("LoomIP")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.LoomNumber)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.LoomTypeId).HasColumnName("LoomTypeID");

                entity.Property(e => e.Serial)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.LoomGroup)
                    .WithMany(p => p.Loom)
                    .HasForeignKey(d => d.LoomGroupId)
                    .HasConstraintName("FK_LOOM_GROUP_ID");

                entity.HasOne(d => d.LoomType)
                    .WithMany(p => p.Loom)
                    .HasForeignKey(d => d.LoomTypeId)
                    .HasConstraintName("RefLOOM_TYPE10");
            });

            modelBuilder.Entity<LoomGroup>(entity =>
            {
                entity.HasKey(e => e.LoomGroupId)
                    .HasName("GROUPLOOMPK54")
                    .IsClustered(false);

                entity.ToTable("LOOM_GROUP");

                entity.Property(e => e.LoomGroupId).HasColumnName("LoomGroupID");

                entity.Property(e => e.Name)
                    .HasMaxLength(25)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<LoomPos>(entity =>
            {
                entity.HasKey(e => new { e.LoomId, e.Position })
                    .HasName("PK13")
                    .IsClustered(false);

                entity.ToTable("LOOM_POS");

                entity.Property(e => e.LoomId).HasColumnName("LoomID");

                entity.Property(e => e.BeamId).HasColumnName("BeamID");

                entity.Property(e => e.LinkId).HasColumnName("LinkID");

                entity.HasOne(d => d.Beam)
                    .WithMany(p => p.LoomPos)
                    .HasForeignKey(d => d.BeamId)
                    .HasConstraintName("FK_LOOM_POS_BeamID");

                entity.HasOne(d => d.Link)
                    .WithMany(p => p.LoomPos)
                    .HasForeignKey(d => d.LinkId)
                    .HasConstraintName("RefLINK_TYPE29");

                entity.HasOne(d => d.Loom)
                    .WithMany(p => p.LoomPos)
                    .HasForeignKey(d => d.LoomId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("RefLOOM12");
            });

            modelBuilder.Entity<LoomState>(entity =>
            {
                entity.HasKey(e => e.LoomId)
                    .HasName("PKLOOMSTATE")
                    .IsClustered(false);

                entity.ToTable("LOOM_STATE");

                entity.Property(e => e.LoomId)
                    .HasColumnName("LoomID")
                    .ValueGeneratedNever();

                entity.Property(e => e.DurationDate).HasColumnType("datetime");

                entity.Property(e => e.StateId).HasColumnName("StateID");

                entity.HasOne(d => d.Loom)
                    .WithOne(p => p.LoomState)
                    .HasForeignKey<LoomState>(d => d.LoomId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_LOOM_STATE_LoomID");

                entity.HasOne(d => d.State)
                    .WithMany(p => p.LoomState)
                    .HasForeignKey(d => d.StateId)
                    .HasConstraintName("FK_LOOM_STATE_StateID");
            });

            modelBuilder.Entity<LoomType>(entity =>
            {
                entity.HasKey(e => e.LoomTypeId)
                    .HasName("PK12")
                    .IsClustered(false);

                entity.ToTable("LOOM_TYPE");

                entity.Property(e => e.LoomTypeId).HasColumnName("LoomTypeID");

                entity.Property(e => e.Description)
                    .HasMaxLength(20)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Machine>(entity =>
            {
                entity.HasKey(e => e.MachineId)
                    .HasName("PK2")
                    .IsClustered(false);

                entity.ToTable("MACHINE", "finish");

                entity.Property(e => e.MachineId).ValueGeneratedNever();

                entity.Property(e => e.MachineDescription)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.MachineIp)
                    .HasColumnName("MachineIP")
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Module>(entity =>
            {
                entity.HasKey(e => e.ModuleId);

                entity.ToTable("MODULES", "settings");

                entity.Property(e => e.Description)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<PbAuthorizedUser>(entity =>
            {
                entity.HasKey(e => e.UserId)
                    .HasName("PK_PB_AUTHORIZED_USER_UserID");

                entity.ToTable("PB_AUTHORIZED_USER");

                entity.Property(e => e.UserId)
                    .HasColumnName("UserID")
                    .HasMaxLength(20)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<PbStatus>(entity =>
            {
                entity.HasKey(e => e.StatusId)
                    .HasName("PK11")
                    .IsClustered(false);

                entity.ToTable("PB_STATUS");

                entity.Property(e => e.StatusId)
                    .HasColumnName("StatusID")
                    .ValueGeneratedNever();
            });

            modelBuilder.Entity<PbViewerFilter>(entity =>
            {
                entity.HasKey(e => e.FilterName)
                    .HasName("PBVIEWER_PK_1");

                entity.ToTable("PB_VIEWER_FILTER");

                entity.Property(e => e.FilterName)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Description).HasColumnType("text");

                entity.Property(e => e.FilterType)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.HideLoom).HasDefaultValueSql("((0))");

                entity.Property(e => e.Minutes).HasDefaultValueSql("((0))");

                entity.Property(e => e.UserId)
                    .IsRequired()
                    .HasColumnName("UserID")
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.Zoom).HasDefaultValueSql("((0))");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.PbViewerFilter)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("PBVIEWER_FILTER_FK1_USER");
            });

            modelBuilder.Entity<PbViewerFilterData>(entity =>
            {
                entity.HasKey(e => new { e.FilterName, e.FilterData })
                    .HasName("FILTER_DATA_PK_3");

                entity.ToTable("PB_VIEWER_FILTER_DATA");

                entity.Property(e => e.FilterName)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.FilterData)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.FilterNameNavigation)
                    .WithMany(p => p.PbViewerFilterData)
                    .HasForeignKey(d => d.FilterName)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FILTER_DATA_PK_12");
            });

            modelBuilder.Entity<Permission>(entity =>
            {
                entity.HasKey(e => e.PermissionId);

                entity.ToTable("PERMISSIONS", "settings");

                entity.Property(e => e.Description)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Piece>(entity =>
            {
                entity.HasKey(e => e.PieceNumber)
                    .HasName("PK6")
                    .IsClustered(false);

                entity.ToTable("PIECE");

                entity.Property(e => e.PieceNumber)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.BatchNumber).HasMaxLength(50);

                entity.Property(e => e.ProgStatus)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.Quality)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.RouteId)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.TicketId)
                    .IsRequired()
                    .HasColumnName("TicketID")
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.HasOne(d => d.Ticket)
                    .WithMany(p => p.Piece)
                    .HasForeignKey(d => d.TicketId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("RefTICKET34");
            });

            modelBuilder.Entity<Process>(entity =>
            {
                entity.HasKey(e => e.ProcessId)
                    .HasName("PK1")
                    .IsClustered(false);

                entity.ToTable("PROCESS", "finish");

                entity.Property(e => e.ProcessId).ValueGeneratedNever();

                entity.Property(e => e.ProcessDescription)
                    .HasMaxLength(100)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Processmachine>(entity =>
            {
                entity.HasKey(e => new { e.ProcessId, e.MachineId })
                    .HasName("PK3")
                    .IsClustered(false);

                entity.ToTable("PROCESSMACHINE", "finish");

                entity.HasOne(d => d.Machine)
                    .WithMany(p => p.Processmachine)
                    .HasForeignKey(d => d.MachineId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("RefMachine13");

                entity.HasOne(d => d.Process)
                    .WithMany(p => p.Processmachine)
                    .HasForeignKey(d => d.ProcessId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("RefProcess12");
            });

            modelBuilder.Entity<ProductionHistory>(entity =>
            {
                entity.HasKey(e => new { e.LoomId, e.Date, e.Shift })
                    .HasName("PK12343")
                    .IsClustered(false);

                entity.ToTable("PRODUCTION_HISTORY");

                entity.Property(e => e.LoomId).HasColumnName("LoomID");

                entity.Property(e => e.Date).HasColumnType("date");

                entity.Property(e => e.Shift)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.ActualEff).HasColumnType("decimal(10, 2)");

                entity.Property(e => e.Article)
                    .HasMaxLength(25)
                    .IsUnicode(false);

                entity.Property(e => e.LoomGroupId).HasColumnName("LoomGroupID");

                entity.Property(e => e.Ppc)
                    .HasColumnName("PPC")
                    .HasColumnType("decimal(18, 2)");

                entity.Property(e => e.StdEff).HasColumnType("decimal(10, 2)");

                entity.Property(e => e.WeaverId).HasColumnName("WeaverID");

                entity.HasOne(d => d.Loom)
                    .WithMany(p => p.ProductionHistory)
                    .HasForeignKey(d => d.LoomId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKLOOMHISTORY13");

                entity.HasOne(d => d.Weaver)
                    .WithMany(p => p.ProductionHistory)
                    .HasForeignKey(d => d.WeaverId)
                    .HasConstraintName("RefWORKER53");
            });

            modelBuilder.Entity<ResumeTable>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("RESUME_TABLE");

                entity.Property(e => e.BeamId)
                    .HasColumnName("BeamID")
                    .IsUnicode(false);

                entity.Property(e => e.Cuarta).IsUnicode(false);

                entity.Property(e => e.Customer).IsUnicode(false);

                entity.Property(e => e.Date).HasColumnType("datetime");

                entity.Property(e => e.JobNumber).IsUnicode(false);

                entity.Property(e => e.LinkDescription).IsUnicode(false);

                entity.Property(e => e.LinkHours).IsUnicode(false);

                entity.Property(e => e.LinkId)
                    .HasColumnName("LinkID")
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.LoomId)
                    .HasColumnName("LoomID")
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.LoomNumber)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.PrimeraLinea).IsUnicode(false);

                entity.Property(e => e.ProgStatus).IsUnicode(false);

                entity.Property(e => e.SegundaLinea).IsUnicode(false);

                entity.Property(e => e.Tercera).IsUnicode(false);

                entity.Property(e => e.TicketName).IsUnicode(false);

                entity.Property(e => e.WeekNumber)
                    .HasColumnName("weekNumber")
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Route>(entity =>
            {
                entity.HasKey(e => e.RouteId)
                    .HasName("PK5")
                    .IsClustered(false);

                entity.ToTable("ROUTE", "finish");

                entity.Property(e => e.RouteId)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.RouteDescription)
                    .HasMaxLength(100)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Routeitem>(entity =>
            {
                entity.HasKey(e => new { e.ProcessId, e.RouteId, e.NumberOrder })
                    .HasName("PK6")
                    .IsClustered(false);

                entity.ToTable("ROUTEITEM", "finish");

                entity.Property(e => e.RouteId)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.Process)
                    .WithMany(p => p.Routeitem)
                    .HasForeignKey(d => d.ProcessId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("RefProcess7");

                entity.HasOne(d => d.Route)
                    .WithMany(p => p.Routeitem)
                    .HasForeignKey(d => d.RouteId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("RefRoute8");
            });

            modelBuilder.Entity<Shift>(entity =>
            {
                entity.HasKey(e => e.ShiftId)
                    .HasName("PKSHIFT55654")
                    .IsClustered(false);

                entity.ToTable("SHIFT");

                entity.Property(e => e.ShiftId).HasColumnName("ShiftID");

                entity.Property(e => e.EndHour).HasColumnType("datetime");

                entity.Property(e => e.ShiftLetter)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.StartHour).HasColumnType("datetime");
            });

            modelBuilder.Entity<ShiftHistory>(entity =>
            {
                entity.HasKey(e => new { e.Shift, e.Date })
                    .HasName("PK41")
                    .IsClustered(false);

                entity.ToTable("SHIFT_HISTORY");

                entity.Property(e => e.Shift)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.Date).HasColumnType("date");

                entity.Property(e => e.SupervisorId).HasColumnName("SupervisorID");

                entity.HasOne(d => d.Supervisor)
                    .WithMany(p => p.ShiftHistory)
                    .HasForeignKey(d => d.SupervisorId)
                    .HasConstraintName("RefWORKER54");
            });

            modelBuilder.Entity<ShiftPattern>(entity =>
            {
                entity.HasKey(e => e.PatternId)
                .HasName("PK_SHIFT_PATTERN")
                 .IsClustered(false);

                entity.ToTable("SHIFT_PATTERN");

                entity.Property(e => e.Pattern)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.PatternId)
                    .HasColumnName("PatternID")
                    .HasMaxLength(10)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<ShiftStatus>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("SHIFT_STATUS");

                entity.Property(e => e.LoomTypeId).HasColumnName("LoomTypeID");
            });

            modelBuilder.Entity<Draft>(entity =>
            {
                entity.HasKey(e => e.DraftId)
                    .HasName("PK_SLEY");

                entity.ToTable("DRAFT", "design");

                entity.HasIndex(e => e.Code)
                    .HasName("UQ__SLEYS")
                    .IsUnique();


                entity.HasOne(d => d.User)
                    .WithMany(p => p.Sleys)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_DRAFTS_USERS");

                entity.HasOne(d => d.User)
                   .WithMany(p => p.Sleys)
                   .HasForeignKey(d => d.AuthorizedBy);
            });
            modelBuilder.Entity<State>(entity =>
            {
                entity.HasKey(e => e.StateId)
                    .HasName("PK18")
                    .IsClustered(false);

                entity.ToTable("STATE");

                entity.HasIndex(e => e.StateId)
                    .HasName("KEY_STATE_StateID")
                    .IsUnique();

                entity.Property(e => e.StateId).HasColumnName("StateID");

                entity.Property(e => e.Description)
                    .HasMaxLength(30)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<StopHistory>(entity =>
            {
                entity.HasKey(e => new { e.LoomId, e.Date, e.Shift, e.StopTypeId })
                    .HasName("PK34")
                    .IsClustered(false);

                entity.ToTable("STOP_HISTORY");

                entity.Property(e => e.LoomId).HasColumnName("LoomID");

                entity.Property(e => e.Date).HasColumnType("date");

                entity.Property(e => e.Shift)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.StopTypeId).HasColumnName("StopTypeID");

                entity.HasOne(d => d.StopType)
                    .WithMany(p => p.StopHistory)
                    .HasForeignKey(d => d.StopTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("RefSTOP_TYPE35");

                entity.HasOne(d => d.ProductionHistory)
                    .WithMany(p => p.StopHistory)
                    .HasForeignKey(d => new { d.LoomId, d.Date, d.Shift })
                    .HasConstraintName("RefPRODUCTION_HISTORY34");
            });

            modelBuilder.Entity<StopKnottDchange>(entity =>
            {
                entity.HasKey(e => e.LoomId)
                    .HasName("PK_StopKnottChange");

                entity.ToTable("STOP_KNOTT_DCHANGE");

                entity.Property(e => e.LoomId)
                    .HasColumnName("LoomID")
                    .ValueGeneratedNever();

                entity.Property(e => e.DateStart).HasColumnType("datetime");

                entity.Property(e => e.LinkId).HasColumnName("LinkID");
            });

            modelBuilder.Entity<StopType>(entity =>
            {
                entity.HasKey(e => e.StopTypeId)
                    .HasName("PK2")
                    .IsClustered(false);

                entity.ToTable("STOP_TYPE");

                entity.Property(e => e.StopTypeId).HasColumnName("StopTypeID");

                entity.Property(e => e.Description)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Ticket>(entity =>
            {
                entity.HasKey(e => e.TicketId)
                    .HasName("PK7")
                    .IsClustered(false);

                entity.ToTable("TICKET");

                entity.Property(e => e.TicketId)
                    .HasColumnName("TicketID")
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.Customer)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Design)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.Draft)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.Healds)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.LoomNumber)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.Pcolour)
                    .HasColumnName("PColour")
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.Ppc)
                    .HasColumnName("PPC")
                    .HasColumnType("decimal(18, 2)");

                entity.Property(e => e.ProductionOrder)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.Quality)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.Selvedge)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.SelvedgeColour)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.SelvedgeColourId)
                    .HasColumnName("SelvedgeColourID")
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.SelvedgeWording)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.Sley).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.ToWarpDate).HasColumnType("date");

                entity.Property(e => e.WarpShade)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.WarpingDate).HasColumnType("date");

                entity.Property(e => e.WeaveWeek)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.WeavingDate).HasColumnType("date");

                entity.Property(e => e.WeftShade)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.YarnStoreDate).HasColumnType("date");
            });

            modelBuilder.Entity<Tracking>(entity =>
            {
                entity.HasKey(e => new { e.PieceNumber, e.ProcessId, e.MachineId });

                entity.ToTable("TRACKING", "finish");

                entity.Property(e => e.PieceNumber)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ScannDate).HasColumnType("datetime");

                entity.HasOne(d => d.Processmachine)
                    .WithMany(p => p.Tracking)
                    .HasForeignKey(d => new { d.ProcessId, d.MachineId })
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("RefProcessMachine17");
            });

            modelBuilder.Entity<UserPermission>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.PermissionId, e.ModuleId });

                entity.ToTable("USER_PERMISSIONS", "settings");

                entity.HasOne(d => d.Module)
                    .WithMany(p => p.UserPermissions)
                    .HasForeignKey(d => d.ModuleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_USER_PERMISSIONS_MODULES1");

                entity.HasOne(d => d.Permission)
                    .WithMany(p => p.UserPermissions)
                    .HasForeignKey(d => d.PermissionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_USER_PERMISSIONS_PERMISSIONS");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UserPermissions)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_USER_PERMISSIONS_USERS");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.UserId);

                entity.ToTable("USERS", "settings");

                entity.Property(e => e.Culture)
                    .IsRequired()
                    .HasMaxLength(5);

                entity.Property(e => e.Department).HasMaxLength(100);

                entity.Property(e => e.Email).HasMaxLength(128);

                entity.Property(e => e.FirstName).HasMaxLength(20);

                entity.Property(e => e.Language)
                    .IsRequired()
                    .HasMaxLength(5);

                entity.Property(e => e.LastLogin).HasColumnType("datetime");

                entity.Property(e => e.LastName).HasMaxLength(20);

                entity.Property(e => e.PhoneLine).HasMaxLength(20);

                entity.Property(e => e.Photo).IsUnicode(false);

                entity.Property(e => e.UserName)
                    .IsRequired()
                    .HasMaxLength(20);
            });

            modelBuilder.Entity<WeaverAssignation>(entity =>
            {
                entity.HasKey(e => new { e.LoomGroupId, e.WorkerGroupId })
                    .HasName("PK39")
                    .IsClustered(false);

                entity.ToTable("WEAVER_ASSIGNATION");

                entity.Property(e => e.LoomGroupId).HasColumnName("LoomGroupID");

                entity.Property(e => e.WorkerGroupId).HasColumnName("WorkerGroupID");

                entity.Property(e => e.WorkerId).HasColumnName("WorkerID");

                entity.HasOne(d => d.LoomGroup)
                    .WithMany(p => p.WeaverAssignation)
                    .HasForeignKey(d => d.LoomGroupId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("RefLOOM_GROUP47");

                entity.HasOne(d => d.WorkerGroup)
                    .WithMany(p => p.WeaverAssignation)
                    .HasForeignKey(d => d.WorkerGroupId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("RefWORKER_GROUP49");

                entity.HasOne(d => d.Worker)
                    .WithMany(p => p.WeaverAssignation)
                    .HasForeignKey(d => d.WorkerId)
                    .HasConstraintName("RefWORKER46");
            });

            modelBuilder.Entity<Worker>(entity =>
            {
                entity.HasKey(e => e.WorkerId)
                    .HasName("PK37")
                    .IsClustered(false);

                entity.ToTable("WORKER");

                entity.Property(e => e.WorkerId).HasColumnName("WorkerID");

                entity.Property(e => e.Ficha)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.WorkerPosition)
                    .WithMany(p => p.Worker)
                    .HasForeignKey(d => d.PositionId);

                entity.HasOne(d => d.WorkerGroup)
                    .WithMany(p => p.Worker)
                    .HasForeignKey(d => d.WorkerGroupId);

                entity.Property(e => e.WorkerName)
                    .HasMaxLength(100)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<WorkerGroup>(entity =>
            {
                entity.HasKey(e => e.WorkerGroupId)
                    .HasName("PKGROUPWEAVER12")
                    .IsClustered(false);

                entity.ToTable("WORKER_GROUP");

                entity.Property(e => e.WorkerGroupId).HasColumnName("WorkerGroupID");

                entity.Property(e => e.WorkerGroupName)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<WorkerPosition>(entity =>
            {
                entity.HasKey(e => e.PositionId)
                    .HasName("PK38")
                    .IsClustered(false);

                entity.ToTable("WORKER_POSITION");

                entity.Property(e => e.PositionId).HasColumnName("PositionID");

                entity.Property(e => e.Endescr)
                    .HasColumnName("ENDESCR")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Spdescr)
                    .HasColumnName("SPDESCR")
                    .HasMaxLength(100)
                    .IsUnicode(false);
            });


            modelBuilder.Entity<Design>(entity =>
            {
                entity.HasKey(e => e.DesignId);

                entity.ToTable("DESIGN", "design");

                entity.Property(u => u.DesignId)
                      .ValueGeneratedOnAdd();

                entity.HasOne(d => d.Draft)
                   .WithMany(p => p.Designs)
                   .HasForeignKey(d => d.DraftId);

                entity.HasOne(d => d.PegPlan)
                   .WithMany(p => p.Designs)
                   .HasForeignKey(d => d.PegPlanId);
            });

            modelBuilder.Entity<DesignYarn>(entity =>
            {
                entity.HasKey(e => e.DesignYarnId);

                entity.ToTable("DesignYarn", "design");

                entity.HasOne(d => d.Design)
                   .WithMany(p => p.DesignYarns)
                   .HasForeignKey(d => d.DesignId);

                entity.HasOne(d => d.Yarn)
                   .WithMany(p => p.DesignYarns)
                   .HasForeignKey(d => d.YarnId);
            });


            modelBuilder.Entity<Yarn>(entity =>
            {
                entity.HasKey(e => e.YarnId);

                entity.ToTable("YARN", "design");

            });

            modelBuilder.Entity<PerformanceEvaluation>(entity =>
            {
                entity.HasKey(e => e.PerformanceEvaluationId);

                entity.ToTable("PERFORMANCE_EVALUATION");

            });


            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
