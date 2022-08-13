using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using LOD_APR.Areas.Admin.Models;
using LOD_APR.Models;
using LOD_APR.Areas.GLOD.Models;
using LOD_APR.Areas.GLOD.ModelsViews;
using System;
using LOD_APR.Helpers;

namespace LOD_APR.Areas.GLOD.Controllers
{
    [CustomAuthorize]
    public class RepositorioController : Controller
    {
        private LOD_DB db = new LOD_DB();

        // GET: Admin/SeguimientoCom
        public async Task<ActionResult> Index()
        {
            List<CON_Contratos> list_Contratos = await db.CON_Contratos.ToListAsync();
            IndexCodView indexView = new IndexCodView();
            List<ItemIndexCod> listaAux = new List<ItemIndexCod>();
            foreach (var item in list_Contratos)
            { 
                List<LOD_LibroObras> list_Libros = await db.LOD_LibroObras.Where(x => x.IdContrato == item.IdContrato).ToListAsync();
                List<int> list_intLibros = list_Libros.Select(x => x.IdLod).ToList();

                List<LOD_Anotaciones> list_Anotaciones = await db.LOD_Anotaciones.Where(x => list_intLibros.Contains(x.IdLod)).ToListAsync();
                foreach (var com in list_Anotaciones)
                {
                    //listado de control documentario según anotaciones del libro 
                    List<MAE_CodSubCom> list_CodSubCom = list_CodSubCom = db.MAE_CodSubCom.Where(x => x.IdTipoSub == com.IdTipoSub).ToList();

                    List<MAE_TipoDocumento> list_TipoDocSub = new List<MAE_TipoDocumento>();
                    foreach (var cod in list_CodSubCom)
                    {
                        MAE_TipoDocumento auxTipo = db.LOD_docAnotacion.Where(x => x.IdAnotacion == com.IdAnotacion && x.LOD_Anotaciones.IdTipoSub == com.IdTipoSub && x.IdTipoDoc == cod.IdTipo).Select(x => x.MAE_TipoDocumento).FirstOrDefault();
                        if(auxTipo != null)
                            list_TipoDocSub.Add(auxTipo);
                    }
                    ItemIndexCod item_index = new ItemIndexCod();
                    int estado = 1;
                    if (list_TipoDocSub.Count() < list_CodSubCom.Count())
                    {
                        estado = 2;
                        item_index.CON_Contratos = com.LOD_LibroObras.CON_Contratos;
                        item_index.Estado = estado;
                        if (!listaAux.Contains(item_index))
                            listaAux.Add(item_index);
                        break;
                    }


                    item_index.CON_Contratos = com.LOD_LibroObras.CON_Contratos;
                    item_index.Estado = estado;
                    if (!listaAux.Contains(item_index))
                        listaAux.Add(item_index);

                }
                if(list_Anotaciones.Count() == 0)
                {
                    ItemIndexCod item_index = new ItemIndexCod();
                    item_index.CON_Contratos = item;
                    item_index.Estado = 3;
                    listaAux.Add(item_index);
                }
                
            }
            indexView.list_Item = listaAux;

            if (ValidaPermisos.ValidaPermisosEnController("0020050000"))
            {

                return View(indexView);
            }
            else
            {
                return RedirectToAction("SinPermiso", "../Pub");
            }

            
        }

