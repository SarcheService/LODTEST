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
using LOD_APR.Areas.GLOD.ModelsViews;
using LOD_APR.Areas.Admin.Models;
using LOD_APR.Helpers;
using LOD_APR.Areas.GLOD.Helpers;
using System.IO;

namespace LOD_APR.Areas.GLOD.Controllers
{
    [CustomAuthorize]
    public class LiquidacionContratoController : Controller
    {
        private LOD_DB db = new LOD_DB();

        // GET: GLOD/LiquidacionContrato
        public async Task<ActionResult> Index()
        {
            string userid = User.Identity.GetUserId();
            List<int> contratos = db.LOD_UsuariosLod.Where(x => x.UserId == userid).Select(x => x.LOD_LibroObras.IdContrato).Distinct().ToList();

            var ContratosDisponibles = db.CON_Contratos.Where(x => contratos.Contains(x.IdContrato) && x.EstadoContrato == 1).ToList();
            var IdContratoInicial = 0;
            //if(ContratosDisponibles.Count > 0)
            //    IdContratoInicial = ContratosDisponibles.First().IdContrato;


            List<AutoSearch> search = new List<AutoSearch>();
            search.Add(new AutoSearch()
            {
                id = "0",
                name = "<Seleccione Empresa Contratista>"
            });

            foreach (var item in ContratosDisponibles)
            {
                search.Add(new AutoSearch()
                {
                    id = item.IdContrato.ToString(),
                    name = item.CodigoContrato.ToString() + "; " + item.NombreContrato.ToUpper()
                });
            }

            ViewBag.IdContrato = new SelectList(search, "id", "name", IdContratoInicial);

            if (ValidaPermisos.ValidaPermisosEnController("0020020000"))
            {

                return View();
            }
            else
            {
                return RedirectToAction("SinPermiso", "../Pub");
            }

        }

        public async Task<ActionResult> GetInfoContrato(int id)
        {
            CON_Contratos cON_Contratos = await db.CON_Contratos.FindAsync(id);
            ViewBag.NombreContrato = cON_Contratos.CodigoContrato + "-" + cON_Contratos.NombreContrato;
            ViewBag.EstadoContrato = cON_Contratos.EstadoContrato;
            ViewBag.TotalLibros = db.LOD_LibroObras.Where(x => x.IdContrato == cON_Contratos.IdContrato).ToList().Count();
            List<int> listIntReq = GetDocLiquidacion().Select(x => x.IdTipo).ToList();
            ViewBag.TotalDocumentosAprobados = db.LOD_docAnotacion.Where(x => x.LOD_Anotaciones.LOD_LibroObras.IdContrato == cON_Contratos.IdContrato && x.EstadoDoc == 2 && listIntReq.Contains(x.IdTipoDoc)).ToList().Count();
            ViewBag.LibrosPorCerrar = db.LOD_LibroObras.Where(x => x.IdContrato == cON_Contratos.IdContrato && x.Estado == 1).ToList().Count();
            ViewBag.IdContrato = cON_Contratos.IdContrato;

            ViewBag.EsInspectorFiscal = false;
            string userId = User.Identity.GetUserId();
            ViewBag.UsuarioActual = userId;

            List<LOD_RolesCttosContrato> rolesUsuario = db.LOD_UsuariosLod.Where(x => x.UserId == userId && x.LOD_LibroObras.IdContrato == cON_Contratos.IdContrato && x.Activo).Select(x => x.LOD_RolesCttosContrato).ToList();
            if (rolesUsuario.Select(x => x.IdRolCtto).Contains(2)) //2 es el rol Inspector Fiscal en la Tabla MAE_RolesContratos
                ViewBag.EsInspectorFiscal = true;


            return PartialView("_GetInfoContrato");
        }

