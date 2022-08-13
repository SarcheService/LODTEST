using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web.Mvc;
using LOD_APR.Models;
using LOD_APR.Areas.Admin.Models;
using LOD_APR.Helpers;

namespace LOD_APR.Areas.Admin.Controllers
{
    [CustomAuthorize]
    public class ConfigEmailController : Controller
    {
        private LOD_DB db = new LOD_DB();

        // GET: Admin/ConfigEmail
        public async Task<ActionResult> Edit()
        {
            MAE_ConfigEmail configEmail = new MAE_ConfigEmail();
            if (db.MAE_ConfigEmail.Count() > 0)
            {
                configEmail = db.MAE_ConfigEmail.FirstOrDefault();
                ViewBag.IdTCMail = new SelectList(db.MAE_tipoCtaEmails, "IdTCMail", "NombreTCE", configEmail.IdTCMail);
            }
            else
            {
                ViewBag.IdTCMail = new SelectList(db.MAE_tipoCtaEmails, "IdTCMail", "NombreTCE");
            }

            if (ValidaPermisos.ValidaPermisosEnController("0010140002"))
            {

                return View(configEmail);
            }
            else
            {
                return RedirectToAction("SinPermiso", "../Pub");
            }

            
        }

        [HttpPost]
        public async Task<ActionResult> Edit(MAE_ConfigEmail ConfigEmail)
        {
            
            if (ConfigEmail.IdConfig != 0)
            {
                db.Entry(ConfigEmail).State = EntityState.Modified;
                db.SaveChanges();
            }
            else
            {
                db.MAE_ConfigEmail.Add(ConfigEmail);
                db.SaveChanges();
            }
            ViewBag.IdTCMail = new SelectList(db.MAE_tipoCtaEmails, "IdTCMail", "NombreTCE", ConfigEmail.IdTCMail);

            if (ValidaPermisos.ValidaPermisosEnController("0010140002"))
            {

                return View(ConfigEmail);
            }
            else
            {
                return RedirectToAction("SinPermiso", "../Pub");
            }


            
        }




        // GET: Admin/ConfigEmail/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MAE_ConfigEmail mAE_ConfigEmail = await db.MAE_ConfigEmail.FindAsync(id);
            if (mAE_ConfigEmail == null)
            {
                return HttpNotFound();
            }
            return View(mAE_ConfigEmail);
        }

        // GET: Admin/ConfigEmail/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/ConfigEmail/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "IdConfig,Email,PassEmail,PuertoSalida,UriServer,IsActive,IsSSL")] MAE_ConfigEmail mAE_ConfigEmail)
        {
            if (ModelState.IsValid)
            {
                db.MAE_ConfigEmail.Add(mAE_ConfigEmail);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(mAE_ConfigEmail);
        }

        // GET: Admin/ConfigEmail/Edit/5
      

        // GET: Admin/ConfigEmail/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MAE_ConfigEmail mAE_ConfigEmail = await db.MAE_ConfigEmail.FindAsync(id);
            if (mAE_ConfigEmail == null)
            {
                return HttpNotFound();
            }
            return View(mAE_ConfigEmail);
        }

        // POST: Admin/ConfigEmail/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            MAE_ConfigEmail mAE_ConfigEmail = await db.MAE_ConfigEmail.FindAsync(id);
            db.MAE_ConfigEmail.Remove(mAE_ConfigEmail);
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
