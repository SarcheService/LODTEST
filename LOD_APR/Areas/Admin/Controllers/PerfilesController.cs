using LOD_APR.Areas.Admin.Models;
using LOD_APR.Helpers;
using LOD_APR.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Galena.Areas.Admin.Controllers
{
    [CustomAuthorize]
    public class PerfilesController : Controller
    {
        private LOD_DB db = new LOD_DB();
        //se cambia dependiendo la aplicacion
        private int IdSistemaPerfil = 1;

        // GET: Admin/Perfil
        public ActionResult Index()
        {
            @ViewBag.App = "Administración";
            List<ApplicationRole> perfiles = getAllPerfiles();     
            return View(perfiles);
        }

        public ActionResult Create()
        {
            return View();
        }

        

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name")] ApplicationRole perfilModel)
        {
            if (ModelState.IsValid)
            {
                perfilModel.IdSistema = IdSistemaPerfil;
                db.Roles.Add(perfilModel);
                db.SaveChanges();
                return RedirectToAction("Edit", new { id = perfilModel.Id });
            }

            return View(perfilModel);
        }


        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }


            ApplicationRole perfilModel = db.Roles.Find(id);

            if (perfilModel == null)
            {
                return HttpNotFound();
            }

            
            if (perfilModel.IdSistema != IdSistemaPerfil)
            {
                @ViewBag.App = "Administración";
                List<ApplicationRole> perfiles = getAllPerfiles();
                
                return View("Index", perfiles);
            }



            //var ListSistLicencia = db.MAE_sistemaLicencia.Join(db.MAE_sistema,
            //                                                sl => sl.IdSistema,
            //                                                s => s.IdSistema,
            //                                                (sl, s) => new { sisLic = sl, sistema = s })
            //                                                .Where(x => x.sistema.IdSistema == IdSistemaPerfil)
            //                                                .Select(x => new { x.sistema.IdSistema, x.sistema.NombreSistema }).OrderBy(x => x.NombreSistema).ToList();


            //ViewBag.IdSistema = new SelectList(ListSistLicencia, "IdSistema", "NombreSistema", ListSistLicencia[0].IdSistema);

            return View(perfilModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Name")] IdentityRole perfilModel)
        {
            if (ModelState.IsValid)
            {
                db.Entry(perfilModel).State = EntityState.Modified;
                await db.SaveChangesAsync();
                //return RedirectToAction("Index");
            }
            //return View(perfilModel);
            return Content("true");
        }

        public ActionResult Delete(string id)
        {
            IdentityRole perfilModel = db.Roles.Find(id);
            //return PartialView("_Delete", perfilModel);
            return PartialView("~/Areas/Admin/Views/Perfil/_Delete.cshtml", perfilModel);

        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string id)
        {
            try
            {
                ApplicationRole perfilModel = db.Roles.Find(id);
                db.Roles.Remove(perfilModel);
                await db.SaveChangesAsync();
                return Content("true");
            }
            catch (Exception err)
            {
                return Content(err.Message);
            }
        }


        public ActionResult getPerfiles()
        {
            List<ApplicationRole> perfiles =  getAllPerfiles();

            return PartialView("~/Areas/Admin/Views/Perfil/_getTable.cshtml", perfiles);
        }

        public async Task<ActionResult> getOpciones(string idPerfil, int IdSistema)
        {

            List<MAE_opcionesMenu> opciones = await db.MAE_opcionesMenu.OrderBy(o => o.IdModulo).OrderBy(o => o.Indice).ToListAsync();
            List<MAE_modulos> modulos = await db.MAE_modulos.Where(x => x.IdSistema == IdSistema).OrderBy(o => o.indice).ToListAsync();
            List<MAE_OpcionesRoles> permisos = await db.MAE_OpcionesRoles.ToListAsync();
            ViewBag.Modulos = modulos;
            ViewBag.Permisos = permisos;
            ViewBag.IdPerfil = idPerfil;

            return PartialView("~/Areas/Admin/Views/Perfiles/_Opciones.cshtml", opciones);
        }

        [HttpPost]
        public async Task<ActionResult> savePermiso(string idPerfil, string idPermiso, bool isChecked)
        {
            try
            {
                if (isChecked)
                {
                    MAE_OpcionesRoles permiso = new MAE_OpcionesRoles() { IdOpcion = idPermiso, IdRol = idPerfil };
                    db.MAE_OpcionesRoles.Add(permiso);
                    await db.SaveChangesAsync();
                    return Content("true");
                }
                else
                {
                    MAE_OpcionesRoles permiso = await db.MAE_OpcionesRoles.Where(p => p.IdOpcion == idPermiso && p.IdRol == idPerfil).FirstAsync();
                    db.MAE_OpcionesRoles.Remove(permiso);
                    await db.SaveChangesAsync();
                    return Content("true");
                }
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
        }

        public List<ApplicationRole> getAllPerfiles()
        {
            List<ApplicationRole> perfiles = db.Roles.Where(x => x.IdSistema == IdSistemaPerfil).ToList();
            
            return perfiles;
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