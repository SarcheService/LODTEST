using LOD_APR.Areas.GLOD.Models;
using LOD_APR.Helpers;
using LOD_APR.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using System.Web.Mvc.Html;

namespace LOD_APR
{
    [CustomAuthorize]
    public static class HMTLHelperExtensions
    {

        [CustomAuthorize]
        public static string IsSelected(this HtmlHelper html, string controller = null, string action = null)
        {
            string cssClass = "active";
            string currentAction = (string)html.ViewContext.RouteData.Values["action"];
            string currentController = (string)html.ViewContext.RouteData.Values["controller"];

            if (String.IsNullOrEmpty(controller))
                controller = currentController;

            if (String.IsNullOrEmpty(action))
                action = currentAction;

            return controller == currentController && action == currentAction ?
                cssClass : String.Empty;
        }
		public static string IsSelected(this HtmlHelper html, string[] controller = null, string action = null)
		{
			string cssClass = "active";
			string currentAction = (string)html.ViewContext.RouteData.Values["action"];
			string currentController = (string)html.ViewContext.RouteData.Values["controller"];
			string control_final = string.Empty;

			if (controller.Length == 0)
			{
				control_final = currentController;
			}
			else {
				foreach (string c in controller) {
					if(c == currentController)
						control_final = currentController;
				}
			}

			if (String.IsNullOrEmpty(action))
				action = currentAction;

			return control_final == currentController && action == currentAction ?
				cssClass : String.Empty;

		}
		public static MvcHtmlString RawActionLink(this AjaxHelper ajaxHelper, string linkText, string actionName, string controllerName, object routeValues, AjaxOptions ajaxOptions, object htmlAttributes)
		{
			var repID = Guid.NewGuid().ToString();
			var lnk = ajaxHelper.ActionLink(repID, actionName, controllerName, routeValues, ajaxOptions, htmlAttributes);
			return MvcHtmlString.Create(lnk.ToString().Replace(repID, linkText));
		}
		public static string PageClass(this HtmlHelper html)
        {
            string currentAction = (string)html.ViewContext.RouteData.Values["action"];
            return currentAction;
        }
		public static string ImageLetter(this HtmlHelper helper, string nombre)
		{
			string letras;
            nombre = nombre.Trim();
			string[] palabras = nombre.Split(' ');
            bool vacio = false;
            foreach (var palabra in palabras)
            {
                if(palabra=="")
                {
                    vacio = true;
                }
            }
          
			if (palabras.Length > 1)
			{
                if (vacio)
                {
                    letras = palabras[0][0].ToString() + palabras[2][0].ToString();
                }
                else
                {
                    letras = palabras[0][0].ToString() + palabras[1][0].ToString();
                }
			}
			else
			{
                /*RH 15-04-2019 [*MODIFICAR*] */

                //letras = palabras[0][0].ToString();
                letras = palabras[0].ToString();

                /*FIN RH 15-04-2019*/
            }

            return letras.ToUpper();

		}
		public static string ImageLetter(this HtmlHelper helper, string nombres, string apellidos)
		{
			string letras;
            nombres = nombres.Trim();
            apellidos = apellidos.Trim();
            string[] n = nombres.Split(' ');
			string[] a = apellidos.Split(' ');


			if (a.Length >= 1)
			{
				letras = n[0][0].ToString() + a[0][0].ToString();
			}
			else
			{
				letras = n[0][0].ToString();
			}

			return letras.ToUpper();

		}
		public static string ImageLetter(string nombre)
		{
			string letras;
			string[] palabras = nombre.Split(' ');


            try
            {
                if (palabras.Length > 1)
                {
                    letras = palabras[0][0].ToString() + palabras[1][0].ToString();
                }
                else
                {
                    letras = palabras[0][0].ToString();
                }
            }
            catch (Exception)
            {
                return "-";
            }
		

			return letras.ToUpper();

		}
		public static string ImageLetter(string nombres, string apellidos)
		{
			string letras;
			string[] n = nombres.Split(' ');
			string[] a = apellidos.Split(' ');


			if (a.Length >= 1)
			{
				letras = n[0][0].ToString() + a[0][0].ToString();
			}
			else
			{
				letras = n[0][0].ToString();
			}

			return letras.ToUpper();

		}

        public static string EnumeracionRomanos(this HtmlHelper helper, int N)
        {
            String[] Unidad = { "", "I", "II", "III", "IV", "V", "VI", "VII", "VIII", "IX" };
            String[] Decena = { "", "X", "XX", "XXX", "XL", "L", "LX", "LXX", "LXXX", "XC" };
            String[] Centena = { "", "C", "CC", "CCC", "CD", "D", "DC", "DCC", "DCCC", "CM" };
            int u = N % 10;
            int d = (N / 10) % 10;
            int c = N / 100;
            if (N >= 100)
            {
                return(Centena[c] + Decena[d] + Unidad[u]);
            }
            else
            {
                if(N >= 10)
                {
                    return(Decena[d] + Unidad[u]);
                }
                else
                {
                    return(Unidad[N]);
                }
            }
         }

