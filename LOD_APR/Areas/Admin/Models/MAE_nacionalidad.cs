using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace LOD_APR.Areas.Admin.Models
{
	[Table("MAE_nacionalidad")]
	public class MAE_nacionalidad
	{
		[Key]
		[Display(Name = "Nacionalidad")]
		public int IdNacionalidad { get; set; }
		[Required(ErrorMessage = "Dato obligatorio")]
		public string Nacionalidad { get; set; }
	}
}