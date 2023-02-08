using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace TCS.WebUI
{
    public partial class ALARMContext : DbContext
    {
        public ALARMContext()
        {
        }

        public ALARMContext(DbContextOptions<ALARMContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Auditorium> Auditoria { get; set; }
        public virtual DbSet<ContactoEmergencium> ContactoEmergencia { get; set; }
        public virtual DbSet<Coordenadum> Coordenada { get; set; }
        public virtual DbSet<Dispositivo> Dispositivos { get; set; }
        public virtual DbSet<DispositivoCoordenadum> DispositivoCoordenada { get; set; }
        public virtual DbSet<Empresa> Empresas { get; set; }
        public virtual DbSet<Evento> Eventos { get; set; }
        public virtual DbSet<Password> Passwords { get; set; }
        public virtual DbSet<Portador> Portadors { get; set; }
        public virtual DbSet<Rondum> Ronda { get; set; }
        public virtual DbSet<Usuario> Usuarios { get; set; }
        public virtual DbSet<Vehiculo> Vehiculos { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
//#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
//                optionsBuilder.UseSqlServer("Server=192.168.24.11;Database=ALARM;User Id=sa; Password=Cr0ssv1ll3");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "Modern_Spanish_CI_AS");

            modelBuilder.Entity<Auditorium>(entity =>
            {
                entity.HasKey(e => e.AuditoriaId)
                    .HasName("PK_Auditoria");

                entity.HasOne(d => d.AddUser)
                    .WithMany(p => p.AuditoriumAddUsers)
                    .HasForeignKey(d => d.AddUserId)
                    .HasConstraintName("FK_Auditoria_Usuario_AddUserId");

                entity.HasOne(d => d.ChgUser)
                    .WithMany(p => p.AuditoriumChgUsers)
                    .HasForeignKey(d => d.ChgUserId)
                    .HasConstraintName("FK_Auditoria_Usuario_ChgUserId");

                entity.HasOne(d => d.Dispositivo)
                    .WithMany(p => p.Auditoria)
                    .HasForeignKey(d => d.DispositivoId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Auditoria_Dispositivo_DispositivoId");

                entity.HasOne(d => d.Usuario)
                    .WithMany(p => p.AuditoriumUsuarios)
                    .HasForeignKey(d => d.UsuarioId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Auditoria_Usuario_UsuarioId");
            });

            modelBuilder.Entity<ContactoEmergencium>(entity =>
            {
                entity.HasKey(e => e.ContactoEmergenciaId)
                    .HasName("PK_ContactoEmergencia");

                entity.HasOne(d => d.AddUser)
                    .WithMany(p => p.ContactoEmergenciumAddUsers)
                    .HasForeignKey(d => d.AddUserId)
                    .HasConstraintName("FK_ContactoEmergencia_Usuario_AddUserId");

                entity.HasOne(d => d.ChgUser)
                    .WithMany(p => p.ContactoEmergenciumChgUsers)
                    .HasForeignKey(d => d.ChgUserId)
                    .HasConstraintName("FK_ContactoEmergencia_Usuario_ChgUserId");

                entity.HasOne(d => d.Portador)
                    .WithMany(p => p.ContactoEmergencia)
                    .HasForeignKey(d => d.PortadorId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ContactoEmergencia_Portador_PortadorId");
            });

            modelBuilder.Entity<Coordenadum>(entity =>
            {
                entity.HasKey(e => e.CoordenadaId)
                    .HasName("PK_Coordenada");

                entity.HasOne(d => d.AddUser)
                    .WithMany(p => p.CoordenadumAddUsers)
                    .HasForeignKey(d => d.AddUserId)
                    .HasConstraintName("FK_Coordenada_Usuario_AddUserId");

                entity.HasOne(d => d.ChgUser)
                    .WithMany(p => p.CoordenadumChgUsers)
                    .HasForeignKey(d => d.ChgUserId)
                    .HasConstraintName("FK_Coordenada_Usuario_ChgUserId");

                entity.HasOne(d => d.Ronda)
                    .WithMany(p => p.Coordenada)
                    .HasForeignKey(d => d.RondaId)
                    .HasConstraintName("FK_Coordenada_Ronda_RondaId");
            });

            modelBuilder.Entity<Dispositivo>(entity =>
            {
                entity.HasOne(d => d.AddUser)
                    .WithMany(p => p.DispositivoAddUsers)
                    .HasForeignKey(d => d.AddUserId)
                    .HasConstraintName("FK_Dispositivo_Usuario_AddUserId");

                entity.HasOne(d => d.ChgUser)
                    .WithMany(p => p.DispositivoChgUsers)
                    .HasForeignKey(d => d.ChgUserId)
                    .HasConstraintName("FK_Dispositivo_Usuario_ChgUserId");

                entity.HasOne(d => d.Empresa)
                    .WithMany(p => p.Dispositivos)
                    .HasForeignKey(d => d.EmpresaId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Dispositivo_Empresa_EmpresaId");

                entity.HasOne(d => d.Portador)
                    .WithMany(p => p.Dispositivos)
                    .HasForeignKey(d => d.PortadorId)
                    .HasConstraintName("FK_Dispositivo_Portador_PortadorId");
            });

            modelBuilder.Entity<DispositivoCoordenadum>(entity =>
            {
                entity.HasKey(e => e.DispositivoCoordenadaId)
                    .HasName("PK_DispositivoCoordenada");

                entity.HasOne(d => d.AddUser)
                    .WithMany(p => p.DispositivoCoordenadumAddUsers)
                    .HasForeignKey(d => d.AddUserId)
                    .HasConstraintName("FK_DispositivoCoordenada_Usuario_AddUserId");

                entity.HasOne(d => d.ChgUser)
                    .WithMany(p => p.DispositivoCoordenadumChgUsers)
                    .HasForeignKey(d => d.ChgUserId)
                    .HasConstraintName("FK_DispositivoCoordenada_Usuario_ChgUserId");

                entity.HasOne(d => d.Coordenada)
                    .WithMany(p => p.DispositivoCoordenada)
                    .HasForeignKey(d => d.CoordenadaId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_DispositivoCoordenada_Coordenada_CoordenadaId");

                entity.HasOne(d => d.Dispositivo)
                    .WithMany(p => p.DispositivoCoordenada)
                    .HasForeignKey(d => d.DispositivoId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_DispositivoCoordenada_Dispositivo_DispositivoId");
            });

            modelBuilder.Entity<Empresa>(entity =>
            {
                entity.HasOne(d => d.AddUser)
                    .WithMany(p => p.EmpresaAddUsers)
                    .HasForeignKey(d => d.AddUserId)
                    .HasConstraintName("FK_Empresa_Usuario_AddUserId");

                entity.HasOne(d => d.ChgUser)
                    .WithMany(p => p.EmpresaChgUsers)
                    .HasForeignKey(d => d.ChgUserId)
                    .HasConstraintName("FK_Empresa_Usuario_ChgUserId");
            });

            modelBuilder.Entity<Evento>(entity =>
            {
                entity.HasOne(d => d.AddUser)
                    .WithMany(p => p.EventoAddUsers)
                    .HasForeignKey(d => d.AddUserId)
                    .HasConstraintName("FK_Evento_Usuario_AddUserId");

                entity.HasOne(d => d.ChgUser)
                    .WithMany(p => p.EventoChgUsers)
                    .HasForeignKey(d => d.ChgUserId)
                    .HasConstraintName("FK_Evento_Usuario_ChgUserId");

                entity.HasOne(d => d.Dispositivo)
                    .WithMany(p => p.Eventos)
                    .HasForeignKey(d => d.DispositivoId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Evento_Dispositivo_DispositivoId");

                entity.HasOne(d => d.Usuario)
                    .WithMany(p => p.EventoUsuarios)
                    .HasForeignKey(d => d.UsuarioId)
                    .HasConstraintName("FK_Evento_Usuario_UsuarioId");
            });

            modelBuilder.Entity<Password>(entity =>
            {
                entity.HasOne(d => d.AddUser)
                    .WithMany(p => p.PasswordAddUsers)
                    .HasForeignKey(d => d.AddUserId)
                    .HasConstraintName("FK_Password_Usuario_AddUserId");

                entity.HasOne(d => d.ChgUser)
                    .WithMany(p => p.PasswordChgUsers)
                    .HasForeignKey(d => d.ChgUserId)
                    .HasConstraintName("FK_Password_Usuario_ChgUserId");

                entity.HasOne(d => d.Usuario)
                    .WithMany(p => p.PasswordUsuarios)
                    .HasForeignKey(d => d.UsuarioId)
                    .HasConstraintName("FK_Password_Usuario_UsuarioId");
            });

            modelBuilder.Entity<Portador>(entity =>
            {
                entity.HasOne(d => d.AddUser)
                    .WithMany(p => p.PortadorAddUsers)
                    .HasForeignKey(d => d.AddUserId)
                    .HasConstraintName("FK_Portador_Usuario_AddUserId");

                entity.HasOne(d => d.ChgUser)
                    .WithMany(p => p.PortadorChgUsers)
                    .HasForeignKey(d => d.ChgUserId)
                    .HasConstraintName("FK_Portador_Usuario_ChgUserId");

                entity.HasOne(d => d.Empresa)
                    .WithMany(p => p.Portadors)
                    .HasForeignKey(d => d.EmpresaId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Portador_Empresa_EmpresaId");

                entity.HasOne(d => d.Vehiculo)
                    .WithMany(p => p.Portadors)
                    .HasForeignKey(d => d.VehiculoId)
                    .HasConstraintName("FK_Portador_Vehiculo_VehiculoId");
            });

            modelBuilder.Entity<Rondum>(entity =>
            {
                entity.HasKey(e => e.RondaId)
                    .HasName("PK_Ronda");

                entity.HasOne(d => d.AddUser)
                    .WithMany(p => p.RondumAddUsers)
                    .HasForeignKey(d => d.AddUserId)
                    .HasConstraintName("FK_Ronda_Usuario_AddUserId");

                entity.HasOne(d => d.ChgUser)
                    .WithMany(p => p.RondumChgUsers)
                    .HasForeignKey(d => d.ChgUserId)
                    .HasConstraintName("FK_Ronda_Usuario_ChgUserId");

                entity.HasOne(d => d.Empresa)
                    .WithMany(p => p.Ronda)
                    .HasForeignKey(d => d.EmpresaId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Ronda_Empresa_EmpresaId");
            });

            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.HasOne(d => d.AddUser)
                    .WithMany(p => p.InverseAddUser)
                    .HasForeignKey(d => d.AddUserId)
                    .HasConstraintName("FK_Usuario_Usuario_AddUserId");

                entity.HasOne(d => d.ChgUser)
                    .WithMany(p => p.InverseChgUser)
                    .HasForeignKey(d => d.ChgUserId)
                    .HasConstraintName("FK_Usuario_Usuario_ChgUserId");

                entity.HasOne(d => d.Empresa)
                    .WithMany(p => p.Usuarios)
                    .HasForeignKey(d => d.EmpresaId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Usuario_Empresa_EmpresaId");
            });

            modelBuilder.Entity<Vehiculo>(entity =>
            {
                entity.HasOne(d => d.AddUser)
                    .WithMany(p => p.VehiculoAddUsers)
                    .HasForeignKey(d => d.AddUserId)
                    .HasConstraintName("FK_Vehiculo_Usuario_AddUserId");

                entity.HasOne(d => d.ChgUser)
                    .WithMany(p => p.VehiculoChgUsers)
                    .HasForeignKey(d => d.ChgUserId)
                    .HasConstraintName("FK_Vehiculo_Usuario_ChgUserId");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
