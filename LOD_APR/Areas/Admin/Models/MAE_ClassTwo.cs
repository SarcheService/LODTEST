using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace LOD_APR.Areas.Admin.Models
{
    [Table("MAE_ClassTwo")]
    public class MAE_ClassTwo
    {
        [Key]
        public int IdClassTwo { get; set; }

        [DisplayName("Nombres")]
        [Required(ErrorMessage = "Dato obligatorio")]
        public string Nombre { get; set; }
        [DisplayName("Descripción")]
        [DataType(DataType.MultilineText)]
        public string Descripcion { get; set; }
        [DisplayName("Activo")]
        public bool Activo { get; set; }

        [DisplayName("Clasificación 1")]
        [ForeignKey("MAE_ClassOne")]
        public int IdClassOne { get; set; }
        public virtual MAE_ClassOne MAE_ClassOne { get; set; }
    }
}