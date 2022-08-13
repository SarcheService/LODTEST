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
    public class ModulosController : Controller
    {
        private LOD_DB db = new LOD_DB();

        //Jg 28-12-2019
        public ActionResult Index(int Id)
        {
            ViewBag.NombreSistema = db.MAE_sistema.Find(Id).NombreSistema;
            ViewBag.Id=Id;
            return View();
        }


        //Jg 28-12-2019
        public ActionResult getTable(int Id)
        {
            List<MAE_modulos> modulos = new List<MAE_modulos>();
            modulos = db.MAE_modulos.Where(x => x.IdSistema == Id).ToList();
            return PartialView("_getTable", modulos);
        }

        //JG 28-12-2019
        public ActionResult Create(int Id)
        {
            ViewBag.Titulo = "Nuevo Modulo";
            ViewBag.ClsModal = "hmodal-success";
            ViewBag.Action = "Create";
            MAE_modulos modulo = new MAE_modulos();
            if (db.MAE_modulos.Where(x => x.IdSistema == Id).Count() >0)
            {
                modulo.indice = db.MAE_modulos.Where(x => x.IdSistema == Id).Max(x => x.indice)+1;
            }
            else
            {
                modulo.indice = 1;
            }
           
            modulo.IdSistema = Id;
           
            return PartialView("_Edit",modulo);
        }
        //JG 28-12-2019
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create( MAE_modulos mAE_modulos)
        {
            try
            {
                string CorrelativoMaxString = "";
                if (db.MAE_modulos.Where(x => x.IdSistema == mAE_modulos.IdSistema).Count() >0)
                {
                    CorrelativoMaxString = db.MAE_modulos.Where(x => x.IdSistema == mAE_modulos.IdSistema).Max(x => x.IdModulo);
                    int CorrelativoMaxInt = Convert.ToInt32(CorrelativoMaxString) + 1;
                    CorrelativoMaxString = Convert.ToString(CorrelativoMaxInt);
                    int Length = CorrelativoMaxString.Length - 6;
                    if (Length < 0)
                    {
                        for (int i = 0; i < Length * -1; i++)
                        {
                            CorrelativoMaxString = "0" + CorrelativoMaxString;
                        }
                    }
                }
                else
                {
                    int CorrelativoSistema = db.MAE_sistema.Find(mAE_modulos.IdSistema).IdSistema;
                    CorrelativoMaxString = CorrelativoSistema.ToString();
                    for (int i = 0; i < (CorrelativoSistema.ToString().Length -3)*-1; i++)
                    {
                        CorrelativoMaxString = "0" + CorrelativoMaxString;
                    }
                    CorrelativoMaxString = CorrelativoMaxString + "000";
                }
                mAE_modulos.IdModulo = CorrelativoMaxString;
                db.MAE_modulos.Add(mAE_modulos);
                db.SaveChanges();
                return Content("true");
            }
            catch (Exception ex)
            {

                return Content(ex.Message);
            }
        }

        //JG 28-12-2019
        public ActionResult Edit(string id)
        {
            MAE_modulos mAE_modulos = db.MAE_modulos.Find(id);
            ViewBag.Titulo = "Editar Modulo";
            ViewBag.Action = "Edit";
            ViewBag.ClsModal = "hmodal-warning";
            return PartialView("_Edit", mAE_modulos);
        }

        //JG 28-12-2019
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(MAE_modulos mAE_modulos)
        {

            try
            {

                //Falta logica. ELIMINAR Y INSERTAR NUEVOS

                List<MAE_ParametrosModulo> OldParametros = db.MAE_ParametrosModulo.AsNoTracking().Where(x => x.IdModulo == mAE_modulos.IdModulo).ToList();
                
                List<MAE_ParametrosModulo> NewParametros = (mAE_modulos.MAE_ParametrosModulo != null)?OldParametros.Except(mAE_modulos.MAE_ParametrosModulo).ToList():null;

                db.Entry(mAE_modulos).State = EntityState.Modified;
              
                db.SaveChanges();
                return Content("true");
            }
            catch (Exception ex)
            {
                return Content(ex.Message);

            }
        }

      
        public ActionResult Delete(string id)
        {
            MAE_modulos mAE_modulos = db.MAE_modulos.Find(id);
            return PartialView("_Delete", mAE_modulos);
        }

       //JG 28-12-2019
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {

            try
            {
                MAE_modulos mAE_modulos = db.MAE_modulos.Find(id);
                db.MAE_modulos.Remove(mAE_modulos);
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
