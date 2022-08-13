using LOD_APR.Areas.Admin.Models;
using LOD_APR.Areas.GLOD.Helpers;
using LOD_APR.Areas.GLOD.Models;
using LOD_APR.Areas.GLOD.ModelsViews;
using LOD_APR.Helpers;
using LOD_APR.Helpers.ModelsHelpers;
using LOD_APR.Models;
using MessagingToolkit.QRCode.Codec;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Reporting.WebForms;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using static LOD_APR.Models.AuxiliaresReport;

namespace LOD_APR.Areas.GLOD.Controllers
{
    [CustomAuthorize]
    public class AnotacionesController : Controller
    {

        private ApplicationUserManager _userManager;
        private LOD_DB db = new LOD_DB();
        private Log_Helper Log_Helper = new Log_Helper();
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        public ActionResult Index(int id, int? tipo)
        {
            LOD_LibroObras libro = db.LOD_LibroObras.Find(id);
            string userId = GetUserInSession().Id;
            //EL USUARIO DEBE TENER ACCESO AL LIBRO, SINO ES REDIRECCIONADO, ES PARA QUE NO INGRESEN POR URL
            LOD_UsuariosLod userLod = db.LOD_UsuariosLod.Where(x => x.IdLod == libro.IdLod && x.UserId == userId).FirstOrDefault();
            if (userLod == null)
                return RedirectToAction("InicioRapido", "Contratos");


            if (tipo == null) tipo = 1;
            ViewBag.Tipo = tipo;

            //PARA VALIDAR ROLES EN VISTA
            ViewBag.IdLod = libro.IdLod;
            ViewBag.IdUserActual = userId;



            List<SelectListItem> remitentes = new List<SelectListItem>();
            List<ApplicationUser> auxRemitentes = db.LOD_Anotaciones.Where(x => x.Estado == 2 && x.IdLod == libro.IdLod).Select(x => x.UsuarioRemitente).Distinct().ToList();
            foreach (var item in auxRemitentes)
            {
                remitentes.Add(new SelectListItem() { Text = String.Format("{0} {1}", item.Nombres, item.Apellidos), Value = item.Id });
            }

            List<SelectListItem> destinatario = new List<SelectListItem>();
            List<LOD_UsuariosLod> auxDestinatarios = db.LOD_UserAnotacion.Where(x => x.RespVB && x.LOD_Anotaciones.IdLod == libro.IdLod).Select(x => x.LOD_UsuarioLod).Distinct().ToList();
            foreach (var item in auxDestinatarios)
            {
                destinatario.Add(new SelectListItem() { Text = String.Format("{0} {1}", item.ApplicationUser.Nombres, item.ApplicationUser.Apellidos), Value = item.UserId });
            }

            ViewBag.IdRemitente = new SelectList(remitentes.ToList(), "Value", "Text"); ;
            ViewBag.IdDestinatario = new SelectList(destinatario.ToList(), "Value", "Text"); ;
            ViewBag.Anotaciones = GetAnotaciones(libro.IdLod);

            CON_Contratos contrato = db.CON_Contratos.Where(x => x.IdContrato == libro.IdContrato).FirstOrDefault();
            ViewBag.ContratoLiquidado = false;
            if (contrato.EstadoContrato == 3)
                ViewBag.ContratoLiquidado = true;

            return View(libro);
        }

