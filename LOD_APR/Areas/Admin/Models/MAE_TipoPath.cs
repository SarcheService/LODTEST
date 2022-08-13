using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace LOD_APR.Areas.Admin.Models
{
    [Table("MAE_TipoPath")]
    public class MAE_TipoPath
    {
        [Key]
        public int IdTipo { get; set; }
        public string TipoPath { get; set; }
    }
}