        public static string EnumeracionLetras(this HtmlHelper helper, int N)
        {
            String[] Letras = {"","a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n","ñ", "o", "p", "q", "r", "s", "t", "u", "v","w", "x", "y","z" };
            int L = N % 28;
            int D = N / 28;
            return (Letras[D]+Letras[L]);
        }
        public static string EnumeracionLetras(int N)
        {
            String[] Letras = { "", "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "ñ", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z" };
            int L = N % 28;
            int D = N / 28;
            return (Letras[D] + Letras[L]);
        }
        //SE incorpora 30-04-2018
        public static bool ValidaPermisos(this HtmlHelper html, string IdOpcion)
        {

            ///ACA_Sesion se = new ACA_Sesion();
            clsSession se = new clsSession();
            List<string> p = (List<string>)se.get("List_permisos");
            if (p != null)
            {
                if(p.Contains("MDTECH"))
                {
                    return true;
                }
                //Si es por grupo
                if (Convert.ToInt32(se.get("TipoPerfil")) == 1 && IdOpcion.Contains(";"))
                {
                    return p.Contains(IdOpcion);
                }
                else if (Convert.ToInt32(se.get("TipoPerfil")) == 1)
                {
                    return (0 < p.Where(x => x.Split(';')[0] == IdOpcion).Count());
                }
                else//por empresa
                {
                    if (IdOpcion.Contains(";"))
                    {
                        IdOpcion = IdOpcion.Split(';')[0];
                    }
                    return p.Contains(IdOpcion+";0");
                }
            }
            else
            {
                return false;
            }
        }

        public static bool ValidaRoles(this HtmlHelper html, int IdLod, int tipo, string UserId)
        {
            LOD_DB db = new LOD_DB();
            LOD_UsuariosLod userLod = db.LOD_UsuariosLod.Where(x => x.IdLod == IdLod && x.UserId == UserId).FirstOrDefault();
            if (userLod != null)
            {
                LOD_PermisosRolesContrato permisos = db.LOD_PermisosRolesContrato.Where(x => x.IdRCContrato == userLod.IdRCContrato && x.IdLod == userLod.IdLod).FirstOrDefault();
                bool respuesta = false;
                if (permisos != null)
                {

                    switch (tipo)
                    {
                        case 1:
                            if (permisos.Lectura)
                                respuesta = true;
                            break;
                        case 2:
                            if (permisos.Escritura)
                                respuesta = true;
                            break;
                        case 3:
                            if (permisos.FirmaGob)
                                respuesta = true;
                            break;
                        case 4:
                            if (permisos.FirmaFea)
                                respuesta = true;
                            break;
                        case 5:
                            if (permisos.FirmaSimple)
                                respuesta = true;
                            break;
                        case 6:
                            if (permisos.FirmaSimple || permisos.FirmaGob || permisos.FirmaFea)
                                respuesta = true;
                            break;

                        default:
                            break;
                    }

                    return respuesta;
                }
                else
                {
                    return respuesta;
                }

                
            }
            else
            {
                return false;
            }
        }



        public static bool ValidaPermisosAPP(this HtmlHelper html, string IdApp)
        {
            ///ACA_Sesion se = new ACA_Sesion();
            clsSession se = new clsSession();
            List<string> permisos = (List<string>)se.get("List_permisos");
            if (permisos.Contains("MDTECH"))
            {
                return true;
            }
            foreach (var per in permisos)
            {
                if (Convert.ToInt16(per.Substring(0, 3)) == Convert.ToInt16(IdApp))
                {
                    return true;
                }
            };
            return false;
        }

        public static int AccesoMenuAPP(this HtmlHelper html)
        {
            LOD_DB db = new LOD_DB();
            var IdsSistemas = db.MAE_sistema.Where(x=>x.Activo==true).Select(x=>x.IdSistema).ToList();
            int contar = 0;

            clsSession se = new clsSession();
            List<string> permisos = (List<string>)se.get("List_permisos");
            //if (permisos.Contains("MDTECH"))
            //{
            //    return IdsSistemas.Count;
            //}
            foreach (var sistema in IdsSistemas)
            {
                if (permisos != null)
                {
                    foreach (var per in permisos)
                    {
                        if (Convert.ToInt16(per.Substring(0, 3)) == Convert.ToInt16(sistema))
                        {
                            contar++;
                            break;
                        }
                    }
                }
            };
            return contar;
        }

       
    }
}
