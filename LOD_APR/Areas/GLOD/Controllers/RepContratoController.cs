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
using Microsoft.AspNet.Identity;
using LOD_APR.Helpers;
using System.IO;
using System.Web;
using LOD_APR.Areas.GLOD.Helpers;

namespace LOD_APR.Areas.GLOD.Controllers
{
    [CustomAuthorize]
    public class RepContratoController : Controller
    {
        private LOD_DB db = new LOD_DB();

        // GET: Admin/SeguimientoCom
        public async Task<ActionResult> SeguimientoContrato(int Id, int Padre = 0, int Tipo = 0)
        {
            ViewBag.NombreCarpeta = "Biblioteca://";
            CON_Contratos cON_Contratos = db.CON_Contratos.Find(Id);
            List<LOD_LibroObras> list_Libros = await db.LOD_LibroObras.Where(x => x.IdContrato == cON_Contratos.IdContrato).ToListAsync();
            List<int> list_intLibros = list_Libros.Select(x => x.IdLod).ToList();

            List<LOD_Anotaciones> list_Anotaciones = await db.LOD_Anotaciones.Where(x => list_intLibros.Contains(x.IdLod)).ToListAsync();

            bool Alerta = false;
            int totalDocumentos = 0;
            int totalDocCargados = 0;
            int totalDocFaltantes = 0;
            List<MAE_TipoDocumento> documentosCargados = new List<MAE_TipoDocumento>();
            List<MAE_TipoDocumento> documentosNoCargados = new List<MAE_TipoDocumento>();

            foreach (var anot in list_Anotaciones)
            {
                //listado de control documentario según anotaciones del libro 
                List<MAE_CodSubCom> list_CodSubCom = list_CodSubCom = db.MAE_CodSubCom.Where(x => x.IdTipoSub == anot.IdTipoSub).ToList();
                totalDocumentos = list_CodSubCom.Count();
                
                foreach (var cod in list_CodSubCom)
                {
                    MAE_TipoDocumento auxTipo = db.LOD_docAnotacion.Where(x => x.IdAnotacion == anot.IdAnotacion && x.LOD_Anotaciones.IdTipoSub == anot.IdTipoSub && x.IdTipoDoc == cod.IdTipo).Select(x => x.MAE_TipoDocumento).FirstOrDefault();
                    if (auxTipo != null)
                        documentosCargados.Add(auxTipo);
                    else
                        documentosNoCargados.Add(cod.MAE_TipoDocumento);
                }
                if(documentosCargados.Count() < list_CodSubCom.Count())
                {
                    Alerta = true;
                }
                
            }
            totalDocCargados = documentosCargados.Count();
            totalDocFaltantes = documentosNoCargados.Count();
            ViewBag.Cargados = totalDocCargados;
            ViewBag.Faltantes = totalDocFaltantes;
            ViewBag.Totales = totalDocumentos;
            ViewBag.Alerta = Alerta;
            ViewBag.IdContrato = cON_Contratos.IdContrato;
            ViewBag.Tipo = Tipo;
            ViewBag.Padre = Padre;
            ViewBag.CodigoContrato = cON_Contratos.CodigoContrato;
            ViewBag.NombreContrato = cON_Contratos.NombreContrato;

            if (ValidaPermisos.ValidaPermisosEnController("0020050000"))
            {

                return View();
            }
            else
            {
                return RedirectToAction("SinPermiso", "../Pub");
            }

            
        }

        //public ActionResult GetClasificaciones(int IdContrato)
        //{
        //    List<MAE_ClassOne> listadoClassOne = db.MAE_ClassOne.ToList();

        //}

        //public ActionResult GetClasificacionesTwo(int IdContrato)
        //{
        //    List<MAE_ClassTwo> listadoClassTwo = db.MAE_ClassOne.ToList();

        //}


