using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace LOD_APR.Areas.Admin.Models
{
    [Table("MAE_sistema")]
    public class MAE_sistema
    {
        [Key]
        public int IdSistema { get; set; }

        public string Sistema { get; set; }

        [Display(Name = "Nombres Sistema")]
        public string NombreSistema { get; set; }

        public bool Activo { get; set; }

        public decimal DocSize { get; set; }

        public Nullable<decimal> SizeActual { get; set; }

        public int indice { get; set; }

        public string Accion { get; set; }

        public string Controlador { get; set; }


        public virtual List<MAE_modulos> MAE_modulos { get; set; }

        [NotMapped]
        public string RolUSerApp { get; set; }
    }
}