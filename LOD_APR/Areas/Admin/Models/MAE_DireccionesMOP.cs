using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace LOD_APR.Areas.Admin.Models
{
    [Table("MAE_DireccionesMOP")]
    public class MAE_DireccionesMOP
    {
        [Key]
        public int IdDireccion { get; set; }
        [DisplayName("Nombre Dirección")]
        public string NombreDireccion { get; set; }
        [DisplayName("Descripción Dirección")]
        [DataType(DataType.MultilineText)]
        public string DescripcionDireccion { get; set; }
    }
}