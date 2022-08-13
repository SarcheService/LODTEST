using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace LOD_APR.Areas.Admin.Models
{
  
    [Table("MAE_OpcionesRoles")]
    public class MAE_OpcionesRoles
    {
        [Key]

        public int IdOpcRoles { get; set; }

        public int IdEmpresa { get; set; }

        //[ForeignKey("SEG_Roles")]
        public string IdRol { get; set; }
        //public virtual MAE_personal MAE_personal { get; set; }


       
        public string IdOpcion { get; set; }
        
        //public virtual MAE_personal MAE_personal { get; set; }

    }
}