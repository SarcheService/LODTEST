using LOD_APR.Areas.Admin.Models;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LOD_APR.Areas.GLOD.Models
{
    [Table("FORM_InformesItemsData")]
    public class FORM_InformesItemsData
    {
        [Key]
        [DisplayName("ID Campo")]
        public int IdCampo { get; set; }

        [ForeignKey("FORM_InformesItems")]
        public int IdItem { get; set; }
        public virtual FORM_InformesItems FORM_InformesItems { get; set; }

        [ForeignKey("FORM_FormPreguntas")]
        [DisplayName("Id Pregunta")]
        public string IdPregunta { get; set; }
        public virtual FORM_FormPreguntas FORM_FormPreguntas { get; set; }

        [DisplayName("Pregunta")]
        public string Pregunta { get; set; }

        [DisplayName("Respuesta")]
        public string Respuesta { get; set; }

        [NotMapped]
        public string DataType {
            get {
                string type = string.Empty;
                if(this.FORM_FormPreguntas.TipoParam == 3)
                {
                    type = "System.Int32";
                }else if (this.FORM_FormPreguntas.TipoParam==4)
                {
                    type = "System.Decimal";
                }
                else if (this.FORM_FormPreguntas.TipoParam == 9 || this.FORM_FormPreguntas.TipoParam == 902)
                {
                    type = "System.DateTime";
                }
                else
                {
                    type = "System.String";
                }
                return type;
            }
        }
    }
}