using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using LOD_APR.Areas.Admin.Models;
using LOD_APR.Models;
using LOD_APR.Helpers;

namespace LOD_APR.Areas.Admin.Controllers
{
    [CustomAuthorize]
    public class TipoLODController : Controller
    {
        private LOD_DB db = new LOD_DB();
        private GoogleReCaptchaHelper reCaptchaHelper = new GoogleReCaptchaHelper();
        // GET: Admin/TipoLOD
        public async Task<ActionResult> Index()
        {
            List<MAE_TipoLOD> list = await db.MAE_TipoLOD.Where(x => x.Activo).ToListAsync();

            if (ValidaPermisos.ValidaPermisosEnController("0010050000"))
            {
                return View(list);
            }
            else
            {
                return RedirectToAction("SinPermiso", "../Pub");
            }
           
        }

       

        // GET: Admin/TipoLOD/Create
        public ActionResult Create()
        {
            ViewBag.Titulo = "Nuevo Tipo LOD";
            ViewBag.ClsModal = "hmodal-success";
            ViewBag.Action = "Create";
            MAE_TipoLOD mAE_TipoLOD = new MAE_TipoLOD() { Activo = true };

            if (ValidaPermisos.ValidaPermisosEnController("0010050001"))
            {
                return PartialView("_modalForm", mAE_TipoLOD);
            }
            else
            {
                return RedirectToAction("SinPermiso", "../Pub");
            }

            
        }

        // POST: Admin/TipoLOD/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Nombre,Descripcion,Activo,Color")] MAE_TipoLOD mAE_TipoLOD)
        {
            try {
                if (ModelState.IsValid)
                {
                    if (!reCaptchaHelper.IsCaptchaValid(Request.Form["foo"]))
                    {
                        var response = "Error en el captcha, Intente nuevamente.";
                        return Content(response);
                    }
                    db.MAE_TipoLOD.Add(mAE_TipoLOD);
                    await db.SaveChangesAsync();
                    return Content("true");
                }
                else
                {
                    return Content("Ocurrió un error al tratar de guardar los datos");
                }
            }
            catch(Exception ex) { 
                return Content(ex.Message); 
            }
            
        }

        // GET: Admin/TipoLOD/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            ViewBag.Titulo = "Editar Tipo LOD";
            ViewBag.ClsModal = "hmodal-warning";
            ViewBag.Action = "Edit";
            MAE_TipoLOD mAE_TipoLod = await db.MAE_TipoLOD.FindAsync(id);

            if (ValidaPermisos.ValidaPermisosEnController("0010050002"))
            {
                return PartialView("_modalFormEdit", mAE_TipoLod);
            }
            else
            {
                return RedirectToAction("SinPermiso", "../Pub");
            }

            
        }

        // POST: Admin/TipoLOD/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "IdTipoLod,Nombre,Descripcion,Activo,Color")] MAE_TipoLOD mAE_TipoLOD)
        {

            if (ModelState.IsValid)
            {
                try
                {
                    if (!reCaptchaHelper.IsCaptchaValid(Request.Form["foo"]))
                    {
                        var response = "Error en el captcha, Intente nuevamente.";
                        return Content(response);
                    }
                    db.Entry(mAE_TipoLOD).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                    return Content("true");
                }
                catch (Exception ex)
                {
                    return Content(ex.Message);
                }
            }
            else
            {
                return Content("Ocurrió un error al tratar de guardar los datos");
            }
            
        }

        // GET: Admin/TipoLOD/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            MAE_TipoLOD mAE_TipoLod = await db.MAE_TipoLOD.FindAsync(id);

            if (ValidaPermisos.ValidaPermisosEnController("0010050003"))
            {
                return PartialView("_Delete", mAE_TipoLod);
            }
            else
            {
                return RedirectToAction("SinPermiso", "../Pub");
            }

            
        }

        // POST: Admin/TipoLOD/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            try
            {
                MAE_TipoLOD mAE_TipoLOD = await db.MAE_TipoLOD.FindAsync(id);
                db.MAE_TipoLOD.Remove(mAE_TipoLOD);
                await db.SaveChangesAsync();
                return Content("true");
            }
            catch (Exception err)
            {
                return Content(err.Message);
            }
        }

        public async Task<ActionResult> getTable()
        {
            List<MAE_TipoLOD> list = new List<MAE_TipoLOD>();
            list = await db.MAE_TipoLOD.ToListAsync();
            return PartialView("_getTable", list);
        }

        // GET: Admin/TipoLOD/Details/5
        //public async Task<ActionResult> Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    MAE_TipoLOD mAE_TipoLOD = await db.MAE_TipoLOD.FindAsync(id);
        //    if (mAE_TipoLOD == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(mAE_TipoLOD);
        //}


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
