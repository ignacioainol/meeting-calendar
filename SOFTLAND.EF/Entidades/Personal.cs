using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SOFTLAND.EF.Entidades
{
    [Table("sw_personal", Schema = "softland")]
    public class Personal
    {
        [Key]
        public string ficha { get; set; }
        public string codBancoSuc { get; set; } = string.Empty;
        public string codEstudios { get; set; } = string.Empty;
        public string codCajaCompens { get; set; } = string.Empty;
        public string nombres { get; set; } = string.Empty;
        public string rut { get; set; } = string.Empty;
        public string direccion { get; set; } = string.Empty;
        public string codComuna { get; set; } = string.Empty;
        public string codCiudad { get; set; } = string.Empty;
        public string telefono1 { get; set; } = string.Empty;
        public string telefono2 { get; set; } = string.Empty;
        public string telefono3 { get; set; } = string.Empty;
        public string fax { get; set; } = string.Empty;
        public string sexo { get; set; } = string.Empty;
        public DateTime? fechaNacimient { get; set; }
        public string estadoCivil { get; set; } = string.Empty;
        public string nacionalidad { get; set; } = string.Empty;
        public string situacionMilit { get; set; } = string.Empty;
        public int numCargasSimp { get; set; }
        public int numCargasInval { get; set; }
        public int numCargasMater { get; set; }
        public DateTime? fechaIngreso { get; set; }
        public DateTime? fechaPrimerCon { get; set; }
        public DateTime? fechaContratoV { get; set; }
        public string codINE { get; set; } = string.Empty;
        public DateTime? fechaFiniquito { get; set; }
        public string tipoPago { get; set; } = string.Empty;
        public string formPagoAntic { get; set; } = string.Empty;
        public string formPagoLiquiM { get; set; } = string.Empty;
        public string formPagoRentAc { get; set; } = string.Empty;
        public string formPagoFiniq { get; set; } = string.Empty;
        public string tipoCotizIsapr { get; set; } = string.Empty;
        public string adicional2pcie { get; set; } = string.Empty;
        public string AdicSegAFP { get; set; } = string.Empty;
        public string codExCaja { get; set; } = string.Empty;
        public string numCtaCte { get; set; } = string.Empty;
        public string numTarjetaConH { get; set; } = string.Empty;
        public string certSueldos { get; set; } = string.Empty;
        public string certHonorar { get; set; } = string.Empty;
        public string certHonorPart { get; set; } = string.Empty;
        public string foto { get; set; } = string.Empty;
        public string firma { get; set; } = string.Empty;
        public string rentaAccesoria { get; set; } = string.Empty;
        public string appaterno { get; set; } = string.Empty;
        public string apmaterno { get; set; } = string.Empty;
        public string nombre { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string WebSite { get; set; } = string.Empty;
        public DateTime? FecCalVac { get; set; }
        public int? AnoOtraEm { get; set; }
        public string CodSucurBan { get; set; } = string.Empty;
        public string TipoDeposito { get; set; } = string.Empty;
        public string TipoVvista { get; set; } = string.Empty;
        public string CodTipEfe { get; set; } = string.Empty;
        public DateTime? FecCertVacPro { get; set; }
        public string RolPrivado { get; set; } = string.Empty;
        public DateTime? FecTermContrato { get; set; }
        public string Usuario { get; set; } = string.Empty;
        public int? Id_ArchivoFoto { get; set; }
        public int? Id_ArchivoFirma { get; set; }
        public int? ActivoPortal { get; set; }
        public string UsuarioOT { get; set; } = string.Empty;
        public string JefeDirecto { get; set; } = string.Empty;
        public string Art145L { get; set; } = string.Empty;
        public string Anexo { get; set; } = string.Empty;
        [NotMapped]
        public string RutFormateado
        {
            get
            {
                rut = rut.Replace(".", "").TrimStart(new Char[] { '0' });
                return rut;
            }
            set
            {
                rut = value;
            }
        }
        public Personal()
        {
        }

        public virtual ICollection<Ccostoper> ccostopers { get; set; }
    }
}
