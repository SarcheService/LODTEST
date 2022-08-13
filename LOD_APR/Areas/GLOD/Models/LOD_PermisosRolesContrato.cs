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
    [Table("LOD_PermisosRolesContrato")]
    public class LOD_PermisosRolesContrato
    {

        [Key]        
        public int IdPermiso { get; set; }

        [ForeignKey("LOD_RolesCttosContrato")]
        public int? IdRCContrato { get; set; }
        public virtual LOD_RolesCttosContrato LOD_RolesCttosContrato { get; set; }

        [ForeignKey("LOD_LibroObras")]
        public int IdLod { get; set; }
        public virtual LOD_LibroObras LOD_LibroObras { get; set; }

        public bool Lectura { get; set; }
        public bool Escritura { get; set; }
        public bool FirmaGob { get; set; }
        public bool FirmaFea { get; set; }
        public bool FirmaSimple { get; set; }

        [NotMapped]
        public int Indice { get; set; }
        [NotMapped]
        public int SubIndice { get; set; }

    }
}