        public ActionResult GetDocumentos(int Padre, int Tipo, int IdContrato)
        {
            CON_Contratos contrato = db.CON_Contratos.Find(IdContrato);
            ViewBag.Root = 0;
            ViewBag.Padre = Padre;
            ViewBag.NombreCarpeta = "Repositorio Documentos://";
            ViewBag.FolderName = "Repositorio";
            ViewBag.Fecha_Documento = "Sin Fecha";
            ViewBag.Color = "warning";
            ViewBag.Tipo = 0;
            ViewBag.TipoElemento = "Clasificación";
            ViewBag.IdContrato = IdContrato;

            List<RepositorioView> listadoRep = new List<RepositorioView>();

            if (Padre > 0)
            {
                if(Tipo == 1)
                {
                    MAE_ClassOne mAE_ClassOne = db.MAE_ClassOne.Find(Padre);
                    ViewBag.TipoElemento = 1;
                    ViewBag.NombreCarpeta = $"~/Repositorio/{mAE_ClassOne.Nombre}";
                    ViewBag.Root = mAE_ClassOne.IdClassOne;
                    ViewBag.FolderName = mAE_ClassOne.Nombre;                    
                    ViewBag.Color = "warning2";
                    ViewBag.Tipo = 1;
                    ViewBag.TipoElemento = "Subclasificación";
                    


                    List<MAE_ClassTwo> listadoTwo = db.MAE_ClassTwo.Where(x => x.IdClassOne == mAE_ClassOne.IdClassOne).ToList();
                    foreach (var item in listadoTwo)
                    {
                        List<int> list_ClassDoc = db.MAE_ClassDoc.Where(x => x.IdClassTwo == item.IdClassTwo).Select(x => x.IdTipo).ToList();
                        List<LOD_docAnotacion> listadoDoc = db.LOD_docAnotacion.Where(x => x.IdContrato == contrato.IdContrato && list_ClassDoc.Contains(x.IdTipoDoc) && x.MAE_documentos.IdPath == item.IdClassTwo && (x.LOD_Anotaciones.Estado == 2 || x.IdAnotacion == null)).ToList();


                        RepositorioView elemento = new RepositorioView();
                        elemento.IdElemento = item.IdClassTwo;
                        elemento.TipoElemento = 1;
                        elemento.NombreElemento = item.Nombre;
                        elemento.Obligatorio = false;
                        elemento.EstadoElemento = 0;
                        elemento.totalArchivos = listadoDoc.Count();
                        listadoRep.Add(elemento);
                    }
                }
                else if(Tipo == 2)
                {
                    MAE_ClassTwo classTwo = db.MAE_ClassTwo.Find(Padre);
                    ViewBag.TipoElemento = 1;
                    ViewBag.NombreCarpeta = $"~/Repositorio/{classTwo.MAE_ClassOne.Nombre}/{classTwo.Nombre}";
                    ViewBag.Root = classTwo.IdClassOne;
                    ViewBag.FolderName = classTwo.Nombre;
                    ViewBag.Color = "success";
                    ViewBag.Tipo = 2;
                    ViewBag.TipoElemento = "Subclasificación";
                    ViewBag.Padre = classTwo.IdClassOne;
                    ViewBag.IdClassTwo = classTwo.IdClassTwo;

                    List<MAE_ClassDoc> listadoRequeridos = db.MAE_ClassDoc.Where(x => x.IdClassTwo == classTwo.IdClassTwo).ToList();
                    List<int> enterosRequeridos = listadoRequeridos.Select(x => x.IdTipo).ToList();
                    List<LOD_docAnotacion> listadoCargados = db.LOD_docAnotacion.Where(x => x.IdContrato == IdContrato && enterosRequeridos.Contains(x.IdTipoDoc) && x.MAE_documentos.IdPath == classTwo.IdClassTwo && x.LOD_Anotaciones.Estado == 2 ).ToList();
                    List<MAE_TipoDocumento> listadoFaltantes = listadoRequeridos.Select(x => x.MAE_TipoDocumento).ToList().Except(listadoCargados.Select(x => x.MAE_TipoDocumento)).ToList();
                    
                    foreach (var item in listadoFaltantes)
                    {
                        RepositorioView auxRepo1 = new RepositorioView();

                        int? idSubtipo = listadoRequeridos.Where(x => x.IdTipo == item.IdTipo).FirstOrDefault().IdTipoSub;
                        if (idSubtipo == null) {
                            auxRepo1.Obligatorio = false;
                            auxRepo1.IdAnotacion = 0;
                        }
                        else {
                            MAE_SubtipoComunicacion subtipo = db.MAE_SubtipoComunicacion.Find(idSubtipo);
                            LOD_LibroObras libro = db.LOD_LibroObras.Where(x => x.IdContrato == contrato.IdContrato && x.IdTipoLod == subtipo.MAE_TipoComunicacion.IdTipoLod).FirstOrDefault();
                            if (libro != null)
                                auxRepo1.IdLibro = libro.IdLod;

                            MAE_CodSubCom control = db.MAE_CodSubCom.Where(x => x.IdTipoSub == subtipo.IdTipoSub).FirstOrDefault();
                            if (control != null)
                            {
                                auxRepo1.Obligatorio = true;
                            }
                        }
                        auxRepo1.IdElemento = item.IdTipo;
                        auxRepo1.IdTipoDocumento = item.IdTipo;
                        auxRepo1.NombreElemento = item.Tipo;
                        auxRepo1.EstadoElemento = 0;
                        auxRepo1.TipoElemento = 2;
                        auxRepo1.totalArchivos = 0;
                        listadoRep.Add(auxRepo1);
                    }

                    foreach (var item in listadoCargados)
                    {
                        RepositorioView auxRepo2 = new RepositorioView();
                       
                        auxRepo2.IdElemento = item.IdTipoDoc;
                        auxRepo2.IdTipoDocumento = item.IdTipoDoc;
                        auxRepo2.NombreElemento = item.MAE_TipoDocumento.Tipo;
                        auxRepo2.Obligatorio = false;
                        if (item.IdAnotacion != null) {
                            MAE_CodSubCom control = db.MAE_CodSubCom.Where(x => x.IdTipoSub == item.LOD_Anotaciones.IdTipoSub && x.IdTipo == item.IdTipoDoc).FirstOrDefault();
                            if (control != null)
                                auxRepo2.Obligatorio = true;

                            auxRepo2.IdAnotacion = item.IdAnotacion;
                            auxRepo2.IdLibro = item.LOD_Anotaciones.IdLod;
                        }

                        auxRepo2.totalArchivos = listadoCargados.Where(x => x.IdTipoDoc == item.IdTipoDoc).Count();
                        auxRepo2.EstadoElemento = 1;
                        auxRepo2.TipoElemento = 2;

                        if (listadoRep.Select(x => x.IdTipoDocumento).Contains(auxRepo2.IdTipoDocumento))
                            auxRepo2.totalArchivos = auxRepo2.totalArchivos + 1;
                        else
                            listadoRep.Add(auxRepo2);
                    }
                }

            }
            else
            {
                List<MAE_ClassOne> listadoOne = db.MAE_ClassOne.ToList();
                foreach (var item in listadoOne)
                {
                    List<int> listadoClassTwo = db.MAE_ClassTwo.Where(x => x.IdClassOne == item.IdClassOne).Select(x => x.IdClassTwo).ToList();
                    List<int> list_ClassDoc = db.MAE_ClassDoc.Where(x => listadoClassTwo.Contains(x.IdClassTwo)).Select(x => x.IdTipo).ToList();
                    List<LOD_docAnotacion> listadoDoc = db.LOD_docAnotacion.Where(x => x.IdContrato == contrato.IdContrato && list_ClassDoc.Contains(x.IdTipoDoc) && listadoClassTwo.Contains(x.MAE_documentos.IdPath.Value) && x.LOD_Anotaciones.Estado == 2).ToList();

                    RepositorioView elemento = new RepositorioView();
                    elemento.IdElemento = item.IdClassOne;
                    elemento.TipoElemento = 1;
                    elemento.NombreElemento = item.Nombre;
                    elemento.Obligatorio = false;
                    elemento.EstadoElemento = 0;
                    elemento.totalArchivos = listadoDoc.Count();
                    listadoRep.Add(elemento);
                }                
            }

            ViewBag.UserName = User.Identity.GetUserName();
            ViewBag.NumRegistro = listadoRep.Count();

            return PartialView("_GetDocumentos", listadoRep.OrderBy(x => x.IdElemento));
        }

