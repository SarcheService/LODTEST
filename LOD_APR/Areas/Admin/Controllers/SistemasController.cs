using LOD_APR.Areas.Admin.Models;
using LOD_APR.Helpers;
using LOD_APR.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace LOD_APR.Areas.Admin.Controllers
{
    [CustomAuthorize]
    public class SistemasController : Controller
    {
        private LOD_DB db = new LOD_DB();

        // GET: Admin/Sistemas
        public ActionResult Index()
        {


            //List <MAE_OpcionesRoles> mAE_OpcionesRoles  = db.MAE_OpcionesRoles.Where(x => x.IdOpcion.Substring(0,3) == "011").ToList();

            //foreach (var item in mAE_OpcionesRoles)
            //{
            //    db.MAE_OpcionesRoles.Remove(item);
            //    db.SaveChanges();
            //}

            //MAE_sistema mAE_Sistema = db.MAE_sistema.Find(11);
            //foreach (var modulo in mAE_Sistema.MAE_modulos)
            //{

            //    foreach (var op in modulo.MAE_opcionesMenu)
            //    {

            //        db.MAE_opcionesMenu.Remove(op);
            //        db.SaveChanges();



            //    }

            //}



            return View(/*db.MAE_sistema.ToList()*/);
        }

        //Jg 28-12-2019
        public ActionResult getTable()
        {
            List<MAE_sistema> sistemas = new List<MAE_sistema>();
            sistemas = db.MAE_sistema.ToList();
            return PartialView("_getTable", sistemas);
        }

// Jg 28-12-2019
        public ActionResult Create()
        {
            ViewBag.Titulo = "Nuevo Sistema";
            ViewBag.ClsModal = "hmodal-success";
            ViewBag.Action = "Create";
            MAE_sistema sistema = new MAE_sistema();

            ViewBag.Sistemas = new SelectList(db.MAE_sistema,"IdSistema","Sistema");

            return PartialView("_Edit", sistema);
        }
        // Jg 28-12-2019
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(MAE_sistema mAE_sistema)
        {
            try
            {
                // El IdModulo debe ser modificado a String para el correlativo 00x
                db.MAE_sistema.Add(mAE_sistema);
                db.SaveChanges();
                return Content("true");
            }
            catch (Exception ex )
            {

                return Content( ex.Message);
            }
            
          
        }


      //Jg 28-12-2019
        public ActionResult Edit(int Id)
        {

            MAE_sistema mAE_sistema = db.MAE_sistema.Find(Id);
            ViewBag.IdSistema = new SelectList(db.MAE_sistema.Where(x=> x.IdSistema != Id), "IdSistema", "Sistema");
            ViewBag.Sistemas = new SelectList(db.MAE_sistema, "IdSistema", "Sistema");
            ViewBag.Titulo = "Editar Sistema";
            ViewBag.Action = "Edit";
            ViewBag.ClsModal = "hmodal-warning";
            return PartialView("_Edit", mAE_sistema);
        }

        //Jg 28-12-2019
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(MAE_sistema mAE_sistema)
        {

            try
            {
               
                    db.Entry(mAE_sistema).State = EntityState.Modified;
                    db.SaveChanges();
                    return Content("true");
                
            }
            catch (Exception ex)
            {
                return View(ex.Message);

            }
            
           
        }
        // Jg 28-12-2019
        public ActionResult Delete(int Id)
        {
            MAE_sistema model = db.MAE_sistema.Find(Id);
            return PartialView("_Delete", model);

        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {

            try
            {
                MAE_sistema mAE_sistema = db.MAE_sistema.Find(id);
                db.MAE_sistema.Remove(mAE_sistema);
                db.SaveChanges();
                return Content("true");
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
