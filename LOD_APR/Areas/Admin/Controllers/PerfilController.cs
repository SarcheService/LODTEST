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

namespace LOD_APR.Areas.Admin.Controllers
{
    [CustomAuthorize]
    public class PerfilController : Controller
    {
        private LOD_DB db = new LOD_DB();

        //private ApplicationRoleManager _roleManager;


        //public PerfilController(ApplicationRoleManager roleManager)
        //{
        //    RoleManager = roleManager;
        //}

        //public ApplicationRoleManager RoleManager
        //{
        //    get
        //    {
        //        return _roleManager ?? HttpContext.GetOwinContext().Get<ApplicationRoleManager>();
        //    }
        //    private set
        //    {
        //        _roleManager = value;
        //    }
        //}




        // GET: Admin/Perfil
        public ActionResult Index()
        {

            List<ApplicationUser> users = db.Users.ToList();
            //var roles = db.Roles.Include(r => r.Users);


            List<ApplicationRole> perfiles = getAllPerfiles();

            if (ValidaPermisos.ValidaPermisosEnController("0010020000"))
            {
                return View(perfiles);
            }
            else
            {
                return RedirectToAction("SinPermiso", "../Pub");
            }

            
        }

        public ActionResult Create()
        {
            if (ValidaPermisos.ValidaPermisosEnController("0010020001"))
            {
                return View();
            }
            else
            {
                return RedirectToAction("SinPermiso", "../Pub");
            }

            
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name")] ApplicationRole perfilModel)
        {
            if (ModelState.IsValid)
            {

                db.Roles.Add(perfilModel);
                db.SaveChanges();
                return RedirectToAction("Edit", new { id = perfilModel.Id });
            }


            return View(perfilModel);
        }


        public ActionResult Edit(string id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            ApplicationRole perfilModel = db.Roles.Find(id);
            if (perfilModel == null)
                return HttpNotFound();


            //if (perfilModel.MAE_sistema != null)
            //{
            //    int? IdSistema = db.Roles.Find(perfilModel.MAE_sistema.IdSistema).IdSistema;
            //    ViewBag.IdSistema = new SelectList(db.MAE_sistema.ToList(), "IdSistema", "NombreSistema", IdSistema);
            //}
            //else
            //{
            //    ViewBag.IdSistema = new SelectList(db.MAE_sistema.ToList(), "IdSistema", "NombreSistema");
            //}

            //ViewBag.IdApp = perfilModel.IdSistema;

            if (ValidaPermisos.ValidaPermisosEnController("0010020002"))
            {
                return View(perfilModel);
            }
            else
            {
                return RedirectToAction("SinPermiso", "../Pub");
            }

            
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

            if (ValidaPermisos.ValidaPermisosEnController("0010020003"))
            {
                return PartialView("~/Areas/Admin/Views/Perfil/_Delete.cshtml", perfilModel);
            }
            else
            {
                return RedirectToAction("SinPermiso", "../Pub");
            }

            

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
            List<ApplicationRole> perfiles = getAllPerfiles();

            return PartialView("~/Areas/Admin/Views/Perfil/_getTable.cshtml", perfiles);
        }

        public async Task<ActionResult> getOpciones(string idPerfil)
        {

            List<MAE_opcionesMenu> opciones = await db.MAE_opcionesMenu.OrderBy(o => o.IdModulo).OrderBy(o => o.Indice).ToListAsync();
            List<MAE_modulos> modulos = await db.MAE_modulos.OrderBy(o => o.indice).ToListAsync();
            List<MAE_OpcionesRoles> permisos = await db.MAE_OpcionesRoles.ToListAsync();
            ViewBag.Modulos = modulos.OrderByDescending(x => x.IdSistema).ThenBy(x => x.Modulo);
            ViewBag.Permisos = permisos;
            ViewBag.IdPerfil = idPerfil;


            return PartialView("~/Areas/Admin/Views/Perfil/_Opciones.cshtml", opciones.OrderByDescending(x => x.MAE_modulos.IdSistema).ThenBy(x => x.Opcion));

        }

        public async Task<ActionResult> getTipo(string idPerfil)
        {

            ApplicationRole rol = await db.Roles.Where(x => x.Id == idPerfil).FirstOrDefaultAsync();
            ViewBag.IdPerfil = idPerfil;


            return PartialView("~/Areas/Admin/Views/Perfil/_Tipos.cshtml", rol);

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

        [HttpPost]
        public async Task<ActionResult> savePermisoTipo(string idPerfil, string tipo, bool isChecked)
        {
            try
            {
                ApplicationRole rol = db.Roles.Where(x => x.Id == idPerfil).FirstOrDefault();
                if (tipo == "IsGoburnamental")
                    rol.IsGubernamental = isChecked;
                else if (tipo == "IsFiscalizador")
                    rol.IsFiscalizador = isChecked;
                else if (tipo == "IsContratista")
                    rol.IsContratista = isChecked;

                db.Entry(rol).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return Content("true");

            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
        }

        public List<ApplicationRole> getAllPerfiles()
        {
            List<ApplicationRole> perfiles = db.Roles.ToList();

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