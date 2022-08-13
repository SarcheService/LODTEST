using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace LOD_APR.Areas.Admin.Models
{
    [Table("MAE_ConfigSecurityEmail")]
    public class MAE_ConfigSecurityEmail
    {
        [Key]
        public int IdConfig { get; set; }

        [Required(ErrorMessage = "Dato obligatorio")]
        [Range(6, 50, ErrorMessage = "Ingrese un valor entre 6 y 50")]
        [Display(Name = "Largo Mínimo del Password (mínimo 6 caracteres)")]
        public int RequiredLength { get; set; }

        [Display(Name = "Requiere un caracter que no sea letra o número")]
        public bool RequireNonLetterOrDigit { get; set; }

        [Display(Name = "Requiere por lo menos un número")]
        public bool RequireDigit { get; set; }

        [Display(Name = "Requiere por lo menos una minúscula")]
        public bool RequireLowercase { get; set; }

        [Display(Name = "Requiere por lo menos una mayúscula")]
        public bool RequireUppercase { get; set; }

        [Required(ErrorMessage = "Dato obligatorio")]
        [Range(3, 10, ErrorMessage = "Ingrese un valor entre 3 y 10")]
        [Display(Name = "Reintentos máximos antes del bloqueo de la cuenta (mínimo 3)")]
        public int MaxFailedAccessAttemptsBeforeLockout { get; set; }

        [Required(ErrorMessage = "Dato obligatorio")]
        [Range(3, 1440, ErrorMessage = "Ingrese un valor entre 3 y 1440")]
        [Display(Name = "Duración en minutos del bloqueo de la cuenta (de 3 min. a 24 hrs)")]
        public int DefaultAccountLockoutTimeSpan { get; set; }

    }
}