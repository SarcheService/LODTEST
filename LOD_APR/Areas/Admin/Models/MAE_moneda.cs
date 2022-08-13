using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace LOD_APR.Areas.Admin.Models
{
	[Table("MAE_moneda")]
	public class MAE_moneda
	{
		[Key]
		public int IdMoneda { get; set; }
		[Required(ErrorMessage = "Dato obligatorio")]
		public string NomMoneda { get; set; }
		[Required(ErrorMessage = "Dato obligatorio")]
		public string Abreviatura { get; set; }
	}
}