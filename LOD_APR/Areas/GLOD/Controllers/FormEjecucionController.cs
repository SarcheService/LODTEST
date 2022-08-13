using LOD_APR.Areas.Admin.Helpers;
using LOD_APR.Areas.Admin.Models;
using LOD_APR.Areas.GLOD.Models;
using LOD_APR.Areas.GLOD.ModelsViews;
using LOD_APR.Helpers;
using LOD_APR.Models;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace LOD_APR.Areas.GLOD.Controllers
{
    [CustomAuthorize]
    public class FormEjecucionController : Controller
    {
        private LOD_DB db = new LOD_DB();

        [CustomAuthorize]
        public ActionResult Ejecuciones()
        {
            List<FORM_Ejecucion> lista = db.FORM_Ejecucion.ToList();
            return View(lista);
        }

        [CustomAuthorize]
        public ActionResult VerEjecucion(string id)
        { 
            FORM_Ejecucion ejec = db.FORM_Ejecucion.Find(id);
            if (ejec == null)
                return HttpNotFound();

            EjecucionView form = JsonConvert.DeserializeObject<EjecucionView>(ejec.FormData);

            return View(form);
        }

        [CustomAuthorize]
        public ActionResult ExecForm(string id)
        {
            FORM_Formularios formularioEvaluacion = db.FORM_Formularios.Find(id);
            if (formularioEvaluacion == null || !formularioEvaluacion.Activa)
                return HttpNotFound();

            MAE_Empresa MAE_Empresa = db.MAE_Empresa.Find(1);
            ViewBag.Logo = MAE_Empresa.LogoData;
            return View(formularioEvaluacion);
        }

        [CustomAuthorize]
        public ActionResult ExecFormHelpDesk(string id)
        {
            FORM_Formularios formularioEvaluacion = db.FORM_Formularios.Find(id);
            if (formularioEvaluacion == null || !formularioEvaluacion.Activa)
                return HttpNotFound();

            return PartialView("_GetForm",formularioEvaluacion);
        }
       
        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> PostForm(string __Ejecucion, string IdFormulario, HttpPostedFileBase[] fileName)
        {
            var formulario = await db.FORM_Formularios.FindAsync(IdFormulario);
            if (formulario == null || formulario.Activa == false)
                return Content("false");

            int? IdPersonalUser = 0;
            string Username = "Usuario Anónimo";
            string userID = string.Empty;

            userID = User.Identity.GetUserId();
            if (!String.IsNullOrEmpty(userID))
            {
                IdPersonalUser = db.SEG_UserContacto.Where(p => p.UserId == userID).FirstOrDefault().IdContacto;
                Username = User.Identity.GetUserName();
            }            
            
            try
            {
                EjecucionView obj = JsonConvert.DeserializeObject<EjecucionView>(__Ejecucion);
                obj.ExecutionTime = DateTime.Now;
                obj.UserName = Username;
                obj.Form = new Form() { FormID = formulario.IdForm, FormName = formulario.Titulo };
                obj.Folio = Convert.ToInt32(db.FORM_Ejecucion.Where(i => i.IdForm == formulario.IdForm).Count()) + 1;
                obj.ID = RandomString.GetRandomIDString(10);

                foreach (var o in obj.ItemsData)
                {
                    o.ItemName = db.FORM_FormItems.Find(o.ItemID).Titulo;
                    foreach(var p in o.Fields)
                    {
                        p.FieldName = db.FORM_FormPreguntas.Find(p.FieldID).Titulo;
                        p.Type.TypeName = TiposPreguntas.GetTipo(p.Type.Type).Titulo;
                        p.Type.TypeWidth = db.FORM_FormPreguntas.Find(p.FieldID).Largo;
                        foreach (var a in p.Options)
                        {
                            a.OptionName = db.FORM_FormAlternativa.Find(a.OptionID).Titulo;
                        }
                    }
                }

                foreach (var f in obj.FormsData)
                {
                    FORM_Formularios form = db.FORM_Formularios.Find(f.FormID);
                    f.FormName = form.Titulo;
                    foreach (var p in form.FORM_FormItems.First().FORM_FormPreguntas.OrderBy(o=> o.Indice))
                    {
                        f.Fields.Add(p.Titulo);
                    }
                }

                string dynObj = JsonConvert.SerializeObject(obj);

                //EjecucionView test = JsonConvert.DeserializeObject<EjecucionView>(dynObj);
                FORM_Ejecucion ej = new FORM_Ejecucion()
                {
                    IdEjecucion = obj.ID,
                    IdForm = obj.Form.FormID,
                    FH_Ejecucion = obj.ExecutionTime,
                    Folio = obj.Folio,
                    Usuario = Username,
                    FormData = dynObj
                };

                db.FORM_Ejecucion.Add(ej);
                db.SaveChanges();
                
                return Content("true;" + ej.IdEjecucion);

            }
            catch (Exception ex)
            {
                return Content("false;" + ex.Message);
            }

        }

        [AllowAnonymous]
        public ActionResult FormEnd(string id)
        {
            FORM_Ejecucion form = db.FORM_Ejecucion.Find(id);
            return PartialView(form);
        }

        [AllowAnonymous]
        public ActionResult FormError(string error)
        {
            ViewBag.Error = error;
            return PartialView();
        }

        [AllowAnonymous]
        public async Task<ActionResult> ModalFormItems(string IdForm)
        {
            ViewBag.IdForm = IdForm;
            FORM_Formularios formularioEvaluacion = await db.FORM_Formularios.FindAsync(IdForm);
            return PartialView("_FormAddItem", formularioEvaluacion.FORM_FormItems.First().FORM_FormPreguntas.ToList());
        }

        public void NotificarEnvio(FORM_Ejecucion ej, string user, string user_email)
        {
            MailServer email = new MailServer($"Envío de Formulario {ej.FORM_Formularios.Titulo}", user);
            email.InsertBodyItem($"Envío de Formulario " + ej.FORM_Formularios.Titulo, MailServer.ItemType.H3, MailServer.Align.Center);
            email.InsertLine();
            email.InsertSpace();
            email.InsertBodyItem($"Se informa que el usuario \"{user}\" ha enviado este formulario con los datos que acontinuación se presentan. Usted se encuentra en la lista de usuarios a notificar cada vez que se envíe este formulario.", MailServer.ItemType.p, MailServer.Align.Left);
            email.InsertFlexBoxTable(ej, s => s.FolioPad, s=> s.IdEjecucion, s => s.FORM_Formularios.Titulo, s => s.Usuario, s => s.FH_Ejecucion);
            email.InsertSpace();           

            email.SendEmail(user_email);
            
        }

    }
}