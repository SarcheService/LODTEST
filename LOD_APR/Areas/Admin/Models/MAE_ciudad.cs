using LOD_APR.ModelsView;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LOD_APR.Areas.Admin.Models
{
    [Table("MAE_ciudad")]
    public class MAE_ciudad
    {
        [Key]
		[Display(Name = "Ciudad")]
		public int IdCiudad { get; set; }
        [Required(ErrorMessage = "Dato obligatorio")]
        public string Ciudad { get; set; }
        public string ZonaHoraria { get; set; }
        public double Latitud { get; set; }
        public double Longitud { get; set; }

        [ForeignKey("MAE_region")]
        public int? IdRegion { get; set; }
        public virtual MAE_region MAE_region { get; set; }

        [NotMapped]
        public SelectDropdown SelectDropdown
        {
            get
            {
                SelectDropdown d = new SelectDropdown()
                {
                    value = this.IdCiudad.ToString(),
                    text = this.Ciudad,
                    image = null,
                    description = "Región de " + this.MAE_region.Region,
                    shortext = null,
                    dataletters = null
                };

                return d;
            }

        }

    }
}