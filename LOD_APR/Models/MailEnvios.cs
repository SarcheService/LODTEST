using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace LOD_APR.Models
{
    [Table("MAIL_Envio")]
    public class MAIL_Envio
    {
        [Key]
        public int IdEnvio { get; set; }

        [Display(Name = "Asunto")]
        [StringLength(500, ErrorMessage = "El campo no puede exceder los 500 caracteres")]
        [Required(ErrorMessage = "Dato obligatorio")]
        public string Asunto { get; set; }

        [Display(Name = "Email")]
        [StringLength(256, ErrorMessage = "El campo no puede exceder los 256 caracteres")]
        [Required(ErrorMessage = "Dato obligatorio")]
        public string Email { get; set; }

        [Display(Name = "Fecha Ingreso")]
        [Required(ErrorMessage = "Dato obligatorio")]
        public DateTime FH_IngresoBD { get; set; }

        [Display(Name = "Fecha Procesamiento")]
        public DateTime? FH_Procesamiento { get; set; }

        [Display(Name = "Fecha Programación")]
        public DateTime? FH_Programacion { get; set; }

        [Display(Name = "Enviado")]
        public bool EstadoEnvio { get; set; }

        [Display(Name = "Usuario")]
        [Required(ErrorMessage = "Dato obligatorio")]
        [StringLength(256, ErrorMessage = "El campo no puede exceder los 256 caracteres")]
        public string Usuario { get; set; }

        [Display(Name = "Body")]
        [Required(ErrorMessage = "Dato obligatorio")]
        public string Body { get; set; }
    }
}