using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LOD_APR.Areas.Admin.Models
{

    [Table("MAE_unidadMedida")]
    public class MAE_Umedida
    {
        [Key]
        public int IdUMedida { get; set; }
        [Required(ErrorMessage = "Dato obligatorio")]
        public string NomUmedida { get; set; }
        [Required(ErrorMessage = "Dato obligatorio")]
        public string UMedida { get; set; }

        [ForeignKey("MAE_tipoMedida")]
        public int IdTipoMedida { get; set; }
        public virtual MAE_tipoMedida MAE_tipoMedida { get; set; }
    }

}