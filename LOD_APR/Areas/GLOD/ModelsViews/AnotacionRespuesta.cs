using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LOD_APR.Areas.GLOD.ModelsViews
{
    public class AnotacionRespuesta
    {
        [Required(ErrorMessage = "La Id de Referencia es obligatoria")]
        public int IdAnotacionRef { get; set; }

        [Required(ErrorMessage = "El libro de Obras es obligatorio")]
        public int IdLibro { get; set; }

        public int IdContrato { get; set; }

        [Required(ErrorMessage = "El Título es obligatorio")]
        [StringLength(150, ErrorMessage = "El campo no puede exceder los 150 caracteres")]
        public string Titulo { get; set; }

        [Required(ErrorMessage = "El Cuerpo es obligatorio")]
        [AllowHtml]
        [DataType(DataType.MultilineText)]
        public string Cuerpo { get; set; }

        [DisplayName("Subtipo Comunicación")]
        [Required(ErrorMessage = "El Subtipo es obligatorio")]
        [Range(1, 99999, ErrorMessage = "El Subtipo es obligatorio")]
        public int IdTipoSub { get; set; }

    }
}