        public async Task<ActionResult> SeguimientoLibros(int id)
        {
            CON_Contratos cON_Contratos = await db.CON_Contratos.FindAsync(id);
            List<LOD_LibroObras> list_Libros = await db.LOD_LibroObras.Where(x => x.IdContrato == cON_Contratos.IdContrato).ToListAsync();
            SeguimientoView seguimientoView = new SeguimientoView();
            seguimientoView.Contrato = cON_Contratos;
            seguimientoView.lista_Libros = list_Libros;
            seguimientoView.lista_Items = new List<ItemSeguimiento>();
            seguimientoView.OtrosDocumentos = db.LOD_docAnotacion.Where(x => x.LOD_Anotaciones.LOD_LibroObras.IdContrato == cON_Contratos.IdContrato && x.IdTipoDoc == 19).ToList();
            foreach (var item in seguimientoView.OtrosDocumentos)
            {
                item.ClassDoc = db.MAE_ClassDoc.Where(x => x.IdTipoSub == item.LOD_Anotaciones.IdTipoSub).FirstOrDefault();
            }
            
            foreach (var item in list_Libros)
            {
                List<LOD_Anotaciones> listadoTotal = new List<LOD_Anotaciones>();
                List<LOD_Anotaciones> listAnotCargados = await db.LOD_Anotaciones.Where(x => x.IdLod == item.IdLod).ToListAsync();

                List<int> listCom = await db.MAE_TipoComunicacion.Where(x => x.IdTipoLod == item.IdTipoLod).Select(x => x.IdTipoCom).ToListAsync();
                List<MAE_SubtipoComunicacion> listadoSubtipo = await db.MAE_SubtipoComunicacion.Where(x => listCom.Contains(x.IdTipoCom)).ToListAsync();
                listadoSubtipo = listadoSubtipo.Except(listAnotCargados.Select(x => x.MAE_SubtipoComunicacion)).ToList();
                foreach (var subtipo in listadoSubtipo)
                {
                    LOD_Anotaciones lodAux = new LOD_Anotaciones();
                    lodAux.IdAnotacion = 0;
                    lodAux.IdLod = item.IdLod;
                    lodAux.IdTipoSub = subtipo.IdTipoSub;
                    lodAux.MAE_SubtipoComunicacion = subtipo;
                    listadoTotal.Add(lodAux);
                }

                listadoTotal.AddRange(listAnotCargados);

                foreach (var anot in listadoTotal)
                {
                    List<MAE_CodSubCom> listado_Cod = await db.MAE_CodSubCom.Where(x => x.IdTipoSub == anot.IdTipoSub && x.IdTipo != 19).ToListAsync();
                    List<LOD_docAnotacion> listado_Doc = await db.LOD_docAnotacion.Where(x => x.IdAnotacion == anot.IdAnotacion).ToListAsync();
                    foreach (var cod in listado_Cod) 
                    {
                        ItemSeguimiento itemSeguimiento = new ItemSeguimiento();
                        itemSeguimiento.IdLod = item.IdLod;
                        itemSeguimiento.mAE_CodSubCom = cod;
                        List<LOD_docAnotacion> lOD_DocAnotacion = listado_Doc.Where(x => x.IdTipoDoc == cod.IdTipo && x.LOD_Anotaciones.IdTipoSub == cod.IdTipoSub && x.LOD_Anotaciones.Estado == 2).ToList(); //ESTADO = 2
                        if (lOD_DocAnotacion.Count > 0)
                        {
                            itemSeguimiento.totalDoc = lOD_DocAnotacion.Count;
                            itemSeguimiento.Estado = 1;
                        }
                        else
                        {
                            itemSeguimiento.totalDoc = 0;
                            itemSeguimiento.Estado = 2;
                        }
                        MAE_ClassDoc mAE_ClassDoc = db.MAE_ClassDoc.Where(x => x.IdTipo == cod.IdTipo && x.IdTipoSub == cod.IdTipoSub).FirstOrDefault();
                        itemSeguimiento.mAE_ClassDoc = mAE_ClassDoc;

                        if (cod.Obligatorio == true && itemSeguimiento.Estado == 2)
                            itemSeguimiento.Alerta = true;
                        else
                            itemSeguimiento.Alerta = false;


                        if (seguimientoView.lista_Items.Select(x => x.mAE_CodSubCom.IdTipo).ToList().Contains(itemSeguimiento.mAE_CodSubCom.IdTipo))
                        {
                            if(itemSeguimiento.totalDoc > 0)
                            {
                                int auxCount = seguimientoView.lista_Items.Where(x => x.mAE_CodSubCom.IdTipo == itemSeguimiento.mAE_CodSubCom.IdTipo).FirstOrDefault().totalDoc;
                                seguimientoView.lista_Items.Remove(seguimientoView.lista_Items.Where(x => x.mAE_CodSubCom.IdTipo == itemSeguimiento.mAE_CodSubCom.IdTipo).FirstOrDefault());
                                itemSeguimiento.totalDoc = itemSeguimiento.totalDoc + auxCount;
                                seguimientoView.lista_Items.Add(itemSeguimiento);
                            }
                        }
                        else
                        {
                            seguimientoView.lista_Items.Add(itemSeguimiento);
                        }
                        
                    }
                }
            }

            return View(seguimientoView);
        }

        public async Task<ActionResult> detalleAnotacion(int id, int idLibro)
        {
            LOD_LibroObras libro = db.LOD_LibroObras.Find(idLibro);
            List<LOD_docAnotacion> listadoDocumentos = db.LOD_docAnotacion.Where(x => x.IdTipoDoc == id && x.LOD_Anotaciones.IdLod == libro.IdLod && x.LOD_Anotaciones.Estado == 2).ToList();
            foreach (var item in listadoDocumentos)
            {
                item.ClassDoc = db.MAE_ClassDoc.Where(x => x.IdTipoSub == item.LOD_Anotaciones.IdTipoSub).FirstOrDefault();
            }
            return PartialView("_detalleDocAnotacion", listadoDocumentos);
        }

        public async Task<ActionResult> detalleAnotacionOtros(int id, int idLibro)
        {
            LOD_LibroObras libro = db.LOD_LibroObras.Find(idLibro);
            List<LOD_docAnotacion> listadoDocumentos = db.LOD_docAnotacion.Where(x => x.IdTipoDoc == id && x.LOD_Anotaciones.IdLod == libro.IdLod && x.LOD_Anotaciones.Estado == 2).ToList();
            foreach (var item in listadoDocumentos)
            {
                item.ClassDoc = db.MAE_ClassDoc.Where(x => x.IdTipoSub == item.LOD_Anotaciones.IdTipoSub).FirstOrDefault();
            }

            return PartialView("_detalleDocAnotacion", listadoDocumentos);
        }

        public async Task<ActionResult> AprobarDoc(int id)
        {
            try
            {
                LOD_docAnotacion lOD_DocAnotacion = db.LOD_docAnotacion.Find(id);
                lOD_DocAnotacion.EstadoDoc = 1;
                db.Entry(lOD_DocAnotacion).State = EntityState.Modified;
                db.SaveChanges();


                return Content("true");
            }catch(Exception ex)
            {
                return Content(ex.Message);
            }
        }

        public async Task<ActionResult> RechazarDoc(int id)
        {
            try
            {
                LOD_docAnotacion lOD_DocAnotacion = db.LOD_docAnotacion.Find(id);
                lOD_DocAnotacion.EstadoDoc = 1;
                LOD_Anotaciones lOD_Anotaciones = db.LOD_Anotaciones.Find(lOD_DocAnotacion.IdAnotacion);
                lOD_Anotaciones.Estado = 5; //Documentos rechazados? para permitir que en el libro se pueda volver a crear la anotación, cuando se crea una anotación se debe filtrar por las que no estan creadas.
                db.Entry(lOD_DocAnotacion).State = EntityState.Modified;
                db.Entry(lOD_Anotaciones).State = EntityState.Modified;//Documentos rechazados? para permitir que en el libro se pueda volver a crear la anotación, cuando se crea una anotación se debe filtrar por las que no estan creadas.
                db.SaveChanges();

                return Content("true");
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
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
