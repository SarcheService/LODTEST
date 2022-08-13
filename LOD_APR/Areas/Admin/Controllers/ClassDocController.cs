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
    public class ClassDocController : Controller
    {
        private LOD_DB db = new LOD_DB();
        private GoogleReCaptchaHelper reCaptchaHelper = new GoogleReCaptchaHelper();
        // GET: Admin/ClassDoc
        public async Task<ActionResult> Index()
        {
            var mAE_ClassDoc = db.MAE_ClassDoc.Include(m => m.MAE_ClassTwo).Include(m => m.MAE_SubtipoComunicacion).Include(m => m.MAE_TipoDocumento);

            if (ValidaPermisos.ValidaPermisosEnController("0010090000"))
            {
                return View(await mAE_ClassDoc.OrderBy(x => x.MAE_ClassTwo.IdClassOne).ThenBy(x => x.IdClassTwo).ToListAsync());
            }
            else
            {
                return RedirectToAction("SinPermiso", "../Pub");
            }

            
        }

        public async Task<ActionResult> getTable()
        {
            var mAE_ClassDoc = db.MAE_ClassDoc.Include(m => m.MAE_ClassTwo).Include(m => m.MAE_SubtipoComunicacion).Include(m => m.MAE_TipoDocumento);
            return PartialView("_getTable", await mAE_ClassDoc.OrderBy(x => x.MAE_ClassTwo.IdClassOne).ThenBy(x => x.IdClassTwo).ToListAsync());
        }

        // GET: Admin/ClassDoc/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MAE_ClassDoc mAE_ClassDoc = await db.MAE_ClassDoc.FindAsync(id);
            if (mAE_ClassDoc == null)
            {
                return HttpNotFound();
            }
            return View(mAE_ClassDoc);
        }

        // GET: Admin/ClassDoc/Create
        public ActionResult Create()
        {
            ViewBag.IdClassTwo = new SelectList(db.MAE_ClassTwo, "IdClassTwo", "Nombre");
            ViewBag.IdTipoSub = new SelectList(db.MAE_SubtipoComunicacion, "IdTipoSub", "Nombre");
            ViewBag.IdTipo = new SelectList(db.MAE_TipoDocumento, "IdTipo", "Tipo");



            ViewBag.Titulo = "Nuev Control Doc.";
            ViewBag.ClsModal = "hmodal-success";
            ViewBag.Action = "Create";
            MAE_ClassDoc mAE_ClassDoc = new MAE_ClassDoc() { };

            if (ValidaPermisos.ValidaPermisosEnController("0010090001"))
            {
                return PartialView("_modalForm", mAE_ClassDoc);
            }
            else
            {
                return RedirectToAction("SinPermiso", "../Pub");
            }

            
        }

        // POST: Admin/ClassDoc/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "IdClassTwo,IdTipo,IdTipoSub,EsLiquidacion")] MAE_ClassDoc mAE_ClassDoc)
        {


            try
            {
                if (ModelState.IsValid)
                {
                    if (!reCaptchaHelper.IsCaptchaValid(Request.Form["foo"]))
                    {
                        var response = "Error en el captcha, Intente nuevamente.";
                        return Content(response);
                    }
                    db.MAE_ClassDoc.Add(mAE_ClassDoc);
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

        // GET: Admin/ClassDoc/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MAE_ClassDoc mAE_ClassDoc = await db.MAE_ClassDoc.FindAsync(id);
            if (mAE_ClassDoc == null)
            {
                return HttpNotFound();
            }
            ViewBag.IdClassTwo = new SelectList(db.MAE_ClassTwo, "IdClassTwo", "Nombre", mAE_ClassDoc.IdClassTwo);
            ViewBag.IdTipoSub = new SelectList(db.MAE_SubtipoComunicacion, "IdTipoSub", "Nombre", mAE_ClassDoc.IdTipoSub);
            ViewBag.IdTipo = new SelectList(db.MAE_TipoDocumento, "IdTipo", "Tipo", mAE_ClassDoc.IdTipo);

            ViewBag.Titulo = "Detalle Control Documento";
            ViewBag.ClsModal = "hmodal-info";
            ViewBag.Action = "Edit";

            if (ValidaPermisos.ValidaPermisosEnController("0010090002"))
            {
                return PartialView("_modalFormEdit", mAE_ClassDoc);
            }
            else
            {
                return RedirectToAction("SinPermiso", "../Pub");
            }

            
        }

        // POST: Admin/ClassDoc/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "IdClassDoc,IdClassTwo,IdTipo,IdTipoSub,EsLiquidacion")] MAE_ClassDoc mAE_ClassDoc)
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
                    db.Entry(mAE_ClassDoc).State = EntityState.Modified;
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

        // GET: Admin/ClassDoc/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MAE_ClassDoc mAE_ClassDoc = await db.MAE_ClassDoc.FindAsync(id);
            if (mAE_ClassDoc == null)
            {
                return HttpNotFound();
            }

            if (ValidaPermisos.ValidaPermisosEnController("0010090003"))
            {

                return PartialView("_Delete", mAE_ClassDoc);
            }
            else
            {
                return RedirectToAction("SinPermiso", "../Pub");
            }

        }

        // POST: Admin/ClassDoc/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {

            try
            {
                MAE_ClassDoc mAE_ClassDoc = await db.MAE_ClassDoc.FindAsync(id);
                db.MAE_ClassDoc.Remove(mAE_ClassDoc);
                await db.SaveChangesAsync();
                return Content("true");
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
