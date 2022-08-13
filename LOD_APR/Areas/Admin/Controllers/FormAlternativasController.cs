using LOD_APR.Areas.Admin.Models;
using LOD_APR.Helpers;
using LOD_APR.Models;
using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace LOD_APR.Areas.Admin.Controllers
{
    [CustomAuthorize]
    public class FormAlternativasController : Controller
    {
        private LOD_DB db = new LOD_DB();

        public ActionResult Create(string id)
        {
            ViewBag.Titulo = "Nueva Alternativa";
            ViewBag.ClsModal = "hmodal-success";
            ViewBag.Color = "success";
            ViewBag.Action = "Create";

            FORM_FormAlternativa alternativa = new FORM_FormAlternativa();
            alternativa.IdPregunta = id;
            
            return PartialView("_modalForm", alternativa);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "IdAlternativa,IdPregunta,Titulo")] FORM_FormAlternativa eVA_alternativasSeleccionMultiple)
        {
            try
            {
                var param = db.FORM_FormPreguntas.Find(eVA_alternativasSeleccionMultiple.IdPregunta);
                
                eVA_alternativasSeleccionMultiple.Indice =db.FORM_FormAlternativa.Where(x=>x.IdPregunta== eVA_alternativasSeleccionMultiple.IdPregunta).Count()+1;
                if (db.FORM_FormAlternativa.Where(x=> x.IdPregunta == eVA_alternativasSeleccionMultiple.IdPregunta).Count() >0)
                    eVA_alternativasSeleccionMultiple.Indice = db.FORM_FormAlternativa.Where(x => x.IdPregunta == eVA_alternativasSeleccionMultiple.IdPregunta).Max(x => x.Indice) + 1;

                eVA_alternativasSeleccionMultiple.IdAlternativa = RandomString.GetRandomString(10);
                db.FORM_FormAlternativa.Add(eVA_alternativasSeleccionMultiple);
                await db.SaveChangesAsync();
                return Content("true;" + param.IdItem);
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
        }

        public ActionResult Edit(string id)
        {
            FORM_FormAlternativa eVA_alternativasSeleccionMultiple = db.FORM_FormAlternativa.Find(id);
            ViewBag.Titulo = "Editar Alternativa";
            ViewBag.ClsModal = "hmodal-warning";
            ViewBag.Color = "warning";
            ViewBag.Action = "edit";             
            return PartialView("_modalForm", eVA_alternativasSeleccionMultiple);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "IdAlternativa,Titulo")] FORM_FormAlternativa eVA_alternativasSeleccionMultiple)
        {
            try
            {
                var alternativa = db.FORM_FormAlternativa.Find(eVA_alternativasSeleccionMultiple.IdAlternativa);
                var param = db.FORM_FormPreguntas.Find(alternativa.IdPregunta);
                alternativa.Titulo = eVA_alternativasSeleccionMultiple.Titulo;
                db.Entry(alternativa).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return Content("true;" + alternativa.FORM_FormPreguntas.IdItem);
            }
            catch (Exception ex)
            {

                return Content(ex.Message);
            }
        }
        
        public ActionResult Delete(string id)
        {
            FORM_FormAlternativa alternativa = db.FORM_FormAlternativa.Find(id);
            return PartialView("_Delete", alternativa);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string IdAlternativa)
        {
            try
            {
                FORM_FormAlternativa alternativa = db.FORM_FormAlternativa.Find(IdAlternativa);
                string IdForm = db.FORM_FormPreguntas.Find(alternativa.IdPregunta).IdItem;
                db.FORM_FormAlternativa.Remove(alternativa);
                await db.SaveChangesAsync();
                return Content("true;" + IdForm);
            }
            catch (Exception err)
            {
                return Content(err.Message);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
