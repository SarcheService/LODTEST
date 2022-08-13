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
    public class ClassOneController : Controller
    {
        private LOD_DB db = new LOD_DB();
        private GoogleReCaptchaHelper reCaptchaHelper = new GoogleReCaptchaHelper();
        // GET: Admin/ClassOne
        public async Task<ActionResult> Index()
        {
            if (ValidaPermisos.ValidaPermisosEnController("0010060000"))
            {

                return View(await db.MAE_ClassOne.ToListAsync());
            }
            else
            {
                return RedirectToAction("SinPermiso", "../Pub");
            }

            
        }

        public async Task<ActionResult> getTable()
        {
            return PartialView("_getTable",await db.MAE_ClassOne.ToListAsync());
        }

        // GET: Admin/ClassOne/Create
        public ActionResult Create()
        {
            ViewBag.Titulo = "Nuevo Clasificación Doc.";
            ViewBag.ClsModal = "hmodal-success";
            ViewBag.Action = "Create";
            MAE_ClassOne mAE_ClassOne = new MAE_ClassOne() { Activo = true };

            if (ValidaPermisos.ValidaPermisosEnController("0010060001"))
            {

                return PartialView("_modalForm", mAE_ClassOne);
            }
            else
            {
                return RedirectToAction("SinPermiso", "../Pub");
            }

            
        }

        // POST: Admin/ClassOne/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Nombre,Descripcion,Activo")] MAE_ClassOne mAE_ClassOne)
        {
            try
            {
                string response = "Ocurrió un error al tratar de guardar los datos";
                if (ModelState.IsValid)
                {
                    if (!reCaptchaHelper.IsCaptchaValid(Request.Form["foo"]))
                    {
                        response = "Error en el captcha, Intente nuevamente.";
                        return Content(response);
                    }
                    else
                    {
                        response = "true";
                    }

                    db.MAE_ClassOne.Add(mAE_ClassOne);
                    await db.SaveChangesAsync();
                    return Content(response);
                }
                else
                {
                    if (!reCaptchaHelper.IsCaptchaValid(Request.Form["foo"]))
                    {
                        response = "Ha fallado el captcha. Intente nuevamente.";
                    }

                    return Content(response);
                }
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }

        }

        // GET: Admin/ClassOne/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MAE_ClassOne mAE_ClassOne = await db.MAE_ClassOne.FindAsync(id);
            if (mAE_ClassOne == null)
            {
                return HttpNotFound();
            }
            ViewBag.Titulo = "Editar Clasificación Doc.";
            ViewBag.ClsModal = "hmodal-warning";
            ViewBag.Action = "Edit";

            if (ValidaPermisos.ValidaPermisosEnController("0010060002"))
            {

                return PartialView("_modalFormEdit", mAE_ClassOne);
            }
            else
            {
                return RedirectToAction("SinPermiso", "../Pub");
            }
            
        }

        // POST: Admin/ClassOne/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "IdClassOne,Nombre,Descripcion,Activo")] MAE_ClassOne mAE_ClassOne)
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

                    db.Entry(mAE_ClassOne).State = EntityState.Modified;
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

        // GET: Admin/ClassOne/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MAE_ClassOne mAE_ClassOne = await db.MAE_ClassOne.FindAsync(id);
            if (mAE_ClassOne == null)
            {
                return HttpNotFound();
            }

            if (ValidaPermisos.ValidaPermisosEnController("0010060003"))
            {

                return PartialView("_Delete", mAE_ClassOne);
            }
            else
            {
                return RedirectToAction("SinPermiso", "../Pub");
            }

            
        }

        // POST: Admin/ClassOne/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {

            try
            {
                MAE_ClassOne mAE_ClassOne = await db.MAE_ClassOne.FindAsync(id);
                db.MAE_ClassOne.Remove(mAE_ClassOne);
                await db.SaveChangesAsync();
                return Content("true");
            }
            catch (Exception err)
            {
                return Content(err.Message);
            }

        }


        //// GET: Admin/ClassOne/Details/5
        //public async Task<ActionResult> Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    MAE_ClassOne mAE_ClassOne = await db.MAE_ClassOne.FindAsync(id);
        //    if (mAE_ClassOne == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(mAE_ClassOne);
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
