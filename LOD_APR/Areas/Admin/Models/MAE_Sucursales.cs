using LOD_APR.Areas.Admin.Models;
using LOD_APR.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace LOD_APR.Areas.Admin.Models
{
	[Table("MAE_Sucursales")]
	public class MAE_Sucursal
	{
		[Key]
		public int IdSucursal { get; set; }

		[Display(Name = "Sucursal")]
		[MaxLength(50, ErrorMessage = "Máx. 50 Caracteres")]
		[Required(ErrorMessage = "Dato obligatorio")]
		public string Sucursal { get; set; }

		[ForeignKey("MAE_sujetoEconomico")]
		[Display(Name = "Sujeto Economico")]
		public int IdSujeto { get; set; }
		public virtual MAE_sujetoEconomico MAE_sujetoEconomico { get; set; }

		[ForeignKey("MAE_ciudad")]
		[Display(Name = "Ciudad")]
		public int IdCiudad { get; set; }
		public virtual MAE_ciudad MAE_ciudad { get; set; }

		[Display(Name = "Encargado/a")]
		[MaxLength(100, ErrorMessage = "Máx. 100 Caracteres")]
		public string Encargado { get; set; }

		[Display(Name = "Dirección")]
		[MaxLength(100, ErrorMessage = "Máx. 100 Caracteres")]
		[Required(ErrorMessage = "Dato obligatorio")]
		public string Direccion { get; set; }

		[Display(Name = "Teléfono")]
		[MaxLength(13, ErrorMessage = "Máx. 13 Caracteres")]
		public string Telefono { get; set; }

		[Display(Name = "E-mail")]
		[EmailAddress]
		[StringLength(50, ErrorMessage = "Máx 50 caracteres")]
		public string Email { get; set; }
		public bool EsCentral { get; set; }
		[ForeignKey("MAE_DireccionesMOP")]
		public int? IdDireccion { get; set; }
		public virtual MAE_DireccionesMOP MAE_DireccionesMOP {get; set;}
		public virtual List<ApplicationUser> Usuarios { get; set; }


	}
}