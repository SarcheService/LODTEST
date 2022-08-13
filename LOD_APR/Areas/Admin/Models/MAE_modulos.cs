using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LOD_APR.Areas.Admin.Models
{
    [Table("MAE_modulos")]
    public class MAE_modulos
    {
        [Key]
        public string IdModulo { get; set; }
        public string Modulo { get; set; }
        public int indice { get; set; }

        public int IdTipo { get; set; }

        [ForeignKey("MAE_sistema")]
        public int IdSistema { get; set; }
        public virtual MAE_sistema MAE_sistema { get; set; }

        public virtual List<MAE_opcionesMenu> MAE_opcionesMenu { get; set; }
        public virtual List<MAE_ParametrosModulo> MAE_ParametrosModulo { get; set; }

    }
}