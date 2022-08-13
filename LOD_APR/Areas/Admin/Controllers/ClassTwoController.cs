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
    public class ClassTwoController : Controller
    {
        private LOD_DB db = new LOD_DB();
        private GoogleReCaptchaHelper reCaptchaHelper = new GoogleReCaptchaHelper();
        // GET: Admin/ClassTwo
        public async Task<ActionResult> Index()
        {
            var mAE_ClassTwo = db.MAE_ClassTwo.Include(m => m.MAE_ClassOne).OrderBy(x => x.IdClassOne);

            if (ValidaPermisos.ValidaPermisosEnController("0010070000"))
            {
                return View(await mAE_ClassTwo.OrderBy(x => x.IdClassOne).ToListAsync());
            }
            else
            {
                return RedirectToAction("SinPermiso", "../Pub");
            }

            
        }
        public async Task<ActionResult> getTable()
        {
            var mAE_ClassTwo = db.MAE_ClassTwo.Include(m => m.MAE_ClassOne).OrderBy(x => x.IdClassOne);
            return PartialView("_getTable",await mAE_ClassTwo.OrderBy(x => x.IdClassOne).ToListAsync());
        }


        // GET: Admin/ClassTwo/Create
        public ActionResult Create()
        {
            List<MAE_ClassOne> mAE_ClassOne = db.MAE_ClassOne.Where(x => x.Activo).ToList();
            ViewBag.IdClassOne = new SelectList((from p in mAE_ClassOne.ToList()
                                                select new
                                                {
                                                    IdClassOne = p.IdClassOne,
                                                    Nombre = p.Nombre
                                                }),
                                                       "IdClassOne",
                                                       "Nombre");


            ViewBag.Titulo = "Nueva Subclasificación Doc.";
            ViewBag.ClsModal = "hmodal-success";
            ViewBag.Action = "Create";
            MAE_ClassTwo mAE_ClassTwo = new MAE_ClassTwo() { Activo = true };

            if (ValidaPermisos.ValidaPermisosEnController("0010070001"))
            {
                return PartialView("_modalForm", mAE_ClassTwo);
            }
            else
            {
                return RedirectToAction("SinPermiso", "../Pub");
            }

            
        }

        // POST: Admin/ClassTwo/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Nombre,Descripcion,Activo,IdClassOne")] MAE_ClassTwo mAE_ClassTwo)
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
                    db.MAE_ClassTwo.Add(mAE_ClassTwo);
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

        // GET: Admin/ClassTwo/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MAE_ClassTwo mAE_ClassTwo = await db.MAE_ClassTwo.FindAsync(id);
            if (mAE_ClassTwo == null)
            {
                return HttpNotFound();
            }
            ViewBag.Titulo = "Editar Subclasificación";
            ViewBag.ClsModal = "hmodal-warning";
            ViewBag.Action = "Edit";

            if (ValidaPermisos.ValidaPermisosEnController("0010070002"))
            {
                return PartialView("_modalFormEdit", mAE_ClassTwo);
            }
            else
            {
                return RedirectToAction("SinPermiso", "../Pub");
            }

            
        }

        // POST: Admin/ClassTwo/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "IdClassTwo,Nombre,Descripcion,Activo,IdClassOne")] MAE_ClassTwo mAE_ClassTwo)
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
                    db.Entry(mAE_ClassTwo).State = EntityState.Modified;
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

        // GET: Admin/ClassTwo/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MAE_ClassTwo mAE_ClassTwo = await db.MAE_ClassTwo.FindAsync(id);
            if (mAE_ClassTwo == null)
            {
                return HttpNotFound();
            }

            if (ValidaPermisos.ValidaPermisosEnController("0010070003"))
            {
                return PartialView("_Delete", mAE_ClassTwo);
            }
            else
            {
                return RedirectToAction("SinPermiso", "../Pub");
            }

            
        }

        // POST: Admin/ClassTwo/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            try
            {
                MAE_ClassTwo mAE_ClassTwo = await db.MAE_ClassTwo.FindAsync(id);
                db.MAE_ClassTwo.Remove(mAE_ClassTwo);
                await db.SaveChangesAsync();
                return Content("true");
            }
            catch (Exception err)
            {
                return Content(err.Message);
            }
            
        }


        //// GET: Admin/ClassTwo/Details/5
        //public async Task<ActionResult> Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    MAE_ClassTwo mAE_ClassTwo = await db.MAE_ClassTwo.FindAsync(id);
        //    if (mAE_ClassTwo == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(mAE_ClassTwo);
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
