using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace LOD_APR.Areas.Admin.Models
{

    [Table("MAE_tipoMedida")]
    public class MAE_tipoMedida
    {
        [Key]
        public int IdTipoMedida { get; set; }
        public string Tipo { get; set; }
        public string Variable { get; set; }
    }

}