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
    [CustomAuthorize] //Validación
    public class DireccionesMOPController : Controller
    {
        private LOD_DB db = new LOD_DB();
        private GoogleReCaptchaHelper reCaptchaHelper = new GoogleReCaptchaHelper();
        // GET: Admin/DireccionesMOP
        public async Task<ActionResult> Index()
        {

            if (ValidaPermisos.ValidaPermisosEnController("0010100000"))
            {
                return View(await db.MAE_DireccionesMOP.ToListAsync());
            }
            else
            {
                return RedirectToAction("SinPermiso", "../Pub");
            }

            
        }

        public async Task<ActionResult> getTable()
        {
           
            var list = await db.MAE_DireccionesMOP.ToListAsync();
            return PartialView("_getTable", list);
        }

        // GET: Admin/DireccionesMOP/Create
        public ActionResult Create()
        {
            ViewBag.Titulo = "Nueva Dirección MOP";
            ViewBag.ClsModal = "hmodal-success";
            ViewBag.Action = "Create";

            if (ValidaPermisos.ValidaPermisosEnController("0010100001"))
            {
                return PartialView("_modalForm");
            }
            else
            {
                return RedirectToAction("SinPermiso", "../Pub");
            }

                
        }

        // POST: Admin/DireccionesMOP/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "NombreDireccion,DescripcionDireccion")] MAE_DireccionesMOP mAE_DireccionesMOP)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.MAE_DireccionesMOP.Add(mAE_DireccionesMOP);
                    await db.SaveChangesAsync();
                    return Content("true");
                }
                else
                {
                    return Content("Ocurrió un error al tratar de guardar los datos");
                }
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }

        }

        // GET: Admin/DireccionesMOP/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MAE_DireccionesMOP mAE_DireccionesMOP = await db.MAE_DireccionesMOP.FindAsync(id);
            if (mAE_DireccionesMOP == null)
            {
                return HttpNotFound();
            }

            ViewBag.Titulo = "Editar Dirección MOP";
            ViewBag.ClsModal = "hmodal-warning";
            ViewBag.Action = "Edit";

            if (ValidaPermisos.ValidaPermisosEnController("0010100002"))
            {
                return PartialView("_modalFormEdit", mAE_DireccionesMOP);
            }
            else
            {
                return RedirectToAction("SinPermiso", "../Pub");
            }

            
        }

        // POST: Admin/DireccionesMOP/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "IdDireccion,NombreDireccion,DescripcionDireccion")] MAE_DireccionesMOP mAE_DireccionesMOP)
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
                    db.Entry(mAE_DireccionesMOP).State = EntityState.Modified;
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

        // GET: Admin/DireccionesMOP/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MAE_DireccionesMOP mAE_DireccionesMOP = await db.MAE_DireccionesMOP.FindAsync(id);
            if (mAE_DireccionesMOP == null)
            {
                return HttpNotFound();
            }

            if (ValidaPermisos.ValidaPermisosEnController("0010100003"))
            {
                return PartialView("_Delete", mAE_DireccionesMOP);
            }
            else
            {
                return RedirectToAction("SinPermiso", "../Pub");
            }

            
        }

        // POST: Admin/DireccionesMOP/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {

            try
            {
                MAE_DireccionesMOP mAE_DireccionesMOP = await db.MAE_DireccionesMOP.FindAsync(id);
                db.MAE_DireccionesMOP.Remove(mAE_DireccionesMOP);
                await db.SaveChangesAsync();
                return Content("true");
            }
            catch (Exception err)
            {
                return Content(err.Message);
            }

        }

        // GET: Admin/DireccionesMOP/Details/5
        //public async Task<ActionResult> Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    MAE_DireccionesMOP mAE_DireccionesMOP = await db.MAE_DireccionesMOP.FindAsync(id);
        //    if (mAE_DireccionesMOP == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(mAE_DireccionesMOP);
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