        public async Task<ActionResult> AddDocumento(int IdTipoDoc, int IdContrato, int IdClassTwo)
        {

            MAE_TipoDocumento mAE_TipoDocumento = db.MAE_TipoDocumento.Where(x => x.IdTipo == IdTipoDoc).FirstOrDefault();
            MAE_ClassDoc mAE_ClassDoc = db.MAE_ClassDoc.Where(x => x.IdClassTwo == IdClassTwo && x.IdTipo == IdTipoDoc).FirstOrDefault();
            LOD_docAnotacion docAnot = new LOD_docAnotacion()
            {
                IdTipoDoc = mAE_TipoDocumento.IdTipo,
                EstadoDoc = 0,
                MAE_TipoDocumento = mAE_TipoDocumento,
                IdContrato = IdContrato,
                IdClassDoc = mAE_ClassDoc.IdClassDoc,
                ClassDoc = mAE_ClassDoc,
                IdClassTwo = mAE_ClassDoc.IdClassTwo
            };

            return PartialView("_ModalAddFile", docAnot);
        }

        [HttpPost, ActionName("AddDocumento")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddDocumento(LOD_docAnotacion lOD_DocAnotacion, HttpPostedFileBase PerFileName)
        {

            try
            {
                HelperDocumentos helper_docs = new HelperDocumentos();

                MAE_documentos newDoc = new MAE_documentos();
                MAE_TipoDocumento tipoDoc = db.MAE_TipoDocumento.Find(lOD_DocAnotacion.IdTipoDoc);
                MAE_ClassDoc mAE_ClassDoc = await db.MAE_ClassDoc.Where(x => x.IdTipo == tipoDoc.IdTipo && x.IdClassDoc == lOD_DocAnotacion.IdClassDoc).FirstOrDefaultAsync();
                lOD_DocAnotacion.ClassDoc = mAE_ClassDoc;

                newDoc.UserId = User.Identity.GetUserId();
                newDoc.NombreDoc = lOD_DocAnotacion.MAE_documentos.NombreDoc;
                newDoc.Descripcion = lOD_DocAnotacion.MAE_documentos.Descripcion;
                newDoc.IdPath = lOD_DocAnotacion.MAE_documentos.IdPath;
                newDoc.Extension = Path.GetExtension(PerFileName.FileName);
                //string PrimaryKeyIdentify = ;
                string doc_pre_name = tipoDoc.Tipo;
                //MAE_TipoPath tipo_path = db.MAE_TipoPath.Find(PathId);
                string rutaBase = $"~/Files/Contratos/{mAE_ClassDoc.MAE_ClassTwo.MAE_ClassOne.Nombre}/{mAE_ClassDoc.MAE_ClassTwo.Nombre}/";
                string rutaSave = $"Files/Contratos/{mAE_ClassDoc.MAE_ClassTwo.MAE_ClassOne.Nombre}/{mAE_ClassDoc.MAE_ClassTwo.Nombre}/";

                Status_Error save_file = helper_docs.SaveFileToDisk(0, lOD_DocAnotacion, PerFileName, rutaBase, rutaSave, doc_pre_name, newDoc);

                if (save_file.Error)
                {
                    return Content("Error al guardar el documento");
                }
                else
                { 
                    return RedirectToAction("SeguimientoContrato", new { Padre = lOD_DocAnotacion.IdClassTwo, Tipo = 2, Id = lOD_DocAnotacion.IdContrato });
                }

            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
        }

        public async Task<ActionResult> eliminarDocumento(int IdDocAnotacion, int IdClassTwo)
        {
            LOD_docAnotacion docAnot = await db.LOD_docAnotacion.FindAsync(IdDocAnotacion);
            docAnot.IdClassTwo = IdClassTwo;
            return PartialView("_DeleteDocumento", docAnot);
        }

        [HttpPost, ActionName("eliminarDocumento")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> eliminarDocumento(LOD_docAnotacion lOD_DocAnotacion, HttpPostedFileBase PerFileName)
        {

            try
            {
                int PadreInfo = lOD_DocAnotacion.IdClassTwo;
                int Contrato = lOD_DocAnotacion.IdContrato;
                LOD_docAnotacion docAnotacion = db.LOD_docAnotacion.Find(lOD_DocAnotacion.IdDocAnotacion);
                db.LOD_docAnotacion.Remove(docAnotacion);
                await db.SaveChangesAsync();
              
                return RedirectToAction("SeguimientoContrato", new { Padre = PadreInfo, Tipo = 2, Id = Contrato });
                

            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
        }

        public async Task<ActionResult> DescargarDocumento(int id)
        {
            LOD_docAnotacion documento = db.LOD_docAnotacion.Find(id);
            string tempPath = Path.Combine(Server.MapPath("~/"), documento.MAE_documentos.Ruta);

            var memory = new MemoryStream();
            using (var stream = new FileStream(tempPath, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;

            Log_Helper log_Helper = new Log_Helper();
            string tipodoc = documento.MAE_TipoDocumento.Tipo;
            string accion = "Se ha descargado el documento:" + tipodoc;
            bool response = await log_Helper.SetObjectLog(0, documento.LOD_Anotaciones, accion, GetUserInSession().Id);

            return File(memory, documento.MAE_documentos.ContentType, Path.GetFileName(tempPath));
        }

        public async Task<ActionResult> detalleAnotacion(int IdTipoDoc, int IdLibro, int IdContrato, int IdClassTwo)
        {
            LOD_LibroObras libro = db.LOD_LibroObras.Find(IdLibro);
            List<LOD_docAnotacion> listadoDocumentos = await db.LOD_docAnotacion.Where(x => x.IdTipoDoc == IdTipoDoc && x.LOD_Anotaciones.IdLod == libro.IdLod && x.LOD_Anotaciones.Estado == 2 && x.IdContrato == IdContrato && x.MAE_documentos.IdPath == IdClassTwo).ToListAsync();
            foreach (var item in listadoDocumentos)
            {
                item.ClassDoc = db.MAE_ClassDoc.Where(x => x.IdClassTwo == IdClassTwo).FirstOrDefault();
            }
            ViewBag.IdClassTwo = IdClassTwo;
            return PartialView("_detalleDocAnotacion", listadoDocumentos);
        }

        public ApplicationUser GetUserInSession()
        {
            string userName = User.Identity.GetUserId();
            var user = db.Users.Find(userName);
            return user;
        }

        public async Task<ActionResult> detalleDocumento(int IdTipoDoc, int IdContrato, int IdClassTwo)
        {
            List<LOD_docAnotacion> listadoDocumentos = await db.LOD_docAnotacion.Where(x => x.IdTipoDoc == IdTipoDoc && x.IdContrato == IdContrato && x.IdAnotacion == null && x.MAE_documentos.IdPath == IdClassTwo).ToListAsync();
            foreach (var item in listadoDocumentos)
            {
                item.ClassDoc = db.MAE_ClassDoc.Where(x => x.IdTipo == item.IdTipoDoc && x.IdClassTwo == item.MAE_documentos.IdPath).FirstOrDefault();
            }
            ViewBag.IdClassTwo = IdClassTwo;
            return PartialView("_detalleDocAnotacion", listadoDocumentos);
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
