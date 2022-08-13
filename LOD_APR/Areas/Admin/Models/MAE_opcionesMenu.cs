using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LOD_APR.Areas.Admin.Models
{
    [Table("MAE_opcionesMenu")]
    public class MAE_opcionesMenu
    {
        [Key]
        public string IdOpcion { get; set; }
        public string Opcion { get; set; }
        public int Indice { get; set; }

        [ForeignKey("MAE_modulos")]
        public string IdModulo { get; set; }
        public virtual MAE_modulos MAE_modulos { get; set; }

        [NotMapped]
        public string NumObj { get; set; }
        //jg 02-01-2020

    }
    //jg 30-12-2019
    public class DropDown
    {
        public string id { get; set; }
        public string label { get; set; }
    }

}