using LOD_APR.Helpers;
using LOD_APR.Helpers.Validadores;
using LOD_APR.ModelsView;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web;

namespace LOD_APR.Areas.Admin.Models
{
    [Table("MAE_sujetoEconomico")]
    public class MAE_sujetoEconomico: IValidatableObject
    {

		[Key]
        public int IdSujEcon { get; set; }
        
        //Falta Formato de Rut
        [StringLength(13, ErrorMessage = "Máx. 13 caracteres")]
        [Required(ErrorMessage = "Dato obligatorio")]
		[RutValido]
		public string Rut { get; set; }

        [Display(Name = "Razón Social")]
        [StringLength(200, ErrorMessage = "Máx 200 caracteres")]
        [Required(ErrorMessage = "Dato obligatorio")]
        public string RazonSocial { get; set; }

		[Display(Name = "Nombre Fantasía")]
		[StringLength(100, ErrorMessage = "Máx 100 caracteres")]
        [Required(ErrorMessage = "Dato obligatorio")]
        public string NomFantasia { get; set; }

		[StringLength(300, ErrorMessage = "Máx 300 caracteres")]
		[Required(ErrorMessage = "Dato obligatorio")]
		public string Giro { get; set; }

		[Display(Name = "Dirección")]
		[StringLength(300, ErrorMessage = "Máx 300 caracteres")]
		[Required(ErrorMessage = "Dato obligatorio")]
		public string Direccion { get; set; }

		[Display(Name = "Teléfono")]
		[StringLength(13, ErrorMessage = "Máx 13 caracteres")]
		public string Telefono { get; set; }

		[Display(Name = "E-mail")]
		[EmailAddress]
		[StringLength(50, ErrorMessage = "Máx 50 caracteres")]
		public string email { get; set; }

		[Display(Name = "Sitio Web")]
		[StringLength(50, ErrorMessage = "Máx 50 caracteres")]
		public string web { get; set; }

		[StringLength(250, ErrorMessage = "Máx 250 caracteres")]
		public string UrlFacebook { get; set; }
		[StringLength(250, ErrorMessage = "Máx 250 caracteres")]
		public string UrlTwitter { get; set; }
		[StringLength(250, ErrorMessage = "Máx 250 caracteres")]
		public string UrlLinkedin { get; set; }

		[ForeignKey("MAE_ciudad")]
		[Required(ErrorMessage = "Dato obligatorio")]
		public int? IdCiudad { get; set; }
		public virtual MAE_ciudad MAE_ciudad { get; set; }
        
		[Display(Name = "Es Proveedor")]
		public bool EsMandante { get; set; }

		[Display(Name = "Es Contratista")]
		public bool EsContratista { get; set; }

        [Display(Name = "Es Organismo de Gobierno")]
        public bool EsGubernamental { get; set; }

        public DateTime FechaCreacion { get; set; }
		public string RutaImagen { get; set; }
		public bool Activo { get; set; }

		public string DataLetters { get; set; }

		[NotMapped]
		public HttpPostedFile fileImage { get; set; }

        [DisplayName("Ciudad")]
        [NotMapped]
        public string GetCiudad
        {
            get
            {
                return (MAE_ciudad != null) ? MAE_ciudad.Ciudad : "-";
            }
        }

        [NotMapped]
        public string Iniciales
        {
            get
            {
                return Data_Letters.ImageLetter(this.RazonSocial);
            }
        }
        [NotMapped]
        public SelectDropdown SelectDropdown
        {
            get
            {
                SelectDropdown d = new SelectDropdown()
                {
                    value = this.IdSujEcon.ToString(),
                    text = this.RazonSocial,
                    image = (String.IsNullOrEmpty(this.RutaImagen) ? null : "/Images/Sujetos/" + this.RutaImagen),
                    description = this.GetCiudad,
                    shortext = this.Iniciales,
                    dataletters = this.DataLetters
                };

                return d;
            }
        }
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            //LOD_DB db = new LOD_DB();
            var errores = new List<ValidationResult>();
            //var existeRut = db.MAE_sujetoEconomico.Where(s => s.Rut == Rut).FirstOrDefault();
            //if (existeRut != null) {
            //	errores.Add(new ValidationResult("El Rut ya se encuentra ingresado",new string[] { "Rut" }));
            //}
            return errores;
        }

        public virtual List<MAE_Sucursal> Sucursales { get; set; }
    }
}


  


  
