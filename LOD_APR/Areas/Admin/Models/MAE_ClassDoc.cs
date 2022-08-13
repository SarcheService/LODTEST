using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace LOD_APR.Areas.Admin.Models
{
    [Table("MAE_ClassDoc")]
    public class MAE_ClassDoc
    {
        [Key]
        public int IdClassDoc { get; set; }

        [DisplayName("Clasificación 2")]
        [ForeignKey("MAE_ClassTwo")]
        public int IdClassTwo { get; set; }
        public virtual MAE_ClassTwo MAE_ClassTwo { get; set; }

        [DisplayName("Tipo Documento")]
        [ForeignKey("MAE_TipoDocumento")]
        public int IdTipo { get; set; }
        public virtual MAE_TipoDocumento MAE_TipoDocumento { get; set; }

        [DisplayName("Relación con Subtipo de Comunicación")]
        [ForeignKey("MAE_SubtipoComunicacion")]
        public int? IdTipoSub { get; set; }
        public virtual MAE_SubtipoComunicacion MAE_SubtipoComunicacion { get; set; }
        public bool EsLiquidacion { get; set; }
    }
}