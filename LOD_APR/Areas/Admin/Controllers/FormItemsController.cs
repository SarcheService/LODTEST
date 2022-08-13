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
    public class FormItemsController : Controller
    {
        private LOD_DB db = new LOD_DB();

        public async Task<ActionResult> UpdateIndex(string id, int index)
        {
            var item = db.FORM_FormItems.Find(id);
            if (item == null)
                return HttpNotFound();

            item.Indice = index;
            db.Entry(item).State = EntityState.Modified;
            await db.SaveChangesAsync();
            return Content(index.ToString());
        }

        public ActionResult Create(string id)
        {
            //Parametros del Modal
            ViewBag.Titulo = "Nuevo Item";
            ViewBag.ClsModal = "hmodal-success";
            ViewBag.Color = "success";
            ViewBag.Action = "Create";
            FORM_FormItems ItemEva = new FORM_FormItems();
            ItemEva.FORM_Formularios = db.FORM_Formularios.Find(id);
            ItemEva.IdForm = id;
            return PartialView("_modalForm", ItemEva);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IdForm,Titulo,Descripcion")] FORM_FormItems eVA_itemsFormularioEvaluacion)
        {
            try
            {
                eVA_itemsFormularioEvaluacion.IdItem = RandomString.GetRandomString(10);
                eVA_itemsFormularioEvaluacion.Indice = Convert.ToInt16(db.FORM_FormItems.Where(i => i.IdForm == eVA_itemsFormularioEvaluacion.IdForm).Count()) + 1;
                db.FORM_FormItems.Add(eVA_itemsFormularioEvaluacion);
                db.SaveChanges();
          
                return Content("true;" + eVA_itemsFormularioEvaluacion.IdForm);
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
        }

        public async Task<ActionResult> Edit(string id)
        {
            ViewBag.Titulo = "Editar Item";
            ViewBag.ClsModal = "hmodal-warning";
            ViewBag.Color = "warning";
            ViewBag.Action = "Edit";
            FORM_FormItems ItemEva = db.FORM_FormItems.Find(id);
            
            return PartialView("_modalForm", ItemEva);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "IdItem,IdForm,Titulo,Descripcion")] FORM_FormItems eVA_itemsFormularioEvaluacion)
        {
            try
            {
                FORM_FormItems modificado = await db.FORM_FormItems.FindAsync(eVA_itemsFormularioEvaluacion.IdItem);
                modificado.Titulo = eVA_itemsFormularioEvaluacion.Titulo;
                modificado.Descripcion = eVA_itemsFormularioEvaluacion.Descripcion;
                db.Entry(modificado).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return Content("true;" + modificado.IdForm);
            }
            catch (Exception ex)
            {
                return Content("error;" + ex.Message);
            }
        }

        public async Task<ActionResult> Delete(string id)
        {
            FORM_FormItems itemsFormularioEvaluacion = db.FORM_FormItems.Find(id);

            return PartialView("_Delete", itemsFormularioEvaluacion);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string id)
        {
            try
            {
                FORM_FormItems itemsFormularioEvaluacion = db.FORM_FormItems.Find(id);
                string IdForm = itemsFormularioEvaluacion.IdForm;
              
                db.FORM_FormItems.Remove(itemsFormularioEvaluacion);
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
