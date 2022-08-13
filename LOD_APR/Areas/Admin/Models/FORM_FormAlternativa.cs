using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LOD_APR.Areas.Admin.Models
{
    [Table("FORM_FormAlternativa")]
    public class FORM_FormAlternativa
    {
        [Key]
        public string IdAlternativa { get; set; }

        [ForeignKey("FORM_FormPreguntas")]
        public string IdPregunta { get; set; }
        public virtual FORM_FormPreguntas FORM_FormPreguntas { get; set; }

        [MaxLength(200, ErrorMessage = "Máximo 200 Caracteres")]
        [Required]
        [DisplayName("Alternativa")]
        public string Titulo { get; set; }

        public int Indice { get; set; }

    }
}