using LOD_APR.Areas.Admin.Helpers;
using LOD_APR.Areas.Admin.Models;
using LOD_APR.Helpers;
using LOD_APR.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using static LOD_APR.Models.Auxiliares;

namespace LOD_APR.Areas.Admin.Controllers
{
    [CustomAuthorize]
    public class FormPreguntasController : Controller
    {
        private LOD_DB db = new LOD_DB();

        public async Task<ActionResult> UpdateIndex(string id, int index)
        {
            var item = db.FORM_FormPreguntas.Find(id);
            if (item == null)
                return HttpNotFound();

            item.Indice = index;
            db.Entry(item).State = EntityState.Modified;
            await db.SaveChangesAsync();
            return Content(index.ToString());
        }

        public ActionResult Create(string id, bool embebido)
        {
            ViewBag.Titulo = "Nueva Pregunta";
            ViewBag.ClsModal = "hmodal-success";
            ViewBag.Color = "success";
            ViewBag.Action = "Create";
            FORM_FormPreguntas model = new FORM_FormPreguntas();
            model.IdItem = id;
            model.Largo = 10;
        
            List<ComboBoxEstandar> ListTipoParam = TiposPreguntas.ListTipoPregunta(embebido);
            ViewBag.TipoParam = new SelectList(ListTipoParam.OrderBy(x => x.Id), "Id", "Value", 1);
            return PartialView("_modalForm", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create( FORM_FormPreguntas eVA_parametrosEvaluacion)
        {
            try
            {              
                eVA_parametrosEvaluacion.Indice = db.FORM_FormPreguntas.Where(x=>x.IdItem==eVA_parametrosEvaluacion.IdItem).Count()+1;
                if (db.FORM_FormPreguntas.Where(x => x.IdItem == eVA_parametrosEvaluacion.IdItem).Count()>0)
                    eVA_parametrosEvaluacion.Indice = db.FORM_FormPreguntas.Where(x => x.IdItem == eVA_parametrosEvaluacion.IdItem).Max(x=> x.Indice) +1;

                eVA_parametrosEvaluacion.IdPregunta = RandomString.GetRandomString(10);
                db.FORM_FormPreguntas.Add(eVA_parametrosEvaluacion);
                db.SaveChanges();
                return Content("true;" + eVA_parametrosEvaluacion.IdItem);
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
        }

        public async Task<ActionResult> Edit(string id, bool embebido)
        {
            FORM_FormPreguntas parametro = db.FORM_FormPreguntas.Find(id);
            ViewBag.Titulo = "Editar Pregunta";
            ViewBag.ClsModal = "hmodal-warning";
            ViewBag.Color = "warning";
            ViewBag.Action = "Edit";
            List<ComboBoxEstandar> ListTipoParam = TiposPreguntas.ListTipoPregunta(embebido);
            ViewBag.TipoParam = new SelectList(ListTipoParam.OrderBy(x => x.Id), "Id", "Value", parametro.TipoParam);
            return PartialView("_modalEditForm", parametro);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit( FORM_FormPreguntas eVA_parametrosEvaluacion)
        {
            try
            {
                var paramEdit = db.FORM_FormPreguntas.Find(eVA_parametrosEvaluacion.IdPregunta);
                if (paramEdit == null)
                    return HttpNotFound();

                paramEdit.Titulo = eVA_parametrosEvaluacion.Titulo;
                paramEdit.Descripcion = eVA_parametrosEvaluacion.Descripcion;
                paramEdit.Obligatoria = eVA_parametrosEvaluacion.Obligatoria;
                paramEdit.Largo = eVA_parametrosEvaluacion.Largo;
                
                db.Entry(paramEdit).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return Content("true;" + paramEdit.IdItem);
            }
            catch (Exception ex)
            {
                return Content("error;" + ex.Message);
            }
        }

        public async Task<ActionResult> Delete(string id)
        {
            FORM_FormPreguntas parametro = db.FORM_FormPreguntas.Find(id);
            return PartialView("_delete", parametro);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string IdPregunta)
        {
            try
            {
                FORM_FormPreguntas parametro = db.FORM_FormPreguntas.Find(IdPregunta);
                string IdObj = parametro.IdItem;
                db.FORM_FormPreguntas.Remove(parametro);
                await db.SaveChangesAsync();
                return Content("true;"+ IdObj);
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
