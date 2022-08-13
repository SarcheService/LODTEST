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
    public class TipoComunicacionController : Controller
    {
        private LOD_DB db = new LOD_DB();
        private GoogleReCaptchaHelper reCaptchaHelper = new GoogleReCaptchaHelper();
        // GET: Admin/TipoAnotacion
        public async Task<ActionResult> Index()
        {
            List<MAE_TipoComunicacion> list = await db.MAE_TipoComunicacion.ToListAsync();

            if (ValidaPermisos.ValidaPermisosEnController("0010030000"))
            {
                return View(list.OrderBy(x => x.IdTipoLod));
            }
            else
            {
                return RedirectToAction("SinPermiso", "../Pub");
            }


           
        }

        // GET: Admin/TipoAnotacion/Details/5


        // GET: Admin/TipoAnotacion/Create
        public ActionResult Create()
        {
            ViewBag.Titulo = "Nuevo Tipo de Comunicación";
            ViewBag.ClsModal = "hmodal-success";
            ViewBag.Action = "Create";

            List<MAE_TipoLOD> mAE_TipoLOD = db.MAE_TipoLOD.Where(x => x.Activo).ToList();
            ViewBag.IdTipoLod = new SelectList((from p in mAE_TipoLOD.ToList()
                                                select new
                                                {
                                                    IdTipoLod = p.IdTipoLod,
                                                    Nombre = p.Nombre
                                                }),
                                                       "IdTipoLod",
                                                       "Nombre");

            if (ValidaPermisos.ValidaPermisosEnController("0010030001"))
            {
                return PartialView("_modalForm");
            }
            else
            {
                return RedirectToAction("SinPermiso", "../Pub");
            }

            
        }

        // POST: Admin/TipoAnotacion/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Nombre,IdTipoLod,Descripcion,Activo")] MAE_TipoComunicacion mAE_TipoAnotacion)
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
                    db.MAE_TipoComunicacion.Add(mAE_TipoAnotacion);
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

        // GET: Admin/TipoAnotacion/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            ViewBag.Titulo = "Editar Tipo de Comunicación";
            ViewBag.ClsModal = "hmodal-warning";
            ViewBag.Action = "Edit";
            MAE_TipoComunicacion mAE_TipoAnot = await db.MAE_TipoComunicacion.FindAsync(id);

            List<MAE_TipoLOD> mAE_TipoLOD = db.MAE_TipoLOD.Where(x => x.Activo).ToList();
            ViewBag.IdTipoLod = new SelectList((from p in mAE_TipoLOD.ToList()
                                            select new
                                            {
                                                IdTipoLod = p.IdTipoLod,
                                                Nombre = p.Nombre
                                            }),
                                                       "IdTipoLod",
                                                       "Nombre");

            if (ValidaPermisos.ValidaPermisosEnController("0010030002"))
            {
                return PartialView("_modalFormEdit", mAE_TipoAnot);
            }
            else
            {
                return RedirectToAction("SinPermiso", "../Pub");
            }

            
        }

        // POST: Admin/TipoAnotacion/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "IdTipoCom,IdTipoLod,Nombre,Descripcion,Activo")] MAE_TipoComunicacion mAE_TipoAnotacion)
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
                    db.Entry(mAE_TipoAnotacion).State = EntityState.Modified;
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

        // GET: Admin/TipoAnotacion/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            MAE_TipoComunicacion mAE_TipoComunicacion = await db.MAE_TipoComunicacion.FindAsync(id);

            if (ValidaPermisos.ValidaPermisosEnController("0010030003"))
            {
                return PartialView("_Delete", mAE_TipoComunicacion);
            }
            else
            {
                return RedirectToAction("SinPermiso", "../Pub");
            }

            
        }

        // POST: Admin/TipoAnotacion/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {

            try
            {
                MAE_TipoComunicacion mAE_TipoComunicacion = await db.MAE_TipoComunicacion.FindAsync(id);
                db.MAE_TipoComunicacion.Remove(mAE_TipoComunicacion);
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
            List<MAE_TipoComunicacion> list = new List<MAE_TipoComunicacion>();
            list = await db.MAE_TipoComunicacion.ToListAsync();
            return PartialView("_getTable", list.OrderBy(x => x.IdTipoLod));
        }

        //public async Task<ActionResult> Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    MAE_TipoComunicacion mAE_TipoAnotacion = await db.MAE_TipoComunicacion.FindAsync(id);
        //    if (mAE_TipoAnotacion == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(mAE_TipoAnotacion);
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
