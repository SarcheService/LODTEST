using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace LOD_APR.Areas.Admin.Models
{
    [Table("MAE_TipoComunicacion")]
    public class MAE_TipoComunicacion
    {
        [Key]
        public int IdTipoCom { get; set; }
        [DisplayName("Nombres")]
        [Required(ErrorMessage = "Dato obligatorio")]
        public string Nombre { get; set; }
        [DisplayName("Descripción")]
        [DataType(DataType.MultilineText)]
        public string Descripcion { get; set; }
        [DisplayName("Activo")]
        public bool Activo { get; set; }

        [DisplayName("Tipo de Libro de Obra")]
        [ForeignKey("MAE_TipoLOD")]
        public int IdTipoLod { get; set; }
        public virtual MAE_TipoLOD MAE_TipoLOD { get; set; }
        
    }
}