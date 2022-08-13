using System;
using LOD_APR.Areas.ASP.Models;
using LOD_APR.Areas.EVA.Models;
using LOD_APR.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static LOD_APR.Models.Auxiliares;
using System.Data;
using System.Globalization;
using System.Data.Entity;
using static LOD_APR.Helpers.HelperEmails;
using System.Configuration;
using Microsoft.ReportingServices.ReportProcessing.ReportObjectModel;
using System.IO;
using System.Web;
using System.Web.Mvc;


namespace LOD_APR.Helpers
{
 
    public class Evaluaciones
    {       
        //Prepara el email a los receptores
        //Caso 1.-programación, 2.-Vencimiento
        public static async Task EmailEval(int Caso, string objeto, string Cod, string Nombre, DateTime FechaProg,string EmailResponsable,int IdEmpresa)
        {
            //Formateamos el Email a Mandar
            //Envío Detalle
            List<ItemCorreo> items = new List<ItemCorreo>();
            if(Cod != null && Cod != "")
            {
                Nombre = Cod + "- " + Nombre;
            }
            items.Add(new ItemCorreo() { Item = objeto, Descripcion = Nombre });
            items.Add(new ItemCorreo() { Item = "Fecha de Programación        ", Descripcion = FechaProg.ToShortDateString() });

            //Obtengo el Cuerpo del Correo
            LOD_DB db = new LOD_DB();
            string ruta = db.MAE_Empresa.Find(IdEmpresa).LogoData;
            string bodyFormateado = formatearCorreo( Caso , items, ruta);

            //A quien Envío el Correo
            List<Email> Correo = new List<Email>();

            if (EmailResponsable != null || EmailResponsable != string.Empty)
            {
                Correo.Add(new Email { Correo = EmailResponsable, Subject = "Evaluación de Contratistas", Body = bodyFormateado });
            }
            await sendEmail(Correo);
        }
        public static string formatearCorreo(int tipo, List<ItemCorreo> items, string ruta)
        {
            string rutaImg = HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Authority + HttpContext.Current.Request.ApplicationPath.TrimEnd('/') + ruta.Substring(1);
            string titulo = "Evaluación de Contratistas";
            string encabezado = string.Empty;
            string comentario = "Esta notificación le ha llega a usted por ser el ITO responsable de evaluar.";

            switch (tipo)
            {
                case 1: //Progamacion de Evaluación
                    encabezado = "Se informa que se ha programado la siguiente Evaluación:";
                    break;
                case 2: //Vencimiento de Plazo para Evaluar
                    encabezado = "Se informa que se ha vencido el plazo de evaluación de:";
                    break;
            }

            try
            {
                string html = File.ReadAllText(HttpContext.Current.Server.MapPath("/Helpers/EmailEvaluaciones.html"));
                string htmlFormateado = String.Format(html, rutaImg, titulo, encabezado,items[0].Item.ToString(),items[0].Descripcion.ToString(), items[1].Item.ToString(), items[1].Descripcion.ToString(),comentario);
                return htmlFormateado;
            }
            catch
            {  
                return "";
            }

        }

        //***********************************************************************************************************
        public static List<ComboBoxEstandar> ListTipoParam()
        {
            List<ComboBoxEstandar> ListTipoParam = new List<ComboBoxEstandar>();
            ListTipoParam.Add(new ComboBoxEstandar { Id = "1", Value = "Evaluación Directa" });
            ListTipoParam.Add(new ComboBoxEstandar { Id = "2", Value = "Pregunta Abierta" });
            //ListTipoParam.Add(new ComboBoxEstandar { Id = "3", Value = "Selección Múltiple" });
            ListTipoParam.Add(new ComboBoxEstandar { Id = "4", Value = "Selección Múltiple" });
            ListTipoParam.Add(new ComboBoxEstandar { Id = "5", Value = "Selección Única" });
            return (ListTipoParam);
        }
        //se usa en el Reporte
        public static string EnumeracionRomanos(int N)
        {
            String[] Unidad = { "", "I", "II", "III", "IV", "V", "VI", "VII", "VIII", "IX" };
            String[] Decena = { "", "X", "XX", "XXX", "XL", "L", "LX", "LXX", "LXXX", "XC" };
            String[] Centena = { "", "C", "CC", "CCC", "CD", "D", "DC", "DCC", "DCCC", "CM" };
            int u = N % 10;
            int d = (N / 10) % 10;
            int c = N / 100;
            if (N >= 100)
            {
                return (Centena[c] + Decena[d] + Unidad[u]);
            }
            else
            {
                if (N >= 10)
                {
                    return (Decena[d] + Unidad[u]);
                }
                else
                {
                    return (Unidad[N]);
                }
            }
        }
        
    }
}