        //*************************ANOTACION**********************************
        public ActionResult AddAnotacion(int IdLibro)
        {
            LOD_Anotaciones anot = new LOD_Anotaciones() { IdLod = IdLibro };
            return PartialView("_sidebarAddAnotacion", anot);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> AddAnotacion([Bind(Include = "Titulo,Cuerpo,IdLod,IdTipoSub")]LOD_Anotaciones anotacion)
        {
            Form_Validation val = new Form_Validation() { Status = true, ErrorMessage = new List<string>() };
            try
            {
                if (ModelState.IsValid)
                {
                    string userId = User.Identity.GetUserId();
                    var lod_UsuariosLod = db.LOD_UsuariosLod.Where(x => x.UserId == userId && x.IdLod == anotacion.IdLod).FirstOrDefault();
                    if (lod_UsuariosLod == null)
                    {
                        val.Status = false;
                        val.ErrorMessage.Add("El usuario NO tiene permisos para ingresar a este Libro.");
                        return Json(val, JsonRequestBehavior.AllowGet);
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
                    val.Parametros = $"/GLOD/Anotaciones/Edit/{anotacion.IdAnotacion}";

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


                        string accion = "Se ha añadido una nueva anotación en estado Borrador";
                        bool response = await Log_Helper.SetLOGAnotacionAsync(anotacion, accion, User.Identity.GetUserId());
                    }
                    catch (Exception ex)
                    {
                        val.Status = false;
                        val.ErrorMessage.Add(ex.Message);
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

        public async Task<ActionResult> ResponderAnotacion(int id)
        {
            LOD_Anotaciones anot = await db.LOD_Anotaciones.FindAsync(id);
            AnotacionRespuesta newAnot = new AnotacionRespuesta() { IdAnotacionRef = id, IdContrato = anot.LOD_LibroObras.IdContrato };
            return PartialView("_sidebarResponderAnotacion", newAnot);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> ResponderAnotacion([Bind(Include = "IdAnotacionRef,Titulo,Cuerpo,IdLibro,IdTipoSub")]AnotacionRespuesta respuesta)
        {
            Form_Validation val = new Form_Validation() { Status = true, ErrorMessage = new List<string>() };
            try
            {
                if (ModelState.IsValid)
                {
                    string userId = User.Identity.GetUserId();
                    var lod_UsuariosLod = db.LOD_UsuariosLod.Where(x => x.UserId == userId && x.IdLod == respuesta.IdLibro).FirstOrDefault();
                    if (lod_UsuariosLod == null)
                    {
                        val.Status = false;
                        val.ErrorMessage.Add("El usuario NO tiene permisos para ingresar a este Libro.");
                        return Json(val, JsonRequestBehavior.AllowGet);
                    }

                    LOD_Anotaciones anotacion = new LOD_Anotaciones();
                    anotacion.IdLod = respuesta.IdLibro;
                    anotacion.Titulo = respuesta.Titulo;
                    anotacion.Cuerpo = respuesta.Cuerpo;
                    anotacion.IdTipoSub = respuesta.IdTipoSub;
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
                    val.Parametros = $"/GLOD/Anotaciones/Edit/{anotacion.IdAnotacion}";

                    try
                    {
                        ////SE AGREGA UN NUEVO USUARIO A LA TABLA DE USER ANOTACION
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

                        LOD_ReferenciasAnot refe = new LOD_ReferenciasAnot()
                        {
                            IdAnotacion = anotacion.IdAnotacion,
                            IdAnontacionRef = respuesta.IdAnotacionRef,
                            EsRepuesta = true,
                            Observacion = "En respuesta a lo solicitado en este Folio"
                        };
                        db.LOD_ReferenciasAnot.Add(refe);
                        db.SaveChanges();

                        string accion = "Se ha añadido una nueva anotación en estado Borrador";
                        bool response = await Log_Helper.SetLOGAnotacionAsync(anotacion, accion, User.Identity.GetUserId());
                    }
                    catch (Exception ex)
                    {
                        val.Status = false;
                        val.ErrorMessage.Add(ex.Message);
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

        public async Task<ActionResult> Edit(int id, bool tabDoc = false)
        {
            string userid = User.Identity.GetUserId();
            LOD_Anotaciones anot = db.LOD_Anotaciones.Find(id);
            //VALIDA SI EL USUARIO TIENE PERMISOS PARA INGRESAR A ESE LIBRO O ANOTACIÓN, ESTO ES PARA QUE NO INGRESEN POR URL, SI TIENE ACCESO PERO NO TIENE PERMISOS DE LECTURA NO VERA EL LISTADO DE ANOTACIONES O NO PODRÁ CREAR ANOTACIONES, TAMPOCO VERÁ EL FILTRO
            //para comprobar los permisos que hay que ajustar y hacer pruebas es en la tabla PermisosRolesContratos con los registros que tienen el id 45 y 46
            LOD_UsuariosLod userLod = db.LOD_UsuariosLod.Where(x => x.IdLod == anot.IdLod && x.UserId == userid).FirstOrDefault();
            if (userLod == null)
                return RedirectToAction("InicioRapido", "Contratos");

            //PARA VALIDAR PERMISOS EN VISTA
            ViewBag.IdLod = anot.IdLod;
            ViewBag.IdUserActual = GetUserInSession().Id;
            ViewBag.TabDoc = tabDoc;
            

            var userAnotacionActual = db.LOD_UserAnotacion.Where(x => x.IdAnotacion == id && x.LOD_UsuarioLod.UserId == userid).FirstOrDefault();
            if (userAnotacionActual == null)
            {
                try
                {
                    LOD_UsuariosLod lOD_UsuariosLod = db.LOD_UsuariosLod.Where(x => x.UserId == userid).FirstOrDefault();
                    LOD_UserAnotacion lOD_UserAnotacion = new LOD_UserAnotacion();
                    lOD_UserAnotacion.IdUsLod = lOD_UsuariosLod.IdUsLod;
                    lOD_UserAnotacion.Leido = true;
                    lOD_UserAnotacion.IdAnotacion = id;
                    lOD_UserAnotacion.EsRespRespuesta = false;
                    lOD_UserAnotacion.EsPrincipal = false;
                    lOD_UserAnotacion.RespVB = false;
                    db.LOD_UserAnotacion.Add(lOD_UserAnotacion);
                    await db.SaveChangesAsync();
                }
                catch { }
            }
            else if (!userAnotacionActual.Leido)
            {
                userAnotacionActual.Leido = true;
                db.Entry(userAnotacionActual).State = EntityState.Modified;
                await db.SaveChangesAsync();
            }

            ViewBag.IdAnotacion = id;

            CON_Contratos contrato = db.CON_Contratos.Where(x => x.IdContrato == anot.LOD_LibroObras.IdContrato).FirstOrDefault();
            ViewBag.ContratoLiquidado = false;
            if (contrato.EstadoContrato == 3)
                ViewBag.ContratoLiquidado = true;

            return View();
        }

        public ActionResult DeleteReferencia(int id)
        {
            LOD_ReferenciasAnot referencia = db.LOD_ReferenciasAnot.Find(id);
            return PartialView("_sidebarDeleteReferencia", referencia);
        }

        [HttpPost]
        public async Task<JsonResult> DeleteReferencia(LOD_ReferenciasAnot refer)
        {
            Form_Validation val = new Form_Validation() { Status = true, ErrorMessage = new List<string>() };
            try
            {
                LOD_ReferenciasAnot referencia = db.LOD_ReferenciasAnot.Find(refer.IdRefAnot);
                if(referencia != null)
                {
                    LOD_ReferenciasAnot referenciaAnot = db.LOD_ReferenciasAnot.Find(refer.IdRefAnot);
                    LOD_Anotaciones anot = db.LOD_Anotaciones.Find(referenciaAnot.IdAnontacionRef);
                    db.LOD_ReferenciasAnot.Remove(referencia);
                    await db.SaveChangesAsync();
                    


                    string accion = "Se ha eliminado la referencia a la anotación '" + anot.Titulo + "'";
                    bool response = await Log_Helper.SetLOGAnotacionAsync(anot, accion, User.Identity.GetUserId());
                }
                else
                {
                    val.Status = false;
                    val.ErrorMessage.Add("La referencia se encuentra inhabilitada para su modificación.");
                }


            }catch(Exception ex)
            {
                val.Status = false;
                val.ErrorMessage.Add(ex.Message);
            }

            return Json(val, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<JsonResult> Update(AnotacionView anotacion)
        {
            Form_Validation val = new Form_Validation() { Status = true, ErrorMessage = new List<string>() };
            try
            {
                LOD_Anotaciones lOD_Anotaciones = db.LOD_Anotaciones.Find(anotacion.IdAnotacion);
                if (lOD_Anotaciones != null)
                {
                    lOD_Anotaciones.Titulo = anotacion.Titulo;
                    lOD_Anotaciones.Cuerpo = anotacion.Cuerpo;
                    lOD_Anotaciones.SolicitudRest = anotacion.SolicitudRest;
                    if (anotacion.SolicitudRest)
                        lOD_Anotaciones.FechaTopeRespuesta = Convert.ToDateTime(anotacion.FechaTopeRespuesta);
                    else
                        lOD_Anotaciones.FechaTopeRespuesta = null;

                    db.Entry(lOD_Anotaciones).State = EntityState.Modified;
                    await db.SaveChangesAsync();

                    string accion = "Se ha actualizado la anotación '" + lOD_Anotaciones.Titulo + "'";
                    bool response = await Log_Helper.SetLOGAnotacionAsync(lOD_Anotaciones, accion, User.Identity.GetUserId());
                }
                else
                {
                    val.Status = false;
                    val.ErrorMessage.Add("La Anotación se encuentra inhabilitada para su modificación.");
                }
            }
            catch (Exception ex)
            {
                val.Status = false;
                val.ErrorMessage.Add(ex.Message);
            }

            return Json(val, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> DeleteAnotacion(int id)
        {
            //BUSVAR ANOTACION EN BBDD
            LOD_Anotaciones anot = db.LOD_Anotaciones.Find(id);
            return PartialView("_sidebarDeleteAnotacion", anot);
        }

        [HttpPost, ActionName("DeleteAnotacion")]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> DeleteAnotacionConfirm(int IdAnotacion)
        {
            Form_Validation val = new Form_Validation() { Status = true, ErrorMessage = new List<string>() };
            try
            {
                //VALIDAR QUE SE PUEDA BORRAR LA ANOTACION (SI NO ESTA FIRMADA, ETC)
                LOD_Anotaciones anot = db.LOD_Anotaciones.Find(IdAnotacion);
                int idlibro = anot.IdLod;
                if (!anot.EstadoFirma)
                {
                    db.LOD_Anotaciones.Remove(anot);
                    await db.SaveChangesAsync();
                    //*****************************************
                    val.Parametros = $"/GLOD/Anotaciones/Index/" + idlibro;

                    string accion = "Se ha borrado la anotación: '" + anot.IdAnotacion + "'";
                    bool response = await Log_Helper.SetLOGAnotacionAsync(anot, accion, User.Identity.GetUserId());
                }
                else
                {
                    val.Status = false;
                    val.ErrorMessage.Add("La Anotación se encuentra inhabilitada para su eliminación.");
                }
            }
            catch (Exception ex)
            {
                val.Status = false;
                val.ErrorMessage.Add(ex.Message);
            }

            return Json(val, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> AddReferencia(int id)
        {
            LOD_Anotaciones anot = await db.LOD_Anotaciones.FindAsync(id);
            ReferenciasView refe = new ReferenciasView()
            {
                IdAnotacion = id,
                IdContrato = anot.LOD_LibroObras.IdContrato
            };
            return PartialView("_sidebarAddReferencia", refe);
        }

        [HttpPost, ActionName("AddReferencia")]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> AddReferenciaConfirm(ReferenciasView referencia)
        {
            Form_Validation val = new Form_Validation() { Status = true, ErrorMessage = new List<string>() };
            try
            {
                if (ModelState.IsValid)
                {
                    //VALIDAR QUE SE PUEDA BORRAR LA ANOTACION (SI NO ESTA FIRMADA, ETC)
                    var anot = await db.LOD_Anotaciones.Where(a => a.IdAnotacion == referencia.IdAnotacion && a.EstadoFirma == false).FirstOrDefaultAsync();
                    if (anot == null)
                    {
                        val.Status = false;
                        val.ErrorMessage.Add("La Anotación se encuentra inhabilitada para su modificación.");
                    }
                    else
                    {
                        LOD_ReferenciasAnot refe = new LOD_ReferenciasAnot()
                        {
                            IdAnontacionRef = referencia.IdAnotacionRef.Value,
                            IdAnotacion = referencia.IdAnotacion,
                            EsRepuesta = false,
                            Observacion = referencia.Observacion
                        };
                        db.LOD_ReferenciasAnot.Add(refe);
                        await db.SaveChangesAsync();
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

        //********************************************************************

        //*************************FIRMA**************************************
        public async Task<ActionResult> SolicitarFirma(int id)
        {
            LOD_Anotaciones anot = await db.LOD_Anotaciones.FindAsync(id);
            ViewBag.IdAnotacion = id;
            List<LOD_UsuariosLod> listReceptores = new List<LOD_UsuariosLod>();
            List<LOD_UsuariosLod> listLOD = db.LOD_UsuariosLod.Where(x => x.IdLod == anot.IdLod && x.Activo).ToList();
            List<LOD_UsuariosLod> listAnotaciones = db.LOD_UserAnotacion.Where(x => x.IdAnotacion == anot.IdAnotacion).Select(x => x.LOD_UsuarioLod).Distinct().ToList();
            listReceptores = listLOD.Except(listAnotaciones).ToList();

            List<SelectListItem> receptores = new List<SelectListItem>();
            foreach (var item in listReceptores)
            {
                receptores.Add(new SelectListItem() { Text = item.ApplicationUser.Nombres + " " + item.ApplicationUser.Apellidos, Value = item.IdUsLod.ToString() });
            }

            ViewBag.IdRemitente = new SelectList(receptores.OrderBy(x => x.Text), "Value", "Text");

            return PartialView("_sidebarSolicitarFirma");
        }

        [HttpPost, ActionName("SolicitarFirma")]
        public async Task<JsonResult> SolicitarFirmaConfirmed(int IdAnotacion, int IdRemitente)
        {
            Form_Validation val = new Form_Validation() { Status = false, ErrorMessage = new List<string>() };

            LOD_Anotaciones anot = await db.LOD_Anotaciones.FindAsync(IdAnotacion);
            if (anot == null)
                val.ErrorMessage.Add("Ocurrió un Problema al tratar de buscar la Anotación.");

            if (anot.EstadoFirma)
                val.ErrorMessage.Add("La anotación se encuentra inhabilitada para efectuar cambios.");

            try
            {
                LOD_UsuariosLod userLod = await db.LOD_UsuariosLod.FindAsync(IdRemitente);
                anot.Estado = 1;
                anot.UserId = userLod.UserId;
                db.Entry(anot).State = EntityState.Modified;
                await db.SaveChangesAsync();

                val.Status = true;
                string accion = "Solicitud de Firma Enviada Correctamente";
                bool response = await Log_Helper.SetLOGAnotacionAsync(anot, accion, User.Identity.GetUserId());

                GLOD_Notificaciones noty = new GLOD_Notificaciones();
                int result = await noty.NotificarSolicitudFirma(anot, GetUserInSession().Id, userLod.UserId);

                val.Status = true;
            }
            catch (Exception ex)
            {
                val.ErrorMessage.Add(ex.Message);
            }

            return Json(val, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> ReenviarAnotacion(int id)
        {
            LOD_Anotaciones anot = await db.LOD_Anotaciones.FindAsync(id);
            ViewBag.IdAnotacion = id;

            List<LOD_UsuariosLod> listAnotaciones = db.LOD_UsuariosLod.Where(x => x.IdLod== anot.IdLod).ToList();

            List<SelectListItem> receptores = new List<SelectListItem>();
            foreach (var item in listAnotaciones)
            {
                receptores.Add(new SelectListItem() { Text = item.ApplicationUser.NombreCompleto + " | " + item.LOD_RolesCttosContrato.NombreRol, Value = item.IdUsLod.ToString() });
            }

            ViewBag.IdRemitente = new SelectList(receptores.OrderBy(x => x.Text), "Value", "Text");

            return PartialView("_sidebarReenviarAnotacion");
        }

        [HttpPost, ActionName("ReenviarAnotacion")]
        public async Task<JsonResult> ReenviarAnotacion(int IdAnotacion, int IdRemitente)
        {
            Form_Validation val = new Form_Validation() { Status = true, ErrorMessage = new List<string>() };

            LOD_Anotaciones anot = await db.LOD_Anotaciones.FindAsync(IdAnotacion);
            if (anot == null)
                val.ErrorMessage.Add("Ocurrió un Problema al tratar de buscar la Anotación.");


            try
            {
                LOD_UsuariosLod userLod = await db.LOD_UsuariosLod.FindAsync(IdRemitente);

                GLOD_Notificaciones noty = new GLOD_Notificaciones(); //NOTIFICACIÓN
                int result = await noty.NotificarReenvio(anot, GetUserInSession().Id, userLod.UserId);
            }
            catch (Exception ex)
            {
                val.Status = false;
                val.ErrorMessage.Add(ex.Message);
            }

            return Json(val, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> VBAnotacion(int id)
        {
            var user = GetUserInSession();

            ViewBag.IdAnotacion = id;
            ViewBag.IdLod = db.LOD_Anotaciones.Find(id).IdLod;
            ViewBag.IdUserActual = user.Id;

            ViewBag.EsGubernamental = user.MAE_Sucursal.MAE_sujetoEconomico.EsGubernamental;

            return PartialView("_sidebarVBAnotacion");
        }

        [HttpPost, ActionName("VBAnotacion")]
        public async Task<JsonResult> VBAnotacionConfirmed(int IdAnotacion, int tipo, string password)
        {
            Form_Validation val = new Form_Validation() { Status = false, ErrorMessage = new List<string>() };
            //BUSCAR ANOTACION EN BBDD
            LOD_Anotaciones anot = await db.LOD_Anotaciones.FindAsync(IdAnotacion);
            if (String.IsNullOrEmpty(password) && tipo > 1)
            {
                val.ErrorMessage.Add("El Password/OTP es obligatorio.");
                return Json(val, JsonRequestBehavior.AllowGet);
            }

            if (anot == null)
                val.ErrorMessage.Add("Ocurrió un Problema al tratar de buscar la Anotación.");

            if (!anot.EstadoFirma)
                val.ErrorMessage.Add("La anotación aún no ha sido firmada y emitida.");

            //FirmarAnotacion firm = new FirmarAnotacion();
           // string path = await firm.PathDescargarAnotacion(IdAnotacion);

            try
            {
                if (tipo == 1)//FEA
                {
                    Random rnd = new Random();
                    int first = rnd.Next(0, 9);
                    int second = rnd.Next(0, 99);
                    string ambiente = ConfigurationManager.AppSettings.Get("Ambiente").ToString();

                    string userid = User.Identity.GetUserId();
                    var userAnotacionActual = db.LOD_UserAnotacion.Where(x => x.IdAnotacion == IdAnotacion && x.LOD_UsuarioLod.UserId == userid).FirstOrDefault();
                    userAnotacionActual.TempCode = $"{ambiente}V{first}-{second.ToString().PadLeft(2, '0')}";

                    db.Entry(userAnotacionActual).State = EntityState.Modified;
                    await db.SaveChangesAsync();

                    val.Status = true;
                    val.Parametros = userAnotacionActual.TempCode;

                    string accion = "Toma de conocimiento realizado Correctamente";
                    bool response = await Log_Helper.SetLOGAnotacionAsync(anot, accion, User.Identity.GetUserId());

                    return Json(val, JsonRequestBehavior.AllowGet);

                }
                else if (tipo == 2)//MINSEGPRE
                {
                    FirmarAnotacion firm = new FirmarAnotacion();
                    string pdfBase64 = await firm.AnotacionPDFToBase64(Path.Combine(Server.MapPath("~/"),anot.RutaPdfConFirma));
                    if (!String.IsNullOrEmpty(pdfBase64))
                    {
                        string userId = User.Identity.GetUserId();
                        var run = db.Users.Find(userId);
                        string mop_api_token_key = ConfigurationManager.AppSettings.Get("mop_api_token_key").ToString();
                        string mop_api_token_secret = ConfigurationManager.AppSettings.Get("mop_api_token_secret").ToString();

                        JWT token = new JWT();
                        string tokenFirmado = token.GenerateJwtToken(mop_api_token_secret, "Propósito General", "Dirección General de Obras Públicas", run.RunToken);
                        MOP_Post_Sign mop = new MOP_Post_Sign()
                        {
                            api_token_key = mop_api_token_key,
                            token = tokenFirmado,
                            files = new List<ModelsViews.File>(),
                            otp = password
                        };
                        ModelsViews.File pdf = new ModelsViews.File()
                        {
                            contentType = "application/pdf",
                            checksum = "",
                            content = pdfBase64,
                            description = "Anotación Folio " + anot.Correlativo.ToString().PadLeft(6, '0')
                        };
                        mop.files.Add(pdf);

                        MOP_Post_Response result = await Mop_API.API_postListSMS(mop);

                        if (String.IsNullOrEmpty(result.error))
                        {
                            //LOD_Anotaciones anota = db.LOD_Anotaciones.re.Find(IdAnotacion);
                            MemoryStream ms = new MemoryStream(Convert.FromBase64String(result.files[0].content));
                            string tempPath = Path.Combine(Server.MapPath("~/"), anot.RutaCarpetaPdf);

                            if (!Directory.Exists(tempPath))
                                Directory.CreateDirectory(tempPath);

                            string informeTempPath = Path.Combine(Server.MapPath("~/"), anot.RutaPdfConFirma);

                            FileStream newFile = new FileStream(informeTempPath, FileMode.Create, FileAccess.Write);
                            ms.WriteTo(newFile);
                            newFile.Close();

                            //System.IO.File.Delete(filePath);
                            val.Status = true;
                            val.Parametros = $"/GLOD/Anotaciones/Edit/{IdAnotacion}";
                            string accion = "Toma de conocimiento realizado Correctamente";
                            bool response = await Log_Helper.SetLOGAnotacionAsync(anot, accion, User.Identity.GetUserId());
                        }
                        else
                        {
                            val.ErrorMessage.Add(result.error);
                        }
                    }
                    else
                    {
                        val.ErrorMessage.Add("Ocurrió un Problema al tratar de Convertir a Base 64 el archivo pdf de la Anotación.");
                    }
                }
                else
                {
                    var valid = (await UserManager.PasswordValidator.ValidateAsync(password)).Succeeded;
                    if (valid)
                    {
                        val.Parametros = $"/GLOD/Anotaciones/Edit/{anot.IdAnotacion}";
                        string accion = "Toma de Conocimiento Firmada Correctamente";
                        val.Status = true;
                        bool response = await Log_Helper.SetLOGAnotacionAsync(anot, accion, User.Identity.GetUserId());
                    }
                    else
                    {
                        val.ErrorMessage.Add("El password ingresado no es válido para el usuario en sesión.");
                    }

                }
            }
            catch (Exception ex)
            {
                val.ErrorMessage.Add(ex.Message);
            }

            if (val.Status && tipo > 1)
            {
                try
                {
                    string userid = User.Identity.GetUserId();
                    var userAnotacionActual = db.LOD_UserAnotacion.Where(x => x.IdAnotacion == IdAnotacion && x.LOD_UsuarioLod.UserId == userid).FirstOrDefault();
                    if (userAnotacionActual != null)
                    {
                        userAnotacionActual.FechaVB = DateTime.Now;
                        userAnotacionActual.VistoBueno = true;
                        userAnotacionActual.TipoVB = tipo;
                        db.Entry(userAnotacionActual).State = EntityState.Modified;
                        await db.SaveChangesAsync();
                        GLOD_Notificaciones noty = new GLOD_Notificaciones();  //NOTIFICACIÓN
                        int resu = await noty.NotificarVB(anot, GetUserInSession().Id);
                    }

                    string accion = $"{userAnotacionActual.LOD_UsuarioLod.ApplicationUser.NombreCompleto} confirma Toma de Conocimiento correctamente";
                    bool response = await Log_Helper.SetLOGAnotacionAsync(anot, accion, User.Identity.GetUserId());
                }
                catch { }
            }

            return Json(val, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> FirmarAnotacion(int id)
        {
            var user = GetUserInSession();

            ViewBag.IdAnotacion = id;
            ViewBag.IdLod = db.LOD_Anotaciones.Find(id).IdLod;
            ViewBag.IdUserActual = user.Id;
            
            ViewBag.EsGubernamental = user.MAE_Sucursal.MAE_sujetoEconomico.EsGubernamental;

            return PartialView("_sidebarFirmarAnotacion");
        }

        [HttpPost, ActionName("FirmarAnotacion")]
        public async Task<JsonResult> FirmarAnotacionConfirmed(int IdAnotacion, int tipo, string password)
        {
            Form_Validation val = new Form_Validation() { Status = false, ErrorMessage = new List<string>() };
            if (String.IsNullOrEmpty(password) && tipo > 1)
            {
                val.ErrorMessage.Add("El Password/OTP es obligatorio.");
                return Json(val, JsonRequestBehavior.AllowGet);
            }

            //BUSCAR ANOTACION EN BBDD
            LOD_Anotaciones anot = await db.LOD_Anotaciones.FindAsync(IdAnotacion);
            if (anot == null)
            {
                val.ErrorMessage.Add("Ocurrió un Problema al tratar de buscar la Anotación.");
                return Json(val, JsonRequestBehavior.AllowGet);
            }

            if (anot.EstadoFirma)
            {
                val.ErrorMessage.Add("La anotación se encuentra inhabilitada para efectuar cambios.");
                return Json(val, JsonRequestBehavior.AllowGet);
            }

            if (await IsDestPendientes(IdAnotacion))
            {
                bool response = await SetDocsPendientes(IdAnotacion);
                
                //val.ErrorMessage.Add("Debe agregar a lo menos un Receptor en la anotación.");
                //return Json(val, JsonRequestBehavior.AllowGet);
            }else if(await IsDestPrincipal(IdAnotacion) == false && anot.MAE_SubtipoComunicacion.MAE_TipoComunicacion.IdTipoLod == 1)
            {
                bool response = await SetDocsPendientes(IdAnotacion);
            }
            

            if (await IsDocsPendientes(IdAnotacion))
            {
                val.ErrorMessage.Add("Hay documentos obligatorios que no han sido cargados en la anotación.");
                return Json(val, JsonRequestBehavior.AllowGet);
            }

            //if (await IsPrincipalPendiente(IdAnotacion))
            //{
            //    val.ErrorMessage.Add("Debe agregar a lo menos un Receptor como Principal en la anotación.");
            //    return Json(val, JsonRequestBehavior.AllowGet);
            //}

            //if (await IsRespRespuestaPendiente(IdAnotacion, anot.SolicitudRest))
            //{
            //    val.ErrorMessage.Add("Debe agregar a lo menos un Receptor como Responsable de Respuesta en la anotación.");
            //    return Json(val, JsonRequestBehavior.AllowGet);
            //}

            FirmarAnotacion firm = new FirmarAnotacion();
            string filePath = string.Empty;
            if (tipo > 1)
            {
                if (await firm.FirmarAnotacionDB(IdAnotacion, tipo, User.Identity.GetUserId()))
                {
                    db.Entry(anot).Reload(); //REFRESCAMOS LA ANOTACION EN MEMORIA CON LOS NUEVOS CAMBIOS DE LA BBDD
                    filePath = await firm.GeneratePDF(IdAnotacion);
                    if (String.IsNullOrEmpty(filePath))
                    {
                        val.ErrorMessage.Add("Ocurrió un Problema al tratar de guardar el archivo pdf de la Anotación.");
                        await firm.QuitarFirmaAnotacionDB(IdAnotacion);
                        return Json(val, JsonRequestBehavior.AllowGet);
                    }

                    GLOD_Notificaciones notificaciones = new GLOD_Notificaciones();
                    string userid = User.Identity.GetUserId();
                    LOD_ReferenciasAnot referencia = db.LOD_ReferenciasAnot.Where(x => x.IdAnotacion == anot.IdAnotacion).FirstOrDefault();
                    int resu = 0;
                    if (referencia == null)  //NOTIFICAR PUBLICACION O NOTIFICAR RESPUESTA
                        resu = await notificaciones.NotificarPublicacion(anot, userid);
                    else
                        resu = await notificaciones.NotificarRespuesta(anot, userid);
                }
                else
                {
                    val.ErrorMessage.Add("Ocurrió un Problema al tratar de guardar los datos de firma de la Anotación.");
                    return Json(val, JsonRequestBehavior.AllowGet);
                }
            }

            try
            {
                if (tipo == 1)//FEA
                {
                    Random rnd = new Random();
                    int first = rnd.Next(0, 99);
                    int second = rnd.Next(0, 99);
                    string ambiente = ConfigurationManager.AppSettings.Get("Ambiente").ToString();

                    anot.TempCode = $"{ambiente}{first.ToString().PadLeft(2, '0')}-{second.ToString().PadLeft(2, '0')}";
                    anot.UserId = User.Identity.GetUserId();

                    db.Entry(anot).State = EntityState.Modified;
                    await db.SaveChangesAsync();

                    val.Status = true;
                    val.Parametros = anot.TempCode;
                    return Json(val, JsonRequestBehavior.AllowGet);
                }
                else if (tipo == 2)//MINSEGPRE
                {
                    string pdfBase64 = await firm.AnotacionPDFToBase64(filePath);
                    if (!String.IsNullOrEmpty(pdfBase64))
                    {
                        string userId = User.Identity.GetUserId();
                        var run = db.Users.Find(userId);
                        string mop_api_token_key = ConfigurationManager.AppSettings.Get("mop_api_token_key").ToString();
                        string mop_api_token_secret = ConfigurationManager.AppSettings.Get("mop_api_token_secret").ToString();

                        JWT token = new JWT();
                        string tokenFirmado = token.GenerateJwtToken(mop_api_token_secret, "Propósito General", "Dirección General de Obras Públicas", run.RunToken);
                        MOP_Post_Sign mop = new MOP_Post_Sign()
                        {
                            api_token_key = mop_api_token_key,
                            token = tokenFirmado,
                            files = new List<ModelsViews.File>(),
                            otp = password
                        };
                        ModelsViews.File pdf = new ModelsViews.File()
                        {
                            contentType = "application/pdf",
                            checksum = "",
                            content = pdfBase64,
                            description = "Anotación Folio " + anot.Correlativo.ToString().PadLeft(6, '0')
                        };
                        mop.files.Add(pdf);

                        MOP_Post_Response result = await Mop_API.API_postListSMS(mop);

                        if (String.IsNullOrEmpty(result.error))
                        {
                            //LOD_Anotaciones anota = db.LOD_Anotaciones.re.Find(IdAnotacion);
                            MemoryStream ms = new MemoryStream(Convert.FromBase64String(result.files[0].content));
                            string tempPath = Path.Combine(Server.MapPath("~/"), anot.RutaCarpetaPdf);

                            if (!Directory.Exists(tempPath))
                                Directory.CreateDirectory(tempPath);

                            string informeTempPath = Path.Combine(Server.MapPath("~/"), anot.RutaPdfConFirma);

                            FileStream newFile = new FileStream(informeTempPath, FileMode.Create, FileAccess.Write);
                            ms.WriteTo(newFile);
                            newFile.Close();

                            System.IO.File.Delete(filePath);
                            val.Status = true;
                            val.Parametros = $"/GLOD/Anotaciones/Edit/{IdAnotacion}";
                            string accion = "Anotación Firmada Correctamente";
                            bool response = await Log_Helper.SetLOGAnotacionAsync(anot, accion, User.Identity.GetUserId());
                        }
                        else
                        {
                            val.ErrorMessage.Add(result.error);
                        }
                    }
                    else
                    {
                        val.ErrorMessage.Add("Ocurrió un Problema al tratar de Convertir a Base 64 el archivo pdf de la Anotación.");
                    }
                }
                else
                {
                    var valid = (await UserManager.PasswordValidator.ValidateAsync(password)).Succeeded;

                    if (valid)
                    {
                        val.Parametros = $"/GLOD/Anotaciones/Edit/{anot.IdAnotacion}";
                        System.IO.File.Move(filePath, filePath.Replace(".preview", ""));
                        string accion = "Anotación Firmada Correctamente";
                        val.Status = true;
                        bool response = await Log_Helper.SetLOGAnotacionAsync(anot, accion, User.Identity.GetUserId());
                    }
                    else
                    {
                        val.ErrorMessage.Add("El password ingresado no es válido para el usuario en sesión.");
                    }

                }
            }
            catch (Exception ex)
            {
                val.ErrorMessage.Add(ex.Message);
            }

            if (!val.Status)
            {
                try
                {
                    await firm.QuitarFirmaAnotacionDB(IdAnotacion);
                    bool response2 = await SetNoAprobadoDocsPendientes(IdAnotacion);
                    System.IO.File.Delete(filePath);
                }
                catch { }
            }
            else
            {


                if (tipo > 1)//CON FIRMA FEA NO SE DEBEN MOVER LOS ADJUNTOS DEBIDO A QUE LO HACE EN API
                {
                    try
                    {
                        List<LOD_docAnotacion> docsAnotacion = await db.LOD_docAnotacion.Where(d => d.IdAnotacion == IdAnotacion).ToListAsync();
                        string rutaBase = System.IO.Path.GetDirectoryName(filePath);
                        foreach (var doc in docsAnotacion)
                        {
                            string borrador = Path.Combine(Server.MapPath("~/"), doc.MAE_documentos.Ruta);
                            string destino = Path.Combine(Server.MapPath("~/"), anot.RutaCarpetaPdf, doc.MAE_documentos.NombreDoc);
                            System.IO.File.Move(borrador, destino);

                            doc.MAE_documentos.Ruta = $"{anot.RutaCarpetaPdf}/{doc.MAE_documentos.NombreDoc}";
                            db.Entry(doc).State = EntityState.Modified;
                            await db.SaveChangesAsync();
                        }
                        System.IO.Directory.Delete(Path.Combine(Server.MapPath("~/"), anot.RutaCarpetaBorradores));
                    }
                    catch { }
                }
            }

            return Json(val, JsonRequestBehavior.AllowGet);
        }
        public async Task<JsonResult> GetSignState(int id)
        {
            Form_Validation val = new Form_Validation() { Status = false, ErrorMessage = new List<string>() };
            try
            {
                var anot = await db.LOD_Anotaciones.Where(a => a.EstadoFirma && a.IdAnotacion == id && a.TempCode == null).FirstOrDefaultAsync();
                if (anot != null)
                    val.Status = true;

            }
            catch (Exception ex)
            {
                val.Status = false;
                val.ErrorMessage.Add(ex.Message);
            }

            return Json(val, JsonRequestBehavior.AllowGet);
        }
        public async Task<JsonResult> GetVBState(int id, string code)
        {
            Form_Validation val = new Form_Validation() { Status = false, ErrorMessage = new List<string>() };
            try
            {
                var anot = await db.LOD_UserAnotacion.Where(a => a.LOD_Anotaciones.EstadoFirma && a.IdAnotacion == id && a.TempCode == code && a.FechaVB != null).FirstOrDefaultAsync();
                if (anot != null)
                    val.Status = true;

            }
            catch (Exception ex)
            {
                val.Status = false;
                val.ErrorMessage.Add(ex.Message);
            }

            return Json(val, JsonRequestBehavior.AllowGet);
        }
        //********************************************************************

        //*************************RECEPTORES*********************************
        public async Task<ActionResult> AddReceptor(int id)
        {
            //BUSCAR ANOTACION EN BBDD INCLUYENDO LOS RECEPTORES HABILITADOS Y QUE YA NO ESTEN EN LA ANOTACION
            LOD_Anotaciones anotacion = db.LOD_Anotaciones.Find(id);

            List<LOD_UsuariosLod> listReceptores = new List<LOD_UsuariosLod>();
            
            List<LOD_UsuariosLod> listLOD = db.LOD_UsuariosLod.Where(x => x.IdLod == anotacion.IdLod && x.Activo).ToList();


            List<LOD_UsuariosLod> listAnotaciones = db.LOD_UserAnotacion.Where(x => x.IdAnotacion == anotacion.IdAnotacion).Select(x => x.LOD_UsuarioLod).ToList();
            listReceptores = listLOD.Except(listAnotaciones).ToList();

            List<SelectListItem> receptores = new List<SelectListItem>();
            foreach (var item in listReceptores)
            {
                receptores.Add(new SelectListItem() { Text = item.ApplicationUser.Nombres + " " + item.ApplicationUser.Apellidos, Value = item.IdUsLod.ToString() });
            }

            ViewBag.IdReceptor = new SelectList(receptores.OrderBy(x => x.Text), "Value", "Text");
            ViewBag.IdAnotacion = id;
            
            ViewBag.PermiteResponsable = true;
            ViewBag.PermiteReceptor = true;

            if (db.LOD_UserAnotacion.Where(x => x.IdAnotacion == anotacion.IdAnotacion && x.EsPrincipal).FirstOrDefault() != null)
                ViewBag.PermiteReceptor = false;

            if (db.LOD_UserAnotacion.Where(x => x.IdAnotacion == anotacion.IdAnotacion && x.EsRespRespuesta).FirstOrDefault() != null)
                ViewBag.PermiteResponsable = false;

            return PartialView("_sidebarAddReceptor");
        }

        [HttpPost, ActionName("AddReceptor")]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> AddReceptorConfirm(Receptor receptor)
        {
            Form_Validation val = new Form_Validation() { Status = true, ErrorMessage = new List<string>() };
            try
            {
                //VALIDAR QUE SE PUEDA AGREGAR AL NUEVO RECEPTOR (SI NO ESTA FIRMADA, ETC)
                LOD_Anotaciones anot = db.LOD_Anotaciones.Find(receptor.IdAnotacion);
                if (!anot.EstadoFirma)
                {
                    LOD_UsuariosLod lOD_UsuariosLod = db.LOD_UsuariosLod.Where(x => x.IdUsLod == receptor.IdReceptor).FirstOrDefault();
                    LOD_UserAnotacion lOD_UserAnotacion = new LOD_UserAnotacion();
                    lOD_UserAnotacion.IdUsLod = lOD_UsuariosLod.IdUsLod;
                    lOD_UserAnotacion.Leido = false;
                    lOD_UserAnotacion.IdAnotacion = anot.IdAnotacion;
                    lOD_UserAnotacion.EsRespRespuesta = receptor.RespRespuesta;
                    lOD_UserAnotacion.EsPrincipal = receptor.EsPrincipal;
                    lOD_UserAnotacion.RespVB = true;
                    db.LOD_UserAnotacion.Add(lOD_UserAnotacion);
                    await db.SaveChangesAsync();

                    string accion = "Se ha agregado un nuevo receptor:" + lOD_UsuariosLod.ApplicationUser.Nombres + " " + lOD_UsuariosLod.ApplicationUser.Apellidos + " en la anotación: '" + lOD_UserAnotacion.IdAnotacion + "'";
                    bool response = await Log_Helper.SetLOGAnotacionAsync(anot, accion, User.Identity.GetUserId());
                }
                else
                {
                    val.Status = false;
                    val.ErrorMessage.Add("La Anotación se encuentra inhabilitada para agregar nuevos receptores.");
                }
            }
            catch (Exception ex)
            {
                val.Status = false;
                val.ErrorMessage.Add(ex.Message);
            }

            return Json(val, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> DeleteReceptor(int id, int idReceptor)
        {
            LOD_Anotaciones lOD_Anotaciones = db.LOD_Anotaciones.Find(id);
            LOD_UserAnotacion lOD_UserAnotacion = db.LOD_UserAnotacion.Where(x => x.IdAnotacion == lOD_Anotaciones.IdAnotacion && x.IdUsLod == idReceptor).FirstOrDefault();
            LOD_UsuariosLod lOD_UsuariosLod = db.LOD_UsuariosLod.Where(x => x.IdUsLod == idReceptor).FirstOrDefault();
            //BUSCAR RECEPTOR EN BBDD
            Receptor r = new Receptor()
            {
                Nombre = lOD_UsuariosLod.ApplicationUser.Nombres + " " + lOD_UsuariosLod.ApplicationUser.Apellidos,
                Iniciales = lOD_UsuariosLod.ApplicationUser.Nombres.Substring(0, 1) + lOD_UsuariosLod.ApplicationUser.Apellidos.Substring(0, 1),
                ReqVB = lOD_UserAnotacion.RespVB,
                RespRespuesta = lOD_UserAnotacion.EsRespRespuesta,
                EsPrincipal = lOD_UserAnotacion.EsPrincipal,
                Cargo = lOD_UsuariosLod.ApplicationUser.CargoContacto,
                IdAnotacion = lOD_Anotaciones.IdAnotacion,
                IdReceptor = lOD_UsuariosLod.IdUsLod
            };
            return PartialView("_sidebarDeleteReceptor", r);
        }

        [HttpPost, ActionName("DeleteReceptor")]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> DeleteReceptorConfirm(Receptor receptor)
        {
            Form_Validation val = new Form_Validation() { Status = true, ErrorMessage = new List<string>() };
            try
            {
                //VALIDAR QUE SE PUEDA BORRAR AL RECEPTOR (SI NO ESTA FIRMADA, ETC)
                LOD_Anotaciones anot = db.LOD_Anotaciones.Find(receptor.IdAnotacion);
                if (!anot.EstadoFirma)
                {
                    LOD_UsuariosLod lOD_UsuariosLod = db.LOD_UsuariosLod.Where(x => x.IdUsLod == receptor.IdReceptor).FirstOrDefault();
                    LOD_UserAnotacion lOD_UserAnotacion = db.LOD_UserAnotacion.Where(x => x.IdAnotacion == receptor.IdAnotacion && x.IdUsLod == lOD_UsuariosLod.IdUsLod).FirstOrDefault();
                    db.LOD_UserAnotacion.Remove(lOD_UserAnotacion);
                    await db.SaveChangesAsync();
                    //*****************************************
                    
                    string accion = "Se ha eliminado el receptor:" + lOD_UsuariosLod.ApplicationUser.Nombres + " " + lOD_UsuariosLod.ApplicationUser.Apellidos + " en la anotación: '" + lOD_UserAnotacion.IdAnotacion + "'";
                    bool response = await Log_Helper.SetLOGAnotacionAsync(anot, accion, User.Identity.GetUserId());
                }
                else
                {
                    val.Status = false;
                    val.ErrorMessage.Add("El receptor se encuentra inhabilitado para su eliminación.");
                }
            }
            catch (Exception ex)
            {
                val.Status = false;
                val.ErrorMessage.Add(ex.Message);
            }

            return Json(val, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> EditReceptor(int id, int idReceptor)
        {
            LOD_Anotaciones lOD_Anotaciones = db.LOD_Anotaciones.Find(id);
            LOD_UserAnotacion lOD_UserAnotacion = db.LOD_UserAnotacion.Where(x => x.IdAnotacion == lOD_Anotaciones.IdAnotacion && x.IdUsLod == idReceptor).FirstOrDefault();
            LOD_UsuariosLod lOD_UsuariosLod = db.LOD_UsuariosLod.Where(x => x.IdUsLod == idReceptor).FirstOrDefault();
            //BUSCAR RECEPTOR EN BBDD
            Receptor r = new Receptor()
            {
                Nombre = lOD_UsuariosLod.ApplicationUser.Nombres + " " + lOD_UsuariosLod.ApplicationUser.Apellidos,
                Iniciales = lOD_UsuariosLod.ApplicationUser.Nombres.Substring(0, 1) + lOD_UsuariosLod.ApplicationUser.Apellidos.Substring(0, 1),
                ReqVB = lOD_UserAnotacion.RespVB,
                RespRespuesta = lOD_UserAnotacion.EsRespRespuesta,
                EsPrincipal = lOD_UserAnotacion.EsPrincipal,
                Cargo = lOD_UsuariosLod.ApplicationUser.CargoContacto,
                IdAnotacion = lOD_Anotaciones.IdAnotacion,
                IdReceptor = lOD_UsuariosLod.IdUsLod
            };
            ViewBag.IdReceptor = idReceptor;
            ViewBag.IdAnotacion = id;
            ViewBag.PermiteResponsable = true;
            ViewBag.PermiteReceptor = true;
            if (db.LOD_UserAnotacion.Where(x => x.IdAnotacion == lOD_Anotaciones.IdAnotacion && x.EsPrincipal).FirstOrDefault() != null)
                ViewBag.PermiteResponsable = false;
            if (db.LOD_UserAnotacion.Where(x => x.IdAnotacion == lOD_Anotaciones.IdAnotacion && x.EsRespRespuesta).FirstOrDefault() != null)
                ViewBag.PermiteResponsable = false;

            return PartialView("_sidebarEditReceptor", r);
        }

        [HttpPost, ActionName("EditReceptor")]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> EditReceptorConfirm(Receptor receptor)
        {
            Form_Validation val = new Form_Validation() { Status = true, ErrorMessage = new List<string>() };
            try
            {
                //VALIDAR QUE SE PUEDA EDITAR AL RECEPTOR (SI NO ESTA FIRMADA, ETC)
                LOD_Anotaciones anot = db.LOD_Anotaciones.Find(receptor.IdAnotacion);
                if (!anot.EstadoFirma)
                {
                    LOD_UsuariosLod lOD_UsuariosLod = db.LOD_UsuariosLod.Where(x => x.IdUsLod == receptor.IdReceptor).FirstOrDefault();
                    LOD_UserAnotacion lOD_UserAnotacion = db.LOD_UserAnotacion.Where(x => x.IdAnotacion == receptor.IdAnotacion && x.IdUsLod == lOD_UsuariosLod.IdUsLod).FirstOrDefault();
                    lOD_UserAnotacion.EsPrincipal = receptor.EsPrincipal;
                    lOD_UserAnotacion.EsRespRespuesta = receptor.RespRespuesta;

                    db.Entry(lOD_UserAnotacion).State = EntityState.Modified;
                    await db.SaveChangesAsync();

                    string accion = "Se ha editado el receptor:" + lOD_UsuariosLod.ApplicationUser.Nombres + " " + lOD_UsuariosLod.ApplicationUser.Apellidos + " en la anotación: '" + lOD_UserAnotacion.IdAnotacion + "'";
                    bool response = await Log_Helper.SetLOGAnotacionAsync(anot, accion, User.Identity.GetUserId());
                }
                else
                {
                    val.Status = false;
                    val.ErrorMessage.Add("El receptor se encuentra inhabilitado para su modificación.");
                }
            }
            catch (Exception ex)
            {
                val.Status = false;
                val.ErrorMessage.Add(ex.Message);
            }

            return Json(val, JsonRequestBehavior.AllowGet);
        }
        //********************************************************************

        //*************************FILTROS************************************
        public async Task<ActionResult> GetFiltroRapido(int id, int idLibro)
        {
            //LOGICAS DEL FILTRO RAPIDO SEGUN LA ID
            return PartialView("_GetTable",await GetAnotaciones(id, idLibro));
        }

        [HttpPost]
        public async Task<ActionResult> GetFiltroAvanzado(FiltroAvanzado siltro)
        {
            var lod_anot = db.LOD_Anotaciones.AsQueryable();

            if (!String.IsNullOrEmpty(siltro.IdDestinatario))
                lod_anot = db.LOD_UserAnotacion.Where(x => x.LOD_UsuarioLod.UserId == siltro.IdDestinatario && x.LOD_UsuarioLod.IdLod == siltro.IdLibro && x.RespVB && x.LOD_Anotaciones.EstadoFirma).Select(x => x.LOD_Anotaciones);

            if (!String.IsNullOrEmpty(siltro.IdRemitente)) //Agregar estado 2
                lod_anot = lod_anot.Where(x => x.UserId == siltro.IdRemitente && x.IdLod== siltro.IdLibro);

            if (!String.IsNullOrEmpty(siltro.FechaPub) && lod_anot.Count() != 0)
            {
                string[] fechas = siltro.FechaPub.Split('~');
                DateTime first = Convert.ToDateTime(fechas[0]);
                DateTime second = Convert.ToDateTime(fechas[1]).AddHours(23).AddMinutes(59).AddSeconds(59);
                lod_anot = lod_anot.Where(x => (x.FechaPub >= first && x.FechaPub <= second) && x.IdLod == siltro.IdLibro);
            }

            if (!String.IsNullOrEmpty(siltro.searchCuerpo) && lod_anot.Count() != 0)
                lod_anot = lod_anot.Where(x => x.Cuerpo.Contains(siltro.searchCuerpo) && x.IdLod == siltro.IdLibro);

            return PartialView("_GetTable", GetAnotaciones(lod_anot.ToList()));
        }

        public async Task<JsonResult> GetTipos(int id)
        {
            LOD_LibroObras lOD_LibroObras = db.LOD_LibroObras.Find(id);
            List<SelectListItem> tipo = new List<SelectListItem>();
            List<MAE_TipoComunicacion> mAE_TipoComunicacion = new List<MAE_TipoComunicacion>();
            if (lOD_LibroObras.IdTipoLod == 1 || lOD_LibroObras.IdTipoLod == 2)
            {
                mAE_TipoComunicacion = db.MAE_TipoComunicacion.Where(x => !x.Nombre.Equals("Comunicación General")).ToList();
                MAE_TipoComunicacion auxGeneral = db.MAE_TipoComunicacion.Where(x => x.IdTipoLod == lOD_LibroObras.IdTipoLod && x.Nombre.Equals("Comunicación General")).FirstOrDefault();
                mAE_TipoComunicacion.Add(auxGeneral);
            }
            else
            {
                mAE_TipoComunicacion = db.MAE_TipoComunicacion.Where(x => x.Activo && x.IdTipoLod == lOD_LibroObras.IdTipoLod).ToList();
            }
            
            foreach (var item in mAE_TipoComunicacion.OrderBy(x => x.Nombre))
                tipo.Add(new SelectListItem() { Text = item.Nombre, Value = item.IdTipoCom.ToString() });

            return Json(tipo, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> GetSubTipos(int id)
        {
            List<SelectListItem> subtipo = new List<SelectListItem>();
            List<MAE_SubtipoComunicacion> mAE_SubtipoComunicacion = db.MAE_SubtipoComunicacion.Where(x => x.IdTipoCom == id).ToList();
            foreach (var item in mAE_SubtipoComunicacion.OrderBy(x => x.Nombre))
                subtipo.Add(new SelectListItem() { Text = item.Nombre, Value = item.IdTipoSub.ToString() });

            return Json(subtipo, JsonRequestBehavior.AllowGet);
        }

        public List<AnotaTablaView> GetAnotaciones(int idLibro)
        {

            List<AnotaTablaView> anotaciones = new List<AnotaTablaView>();
            List<LOD_Anotaciones> list_Anot = db.LOD_Anotaciones.Where(x => x.IdLod == idLibro && x.Estado == 2).ToList(); //ESTADO PUBLICADO
            string userName = User.Identity.GetUserId();

            foreach (var item in list_Anot)
            {
                string iniciales = item.UsuarioRemitente.Nombres.Substring(0, 1);
                iniciales = iniciales + item.UsuarioRemitente.Apellidos.Substring(0, 1);
                LOD_ReferenciasAnot refe = db.LOD_ReferenciasAnot.Where(x => x.IdAnotacion == item.IdAnotacion).FirstOrDefault();
                bool esRef = false;
                if (refe != null)
                    esRef = true;

                LOD_UserAnotacion lOD_UserAnotacion = db.LOD_UserAnotacion.Where(x => x.IdAnotacion == item.IdAnotacion && x.LOD_UsuarioLod.UserId == userName).FirstOrDefault();
                bool esleido = false;
                bool esDestacado = false;
                if (lOD_UserAnotacion != null)
                {
                    esleido = lOD_UserAnotacion.Leido;
                    esDestacado = lOD_UserAnotacion.Destacado;
                }

                AnotaTablaView a = new AnotaTablaView(item.EstadoFirma, item.TipoFirma)
                {
                    IdAnotacion = item.IdAnotacion,
                    Titulo = item.Titulo,
                    DatosRemitente = new Remitente()
                    {
                        Nombre = item.UsuarioRemitente.Nombres + " " + item.UsuarioRemitente.Apellidos,
                        ImgRemitente = item.UsuarioRemitente.RutaImagen,
                        InicialesRemitente = iniciales
                    },
                    Correlativo = item.Correlativo.ToString(),
                    IsLeida = esleido,
                    IsDestacada = esDestacado,
                    FechaPublicacion = item.FechaPub.ToString(),
                    IsReferencias = esRef
                };
                anotaciones.Add(a);
            }

            return anotaciones.OrderByDescending(o => o.correlativoNum).ThenBy(x => x.FechaPublicacion).ToList();
        }

        public async Task<List<AnotaTablaView>> GetAnotaciones(int id, int idLibro)
        {
            Random rnd = new Random();
            int count = rnd.Next(1, 210);
            List<AnotaTablaView> anotaciones = new List<AnotaTablaView>();
            List<LOD_Anotaciones> list_Anot = new List<LOD_Anotaciones>();

            string idUser = User.Identity.GetUserId();

            switch (id) //count en stats
            {
                case 1:
                    list_Anot = await db.LOD_Anotaciones.Where(x => x.IdLod == idLibro && x.Estado == 2).ToListAsync();
                    break;
                case 2:
                    list_Anot = await db.LOD_Anotaciones.Where(x => x.UserId.Equals(idUser) && x.Estado == 2 && x.IdLod == idLibro).ToListAsync();
                    break;
                case 3:
                    list_Anot = await db.LOD_Anotaciones.Where(x => x.Estado == 0 && x.UserIdBorrador.Equals(idUser) && x.IdLod == idLibro).ToListAsync();
                    break;
                case 4:
                    list_Anot = await db.LOD_UserAnotacion.Where(x => x.LOD_UsuarioLod.UserId.Equals(idUser) && x.Destacado && x.LOD_Anotaciones.IdLod == idLibro).Select(x => x.LOD_Anotaciones).ToListAsync();
                    break;
                case 5:
                    list_Anot = await db.LOD_UserAnotacion.Where(x => x.LOD_UsuarioLod.UserId.Equals(idUser) && x.LOD_Anotaciones.IdLod == idLibro && x.LOD_Anotaciones.Estado == 2).Select(x => x.LOD_Anotaciones).ToListAsync();
                    break;
                case 6:
                    list_Anot = await db.LOD_Anotaciones.Where(x => x.Estado == 1 && x.UserId.Equals(idUser) && x.IdLod == idLibro).ToListAsync();
                    break;
                case 7: //pendientes de respuestas
                    var referenciasAnot = await db.LOD_UserAnotacion.Where(x => x.LOD_Anotaciones.EstadoFirma && x.LOD_UsuarioLod.UserId.Equals(idUser) && x.LOD_Anotaciones.SolicitudRest && x.EsRespRespuesta && x.LOD_Anotaciones.IdLod == idLibro).Select(x => x.LOD_Anotaciones).ToListAsync();
                    //&& x.LOD_UsuarioLod.UserId.Equals(id)
                    List<int> pendFirma = db.LOD_ReferenciasAnot.Where(r => r.EsRepuesta && r.AnotacionReferencia.IdLod == idLibro).Select(s => s.IdAnontacionRef).ToList();
                    list_Anot = referenciasAnot.Where(w => !pendFirma.Contains(w.IdAnotacion)).ToList();
                    break;
                default:
                    break;
            }

            foreach (var item in list_Anot)
            {
                string iniciales = "";
                string nombres = "";
                string apellidos = "";
                string rutaImagen = "";

                if (item.UsuarioRemitente != null)
                {
                    iniciales = item.UsuarioRemitente.Nombres.Substring(0, 1) + item.UsuarioRemitente.Apellidos.Substring(0, 1);
                    nombres = item.UsuarioRemitente.Nombres;
                    apellidos = item.UsuarioRemitente.Apellidos;
                    rutaImagen = item.UsuarioRemitente.RutaImagen;
                }

                else if (item.UserIdBorrador != null)
                {
                    nombres = db.Users.Find(item.UserIdBorrador).Nombres;
                    apellidos = db.Users.Find(item.UserIdBorrador).Apellidos;
                    iniciales = db.Users.Find(item.UserIdBorrador).Nombres.Substring(0, 1) + db.Users.Find(item.UserIdBorrador).Apellidos.Substring(0, 1);
                    rutaImagen = db.Users.Find(item.UserIdBorrador).RutaImagen;
                }

                //LOD_ReferenciasAnot refe = db.LOD_ReferenciasAnot.Where(x => x.IdAnotacion == item.IdAnotacion).FirstOrDefault();
                //bool esRef = false;
                //if (refe != null)
                //    esRef = true;

                LOD_UserAnotacion lOD_UserAnotacion = db.LOD_UserAnotacion.Where(x => x.IdAnotacion == item.IdAnotacion && x.LOD_UsuarioLod.UserId == idUser).FirstOrDefault();
                bool esleido = false;
                bool esDestacado = false;
                if (lOD_UserAnotacion != null)
                {
                    esleido = lOD_UserAnotacion.Leido;
                    esDestacado = lOD_UserAnotacion.Destacado;
                }

                AnotaTablaView a = new AnotaTablaView(item.EstadoFirma, item.TipoFirma)
                {
                    IdAnotacion = item.IdAnotacion,
                    Titulo = item.Titulo,
                    DatosRemitente = new Remitente()
                    {
                        Nombre = nombres + " " + apellidos,
                        ImgRemitente = rutaImagen,
                        InicialesRemitente = iniciales
                    },
                    Correlativo = item.Correlativo.ToString(),
                    IsLeida = esleido,
                    IsDestacada = esDestacado,
                    FechaPublicacion = item.FechaPub.ToString(),
                    //IsReferencias = esRef
                };
                anotaciones.Add(a);
            }

            return anotaciones.OrderByDescending(o => o.IdAnotacion).ToList();
        }

        public List<AnotaTablaView> GetAnotaciones(List<LOD_Anotaciones> list_Anot)
        {
            List<AnotaTablaView> anotaciones = new List<AnotaTablaView>();

            foreach (var item in list_Anot)
            {
                string iniciales = item.UsuarioRemitente.Nombres.Substring(0, 1);
                iniciales = iniciales + item.UsuarioRemitente.Apellidos.Substring(0, 1);
                LOD_ReferenciasAnot refe = db.LOD_ReferenciasAnot.Where(x => x.IdAnotacion == item.IdAnotacion).FirstOrDefault();
                bool esRef = false;
                if (refe != null)
                    esRef = true;

                LOD_UserAnotacion lOD_UserAnotacion = db.LOD_UserAnotacion.Where(x => x.IdAnotacion == item.IdAnotacion).FirstOrDefault();
                bool esleido = false;
                bool esDestacado = false;
                if (lOD_UserAnotacion != null)
                {
                    esleido = lOD_UserAnotacion.Leido;
                    esDestacado = lOD_UserAnotacion.Destacado;
                }

                AnotaTablaView a = new AnotaTablaView(item.EstadoFirma, item.TipoFirma)
                {
                    IdAnotacion = item.IdAnotacion,
                    Titulo = item.Titulo,
                    DatosRemitente = new Remitente()
                    {
                        Nombre = item.UsuarioRemitente.Nombres + " " + item.UsuarioRemitente.Apellidos,
                        ImgRemitente = item.UsuarioRemitente.RutaImagen,
                        InicialesRemitente = iniciales
                    },
                    Correlativo = item.Correlativo.ToString(),
                    IsLeida = esleido,
                    IsDestacada = esDestacado,
                    FechaPublicacion = item.FechaPub.ToString(),
                    IsReferencias = esRef
                };
                anotaciones.Add(a);
            }

            return anotaciones.OrderByDescending(o => o.IdAnotacion).ToList();
        }
        //********************************************************************

        public async Task<JsonResult> GetLogJson(int id)
        {
            List<LOD_log> listado = await db.LOD_log.Where(x => x.IdObjeto == id && x.Objeto.Equals("Anotacion")).ToListAsync();
            List<LogView> list_logView = new List<LogView>();
            foreach (var item in listado)
            {
                LogView logView = new LogView()
                {
                    IdLog = item.IdLog,
                    IdObjeto = item.IdObjeto,
                    Usuario = db.Users.Find(item.UserId).NombreCompleto,
                    UserId = item.UserId,
                    Accion = item.Accion,
                    Campo = item.Campo,
                    FechaLog = item.FechaLog.ToString(),
                    Objeto = item.Objeto
                };
                list_logView.Add(logView);
            }

            return Json(list_logView.OrderByDescending(x => x.FechaLog), JsonRequestBehavior.AllowGet);
        }
        public async Task<JsonResult> GetUserStats(int idLibro)
        {
            string id = User.Identity.GetUserId();
            List<int> referenciasAnot = db.LOD_UserAnotacion.Where(x => x.LOD_Anotaciones.Estado == 2 && x.LOD_Anotaciones.SolicitudRest == true && x.LOD_UsuarioLod.UserId.Equals(id) && x.EsRespRespuesta && x.LOD_Anotaciones.IdLod == idLibro).Select(x => x.IdAnotacion).ToList();
            int pendFirma = await db.LOD_ReferenciasAnot.Where(r => r.EsRepuesta && referenciasAnot.Contains(r.IdAnontacionRef)).CountAsync();
            int totalPend = referenciasAnot.Count - pendFirma;
            LodUserStats stats = new LodUserStats() //CORREGIR
            {
                Borradores = db.LOD_Anotaciones.Where(x => x.Estado == 0 && x.UserIdBorrador.Equals(id) && x.IdLod == idLibro).ToList().Count(),
                Destacadas = db.LOD_UserAnotacion.Where(x => x.LOD_UsuarioLod.UserId.Equals(id) && x.Destacado && x.LOD_Anotaciones.IdLod == idLibro).Select(x => x.LOD_Anotaciones).ToList().Count(),
                MisPub = db.LOD_Anotaciones.Where(x => x.UserId.Equals(id) && x.Estado == 2 && x.IdLod == idLibro).ToList().Count(),
                Nombrado = db.LOD_UserAnotacion.Where(x => x.LOD_UsuarioLod.UserId.Equals(id) && x.LOD_Anotaciones.IdLod == idLibro && x.LOD_Anotaciones.Estado == 2).Select(x => x.LOD_Anotaciones).ToList().Count(),
                FirmasPendientes = db.LOD_Anotaciones.Where(x => x.Estado == 1 && x.UserId.Equals(id) && x.IdLod == idLibro).ToList().Count(),
                Principal = db.LOD_Anotaciones.Where(x => x.IdLod == idLibro && x.Estado == 2).Count(),
                RespuestasPendientes = totalPend<0?0:totalPend
            };
            return Json(stats, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public async Task<JsonResult> SetDestacarAnotacion(int id, bool estado)
        {
            Form_Validation val = new Form_Validation() { Status = true, ErrorMessage = new List<string>() };
            try
            {
                string userid = User.Identity.GetUserId();
                LOD_UserAnotacion lOD_UserAnotacion = db.LOD_UserAnotacion.Where(x => x.IdAnotacion == id && x.LOD_UsuarioLod.UserId == userid).FirstOrDefault();
                if (lOD_UserAnotacion != null)
                {
                    lOD_UserAnotacion.Destacado = estado;
                    db.Entry(lOD_UserAnotacion).State = EntityState.Modified;
                    await db.SaveChangesAsync();

                    string accion = estado?"Usuario ha destacado la anotación":"Usuario ya no destaca la anotación";
                    bool response = await Log_Helper.SetLOGAnotacionAsync(lOD_UserAnotacion.LOD_Anotaciones, accion, User.Identity.GetUserId());
                }
                else
                {
                    val.Status = false;
                    val.ErrorMessage.Add("La Anotación se encuentra inhabilitada para su modificación.");
                }
            }
            catch (Exception ex)
            {
                val.Status = false;
                val.ErrorMessage.Add(ex.Message);
            }

            return Json(val, JsonRequestBehavior.AllowGet);
        }
        public async Task<JsonResult> GetAnotacionData(int id)
        {
            //ApplicationUser userActual = await db.Users.Where(u => u.Id == ).FirstOrDefaultAsync();
            AnotacionHelper anothelp = new AnotacionHelper();
            AnotacionView anot = await anothelp.GetAnotacionData(id);

            //LOD_Anotaciones anotacion = await db.LOD_Anotaciones.FindAsync(id);
            string userId = User.Identity.GetUserId();
            LOD_UserAnotacion userAnotacionActual = db.LOD_UserAnotacion.Where(x => x.IdAnotacion == id && x.LOD_UsuarioLod.UserId == userId).FirstOrDefault();
            bool darVB = (anot.EstadoFirma.IsFirmada && !userAnotacionActual.VistoBueno && userAnotacionActual.RespVB);
            bool debeResponder = (anot.EstadoFirma.IsFirmada  && anot.SolicitudRest && !anot.EstadoRespuesta.Respondida && userAnotacionActual.EsRespRespuesta);
            bool destacado = userAnotacionActual.Destacado;

            var UsuarioActual = new UsuarioActual()
            {//USUARIO EN SESIÓN QUE TIENE PERMISOS PARA INGRESAR EL LOD EN CUESTION
                DebeDarVistoBueno = darVB,//ES TRUE CUANDO: USUARIO EN SESION LOD_USERANOTACION VISTOBUENBO=FALSE
                DebeResponder = debeResponder,//ES TRUE CUANDO: USER EN SESION MARCADO COMO RESP RESPUESTA + ANOTACIÓN MARCADA REQUIERE RESPUESTA + LA ANOTACION ESTE FIRMADA + SIN RESPUESTA (FECHA RESPUESTA == NULL && SIN REGISTRO EN LOD_REFERENCIA)
                EsDestacada = destacado
            };

         
            anot.UsuarioActual = UsuarioActual;

            return Json(anot, JsonRequestBehavior.AllowGet);

        }
        public async Task<JsonResult> GetReceptoresData(int id)
        {
            AnotacionHelper anothelp = new AnotacionHelper();
            return Json(await anothelp.GetReceptoresData(id), JsonRequestBehavior.AllowGet);
        }

        public async Task<bool> IsDocsPendientes(int id)
        {
            LOD_Anotaciones lOD_Anotaciones = db.LOD_Anotaciones.Find(id);
            List<MAE_TipoDocumento> docRequeridos = db.MAE_CodSubCom.Where(x => x.IdTipoSub == lOD_Anotaciones.IdTipoSub).Select(x => x.MAE_TipoDocumento).ToList();
            List<LOD_docAnotacion> docCargados = db.LOD_docAnotacion.Where(x => x.IdAnotacion == lOD_Anotaciones.IdAnotacion).ToList();
            docRequeridos = docRequeridos.Except(docCargados.Select(x => x.MAE_TipoDocumento)).ToList();

            return (docRequeridos.Count > 0) ? true : false;
        }

        public async Task<bool> SetDocsPendientes(int id)
        {
            LOD_Anotaciones lOD_Anotaciones = db.LOD_Anotaciones.Find(id);

            List<LOD_docAnotacion> docCargados = db.LOD_docAnotacion.Where(x => x.IdAnotacion == lOD_Anotaciones.IdAnotacion).ToList();
            bool response = true;
            foreach (var doc in docCargados)
            {
                try
                {
                    doc.EstadoDoc = 2;
                    doc.FechaEvento = DateTime.Now;
                    doc.IdUserEvento = User.Identity.GetUserId();
                    db.Entry(doc).State = EntityState.Modified;
                    await db.SaveChangesAsync();

                    string tipodoc = doc.MAE_TipoDocumento.Tipo;

                    string accion = "Se ha aprobado el documento:" + tipodoc;
                    bool res = await Log_Helper.SetObjectLog(0, lOD_Anotaciones, accion, GetUserInSession().Id);

                    if (doc.LOD_Anotaciones.IdTipoSub.Equals(30))
                    {
                        if (doc.IdTipoDoc.Equals(64))
                        {
                            List<FORM_InformesItems> itemsInformes = db.FORM_InformesItems.Where(x => x.IdAnotacion == doc.IdAnotacion).ToList();
                            if (itemsInformes != null)
                            {
                                foreach (var item in itemsInformes)
                                {
                                    string nombreLimpio = doc.MAE_documentos.NombreDoc.Replace("_", " ");
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
                catch(Exception ex)
                {
                    response = false;
                }
            }



            return response;
        }


        public async Task<bool> SetNoAprobadoDocsPendientes(int id)
        {
            LOD_Anotaciones lOD_Anotaciones = db.LOD_Anotaciones.Find(id);

            List<LOD_docAnotacion> docCargados = db.LOD_docAnotacion.Where(x => x.IdAnotacion == lOD_Anotaciones.IdAnotacion).ToList();
            bool response = true;
            foreach (var item in docCargados)
            {
                try
                {
                    item.EstadoDoc = 1;
                    db.Entry(item).State = EntityState.Modified;
                    await db.SaveChangesAsync();

                }
                catch (Exception ex)
                {
                    response = false;
                }
            }
            return response;
        }

        public async Task<bool> IsDestPendientes(int id)
        {
            int totaluser = db.LOD_UserAnotacion.Where(u => u.IdAnotacion == id && u.RespVB).ToList().Count;
            return (totaluser == 0) ? true : false;
        }

        public async Task<bool> IsDestPrincipal(int id)
        {
            int totaluser = db.LOD_UserAnotacion.Where(u => u.IdAnotacion == id && u.EsPrincipal).ToList().Count;
            return (totaluser != 0) ? true : false;
        }

        public async Task<bool> IsRespRespuestaPendiente(int id, bool reqResp)
        {
            if (reqResp)
            {
                int totaluser = db.LOD_UserAnotacion.Where(u => u.IdAnotacion == id && u.EsRespRespuesta).ToList().Count;
                return (totaluser == 0) ? true : false;
            }
            else
            {
                return false;
            }
        }
        public async Task<bool> IsPrincipalPendiente(int id)
        {
            int totaluser = db.LOD_UserAnotacion.Where(u => u.IdAnotacion == id && u.EsPrincipal).ToList().Count;
            return (totaluser == 0) ? true : false;
        }
        public async Task<bool> FirmarAnotacionDB(int id, int tipo)
        {
            
            try
            {
                LOD_Anotaciones anot = await db.LOD_Anotaciones.FindAsync(id);
                anot.Correlativo = db.LOD_Anotaciones.Where(a => a.Estado == 2 && a.IdLod == anot.IdLod).Count() + 1;
                anot.FechaFirma = DateTime.Now;
                anot.Estado = 2;
                anot.EstadoFirma = true;
                anot.FechaPub = DateTime.Now;
                anot.TipoFirma = tipo;
                anot.UserId = User.Identity.GetUserId();
                db.Entry(anot).State = EntityState.Modified;
                await db.SaveChangesAsync();

                GLOD_Notificaciones notificaciones = new GLOD_Notificaciones();
                string userid = GetUserInSession().Id;
                LOD_ReferenciasAnot referencia = db.LOD_ReferenciasAnot.Where(x => x.IdAnotacion == anot.IdAnotacion).FirstOrDefault();
                int resu = 0;
                if (referencia == null)  //NOTIFICAR PUBLICACION O NOTIFICAR RESPUESTA
                    resu = await notificaciones.NotificarPublicacion(anot, userid);
                else
                    resu = await notificaciones.NotificarRespuesta(anot, userid);
                

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public async Task<bool> QuitarFirmaAnotacionDB(int id)
        {
            try
            {
                LOD_Anotaciones anot = await db.LOD_Anotaciones.FindAsync(id);
                anot.Correlativo = 0;
                anot.FechaFirma = null;
                anot.Estado = 0;
                anot.EstadoFirma = false;
                anot.FechaPub = null;
                anot.TipoFirma = 0;
                anot.UserId = null;
                db.Entry(anot).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }
        public async Task<ActionResult> DescargarAnotacion(int id)
        {
            FirmarAnotacion firm = new FirmarAnotacion();
            string tempPath = await firm.PathDescargarAnotacion(id);

            var memory = new MemoryStream();
            using (var stream = new FileStream(tempPath, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;
            return File(memory, "application/pdf", Path.GetFileName(tempPath));
        }
        public async Task<string> PathDescargarAnotacion(int id)
        {
            LOD_Anotaciones anotacion = await db.LOD_Anotaciones.FindAsync(id);
            //string mandante = anotacion.LOD_LibroObras.CON_Contratos.IdEmpresaFiscalizadora.ToString();
            //string contrato = anotacion.LOD_LibroObras.CON_Contratos.CodigoContrato;
            //string espacio = anotacion.LOD_LibroObras.MAE_TipoLOD.;
            //string libro = anotacion.LOD_LibroObras.MAE_TipoLOD.Nombre;
            //string folio = anotacion.Correlativo.ToString().PadLeft(6, '0');
            string tempPath = Path.Combine(Server.MapPath("~/"), anotacion.RutaPdfConFirma);

            return tempPath;
        }

        public async Task<string> GeneratePDF(int id)
        {
            List<ReportParameter> parametros = new List<ReportParameter>();

            LOD_Anotaciones anotacion = await db.LOD_Anotaciones.FindAsync(id);
            AnotacionHelper anothelp = new AnotacionHelper();
            AnotacionView anot = await anothelp.GetAnotacionData(id);
            anot.Remitente.ImgRemitente = (anot.Remitente.ImgRemitente != "") ? Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath.TrimEnd('/') + anot.Remitente.ImgRemitente : "";
            anot.QR = QRBinary(id, anotacion.IdLod, anotacion.LOD_LibroObras.CON_Contratos.IdEmpresaContratista.Value);

            List<AnotacionView> lstAnot = new List<AnotacionView>();
            lstAnot.Add(anot);

            List<Remitente> lstRem = new List<Remitente>();
            lstRem.Add(anot.Remitente);

            List<EstadoFirma> lstFirm = new List<EstadoFirma>();
            lstFirm.Add(anot.EstadoFirma);

            List<EstadoAnotacion> lstEstado = new List<EstadoAnotacion>();
            lstEstado.Add(anot.EstadoAnotacion);

            List<ReportDataSource> dsources = new List<ReportDataSource>();
            dsources.Add(new ReportDataSource("DsAnotacion", lstAnot));
            dsources.Add(new ReportDataSource("DSRemitente", lstRem));
            dsources.Add(new ReportDataSource("DSReceptor", anot.Receptores));
            dsources.Add(new ReportDataSource("DSEstadoFirma", lstFirm));
            dsources.Add(new ReportDataSource("DSEstadoAnotacion", lstEstado));
            dsources.Add(new ReportDataSource("DSEstadoRespuesta", new List<EstadoRespuesta>()));
            dsources.Add(new ReportDataSource("DSReferencias", anot.Referencias));
            dsources.Add(new ReportDataSource("DSAdjuntos", anot.Adjuntos));

            try
            {

                FileContentResult informePDF = (FileContentResult)GenerarReporte("PDF", "rptAnotacionLod.rdlc", dsources, parametros, anotacion.LOD_LibroObras.CON_Contratos.IdEmpresaContratista);
                MemoryStream ms = new MemoryStream(informePDF.FileContents);
                string auxPath = sanitizePath(anotacion.RutaCarpetaPdf);
                string tempPath = Path.Combine(Server.MapPath("~/"), auxPath);

                if (!Directory.Exists(tempPath))
                    Directory.CreateDirectory(tempPath);

                string auxPdfSinFirma = sanitizePath(anotacion.RutaPdfSinFirma);
                string informeTempPath = Path.Combine(Server.MapPath("~/"), auxPdfSinFirma);

                FileStream newFile = new FileStream(informeTempPath, FileMode.Create, FileAccess.Write);
                ms.WriteTo(newFile);
                newFile.Close();

                return informeTempPath;
            }
            catch
            {
                return string.Empty;
            }


        }


        private static string sanitizePath(string path)
        {
            string message = path;

            try
            {

                message = message.Replace("á", "a");
                message = message.Replace("é", "e");
                message = message.Replace("í", "i");
                message = message.Replace("ó", "o");
                message = message.Replace("ú", "u");

                message = message.Replace("Á", "A");
                message = message.Replace("É", "E");
                message = message.Replace("Í", "I");
                message = message.Replace("Ó", "O");
                message = message.Replace("Ú", "U");

                message = message.Replace("ñ", "n");
                message = message.Replace("Ñ", "N");

            }
            catch (Exception e)
            {
                return path;
            }

            return message;
        }

        public FileContentResult GenerarReporte(string tipo, string nombreArchivoRpt, List<ReportDataSource> dataSources, List<ReportParameter> parametros, int? IdEmpresa)
        {

            //************DATOS EMPRESA*****
            //
            var empresa = db.MAE_sujetoEconomico.Find(IdEmpresa);

            List<DsEmpresa> lstEmpresa = new List<DsEmpresa>();
            DsEmpresa emp = new DsEmpresa();
            emp.Razon = empresa.RazonSocial;
            emp.Direccion = empresa.Direccion;
            emp.Web = empresa.web;
            emp.Telefono = empresa.Telefono;
            if (empresa.RutaImagen != null)
            {
                emp.Logo = Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath.TrimEnd('/') + "/Images/Sujetos/" + empresa.RutaImagen;
            }
            emp.Email = empresa.email;
            lstEmpresa.Add(emp);

            //LISTADO DE VISTAS DEL INFORME
            //PDF
            //Excel
            //Word
            //Image

            LocalReport lr = new LocalReport();
            lr.EnableExternalImages = true;
            lr.EnableHyperlinks = true;
            string path = Path.Combine(Server.MapPath("~/Areas/GLOD/Reports"), nombreArchivoRpt);

            if (System.IO.File.Exists(path))
            {
                lr.ReportPath = path;
            }
            else
            {
                return null;
            }

            //SE AGREGA EL DATASOURCE DE LOS DATOS DE LA EMPRESA
            lr.DataSources.Add(new ReportDataSource("DSEmpresa", lstEmpresa));
            //SE AGREGAN LOS DATASOURCES ESPECIFICOS DEL REPORTE
            foreach (ReportDataSource ds in dataSources)
                lr.DataSources.Add(ds);

            //SE AGREGA EL LISTADO DE PARAMETROS ESPECIFICOS POR CADA REPORTE
            if (parametros != null)
                lr.SetParameters(parametros);
            lr.Refresh();

            string reportType = tipo;
            string mimeType;
            string encoding;
            string fileNameExtension;
            string deviceInfo = "<DeviceInfo>" +
            "  <OutputFormat>" + tipo + "</OutputFormat>" +
            "  <PageWidth>8.5in</PageWidth>" +
            "  <PageHeight>11in</PageHeight>" +
            "  <MarginTop>0.3937in</MarginTop>" +
            "  <MarginLeft>0.7874in</MarginLeft>" +
            "  <MarginRight>0.7874in</MarginRight>" +
            "  <MarginBottom>0.3937in</MarginBottom>" +
            "</DeviceInfo>";

            Warning[] warnings;
            string[] streams;
            byte[] renderedBytes;
            try
            {
                renderedBytes = lr.Render(
                               reportType,
                               deviceInfo,
                               out mimeType,
                               out encoding,
                               out fileNameExtension,
                               out streams,
                               out warnings);
                return File(renderedBytes, mimeType);
            }
            catch (Exception Ex)
            {
                Console.WriteLine(Ex);
            }
            renderedBytes = lr.Render(
                               reportType,
                               deviceInfo,
                               out mimeType,
                               out encoding,
                               out fileNameExtension,
                               out streams,
                               out warnings);
            return File(renderedBytes, mimeType);
        }
        public byte[] QRBinary(int Folio, int libro, int cont)
        {
            string ruta = Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath.TrimEnd('/');

            QRCodeEncoder qrCodeEncoder = new QRCodeEncoder();
            String encoding = "Byte";
            if (encoding == "Byte")
            {
                qrCodeEncoder.QRCodeEncodeMode = QRCodeEncoder.ENCODE_MODE.BYTE;
            }
            else if (encoding == "AlphaNumeric")
            {
                qrCodeEncoder.QRCodeEncodeMode = QRCodeEncoder.ENCODE_MODE.ALPHA_NUMERIC;
            }
            else if (encoding == "Numeric")
            {
                qrCodeEncoder.QRCodeEncodeMode = QRCodeEncoder.ENCODE_MODE.NUMERIC;
            }
            try
            {
                int scale = Convert.ToInt16(4);
                qrCodeEncoder.QRCodeScale = scale;
            }
            catch (Exception ex)
            {
                //MessageBox.Show("Invalid size!");
                //return;
            }
            try
            {
                int version = Convert.ToInt16(7);
                qrCodeEncoder.QRCodeVersion = version;
            }
            catch (Exception ex)
            {
                //MessageBox.Show("Invalid version !");
            }

            string errorCorrect = "Q";
            if (errorCorrect == "L")
                qrCodeEncoder.QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.L;
            else if (errorCorrect == "M")
                qrCodeEncoder.QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.M;
            else if (errorCorrect == "Q")
                qrCodeEncoder.QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.Q;
            else if (errorCorrect == "H")
                qrCodeEncoder.QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.H;

            Image image;
            System.Drawing.Imaging.ImageFormat format = System.Drawing.Imaging.ImageFormat.Jpeg;
            String data = String.Format("{0}/Public/Folio?folio={1}&ldo={2}&cont?{3}", ruta, Folio, libro, cont);
            image = qrCodeEncoder.Encode(data);

            using (MemoryStream ms = new MemoryStream())
            {
                // Convert Image to byte[]
                image.Save(ms, format);
                byte[] imageBytes = ms.ToArray();

                return imageBytes;
            }


        }
        public async Task<string> AnotacionPDFToBase64(string path)
        {
            try
            {
                byte[] bytes;
                var memory = new MemoryStream();
                using (var stream = new FileStream(path, FileMode.Open))
                {
                    await stream.CopyToAsync(memory);
                    bytes = memory.ToArray();
                }

                string base64 = Convert.ToBase64String(bytes);
                return base64;
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }

        public ApplicationUser GetUserInSession()
        {
            string id = User.Identity.GetUserId();
            return db.Users.Where(u => u.Id == id).Include(i => i.MAE_Sucursal).FirstOrDefault();
        }

    }
}