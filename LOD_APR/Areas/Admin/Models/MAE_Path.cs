using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace LOD_APR.Areas.Admin.Models
{
    [Table("MAE_Path")]
    public class MAE_Path
    {
        [Key]
        public int IdPath { get; set; }

        [Required(ErrorMessage = "Dato obligatorio")]
        public string Path { get; set; }
        public int Padre { get; set; }

        [ForeignKey("MAE_TipoPath")]
        public int IdTipoPath { get; set; }
        public virtual MAE_TipoPath MAE_TipoPath { get; set; }
        
        //ER 05/01/2020***************************************
        public bool VisibilidadTree { get; set; }
        //***************************************************

        public DateTime? FH_Desactivacion { get; set; }

        public bool GuardarSharepoint { get; set; }

        [NotMapped]
        public bool del { get; set; }
    }
}