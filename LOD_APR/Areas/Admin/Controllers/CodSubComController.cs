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
    public class CodSubComController : Controller
    {
        private LOD_DB db = new LOD_DB();
        private GoogleReCaptchaHelper reCaptchaHelper = new GoogleReCaptchaHelper();
        // GET: Admin/CodSubCom
        public async Task<ActionResult> Index()
        {
            var mAE_CodSubCom = db.MAE_CodSubCom.Include(m => m.MAE_SubtipoComunicacion).Include(m => m.MAE_TipoDocumento);

            List<MAE_CodSubCom> list_CodSub = await mAE_CodSubCom.ToListAsync();
            foreach (var item in list_CodSub)
            {
                MAE_ClassDoc mAE_ClassDoc = db.MAE_ClassDoc.Where(x => x.IdTipo == item.IdTipo && x.IdTipoSub == item.IdTipoSub).FirstOrDefault();
                if (mAE_ClassDoc != null)
                    item.MAE_ClassDoc = mAE_ClassDoc;
            }

            if (ValidaPermisos.ValidaPermisosEnController("0010080000"))
            {
                return View(list_CodSub.OrderBy(x => x.MAE_SubtipoComunicacion.MAE_TipoComunicacion.IdTipoLod).ThenBy(x => x.MAE_SubtipoComunicacion.IdTipoCom).ToList());
            }
            else
            {
                return RedirectToAction("SinPermiso", "../Pub");
            }

            
        }

        public async Task<ActionResult> getTable()
        {
            var mAE_CodSubCom = db.MAE_CodSubCom.Include(m => m.MAE_SubtipoComunicacion).Include(m => m.MAE_TipoDocumento);

            List<MAE_CodSubCom> list_CodSub = await mAE_CodSubCom.ToListAsync();
            foreach (var item in list_CodSub)
            {
                MAE_ClassDoc mAE_ClassDoc = db.MAE_ClassDoc.Where(x => x.IdTipo == item.IdTipo).FirstOrDefault();
                if (mAE_ClassDoc != null)
                    item.MAE_ClassDoc = mAE_ClassDoc;
            }

            return PartialView("_GetTable", list_CodSub.OrderBy(x => x.MAE_SubtipoComunicacion.MAE_TipoComunicacion.IdTipoLod).ThenBy(x => x.MAE_SubtipoComunicacion.IdTipoCom).ToList());
        }

       
        // GET: Admin/CodSubCom/Create
        public ActionResult Create()
        {

            List<MAE_TipoDocumento> mAE_TipoDocumentos = new List<MAE_TipoDocumento>();
            //List<MAE_TipoDocumento> tiposExcept = db.MAE_CodCom.Select(x => x.MAE_TipoDocumento).ToList();
            List<MAE_TipoDocumento> auxList = db.MAE_TipoDocumento.Where(x => x.Activo).ToList();
            mAE_TipoDocumentos = auxList.ToList();
            ViewBag.IdTipo = new SelectList((from p in mAE_TipoDocumentos.ToList()
                                             select new
                                             {
                                                 IdTipo = p.IdTipo,
                                                 Tipo = p.Tipo
                                             }),
                                                       "IdTipo",
                                                       "Tipo");

            List<MAE_SubtipoComunicacion> mAE_SubtipoComunicacion = db.MAE_SubtipoComunicacion.Where(x => x.Activo && !x.Nombre.Equals("Comunicación General")).ToList();
            ViewBag.IdTipoSub = new SelectList((from p in mAE_SubtipoComunicacion.ToList()
                                                select new
                                                {
                                                    IdTipoSub = p.IdTipoSub,
                                                    Nombre = p.Nombre
                                                }),
                                                       "IdTipoSub",
                                                       "Nombre");


            List<MAE_TipoComunicacion> mAE_TipoComunicacion = db.MAE_TipoComunicacion.Where(x => x.Activo && !x.Nombre.Equals("Comunicación General")).ToList();
            ViewBag.IdTipoCom = new SelectList((from p in mAE_TipoComunicacion.ToList()
                                                select new
                                                {
                                                    IdTipoCom = p.IdTipoCom,
                                                    Nombre = p.Nombre
                                                }),
                                                       "IdTipoCom",
                                                       "Nombre");


            ViewBag.Titulo = "Nuevo Control Documentario";
            ViewBag.ClsModal = "hmodal-success";
            ViewBag.Action = "Create";

            MAE_CodSubCom mAE_CodSubCom = new MAE_CodSubCom();
            mAE_CodSubCom.Activo = true;
            mAE_CodSubCom.Obligatorio = true;


            if (ValidaPermisos.ValidaPermisosEnController("0010080001"))
            {
                return PartialView("_modalForm", mAE_CodSubCom);
            }
            else
            {
                return RedirectToAction("SinPermiso", "../Pub");
            }

            
        }

        // POST: Admin/CodSubCom/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "IdTipoSub,IdTipo,Activo,Obligatorio")] MAE_CodSubCom mAE_CodSubCom)
        {
            if (ModelState.IsValid)
            {
                if (!reCaptchaHelper.IsCaptchaValid(Request.Form["foo"]))
                {
                    var response = "Error en el captcha, Intente nuevamente.";
                    return Content(response);
                }
                db.MAE_CodSubCom.Add(mAE_CodSubCom);
                await db.SaveChangesAsync();
                return Content("true");
            }
            else
            {
                return Content("Hubo un error al guardar los datos");
            }

        }

        // GET: Admin/CodSubCom/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MAE_CodSubCom mAE_CodSubCom = await db.MAE_CodSubCom.FindAsync(id);
            if (mAE_CodSubCom == null)
            {
                return HttpNotFound();
            }

            ViewBag.Titulo = "Editar Control Documentario";
            ViewBag.ClsModal = "hmodal-warning";
            ViewBag.Action = "Edit";
            //ViewBag.PathComunicacion = mAE_CodCom.MAE_Path.Path;

            if (ValidaPermisos.ValidaPermisosEnController("0010080002"))
            {
                return PartialView("_modalFormEdit", mAE_CodSubCom);
            }
            else
            {
                return RedirectToAction("SinPermiso", "../Pub");
            }

            
        }

        // POST: Admin/CodSubCom/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "IdControl,IdTipoSub,IdTipo,Activo,Obligatorio")] MAE_CodSubCom mAE_CodSubCom)
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
                    db.Entry(mAE_CodSubCom).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                    return Content("true");
                }
                else
                {
                    return Content("Error al guardar los datos");
                }
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }

        }

        // GET: Admin/CodSubCom/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MAE_CodSubCom mAE_CodSubCom = await db.MAE_CodSubCom.FindAsync(id);
            if (mAE_CodSubCom == null)
            {
                return HttpNotFound();
            }

            if (ValidaPermisos.ValidaPermisosEnController("0010080003"))
            {
                return PartialView("_Delete", mAE_CodSubCom);
            }
            else
            {
                return RedirectToAction("SinPermiso", "../Pub");
            }

            
        }

        // POST: Admin/CodSubCom/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {

            try
            {
                MAE_CodSubCom mAE_CodSubCom = await db.MAE_CodSubCom.FindAsync(id);
                db.MAE_CodSubCom.Remove(mAE_CodSubCom);
                await db.SaveChangesAsync();
                return Content("true");
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }

        }

        public async Task<ActionResult> GetSubtipo(int? IdCom)
        {
            List<MAE_SubtipoComunicacion> mAE_SubtipoComunicacion = new List<MAE_SubtipoComunicacion>();
            List<MAE_SubtipoComunicacion> listSubCom = await db.MAE_SubtipoComunicacion.Where(x => x.Activo && !x.Nombre.Equals("Comunicación General") && x.IdTipoCom == IdCom).ToListAsync();
            mAE_SubtipoComunicacion = listSubCom.ToList();
            ViewBag.IdTipoSub = new SelectList((from p in mAE_SubtipoComunicacion.ToList()
                                                select new
                                                {
                                                    IdTipoSub = p.IdTipoSub,
                                                    Nombre = p.Nombre
                                                }),
                                                       "IdTipoSub",
                                                       "Nombres");

            return PartialView("_GetSubtipo");
        }

        public async Task<ActionResult> GetTipoDoc(int? IdSub)
        {
            List<MAE_TipoDocumento> listSubCom = db.MAE_CodSubCom.Where(x => x.Activo && x.IdTipoSub == IdSub).Select(x => x.MAE_TipoDocumento).ToList();
            List<MAE_TipoDocumento> mAE_TipoDocumentos = new List<MAE_TipoDocumento>();
            List<MAE_TipoDocumento> auxList = await db.MAE_TipoDocumento.Where(x => x.Activo).ToListAsync();
            mAE_TipoDocumentos = auxList.Except(listSubCom).ToList();
            ViewBag.IdTipo = new SelectList((from p in mAE_TipoDocumentos.ToList()
                                             select new
                                             {
                                                 IdTipo = p.IdTipo,
                                                 Tipo = p.Tipo
                                             }),
                                                       "IdTipo",
                                                       "Tipo");

            return PartialView("_GetTipoDoc");
        }

        //// GET: Admin/CodSubCom/Details/5
        //public async Task<ActionResult> Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    MAE_CodSubCom mAE_CodSubCom = await db.MAE_CodSubCom.FindAsync(id);
        //    if (mAE_CodSubCom == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(mAE_CodSubCom);
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
