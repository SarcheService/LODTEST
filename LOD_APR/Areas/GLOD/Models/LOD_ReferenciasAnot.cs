using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace LOD_APR.Areas.GLOD.Models
{
    [Table("LOD_ReferenciasAnot")]
    public class LOD_ReferenciasAnot
    {
        [Key]
        public int IdRefAnot { get; set; }

        [ForeignKey("AnotacionOrigen")]
        public int IdAnotacion { get; set; }
        public virtual LOD_Anotaciones AnotacionOrigen { get; set; }

        [ForeignKey("AnotacionReferencia")]
        public int IdAnontacionRef { get; set; }
        public virtual LOD_Anotaciones AnotacionReferencia { get; set; }

        public bool EsRepuesta { get; set; }

        public string Observacion { get; set; }
    }
}