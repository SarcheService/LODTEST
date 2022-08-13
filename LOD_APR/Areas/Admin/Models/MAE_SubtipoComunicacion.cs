using LOD_APR.Areas.Admin.Models;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LOD_APR.Areas.Admin.Models
{
    [Table("MAE_SubtipoComunicacion")]
    public class MAE_SubtipoComunicacion
    {
        [Key]
        public int IdTipoSub { get; set; }
        [DisplayName("Nombre Subtipo de Comunicación")]
        [Required(ErrorMessage = "Dato obligatorio")]
        public string Nombre { get; set; }
        [DisplayName("Descripción")]
        [DataType(DataType.MultilineText)]
        public string Descripcion { get; set; }

        [DisplayName("Tipo de Comunicación")]
        [ForeignKey("MAE_TipoComunicacion")]
        public int IdTipoCom { get; set; }
        public virtual MAE_TipoComunicacion MAE_TipoComunicacion { get; set; }

        [DisplayName("Activo")]
        public bool Activo { get; set; }

    }
}