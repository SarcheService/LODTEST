using LOD_APR.Areas.Admin.Models;
using LOD_APR.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace LOD_APR.Areas.GLOD.Models
{
    [Table("LOD_UsuariosLod")]
    public class LOD_UsuariosLod
    {

        [Key]        
        public int IdUsLod { get; set; }


        [ForeignKey("LOD_RolesCttosContrato")]
        public int? IdRCContrato { get; set; }
        public virtual LOD_RolesCttosContrato LOD_RolesCttosContrato { get; set; }

        [ForeignKey("LOD_LibroObras")]
        public int IdLod { get; set; }
        public virtual LOD_LibroObras LOD_LibroObras { get; set; }


        [ForeignKey("ApplicationUser")]
        public string UserId { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }



        public bool Activo { get; set; }

        public DateTime FechaActivacion { get; set; }

        public Nullable<DateTime> FechaDesactivacion { get; set; }


    }
}