using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;

namespace LOD_APR.Helpers
{
	public static class Data_Letters
	{
		public static string getRandomDataLetter() {
			string[] colors = new string[] { "data-letters-galena", "data-letters-violet", "data-letters-yellow", "data-letters-orange", "data-letters-green", "data-letters-red", "data-letters-red-deep", "data-letters-blue", "data-letters-navy-blue" };
			Random rnd = new Random();
			int indice = rnd.Next(0,9);
			return colors[indice];
		}
        public static string ImageLetter(string nombre)
        {
            string letras;
            string[] palabras = nombre.Split(' ');
            if (palabras.Length > 1)
            {
                letras = palabras[0][0].ToString() + palabras[1][0].ToString();
            }
            else
            {
                letras = palabras[0][0].ToString();
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
    }
}