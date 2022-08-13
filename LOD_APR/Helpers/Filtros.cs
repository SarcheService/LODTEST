using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LOD_APR.Helpers
{
	public class Filtro_Personal
	{
		public int IdEmpresa { get; set; }
		public string Texto { get; set; }
		public bool Activo { get; set; }
		public bool Inactivo { get; set; }
		public int Faena { get; set; }
		public int Cargo { get; set; }
		public int Unidad { get; set; }
		public string Tags { get; set; }
	}
	public class Filtro_Sujetos
	{
		public string Texto { get; set; }
		public bool Activo { get; set; }
		public bool Inactivo { get; set; }
		public bool Mandante { get; set; }
		public bool Contratista { get; set; }
		public bool Gubernamental { get; set; }
		//public string Tags { get; set; }
	}

	public class Filtro_Usuarios
	{
		public bool Activo { get; set; }
		public bool Inactivo { get; set; }
		public bool Mandante { get; set; }
		public bool Contratista { get; set; }
		public bool Gubernamental { get; set; }
		public int IdSujetoEconomico { get; set; }
	}

	public class Filtro_Documentos
	{

		public int IdDireccion { get; set; }
		public int IdContrato { get; set; }
		public int IdLibroObra { get; set; }
		public int IdTipoLibroObra { get; set; }
		public int IdTipoComunicacion { get; set; }
		public int IdSubtipoComunicacion { get; set; }
		public int IdTipoDoc { get; set; }
		public int IdSujEcon { get; set; }
		public int IdFiscalizador { get; set; }
		public string FechaCreacion { get; set; }
		public bool Formulario { get; set; }
		public bool DocTecnicos { get; set; }
		public bool DocAdmin { get; set; }
		public bool Otros { get; set; }

	}

	public class Filtro_Log
	{
		public int IdContrato { get; set; }
		public int IdLod { get; set; }
		public int IdAnotacion { get; set; }
		public string UserId { get; set; }
		public string FechaLog { get; set; }
	}

}