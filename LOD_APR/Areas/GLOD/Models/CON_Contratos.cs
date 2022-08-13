using LOD_APR.Areas.Admin.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace LOD_APR.Areas.GLOD.Models
{
    [Table("CON_Contratos")]
    public class CON_Contratos
    {
        public Nullable<int> IdCarpeta { get; set; }
        
        [Key]
        public int IdContrato { get; set; }
              
        [DisplayName("Código")]
        [Required]
        public string CodigoContrato { get; set; }
        [DisplayName("Nombre Contrato")]
        [Required]
        public string NombreContrato { get; set; }

        [StringLength(500, ErrorMessage = "El campo no puede exceder los 500 caracteres")]
        [DataType(DataType.MultilineText)]
        public string DescripcionContrato { get; set; }
        public string RutaImagenContrato { get; set; }
        public DateTime? FechaCreacionContrato { get; set; }
        public string UserId { get; set; }
        public string UserIdInspectorFiscal { get; set; }
        public string NumeroResolucion { get; set; }
        public int? IdTipoContrato { get; set; }

        [ForeignKey("MAE_Sucursal")]
        [Required]
        public int? IdDireccionContrato { get; set; }
        public virtual MAE_Sucursal MAE_Sucursal { get; set; }

        public int? IdModalidadContrato { get; set; }

        //[DisplayFormat(DataFormatString = "{0:N0}", ApplyFormatInEditMode = true)]
        public decimal? MontoInicialContrato { get; set; }

        [NotMapped]
        public string MontoInicialstr
        {
            get
            {
                return Convert.ToInt64(this.MontoInicialContrato).ToString();
            }
            set { this.MontoInicialContrato = Convert.ToDecimal(value.Replace(".", "")); }
        }


        public DateTime? FechaInicioContrato { get; set; }

        [ForeignKey("Empresa_Contratista")]
        [Required]
        public int? IdEmpresaContratista { get; set; }
        public virtual MAE_sujetoEconomico Empresa_Contratista { get; set; }

        public int? ModalidadReajuste { get; set; }
        public decimal? MontoVigenteContrato { get; set; }
        public int? PlazoInicialContrato { get; set; }
        public DateTime? FechaAdjudicacion { get; set; }
        public DateTime? FechaSubcripcion { get; set; }
        public decimal? MontoPresupuestado { get; set; }
        public decimal? MontoModTramite { get; set; }
        public int? PlazoVigente { get; set; }
        public bool? Activo { get; set; }
        public int? EstadoContrato { get; set; }

        [ForeignKey("Empresa_Fiscalizadora")]
        public int? IdEmpresaFiscalizadora{ get; set; }
        public virtual MAE_sujetoEconomico Empresa_Fiscalizadora { get; set; }

        [NotMapped]
        public string Creador { get; set; }

        [NotMapped]
        public HttpPostedFileBase fileImage { get; set; }

        [NotMapped]
        public List<LOD_RolesCttosContrato> Roles { get; set; }
        public List<FORM_InformesItems> FORM_InformesItems { get; set; }

        [NotMapped]
        public int Contratista { get; set; }
        [NotMapped]
        public int Fiscalizadora { get; set; }
        [NotMapped]
        public int MOP { get; set; }
    }
}