using LOD_APR.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace LOD_APR.Areas.Admin.Models
{
   
    [Table("SEG_UserContacto")]
    public class SEG_UserContacto
    {
        [Key]
        [Column(Order = 1)]
        [ForeignKey("ApplicationUser")]
        public string UserId { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }

        [Key]
        [Column(Order = 2)]
        //[ForeignKey("MAE_Contactos")]
        public int IdContacto { get; set; }
        public virtual MAE_Contactos MAE_Contactos { get; set; }

    }
}