        public async Task<ActionResult> GetDocumentos(int id)
        {
            CON_Contratos contrato = db.CON_Contratos.Find(id);

            List<DocumentosAnotacionView> listDocumentosAnotacion = new List<DocumentosAnotacionView>();

            LOD_LibroObras libroCom = db.LOD_LibroObras.Where(x => x.IdTipoLod == 2 && x.IdContrato == contrato.IdContrato).FirstOrDefault();

            ViewBag.PermiteCrear = false;
            if (libroCom.Estado == 1)
            {
                ViewBag.PermiteCrear = true;
            }
            List<string> listadoUsuarios = await db.LOD_Anotaciones.Where(x => x.LOD_LibroObras.IdContrato == contrato.IdContrato && x.IdLod == libroCom.IdLod).Select(x => x.UserId).ToListAsync();
            List<string> listadoUsuariosBorradores = await db.LOD_Anotaciones.Where(x => x.LOD_LibroObras.IdContrato == contrato.IdContrato && x.IdLod == libroCom.IdLod).Select(x => x.UserIdBorrador).ToListAsync();

            List<MAE_TipoDocumento> docRequeridos = GetDocLiquidacion();
            List<int> listIntReq = docRequeridos.Select(x => x.IdTipo).ToList();
            List<LOD_docAnotacion> docCargados = db.LOD_docAnotacion.Where(x => x.LOD_Anotaciones.LOD_LibroObras.IdContrato == contrato.IdContrato && listIntReq.Contains(x.IdTipoDoc)).ToList(); ;
            docRequeridos = docRequeridos.Except(docCargados.Select(x => x.MAE_TipoDocumento)).ToList();
            //****DETERMINAR SI LA ANOTACION AUN NO ESTÁ FIRMADA Y QUE EL USUARIO PERMITIDO PARA EL INGRESO DE LOS DOCUMENTOS
            //SEA EL USUARIO BORRADOR O EL USUARIO FIRMANTE SOLAMENTE
            bool isResp = false;
            string idUser = GetUserInSession().Id;
            LOD_UsuariosLod userLod = db.LOD_UsuariosLod.Where(x => x.UserId == idUser && x.IdLod == libroCom.IdLod && x.Activo).FirstOrDefault();

            if (listadoUsuarios.Contains(idUser) || listadoUsuariosBorradores.Contains(idUser) || userLod != null)
                isResp = true;
            //**************************************************************************************************************
            foreach (var item in docRequeridos)
            {
                MAE_CodSubCom maeCod = db.MAE_CodSubCom.Where(x => x.IdTipo == item.IdTipo).FirstOrDefault();
                string subtipo = "";
                string tipo = "";                
                if(maeCod != null)
                {
                    subtipo = maeCod.MAE_SubtipoComunicacion.Nombre;
                    tipo = maeCod.MAE_SubtipoComunicacion.MAE_TipoComunicacion.Nombre;
                }


                DocumentosAnotacionView newDoc = new DocumentosAnotacionView()
                {
                    IdAnotacion = libroCom.IdLod,
                    IdDocanot = 0,
                    IdEstado = 0,
                    IdTipoDoc = item.IdTipo,
                    TipoClasi = item.TipoClasi,
                    Descripcion = string.Empty,
                    Ruta = string.Empty,
                    TipoDoc = item.Tipo,
                    Titulo = string.Empty,
                    IsResponsable = isResp,
                    Subtipo = subtipo,
                    Tipo = tipo
                };
                listDocumentosAnotacion.Add(newDoc);
            }

            foreach (var item in docCargados)
            {
                MAE_CodSubCom maeCod = db.MAE_CodSubCom.Where(x => x.IdTipo == item.IdTipoDoc).FirstOrDefault();
                string subtipo = "";
                string tipo = "";
                if (maeCod != null)
                {
                    subtipo = maeCod.MAE_SubtipoComunicacion.Nombre;
                    tipo = maeCod.MAE_SubtipoComunicacion.MAE_TipoComunicacion.Nombre;
                }

                DocumentosAnotacionView newDoc = new DocumentosAnotacionView()
                {
                    IdAnotacion = item.IdAnotacion,
                    IdDocanot = item.IdDocAnotacion,
                    IdEstado = item.EstadoDoc,
                    IdTipoDoc = item.MAE_TipoDocumento.IdTipo,
                    TipoClasi = item.MAE_TipoDocumento.TipoClasi,
                    Descripcion = item.Observaciones,
                    Ruta = "/../../" + item.MAE_documentos.Ruta,
                    TipoDoc = item.MAE_TipoDocumento.Tipo,
                    Titulo = item.MAE_documentos.NombreDoc,
                    Subtipo = subtipo,
                    Tipo = tipo
                };
                listDocumentosAnotacion.Add(newDoc);
            }

            return PartialView("_GetDocumentos", listDocumentosAnotacion);

        }

