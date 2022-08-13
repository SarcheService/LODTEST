using LOD_APR.Areas.Admin.Models;
using LOD_APR.Areas.GLOD.Helpers;
using LOD_APR.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;

namespace LOD_APR.Areas.GLOD.Models
{
    [Table("LOD_Anotaciones")]
    public class LOD_Anotaciones
    {
        [Key]
        public int IdAnotacion { get; set; }

        [DisplayName("Título")]
        [Required(ErrorMessage ="El Título es Obligatorio")]
        public string Titulo { get; set; }

        [DisplayName("Libro de Obra Digital")]
        [ForeignKey("LOD_LibroObras")]
        public int IdLod { get; set; }
        public virtual LOD_LibroObras LOD_LibroObras { get; set; }

        [DisplayName("Tipo de Comunicación")]
        [ForeignKey("MAE_SubtipoComunicacion")]
        [Required(ErrorMessage = "El Tipo de Comunicación es Obligatorio")]
        public int IdTipoSub { get; set; }
        public virtual MAE_SubtipoComunicacion MAE_SubtipoComunicacion { get; set; }

        public int Correlativo { get; set; }

        public bool EsEstructurada { get; set; }

        [AllowHtml]
        [DataType(DataType.MultilineText)]
        [Required(ErrorMessage = "El Cuerpo es Obligatorio")]
        public string Cuerpo { get; set; }

        public int Estado { get; set; }

        public DateTime? FechaIngreso { get; set; }
        [DisplayName("Fecha Publicación")]
        public DateTime? FechaPub { get; set; }
        public DateTime? FechaFirma { get; set; }

        public bool SolicitudRest { get; set; }
        public DateTime? FechaResp { get; set; }
        public DateTime? FechaTopeRespuesta { get; set; }
        
        [ForeignKey("UsuarioRemitente")]
        public string UserId { get; set; }
        public virtual ApplicationUser UsuarioRemitente { get; set; }

        public bool SolicitudVB { get; set; }
        public int TipoFirma { get; set; }
        public bool EstadoFirma { get; set; }

        [ForeignKey("UsuarioBorrador")]
        public string UserIdBorrador { get; set; }
        public virtual ApplicationUser UsuarioBorrador { get; set; }
        public string TempCode { get; set; }
        public virtual ICollection<LOD_docAnotacion> LOD_DocAnotacion { get; set; }

        [NotMapped]
        public string RutaCarpetaPdf {
            get {
                //*****************CREAMOS LA RUTA PROVISORIA DE GUARDADO DOC*********************
                EspacioComunicaciones spc = new EspacioComunicaciones(this.LOD_LibroObras.MAE_TipoLOD.TipoLodJer);
                string contrato = this.LOD_LibroObras.CON_Contratos.CodigoContrato;
                string espacio = spc.NombreEspacio;
                string libro = this.LOD_LibroObras.MAE_TipoLOD.Nombre;
                string folio = this.Correlativo.ToString().PadLeft(6, '0');
                return $"Files/Data/{contrato}/{espacio}/{libro}/{folio}/";
            }
        }
        [NotMapped]
        public string RutaPdfSinFirma
        {
            get
            {
                //*****************CREAMOS LA RUTA PROVISORIA DE GUARDADO DOC*********************
                EspacioComunicaciones spc = new EspacioComunicaciones(this.LOD_LibroObras.MAE_TipoLOD.TipoLodJer);
                string contrato = this.LOD_LibroObras.CON_Contratos.CodigoContrato;
                string espacio = spc.NombreEspacio;
                string libro = this.LOD_LibroObras.MAE_TipoLOD.Nombre;
                string folio = this.Correlativo.ToString().PadLeft(6, '0');
                return $"Files/Data/{contrato}/{espacio}/{libro}/{folio}/Folio_{folio}.preview.pdf";
            }
        }
        [NotMapped]
        public string RutaPdfConFirma
        {
            get
            {
                //*****************CREAMOS LA RUTA PROVISORIA DE GUARDADO DOC*********************
                EspacioComunicaciones spc = new EspacioComunicaciones(this.LOD_LibroObras.MAE_TipoLOD.TipoLodJer);
                string contrato = this.LOD_LibroObras.CON_Contratos.CodigoContrato;
                string espacio = spc.NombreEspacio;
                string libro = this.LOD_LibroObras.MAE_TipoLOD.Nombre;
                string folio = this.Correlativo.ToString().PadLeft(6, '0');
                return $"Files/Data/{contrato}/{espacio}/{libro}/{folio}/Folio_{folio}.pdf";
            }
        }
        [NotMapped]
        public string RutaCarpetaBorradores
        {
            get
            {
                //*****************CREAMOS LA RUTA PROVISORIA DE GUARDADO DOC*********************
                EspacioComunicaciones spc = new EspacioComunicaciones(this.LOD_LibroObras.MAE_TipoLOD.TipoLodJer);
                string contrato = this.LOD_LibroObras.CON_Contratos.CodigoContrato;
                string espacio = spc.NombreEspacio;
                string libro = this.LOD_LibroObras.MAE_TipoLOD.Nombre;
                return $"Files/Data/{contrato}/{espacio}/{libro}/Borrador_{this.IdAnotacion}/";
            }
        }

        [NotMapped]
        public int DocCargados { get; set; }
        [NotMapped]
        public int DocRechazados { get; set; }
    }
}