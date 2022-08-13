using LOD_APR.Areas.Admin.Models;
using LOD_APR.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;

namespace LOD_APR.Helpers
{
    public class HelperEmails
    {
       
        public class ItemCorreo
        {
            public string Item { get; set; }
            public string Descripcion { get; set; }
        }
        public class UserEmail
        {
            public string UserId { get; set; }
            public string Email { get; set; }
            public string Nombre { get; set; }
        }
        public class Email
        {
            public string Correo { get; set; }
            public string Subject { get; set; }
            public string Body { get; set; }
        }
        public static async Task sendEmail(List<Email> LstEmail)
        {
            LOD_DB db = new LOD_DB();
            foreach (Email mail in LstEmail)
            {
                try
                {
                    MAIL_Envio email = new MAIL_Envio()
                    {
                        FH_IngresoBD = DateTime.Now,
                        Body = mail.Body,
                        EstadoEnvio = false,
                        Email = mail.Correo,
                        Usuario = "Galena Web",
                        Asunto = mail.Subject
                    };

                    db.MAIL_Envio.Add(email);
                    db.SaveChanges();
                }
                catch(Exception ex)
                {

                }
               
            }
            
        }
        static MAE_Empresa DatEmpresa()
        {
            LOD_DB db = new LOD_DB();
            var empresa=db.MAE_Empresa.Find(1);
            return (empresa);
        }
    }
}

public static class ViewExtensions
{

    public static string RenderToString(this PartialViewResult partialView)
    {
        var httpContext = HttpContext.Current;

        if (httpContext == null)
        {
            throw new NotSupportedException("An HTTP context is required to render the partial view to a string");
        }

        var controllerName = httpContext.Request.RequestContext.RouteData.Values["controller"].ToString();

        var controller = (ControllerBase)ControllerBuilder.Current.GetControllerFactory().CreateController(httpContext.Request.RequestContext, controllerName);

        var controllerContext = new ControllerContext(httpContext.Request.RequestContext, controller);

        var view = ViewEngines.Engines.FindPartialView(controllerContext, partialView.ViewName).View;

        var sb = new StringBuilder();

        using (var sw = new StringWriter(sb))
        {
            using (var tw = new HtmlTextWriter(sw))
            {
                view.Render(new ViewContext(controllerContext, view, partialView.ViewData, partialView.TempData, tw), tw);
            }
        }

        return sb.ToString();
    }
}
