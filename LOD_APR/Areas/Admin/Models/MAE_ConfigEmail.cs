using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace LOD_APR.Areas.Admin.Models
{
    [Table("MAE_ConfigEmail")]
    public class MAE_ConfigEmail
    {
        [Key]
        public int IdConfig { get; set; }

        [DataType(DataType.EmailAddress)]
        [Display(Name = "Correo")]
        public string Email { get; set; }

        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        public string PassEmail { get; set; }

        [Display(Name = "Puerto Salida")]
        public int PuertoSalida { get; set; }

        [Display(Name = "Uri Servidor")]
        public string UriServer { get; set; }
        public int IdTCMail { get; set; }

        public bool IsSSL { get; set; }
    }
}