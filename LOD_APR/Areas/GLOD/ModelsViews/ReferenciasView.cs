using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LOD_APR.Areas.GLOD.ModelsViews
{
    public class ReferenciasView
    {
        public int IdContrato { get; set; }

        [Required(ErrorMessage = "La Anotación de origen es Obligatoria")]
        public int IdAnotacion { get; set; }
        
        [Required(ErrorMessage = "Debe seleccionar la anotación a la cual quiere hacer referencia.")]
        public int? IdAnotacionRef { get; set; }

        [DataType(DataType.MultilineText)]
        [MaxLength(500,ErrorMessage ="Máximo 500 caracteres")]
        public string Observacion { get; set; }
    }
}