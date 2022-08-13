using LOD_APR.Helpers;
using LOD_APR.Helpers.Validadores;
using LOD_APR.Models;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web;

namespace LOD_APR.Areas.Admin.Models
{
    [Table("MAE_documentos")]
    public class MAE_documentos
    {

        [Key]
        public int IdDoc { get; set; }

        [DisplayName("Nombre Documento")]
        [Required(ErrorMessage = "Dato obligatorio")]
        public string NombreDoc { get; set; }

        public string Extension { get; set; }

        [DataType(DataType.MultilineText)]
        [DisplayName("Descripción")]
        public string Descripcion { get; set; }

        public string Ruta { get; set; }

        public decimal? Mb { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd-MM-yyyy}")]
        [DisplayName("Fecha Creación")]
        public DateTime FechaCreacion { get; set; }

        //[ForeignKey("MAE_TipoDocumento")]
        //[Display(Name = "Tipo Documento")]
        //public int? IdTipo { get; set; }
        //public virtual MAE_TipoDocumento MAE_TipoDocumento { get; set; }

        [ForeignKey("MAE_Path")]
        public int? IdPath { get; set; }
        public virtual MAE_Path MAE_Path { get; set; }

        [ForeignKey("CreadoPor")]
        public string UserId { get; set; }
        public virtual ApplicationUser CreadoPor { get; set; }

        public string ContentType { get; set; }

        //[Display(Name = "Caduca")]
        //[DefaultValue(false)]
        //public bool? Caduca { get; set; }

        //[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd-MM-yyyy}")]
        //[Display(Name = "F.Vencimiento")]
        //public DateTime? FechaVencimiento { get; set; }

        //[NotMapped]
        //public int? IdPersonal { get; set; }
        //[NotMapped]
        //public int? IdSujEcon { get; set; }
        //[NotMapped]
        //public int? IdActivo { get; set; }
        //[NotMapped]
        //public int? IdProyecto { get; set; }
        //[NotMapped]
        //public int? IdObra { get; set; }
        //[NotMapped]
        //public int? IdContrato { get; set; }

        //[NotMapped]
        //public int? IdAnotacionLod { get; set; }
        //[NotMapped]
        //public int? IdTipoDocumento { get; set; }

        //[NotMapped]
        //public int? IdAnotacionBit { get; set; }

        //[NotMapped]
        //public int? IdBitacora { get; set; }

        //[NotMapped]
        //public int? IdLibroObra { get; set; }

        //ER 05-01-2020**************************
        [NotMapped]
        public int? IdEmpresa { get; set; }
        //***************************************

        //********NUEVOS CAMPOS MARA MOTOR DE DOCUMENTOS 2.0*******
        [NotMapped]
        public int? PrimaryKey { get; set; }

        //[NotMapped]
        //public OrigenDocumento? Origen { get; set; }
        //*********************************************************

        [NotMapped]
        public int IdTipoDocumento { get; set; }

        [NotMapped]
        public HttpPostedFile fileName { get; set; }

        [NotMapped]
        public string icon { get; set; }

    }
    public class MAE_documentosCDView
    {

        [Display(Name = "Nombres Documento")]
        [Required(ErrorMessage = "Dato obligatorio")]
        public string NombreDoc { get; set; }

        [DataType(DataType.MultilineText)]
        [Display(Name = "Descripción")]
        public string Descripcion { get; set; }

        public bool Caduca { get; set; }
        public int IdTipo { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd-MM-yyyy}")]
        [Display(Name = "F.Vencimiento")]
        [Required]
        public DateTime? FechaVencimiento { get; set; }

        [Required]
        [Display(Name = "Archivo")]
        [Attachment]
        public HttpPostedFileBase fileName { get; set; }

        public int? IdActivo { get; set; }
        public int? IdPersonal { get; set; }

    }
}