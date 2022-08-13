using LOD_APR.Areas.Admin.Models;
using LOD_APR.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace LOD_APR.Areas.GLOD.Models
{
    [Table("LOD_RolesCttosContrato")]
    public class LOD_RolesCttosContrato
    {

        [Key]        
        public int IdRCContrato { get; set; }

    
        [ForeignKey("CON_Contratos")]
        public int IdContrato { get; set; }
        public virtual CON_Contratos CON_Contratos { get; set; }

        [ForeignKey("MAE_RolesContrato")]
        public int? IdRolCtto { get; set; }
        public virtual MAE_RolesContrato MAE_RolesContrato { get; set; }


        [DisplayName("Nombre Rol")]
        [Required(ErrorMessage = "Dato obligatorio")]
        public string NombreRol { get; set; }
        [DisplayName("Descripción")]
        [DataType(DataType.MultilineText)]
        public string Descripcion { get; set; }

        [NotMapped]
        public List<LOD_UsuariosLod> Usuarios { get; set; }

    }
}