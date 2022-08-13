using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace LOD_APR.Helpers.Validadores
{
	static class Rut
	{

		/// <summary>
		/// Metodo de validación de rut con digito verificador
		/// dentro de la cadena
		/// </summary>
		/// <param name="rut">string</param>
		/// <returns>booleano</returns>
		public static bool ValidaRut(string rut)
		{
			rut = rut.Replace(".", "").ToUpper();
			Regex expresion = new Regex("^([0-9]+-[0-9K])$");
			string dv = rut.Substring(rut.Length - 1, 1);
			if (!expresion.IsMatch(rut))
			{
				return false;
			}
			char[] charCorte = { '-' };
			string[] rutTemp = rut.Split(charCorte);
			if (dv != Digito(int.Parse(rutTemp[0])))
			{
				return false;
			}
			return true;
		}


		/// <summary>
		/// Método que valida el rut con el digito verificador
		/// por separado
		/// </summary>
		/// <param name="rut">integer</param>
		/// <param name="dv">char</param>
		/// <returns>booleano</returns>
		public static bool ValidaRut(string rut, string dv)
		{
			return ValidaRut(rut + "-" + dv);
		}

		/// <summary>
		/// método que calcula el digito verificador a partir
		/// de la mantisa del rut
		/// </summary>
		/// <param name="rut"></param>
		/// <returns></returns>
		public static string Digito(int rut)
		{
			int suma = 0;
			int multiplicador = 1;
			while (rut != 0)
			{
				multiplicador++;
				if (multiplicador == 8)
					multiplicador = 2;
				suma += (rut % 10) * multiplicador;
				rut = rut / 10;
			}
			suma = 11 - (suma % 11);
			if (suma == 11)
			{
				return "0";
			}
			else if (suma == 10)
			{
				return "K";
			}
			else
			{
				return suma.ToString();
			}
		}
	}
	class RutValidoAttribute : ValidationAttribute {
		private string rut;
		public RutValidoAttribute()
		: base("Rut Inválido"){
			
		}
		protected override ValidationResult IsValid(object value, ValidationContext validationContext)
		{
			if (value != null) {
				if (!Rut.ValidaRut(value.ToString())){
					var error = FormatErrorMessage(validationContext.DisplayName);
					return new ValidationResult(error);
				}
			}
			return ValidationResult.Success;
		}
	}
	class DateFormatValidation : ValidationAttribute
	{
		private string format = "dd-MM-yyyy";
		public DateFormatValidation()
		: base("Fecha Inválida")
		{

		}
		protected override ValidationResult IsValid(object value, ValidationContext validationContext)
		{
			DateTime date;
			if (value != null)
			{
				string fecha = Convert.ToDateTime(value).ToShortDateString();
				bool parsed = DateTime.TryParseExact(fecha, format, System.Globalization.CultureInfo.InvariantCulture, DateTimeStyles.None, out date);
				if (!parsed)
				{
					var error = FormatErrorMessage(validationContext.DisplayName);
					return new ValidationResult(error);
				}
			}
			return ValidationResult.Success;
		}
	}
	[AttributeUsage(AttributeTargets.Property)]
	public sealed class AttachmentAttribute : ValidationAttribute
	{
		protected override ValidationResult IsValid(object value,
		  ValidationContext validationContext)
		{
			HttpPostedFileBase file = value as HttpPostedFileBase;

			// The file is required.
			if (file == null)
			{
				return new ValidationResult("Por Favor seleccione un archivo válido!");
			}

			// The meximum allowed file size is 10MB.
			int maxFileSize = 10;
			if (file.ContentLength > 1024 * 1024 * maxFileSize) //* 10
			{
				return new ValidationResult(String.Format("El Archivo Ingresado es muy grande (máximo {0}Mb)", maxFileSize));
			}

			// Only PDF can be uploaded.
			string ext = Path.GetExtension(file.FileName);
			if (String.IsNullOrEmpty(ext) ||
			   !ext.Equals(".pdf", StringComparison.OrdinalIgnoreCase))
			{
				return new ValidationResult("El tipo de archivo no está permitido!");
			}

			// Everything OK.
			return ValidationResult.Success;
		}
	}
}