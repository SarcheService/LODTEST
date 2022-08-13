using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using LOD_APR.Models;

namespace LOD_APR.Areas.Admin.Models
{
    [Table("MAE_CodSubCom")]
    public class MAE_CodSubCom
    {
        [Key]
        public int IdControl { get; set; }

        [DisplayName("Subtipo Comunicacíón")]
        [ForeignKey("MAE_SubtipoComunicacion")]
        public int IdTipoSub{ get; set; }
        public virtual MAE_SubtipoComunicacion MAE_SubtipoComunicacion { get; set; }

        [DisplayName("Tipo de Documento")]
        [Required(ErrorMessage = "Dato obligatorio")]
        [ForeignKey("MAE_TipoDocumento")]
        public int IdTipo { get; set; }
        public virtual MAE_TipoDocumento MAE_TipoDocumento { get; set; }

        //[DisplayName("Carpeta")]
        //[ForeignKey("MAE_Path")]
        //public int? IdPath { get; set; }
        //public virtual MAE_Path MAE_Path { get; set; }

        public bool Activo { get; set; }
        public bool Obligatorio { get; set; }

        [NotMapped]
        public int IdTipoCom { get; set; }
        [NotMapped]
        public virtual MAE_TipoComunicacion MAE_TipoComunicacion { get; set; }
        [NotMapped]
        public MAE_ClassDoc MAE_ClassDoc { get; set; }
    }
}