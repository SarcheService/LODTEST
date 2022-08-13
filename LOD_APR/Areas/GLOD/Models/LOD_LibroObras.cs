using LOD_APR.Areas.Admin.Models;
using LOD_APR.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using static LOD_APR.Models.Auxiliares;

namespace LOD_APR.Areas.GLOD.Models
{
    [Table("LOD_LibroObras")]
    public class LOD_LibroObras
    {
        public Nullable<int> IdCarpeta { get; set; }

        [DisplayName("Contrato")]
        [ForeignKey("CON_Contratos")]
        public int IdContrato { get; set; }
        public virtual CON_Contratos CON_Contratos { get; set; }

        [Key]
        public int IdLod { get; set; }

        [Display(Name = "Nombre LOD")]
        [Required(ErrorMessage = "El campo es obligatorio")]
        [StringLength(150, ErrorMessage = "El campo no puede exceder los 200 caracteres")]
        public string NombreLibroObra { get; set; }

        [Display(Name = "Código LOD")]
        //[Required(ErrorMessage = "El campo es obligatorio")]
        [StringLength(50, ErrorMessage = "El campo no puede exceder los 50 caracteres")]
        public string CodigoLObras { get; set; }

        [Display(Name = "Descripción")]
        [StringLength(500, ErrorMessage = "El campo no puede exceder los 500 caracteres")]
        [DataType(DataType.MultilineText)]
        public string DescripcionLObra { get; set; }

        [Display(Name = "F.Creación")]
        [DisplayFormat(ApplyFormatInEditMode = false, DataFormatString = "{dd-MM-yyyy}")]
        public DateTime FechaCreacion { get; set; }

        [ForeignKey("Usuario_Creacion")]
        public string UserId { get; set; }
        public virtual ApplicationUser Usuario_Creacion { get; set; }

        [DisplayName("Tipo de de Libro de Obra Digital")]
        [ForeignKey("MAE_TipoLOD")]
        public int IdTipoLod { get; set; }
        public virtual MAE_TipoLOD MAE_TipoLOD { get; set; }

        public int? Estado { get; set; }


        [DisplayFormat(ApplyFormatInEditMode = false, DataFormatString = "{dd-MM-yyyy}")]
        [Display(Name = "F.Cierre")]
        public Nullable<DateTime> FechaCierre { get; set; }

        [ForeignKey("Usuario_Apertura")]
        public string UsuarioApertura { get; set; }
        public virtual ApplicationUser Usuario_Apertura { get; set; }

        [ForeignKey("Usuario_Cierre")]
        public string UsuarioCierre { get; set; }
        public virtual ApplicationUser Usuario_Cierre { get; set; }

        public DateTime? FechaApertura { get; set; }
        public bool TipoApertura { get; set; }

        public string RutaImagenLObras { get; set; }
        public string OTP { get; set; }

        public bool HerImgPadre { get; set; }

        [NotMapped]
        public HttpPostedFileBase fileImage { get; set; }


        [NotMapped]
        public List<LOD_Anotaciones> LstAnotaciones { get; set; }

        [NotMapped]
        public List<LOD_Anotaciones> AnotPendRespuesta { get; set; }

        [NotMapped]
        public List<int> LstLeidas { get; set; }
        [NotMapped]
        public List<int> LstDestacadas { get; set; }

        [NotMapped]
        public DataPanelLO DataPanelLO { get; set; }


        [NotMapped]
        public string ContratoNombre
        {
            get
            {
                return CON_Contratos.NombreContrato + "/" + NombreLibroObra;
            }
        }

        public virtual ICollection<LOD_UsuariosLod> LOD_UsuariosLod { get; set; }

        [NotMapped]
        public string Creador { get; set; }

        public virtual ICollection<LOD_Anotaciones> LOD_Anotaciones { get; set; }

        [NotMapped]
        public int AnotFirmadasCount
        {
            get
            {
                return (this.LOD_Anotaciones != null) ? this.LOD_Anotaciones.Where(w => w.EstadoFirma).Count() : 0;
            }
        }

        [NotMapped]
        public int MisRespuestasPendientes { get; set; }

        [NotMapped]
        public int MisVBPendientes { get; set; }
        [NotMapped]
        public int tipo { get; set; }

        [NotMapped]
        public string TipoFirmaApertura
        {
            get
            {
                string tipo = "Firma Electrónica Avanzada";
                if (this.TipoApertura)
                    tipo = "Firma MOP";

                return tipo;
            }
        }
    }
}