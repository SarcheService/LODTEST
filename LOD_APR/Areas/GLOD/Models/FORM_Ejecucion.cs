using LOD_APR.Areas.Admin.Models;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LOD_APR.Areas.GLOD.Models
{
    [Table("FORM_Ejecucion")]
    public class FORM_Ejecucion
    {
        [Key]
        [DisplayName("ID Ejecución")]
        public string IdEjecucion { get; set; }

        [ForeignKey("FORM_Formularios")]
        public string IdForm { get; set; }
        public virtual FORM_Formularios FORM_Formularios { get; set; }

        [DisplayName("Folio")]
        public int Folio { get; set; }

        [DisplayName("F. Ejecución")]
        public DateTime FH_Ejecucion { get; set; }

        [DisplayName("Usuario")]
        public string Usuario { get; set; }

        //[ForeignKey("MAE_personal")]
        //[DisplayName("Responsable")]
        //public int? IdPersonal { get; set; }
        //public virtual MAE_personal MAE_personal { get; set; }

        [DisplayName("FormData")]
        public string FormData { get; set; }

        [NotMapped]
        [DisplayName("Folio")]
        public string FolioPad { get { return this.Folio.ToString().PadLeft(8, '0'); } set { } }

    }
}