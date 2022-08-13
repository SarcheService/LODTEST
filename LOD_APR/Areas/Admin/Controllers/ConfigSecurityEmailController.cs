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
    public class ConfigSecurityEmailController : Controller
    {
        private LOD_DB db = new LOD_DB();

        // GET: Admin/ConfigSecurityEmail
        public async Task<ActionResult> Index()
        {
            if (ValidaPermisos.ValidaPermisosEnController("0010180000"))
            {
                return View(await db.MAE_ConfigSecurityEmail.ToListAsync());
            }
            else
            {
                return RedirectToAction("SinPermiso", "../Pub");
            }

           
        }

        // GET: Admin/ConfigSecurityEmail/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MAE_ConfigSecurityEmail mAE_ConfigSecurityEmail = await db.MAE_ConfigSecurityEmail.FindAsync(id);
            if (mAE_ConfigSecurityEmail == null)
            {
                return HttpNotFound();
            }
            return View(mAE_ConfigSecurityEmail);
        }

        // GET: Admin/ConfigSecurityEmail/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/ConfigSecurityEmail/Create
        // Para protegerse de ataques de Publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "IdConfig,RequiredLength,RequireNonLetterOrDigit,RequireDigit,RequireLowercase,RequireUppercase,MaxFailedAccessAttemptsBeforeLockout,DefaultAccountLockoutTimeSpan")] MAE_ConfigSecurityEmail mAE_ConfigSecurityEmail)
        {
            if (ModelState.IsValid)
            {
                db.MAE_ConfigSecurityEmail.Add(mAE_ConfigSecurityEmail);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(mAE_ConfigSecurityEmail);
        }

        // GET: Admin/ConfigSecurityEmail/Edit/5
        public async Task<ActionResult> Edit()
        {
            try
            {
          
                if (db.MAE_ConfigSecurityEmail.Count() > 0)
                {  
                   MAE_ConfigSecurityEmail ConfigSecurityEmail = db.MAE_ConfigSecurityEmail.FirstOrDefault();

                    if (ValidaPermisos.ValidaPermisosEnController("0010180002"))
                    {
                        return View(ConfigSecurityEmail);
                    }
                    else
                    {
                        return RedirectToAction("SinPermiso", "../Pub");
                    }
                    
                }

                MAE_ConfigSecurityEmail configSecurityEmail = new MAE_ConfigSecurityEmail();
                if (ValidaPermisos.ValidaPermisosEnController("0010180002"))
                {
                    return View(configSecurityEmail);
                }
                else
                {
                    return RedirectToAction("SinPermiso", "../Pub");
                }
                
             
            }
            catch (Exception ex)
            {
                if (ValidaPermisos.ValidaPermisosEnController("0010180002"))
                {
                    return View();
                }
                else
                {
                    return RedirectToAction("SinPermiso", "../Pub");
                }

                
            }

       
        }

        // POST: Admin/ConfigSecurityEmail/Edit/5
        // Para protegerse de ataques de Publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "IdConfig,RequiredLength,RequireNonLetterOrDigit,RequireDigit,RequireLowercase,RequireUppercase,MaxFailedAccessAttemptsBeforeLockout,DefaultAccountLockoutTimeSpan")] MAE_ConfigSecurityEmail mAE_ConfigSecurityEmail)
        {

            if (mAE_ConfigSecurityEmail.IdConfig != 0)
            {
                db.Entry(mAE_ConfigSecurityEmail).State = EntityState.Modified;
                db.SaveChanges();
            }
            else
            {
                db.MAE_ConfigSecurityEmail.Add(mAE_ConfigSecurityEmail);
                db.SaveChanges();
            }
            return View(mAE_ConfigSecurityEmail);
        }

        // GET: Admin/ConfigSecurityEmail/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MAE_ConfigSecurityEmail mAE_ConfigSecurityEmail = await db.MAE_ConfigSecurityEmail.FindAsync(id);
            if (mAE_ConfigSecurityEmail == null)
            {
                return HttpNotFound();
            }
            return View(mAE_ConfigSecurityEmail);
        }

        // POST: Admin/ConfigSecurityEmail/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            MAE_ConfigSecurityEmail mAE_ConfigSecurityEmail = await db.MAE_ConfigSecurityEmail.FindAsync(id);
            db.MAE_ConfigSecurityEmail.Remove(mAE_ConfigSecurityEmail);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
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
