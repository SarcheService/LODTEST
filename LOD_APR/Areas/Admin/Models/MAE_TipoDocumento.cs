using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LOD_APR.Areas.Admin.Models
{
    [Table("MAE_TipoDocumento")]
    public class MAE_TipoDocumento
    {
        [Key]
        public int IdTipo { get; set; }

        [DisplayName("Tipo Documento")]
        [Required(ErrorMessage = "Dato obligatorio")]
        public string Tipo { get; set; }

        [DisplayName("Descripción")]
        [DataType(DataType.MultilineText)]
        public string Descripcion { get; set; }

        [DisplayName("Activo")]
        public bool Activo { get; set; }
        
        [DisplayName("Tipo de Documento según Clasificación BIB")]
        public int TipoClasi { get; set; }
        /*
         1 = Formulario
         2 = Documento técnico
         3 = Documento administrativo
         4 = Otros
         */

    }
}