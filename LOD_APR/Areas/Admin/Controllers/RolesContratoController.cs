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
    public class RolesContratoController : Controller
    {
        private LOD_DB db = new LOD_DB();

        // GET: Admin/RolesContrato
        public async Task<ActionResult> Index()
        {
            

            if (ValidaPermisos.ValidaPermisosEnController("0010190000"))
            {

                return View(await db.MAE_RolesContrato.ToListAsync());
            }
            else
            {
                return RedirectToAction("SinPermiso", "../Pub");
            }

        }

        public async Task<ActionResult> getTable()
        {
            return PartialView("_getTable", await db.MAE_RolesContrato.ToListAsync());
        }



        // GET: Admin/RolesContrato/Create
        public ActionResult Create()
        {
            ViewBag.Titulo = "Nuevo Rol de Contrato";
            ViewBag.ClsModal = "hmodal-success";
            ViewBag.Action = "Create";
            MAE_RolesContrato mAE_RolesContrato = new MAE_RolesContrato();

            ViewBag.IdTipoLod = new SelectList(db.MAE_TipoLOD, "IdTipoLod", "Nombre");

            if (ValidaPermisos.ValidaPermisosEnController("0010190001"))
            {
                return PartialView("_modalForm", mAE_RolesContrato);
            }
            else
            {
                return RedirectToAction("SinPermiso", "../Pub");
            }

            
        }

        // POST: Admin/RolesContrato/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "IdRolCtto,NombreRol,Descripcion,Activo,EsGubernamental,EsFiscalizador,EsContratista,IdTipoLod,Lectura,Escritura,FirmaGob,FirmaFea,FirmaSimple,Lectura1,Escritura1,FirmaGob1,FirmaFea1,FirmaSimple1,Lectura2,Escritura2,FirmaGob2,FirmaFea2,FirmaSimple2,Lectura3,Escritura3,FirmaGob3,FirmaFea3,FirmaSimple3,Lectura4,Escritura4,FirmaGob4,FirmaFea4,FirmaSimple4,RolGubernamental,RolFiscalizador,RolContratista")] MAE_RolesContrato mAE_RolesContrato)
        {

            try
            {
                if (ModelState.IsValid)
                {
                    mAE_RolesContrato.Jerarquia = 5;
                    db.MAE_RolesContrato.Add(mAE_RolesContrato);
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

        // GET: Admin/RolesContrato/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MAE_RolesContrato mAE_RolesContrato = await db.MAE_RolesContrato.FindAsync(id);
            if (mAE_RolesContrato == null)
            {
                return HttpNotFound();
            }
            ViewBag.Titulo = "Editar Rol de Contrato";
            ViewBag.ClsModal = "hmodal-warning";
            ViewBag.Action = "Edit";

            ViewBag.IdTipoLod = new SelectList(db.MAE_TipoLOD, "IdTipoLod", "Nombre",mAE_RolesContrato.IdTipoLod);


            if (ValidaPermisos.ValidaPermisosEnController("0010190002"))
            {
                return PartialView("_modalForm", mAE_RolesContrato);
            }
            else
            {
                return RedirectToAction("SinPermiso", "../Pub");
            }

        }

        // POST: Admin/RolesContrato/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "IdRolCtto,NombreRol,Descripcion,Activo,EsGubernamental,EsFiscalizador,EsContratista," +
            "IdTipoLod,Lectura,Escritura,FirmaGob,FirmaFea,FirmaSimple,Lectura1,Escritura1,FirmaGob1,FirmaFea1,FirmaSimple1," +
            "Lectura2,Escritura2,FirmaGob2,FirmaFea2,FirmaSimple2,Lectura3,Escritura3,FirmaGob3,FirmaFea3,FirmaSimple3," +
            "Lectura4,Escritura4,FirmaGob4,FirmaFea4,FirmaSimple4,RolGubernamental,RolFiscalizador,RolContratista")] MAE_RolesContrato mAE_RolesContrato)
        {

            if (ModelState.IsValid)
            {
                try
                {
                    db.Entry(mAE_RolesContrato).State = EntityState.Modified;
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

        // GET: Admin/RolesContrato/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MAE_RolesContrato mAE_RolesContrato = await db.MAE_RolesContrato.FindAsync(id);
            if (mAE_RolesContrato == null)
            {
                return HttpNotFound();
            }

            if (ValidaPermisos.ValidaPermisosEnController("0010190003"))
            {

                return PartialView("_Delete", mAE_RolesContrato);
            }
            else
            {
                return RedirectToAction("SinPermiso", "../Pub");
            }

        }

        // POST: Admin/RolesContrato/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            try
            {
                MAE_RolesContrato mAE_RolesContrato = await db.MAE_RolesContrato.FindAsync(id);
                db.MAE_RolesContrato.Remove(mAE_RolesContrato);
                await db.SaveChangesAsync();
                return Content("true");
            }
            catch (Exception err)
            {
                return Content(err.Message);
            }
            
        }

        // GET: Admin/RolesContrato/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MAE_RolesContrato mAE_RolesContrato = await db.MAE_RolesContrato.FindAsync(id);
            if (mAE_RolesContrato == null)
            {
                return HttpNotFound();
            }
            return View(mAE_RolesContrato);
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
