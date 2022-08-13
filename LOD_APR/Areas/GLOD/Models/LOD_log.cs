using LOD_APR.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace LOD_APR.Areas.GLOD.Models
{
    [Table("LOD_log")]
    public class LOD_log
    {
        [Key]
        public int IdLog { get; set; }
        public string Objeto { get; set; }
        [DisplayName("Anotación")]
        public int IdObjeto { get; set; }
        public DateTime FechaLog { get; set; }

        [DisplayName("Usuario")]
        [ForeignKey("ApplicationUser")]
        public string UserId { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }
        public string Accion { get; set; }
        public string Campo { get; set; }
        public string ValorAnterior { get; set; }
        public string ValorActualizado { get; set; }
        [NotMapped]
        public string NombUser { get; set; }

        [NotMapped]
        public LOD_Anotaciones anotacion { get; set; }

    }
}