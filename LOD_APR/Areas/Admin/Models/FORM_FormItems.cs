using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LOD_APR.Areas.Admin.Models
{
    [Table("FORM_FormItems")]
    public class FORM_FormItems
    {
        [Key]
        public string IdItem { get; set; }

        [ForeignKey("FORM_Formularios")]
        public string IdForm { get; set; }
        public virtual FORM_Formularios FORM_Formularios { get; set; }

        [MaxLength(200, ErrorMessage = "Máximo 200 Caracteres")]
        [DisplayName("Nombre Item")]
        [Required]
        public string Titulo { get; set; }

        [MaxLength(500, ErrorMessage = "Máximo 500 Caracteres")]
        [DisplayName("Instrucción")]
        [DataType(DataType.MultilineText)]
        [DisplayFormat(ConvertEmptyStringToNull = true, NullDisplayText = "")]
        public string Descripcion { get; set; }

        [DisplayName("Indice")]
        public int Indice { get; set; }

        [NotMapped]
        public bool Errores { get; set; }

        [NotMapped]
        public List<string> ErrorList { get; set; }

        public virtual List<FORM_FormPreguntas> FORM_FormPreguntas { get; set; }
    }
}