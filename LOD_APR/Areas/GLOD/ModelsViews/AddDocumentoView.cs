using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace LOD_APR.Areas.GLOD.ModelsViews
{
    public class AddDocumentoView
    {
        public int? IdPath { get; set; }

        [Required(ErrorMessage ="La ID de la Anotación es Obligatoria")]
        public int IdAnotacion { get; set; }

        [Required(ErrorMessage = "La ID del Tipo es Obligatoria")]
        public int IdTipoDoc { get; set; }

        [Required(ErrorMessage = "La ID del Contrato es Obligatoria")]
        public int IdContrato { get; set; }

        [Required(ErrorMessage = "El Nombre del Documento es Obligatorio")]
        public string Nombre { get; set; }

        [DataType(DataType.MultilineText)]
        public string Descripcion { get; set; }

        public string TipoDoc { get; set; }

        [Required(ErrorMessage = "El Archivo del Documento es Obligatorio")]
        public HttpPostedFileBase PerFileName { get; set; }
    }
}