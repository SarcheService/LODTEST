using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using LOD_APR.Helpers;
using LOD_APR.ModelsView;

namespace LOD_APR.Areas.Admin.Models
{
	[Table("MAE_Contactos")]
	public class MAE_Contactos
	{
		[Key]
		public int IdContacto { get; set; }

		[Display(Name = "Nombres")]
		[MaxLength(100, ErrorMessage = "Máx. 100 Caracteres")]
		[Required(ErrorMessage = "Dato obligatorio")]
		public string Nombre { get; set; }

		[Display(Name = "Cargo")]
		[MaxLength(50, ErrorMessage = "Máx. 50 Caracteres")]
		[Required(ErrorMessage = "Dato obligatorio")]
		public string CargoContacto { get; set; }

		[EmailAddress(ErrorMessage ="E-mail inválido")]
		[MaxLength(100,ErrorMessage = "Máx. 100 Caracteres")]
		[Display(Name = "E-mail")]
		public string Email { get; set; }

		[MaxLength(12, ErrorMessage = "Máx. 12 Caracteres")]
		[Display(Name = "Teléfono")]
		public string Telefono { get; set; }

		[MaxLength(12, ErrorMessage = "Máx. 12 Caracteres")]
		[Display(Name = "Celular")]
		public string Movil { get; set; }

		[MaxLength(250, ErrorMessage = "Máx. 250 Caracteres")]
		public string RutaImagen { get; set; }

		[MaxLength(10, ErrorMessage = "Máx. 10 Caracteres")]
		[Display(Name = "Anexo")]
		public string AnexoEmpresa { get; set; }

		[MaxLength(250, ErrorMessage = "Máx. 250 Caracteres")]
		[Display(Name = "Perfil Facebook")]
		public string UrlFacebook { get; set; }

		[MaxLength(250, ErrorMessage = "Máx. 250 Caracteres")]
		[Display(Name = "ECuenta Twitter")]
		public string UrlTwitter { get; set; }

		[MaxLength(100, ErrorMessage = "Máx. 100 Caracteres")]
		[Display(Name = "Perfil Linkedin")]
		public string UrlLinkedin { get; set; }

        [ForeignKey("MAE_sujetoEconomico")]
        [Display(Name = "Sujeto Economico")]
        public int IdSujeto { get; set; }
        public virtual MAE_sujetoEconomico MAE_sujetoEconomico { get; set; }

		[NotMapped]
		public HttpPostedFile fileImage { get; set; }

		public DateTime FechaCreacion { get; set; }

		public bool Activo { get; set; }

        [NotMapped]
        public string Iniciales
        {
            get
            {
                return Data_Letters.ImageLetter(this.Nombre);
            }
        }

        public string DataLetters { get; set; }

        [NotMapped]
        public SelectDropdown SelectDropdown
        {
            get
            {
                SelectDropdown d = new SelectDropdown()
                {
                    value = this.IdContacto.ToString(),
                    text = this.Nombre,
                    image = (String.IsNullOrEmpty(this.RutaImagen) ? null : "/Images/Contactos/" + this.RutaImagen),
                    description = this.MAE_sujetoEconomico.RazonSocial + " | " + this.CargoContacto,
                    shortext = this.Iniciales,
                    dataletters = this.DataLetters
                };

                return d;
            }
        }

    }
}