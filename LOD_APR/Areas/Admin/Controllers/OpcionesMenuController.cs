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
    public class OpcionesMenuController : Controller
    {
        private LOD_DB db = new LOD_DB();

        //Jg 30-12-2019
        public ActionResult Index(string Id)
        {
            MAE_modulos mAE_Modulos = db.MAE_modulos.Find(Id);
            ViewBag.NombreSistema = mAE_Modulos.MAE_sistema.Sistema;
            ViewBag.NombreModulo = mAE_Modulos.Modulo;
            ViewBag.IdSistema = mAE_Modulos.MAE_sistema.IdSistema;
            ViewBag.Id = Id;
            return View();
        }


        //Jg 30-12-2019
        public ActionResult getTable(string Id)
        {
            List<MAE_opcionesMenu> opciones = db.MAE_opcionesMenu.Where(x => x.IdModulo == Id).ToList();
            return PartialView("_getTable", opciones);
        }

        //JG 28-12-2019
        public ActionResult Create(string Id)
        {
            ViewBag.Titulo = "Nueva Opcion";
            ViewBag.ClsModal = "hmodal-success";
            ViewBag.Action = "Create";
            MAE_opcionesMenu opcionesMenu = new MAE_opcionesMenu();
            opcionesMenu.IdModulo = Id;
            ViewBag.NumObj = "00";
            if (db.MAE_opcionesMenu.Where(x => x.IdModulo == Id).Count() > 0)
            {
                opcionesMenu.Indice = db.MAE_opcionesMenu.Where(x => x.IdModulo == Id).Max(x => x.Indice) + 1;
            }
            else
            {
                opcionesMenu.Indice = 0;
            }




          
            ViewBag.DropDown = listOpciones();/*RemoveUsedId(list, Id); */
            return PartialView("_Edit", opcionesMenu);
        }



         public List<DropDown> listOpciones()
         {

             List<DropDown> list = new List<DropDown>();
            list.Add(new DropDown { label = "Ver", id = "00" });
            list.Add(new DropDown { label = "Crear", id = "01" });
            list.Add(new DropDown { label = "Editar", id = "02" });
            list.Add(new DropDown { label = "Eliminar", id = "03" });
            list.Add(new DropDown { label = "Imprimir", id = "04" });
            list.Add(new DropDown { label = "Cancelar", id = "05" });
            list.Add(new DropDown { label = "Archivar", id = "06" });
            list.Add(new DropDown { label = "Abrir", id = "07" });
            list.Add(new DropDown { label = "Ejecutar", id = "08" });
            list.Add(new DropDown { label = "Derivar", id = "09" });
            list.Add(new DropDown { label = "Subir Archivo", id = "10" });
            list.Add(new DropDown { label = "Bajar Archivo", id = "11" });
            list.Add(new DropDown { label = "Eliminar Archivo", id = "12" });
            return list;
         }



        //JG 28-12-2019
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IdOpcion,Opcion,Indice,IdModulo,NumObj")] MAE_opcionesMenu mAE_opcionesMenu)
        {
            try 
            {
                if (mAE_opcionesMenu.NumObj.Length==1)
                {
                    mAE_opcionesMenu.NumObj = "0" + mAE_opcionesMenu.NumObj;
                }
                mAE_opcionesMenu.IdOpcion = mAE_opcionesMenu.IdModulo + mAE_opcionesMenu.NumObj + mAE_opcionesMenu.IdOpcion;

                db.MAE_opcionesMenu.Add(mAE_opcionesMenu);
                db.SaveChanges();
                return Content("true");
            }
            catch (Exception ex)
            {
                if (db.MAE_opcionesMenu.Where(x=> x.IdOpcion == mAE_opcionesMenu.IdOpcion).Count() >0)
                {
                    return Content("Esta id ya existe.");
                }
                return Content(ex.Message);
            }
           
        }


        //JG 28-12-2019
        public ActionResult Edit(string id)
        {
            MAE_opcionesMenu mAE_opcionesMenu = db.MAE_opcionesMenu.Find(id);
            ViewBag.Titulo = "Editar Opcion";
            ViewBag.Action = "Edit";
            ViewBag.ClsModal = "hmodal-warning";
            ViewBag.IdAcc = mAE_opcionesMenu.IdOpcion.Substring(8, 2);
            ViewBag.NumObj = mAE_opcionesMenu.IdOpcion.Substring(6,2);
            ViewBag.DropDown = listOpciones();
            return PartialView("_Edit", mAE_opcionesMenu);
        }

        //JG 28-12-2019
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IdOpcion,Opcion,Indice,IdModulo,NumObj")] MAE_opcionesMenu mAE_opcionesMenu)
        {
            try
            {
                if (mAE_opcionesMenu.NumObj.Length == 1)
                {
                    mAE_opcionesMenu.NumObj = "0" + mAE_opcionesMenu.NumObj;
                }
                mAE_opcionesMenu.IdOpcion = mAE_opcionesMenu.IdModulo + mAE_opcionesMenu.NumObj + mAE_opcionesMenu.IdOpcion;
                db.Entry(mAE_opcionesMenu).State = EntityState.Modified;
                db.SaveChanges();
                return Content("true");
            }
            catch (Exception ex)
            {
                if (db.MAE_opcionesMenu.Where(x => x.IdOpcion == mAE_opcionesMenu.IdOpcion).Count() > 0)
                {
                    return Content("Esta id ya existe.");
                }
                return Content(ex.Message);

            }
        }

      

        //JG 28-12-2019
        public ActionResult Delete(string id)
        {
            MAE_opcionesMenu mAE_opcionesMenu = db.MAE_opcionesMenu.Find(id);
            return PartialView("_Delete", mAE_opcionesMenu);
        }


        //JG 28-12-2019
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {

            try
            {
                MAE_opcionesMenu mAE_opcionesMenu = db.MAE_opcionesMenu.Find(id);
                db.MAE_opcionesMenu.Remove(mAE_opcionesMenu);
                db.SaveChanges();
                return Content("true");
            }
            catch (Exception err)
            {
               
                return Content(err.Message);
            }
          
        }

        //JG 28-12-2019
        //Se utiliza para remover las ids ya usadas en las acciones dentro de los modulos para asi mostrar en el DropDownList
        //Solo las ids no utilizadas 
        public List<DropDown> RemoveUsedId(List<DropDown> list, string Id)
        {
        
            List<MAE_opcionesMenu> Acciones = db.MAE_opcionesMenu.Where(x => x.IdModulo == Id).ToList();
            string idstring = "";
            List<DropDown> listRemove = new List<DropDown>();
            foreach (MAE_opcionesMenu item in Acciones)
            {

                idstring = item.IdOpcion.Substring(6,2 );
                foreach (DropDown DropDown in list)
                {

                    if (DropDown.id.ToString() == idstring)
                    {
                        listRemove.Add(DropDown);
                    }
                }
            }


            foreach (DropDown DropDown in listRemove)
            {
                list.Remove(DropDown);
            }

            return list;
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
