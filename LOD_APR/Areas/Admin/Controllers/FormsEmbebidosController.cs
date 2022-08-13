using LOD_APR.Areas.Admin.Models;
using LOD_APR.Helpers;
using LOD_APR.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace LOD_APR.Areas.Admin.Controllers
{
    [CustomAuthorize]
    public class FormsEmbebidosController : Controller
    {
        private LOD_DB db = new LOD_DB();

        public ActionResult Index()
        {
            var modelo = db.FORM_Formularios.Where(f=> f.Embebido).OrderBy(x => x.Titulo).ToList();
            return View(modelo);
        }
        public async Task<ActionResult> GetTable()
        {
            var modelo = db.FORM_Formularios.Where(f => f.Embebido).OrderBy(x => x.Titulo).ToList();
            return PartialView("_GetTabla", modelo);
        }
        public ActionResult Create()
        {
            FORM_Formularios model = new FORM_Formularios();
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IdForm,Titulo,Descripcion")] FORM_Formularios eVA_formularioEvaluacion)
        {
            try
            {
                eVA_formularioEvaluacion.IdForm = RandomString.GetRandomString(10);
                eVA_formularioEvaluacion.UserId = User.Identity.GetUserName();
                eVA_formularioEvaluacion.FechaCreacion = DateTime.Now;
                eVA_formularioEvaluacion.Activa = false;
                eVA_formularioEvaluacion.Embebido = true;
                eVA_formularioEvaluacion.FORM_FormItems = new List<FORM_FormItems>();
                eVA_formularioEvaluacion.FORM_FormItems.Add(new FORM_FormItems() {IdItem= RandomString.GetRandomString(10), IdForm = eVA_formularioEvaluacion.IdForm, Indice=0, Titulo="CUERPO DEL FORMULARIO" });

                db.FORM_Formularios.Add(eVA_formularioEvaluacion);
                db.SaveChanges();
                return RedirectToAction("Edit", new { id = eVA_formularioEvaluacion.IdForm });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(eVA_formularioEvaluacion);
            }
        }

        public ActionResult Edit(string id)
        {
            var eVA_formularioEvaluacion = db.FORM_Formularios.Find(id);
            if (eVA_formularioEvaluacion == null)
                return HttpNotFound();

            return View(eVA_formularioEvaluacion);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "IdForm,Titulo,Descripcion")] FORM_Formularios eVA_formularioEvaluacion)
        {
            FORM_Formularios modelanterior = db.FORM_Formularios.AsNoTracking().Where(x => x.IdForm == eVA_formularioEvaluacion.IdForm).First();

            try
            {
                modelanterior.Titulo = eVA_formularioEvaluacion.Titulo;
                modelanterior.Descripcion = eVA_formularioEvaluacion.Descripcion;

                db.Entry(modelanterior).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return Content("true");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return Content(ex.Message);
            }
        }

        public async Task<ActionResult> Delete(string id)
        {
            FORM_Formularios formularioEvaluacion = await db.FORM_Formularios.FindAsync(id);
            return PartialView("_Delete", formularioEvaluacion);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string id)
        {
            try
            {
                FORM_Formularios formularioEvaluacion = await db.FORM_Formularios.FindAsync(id);
                db.FORM_Formularios.Remove(formularioEvaluacion);
                await db.SaveChangesAsync();
                return Content("true");
            }
            catch (Exception err)
            {
                return Content(err.Message);
            }
        }

        public ActionResult Details(string id)
        {
            FORM_Formularios model = db.FORM_Formularios.Find(id);
            model.FORM_FormItems = db.FORM_FormItems.Where(i => i.IdForm == id).OrderBy(i => i.Indice).OrderBy(x => x.Indice).ToList();
            ViewBag.EvaluacionActiva = model.Activa;
            
            foreach (var item in model.FORM_FormItems)
            {
                item.ErrorList = new List<string>();
                if (item.FORM_FormPreguntas.Count() == 0)
                {
                    item.Errores = true;
                    item.ErrorList.Add("Ítem sin preguntas");
                }

                foreach (var param in item.FORM_FormPreguntas)
                {

                    param.ErrorList = new List<string>();

                    if (param.TipoParam > 912)
                    {
                        if (param.FORM_FormAlternativa.Count() == 0)
                        {
                            param.ErrorList.Add("Alternativas Pendientes");
                            param.Errores = true;

                            item.Errores = true;
                            item.ErrorList.Add("Pregunta con errores");
                        }

                    }

                }

            }

            return PartialView("_getDetails", model);
        }

    }
}