        public async Task<ActionResult> GetLibros(int id)
        {
            CON_Contratos cON_Contratos = db.CON_Contratos.Find(id);
            List<LOD_LibroObras> libros = db.LOD_LibroObras.Where(x => x.IdContrato == id && (x.Estado == 1 || x.Estado == 2)).ToList();
            
            ViewBag.EsInspectorFiscal = false;
            string userId = User.Identity.GetUserId();
            ViewBag.UsuarioActual = userId;

            List<LOD_RolesCttosContrato> rolesUsuario = db.LOD_UsuariosLod.Where(x => x.UserId == userId && x.LOD_LibroObras.IdContrato == cON_Contratos.IdContrato && x.Activo).Select(x => x.LOD_RolesCttosContrato).ToList();
            if (rolesUsuario.Select(x => x.IdRolCtto).Contains(2)) //2 es el rol Inspector Fiscal en la Tabla MAE_RolesContratos
                ViewBag.EsInspectorFiscal = true;

            return PartialView("_GetTableLibros", libros);
        }

        public async Task<JsonResult> GetContratos(string id)
        {
            List<LOD_UsuariosLod> librosUser = await db.LOD_UsuariosLod.Where(x => x.UserId == id).ToListAsync();
            List<CON_Contratos> IdContratos = librosUser.Select(x => x.LOD_LibroObras.CON_Contratos).Distinct().ToList();

            List<SelectListItem> tipo = new List<SelectListItem>();
            

            foreach (var item in IdContratos)
                tipo.Add(new SelectListItem() { Text = item.CodigoContrato + "-" + item.NombreContrato, Value = item.IdContrato.ToString() });

            return Json(tipo, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> GetDocumentosData(int id)
        {
            CON_Contratos contrato = db.CON_Contratos.Find(id);

            List<DocumentosAnotacionView> listDocumentosAnotacion = new List<DocumentosAnotacionView>();

            LOD_LibroObras libroCom = db.LOD_LibroObras.Where(x => x.IdTipoLod == 2 && x.IdContrato == contrato.IdContrato).FirstOrDefault();
            
            List<string> listadoUsuarios = await db.LOD_Anotaciones.Where(x => x.LOD_LibroObras.IdContrato == contrato.IdContrato && x.IdLod == libroCom.IdLod).Select(x => x.UserId).ToListAsync();
            List<string> listadoUsuariosBorradores = await db.LOD_Anotaciones.Where(x => x.LOD_LibroObras.IdContrato == contrato.IdContrato && x.IdLod == libroCom.IdLod).Select(x => x.UserIdBorrador).ToListAsync();

            List<MAE_TipoDocumento> docRequeridos = GetDocLiquidacion();
            List<LOD_docAnotacion> docCargados = db.LOD_docAnotacion.Where(x => x.LOD_Anotaciones.LOD_LibroObras.IdContrato == contrato.IdContrato).ToList(); ;
            docRequeridos = docRequeridos.Except(docCargados.Select(x => x.MAE_TipoDocumento)).ToList();
            //****DETERMINAR SI LA ANOTACION AUN NO ESTÁ FIRMADA Y QUE EL USUARIO PERMITIDO PARA EL INGRESO DE LOS DOCUMENTOS
            //SEA EL USUARIO BORRADOR O EL USUARIO FIRMANTE SOLAMENTE
            bool isResp = false;
            string idUser = GetUserInSession().Id;
            LOD_UsuariosLod userLod = db.LOD_UsuariosLod.Where(x => x.UserId == idUser && x.IdLod == libroCom.IdLod).FirstOrDefault();

            if (listadoUsuarios.Contains(idUser) || listadoUsuariosBorradores.Contains(idUser) || userLod != null)
                isResp = true;
            //**************************************************************************************************************
            foreach (var item in docRequeridos)
            {
                DocumentosAnotacionView newDoc = new DocumentosAnotacionView()
                {
                    IdAnotacion = libroCom.IdLod,
                    IdDocanot = 0,
                    IdEstado = 0,
                    IdTipoDoc = item.IdTipo,
                    TipoClasi = item.TipoClasi,
                    Descripcion = string.Empty,
                    Ruta = string.Empty,
                    TipoDoc = item.Tipo,
                    Titulo = string.Empty,
                    IsResponsable = isResp
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
                };
                listDocumentosAnotacion.Add(newDoc);
            }

            return Json(listDocumentosAnotacion, JsonRequestBehavior.AllowGet);

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


        public async Task<ActionResult> LiquidarContrato(int IdContrato)
        {
            CON_Contratos contrato = db.CON_Contratos.Find(IdContrato);
            List<LOD_LibroObras> librosContrato = db.LOD_LibroObras.Where(x => x.IdContrato == contrato.IdContrato).ToList();
            List<LOD_LibroObras> librosCerrados = db.LOD_LibroObras.Where(x => x.IdContrato == contrato.IdContrato && x.Estado == 2).ToList();
            List<LOD_LibroObras> librosPorCerrar = db.LOD_LibroObras.Where(x => x.IdContrato == contrato.IdContrato && x.Estado == 1).ToList();
            List<MAE_TipoDocumento> listadoDocCargados = db.LOD_docAnotacion.Where(x => x.LOD_Anotaciones.LOD_LibroObras.IdContrato == contrato.IdContrato && x.LOD_Anotaciones.Estado == 2).Select(x => x.MAE_TipoDocumento).ToList();

            List<int> listadoInt = GetDocLiquidacion().Select(x => x.IdTipo).ToList();
            List<MAE_TipoDocumento> listaDocRequeridos = GetDocLiquidacion();
            List<MAE_TipoDocumento> listDocPorCargar = listaDocRequeridos.Except(listadoDocCargados).ToList();
            List<MAE_TipoDocumento> listNoAprobados = db.LOD_docAnotacion.Where(x => x.LOD_Anotaciones.LOD_LibroObras.IdContrato == contrato.IdContrato && x.LOD_Anotaciones.Estado == 2 && x.EstadoDoc == 1 && listadoInt.Contains(x.IdTipoDoc)).Select(x => x.MAE_TipoDocumento).ToList();

            LiquidacionLibroView liquidacion = new LiquidacionLibroView();
            liquidacion.IdContrato = contrato.IdContrato;
            liquidacion.contrato = contrato;
            liquidacion.librosPorFirmar = librosPorCerrar;
            liquidacion.listadoLiquidacion = listDocPorCargar;
            liquidacion.listadoNoAprobados = listNoAprobados;

            //LiquidacionLibroView liquidacion = new LiquidacionLibroView();
            //liquidacion.IdContrato = contrato.IdContrato;
            //liquidacion.contrato = contrato;
            //liquidacion.librosPorFirmar = new List<LOD_LibroObras>();
            //liquidacion.listadoLiquidacion = new List<MAE_TipoDocumento>();
            //liquidacion.listadoNoAprobados = new List<MAE_TipoDocumento>();

            ViewBag.PermiteCierre = true;
            if (liquidacion.librosPorFirmar.Count > 0 || liquidacion.listadoLiquidacion.Count > 0 || liquidacion.listadoNoAprobados.Count > 0)
                ViewBag.PermiteCierre = false;


            return PartialView("_LiquidacionContrato", liquidacion);
        }


        [HttpPost, ActionName("LiquidarContrato")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> LiquidarContrato(LiquidacionLibroView liquidacion)
        {
            try
            {
                CON_Contratos contrato = await db.CON_Contratos.FindAsync(liquidacion.IdContrato);
                contrato.EstadoContrato = 3; //CONSULTAR ESTADOS

                db.Entry(contrato).State = EntityState.Modified;
                db.SaveChanges();

                Log_Helper log_Helper = new Log_Helper();
                string accion = "Se ha liquidado el contrato: " + contrato.CodigoContrato + "-" + contrato.NombreContrato;
                bool res = await log_Helper.SetObjectLog(2, contrato, accion, User.Identity.GetUserId());

                string response = "true;"+contrato.IdContrato;
                return Content(response);
            }
            catch (Exception e)
            {
                return Content(e.Message);
            }
        }

        public async Task<ActionResult> CierreLibro(int IdLod)
        {
            LOD_LibroObras libro = db.LOD_LibroObras.Find(IdLod);
            List<LOD_Anotaciones> anotaciones = db.LOD_Anotaciones.Where(x => x.IdLod == libro.IdLod).ToList();
            List<LOD_Anotaciones> anotacionesFirmadas = db.LOD_Anotaciones.Where(x => x.IdLod == libro.IdLod && x.Estado == 2).ToList();
            List<LOD_Anotaciones> anotacionesPorFirmar = db.LOD_Anotaciones.Where(x => x.IdLod == libro.IdLod && x.Estado != 2).ToList();
            List<int> listadoInt = GetDocLiquidacion().Select(x => x.IdTipo).ToList();

            List<MAE_TipoDocumento> listadoDocCargados = db.LOD_docAnotacion.Where(x => x.LOD_Anotaciones.IdLod == libro.IdLod && x.LOD_Anotaciones.Estado == 2).Select(x => x.MAE_TipoDocumento).ToList();
            List<MAE_TipoDocumento> listadoNoAprobados = db.LOD_docAnotacion.Where(x => x.LOD_Anotaciones.IdLod == libro.IdLod && x.LOD_Anotaciones.Estado == 2 && x.EstadoDoc == 1 && listadoInt.Contains(x.IdTipoDoc)).Select(x => x.MAE_TipoDocumento).ToList();
            List<MAE_TipoDocumento> listaDocRequeridos = GetDocLiquidacion();
            List<MAE_TipoDocumento> listDocPorCargar = listaDocRequeridos.Except(listadoDocCargados).ToList();

            CierreLibroView cierreView = new CierreLibroView();
            cierreView.IdLibro = libro.IdLod;
            cierreView.LibroObra = libro;
            cierreView.listadoLiquidacion = listDocPorCargar;
            cierreView.listadoNoAprobados = listadoNoAprobados;

            int EstadoComunicacion = db.LOD_LibroObras.Where(x => x.IdContrato == libro.IdContrato && x.IdTipoLod == 2).Select(x => x.Estado.Value).FirstOrDefault();

            ViewBag.CierreLibroObra = true;
            if (libro.IdTipoLod == 1 && EstadoComunicacion != 2)
                ViewBag.CierreLibroObra = false;

            //CierreLibroView cierreView = new CierreLibroView();
            //cierreView.IdLibro = libro.IdLod;
            //cierreView.LibroObra = libro;
            //cierreView.listadoLiquidacion = new List<MAE_TipoDocumento>();
            //cierreView.listadoNoAprobados = new List<MAE_TipoDocumento>();

            ViewBag.PermiteCierre = true;
            if ((cierreView.listadoLiquidacion.Count > 0 || cierreView.listadoNoAprobados.Count > 0) && libro.IdTipoLod == 2) //IdTipoLod == 2, libro de comunicaciones
                ViewBag.PermiteCierre = false;

            return PartialView("_CierreLibro", cierreView);
        }


        [HttpPost, ActionName("CierreLibro")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CierreLibro(CierreLibroView cierreLibro)
        {
            try
            {
                LOD_LibroObras libroobra = await db.LOD_LibroObras.FindAsync(cierreLibro.IdLibro);
                libroobra.FechaCierre = DateTime.Now;
                libroobra.Estado = 2;
                string userid = User.Identity.GetUserId();
                libroobra.UsuarioCierre = userid;
                db.Entry(libroobra).State = EntityState.Modified;
                db.SaveChanges();
                string response = "true;" + libroobra.IdContrato;

                Log_Helper log_Helper = new Log_Helper();
                string accion = "Se ha cerrado el Libro: " + libroobra.NombreLibroObra +" del contrato "+libroobra.CON_Contratos.NombreContrato;
                bool res = await log_Helper.SetObjectLog(1, libroobra, accion, userid);

                return Content(response);
            }
            catch (Exception e)
            {
                return Content(e.Message);
            }
        }

        public List<MAE_TipoDocumento> GetDocLiquidacion()
        {
            List<int> listId = db.MAE_ClassDoc.Where(x => x.EsLiquidacion).Select(x => x.IdTipo).ToList();

            List<MAE_TipoDocumento> listado = db.MAE_TipoDocumento.Where(x => listId.Contains(x.IdTipo)).ToList();
            return listado;
        }

        public ActionResult AddAnotacion(int IdLod, int IdTipoDoc)
        {

            MAE_CodSubCom maeDoc = db.MAE_CodSubCom.Where(x => x.IdTipo == IdTipoDoc).FirstOrDefault();
            int IdTipoSub = 20; //Comunicación general
            if (maeDoc != null)
                IdTipoSub = maeDoc.IdTipoSub;

            MAE_SubtipoComunicacion subtipo = db.MAE_SubtipoComunicacion.Find(IdTipoSub);
            LOD_LibroObras libro = db.LOD_LibroObras.Find(IdLod);

            LOD_Anotaciones anot = new LOD_Anotaciones() { IdLod = IdLod, IdTipoSub = IdTipoSub, MAE_SubtipoComunicacion = subtipo, LOD_LibroObras = libro };
            return PartialView("_modalForm", anot);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddAnotacion([Bind(Include = "Titulo,Cuerpo,IdLod,IdTipoSub")] LOD_Anotaciones anotacion)
        {

            try
            {
                if (ModelState.IsValid)
                {
                    string userId = User.Identity.GetUserId();
                    var lod_UsuariosLod = db.LOD_UsuariosLod.Where(x => x.UserId == userId && x.IdLod == anotacion.IdLod).FirstOrDefault();
                    if (lod_UsuariosLod == null)
                    {
                        return Content("Error");
                    }

                    //GUARDAR ANOTACIÓN COMO BORRADOR EN BBDDD
                    anotacion.Correlativo = 0;
                    anotacion.EsEstructurada = false; //Depende del subtipo de anotación
                    anotacion.Estado = 0; //borrador
                    anotacion.FechaIngreso = DateTime.Now;
                    anotacion.SolicitudRest = false;
                    anotacion.SolicitudVB = false;
                    anotacion.TipoFirma = 1; //depende del tipo de Usuario y su perfil, estado userFirma avanzada
                    anotacion.EstadoFirma = false;
                    anotacion.UserIdBorrador = userId;
                    anotacion.FechaPub = null;
                    anotacion.FechaResp = null;
                    db.LOD_Anotaciones.Add(anotacion);
                    await db.SaveChangesAsync();

                    try
                    {
                        ////QUITAR
                        LOD_UserAnotacion userAnotacion = new LOD_UserAnotacion();
                        userAnotacion.IdAnotacion = anotacion.IdAnotacion;
                        userAnotacion.Destacado = false;
                        userAnotacion.Leido = true;
                        userAnotacion.EsPrincipal = false;
                        userAnotacion.EsRespRespuesta = false;
                        userAnotacion.IdUsLod = lod_UsuariosLod.IdUsLod;
                        userAnotacion.RespVB = false;
                        userAnotacion.VistoBueno = false;
                        db.LOD_UserAnotacion.Add(userAnotacion);
                        db.SaveChanges();
                        //*****************************************

                        Log_Helper helperLog = new Log_Helper();
                        string accion = "Se ha añadido una nueva anotación en estado Borrador";
                        bool response = await helperLog.SetLOGAnotacionAsync(anotacion, accion, User.Identity.GetUserId());
                    }
                    catch (Exception ex)
                    {
                        return Content("Error");
                    }

                }
                else
                {
                    return Content("Error");
                }
            }
            catch (Exception ex)
            {
                return Content("Error");
            }

            return RedirectToAction("Edit","Anotaciones", new { id = anotacion.IdAnotacion});
        }

        public ApplicationUser GetUserInSession()
        {
            string userName = User.Identity.GetUserId();
            var user = db.Users.Find(userName);
            return user;
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
