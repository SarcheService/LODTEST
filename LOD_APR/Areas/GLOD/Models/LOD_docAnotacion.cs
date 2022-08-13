using LOD_APR.Areas.Admin.Models;
using LOD_APR.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace LOD_APR.Areas.GLOD.Models
{
    [Table("LOD_docAnotacion")]
    public class LOD_docAnotacion
    {
        [Key]
        public int IdDocAnotacion { get; set; }

        [DisplayName("Documentos")]
        [ForeignKey("MAE_documentos")]
        public int IdDoc { get; set; }
        public virtual MAE_documentos MAE_documentos { get; set; }

        [DisplayName("Anotación")]
        [ForeignKey("LOD_Anotaciones")]
        public int IdAnotacion { get; set; }
        public virtual LOD_Anotaciones LOD_Anotaciones { get; set; }

        [DisplayName("Tipo de Documento")]
        [ForeignKey("MAE_TipoDocumento")]
        public int IdTipoDoc { get; set; }
        public virtual MAE_TipoDocumento MAE_TipoDocumento { get; set; }

        [Required]
        public int IdContrato { get; set; }
        /*
         0 = pendiente
         1 = Aprobado
         2 = Rechazado
         */
        public int EstadoDoc { get; set; }

        [DataType(DataType.MultilineText)]
        [Display(Name = "Observaciones")]
        public string Observaciones { get; set; }

        public DateTime? FechaEvento { get; set; }

        [ForeignKey("UsuarioEvento")]
        public string IdUserEvento { get; set; }
        public virtual ApplicationUser UsuarioEvento { get; set; }

        [NotMapped]
        public HttpPostedFileBase PerFileName { get; set; }

        [NotMapped]
        public int IdClassDoc { get; set; }
        [NotMapped]
        public virtual MAE_ClassDoc ClassDoc { get; set; }
        [NotMapped]
        public int IdClassTwo { get; set; }
    }
}