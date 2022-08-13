using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using LOD_APR.Models;
using LOD_APR.Areas.Admin.Models;
using LOD_APR.Helpers;

namespace LOD_APR.Areas.Admin.Controllers
{
    [CustomAuthorize]
    public class SubtipoComunicacionController : Controller
    {
        private LOD_DB db = new LOD_DB();
        private GoogleReCaptchaHelper reCaptchaHelper = new GoogleReCaptchaHelper();
        // GET: Admin/SubtipoComunicacion
        public async Task<ActionResult> Index()
        {
            var mAE_SubtipoComunicacion = db.MAE_SubtipoComunicacion.Include(m => m.MAE_TipoComunicacion);

            if (ValidaPermisos.ValidaPermisosEnController("0010040000"))
            {
                return View(await mAE_SubtipoComunicacion.OrderBy(x => x.MAE_TipoComunicacion.IdTipoLod).ThenBy(x => x.IdTipoCom).ToListAsync());
            }
            else
            {
                return RedirectToAction("SinPermiso", "../Pub");
            }

           
        }

        public async Task<ActionResult> getTable()
        {
            List<MAE_SubtipoComunicacion> list = new List<MAE_SubtipoComunicacion>();
            list = await db.MAE_SubtipoComunicacion.ToListAsync();
            return PartialView("_getTable", list.OrderBy(x => x.MAE_TipoComunicacion.IdTipoLod).ThenBy(x => x.IdTipoCom).ToList());
        }

        // GET: Admin/SubtipoComunicacion/Create
        public ActionResult Create()
        {
            List<MAE_TipoComunicacion> mAE_TipoComunicacion = db.MAE_TipoComunicacion.Where(x => x.Activo).ToList();
            ViewBag.IdTipoCom = new SelectList((from p in mAE_TipoComunicacion.ToList()
                                                select new
                                                {
                                                    IdTipoCom = p.IdTipoCom,
                                                    Nombre = p.Nombre + " - "+p.MAE_TipoLOD.Nombre
                                                }),
                                                       "IdTipoCom",
                                                       "Nombre");

            ViewBag.Titulo = "Nuevo Subtipo Comunicación";
            ViewBag.ClsModal = "hmodal-success";
            ViewBag.Action = "Create";

            MAE_SubtipoComunicacion mAE_SubtipoComunicacion = new MAE_SubtipoComunicacion() { Activo = true };

            if (ValidaPermisos.ValidaPermisosEnController("0010040001"))
            {
                return PartialView("_modalForm", mAE_SubtipoComunicacion);
            }
            else
            {
                return RedirectToAction("SinPermiso", "../Pub");
            }

            
        }

        // POST: Admin/SubtipoComunicacion/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Nombre,Descripcion,IdTipoCom,Activo")] MAE_SubtipoComunicacion mAE_SubtipoComunicacion)
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
                    db.MAE_SubtipoComunicacion.Add(mAE_SubtipoComunicacion);
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

        // GET: Admin/SubtipoComunicacion/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MAE_SubtipoComunicacion mAE_SubtipoComunicacion = await db.MAE_SubtipoComunicacion.FindAsync(id);
            if (mAE_SubtipoComunicacion == null)
            {
                return HttpNotFound();
            }


            List<MAE_TipoComunicacion> mAE_TipoComunicacion = db.MAE_TipoComunicacion.Where(x => x.Activo).ToList();
            ViewBag.IdTipoCom = new SelectList((from p in mAE_TipoComunicacion.ToList()
                                                select new
                                                {
                                                    IdTipoCom = p.IdTipoCom,
                                                    Nombre = p.Nombre + " - " + p.MAE_TipoLOD.Nombre
                                                }),
                                                       "IdTipoCom",
                                                       "Nombre", mAE_SubtipoComunicacion.MAE_TipoComunicacion);


            ViewBag.Titulo = "Editar Subtipo de Comunicación";
            ViewBag.ClsModal = "hmodal-warning";
            ViewBag.Action = "Edit";

            if (ValidaPermisos.ValidaPermisosEnController("0010040002"))
            {
                return PartialView("_modalFormEdit", mAE_SubtipoComunicacion);
            }
            else
            {
                return RedirectToAction("SinPermiso", "../Pub");
            }

            
        }

        // POST: Admin/SubtipoComunicacion/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "IdTipoSub,Nombre,Descripcion,IdTipoCom,Activo")] MAE_SubtipoComunicacion mAE_SubtipoComunicacion)
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
                    db.Entry(mAE_SubtipoComunicacion).State = EntityState.Modified;
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

        // GET: Admin/SubtipoComunicacion/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MAE_SubtipoComunicacion mAE_SubtipoComunicacion = await db.MAE_SubtipoComunicacion.FindAsync(id);
            if (mAE_SubtipoComunicacion == null)
            {
                return HttpNotFound();
            }

            if (ValidaPermisos.ValidaPermisosEnController("0010040003"))
            {
                return PartialView("_Delete", mAE_SubtipoComunicacion);
            }
            else
            {
                return RedirectToAction("SinPermiso", "../Pub");
            }

            
        }

        // POST: Admin/SubtipoComunicacion/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            try
            {
                MAE_SubtipoComunicacion mAE_SubtipoComunicacion = await db.MAE_SubtipoComunicacion.FindAsync(id);
                db.MAE_SubtipoComunicacion.Remove(mAE_SubtipoComunicacion);
                await db.SaveChangesAsync();
                return Content("true");
            }
            catch (Exception err)
            {
                return Content(err.Message);
            }

        }

        //// GET: Admin/SubtipoComunicacion/Details/5
        //public async Task<ActionResult> Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    MAE_SubtipoComunicacion mAE_SubtipoComunicacion = await db.MAE_SubtipoComunicacion.FindAsync(id);
        //    if (mAE_SubtipoComunicacion == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(mAE_SubtipoComunicacion);
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
