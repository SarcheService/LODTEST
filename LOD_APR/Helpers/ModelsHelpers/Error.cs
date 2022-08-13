using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LOD_APR.Helpers.ModelsHelpers
{
	public class Status_Error
	{
		public int ErrorCode { get; set; }
		public string ErrorMnsg { get; set; }
		public string Status { get; set; }
		public bool Error { get; set; }
	}
    public class Form_Validation
    {
        public List<string> ErrorMessage { get; set; }
        public string Parametros { get; set; }
        public bool Status { get; set; }
    }
    public class Post_Status
	{
		public string Mnsg { get; set; }
		public bool Error { get; set; }
	}

    public class ErrorView
    {
        public string Titulo { get; set; }
        public string Descripcion { get; set; }


        public static ErrorView ErrorSet(string _Titulo, string _Descripcion)
        {
            ErrorView Error = new ErrorView { Titulo = _Titulo, Descripcion = _Descripcion };
            return Error;
        }
    }
}