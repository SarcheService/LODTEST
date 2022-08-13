using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LOD_APR.Areas.Admin.Models
{
    [Table("FORM_Formularios")]
    public class FORM_Formularios
    {
        [Key]
        public string IdForm { get; set; }

        [MaxLength(200, ErrorMessage = "Máximo 200 Caracteres")]
        [DisplayName("Nombre Formulario")]
        [Required]
        public string Titulo { get; set; }

        [MaxLength(500, ErrorMessage = "Máximo 500 Caracteres")]
        [DisplayName("Descripción")]
        [DataType(DataType.MultilineText)]
        public string Descripcion { get; set; }
        
        [DisplayName("Fecha Creación")]
        [Required]
        public DateTime FechaCreacion { get; set; }

        [DisplayName("Usuario")]
        public string UserId { get; set; }

        [DisplayName("Activo")]
        public bool Activa { get; set; }
        
        [DisplayName("Formulario Embebido")]
        public bool Embebido { get; set; }

        [DisplayName("Obligatorio")]
        public bool Obligatorio { get; set; }

        [DisplayName("Tipo")]
        public int Tipo { get; set; }

        public virtual List<FORM_FormItems> FORM_FormItems { get; set; }
        
    }
}