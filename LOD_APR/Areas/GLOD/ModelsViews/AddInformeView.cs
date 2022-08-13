using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LOD_APR.Areas.GLOD.ModelsViews
{
    public class AddInformeView
    {
        [Required(ErrorMessage = "La ID del Informe es Obligatoria")]
        public int IdInforme { get; set; }

        [Required(ErrorMessage = "La ID de la Anotación es Obligatoria")]
        public int IdAnotacion { get; set; }

        [Required(ErrorMessage = "Debe seleccionar el Informe que desea agregar")]
        public int? IdItem { get; set; }

        public int IdContrato { get; set; }
        public string Folio { get; set; }
        public string Fecha { get; set; }
        public string MesInformado { get; set; }
        public string Periodo { get; set; }

    }
}