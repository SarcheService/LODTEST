using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace LOD_APR.Areas.GLOD.Models
{
    [Table("LOD_Carpetas")]
    public class LOD_Carpetas
    {

        public Nullable<int> IdCarpPadre { get; set; }
        public Nullable<int> IdContrato { get; set; }
        [Key]
        public int IdCarpeta { get; set; }

        [Display(Name = "Nombre Carpeta")]
        [Required]
        public string NombreCarpeta { get; set; }
        public DateTime FechaCreacion { get; set; }
        public string UserId { get; set; }
        public bool EsPortafolio { get; set; }
        public virtual List<CON_Contratos> CON_Contratos { get; set; }

        [NotMapped]
        public string Creador { get; set; }
    }
}