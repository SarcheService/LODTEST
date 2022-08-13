using LOD_APR.Areas.Admin.Models;
using LOD_APR.Areas.GLOD.Helpers;
using LOD_APR.Areas.GLOD.Models;
using LOD_APR.Areas.GLOD.ModelsViews;
using LOD_APR.Helpers;
using LOD_APR.Helpers.ModelsHelpers;
using LOD_APR.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace LOD_APR.Areas.GLOD.Controllers
{
    [CustomAuthorize]
    public class AnotDocsController : Controller
    {
        private LOD_DB db = new LOD_DB();
        private Log_Helper Log_Helper = new Log_Helper();

        public async Task<ActionResult> VerDocumento(int id)
        {
            LOD_docAnotacion docAnot = await db.LOD_docAnotacion.FindAsync(id);

            //****DETERMINAR SI LA ANOTACION ESTÁ FIRMADA Y QUE EL USUARIO PERMITIDO PARA 
            //LA APROBACIÓN/RECHAZO DEL DOCUMENTO SEA EL USUARIO RECEPTOR PRINCIPAL
            bool isResp = false;
            string idUser = User.Identity.GetUserId();
            var isPrincipal = await db.LOD_UserAnotacion.Where(u => u.LOD_UsuarioLod.UserId == idUser && u.IdAnotacion == docAnot.IdAnotacion).FirstOrDefaultAsync();
            if (docAnot.LOD_Anotaciones.EstadoFirma && isPrincipal.EsPrincipal)
                isResp = true;
            //**************************************************************************************************************

            DocumentosAnotacionView newDoc = new DocumentosAnotacionView()
            {
                IdAnotacion = docAnot.IdAnotacion,
                IdDocanot = id,
                IdEstado = docAnot.EstadoDoc,
                IdTipoDoc = docAnot.IdTipoDoc,
                TipoClasi = docAnot.MAE_TipoDocumento.TipoClasi,
                Descripcion = docAnot.MAE_documentos.Descripcion,
                Ruta = "/../../" + docAnot.MAE_documentos.Ruta,
                TipoDoc = docAnot.MAE_TipoDocumento.Tipo,
                Titulo = docAnot.MAE_documentos.NombreDoc,
                CreadoEl = docAnot.MAE_documentos.FechaCreacion.ToLongDateString() + " a las " + docAnot.MAE_documentos.FechaCreacion.ToShortTimeString(),
                CreadoPor = docAnot.MAE_documentos.CreadoPor.Nombres.Split(' ')[0] + " " + docAnot.MAE_documentos.CreadoPor.Apellidos,
                FechaEvento = docAnot.FechaEvento.ToString(),
                UsuarioEvento = (docAnot.UsuarioEvento != null) ? docAnot.UsuarioEvento.Nombres + docAnot.UsuarioEvento.Apellidos : "",
                Observaciones = docAnot.Observaciones,
                IsResponsable = isResp
            };

            CON_Contratos contrato = db.CON_Contratos.Where(x => x.IdContrato == docAnot.IdContrato).FirstOrDefault();
            ViewBag.ContratoLiquidado = false;
            if (contrato.EstadoContrato == 3)
                ViewBag.ContratoLiquidado = true;

            return PartialView("_sidebarVerDocumento", newDoc);
        }

        public async Task<JsonResult> GetDocumentosData(int id)
        {
            List<DocumentosAnotacionView> listDocumentosAnotacion = new List<DocumentosAnotacionView>();
            LOD_Anotaciones anotacion = await db.LOD_Anotaciones.FindAsync(id);
            List<MAE_TipoDocumento> docRequeridos = db.MAE_CodSubCom.Where(x => x.IdTipoSub == anotacion.IdTipoSub).Select(x => x.MAE_TipoDocumento).ToList();
            List<LOD_docAnotacion> docCargados = db.LOD_docAnotacion.Where(x => x.IdAnotacion == anotacion.IdAnotacion).ToList();
            docRequeridos = docRequeridos.Except(docCargados.Select(x => x.MAE_TipoDocumento)).ToList();
            //****DETERMINAR SI LA ANOTACION AUN NO ESTÁ FIRMADA Y QUE EL USUARIO PERMITIDO PARA EL INGRESO DE LOS DOCUMENTOS
            //SEA EL USUARIO BORRADOR O EL USUARIO FIRMANTE SOLAMENTE
            bool isResp = false;
            string idUser = User.Identity.GetUserId();
            if (!anotacion.EstadoFirma && (anotacion.UserId == idUser || anotacion.UserIdBorrador == idUser))
                isResp = true;
            //**************************************************************************************************************
            foreach (var item in docRequeridos)
            {
                DocumentosAnotacionView newDoc = new DocumentosAnotacionView()
                {
                    IdAnotacion = id,
                    IdDocanot = 0,
                    IdEstado = 0,
                    IdTipoDoc = item.IdTipo,
                    TipoClasi = item.TipoClasi,
                    Descripcion = string.Empty,
                    Ruta = string.Empty,
                    TipoDoc = item.Tipo,
                    Titulo = string.Empty,
                    IsResponsable = isResp,
                    anotFirmada = anotacion.EstadoFirma
                };
                listDocumentosAnotacion.Add(newDoc);
            }

            foreach (var item in docCargados)
            {
                DocumentosAnotacionView newDoc = new DocumentosAnotacionView()
                {
                    IdAnotacion = id,
                    IdDocanot = item.IdDocAnotacion,
                    IdEstado = item.EstadoDoc,
                    IdTipoDoc = item.MAE_TipoDocumento.IdTipo,
                    TipoClasi = item.MAE_TipoDocumento.TipoClasi,
                    Descripcion = item.Observaciones,
                    Ruta = "/../../" + item.MAE_documentos.Ruta,
                    TipoDoc = item.MAE_TipoDocumento.Tipo,
                    Titulo = item.MAE_documentos.NombreDoc,
                    anotFirmada = anotacion.EstadoFirma
                };
                listDocumentosAnotacion.Add(newDoc);
            }

            return Json(listDocumentosAnotacion, JsonRequestBehavior.AllowGet);

        }

        public async Task<ActionResult> AddDocumento(int id, int IdTipoDoc)
        {
            var anot = await db.LOD_Anotaciones.FindAsync(id);
            var tipo = await db.MAE_TipoDocumento.FindAsync(IdTipoDoc);
            AddDocumentoView docAnot = new AddDocumentoView()
            {
                IdAnotacion = id,
                IdTipoDoc = IdTipoDoc,
                TipoDoc = tipo.Tipo,
                IdContrato = anot.LOD_LibroObras.IdContrato
            };

            return PartialView("_sidebarAddDocumento", docAnot);
        }

        [HttpPost, ActionName("AddDocumento")]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> AddDocumento([Bind(Include = "IdAnotacion,Nombre,Descripcion,IdTipoDoc,PerFileName,IdContrato")]AddDocumentoView documento, HttpPostedFileBase PerFileName)
        {
            Form_Validation val = new Form_Validation() { Status = true, ErrorMessage = new List<string>() };
            if(PerFileName==null || PerFileName.ContentLength <= 0)
            {
                val.Status = false;
                val.ErrorMessage.Add("El Archivo del Documento es Obligatorio");
                return Json(val, JsonRequestBehavior.AllowGet);
            }
           
            try
            {
                if (ModelState.IsValid)
                {
                    HelperDocumentos helper_docs = new HelperDocumentos();
                    var anot = await db.LOD_Anotaciones.Where(a => a.IdAnotacion == documento.IdAnotacion && a.EstadoFirma == false).FirstOrDefaultAsync();
                    if (anot != null)
                    {

                        MAE_TipoDocumento tipoDoc = db.MAE_TipoDocumento.Find(documento.IdTipoDoc);
                        MAE_ClassDoc mAE_ClassDoc = db.MAE_ClassDoc.Where(x => x.IdTipo == tipoDoc.IdTipo && x.IdTipoSub == anot.IdTipoSub).FirstOrDefault();

                        MAE_documentos newDoc = new MAE_documentos();
                        newDoc.UserId = User.Identity.GetUserId();
                        newDoc.NombreDoc = documento.Nombre;
                        newDoc.Descripcion = documento.Descripcion;
                        newDoc.IdPath = mAE_ClassDoc.IdClassTwo;
                        newDoc.Extension = Path.GetExtension(PerFileName.FileName);
                        string PrimaryKeyIdentify = documento.IdAnotacion.ToString();
                        //string doc_pre_name = DateTime.Now.ToString("yyyyMMddTHHmmss");
                     
                        LOD_APR.Helpers.Status_Error save_file = helper_docs.SaveFileToDisk(1, documento, PerFileName, $"~/{anot.RutaCarpetaBorradores}", anot.RutaCarpetaBorradores,"", newDoc);

                        if (!save_file.Error)
                        {
                            string accion = $"Se ha agregado el documento: {documento.Nombre} en la anotación.";
                            bool response = await Log_Helper.SetObjectLog(0, anot, accion, GetUserInSession().Id);
                        }
                        else
                        {
                            val.Status = false;
                            val.ErrorMessage.Add("Ocurrió un error al subir el documento:");
                        }

                    }
                    else
                    {
                        val.Status = false;
                        val.ErrorMessage.Add("La anotación se encuentra inhabilitada para su modificación.");
                    }
                }
                else
                {
                    IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
                    val.Status = false;
                    foreach (var err in allErrors)
                        val.ErrorMessage.Add(err.ErrorMessage);
                }
            }
            catch (Exception ex)
            {
                val.Status = false;
                val.ErrorMessage.Add(ex.Message);
            }

            return Json(val, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> AddOtroDocumento(int id)
        {
            var anot = await db.LOD_Anotaciones.FindAsync(id);
            AddDocumentoView docAnot = new AddDocumentoView()
            {
                IdAnotacion = id,
                IdContrato = anot.LOD_LibroObras.IdContrato
            };

            return PartialView("_sidebarAddOtrosDocs", docAnot);
        }

        [HttpPost, ActionName("AddOtroDocumento")]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> AddOtroDocumento([Bind(Include = "IdAnotacion,Nombre,Descripcion,IdTipoDoc,PerFileName,IdContrato")]AddDocumentoView documento, HttpPostedFileBase PerFileName)
        {
            Form_Validation val = new Form_Validation() { Status = true, ErrorMessage = new List<string>() };
            try
            {
                if (PerFileName == null || PerFileName.ContentLength <= 0)
                {
                    val.Status = false;
                    val.ErrorMessage.Add("El Archivo del Documento es Obligatorio");
                    return Json(val, JsonRequestBehavior.AllowGet);
                }

                if (ModelState.IsValid)
                {
                    HelperDocumentos helper_docs = new HelperDocumentos();

                    var anot = await db.LOD_Anotaciones.Where(a => a.IdAnotacion == documento.IdAnotacion && a.EstadoFirma == false).FirstOrDefaultAsync();
                    if (anot != null)
                    {
                        MAE_documentos newDoc = new MAE_documentos();
                        //********BUSCVAMPOS EL ID DEL PATH (IDCLASSTWO)
                        MAE_ClassDoc mAE_ClassDoc = db.MAE_ClassDoc.Where(x => x.IdTipo == documento.IdTipoDoc).FirstOrDefault();
                        documento.IdPath = mAE_ClassDoc.IdClassTwo;
                        //**************************************************
                        newDoc.UserId = User.Identity.GetUserId();
                        newDoc.NombreDoc = documento.Nombre;
                        newDoc.Descripcion = documento.Descripcion;
                        newDoc.IdPath = documento.IdPath;
                        newDoc.Extension = Path.GetExtension(PerFileName.FileName);
                        string PrimaryKeyIdentify = documento.IdAnotacion.ToString();
                        //string doc_pre_name = DateTime.Now.ToString("yyyyMMddTHHmmss");
                     
                        LOD_APR.Helpers.Status_Error save_file = helper_docs.SaveFileToDisk(1, documento, PerFileName, $"~/{anot.RutaCarpetaBorradores}", anot.RutaCarpetaBorradores, "", newDoc);

                        if (!save_file.Error)
                        {
                            string accion = $"Se ha agregado el documento: {documento.Nombre} en la anotación.";
                            bool response = await Log_Helper.SetObjectLog(0, anot, accion, GetUserInSession().Id);
                        }
                        else
                        {
                            val.Status = false;
                            val.ErrorMessage.Add("Ocurrió un error al subir el documento:");
                        }

                    }
                    else
                    {
                        val.Status = false;
                        val.ErrorMessage.Add("La anotación se encuentra inhabilitada para su modificación.");
                    }
                }
                else
                {
                    IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
                    val.Status = false;
                    foreach (var err in allErrors)
                        val.ErrorMessage.Add(err.ErrorMessage);
                }
            }
            catch (Exception ex)
            {
                val.Status = false;
                val.ErrorMessage.Add(ex.Message);
            }

            return Json(val, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> AprobarDocumento(int id)
        {
            LOD_docAnotacion docAnot = await db.LOD_docAnotacion.FindAsync(id);

            return PartialView("_sidebarAprobarDocumento", docAnot);
        }

        [HttpPost, ActionName("AprobarDocumento")]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> AprobarDocumento(LOD_docAnotacion docAnotacion)
        {
            Form_Validation val = new Form_Validation() { Status = true, ErrorMessage = new List<string>() };
            try
            {
                HelperDocumentos helper_docs = new HelperDocumentos();
                //VALIDAR QUE SE PUEDA EDITAR AL RECEPTOR (SI NO ESTA FIRMADA, ETC)
                LOD_Anotaciones anot = db.LOD_Anotaciones.Find(docAnotacion.IdAnotacion);
                if (anot != null)
                {
                    LOD_docAnotacion docAnot = db.LOD_docAnotacion.Find(docAnotacion.IdDocAnotacion);
                    docAnot.EstadoDoc = 2;
                    docAnot.FechaEvento = DateTime.Now;
                    docAnot.IdUserEvento = User.Identity.GetUserId();
                    db.Entry(docAnot).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                    val.Parametros = "/GLOD/Anotaciones/Edit?id=" + docAnotacion.IdAnotacion +"&tabDoc=true";
                    string tipodoc = db.MAE_TipoDocumento.Find(docAnotacion.IdTipoDoc).Tipo;
                    string accion = "Se ha aprobado el documento:" + tipodoc;
                    bool response = await Log_Helper.SetObjectLog(0, anot, accion, GetUserInSession().Id);

                   
                    GLOD_Notificaciones noty = new GLOD_Notificaciones();  //NOTIFICACION
                    int resu = await noty.NotificarAprobacionDoc(anot, docAnot, GetUserInSession().Id);

                    if (docAnot.LOD_Anotaciones.IdTipoSub.Equals(30))
                    {
                        if (docAnot.IdTipoDoc.Equals(64))
                        {
                            List<FORM_InformesItems> itemsInformes = db.FORM_InformesItems.Where(x => x.IdAnotacion == docAnot.IdAnotacion).ToList();
                            if(itemsInformes != null)
                            {
                                foreach (var item in itemsInformes)
                                {
                                    string nombreLimpio = docAnot.MAE_documentos.NombreDoc.Replace("_", " ");
                                    if (nombreLimpio.Contains(item.Titulo))
                                    {
                                        item.Estado = 3;
                                        db.Entry(item).State = EntityState.Modified;
                                        await db.SaveChangesAsync();
                                    }
                                   
                                }
                               
                            }
                        }
                    }
                }
                else
                {
                    val.Status = false;
                    val.ErrorMessage.Add("La anotación se encuentra inhabilitada para su modificación.");
                }
            }
            catch (Exception ex)
            {
                val.Status = false;
                val.ErrorMessage.Add(ex.Message);
            }

            return Json(val, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> RechazarDocumento(int id)
        {
            LOD_docAnotacion docAnot = await db.LOD_docAnotacion.FindAsync(id);

            return PartialView("_sidebarRechazarDocumento", docAnot);
        }

        [HttpPost, ActionName("RechazarDocumento")]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> RechazarDocumento(LOD_docAnotacion docAnotacion)
        {
            Form_Validation val = new Form_Validation() { Status = true, ErrorMessage = new List<string>() };
            try
            {
                HelperDocumentos helper_docs = new HelperDocumentos();
                //VALIDAR QUE SE PUEDA EDITAR AL RECEPTOR (SI NO ESTA FIRMADA, ETC)
                LOD_Anotaciones anot = db.LOD_Anotaciones.Find(docAnotacion.IdAnotacion);
                if (anot != null)
                {
                    LOD_docAnotacion docAnot = db.LOD_docAnotacion.Find(docAnotacion.IdDocAnotacion);
                    docAnot.EstadoDoc = 3;
                    docAnot.FechaEvento = DateTime.Now;
                    docAnot.IdUserEvento = User.Identity.GetUserId();
                    db.Entry(docAnot).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                    val.Parametros = "/GLOD/Anotaciones/Edit?id=" + docAnotacion.IdAnotacion + "&tabDoc=true";
                    string tipodoc = db.MAE_TipoDocumento.Find(docAnotacion.IdTipoDoc).Tipo;
                    string accion = "Se ha aprobado el documento:" + tipodoc;
                    bool response = await Log_Helper.SetObjectLog(0, anot, accion, GetUserInSession().Id);

                    GLOD_Notificaciones noty = new GLOD_Notificaciones();  //NOTIFICACION
                    int resu = await noty.NotificarRechazoDoc(anot, docAnot, GetUserInSession().Id);

                    if (docAnot.LOD_Anotaciones.IdTipoSub.Equals(30))
                    {
                        if (docAnot.IdTipoDoc.Equals(64))
                        {
                            List<FORM_InformesItems> itemsInformes = db.FORM_InformesItems.Where(x => x.IdAnotacion == docAnot.IdAnotacion).ToList();
                            if (itemsInformes != null)
                            {
                                foreach (var item in itemsInformes)
                                {
                                    string nombreLimpio = docAnot.MAE_documentos.NombreDoc.Replace("_", " ");
                                    if (nombreLimpio.Contains(item.Titulo))
                                    {
                                        item.Estado = 4;
                                        db.Entry(item).State = EntityState.Modified;
                                        await db.SaveChangesAsync();
                                    }

                                }

                            }
                        }
                    }
                }
                else
                {
                    val.Status = false;
                    val.ErrorMessage.Add("La anotación se encuentra inhabilitada para su modificación.");
                }
            }
            catch (Exception ex)
            {
                val.Status = false;
                val.ErrorMessage.Add(ex.Message);
            }

            return Json(val, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> DeleteDocumento(int id)
        {
            LOD_docAnotacion docAnot = await db.LOD_docAnotacion.FindAsync(id);

            return PartialView("_sidebarDeleteDocumento", docAnot);
        }

        [HttpPost]
        public async Task<JsonResult> DeleteDocumento(LOD_docAnotacion docAnotacion)
        {
            Form_Validation val = new Form_Validation() { Status = true, ErrorMessage = new List<string>() };
            try
            {
                HelperDocumentos helper_docs = new HelperDocumentos();
                //VALIDAR QUE SE PUEDA EDITAR AL RECEPTOR (SI NO ESTA FIRMADA, ETC)
                LOD_Anotaciones anot = db.LOD_Anotaciones.Find(docAnotacion.IdAnotacion);
                if (anot != null)
                {
                    


                    LOD_docAnotacion docAnot = db.LOD_docAnotacion.Find(docAnotacion.IdDocAnotacion);
                    string tipodoc = db.MAE_TipoDocumento.Find(docAnotacion.IdTipoDoc).Tipo;

                    string borrador = Path.Combine(Server.MapPath("~/"), docAnot.MAE_documentos.Ruta);
                    System.IO.File.Delete(borrador);

                    db.LOD_docAnotacion.Remove(docAnot);
                    await db.SaveChangesAsync();

                    val.Parametros = "/GLOD/Anotaciones/Edit/" + anot.IdAnotacion;
                    
                    string accion = "Se ha eliminado el documento:" + tipodoc;
                    bool response = await Log_Helper.SetObjectLog(0, anot, accion, GetUserInSession().Id);


                }
                else
                {
                    val.Status = false;
                    val.ErrorMessage.Add("La anotación se encuentra inhabilitada para su modificación.");
                }
            }
            catch (Exception ex)
            {
                val.Status = false;
                val.ErrorMessage.Add(ex.Message);
            }

            return Json(val, JsonRequestBehavior.AllowGet);
        }

        public ApplicationUser GetUserInSession()
        {
            string userName = User.Identity.GetUserId();
            var user = db.Users.Find(userName);
            return user;
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

            string tipodoc = documento.MAE_TipoDocumento.Tipo;
            string accion = "Se ha descargado el documento:" + tipodoc;
            bool response = await Log_Helper.SetObjectLog(0, documento.LOD_Anotaciones, accion, GetUserInSession().Id);

            return File(memory, documento.MAE_documentos.ContentType, Path.GetFileName(tempPath));
        }

        public async Task<JsonResult> GetTipoDocumento(int id)
        {
            List<SelectListItem> tipo = new List<SelectListItem>();
            List<MAE_TipoDocumento> listTipos = await db.MAE_ClassDoc.Where(x => x.IdClassTwo == id).Select(s=> s.MAE_TipoDocumento).ToListAsync();

            foreach (var item in listTipos)
                tipo.Add(new SelectListItem() { Text = item.Tipo, Value = item.IdTipo.ToString() });

            return Json(tipo, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> AddInforme(int id)
        {
            LOD_Anotaciones anot = await db.LOD_Anotaciones.FindAsync(id);
            int idcontrato = anot.LOD_LibroObras.IdContrato;
            var informe = db.FORM_Informes.Where(i => i.IdContrato == idcontrato && i.Estado == false).FirstOrDefault();
            AddInformeView report = new AddInformeView() { IdAnotacion = id, IdInforme = 0, IdContrato= idcontrato };
           
            if (informe != null)
            {
                report.IdInforme = informe.IdEnvio;
                report.Periodo = $"{informe.Mes.PadLeft(2, '0')}-{informe.Anio}";
                report.Folio = informe.IdEnvio.ToString().PadLeft(6, '0');
                report.Fecha = informe.FechaCreacion.ToLongDateString();
                report.MesInformado = informe.MesInformado;
            }
               
            return PartialView("_sidebarAddInformeMensual", report);
        }

        [HttpPost, ActionName("AddInforme")]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> AddInformeConfirmed(AddInformeView informe)
        {
            Form_Validation val = new Form_Validation() { Status = true, ErrorMessage = new List<string>() };
            try
            {
                if (ModelState.IsValid)
                {
                    LOD_Anotaciones anot = db.LOD_Anotaciones.Find(informe.IdAnotacion);
                    FORM_InformesItems item = db.FORM_InformesItems.Find(informe.IdItem);
                    InformeToDataTable infToDt = new InformeToDataTable();
                    DataTable dt = new DataTable();
                    string fileName = string.Empty;
                    string description = string.Empty;

                    if (item.FORM_Formularios.Tipo > 1)
                    {
                        fileName = $"Informe_Ejecutivo_Contrato_{item.FORM_Informes.CON_Contratos.CodigoContrato}_{item.Titulo}_{item.FORM_Informes.MesInformado}.xlsx";
                        description = $"Informe ejecutivo mensual \"{item.Titulo}\" correspondiente al periodo \"{item.FORM_Informes.MesInformado}\" del informe Folio \"{item.FORM_Informes.IdEnvio.ToString().PadLeft(6, '0')}\"";
                        dt = await infToDt.GenerateDataTable(item);
                    }
                    else
                    {
                        fileName = $"Informe_Incidentes_Contrato_{item.CON_Contratos.CodigoContrato.Replace(" ", "_")}_{item.Titulo.Replace(" ", "_")}.xlsx";
                        description = $"Informe Reporte Incidentes con Informe Folio \"{item.IdItem.ToString().PadLeft(6, '0')}\"";
                        dt = await infToDt.GenerateDataTableIncidentes(item);
                    }
                 
                    if (!System.IO.Directory.Exists(Server.MapPath("~/" + anot.RutaCarpetaBorradores)))
                        System.IO.Directory.CreateDirectory(Server.MapPath("~/" + anot.RutaCarpetaBorradores));

                    string rutaExcel = ACA_ExportarExcelWeb.ExportarExcelOnMemory(dt, fileName, anot.RutaCarpetaBorradores);

                    if (!String.IsNullOrEmpty(rutaExcel))
                    {
                        HelperDocumentos helper_docs = new HelperDocumentos();
                        MAE_TipoDocumento tipoDoc = db.MAE_TipoDocumento.Find(64);//INFORME MENSUAL CODIGO 64 SEGÚN BBDD
                        MAE_ClassDoc mAE_ClassDoc = db.MAE_ClassDoc.Where(x => x.IdTipo == tipoDoc.IdTipo && x.IdTipoSub == anot.IdTipoSub).FirstOrDefault();

                        MAE_documentos newDoc = new MAE_documentos();
                        newDoc.UserId = User.Identity.GetUserId();
                        newDoc.NombreDoc = fileName;
                        newDoc.Descripcion = description;
                        newDoc.IdPath = mAE_ClassDoc.IdClassTwo;
                        newDoc.Extension = ".xlsx";
                        newDoc.Mb = 0;                   
                        newDoc.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                        newDoc.FechaCreacion = DateTime.Now;
                        newDoc.Ruta = rutaExcel;
                        db.MAE_documentos.Add(newDoc);
                        await db.SaveChangesAsync();

                        //SE AGREGA LA RELACIÓN AL LOD DOC ANOTACION
                        LOD_docAnotacion otros_docs = new LOD_docAnotacion()
                        {
                            IdDoc = newDoc.IdDoc,
                            IdTipoDoc = 64, //tipo informe mensual
                            IdAnotacion = informe.IdAnotacion,
                            EstadoDoc = 1,
                            MAE_documentos = newDoc,
                            IdContrato = anot.LOD_LibroObras.IdContrato
                        };
                        db.LOD_docAnotacion.Add(otros_docs);
                        await db.SaveChangesAsync();

                        //AGREGAMOS EL FOLIO AL INFORME Y CAMBIAMOS EL ESTADO
                        item.Estado = 2;
                        item.IdAnotacion = informe.IdAnotacion;
                        db.Entry(item).State = EntityState.Modified;
                        await db.SaveChangesAsync();
                        
                        string accion = $"Se ha agregado el documento: {fileName} en la anotación.";
                        bool response = await Log_Helper.SetObjectLog(0, anot, accion, GetUserInSession().Id);

                    }
                    else
                    {
                        val.Status = false;
                        val.ErrorMessage.Add("Ocurrió un problema al tratar de general el archivo Excel");
                    }
                }
                else
                {
                    IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
                    val.Status = false;
                    foreach (var err in allErrors)
                        val.ErrorMessage.Add(err.ErrorMessage);
                }
            }
            catch (Exception ex)
            {
                val.Status = false;
                val.ErrorMessage.Add(ex.Message);
            }

            return Json(val, JsonRequestBehavior.AllowGet);
        }

    }
}