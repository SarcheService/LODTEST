using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using LOD_APR.Areas.GLOD.Models;
using LOD_APR.Models;
using Microsoft.AspNet.Identity;
using LOD_APR.Helpers;

namespace LOD_APR.Areas.GLOD.Controllers
{
    [CustomAuthorize]
    public class CarpetasController : Controller
    {
        private LOD_DB db = new LOD_DB();
        private GoogleReCaptchaHelper reCaptchaHelper = new GoogleReCaptchaHelper();
        // GET: ASP/Carpetas
        public async Task<ActionResult> Index()
        {
            var lod_carpetas = db.LOD_Carpetas;
            return View(await lod_carpetas.ToListAsync());
        }

        //ER OK
        // GET: ASP/Carpetas/Details/5
        public ActionResult Details(int id)
        {
            LOD_Carpetas lod_carpetas = db.LOD_Carpetas.Find(id);
            ViewBag.TipoMenu = 1;
            int IdContrato = 0;
            ViewBag.EsInspectorFiscal = false;
            if (lod_carpetas.IdCarpPadre != null)
            {
                LOD_Carpetas Carp = db.LOD_Carpetas.Find(lod_carpetas.IdCarpPadre);
                ViewBag.TipoMenu = BuscarDependencia(Carp);
            }
            else if (lod_carpetas.IdContrato != null)
            {
                ViewBag.TipoMenu = 2;
                IdContrato = lod_carpetas.IdContrato.Value;

                ViewBag.EsInspectorFiscal = false;
                string userId = User.Identity.GetUserId();
                List<LOD_RolesCttosContrato> rolesUsuario = db.LOD_UsuariosLod.Where(x => x.UserId == userId && x.LOD_LibroObras.IdContrato == IdContrato && x.Activo).Select(x => x.LOD_RolesCttosContrato).ToList();
                if (rolesUsuario.Select(x => x.IdRolCtto).Contains(2)) //2 es el rol Inspector Fiscal en la Tabla MAE_RolesContratos
                    ViewBag.EsInspectorFiscal = true;
            }

            

            //Thread.Sleep(1000);
            lod_carpetas.Creador = db.Users.Find(lod_carpetas.UserId).Nombres +" "+ db.Users.Find(lod_carpetas.UserId).Apellidos;
            return PartialView("_getDetailsCarpeta", lod_carpetas);


        }


        //ER OK
        public int BuscarDependencia(LOD_Carpetas carpeta)
        {
            int TipoMenu = 1;
            if (carpeta.IdCarpPadre != null)
            {
                LOD_Carpetas Carp = db.LOD_Carpetas.Find(carpeta.IdCarpPadre);
                TipoMenu = BuscarDependencia(Carp);
            }
            else if (carpeta.IdContrato != null)
            {
                return TipoMenu = 2;
            }
            else
            {
                return TipoMenu;
            }
            return TipoMenu;
        }


        // GET: ASP/Carpetas/Create
        public ActionResult Create(int? Padre, string Tipo)
        {
            ViewBag.Titulo = "Nueva Carpeta";
            ViewBag.ClsModal = "success";
            ViewBag.Action = "Create";
            LOD_Carpetas carpeta = new LOD_Carpetas();
            if (Tipo == "con")
            {
                carpeta.IdContrato = Convert.ToInt32(Padre);
            }
            else if(Tipo=="carp")
            {
                carpeta.IdCarpPadre = Padre;
            }

            ViewBag.Tipo = Tipo;

            if (ValidaPermisos.ValidaPermisosEnController("0020000101"))
            {
                return PartialView("_modalForm", carpeta);
            }
            else
            {
                return RedirectToAction("SinPermiso", "../Pub");
            }

        }

        // POST: ASP/Carpetas/Create  
        //ER OK
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "IdCarpeta,IdContrato,NombreCarpeta,IdCarpPadre, EsPortafolio")] LOD_Carpetas aSP_carpetas)
        {
            try
            {
                if (!reCaptchaHelper.IsCaptchaValid(Request.Form["foo"]))
                {
                    var response = "Error en el captcha, Intente nuevamente.";
                    return Content(response);
                }
                aSP_carpetas.UserId = User.Identity.GetUserId();
                aSP_carpetas.FechaCreacion = DateTime.Now;
                db.LOD_Carpetas.Add(aSP_carpetas);
                await db.SaveChangesAsync();
                string content = Convert.ToString(db.LOD_Carpetas.Max(x => x.IdCarpeta));
                return Content("true;" + content);
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
        }

        // GET: ASP/Carpetas/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            ViewBag.Titulo = "Editar Carpeta";
            ViewBag.ClsModal = "warning";
            ViewBag.Action = "Edit";
            LOD_Carpetas carpeta = db.LOD_Carpetas.Find(id);
            ViewBag.Tipo = "ok";
            if (carpeta.EsPortafolio)
            {
                ViewBag.Tipo = null;
            }
            else if ((carpeta.IdCarpPadre == null) && (carpeta.IdContrato == null))
            {
                ViewBag.Tipo = null;
            }

            return PartialView("_modalForm", carpeta);
        }

        //ER OK
        // POST: ASP/Carpetas/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(LOD_Carpetas aSP_carpetas)
        {
            try
            {
                //ver la opción de colocar unlog carpeta
                //aSP_carpetas.IdUsuario = 1;//Por mientras que no existen Usuarios
                //aSP_carpetas.FechaCreacion = DateTime.Now;
                if (!reCaptchaHelper.IsCaptchaValid(Request.Form["foo"]))
                {
                    var response = "Error en el captcha, Intente nuevamente.";
                    return Content(response);
                }
                db.Entry(aSP_carpetas).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return Content("true;" + aSP_carpetas.IdCarpeta.ToString());
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
        }

        // GET: ASP/Carpetas/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            LOD_Carpetas carpeta = await db.LOD_Carpetas.FindAsync(id);

            if (ValidaPermisos.ValidaPermisosEnController("0020000103"))
            {
                return PartialView("_Delete", carpeta);
            }
            else
            {
                return RedirectToAction("SinPermiso", "../Pub");
            }

        }

        // POST: ASP/Carpetas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            try
            {
                LOD_Carpetas carpeta = await db.LOD_Carpetas.FindAsync(id);
                //obtener Padre
                string padre = string.Empty;
                if (carpeta.IdCarpPadre != null)
                {
                    padre = "f_" + carpeta.IdCarpPadre.ToString();
                }
                else if (carpeta.IdContrato != null)
                {
                    padre = "c_" + carpeta.IdContrato.ToString();
                }
                else
                {
                    padre = "t_0";
                }

                db.LOD_Carpetas.Remove(carpeta);
                await db.SaveChangesAsync();
                return Content("delete;" + padre);
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
