using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LOD_APR.Areas.Admin.Models
{
    [Table("MAE_ParametrosModulo")]
    public class MAE_ParametrosModulo
    {
        [Key]
        public int IdParametro { get; set; }

        [ForeignKey("MAE_modulos")]
        public string IdModulo { get; set; }
        public virtual MAE_modulos MAE_modulos { get; set; }

        public string Parametro { get; set; }
        public string Valor { get; set; }
    }
}