using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace LOD_APR.Areas.Admin.Models
{
    [Table("MAE_tipoCtaEmails")]
    public class MAE_tipoCtaEmails
    {
        [Key]
        public int IdTCMail { get; set; }
        public string NombreTCE { get; set; }
    }
}