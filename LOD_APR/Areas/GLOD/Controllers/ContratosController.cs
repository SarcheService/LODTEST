
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using ACAMicroFramework.Archivos;
using LOD_APR.Areas.Admin.Models;
using LOD_APR.Areas.GLOD.Helpers;
using LOD_APR.Areas.GLOD.Models;
using LOD_APR.Areas.GLOD.ModelsViews;
using LOD_APR.Helpers;
using LOD_APR.Models;
using Microsoft.AspNet.Identity;

namespace LOD_APR.Areas.GLOD.Controllers
{
    [CustomAuthorize]
    public class ContratosController : Controller
    {
        private LOD_DB db = new LOD_DB();
        private GoogleReCaptchaHelper reCaptchaHelper = new GoogleReCaptchaHelper();
        public ActionResult InicioRapido()
        {
            //Buscamos los contratos Disponibles
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
                name = "<Seleccione Contrato>"
            });

            foreach (var item in ContratosDisponibles)
            {
                search.Add(new AutoSearch()
                {
                    id = item.IdContrato.ToString(),
                    name = item.CodigoContrato.ToString() + "; " + item.NombreContrato.ToUpper()
                });
            }
            ViewBag.ContratosDisponible = new SelectList(search, "id", "name", IdContratoInicial);


            string IdUsuario = User.Identity.GetUserId();

            List<LOD_LibroObras> libros = new List<LOD_LibroObras>();

            //Se bloquean mientras agregó los Usuarios del libro
            //List<int> LstLib = db.LOD_UsuariosLod.Where(l => l.UserId == IdUsuario && l.Activo == true).Select(l => l.IdLod).ToList();
            //libros = db.LOD_LibroObras.Where(x => x.Estado == 1 && LstLib.Contains(x.IdLod)).OrderBy(x => x.NombreLibroObra).ToList();
            if (ValidaPermisos.ValidaPermisosEnController("0020060000"))
            {
                return View(libros);
            }
            else
            {
                return RedirectToAction("SinPermiso", "../Pub");
            }

        }

        public async Task<ActionResult> InicioRapidoBuscar(int IdContrato)
        {
            var IdContratoInicial = IdContrato;/* ContratosDisponibles.First().IdContrato;*/
            string IdUsuario = User.Identity.GetUserId();

            List<LOD_LibroObras> libros = new List<LOD_LibroObras>();

            //Libros Estado Abiertos 1
            List<int> IdLibros = db.LOD_UsuariosLod.Where(x => x.UserId == IdUsuario).Select(x => x.IdLod).ToList();

            libros = db.LOD_LibroObras.Where(x => x.Estado == 1 && x.IdContrato == IdContratoInicial && IdLibros.Contains(x.IdLod)).OrderBy(x => x.NombreLibroObra).ToList();
            foreach (var item in libros)
            {
                var referenciasAnot = await db.LOD_UserAnotacion.Where(x => x.LOD_Anotaciones.EstadoFirma && x.LOD_UsuarioLod.UserId.Equals(IdUsuario) && x.LOD_Anotaciones.SolicitudRest && x.EsRespRespuesta && x.LOD_Anotaciones.IdLod == item.IdLod).Select(x => x.LOD_Anotaciones).ToListAsync();
                List<int> pendFirma = db.LOD_ReferenciasAnot.Where(r => r.EsRepuesta && r.AnotacionReferencia.IdLod == item.IdLod).Select(s => s.IdAnontacionRef).ToList();
                item.MisRespuestasPendientes = referenciasAnot.Where(w => !pendFirma.Contains(w.IdAnotacion)).ToList().Count;
                item.MisVBPendientes = db.LOD_UserAnotacion.Where(a => a.LOD_Anotaciones.EstadoFirma && a.LOD_Anotaciones.IdLod == item.IdLod && a.LOD_UsuarioLod.UserId == IdUsuario && a.RespVB && a.VistoBueno == false).ToList().Count;
            }
            
            ViewBag.TipoVista = "IndexUsr";
            ViewBag.ResumenContrato = true;

            return PartialView("_InicioRapidoBuscar", libros);
        }

        public async Task<ActionResult> GetInfoDoc(int IdContrato)
        {
            string userid = User.Identity.GetUserId();
            List<LOD_LibroObras> libros = new List<LOD_LibroObras>();
            int TotalDoc = 0;
            //int DocRechazados = 0;
            int DocPendientes = 0;
            //int auxTotalDoc = 0;


            List<int> IdLibros = db.LOD_UsuariosLod.Where(x => x.UserId == userid).Select(x => x.IdLod).ToList();
            libros = db.LOD_LibroObras.Where(x => x.Estado == 1 && x.IdContrato == IdContrato && IdLibros.Contains(x.IdLod)).OrderBy(x => x.NombreLibroObra).ToList();
            List<LOD_Anotaciones> misVBPendientes = new List<LOD_Anotaciones>();
            List<LOD_Anotaciones> misRespPendientes = new List<LOD_Anotaciones>();

            foreach (var item in libros)
            {
                item.LstAnotaciones = db.LOD_UserAnotacion.Where(x => x.LOD_Anotaciones.IdLod == item.IdLod && (x.LOD_UsuarioLod.UserId == userid) && x.EsPrincipal).Select(x => x.LOD_Anotaciones).ToList();
                foreach (var anot in item.LstAnotaciones)
                {
                    List<MAE_TipoDocumento> listNoAprobados = new List<MAE_TipoDocumento>();
                    listNoAprobados = db.LOD_docAnotacion.Where(x => x.IdAnotacion == anot.IdAnotacion && x.EstadoDoc == 1).Select(x => x.MAE_TipoDocumento).ToList();
                    anot.DocCargados = db.LOD_docAnotacion.Where(x => x.IdAnotacion == anot.IdAnotacion).Count();
                    //List<MAE_TipoDocumento> listRechazados = new List<MAE_TipoDocumento>();
                    //List<MAE_TipoDocumento> docRequeridos = new List<MAE_TipoDocumento>();
                    // docRequeridos = db.MAE_CodSubCom.Where(x => x.IdTipoSub == anot.IdTipoSub).Select(x => x.MAE_TipoDocumento).ToList();
                    //listPendientes = docRequeridos.Except(listCargados).ToList();
                    //listRechazados = db.LOD_docAnotacion.Where(x => x.IdAnotacion == anot.IdAnotacion && x.EstadoDoc == 3).Select(x => x.MAE_TipoDocumento).ToList();
                    //anot.DocRechazados = listRechazados.Count();

                    DocPendientes = DocPendientes + listNoAprobados.Count();

                    TotalDoc = TotalDoc + anot.DocCargados;
                    //DocRechazados = DocRechazados + anot.DocRechazados;
                }

                var referenciasAnot = await db.LOD_UserAnotacion.Where(x => x.LOD_Anotaciones.EstadoFirma && x.LOD_UsuarioLod.UserId.Equals(userid) && x.LOD_Anotaciones.SolicitudRest && x.EsRespRespuesta && x.LOD_Anotaciones.IdLod == item.IdLod).Select(x => x.LOD_Anotaciones).ToListAsync();
                List<int> pendFirma = db.LOD_ReferenciasAnot.Where(r => r.EsRepuesta && r.AnotacionReferencia.IdLod == item.IdLod).Select(s => s.IdAnontacionRef).ToList();
                misRespPendientes.AddRange(referenciasAnot.Where(w => !pendFirma.Contains(w.IdAnotacion)).ToList());
                misVBPendientes.AddRange(db.LOD_UserAnotacion.Where(a => a.LOD_Anotaciones.EstadoFirma && a.LOD_Anotaciones.IdLod == item.IdLod && a.LOD_UsuarioLod.UserId == userid && a.RespVB && a.VistoBueno == false).Select(x => x.LOD_Anotaciones).ToList());

            }

            ViewBag.TotalDoc = TotalDoc;
            ViewBag.DocPendientes = DocPendientes;
            //ViewBag.DocRechazados = DocRechazados;

            if (DocPendientes != 0 /*&& DocRechazados != 0*/)
            {
                ViewBag.PorDocPendientes = Math.Round(Convert.ToDouble((TotalDoc * 100) / DocPendientes), 1);
                //ViewBag.PorDocRechazados = Math.Round(Convert.ToDouble((TotalDoc * 100) / DocRechazados), 1);
            }      
            else
            {
                ViewBag.PorDocPendientes = 0;
                //ViewBag.PorDocRechazados = 0;
            }

            
            ViewBag.IdContrato = IdContrato;

            
            ViewData["MisRespuestasPendientes"] = misRespPendientes;
            
            ViewData["MisVBPendientes"] = misVBPendientes;
            if(misRespPendientes.Count > 0 || misVBPendientes.Count > 0)
            {
                ViewBag.IsEmpty = false;
            }
            else
            {
                ViewBag.IsEmpty = true;
            }

            return PartialView("_GetInfoDoc", libros);
        }

        public ActionResult GetInfoAnot(int IdContrato)
        {
            string userid = User.Identity.GetUserId();
            List<LOD_LibroObras> libros = new List<LOD_LibroObras>();
            int TotalAnot = 0;
            int AnotFirmadas = 0;

            List<int> IdLibros = db.LOD_UsuariosLod.Where(x => x.UserId == userid).Select(x => x.IdLod).ToList();
            libros = db.LOD_LibroObras.Where(x => x.Estado == 1 && x.IdContrato == IdContrato && IdLibros.Contains(x.IdLod)).OrderBy(x => x.NombreLibroObra).ToList();
            foreach (var item in libros)
            {
                item.LstAnotaciones = db.LOD_UserAnotacion.Where(x => x.LOD_Anotaciones.IdLod == item.IdLod && (x.LOD_UsuarioLod.UserId == userid)).Select(x => x.LOD_Anotaciones).ToList();
                TotalAnot = TotalAnot + item.LstAnotaciones.Count();
                AnotFirmadas = AnotFirmadas + item.LstAnotaciones.Where(x => x.EstadoFirma).Count();
            }

            ViewBag.TotalAnot = TotalAnot;
            ViewBag.AnotFirmadas = AnotFirmadas;
            if (TotalAnot != 0)
            {
                double auxD = Math.Round(Convert.ToDouble((AnotFirmadas * 100) / TotalAnot), 1);
                ViewBag.ValueAnot = auxD;
                ViewBag.PercentAnot = auxD.ToString() + "%";
            }
            else { ViewBag.ValueAnot = 0; ViewBag.PercentAnot = "0%"; }

            ViewBag.ResumenContrato = true;
            ViewBag.IdContrato = IdContrato;
            return PartialView("_GetInfoAnot", libros);
        }

        //public ActionResult GetNotiAnot(int IdContrato)
        //{
        //    string userid = User.Identity.GetUserId();

        //    var referenciasAnot = db.LOD_UserAnotacion.Where(x => x.LOD_Anotaciones.EstadoFirma && x.LOD_UsuarioLod.UserId.Equals(userid) && x.LOD_Anotaciones.SolicitudRest && x.EsRespRespuesta).Select(x => x.LOD_Anotaciones).ToList();
        //    var refAux = db.LOD_UserAnotacion.Where(x => x.LOD_Anotaciones.EstadoFirma && x.LOD_UsuarioLod.UserId.Equals(userid)).Select(x => x.IdAnotacion).ToList();
        //    List<LOD_Anotaciones> listadoDoc = db.LOD_docAnotacion.Where(x => x.EstadoDoc == 1 && refAux.Contains(x.IdAnotacion)).Select(x => x.LOD_Anotaciones).ToList();
        //    referenciasAnot.AddRange(listadoDoc);
        //    var refPendFirma = db.LOD_Anotaciones.Where(x => x.Estado == 1 && x.UserId == userid).ToList();
        //    referenciasAnot.AddRange(refPendFirma);
        //    List<int> pendFirma = db.LOD_ReferenciasAnot.Where(r => r.EsRepuesta && r.AnotacionReferencia.UserId.Equals(userid)).Select(s => s.IdAnontacionRef).ToList();
        //    ViewBag.AnotPendRespuesta = referenciasAnot.Where(x => !pendFirma.Contains(x.IdAnotacion)).Distinct().ToList().OrderByDescending(x => x.FechaPub);
        //    ViewBag.IdContrato = IdContrato;
        //    return PartialView("_GetNotiAnot");
        //}


        // GET: ASP/Contratos/Details/5
        public async Task<ActionResult> Details(int id)
        {
            CON_Contratos contrato = db.CON_Contratos.Find(id);

            //ValidaPermisos ValidaPermisos = new ValidaPermisos();
            //ViewBag.ItoMandante = ValidaPermisos.EnSesionItoMandante(id);        
            contrato.Creador = db.Users.Find(contrato.UserId).Nombres + " " + db.Users.Find(contrato.UserId).Apellidos;
            //Mandamos Roles del Contrato
            contrato.Roles = db.LOD_RolesCttosContrato.Where(x => x.IdContrato == id).OrderBy(x => x.IdRCContrato).ToList();
            foreach (var Rol in contrato.Roles)
            {
                List<LOD_UsuariosLod> Users = db.LOD_UsuariosLod.Where(x => x.IdRCContrato == Rol.IdRCContrato).Distinct().ToList();
                Rol.Usuarios = new List<LOD_UsuariosLod>();
                foreach (var user in Users)
                {
                    if (!Rol.Usuarios.Select(x => x.UserId).Contains(user.UserId))
                        Rol.Usuarios.Add(user);
                }

                if(Rol.IdRolCtto == null)
                {
                    MAE_RolesContrato newRol = new MAE_RolesContrato() { 
                        IdRolCtto = 0,
                        Jerarquia = 6,
                        NombreRol = "Complementario"
                    };

                    Rol.MAE_RolesContrato = newRol;
                    Rol.IdRolCtto = 0;
                }
            }

            ViewBag.PermiteEditar = true;
            if(db.LOD_UsuariosLod.Where(x => x.Activo && x.LOD_LibroObras.IdContrato == contrato.IdContrato).Count() > 0)
                ViewBag.PermiteEditar = false;

            ViewBag.EsInspectorFiscal = false;
            ViewBag.EsControlContratos = false;
            ViewBag.EsAdministradorMOP = false;
            string userId = User.Identity.GetUserId();
            ViewBag.UsuarioActual = userId;

            List<LOD_RolesCttosContrato> rolesUsuario = db.LOD_UsuariosLod.Where(x => x.UserId == userId && x.LOD_LibroObras.IdContrato == contrato.IdContrato && x.Activo).Select(x => x.LOD_RolesCttosContrato).ToList();
            if (rolesUsuario.Count() > 0 && rolesUsuario.First() != null)
            {
                if (rolesUsuario.Select(x => x.IdRolCtto).Contains(2))
                { //2 es el rol Inspector Fiscal en la Tabla MAE_RolesContratos
                    ViewBag.EsInspectorFiscal = true;
                }
                else if (rolesUsuario.Select(x => x.IdRolCtto).Contains(19))
                { //19 es rol control de contratos
                    ViewBag.EsControlContratos = true;
                }
                else if (rolesUsuario.Select(x => x.IdRolCtto).Contains(21))
                {//20 es rol adminsitrador MOP
                    ViewBag.EsAdministradorMOP = true;
                }
            }
            ViewBag.EsAdministradorPlataforma = false;
            List<ApplicationRole> roles = db.Roles.Where(x => x.Users.Select(a => a.UserId).Contains(userId)).ToList();
            if (roles.Select(x => x.Name).Contains("Administrador Plataforma")) //VEr si es administrador de plataforma
            {
                ViewBag.EsAdministradorPlataforma = true;
            }


            ViewBag.ContratoLiquidado = false;
            if (contrato.EstadoContrato == 3)
                ViewBag.ContratoLiquidado = true;



            return PartialView("_getDetailsContrato", contrato);
        }

        public async Task<ActionResult> getTableRoles(int id)
        {

            //Mandamos Roles del Contrato
            List<LOD_RolesCttosContrato> Roles = db.LOD_RolesCttosContrato.Where(x => x.IdContrato == id).OrderBy(x => x.IdRCContrato).ToList();
            foreach (var Rol in Roles)
            {
                List<LOD_UsuariosLod> Users = db.LOD_UsuariosLod.Where(x => x.IdRCContrato == Rol.IdRCContrato).Distinct().ToList();
                Rol.Usuarios = new List<LOD_UsuariosLod>();
                foreach (var user in Users)
                {
                    if (!Rol.Usuarios.Select(x => x.UserId).Contains(user.UserId))
                        Rol.Usuarios.Add(user);
                }

                //if (Rol.IdRolCtto == null)
                //{
                //    MAE_RolesContrato newRol = new MAE_RolesContrato()
                //    {
                //        IdRolCtto = 0,
                //        Jerarquia = 6,
                //        NombreRol = "Complementario"
                //    };

                //    Rol.MAE_RolesContrato = newRol;
                //    Rol.IdRolCtto = 0;
                //}
            }

            CON_Contratos contrato = db.CON_Contratos.Find(id);
            ViewBag.EsInspectorFiscal = false;
            ViewBag.EsControlContratos = false;
            ViewBag.EsAdministradorMOP = false;
            string userId = User.Identity.GetUserId();
            ViewBag.UsuarioActual = userId;

            List<LOD_RolesCttosContrato> rolesUsuario = db.LOD_UsuariosLod.Where(x => x.UserId == userId && x.LOD_LibroObras.IdContrato == contrato.IdContrato && x.Activo).Select(x => x.LOD_RolesCttosContrato).ToList();
            if (rolesUsuario.Select(x => x.IdRolCtto).Contains(2))
            { //2 es el rol Inspector Fiscal en la Tabla MAE_RolesContratos
                ViewBag.EsInspectorFiscal = true;
            }
            else if (rolesUsuario.Select(x => x.IdRolCtto).Contains(19))
            { //19 es rol control de contratos
                ViewBag.EsControlContratos = true;
            }
            else if (rolesUsuario.Select(x => x.IdRolCtto).Contains(21))
            {//20 es rol adminsitrador MOP
                ViewBag.EsAdministradorMOP = true;
            }

            ViewBag.EsAdministradorPlataforma = false;
            List<ApplicationRole> roles = db.Roles.Where(x => x.Users.Select(a => a.UserId).Contains(userId)).ToList();
            if (roles.Select(x => x.Name).Contains("Administrador Plataforma")) //VEr si es administrador de plataforma
            {
                ViewBag.EsAdministradorPlataforma = true;
            }


            
            ViewBag.ContratoLiquidado = false;
            if (contrato.EstadoContrato == 3)
                ViewBag.ContratoLiquidado = true;

            return PartialView("_getTableRolesContrato", Roles);
        }




        public ActionResult DesactivarRol(int idUser, int IdRol)
        {

            LOD_UsuariosLod userLod = db.LOD_UsuariosLod.Find(idUser);
            userLod.LOD_RolesCttosContrato = db.LOD_RolesCttosContrato.Find(IdRol);

            return PartialView("_DesactivarRol", userLod);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DesactivarRol(LOD_UsuariosLod userLod)
        {
            try {
                List<LOD_UsuariosLod> lOD_UsuariosLod = db.LOD_UsuariosLod.Where(x => x.UserId == userLod.UserId && x.IdRCContrato == userLod.IdRCContrato).ToList();
                foreach (var item in lOD_UsuariosLod)
                {
                    item.Activo = false;
                    item.FechaDesactivacion = DateTime.Now;
                    db.Entry(item).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                }               

                LOD_RolesCttosContrato rol = db.LOD_RolesCttosContrato.Find(userLod.IdRCContrato);
                string response = "true;" + rol.IdContrato;

                Log_Helper log_Helper = new Log_Helper();
                string accion = "Se ha desactivado el usuario "+lOD_UsuariosLod.First().ApplicationUser.NombreCompleto+" en el Contrato: "+rol.CON_Contratos.CodigoContrato+"-"+ rol.CON_Contratos.NombreContrato;
                bool res = await log_Helper.SetObjectLog(2, rol.CON_Contratos, accion, User.Identity.GetUserId());

                return Content(response);
            }catch(Exception ex)
            {
                return Content(ex.Message);
            }
            
        }

        public ActionResult ActivarRol(string idUser, int IdRol, int IdContrato)
        {

            LOD_UsuariosLod userLod = db.LOD_UsuariosLod.Where(x => x.UserId == idUser && x.Activo && x.LOD_RolesCttosContrato.IdContrato == IdContrato).FirstOrDefault();
            LOD_UsuariosLod user = db.LOD_UsuariosLod.Where(x => x.UserId == idUser && x.IdRCContrato == IdRol).FirstOrDefault();
            LOD_RolesCttosContrato rol = db.LOD_RolesCttosContrato.Find(IdRol);
            ViewBag.PermiteActivar = true;
            if (userLod != null)
                ViewBag.PermiteActivar = false;

            ViewBag.NombreRol = rol.NombreRol;

            return PartialView("_ActivarRol", user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ActivarRol(LOD_UsuariosLod userLod)
        {
            try
            {
                List<LOD_UsuariosLod> lOD_UsuariosLod = db.LOD_UsuariosLod.Where(x => x.UserId == userLod.UserId && x.IdRCContrato == userLod.IdRCContrato).ToList();
                foreach (var item in lOD_UsuariosLod)
                {
                    item.Activo = true;
                    db.Entry(item).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                }    

                LOD_RolesCttosContrato rol = db.LOD_RolesCttosContrato.Find(userLod.IdRCContrato);

                Log_Helper log_Helper = new Log_Helper();
                string accion = "Se ha activado el usuario " + lOD_UsuariosLod.First().ApplicationUser.NombreCompleto + " en el Contrato: " + rol.CON_Contratos.CodigoContrato + "-" + rol.CON_Contratos.NombreContrato;
                bool res = await log_Helper.SetObjectLog(2, rol.CON_Contratos, accion, User.Identity.GetUserId());

                string response = "true;" + rol.IdContrato;
                return Content(response);
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }

        }


        // GET: ASP/Contratos/Create
        public ActionResult Create(int? Padre, string Tipo)
        {
            //Parametros del Modal
            ViewBag.Titulo = "Nuevo Contrato";
            ViewBag.ClsModal = "hmodal-success";
            ViewBag.Color = "success";
            ViewBag.Action = "Create";
            //Preparación de combos

            CON_Contratos contrato = new CON_Contratos();
            contrato.IdCarpeta = null;
            if (Padre != null)
            {
                contrato.IdCarpeta = Padre;
            }
            contrato.FechaCreacionContrato = DateTime.Now;
            //Ver Tema de Estado del Contrato
            //Estado: 0(Creado)
            //        1(Iniciado)
            //        2(Cerrado)
            contrato.EstadoContrato = 0;
            //................................
            ViewBag.Edit = false;

            //ViewBag.IdEmpresaContratista = new SelectList(new List<MAE_sujetoEconomico>(), "IdSujEcon", "RazonSocial");
            ViewBag.IdAdminContrato = new SelectList(new List<ApplicationUser>(), "UserId", "Nombre");

            var sujetos = db.MAE_sujetoEconomico.Where(x => x.EsContratista).ToList();
            List<AutoSearch> search = new List<AutoSearch>();
            search.Add(new AutoSearch()
            {
                id = "0",
                name = "<Seleccione Empresa Contratista>"
            });
            foreach (var item in sujetos)
            {
                search.Add(new AutoSearch()
                {
                    id = item.IdSujEcon.ToString(),
                    name = item.Rut + "; " + item.RazonSocial.ToUpper()
                });
            }
            ViewBag.IdEmpresaContratista = new SelectList(search, "id", "name", "0");

            //Enviar Dirección del Contrato
            var sucursales = db.MAE_Sucursal.Where(x => x.MAE_sujetoEconomico.EsGubernamental).OrderBy(x => x.Sucursal).ToList();
            List<AutoSearch> search2 = new List<AutoSearch>();
            search2.Add(new AutoSearch()
            {
                id = "0",
                name = "<Seleccione Dirección del MOP>"
            });
            foreach (var item in sucursales)
            {
                search2.Add(new AutoSearch()
                {
                    id = item.IdSucursal.ToString(),
                    name = item.Sucursal.ToUpper() + " - "+ item.MAE_sujetoEconomico.NomFantasia.ToUpper()
                });
            }
            ViewBag.IdDireccionContrato = new SelectList(search2, "id", "name", "0");
            //Traer Empresa Fiscalizadora
            var Fiscalizadoras = db.MAE_sujetoEconomico.Where(x => x.EsMandante).OrderBy(x => x.RazonSocial).ToList();
            List<AutoSearch> search3 = new List<AutoSearch>();
            search3.Add(new AutoSearch()
            {
                id = "0",
                name = "<Seleccione Empresa Fiscalizadora>"
            });
            foreach (var item in Fiscalizadoras)
            {
                search3.Add(new AutoSearch()
                {
                    id = item.IdSujEcon.ToString(),
                    name = item.Rut + "; " + item.RazonSocial.ToUpper()
                });
            }
            ViewBag.IdEmpresaFiscalizadora = new SelectList(search3, "id", "name", "0");//Ver si ponemos el nombre de Fantasia

            ViewBag.PermiteEditarFiscalizadora = true;
            ViewBag.PermiteEditar = true;
            ViewBag.PermiteEditarContratista = true;
            ViewBag.PermiteEditarMop = true;


            return PartialView("_modalForm", contrato);
        }

        // POST: ASP/Contratos/Create
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CON_Contratos contrato)
        {
            try
            {
                if (!reCaptchaHelper.IsCaptchaValid(Request.Form["foo"]))
                {
                    var response = "Error en el captcha, Intente nuevamente.";
                    return Content(response);
                }

                if (contrato.fileImage != null)
                {
                    //Ruta imagen
                    string filePath = Server.MapPath("~/Images/Contratos/");
                    string fileExt = Path.GetExtension(contrato.fileImage.FileName);
                    string nombreImage = "Cont_" + contrato.NombreContrato + fileExt;
                    //ACA_Archivos aca_archivos = new ACA_Archivos();
                    File_Result save_file = ACA_Archivos.saveFile(contrato.fileImage, filePath, nombreImage, true);
                    contrato.RutaImagenContrato = "~/Images/Contratos/" + nombreImage;
                    //*************************************//
                }

                if(contrato.IdDireccionContrato == 0)
                    return Content("Debe Seleccionar una Dirección MOP");
                
                if (contrato.IdTipoContrato == 0) contrato.IdTipoContrato = null;
                if (contrato.IdEmpresaContratista == 0) contrato.IdEmpresaContratista = null;
                if (contrato.IdEmpresaFiscalizadora == 0) contrato.IdEmpresaFiscalizadora = null;
                contrato.FechaCreacionContrato = DateTime.Now;
                contrato.UserId = User.Identity.GetUserId();
                contrato.EstadoContrato = 1;


                db.CON_Contratos.Add(contrato);
                db.SaveChanges();
                bool LObligatorios = LibrosObligatorios(contrato.IdContrato);
                bool RolesContrato = FuncionRolesContrato(contrato.IdContrato);

                string content = Convert.ToString(contrato.IdContrato);
                return Content("true;" + content);
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
        }

        public ActionResult CreateNuevoRol(int id)
        {
            CON_Contratos contrato = db.CON_Contratos.Find(id);
            List<LOD_LibroObras> libros = db.LOD_LibroObras.Where(x => x.IdContrato == contrato.IdContrato).ToList();
            CreateRolesView model = new CreateRolesView();
            model.IdContrato = contrato.IdContrato;

            ViewBag.Titulo = "Nuevo Rol para: "+contrato.CodigoContrato + "-"+contrato.NombreContrato ;
            ViewBag.ClsModal = "hmodal-success";
            ViewBag.Color = "info";
            ViewBag.Action = "CreateNuevoRol";

            return PartialView("_CreateNuevoRol", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateNuevoRol(CreateRolesView model)
        {
            try
            {
                if (!reCaptchaHelper.IsCaptchaValid(Request.Form["foo"]))
                {
                    var responseRe = "Error en el captcha, Intente nuevamente.";
                    return Content(responseRe);
                }

                LOD_RolesCttosContrato rol = new LOD_RolesCttosContrato();
                rol.IdContrato = model.IdContrato;
                rol.NombreRol = model.NombreRol;
                rol.Descripcion = model.Descripcion;
                db.LOD_RolesCttosContrato.Add(rol);
                await db.SaveChangesAsync();

                string response = "true;" + model.IdContrato;
                return Content(response);

            }catch(Exception ex)
            {
                return Content(ex.Message);
            }
        }

        public bool FuncionRolesContrato(int IdContrato)
        {
            try
            {
                //Buscamos los tipos de libro
                List<int> IdsTipoLod = db.LOD_LibroObras.Where(x => x.IdContrato == IdContrato).Select(x => x.MAE_TipoLOD.IdTipoLod).ToList();

                if (IdsTipoLod.Count() > 0)
                {
                    IdsTipoLod = IdsTipoLod.Distinct().ToList();
                    foreach (int IdTipo in IdsTipoLod)
                    {
                        //Obteneos los roles asociados
                        List<MAE_RolesContrato> RolesPC = db.MAE_RolesContrato.Where(x => x.IdTipoLod == IdTipo && x.Activo).ToList();
                        //Revisamos Casos
                        foreach (MAE_RolesContrato RolPC in RolesPC)
                        {
                            //1.-Crear Rol en el contrato
                            LOD_RolesCttosContrato RolCtto = new LOD_RolesCttosContrato()
                            {
                                IdContrato = IdContrato,
                                IdRolCtto = RolPC.IdRolCtto,
                                NombreRol = RolPC.NombreRol,
                                Descripcion = RolPC.Descripcion
                            };
                            db.LOD_RolesCttosContrato.Add(RolCtto);
                            db.SaveChanges();

                            //Creamos permisos del ROL
                            List<LOD_PermisosRolesContrato> Permisos = new List<LOD_PermisosRolesContrato>();
                            //1.-En Funcion del Mismo Libro
                            if (RolPC.Lectura)
                            {
                                LOD_PermisosRolesContrato Permiso = new LOD_PermisosRolesContrato()
                                {
                                    IdRCContrato = RolCtto.IdRCContrato,
                                    IdLod = db.LOD_LibroObras.Where(x => x.IdTipoLod == RolPC.IdTipoLod && x.IdContrato == IdContrato).FirstOrDefault().IdLod,
                                    Lectura = RolPC.Lectura,
                                    Escritura = RolPC.Escritura,
                                    FirmaGob = RolPC.FirmaGob,
                                    FirmaFea = RolPC.FirmaFea,
                                    FirmaSimple = RolPC.FirmaSimple,
                                };
                                Permisos.Add(Permiso);
                            }
                            //2.-En Funcion del Libro Maestro IdTipoLod=1
                            if (RolPC.Lectura1)
                            {
                                LOD_PermisosRolesContrato Permiso = new LOD_PermisosRolesContrato()
                                {
                                    IdRCContrato = RolCtto.IdRCContrato,
                                    IdLod = db.LOD_LibroObras.Where(x => x.IdTipoLod == 1 && x.IdContrato == IdContrato).FirstOrDefault().IdLod,
                                    Lectura = RolPC.Lectura1,
                                    Escritura = RolPC.Escritura1,
                                    FirmaGob = RolPC.FirmaGob1,
                                    FirmaFea = RolPC.FirmaFea1,
                                    FirmaSimple = RolPC.FirmaSimple1,
                                };
                                Permisos.Add(Permiso);
                            }
                            //3.-En Funcion del Libro Comunicaciones IdTipoLod=2
                            if (RolPC.Lectura2)
                            {
                                LOD_PermisosRolesContrato Permiso = new LOD_PermisosRolesContrato()
                                {
                                    IdRCContrato = RolCtto.IdRCContrato,
                                    IdLod = db.LOD_LibroObras.Where(x => x.IdTipoLod == 2 && x.IdContrato == IdContrato).FirstOrDefault().IdLod,
                                    Lectura = RolPC.Lectura2,
                                    Escritura = RolPC.Escritura2,
                                    FirmaGob = RolPC.FirmaGob2,
                                    FirmaFea = RolPC.FirmaFea2,
                                    FirmaSimple = RolPC.FirmaSimple2,
                                };
                                Permisos.Add(Permiso);
                            }
                            //4.-En Funcion de los Libros Comunicaciones Obligatorios 
                            List<int> IdsLodCom = db.MAE_TipoLOD.Where(x => x.TipoLodJer == 2 && x.EsObligatorio && x.IdTipoLod != 2).Select(x => x.IdTipoLod).ToList();
                            foreach (int IdTLod in IdsLodCom)
                            {
                                if (RolPC.Lectura3)
                                {
                                    LOD_PermisosRolesContrato Permiso = new LOD_PermisosRolesContrato()
                                    {
                                        IdRCContrato = RolCtto.IdRCContrato,
                                        IdLod = db.LOD_LibroObras.Where(x => x.IdTipoLod == IdTLod && x.IdContrato == IdContrato).FirstOrDefault().IdLod,
                                        Lectura = RolPC.Lectura3,
                                        Escritura = RolPC.Escritura3,
                                        FirmaGob = RolPC.FirmaGob3,
                                        FirmaFea = RolPC.FirmaFea3,
                                        FirmaSimple = RolPC.FirmaSimple3,
                                    };
                                    Permisos.Add(Permiso);
                                }
                            }
                            db.LOD_PermisosRolesContrato.AddRange(Permisos);
                            db.SaveChanges();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }




        public bool LibrosObligatorios(int IdContrato)
        {
            try
            {
                //Creamos carpeta Comunicaciones y carpeta de Complementarios
                List<LOD_Carpetas> Carpetas = new List<LOD_Carpetas>();
                LOD_Carpetas comunicaciones = new LOD_Carpetas()
                {
                    IdContrato = IdContrato,
                    NombreCarpeta = "2-Comunicaciones",
                    UserId = User.Identity.GetUserId(),
                    FechaCreacion = DateTime.Now,
                    EsPortafolio = false
                };
                Carpetas.Add(comunicaciones);
                LOD_Carpetas Complementarios = new LOD_Carpetas()
                {
                    IdContrato = IdContrato,
                    NombreCarpeta = "3-Complementarios",
                    UserId = User.Identity.GetUserId(),
                    FechaCreacion = DateTime.Now,
                    EsPortafolio = false
                };
                Carpetas.Add(Complementarios);
                db.LOD_Carpetas.AddRange(Carpetas);
                db.SaveChanges();
                List<LOD_LibroObras> Libros = new List<LOD_LibroObras>();
                List<MAE_TipoLOD> Tlibros = db.MAE_TipoLOD.Where(x => x.Activo && x.EsObligatorio).OrderBy(x => x.TipoLodJer).ToList();
                int i = 1;
                foreach (var tl in Tlibros)
                {
                    LOD_LibroObras Libro = new LOD_LibroObras()
                    {

                        IdContrato = IdContrato,
                        NombreLibroObra = Convert.ToString(tl.TipoLodJer) + "-" + tl.Nombre,
                        IdTipoLod = tl.IdTipoLod,
                        FechaCreacion = DateTime.Now,
                        Estado = 0,//Creado
                        UserId = User.Identity.GetUserId()
                    };
                    if (tl.TipoLodJer == 2 || tl.TipoLodJer == 3)
                    {
                        Libro.IdCarpeta = comunicaciones.IdCarpeta;
                        Libro.NombreLibroObra = Convert.ToString(tl.TipoLodJer) + "." + i + "-" + tl.Nombre;
                        i++;
                    }

                    Libros.Add(Libro);
                }
                db.LOD_LibroObras.AddRange(Libros);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }

        public async Task<ActionResult> LiquidacionContrato(int IdContrato)
        {
            CON_Contratos contrato = db.CON_Contratos.Find(IdContrato);
            List<LOD_LibroObras> librosContrato = db.LOD_LibroObras.Where(x => x.IdContrato == contrato.IdContrato).ToList();
            List<LOD_LibroObras> librosCerrados = db.LOD_LibroObras.Where(x => x.IdContrato == contrato.IdContrato && x.Estado == 2).ToList();
            List<LOD_LibroObras> librosPorCerrar = db.LOD_LibroObras.Where(x => x.IdContrato == contrato.IdContrato && x.Estado == 1).ToList();
            List<MAE_TipoDocumento> listadoDocCargados = db.LOD_docAnotacion.Where(x => x.LOD_Anotaciones.LOD_LibroObras.IdContrato == contrato.IdContrato && x.LOD_Anotaciones.Estado == 2).Select(x => x.MAE_TipoDocumento).ToList();
            List<MAE_TipoDocumento> listaDocRequeridos = GetDocLiquidacion();

            List<int> listadoInt = GetDocLiquidacion().Select(x => x.IdTipo).ToList();
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


        [HttpPost, ActionName("LiquidacionContrato")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> LiquidacionContrato(LiquidacionLibroView liquidacion)
        {
            try
            {
                CON_Contratos contrato = await db.CON_Contratos.FindAsync(liquidacion.IdContrato);
                contrato.EstadoContrato = 3; //CONSULTAR ESTADOS

                db.Entry(contrato).State = EntityState.Modified;
                db.SaveChanges();
                string response = "true;" + contrato.IdContrato;

                Log_Helper log_Helper = new Log_Helper();
                string accion = "Se ha liquidado el contrato: " + contrato.CodigoContrato + "-" + contrato.NombreContrato;
                bool res = await log_Helper.SetObjectLog(2, contrato, accion, User.Identity.GetUserId());

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

        public ActionResult PermisosRol(int id)
        {
            //Buscar Permisos del Rol
            List<LOD_PermisosRolesContrato> permisos = new List<LOD_PermisosRolesContrato>();
            
            LOD_RolesCttosContrato rol = db.LOD_RolesCttosContrato.Where(x => x.IdRCContrato == id).FirstOrDefault();
            
            ViewBag.Titulo = "Permisos del Rol: " + rol.NombreRol.ToString();
            if (db.LOD_PermisosRolesContrato.Where(x => x.IdRCContrato == id).OrderBy(x => x.LOD_LibroObras.MAE_TipoLOD.IdTipoLod).Count() > 0)
            {
                permisos = db.LOD_PermisosRolesContrato.Where(x => x.IdRCContrato == id).OrderBy(x => x.LOD_LibroObras.MAE_TipoLOD.IdTipoLod).ToList();                
            }

            List<int> idLibros = permisos.Select(x => x.IdLod).ToList();
            //Parametros del Modal
            ViewBag.PermiteCrear = false;
            if(db.LOD_LibroObras.Where(x => x.IdContrato == rol.IdContrato).Count() > db.LOD_LibroObras.Where(x => idLibros.Contains(x.IdLod)).Count())
                ViewBag.PermiteCrear = true;

            ViewBag.ClsModal = "hmodal-info";
            ViewBag.Color = "info";
            ViewBag.Action = "index";
            ViewBag.IdRCContrato = rol.IdRCContrato;
            foreach (var item in permisos)
            {
                string[] numeracion = item.LOD_LibroObras.NombreLibroObra.Split('-')[0].Split('.');
                if (numeracion.Length > 1)
                {
                    item.Indice = Convert.ToInt32(numeracion[0]);
                    item.SubIndice = Convert.ToInt32(numeracion[1]);
                }
                else
                {
                    item.Indice = Convert.ToInt32(numeracion[0]);
                    item.SubIndice = 0;
                }
            }

            CON_Contratos contrato = db.CON_Contratos.Where(x => x.IdContrato == rol.IdContrato).FirstOrDefault();
            ViewBag.ContratoLiquidado = false;
            if (contrato.EstadoContrato == 3)
                ViewBag.ContratoLiquidado = true;

            return PartialView("_modalFormPermisosRol", permisos);
        }

        public ActionResult GetPermisosRol(int id)
        {
            //Buscar Permisos del Rol
            List<LOD_PermisosRolesContrato> permisos = new List<LOD_PermisosRolesContrato>();
            ViewBag.Titulo = "Permisos del Rol: " + db.LOD_RolesCttosContrato.Where(x => x.IdRCContrato == id).FirstOrDefault().NombreRol.ToString();
            if (db.LOD_PermisosRolesContrato.Where(x => x.IdRCContrato == id).OrderBy(x => x.LOD_LibroObras.MAE_TipoLOD.IdTipoLod).Count() > 0)
            {
                permisos = db.LOD_PermisosRolesContrato.Where(x => x.IdRCContrato == id).OrderBy(x => x.LOD_LibroObras.MAE_TipoLOD.IdTipoLod).ToList();
            }


            int idContrato = db.LOD_RolesCttosContrato.Where(x => x.IdRCContrato == id).FirstOrDefault().IdContrato;
            CON_Contratos contrato = db.CON_Contratos.Find(idContrato);
            ViewBag.ContratoLiquidado = false;
            if(contrato.EstadoContrato == 3)
                ViewBag.ContratoLiquidado = true;

            return PartialView("_GetTablePermisosRol", permisos);
        }

        public ActionResult CrearPermisoRol(int id)
        {
            //Buscar Permisos del Rol
            LOD_PermisosRolesContrato permiso = new LOD_PermisosRolesContrato();

            //Parametros del Modal
            LOD_RolesCttosContrato rol = db.LOD_RolesCttosContrato.Find(id);
            List<int> IdLibrosExist = db.LOD_PermisosRolesContrato.Where(x => x.IdRCContrato == rol.IdRCContrato).Select(x => x.IdLod).ToList();
            List<LOD_LibroObras> search = db.LOD_LibroObras.Where(x => x.IdContrato == rol.IdContrato && !IdLibrosExist.Contains(x.IdLod)).ToList();
            ViewBag.IdLod = new SelectList(new List<ApplicationUser>(), "IdLod", "NombreLibroObra");
            ViewBag.IdLod = new SelectList(search, "IdLod", "NombreLibroObra", "0");

            ViewBag.Titulo = "Crear Permiso para: " + rol.NombreRol;
            ViewBag.ClsModal = "hmodal-success";
            ViewBag.Color = "success";
            ViewBag.Action = "CrearPermisoRol";
            permiso.IdRCContrato = rol.IdRCContrato;
            ViewBag.Create = true;
            return PartialView("_EditarPermisosRol", permiso);
        }

        [HttpPost, ActionName("CrearPermisoRol")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CrearPermisoRol(LOD_PermisosRolesContrato permiso)
        {
            try
            {
                if (!reCaptchaHelper.IsCaptchaValid(Request.Form["foo"]))
                {
                    var responseRe = "Error en el captcha, Intente nuevamente.";
                    return Content(responseRe);
                }

                db.LOD_PermisosRolesContrato.Add(permiso);
                await db.SaveChangesAsync();

                LOD_RolesCttosContrato rol = db.LOD_RolesCttosContrato.Find(permiso.IdRCContrato);
                string response = "true;" + rol.IdRCContrato;

                bool activo = AsignarUsuarioALod(rol.IdRCContrato, permiso.IdLod);
                LOD_LibroObras libro = db.LOD_LibroObras.Find(permiso.IdLod);

                Log_Helper log_Helper = new Log_Helper();
                string accion = "Se ha añadido el permiso para el libro "+libro.NombreLibroObra+" en el rol: " + rol.NombreRol + ", en el Contrato: " + rol.CON_Contratos.CodigoContrato + "-" + rol.CON_Contratos.NombreContrato;
                bool res = await log_Helper.SetObjectLog(2, rol.CON_Contratos, accion, User.Identity.GetUserId());


                return Content(response);
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
        }

        //Cada vez se asocia un permiso a un rol se debe asignar los usuarios pertenecientes al rol al libro
        public bool AsignarUsuarioALod(int IdRol, int IdLod)
        {
            List<string> usuarios = db.LOD_UsuariosLod.Where(x => x.IdRCContrato == IdRol ).Select(x => x.UserId).Distinct().ToList();
            foreach (var item in usuarios)
            {
                int verdaderos = db.LOD_UsuariosLod.Where(x => x.IdRCContrato == IdRol && x.UserId == item && x.Activo).Count();
                bool activo = false;
                if (verdaderos > 0) activo = true;

                LOD_UsuariosLod user = new LOD_UsuariosLod() { 
                    IdRCContrato = IdRol,
                    IdLod = IdLod,
                    UserId = item,
                    Activo = activo,
                    FechaActivacion = DateTime.Now
                };

                db.LOD_UsuariosLod.Add(user);
                db.SaveChanges();
            }

            return true;
        }


        public ActionResult EditarPermisoRol(int id)
        {
            //Buscar Permisos del Rol
            LOD_PermisosRolesContrato permiso = db.LOD_PermisosRolesContrato.Find(id);

            ViewBag.Titulo = "Editar Permisos para: " + permiso.LOD_LibroObras.NombreLibroObra;
            ViewBag.ClsModal = "hmodal-warning";
            ViewBag.Color = "warning";
            ViewBag.Action = "EditarPermisoRol";

            ViewBag.Create = false;

            return PartialView("_EditarPermisosRol", permiso);
        }

        [HttpPost, ActionName("EditarPermisoRol")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditarPermisoRol(LOD_PermisosRolesContrato permiso)
        {
            try
            {
                LOD_PermisosRolesContrato EditPermiso = db.LOD_PermisosRolesContrato.Find(permiso.IdPermiso);
                EditPermiso.Lectura = permiso.Lectura;
                EditPermiso.Escritura = permiso.Escritura;
                EditPermiso.FirmaFea = permiso.FirmaFea;
                EditPermiso.FirmaGob = permiso.FirmaGob;
                EditPermiso.FirmaSimple = permiso.FirmaSimple;
                db.Entry(EditPermiso).State = EntityState.Modified;
                await db.SaveChangesAsync();

                Log_Helper log_Helper = new Log_Helper();
                string accion = "Se ha editado el permiso para el Libro "+ EditPermiso.LOD_LibroObras.NombreLibroObra + " en el rol: " + EditPermiso.LOD_RolesCttosContrato.NombreRol + ", en el Contrato: " + EditPermiso.LOD_LibroObras.CON_Contratos.CodigoContrato + "-" + EditPermiso.LOD_LibroObras.CON_Contratos.NombreContrato;
                bool res = await log_Helper.SetObjectLog(2, EditPermiso.LOD_LibroObras.CON_Contratos, accion, User.Identity.GetUserId());

                LOD_RolesCttosContrato rol = db.LOD_RolesCttosContrato.Find(permiso.IdRCContrato);
                string response = "true;" + rol.IdRCContrato;
                return Content(response);
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
        }


        //public ActionResult DesactivarRolCto(int id)
        //{
        //    LOD_RolesCttosContrato Rol = db.LOD_RolesCttosContrato.Find(id);


        //    return PartialView("DesactivarRolCto", Rol);
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> DesactivarRolCto(LOD_RolesCttosContrato Rol)
        //{
        //    try
        //    {
        //        List<LOD_UsuariosLod> lOD_UsuariosLod = db.LOD_UsuariosLod.Where(x => x.UserId == userLod.UserId && x.IdRCContrato == userLod.IdRCContrato).ToList();
        //        foreach (var item in lOD_UsuariosLod)
        //        {
        //            item.Activo = true;
        //            db.Entry(item).State = EntityState.Modified;
        //            await db.SaveChangesAsync();
        //        }

        //        LOD_RolesCttosContrato rol = db.LOD_RolesCttosContrato.Find(userLod.IdRCContrato);
        //        string response = "true;" + rol.IdContrato;
        //        return Content(response);
        //    }
        //    catch (Exception ex)
        //    {
        //        return Content(ex.Message);
        //    }

        //}



        public ActionResult AsignarARol(int id)
        {
            //Obtener las Sucursales
            LOD_RolesCttosContrato Rol = db.LOD_RolesCttosContrato.Find(id);
            List<int> sucursales = new List<int>();
            if (Rol.MAE_RolesContrato != null)
            {
                if (Rol.MAE_RolesContrato.RolGubernamental)
                {
                    sucursales.Add(Convert.ToInt32(Rol.CON_Contratos.IdDireccionContrato));
                }
                List<int> sucContratista = new List<int>();

                if (Rol.CON_Contratos.IdEmpresaContratista != null && Rol.MAE_RolesContrato.RolContratista)
                {
                    sucContratista = db.MAE_Sucursal.Where(x => x.IdSujeto == Rol.CON_Contratos.IdEmpresaContratista).Select(x => x.IdSucursal).ToList();
                    sucursales.AddRange(sucContratista);
                }
                List<int> sucFiscalizadores = new List<int>();
                if (Rol.CON_Contratos.IdEmpresaFiscalizadora != null && Rol.MAE_RolesContrato.RolFiscalizador)
                {
                    sucFiscalizadores = db.MAE_Sucursal.Where(x => x.IdSujeto == Rol.CON_Contratos.IdEmpresaFiscalizadora).Select(x => x.IdSucursal).ToList();
                    sucursales.AddRange(sucFiscalizadores);
                }
            }

            else
            {
                sucursales = db.MAE_Sucursal.Where(x => x.IdSujeto == Rol.CON_Contratos.IdEmpresaContratista || x.IdSujeto == Rol.CON_Contratos.IdEmpresaFiscalizadora).Select(x => x.IdSucursal).ToList();
                sucursales.Add(Convert.ToInt32(Rol.CON_Contratos.IdDireccionContrato));
            }


            //Buscar Personal asociados al rol del contrato:
            List<string> userIdsExistentes = db.LOD_UsuariosLod.Where(x => x.LOD_LibroObras.IdContrato == Rol.IdContrato && x.Activo).Select(x => x.UserId).Distinct().ToList();

            //Buscar Permisos del Rol
            LOD_UsuariosLod usuario = new LOD_UsuariosLod()
            {
                IdRCContrato = id,
            };
            //Parametros del Modal
            ViewBag.Titulo = "Asignar Usuario a Rol: " + Rol.NombreRol;
            ViewBag.ClsModal = "hmodal-success";
            ViewBag.Color = "success";
            ViewBag.Action = "AsignarARol";

            //Buscamos Personas para descontar del Rol
            //List<string> Existentes = db.LOD_UsuariosLod.Where(x => x.IdRCContrato == Rol.IdRCContrato).Distinct().Select(x => x.UserId).ToList();

            //Buscar usuarios a asignar
            ViewBag.UserId = new SelectList(new List<ApplicationUser>(), "UserId", "Nombre");
            //Faltaria solo diferenciar por rol
            var sujetos = db.Users.Where(x => sucursales.Contains(x.IdSucursal) && !userIdsExistentes.Contains(x.Id)).ToList();
            //var sujetos = db.Users.ToList();
            List<AutoSearch> search = new List<AutoSearch>();
            search.Add(new AutoSearch()
            {
                id = "0",
                name = "<Seleccione Usuario>"
            });
            foreach (var item in sujetos)
            {
                search.Add(new AutoSearch()
                {
                    id = item.Id.ToString(),
                    name = item.NombreCompleto + " - " + item.MAE_Sucursal.MAE_sujetoEconomico.NomFantasia 
                });
            }
            ViewBag.UserId = new SelectList(search, "id", "name", "0");



            return PartialView("_modalFormAsignarARol", usuario);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AsignarARol(LOD_UsuariosLod Usuario)
        {
            try
            {
                //Buscar IdContrato
                int IdContrato = db.LOD_RolesCttosContrato.Where(x => x.IdRCContrato == Usuario.IdRCContrato).FirstOrDefault().IdContrato;
                //debo Buscar los libros del contrato para asociar
                List<int> IdLods = db.LOD_PermisosRolesContrato.Where(x => x.IdRCContrato == Usuario.IdRCContrato).Select(x => x.IdLod).Distinct().ToList();
                List<LOD_UsuariosLod> accesosUsuario = new List<LOD_UsuariosLod>();
                foreach (int IdLod in IdLods)
                {
                    LOD_UsuariosLod acUser = new LOD_UsuariosLod()
                    {
                        IdLod = IdLod,
                        Activo = true,
                        FechaActivacion = DateTime.Now,
                        UserId = Usuario.UserId,
                        IdRCContrato = Usuario.IdRCContrato
                    };
                    accesosUsuario.Add(acUser);
                }

                db.LOD_UsuariosLod.AddRange(accesosUsuario);
                db.SaveChanges();
                string content = Convert.ToString(IdContrato);

                LOD_RolesCttosContrato rol = db.LOD_RolesCttosContrato.Where(x => x.IdRCContrato == Usuario.IdRCContrato).FirstOrDefault();
                ApplicationUser user = db.LOD_UsuariosLod.Where(x => x.UserId == Usuario.UserId).Select(x => x.ApplicationUser).First();
                Log_Helper log_Helper = new Log_Helper();
                string accion = "Se ha asignado al rol: "+rol.NombreRol+" al usuario " + user.NombreCompleto + " en el Contrato: " + rol.CON_Contratos.CodigoContrato + "-" + rol.CON_Contratos.NombreContrato;
                bool res = await log_Helper.SetObjectLog(2, rol.CON_Contratos, accion, User.Identity.GetUserId());


                return Content("true;" + content);
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
        }

        public ActionResult Edit(int? id)
        {
            CON_Contratos contrato = db.CON_Contratos.Find(id);
            //Parametros del Modal
            ViewBag.Titulo = "Editar Contrato";
            ViewBag.ClsModal = "hmodal-warning";
            ViewBag.Color = "warning";
            ViewBag.Action = "Edit";

            //................................
            ViewBag.Edit = true;

            ViewBag.IdAdminContrato = new SelectList(new List<ApplicationUser>(), "UserId", "Nombre");

            var sujetos = db.MAE_sujetoEconomico.Where(x => x.EsContratista).ToList();
            List<AutoSearch> search = new List<AutoSearch>();
            search.Add(new AutoSearch()
            {
                id = "0",
                name = "<Seleccione Empresa Contratista>"
            });
            foreach (var item in sujetos)
            {
                search.Add(new AutoSearch()
                {
                    id = item.IdSujEcon.ToString(),
                    name = item.Rut + "; " + item.RazonSocial.ToUpper()
                });
            }
            ViewBag.IdEmpresaContratista = new SelectList(search, "id", "name", contrato.IdEmpresaContratista.ToString());

            //Enviar Dirección del Contrato
            var sucursales = db.MAE_Sucursal.Where(x => x.MAE_sujetoEconomico.EsGubernamental).OrderBy(x => x.Sucursal).ToList();
            List<AutoSearch> search2 = new List<AutoSearch>();
            search2.Add(new AutoSearch()
            {
                id = "0",
                name = "<Seleccione Dirección del MOP>"
            });
            foreach (var item in sucursales)
            {
                search2.Add(new AutoSearch()
                {
                    id = item.IdSucursal.ToString(),
                    name = item.Sucursal.ToUpper() + " - " + item.MAE_sujetoEconomico.NomFantasia.ToUpper()
                });
            }

            ViewBag.IdDireccionContrato = new SelectList(search2, "id", "name", contrato.IdDireccionContrato.ToString());
            //Traer Empresa Fiscalizadora
            var Fiscalizadoras = db.MAE_sujetoEconomico.Where(x => x.EsMandante).OrderBy(x => x.RazonSocial).ToList();
            List<AutoSearch> search3 = new List<AutoSearch>();
            search3.Add(new AutoSearch()
            {
                id = "0",
                name = "<Seleccione Empresa Fiscalizadora>"
            });
            foreach (var item in Fiscalizadoras)
            {
                search3.Add(new AutoSearch()
                {
                    id = item.IdSujEcon.ToString(),
                    name = item.Rut + "; " + item.RazonSocial.ToUpper()
                });
            }

            ViewBag.IdEmpresaFiscalizadora = new SelectList(search3, "id", "name", contrato.IdEmpresaFiscalizadora.ToString());//Ver si ponemos el nombre de Fantasia
            string userId = User.Identity.GetUserId();
            ViewBag.PermiteEditar = true;

            if (db.LOD_UsuariosLod.Where(x => x.LOD_RolesCttosContrato.IdContrato == contrato.IdContrato && (x.LOD_LibroObras.Estado == 1 || x.Activo)).Count() > 0)
            {
                ViewBag.PermiteEditarFiscalizadora = false;
                ViewBag.PermiteEditar = false;
                ViewBag.PermiteEditarContratista = false;
                ViewBag.PermiteEditarMOP = false;
                contrato.MOP = contrato.IdDireccionContrato.Value;
                contrato.Contratista = contrato.IdEmpresaContratista.Value;
                contrato.Fiscalizadora = contrato.IdEmpresaFiscalizadora.Value;
            }
            else
            {
                ViewBag.PermiteEditarMOP = true;
                if (db.LOD_UsuariosLod.Where(x => x.LOD_LibroObras.IdContrato == contrato.IdContrato && x.Activo && x.IdRCContrato != null && x.ApplicationUser.IdSucursal == contrato.IdDireccionContrato).Count() > 0)
                {
                    ViewBag.PermiteEditarMOP = false;
                    contrato.MOP = contrato.IdDireccionContrato.Value;
                }

                ViewBag.PermiteEditarContratista = true;
                List<int> listadoSucursalContratista = db.MAE_Sucursal.Where(x => x.IdSujeto == contrato.IdEmpresaContratista).Select(x => x.IdSucursal).ToList();
                if (db.LOD_UsuariosLod.Where(x => x.LOD_LibroObras.IdContrato == contrato.IdContrato && x.Activo && x.IdRCContrato != null && listadoSucursalContratista.Contains(x.ApplicationUser.IdSucursal)).Count() > 0)
                {
                    ViewBag.PermiteEditarContratista = false;
                    contrato.Contratista = contrato.IdEmpresaContratista.Value;
                }

                ViewBag.PermiteEditarFiscalizadora = true;
                List<int> listadoSucursalFiscalizadora = db.MAE_Sucursal.Where(x => x.IdSujeto == contrato.IdEmpresaFiscalizadora).Select(x => x.IdSucursal).ToList();
                if (db.LOD_UsuariosLod.Where(x => x.LOD_LibroObras.IdContrato == contrato.IdContrato && x.Activo && x.IdRCContrato != null && listadoSucursalFiscalizadora.Contains(x.ApplicationUser.IdSucursal)).Count() > 0)
                {
                    ViewBag.PermiteEditarFiscalizadora = false;
                    contrato.Fiscalizadora = contrato.IdEmpresaFiscalizadora.Value;
                }
            }

            if (ValidaPermisos.ValidaPermisosEnController("0020000002"))
            {
                return PartialView("_modalForm", contrato);
            }
            else
            {
                return RedirectToAction("SinPermiso", "../Pub");
            }

            
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(CON_Contratos modelActual)
        {
            if (!reCaptchaHelper.IsCaptchaValid(Request.Form["foo"]))
            {
                var response = "Error en el captcha, Intente nuevamente.";
                return Content(response);
            }

            if (modelActual.IdEmpresaFiscalizadora == 0) modelActual.IdEmpresaFiscalizadora = null;
            if (modelActual.RutaImagenContrato != null && modelActual.fileImage != null)
            {
               
                HttpPostedFileBase ImagenActual = modelActual.fileImage;
                byte[] ImgAct = new byte[modelActual.fileImage.ContentLength];
                string ImgAnt64 = TransImgString(modelActual.RutaImagenContrato);
                string ImgAct64 = TransImgString(modelActual.fileImage);
                //fin Imagen 2

                //Comparamos Imagenes
                if ((ImgAnt64 != ImgAct64) && ImgAnt64 != string.Empty && ImgAct64 != string.Empty)
                {
                    modelActual.RutaImagenContrato = GuardarImagenEnCarpeta("~/Images/Contratos/", modelActual.fileImage, modelActual.NombreContrato);
                }else if(modelActual.RutaImagenContrato != null && ImgAct64 != string.Empty)
                {
                    modelActual.RutaImagenContrato = GuardarImagenEnCarpeta("~/Images/Contratos/", modelActual.fileImage, modelActual.NombreContrato);
                }
            }
            else
            {
                
                if (modelActual.fileImage != null)
                {
                    modelActual.RutaImagenContrato = GuardarImagenEnCarpeta("~/Images/Contratos/", modelActual.fileImage, modelActual.NombreContrato);
                }
            }

            try
            {
                CON_Contratos contrato = db.CON_Contratos.Find(modelActual.IdContrato);

                if (db.LOD_UsuariosLod.Where(x => (x.LOD_LibroObras.Estado == 1 || x.Activo)&& x.LOD_LibroObras.IdContrato == contrato.IdContrato).Count() > 0)
                {
                    contrato.IdDireccionContrato = modelActual.MOP;
                    contrato.IdEmpresaContratista = modelActual.Contratista;
                    contrato.IdEmpresaFiscalizadora = modelActual.Fiscalizadora;
                }
                else
                {
                    if (db.LOD_UsuariosLod.Where(x => x.LOD_LibroObras.IdContrato == contrato.IdContrato && x.Activo && x.IdRCContrato != null && x.ApplicationUser.IdSucursal == contrato.IdDireccionContrato).Count() > 0)
                    {
                        contrato.IdDireccionContrato = modelActual.MOP;
                    }
                    else
                    {
                        contrato.IdDireccionContrato = modelActual.IdDireccionContrato.Value;
                    }

                    List<int> listadoSucursalContratista = db.MAE_Sucursal.Where(x => x.IdSujeto == contrato.IdEmpresaContratista).Select(x => x.IdSucursal).ToList();
                    if (db.LOD_UsuariosLod.Where(x => x.LOD_LibroObras.IdContrato == contrato.IdContrato && x.Activo && x.IdRCContrato != null && listadoSucursalContratista.Contains(x.ApplicationUser.IdSucursal)).Count() > 0)
                    {
                        contrato.IdEmpresaContratista = modelActual.Contratista;
                    }
                    else
                    {
                        if (modelActual.IdEmpresaContratista != null)
                        {
                            contrato.IdEmpresaContratista = modelActual.IdEmpresaContratista.Value;
                        }
                    }

                    List<int> listadoSucursalFiscalizadora = db.MAE_Sucursal.Where(x => x.IdSujeto == contrato.IdEmpresaFiscalizadora).Select(x => x.IdSucursal).ToList();
                    if (db.LOD_UsuariosLod.Where(x => x.LOD_LibroObras.IdContrato == contrato.IdContrato && x.Activo && x.IdRCContrato != null && listadoSucursalFiscalizadora.Contains(x.ApplicationUser.IdSucursal)).Count() > 0)
                    {
                        contrato.IdEmpresaFiscalizadora = modelActual.Fiscalizadora;
                    }
                    else
                    {
                        if (modelActual.IdEmpresaFiscalizadora != null)
                        {
                            contrato.IdEmpresaFiscalizadora = modelActual.IdEmpresaFiscalizadora.Value;
                        }
                    }
                }

                contrato.MontoInicialContrato = modelActual.MontoInicialContrato;
                contrato.PlazoInicialContrato = modelActual.PlazoInicialContrato;
                contrato.NombreContrato = modelActual.NombreContrato;
                contrato.RutaImagenContrato = modelActual.RutaImagenContrato;
                contrato.CodigoContrato = modelActual.CodigoContrato;
                contrato.FechaInicioContrato = modelActual.FechaInicioContrato;
                contrato.DescripcionContrato = modelActual.DescripcionContrato;

                db.Entry(contrato).State = EntityState.Modified;
                db.SaveChanges();
                return Content("true;" + modelActual.IdContrato.ToString());
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }

        }

        public string TransImgString(HttpPostedFileBase Imagen)
        {
            try
            {
                byte[] ImgAct = new byte[Imagen.ContentLength];

                BinaryReader theReader = new BinaryReader(Imagen.InputStream);
                ImgAct = theReader.ReadBytes(Imagen.ContentLength);
                return Convert.ToBase64String(ImgAct);
            }
            catch
            {
                return string.Empty;
            }
        }

        public string TransImgString(string ruta)
        {
            try
            {
                MemoryStream ImagenAnterior = new MemoryStream();
                using (FileStream fs = System.IO.File.OpenRead(Server.MapPath(ruta)))
                {
                    fs.CopyTo(ImagenAnterior);
                }
                byte[] ImgAnt = ImagenAnterior.ToArray();
                return Convert.ToBase64String(ImgAnt);
            }
            catch
            {
                return string.Empty;
            }
        }

        //Función Guardar imagen 
        public string GuardarImagenEnCarpeta(string ruta, HttpPostedFileBase archivo, string nombreCont)
        {
            //Ruta imagen
            nombreCont = nombreCont.Replace("/", "-");
            string RutaImg = string.Empty;
            string filePath = Server.MapPath(ruta);
            string fileExt = Path.GetExtension(archivo.FileName);
            string nombreImage = "Contrato_" + nombreCont + fileExt;
            //ACA_Archivos aca_archivos = new ACA_Archivos();
            File_Result save_file = ACA_Archivos.saveFile(archivo, filePath, nombreImage, true);
            RutaImg = "/Images/Contratos/" + nombreImage;
            //*************************************//
            return RutaImg;
        }


        public async Task<ActionResult> Delete(int? id)
        {
            CON_Contratos contratos = await db.CON_Contratos.FindAsync(id);
            ViewBag.PermiteEliminar = true;
            if (contratos.EstadoContrato != 1 || db.LOD_LibroObras.Where(x => x.IdContrato == contratos.IdContrato && x.Estado != 0).Count() > 0)
                ViewBag.PermiteEliminar = false;

            if (ValidaPermisos.ValidaPermisosEnController("0020000003"))
            {

                return PartialView("_Delete", contratos);
            }
            else
            {
                return RedirectToAction("SinPermiso", "../Pub");
            }

        }

        // POST: ASP/Contratos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(CON_Contratos contratoDelete)
        {
            try
            {
                CON_Contratos contrato = await db.CON_Contratos.FindAsync(contratoDelete.IdContrato);
                //obtener Padre
                string padre = contrato.IdCarpeta.ToString();
                List<LOD_RolesCttosContrato> roles = db.LOD_RolesCttosContrato.Where(x => x.IdContrato == contrato.IdContrato).ToList();
                foreach (var item in roles)
                {

                    List<LOD_PermisosRolesContrato> permisos = db.LOD_PermisosRolesContrato.Where(x => x.IdRCContrato == item.IdRCContrato).ToList();
                    foreach (var perm in permisos)
                    {
                        db.LOD_PermisosRolesContrato.Remove(perm);
                        await db.SaveChangesAsync();
                    }

                    List<LOD_UsuariosLod> lod_usuarios = db.LOD_UsuariosLod.Where(x => x.IdRCContrato == item.IdRCContrato).ToList();
                    foreach (var user in lod_usuarios)
                    {
                        db.LOD_UsuariosLod.Remove(user);
                        await db.SaveChangesAsync();
                    }

                    db.LOD_RolesCttosContrato.Remove(item);
                    await db.SaveChangesAsync();
                }
                List<LOD_LibroObras> libros = db.LOD_LibroObras.Where(x => x.IdContrato == contrato.IdContrato).ToList();
                foreach (var item in libros)
                {
                    db.LOD_LibroObras.Remove(item);
                    await db.SaveChangesAsync();
                }

                List<LOD_Carpetas> carpetas = db.LOD_Carpetas.Where(x => x.IdContrato == contrato.IdContrato).ToList();
                foreach (var item in carpetas)
                {
                    db.LOD_Carpetas.Remove(item);
                    await db.SaveChangesAsync();
                }

                db.CON_Contratos.Remove(contrato);
                await db.SaveChangesAsync();
                return Content("delete;" + padre);
            }
            catch (Exception err)
            {
                return Content(err.Message);
            }
        }

        ////Cambio JT 22-05-2018 Al activar el contrato se activaran sus objetos superiores(Proyecto y Obra)
        //public void ActivarObra(int IdObra)
        //{
        //    if (db.ASP_obras.Find(IdObra).IdEstado != 17)
        //    {
        //        ASP_obras obra = db.ASP_obras.Find(IdObra);
        //        obra.FechaInicioObra = DateTime.Now;
        //        obra.IdEstado = 17;
        //        db.Entry(obra).State = EntityState.Modified;
        //        db.SaveChanges();
        //    }
        //}

        //public void ActivarProyecto(int IdProyecto)
        //{
        //    int estado = db.ASP_proyectos.Find(IdProyecto).IdEstado.Value;
        //    if (estado != 17 && estado != 11 && estado != 12)
        //    {
        //        ASP_proyectos proyecto = db.ASP_proyectos.Find(IdProyecto);
        //        proyecto.FechaInicioProy = DateTime.Now;
        //        proyecto.IdEstado = 17;
        //        db.Entry(proyecto).State = EntityState.Modified;
        //        db.SaveChanges();
        //    }
        //}
        ////FIN.


        //public int BuscarDependenciaDeProyecto(ASP_carpetas carpeta)
        //{
        //    ASP_carpetas carpeta2 = db.ASP_carpetas.Find(carpeta.IdCarpPadre);
        //    if (carpeta2.IdProyecto != null)
        //    {
        //        return Convert.ToInt32(carpeta2.IdProyecto);
        //    }
        //    else if (carpeta2.IdCarpPadre == null)
        //    {
        //        return 0; // es hijo de la raiz Portafolio
        //    }
        //    else
        //    {
        //        return BuscarDependenciaDeProyecto(carpeta2);
        //    }
        //}

        ////*********
        //public int BuscarDependenciaDeObra(ASP_carpetas carpeta)
        //{
        //    ASP_carpetas carpeta2 = db.ASP_carpetas.Find(carpeta.IdCarpPadre);
        //    if (carpeta2.IdObra!= null)
        //    {
        //        return Convert.ToInt32(carpeta2.IdObra);
        //    }
        //    else if (carpeta2.IdCarpPadre == null)
        //    {
        //        return 0; // es hijo de la raiz Portafolio
        //    }
        //    else
        //    {
        //        return BuscarDependenciaDeObra(carpeta2);
        //    }

        //}


        //public ASP_log FunctionLog(DateTime FechaLog, string UserId, int Objeto, int IdObjeto,
        //                         string Accion, string Campo, string ValorAnterior, string ValorActualizado)
        //{
        //    string StringObjeto = "";
        //    switch (Objeto)
        //    {
        //        case 1:
        //            StringObjeto = "Proyecto";
        //            break;
        //        case 2:
        //            StringObjeto = "Obra";
        //            break;
        //        case 3:
        //            StringObjeto = "Contrato";
        //            break;
        //    }

        //    try
        //    {
        //        ASP_log log = new ASP_log()
        //        {
        //            FechaLog = FechaLog,
        //            UserId = UserId,
        //            Objeto = StringObjeto,
        //            IdObjeto = IdObjeto,
        //            Accion = Accion,
        //            Campo = Campo,
        //            ValorAnterior = ValorAnterior,
        //            ValorActualizado = ValorActualizado
        //        };
        //        //db.ASP_log.Add(log);
        //        //db.SaveChanges();
        //        return log;
        //    }
        //    catch (Exception ex)
        //    {
        //        return null;
        //    }

        //}



        ////****EVALUAR CONTRATOS*************************************************
        //// GET: ASP/Contratos/Edit/5
        //public ActionResult Evaluar(int idEvaluacion, int idContrato)
        //{
        //    EVA_evaluaciones Evaluacion = db.EVA_evaluaciones.Find(idEvaluacion);
        //    ASP_contratos Contrato = db.ASP_contratos.Find(idContrato);
        //    EVA_formularioEvaluacion Formulario = db.EVA_formularioEvaluacion.Find(Evaluacion.IdFormEval);
        //    //Formulario.Creador = db.Users.Find(Formulario.UserId).Nombre;
        //    //Parametros del Modal

        //    ViewBag.Titulo = "Evaluación de Contrato";
        //    ViewBag.idContrato = Contrato.IdContrato;
        //    ViewBag.ObjetoEvaluable = Contrato;
        //    ViewBag.Formulario = Formulario;
        //    ViewBag.FechaProgramada = Evaluacion.FechaProgram.Date;
        //    ViewBag.EvaluacioId = Evaluacion.IdEvaluacion;
        //    ViewBag.Estado = Evaluacion.EstadosEval;
        //    ViewBag.ClsModal = "hmodal-info";
        //    ViewBag.Color = "info";
        //    ViewBag.Action = "Evaluar";
        //    //Buscar el rangos de evaluación
        //    ViewBag.RangosMin = db.EVA_rangosEvaluacion.Where(x => x.IdEscala == Formulario.IdEscala).Min(x => x.RangoEvalInicial);
        //    ViewBag.RangosMax = db.EVA_rangosEvaluacion.Where(x => x.IdEscala == Formulario.IdEscala).Max(x => x.RangoEvalFinal);

        //    return PartialView("_modalFormEvaluacion");
        //}

        ////[HttpPost]
        ////[ValidateAntiForgeryToken]
        ////public ActionResult Evaluar(string form,int idContrato)
        ////{
        ////    string UserId= User.Identity.GetUserId();
        ////    List<EVA_RecepcionEval> Respuestas = JsonConvert.DeserializeObject<List<EVA_RecepcionEval>>(form);
        ////    EVA_formularioEvaluacion Formulario = db.EVA_formularioEvaluacion.Find(Convert.ToInt64(Respuestas[2].value));
        ////    bool realizado= Evaluaciones.Evaluar(Respuestas, Formulario, UserId);
        ////    if (realizado)
        ////    {
        ////        return Content("true;"+ idContrato);
        ////    }
        ////    return Content("Ocurrió un Error al realizar la evaluacion");
        ////}

        //public ActionResult getTableEvaluacion(int id)
        //{
        //    //BUscamos las evaluaciones existentes
        //    ASP_contratos contrato = db.ASP_contratos.Find(id);
        //    List<int> EvalEnContratos = db.EVA_EvaluacionContrato.Where(x => x.IdContrato == id).Select(x => x.IdEvaluacion).ToList();
        //    DateTime FechaMax = DateTime.Today.Date.AddDays(3).Date;
        //    contrato.EVA_evaluaciones = db.EVA_evaluaciones.
        //        Where(x => EvalEnContratos.Contains(x.IdEvaluacion) == true &&
        //        x.FechaProgram <= FechaMax).OrderByDescending(x => x.FechaProgram).ToList();
        //    //Peridiciodad
        //    //List<ComboBoxEstandar2> LstPeriodos = Evaluaciones.periodos();
        //    //foreach (EVA_evaluaciones eval in contrato.EVA_evaluaciones)
        //    //{
        //    //    eval.IdContrato = contrato.IdContrato;
        //    //    eval.NombrePeridiocidad = LstPeriodos[eval.EVA_FormulariosObjetos.peridiocidad - 1].Value;
        //    //    if (eval.IdUsuario != null)
        //    //    {
        //    //        eval.NombreEvaluador = db.Users.Find(eval.IdUsuario).Nombre;
        //    //    }
        //    //} 
        //    return PartialView("_getTableEvaluaciones", contrato.EVA_evaluaciones);
        //}


        ////**********************************************************************

        ////******ITEMIZADO*******************************************************
        //public ActionResult Itemizado(int idContrato)
        //{
        //    List<ASP_itemizadoContrato> itemizado = db.ASP_itemizadoContrato.
        //        Where(x => x.IdContrato == idContrato && x.Descontado==false).
        //        OrderBy(x=>x.Item).ToList();




        //    ASP_contratos Contrato = db.ASP_contratos.Find(idContrato);
        //    //Parametros del Modal
        //    ViewBag.Titulo = "Itemizado Estado de Pago";
        //    ViewBag.idContrato = Contrato.IdContrato;
        //    ViewBag.ObjetoEvaluable = Contrato;
        //    ViewBag.ClsModal = "hmodal-info";
        //    ViewBag.Color = "info";
        //    ViewBag.Action = "Itemizado";
        //    //Resultados
        //    if (db.ASP_epCaratulaContrato.Where(x => x.IdContrato == idContrato).Count() > 1)
        //    {
        //        int max = db.ASP_epCaratulaContrato.Where(x => x.IdContrato == idContrato).Max(x => x.NumeroEP);
        //        ViewBag.PorcAcomFinan = db.ASP_epCaratulaContrato
        //            .Where(x => x.IdContrato == idContrato && x.NumeroEP == max)
        //            .First().PorcAvanceFinanAcom;
        //        if (max >= 1)
        //        {
        //           ViewBag.TotalAcomulado = db.ASP_epCaratulaContrato
        //          .Where(x => x.IdContrato == idContrato && x.NumeroEP > 0 && x.NumeroEP <= max)
        //          .Sum(x => x.TotalItems);
        //        }
        //        else
        //        {
        //            ViewBag.TotalAcomuladoAnterior = 0;
        //        }
        //        if (max > 1)
        //        {             
        //            ViewBag.TotalAcomuladoAnterior = ViewBag.TotalAcomulado -
        //                (db.ASP_epCaratulaContrato.Where(x => x.IdContrato == idContrato && x.NumeroEP == max - 1)
        //                .Sum(x => x.TotalItems));
        //        }
        //        else
        //        {
        //            ViewBag.TotalAcomuladoAnterior = 0;
        //        }
        //    }
        //    else
        //    {
        //        ViewBag.PorcAcomFinan = 0;
        //        ViewBag.TotalAcomulado = 0;
        //        ViewBag.TotalAcomuladoAnterior = 0;
        //    }
        //    //Total Itemizado
        //    decimal TotalItemizado = db.ASP_itemizadoContrato.
        //        Where(x => x.IdContrato == idContrato && x.Descontado == false)
        //        .Sum(x => x.PrecioUnitario*x.Cantidad);
        //    ViewBag.TotalItemizado = TotalItemizado;
        //    return PartialView("_modalFormItemizado",itemizado);
        //}

        ////******Anticipo********************************************************
        //// GET: ASP/Contratos/Edit/5
        //public ActionResult Anticipo(int idContrato, bool SinAnticipo = false)
        //{
        //    ASP_contratos Contrato = db.ASP_contratos.Find(idContrato);
        //    ASP_epCaratulaContrato Caratula = new ASP_epCaratulaContrato();
        //    Caratula.PagoAnticipo = Convert.ToDecimal(db.ASP_contratos.Find(idContrato).Anticipo);
        //    Caratula.FechaEP = DateTime.Today;
        //    Caratula.IdContrato = idContrato;
        //    Caratula.PagoAnticipo = Convert.ToDecimal(db.ASP_contratos.Find(idContrato).Anticipo);
        //    Caratula.FechaEP = DateTime.Now;
        //    Caratula.EstadoEP = 10;
        //    Caratula.RefEP = "EP N°0 'Anticipo'";
        //    ViewBag.ClsModal = "hmodal-success";
        //    ViewBag.Color = "success";
        //    ViewBag.Action = "Anticipo";


        //    if (!SinAnticipo)
        //    {
        //        //Parametros del Modal
        //        ViewBag.Titulo = "EP N°0 'Anticipo'";          
        //        ViewBag.ObjetoEvaluable = Contrato;
        //        ViewBag.Abreviatura = Contrato.MAE_moneda.Abreviatura;
        //        Caratula.EstadoEP = 2;
        //        return PartialView("_modalFormAnticipo", Caratula);
        //    }
        //    else
        //    {
        //        ViewBag.NombreContrato = Contrato.NombreContrato;
        //        ViewBag.Titulo = "Inicio Proceso Estados de Pago";
        //        return PartialView("_modalFormInicioProcesoEP",Caratula);
        //    }
        //}

        //[HttpPost]
        ////[ValidateAntiForgeryToken]
        //public ActionResult Anticipo(ASP_epCaratulaContrato Caratula)
        //{
        //    Caratula.NumeroEP = 0;
        //    Caratula.EstadoEP = 2;
        //    Caratula.IdTipoEP = 0;
        //    Caratula.FechaCreacion = DateTime.Now;
        //    Caratula.ValorFinalNeto = Caratula.PagoAnticipo;
        //    if (Caratula.ValorFinalNeto != 0)
        //    {
        //        Caratula.IvaEp = Math.Round((Convert.ToDecimal(Caratula.PagoAnticipo) * 19 / 100), 2);

        //        //NOTIFICACION********************************************
        //        NotificacionEP noti = new NotificacionEP();
        //        noti.EmailNotEP(Caratula.EstadoEP, Caratula);
        //        //********************************************************
        //    }
        //    else
        //    {
        //        Caratula.EstadoEP = 10;
        //    }
        //    try
        //    {
        //        db.ASP_epCaratulaContrato.Add(Caratula);
        //        db.SaveChanges();

        //    }
        //    catch (Exception e)
        //    {
        //        return Content("Error " + e);
        //    }

        //    //APROBAR ESTADO DE PAGO
        //    //Caratula.MensajeAprobador = "Aprobado EP" + DateTime.Today.ToLongDateString();
        //    string UserId = User.Identity.GetUserId();
        //    int IdPersonal = db.SEG_UserPersonal.Where(x => x.UserId == UserId).FirstOrDefault().IdPersonal;
        //    List<ASP_aprobacionesEP> aSP_AprobacionesEP = new List<ASP_aprobacionesEP>();
        //    ASP_aprobacionesEP AprobacionesITO = new ASP_aprobacionesEP()
        //    {
        //        IdAprobador = IdPersonal,
        //        IdCaratulaEP = Caratula.IdCaratulaEP,
        //        FechaAprobacion = DateTime.Now,
        //        Activo = false,
        //        EstadoEP = 1,
        //        ResolucionAprueba = true
        //    };

        //    aSP_AprobacionesEP.Add(AprobacionesITO);
        //    ASP_aprobacionesEP AprobacionesContratista = new ASP_aprobacionesEP()
        //    {
        //        IdAprobador = Convert.ToInt32(db.ASP_contratos.Find(Caratula.IdContrato).IdAdminContrato),
        //        IdCaratulaEP = Caratula.IdCaratulaEP,
        //        FechaAprobacion = DateTime.Now,
        //        Activo = true,
        //        EstadoEP = Caratula.EstadoEP,
        //        ResolucionAprueba = true
        //    };
        //    aSP_AprobacionesEP.Add(AprobacionesContratista);

        //    try
        //    {
        //        if (db.ASP_aprobacionesEP.Where(x => x.IdCaratulaEP == Caratula.IdCaratulaEP && x.Activo).Count() > 0)
        //        {
        //            ASP_aprobacionesEP ActivoAprob = db.ASP_aprobacionesEP.Where(x => x.IdCaratulaEP == Caratula.IdCaratulaEP && x.Activo).First();
        //            ActivoAprob.Activo = false;
        //            db.Entry(ActivoAprob).State = EntityState.Modified;
        //        }
        //        db.ASP_aprobacionesEP.AddRange(aSP_AprobacionesEP);
        //        db.SaveChanges();
        //        return Content("true;" + Caratula.IdContrato);
        //    }
        //    catch (Exception e)
        //    {
        //        return Content("Error");
        //    }
        //}


        //public ActionResult getTableAnticipo(int id)
        //{
        //    ValidaPermisos ValidaPermisos = new ValidaPermisos();
        //    ViewBag.ItoMandante = ValidaPermisos.EnSesionItoMandante(id);
        //    //...............................................
        //    List<ASP_epCaratulaContrato> EPs = db.ASP_epCaratulaContrato.Where(x => x.IdContrato == id).ToList();
        //    return PartialView("_getTableCaratulaEp", EPs);
        //}



        //public ActionResult getTablePanelContratos(int IdEmpresa)
        //{
        //    List<ASP_contratos> contratos = db.ASP_contratos.Where(x => x.IdEmpresa == IdEmpresa).ToList();
        //    return PartialView("_getTableResumenContratos", contratos);
        //}
        ////**********************************************************************

        ////*****Devolución de Retenciones****************************************
        //public ActionResult DevRetenciones(int idContrato)
        //{

        //    ASP_contratos Contrato = db.ASP_contratos.Find(idContrato);
        //    ASP_epCaratulaContrato Caratula = new ASP_epCaratulaContrato();
        //    //ER 04-03-2021 Modificacion para Obtener Las Retenciones Totales segun si hay o no Modificaciones
        //    decimal TotalNetoContrato = 0;
        //    if (db.ASP_HistoricoMontosContrato.Where(x => x.IdContrato == idContrato && x.Activo).Count() == 0)
        //    {
        //        //ER 30-01-2020 Modificacion para ajustar pesos excedentes
        //        TotalNetoContrato = Convert.ToDecimal(db.ASP_contratos.Find(idContrato).MontoContrato);
        //        Caratula.IdHMC = null;
        //    }
        //    else
        //    {
        //        TotalNetoContrato = Convert.ToDecimal(db.ASP_HistoricoMontosContrato.
        //                            Where(x=>x.IdContrato==idContrato &&x.Activo)
        //                            .First().MontoContrato);
        //        Caratula.IdHMC = Convert.ToInt16(db.ASP_HistoricoMontosContrato.
        //                            Where(x => x.IdContrato == idContrato && x.Activo)
        //                            .First().IdHMC);
        //    }
        //    decimal SumEpsAnterios = db.ASP_epCaratulaContrato.Where(x => x.IdContrato == idContrato).Sum(x => x.ValorFinalNeto);

        //    //***************************************************************************************************************
        //    int numeroCaratula = db.ASP_epCaratulaContrato.Where(x => x.IdContrato == idContrato).Max(x => x.NumeroEP) + 1;

        //    Caratula.IdContrato = idContrato;
        //    Caratula.NumeroEP = numeroCaratula;
        //    Caratula.DevRetenciones = Convert.ToDecimal(db.ASP_epCaratulaContrato.Where(x => x.IdContrato == idContrato).Sum(x => x.Retenciones));
        //    //ER 30-01-2020 Cuadramos las retenciones*******************
        //    Caratula.DevRetenciones =TotalNetoContrato - SumEpsAnterios;
        //   //***********************************************************
        //   Caratula.FechaEP = DateTime.Now;
        //    //Parametros del Modal
        //    ViewBag.Titulo = "EP N°"+ numeroCaratula + " 'Dev. Retenciones'";
        //    Caratula.RefEP = ViewBag.Titulo;
        //    ViewBag.ObjetoEvaluable = Contrato;
        //    ViewBag.ClsModal = "hmodal-danger";
        //    ViewBag.Color = "danger";
        //    ViewBag.Action = "DevRetenciones";
        //    ViewBag.Abreviatura = Contrato.MAE_moneda.Abreviatura;
        //    return PartialView("_modalFormDevRetenciones", Caratula);
        //}

        //[HttpPost]
        ////[ValidateAntiForgeryToken]
        //public ActionResult DevRetenciones(ASP_epCaratulaContrato Caratula)
        //{
        //    Caratula.EstadoEP = 2;
        //    Caratula.IdTipoEP = 2;
        //    Caratula.ValorFinalNeto = Caratula.DevRetenciones;
        //    Caratula.FechaCreacion = DateTime.Now;

        //    Caratula.PorcAvanceFinanAcom = Math.Round(db.ASP_epCaratulaContrato.AsNoTracking().Where(x => x.IdContrato == Caratula.IdContrato && x.NumeroEP == (Caratula.NumeroEP - 1)).First().PorcAvanceFinanAcom, 0);
        //    Caratula.PorcAvanceFisicoAcom =Math.Round(db.ASP_epCaratulaContrato.AsNoTracking().Where(x => x.IdContrato == Caratula.IdContrato && x.NumeroEP == (Caratula.NumeroEP - 1)).First().PorcAvanceFisicoAcom,0);
        //    try
        //    {
        //        db.ASP_epCaratulaContrato.Add(Caratula);
        //        db.SaveChanges();
        //        //NOTIFICACION********************************************
        //        NotificacionEP noti = new NotificacionEP();
        //        noti.EmailNotEP(Caratula.EstadoEP, Caratula);
        //        //********************************************************
        //    }
        //    catch (Exception e)
        //    {
        //        return Content("Error");
        //    }

        //    //APROBAR ESTADO DE PAGO
        //    //Caratula.MensajeAprobador = "Aprobado EP" + DateTime.Today.ToLongDateString();
        //    string UserId = User.Identity.GetUserId();
        //    int IdPersonal = db.SEG_UserPersonal.Where(x => x.UserId == UserId).FirstOrDefault().IdPersonal;
        //    List<ASP_aprobacionesEP> aSP_AprobacionesEP = new List<ASP_aprobacionesEP>();
        //    ASP_aprobacionesEP AprobacionesITO = new ASP_aprobacionesEP()
        //    {
        //        IdAprobador = IdPersonal,
        //        IdCaratulaEP = Caratula.IdCaratulaEP,
        //        FechaAprobacion = DateTime.Now,
        //        Activo = false,
        //        EstadoEP = 1,
        //        ResolucionAprueba = true
        //    };

        //    aSP_AprobacionesEP.Add(AprobacionesITO);
        //    ASP_aprobacionesEP AprobacionesContratista = new ASP_aprobacionesEP()
        //    {
        //        IdAprobador = Convert.ToInt32(db.ASP_contratos.Find(Caratula.IdContrato).IdAdminContrato),
        //        IdCaratulaEP = Caratula.IdCaratulaEP,
        //        FechaAprobacion = DateTime.Now,
        //        Activo = true,
        //        EstadoEP = Caratula.EstadoEP,
        //        ResolucionAprueba = true
        //    };
        //    aSP_AprobacionesEP.Add(AprobacionesContratista);

        //    try
        //    {
        //        if (db.ASP_aprobacionesEP.Where(x => x.IdCaratulaEP == Caratula.IdCaratulaEP && x.Activo).Count() > 0)
        //        {
        //            ASP_aprobacionesEP ActivoAprob = db.ASP_aprobacionesEP.Where(x => x.IdCaratulaEP == Caratula.IdCaratulaEP && x.Activo).First();
        //            ActivoAprob.Activo = false;
        //            db.Entry(ActivoAprob).State = EntityState.Modified;
        //        }
        //        db.ASP_aprobacionesEP.AddRange(aSP_AprobacionesEP);
        //        db.SaveChanges();
        //        return Content("true;" + Caratula.IdContrato);
        //    }
        //    catch (Exception e)
        //    {
        //        return Content("Error");
        //    }
        //}

        //public ActionResult MotivoRechazoEP(int idCaratula)
        //{
        //    ASP_epCaratulaContrato Caratula = db.ASP_epCaratulaContrato.Find(idCaratula);
        //    ASP_contratos Contrato = db.ASP_contratos.Find(Caratula.IdContrato);
        //    //Parametros del Modal

        //    ViewBag.Titulo = "Motivo Rechazo EP N°"+Caratula.NumeroEP;
        //    Caratula.RefEP = ViewBag.Titulo;
        //    ViewBag.ObjetoEvaluable = Contrato;
        //    ViewBag.ClsModal = "hmodal-info";
        //    ViewBag.Color = "info";
        //    return PartialView("_modalFormRechazoInfo", Caratula);
        //}
        ////**********************************************************************

        ////Aprobaciones**********************************************************

        //public ActionResult AprobarEP(int idCaratula)
        //{

        //    ASP_epCaratulaContrato Caratula = db.ASP_epCaratulaContrato.Where(x => x.IdCaratulaEP == idCaratula).First();
        //    //ASP_contratos Contrato = db.ASP_contratos.Find(Caratula.IdContrato);
        //    //ViewBag.ObjetoEvaluable = Contrato;
        //    //Parametros del Modal
        //    ViewBag.Titulo = "Aprobar EP N°" + Caratula.NumeroEP;
        //    //ViewBag.ObjetoEvaluable = Contrato;
        //    ViewBag.ClsModal = "hmodal-success";
        //    ViewBag.Color = "success";
        //    ViewBag.Action = "AprobarEP";
        //    return PartialView("_modalFormAprobarEp", Caratula);
        //}

        //[HttpPost]
        //public ActionResult AprobarEP(ASP_epCaratulaContrato car)
        //{
        //    NotificacionEP noti = new NotificacionEP();
        //    ASP_epCaratulaContrato Caratula = db.ASP_epCaratulaContrato.Where(x => x.IdCaratulaEP == car.IdCaratulaEP).First();


        //    if (Caratula.EstadoEP == 3 || Caratula.EstadoEP == 7)
        //    {
        //        Caratula.EstadoEP = 4;
        //        //noti.EmailNotEP(4, Caratula);
        //        Helper_Evaluacion HE = new Helper_Evaluacion();
        //        //HE Estados 1: No corresponde Evaluar;  
        //        //          -1: No existe Programación;
        //        //           2: Evaluación Creada; 
        //        //          -2: Corresponde pero Faltan Roles ; 
        //        //           3: Evaluación Realizada y pasen los 2 dias;
        //        //          -3: Evaluación sin firma de Prev y M.A.; 
        //        int RespHE = HE.GetEstadoEvaluacion(Caratula.IdContrato, Convert.ToInt32(Math.Round(Caratula.PorcAvanceFinanAcom, 0)));


        //        //Ver logica de Notificaciones y alertas

        //        switch (RespHE)
        //        {
        //            case -1:
        //                return Content("No existe programación de evaluación por porcentaje de avance, o se encuentra desactivada");
        //            case -2:
        //                return Content("Roles Faltantes en el Contrato");
        //            case 2:
        //                //Se gatillan Notificaciones para evaluar; 
        //                break;
        //        }

        //        if (RespHE == 1 || Caratula.IdTipoEP == 2)
        //        {
        //            noti.EmailNotEP(4, Caratula);
        //        }

        //    }
        //    else
        //    {
        //        Caratula.EstadoEP = 2;
        //    }
        //    Caratula.MensajeAprobador = "Aprobado EP" + DateTime.Today.ToLongDateString();

        //    string UserId = User.Identity.GetUserId();
        //    int IdPersonal = db.SEG_UserPersonal.Where(x => x.UserId == UserId).FirstOrDefault().IdPersonal;
        //    ASP_aprobacionesEP aSP_AprobacionesEP = new ASP_aprobacionesEP()
        //    {
        //        IdAprobador = IdPersonal,
        //        IdCaratulaEP = Caratula.IdCaratulaEP,
        //        FechaAprobacion = DateTime.Now,
        //        Activo = true,
        //        EstadoEP = Caratula.EstadoEP,
        //        ResolucionAprueba = true
        //    };

        //    try
        //    {
        //        if (db.ASP_aprobacionesEP.Where(x => x.IdCaratulaEP == Caratula.IdCaratulaEP && x.Activo).Count() > 0)
        //        {
        //            ASP_aprobacionesEP ActivoAprob = db.ASP_aprobacionesEP.Where(x => x.IdCaratulaEP == Caratula.IdCaratulaEP && x.Activo).First();
        //            ActivoAprob.Activo = false;
        //            db.Entry(ActivoAprob).State = EntityState.Modified;
        //        }
        //        db.ASP_aprobacionesEP.Add(aSP_AprobacionesEP);
        //        db.Entry(Caratula).State = EntityState.Modified;
        //        db.SaveChanges();
        //        return Content("true;" + Caratula.IdContrato);
        //    }
        //    catch (Exception e)
        //    {
        //        return Content("Error");
        //    }
        //}

        //public ActionResult EditarNombreEP(int idCaratula)
        //{

        //    ASP_epCaratulaContrato Caratula = db.ASP_epCaratulaContrato.Where(x => x.IdCaratulaEP == idCaratula).First();
        //    //Parametros del Modal
        //    ViewBag.Titulo = "Editar Nombre EP N°" + Caratula.NumeroEP;
        //    //ViewBag.ObjetoEvaluable = Contrato;
        //    ViewBag.ClsModal = "hmodal-warning";
        //    ViewBag.Color = "warning";
        //    ViewBag.Action = "EditarNombreEP";
        //    return PartialView("_modalFormEditarNombreEP", Caratula);
        //}




        //[HttpPost]
        //public ActionResult EditarNombreEP(ASP_epCaratulaContrato car)
        //{

        //    ASP_epCaratulaContrato Caratula = db.ASP_epCaratulaContrato.Where(x => x.IdCaratulaEP == car.IdCaratulaEP).First();

        //    try
        //    {
        //        Caratula.RefEP = car.RefEP;
        //        db.Entry(Caratula).State = EntityState.Modified;
        //        db.SaveChanges();
        //        return Content("true;" + Caratula.IdContrato);
        //    }
        //    catch(Exception e)
        //    {
        //        return Content("Error");
        //    }
        //}


        //public ActionResult MultaEP(int idCaratula)
        //{

        //    ASP_epCaratulaContrato Caratula = db.ASP_epCaratulaContrato.Where(x => x.IdCaratulaEP == idCaratula).First();
        //    ASP_contratos Contrato = db.ASP_contratos.Find(Caratula.IdContrato);
        //    ViewBag.ObjetoEvaluable = Contrato;
        //    //Parametros del Modal
        //    ViewBag.Titulo = "Multar EP N°" + Caratula.NumeroEP;
        //    //ViewBag.ObjetoEvaluable = Contrato;
        //    ViewBag.ClsModal = "hmodal-danger";
        //    ViewBag.Color = "danger";
        //    ViewBag.Action = "MultaEP";
        //    return PartialView("_modalFormMulta", Caratula);
        //}

        //[HttpPost]
        //public ActionResult MultaEP(ASP_epCaratulaContrato car)
        //{

        //    ASP_epCaratulaContrato Caratula = db.ASP_epCaratulaContrato.Where(x => x.IdCaratulaEP == car.IdCaratulaEP).First();

        //    try
        //    {
        //        Caratula.Multa = car.Multa;
        //        Caratula.ValorMulta = car.ValorMulta;
        //        db.Entry(Caratula).State = EntityState.Modified;
        //        db.SaveChanges();
        //        return Content("true;" + Caratula.IdContrato);
        //    }
        //    catch (Exception e)
        //    {
        //        return Content("Error");
        //    }
        //}


        //public ActionResult RechazarEP(int idCaratula)
        //{

        //    ASP_epCaratulaContrato Caratula = db.ASP_epCaratulaContrato.Where(x => x.IdCaratulaEP == idCaratula).First();
        //    ASP_contratos Contrato = db.ASP_contratos.Find(Caratula.IdContrato);
        //    Caratula.MensajeAprobador = "";
        //    ViewBag.ObjetoEvaluable = Contrato;
        //    //Parametros del Modal
        //    ViewBag.Titulo = "Rechazo EP N°"+Caratula.NumeroEP;
        //    //ViewBag.ObjetoEvaluable = Contrato;
        //    ViewBag.ClsModal = "hmodal-danger";
        //    ViewBag.Color = "danger";
        //    ViewBag.Action = "RechazarEP";
        //    return PartialView("_modalFormRechazarEP", Caratula);
        //}

        //[HttpPost]
        //public ActionResult RechazarEP(ASP_epCaratulaContrato car)
        //{

        //    ASP_epCaratulaContrato Caratula = db.ASP_epCaratulaContrato.Where(x => x.IdCaratulaEP == car.IdCaratulaEP).First();
        //    Caratula.EstadoEP = 5;
        //    Caratula.MensajeAprobador = car.MensajeAprobador;

        //    string UserId = User.Identity.GetUserId();
        //    int IdPersonal = db.SEG_UserPersonal.Where(x => x.UserId == UserId).FirstOrDefault().IdPersonal;
        //    ASP_aprobacionesEP aSP_AprobacionesEP = new ASP_aprobacionesEP()
        //    {
        //        IdAprobador = IdPersonal,
        //        IdCaratulaEP = Caratula.IdCaratulaEP,
        //        FechaAprobacion = DateTime.Now,
        //        Activo = true,
        //        EstadoEP = Caratula.EstadoEP,
        //        ResolucionAprueba = false,
        //        Mensaje= car.MensajeAprobador
        //};

        //    try
        //    {
        //        if (db.ASP_aprobacionesEP.Where(x => x.IdCaratulaEP == Caratula.IdCaratulaEP && x.Activo).Count() > 0)
        //        {
        //            ASP_aprobacionesEP ActivoAprob = db.ASP_aprobacionesEP.Where(x => x.IdCaratulaEP == Caratula.IdCaratulaEP && x.Activo).First();
        //            ActivoAprob.Activo = false;
        //            db.Entry(ActivoAprob).State = EntityState.Modified;
        //        }
        //        db.ASP_aprobacionesEP.Add(aSP_AprobacionesEP);
        //        db.Entry(Caratula).State = EntityState.Modified;
        //        db.SaveChanges();

        //        //NOTIFICACION********************************************
        //        NotificacionEP noti = new NotificacionEP();
        //        noti.EmailNotEP(Caratula.EstadoEP, Caratula);
        //        //********************************************************

        //        return Content("true;" + Caratula.IdContrato);
        //    }
        //    catch
        //    {
        //        return Content("Error");
        //    }
        //}
        ////**********************************************************************

        ////**********************************************************************


        //public static List<ComboBoxEstandar2> ListModeloContrato()
        //{
        //    List<ComboBoxEstandar2> ListModeloContrato = new List<ComboBoxEstandar2>();
        //    ListModeloContrato.Add(new ComboBoxEstandar2 { Id = 1, Value = "Suma Alzada" });
        //    //ListModeloContrato.Add(new ComboBoxEstandar2 { Id = 2, Value = "Precio Unitario" });
        //    return (ListModeloContrato);
        //}

        //public static List<ComboBoxEstandar2> ListManejoEP()
        //{
        //    List<ComboBoxEstandar2> ListManejoEP = new List<ComboBoxEstandar2>();
        //    ListManejoEP.Add(new ComboBoxEstandar2 { Id = 1, Value = "Porcentual" });
        //    ListManejoEP.Add(new ComboBoxEstandar2 { Id = 2, Value = "Por Cantidades" });
        //    return (ListManejoEP);
        //}

        //[HttpPost]
        //public ActionResult ItemizadoCSV(HttpPostedFileBase csvFile, int IdContrato)
        //{
        //    string timeSpan = DateTime.Now.Ticks.ToString();
        //    string fileName = "Itemizado_" + IdContrato + "_" + timeSpan + ".csv";
        //    if (csvFile == null || csvFile.ContentLength == 0)
        //    {
        //        return Content(JsonConvert.SerializeObject(new { error = true, msg = "El Archivo es obligatorio" }));
        //    }
        //    File_Result save_file = ACA_Archivos.saveFile(csvFile, Server.MapPath("~/Files/Contratos/Uploads/"), fileName, false);

        //    //Aca se lee
        //    string lineas = string.Empty;//Documento
        //    //string[] strArray;
        //    StreamReader sr = new StreamReader(Path.Combine(Server.MapPath("~/Files/Contratos/Uploads/"), fileName),System.Text.Encoding.Default);
        //    lineas = sr.ReadLine();

        //    int linea = 0;
        //    //status.sendStatus("Procesando Archivo CSV cargado... ", "text-info", user);
        //    List<string> registros = new List<string>();
        //    //Lee EL ARCHIVO POR LINEAS
        //    while ((lineas = sr.ReadLine()) != null)
        //    {
        //        registros.Add(lineas);
        //    }
        //    sr.Dispose();
        //    //status.updateStatus(0, true, user);

        //    try
        //    {
        //        List<ASP_itemizadoContrato> ListaItem = new List<ASP_itemizadoContrato>();
        //        foreach (string l in registros)
        //        {
        //            linea++;
        //            //int percent = (linea * 100) / registros.Count;
        //            //status.updateStatus(percent, true, user);

        //            ASP_itemizadoContrato nuevo_item = new ASP_itemizadoContrato();
        //            nuevo_item.EsTitulo = false;
        //            nuevo_item.IdContrato = IdContrato;
        //            nuevo_item.Descontado = false;
        //            nuevo_item.PorcGastosGeneralesPartida = 0;
        //            nuevo_item.PorcUtilidadesPartida = 0;
        //            nuevo_item.CantAvance = 0;


        //            string[] campos = l.Split(';');

        //            //VALIDAMOS QUE EL NUMERO NO VENGA VACIO
        //            if (campos[0].Trim() != string.Empty)
        //            {
        //                nuevo_item.Item = campos[0].Trim();
        //            }
        //            else
        //            {
        //                //status.Error++;
        //                return Content("Error; La fila n°" +linea.ToString() +" No contiene número de ítem");
        //               // break;
        //            };

        //            //VALIDAMOS QUE EL NUMERO NO VENGA VACIO
        //            if (campos[1].Trim() != string.Empty)
        //            {
        //                nuevo_item.Descripcion = campos[1].Trim();
        //            }
        //            else
        //            {
        //                return Content("Error;El item :" + nuevo_item.Item.ToString() + "Debe tener descripción");
        //            };

        //            if (campos[3].Trim() == string.Empty)
        //            {
        //                nuevo_item.EsTitulo = true;
        //            }

        //            if (!nuevo_item.EsTitulo)
        //            {
        //                if (campos[2].Trim() != string.Empty)
        //                {
        //                    nuevo_item.Unidad = campos[2].Trim();
        //                }
        //                else
        //                {
        //                    return Content("Error;El item :" + nuevo_item.Item.ToString() + "Debe tener unidad de medida");
        //                };
        //            }
        //            else
        //            {
        //                if (campos[2].Trim() != string.Empty)
        //                {
        //                    return Content("Error;El item :" + nuevo_item.Item.ToString() + "Corresponde a un título por lo cual no corresponde Unidad de Medida: '" + campos[2].Trim() + "', es posible que en la descripción del item haya incluido 'punto y coma'");
        //                };
        //            }



        //            if (campos[3].Trim() != string.Empty)
        //            {

        //                //VALIDAMOS QUE EL DESTINATARIO SEA NUMERICO
        //                decimal n;
        //                bool isDecimal = decimal.TryParse(campos[3].Trim(), out n);
        //                if (!isDecimal)
        //                {
        //                    return Content("Error;El item :" + nuevo_item.Item.ToString() + "Presenta Error en la cantidad: '" + campos[3].Trim() + "', es posible que en la descripción del item haya incluido 'punto y coma'");

        //                }
        //                if (n == 0)
        //                {
        //                    return Content("Error;El item :" + nuevo_item.Item.ToString() + "la cantidad: '" + campos[3].Trim() + "', debe ser menor o mayor a 0 (cero)");
        //                }
        //                //SE AGREGA A LA LISTA DE SMS
        //                nuevo_item.Cantidad = Math.Round(Convert.ToDecimal(campos[3].Trim()), 6);
        //            }

        //            if (campos[4].Trim() != string.Empty)
        //            {

        //                //VALIDAMOS QUE EL DESTINATARIO SEA NUMERICO
        //                decimal n;
        //                bool isDecimal = decimal.TryParse(campos[4].Trim(), out n);
        //                if (!isDecimal)
        //                {
        //                    return Content("Error;El item :" + nuevo_item.Item.ToString() + "Presenta Error en la Precio Unitario: '" + campos[4].Trim() + "'");
        //                }
        //                if (n <= 0)
        //                {
        //                    return Content("Error;El item :" + nuevo_item.Item.ToString() + "el precio unitario: '" + campos[4].Trim() + "', debe ser mayor a 0 (cero)");
        //                }
        //                //SE AGREGA A LA LISTA DE SMS
        //                nuevo_item.PrecioUnitario = Math.Round(Convert.ToDecimal(campos[4].Trim()), 2);
        //            }

        //            if (campos[5].Trim() != string.Empty)
        //            {
        //                nuevo_item.ExentoIva = false;
        //                int exento = Convert.ToInt32(campos[5].Trim());
        //                if (exento==1)
        //                {
        //                    nuevo_item.ExentoIva = true;

        //                }

        //            }
        //            else { nuevo_item.ExentoIva = false; }


        //            ListaItem.Add(nuevo_item); 
        //        }
        //        //Sí ya Hay Itemizado Eliminamos el Existente*************************************************************
        //        db.ASP_itemizadoContrato.RemoveRange(db.ASP_itemizadoContrato.Where(x => x.IdContrato == IdContrato));
        //        db.SaveChanges();
        //        //********************************************************************************************************
        //        //GUARDAMOS Los Iemes
        //        db.ASP_itemizadoContrato.AddRange(ListaItem);
        //        db.SaveChanges();
        //        //Calculamos Valor del Contrato Neto
        //        decimal CostoDirecto = db.ASP_itemizadoContrato.
        //            Where(x => x.IdContrato == IdContrato && x.EsTitulo == false).
        //            Sum(x => x.Cantidad * x.PrecioUnitario);

        //        if (db.ASP_contratos.Find(IdContrato).IdMoneda == 1)
        //        {
        //            CostoDirecto = Math.Round(CostoDirecto, 0);
        //        }

        //        //Calculamos Valor del Contrato Neto
        //        decimal ValorProforma = 0;
        //        if (db.ASP_itemizadoContrato.
        //            Where(x => x.IdContrato == IdContrato && x.EsTitulo == false && x.ExentoIva == true).Count() > 0)
        //        {
        //             ValorProforma = db.ASP_itemizadoContrato.
        //            Where(x => x.IdContrato == IdContrato && x.EsTitulo == false && x.ExentoIva==true).
        //            Sum(x => x.Cantidad * x.PrecioUnitario);

        //        }

        //        return Content("ok;"+CostoDirecto.ToString()+";"+ ValorProforma.ToString());
        //    }
        //    catch(Exception e)
        //    {
        //        string d = e.Message;
        //        return Content("Error;"+d);
        //    }
        //}


        //public async Task<ActionResult> Cerrar(int id)
        //{
        //    var contratos = await db.ASP_contratos.FindAsync(id);
        //    if (contratos == null)
        //        return HttpNotFound();

        //    int itemizadoCount = db.ASP_itemizadoContrato.Where(c => c.IdContrato == id).Count();
        //    var epPendientes = db.ASP_epCaratulaContrato.Where(c => c.IdTipoEP == 2 && c.EstadoEP == 10 && c.IdContrato == id).FirstOrDefault();
        //    var ldoPendientes = db.ASP_LibroObras.Where(l => l.IdEstado == 1 && l.IdContrato == id).ToList();
        //    var evaPendientes = db.EVA_EvaluacionContrato.Where(e => e.EVA_evaluaciones.EstadosEval == 0 && e.IdContrato == id && e.EVA_evaluaciones.IdPeridiocidad == 11).ToList();
        //    ViewBag.IsAnticipo = db.ASP_epCaratulaContrato.Where(d => d.IdTipoEP == 0 && d.IdContrato == id).Count();

        //    ViewBag.ItemizadoCount = itemizadoCount;
        //    ViewData["epPendientes"] = epPendientes;
        //    ViewData["ldoPendientes"] = ldoPendientes;
        //    ViewData["evaPendientes"] = evaPendientes;

        //    return PartialView("_CerrarContrato", contratos);
        //}
        //[HttpPost, ActionName("Cerrar")]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> CerrarConfirmed(int id)
        //{
        //    try
        //    {
        //        var contratos = await db.ASP_contratos.FindAsync(id);
        //        if (contratos == null)
        //            return HttpNotFound();

        //        contratos.IdEstado = 11;
        //        db.Entry(contratos).State = EntityState.Modified;
        //        await db.SaveChangesAsync();
        //        return Content("true");
        //    }
        //    catch (Exception err)
        //    {
        //        return Content(err.Message);
        //    }
        //}

        //public async Task<ActionResult> HistorialEP(int idCaratula)
        //{
        //    List<ASP_aprobacionesEP> historico = await db.ASP_aprobacionesEP.Where(h => h.IdCaratulaEP == idCaratula).ToListAsync();
        //    foreach (var h in historico)
        //    {
        //        if (db.ASP_epCaratulaContrato.Find(idCaratula).IdTipoEP == 1)
        //        {
        //            if (h.EstadoEP == 1)
        //            {
        //                h.NombreAprobador = db.MAE_Contactos.Find(h.IdAprobador).Nombre;
        //            }
        //            else
        //            {
        //                h.NombreAprobador = db.MAE_personal.Find(h.IdAprobador).NombreCompleto;
        //            }
        //        }
        //        else
        //        {
        //            if (h.EstadoEP == 2)
        //            {
        //                h.NombreAprobador = db.MAE_Contactos.Find(h.IdAprobador).Nombre;

        //            }
        //            else
        //            {
        //                h.NombreAprobador = db.MAE_personal.Find(h.IdAprobador).NombreCompleto;
        //            }
        //        }

        //    }
        //    int IdContrato = db.ASP_epCaratulaContrato.Find(idCaratula).IdContrato;
        //    var contrato = db.ASP_contratos.Where(x => x.IdContrato == IdContrato).FirstOrDefault();
        //    if (db.EVA_FormulariosObjetos.Where(f => f.IdPeridiocidad == 11 && f.IdEmpresa == contrato.IdEmpresa 
        //    && f.Activo && (f.TipoObjeto == contrato.IdTipoContrato || f.TipoObjeto == 0)).Count()>0)
        //    {
        //        var PorcProgramacion = db.EVA_FormulariosObjetos.Where(f => f.IdPeridiocidad == 11 && f.IdEmpresa == contrato.IdEmpresa && f.Activo && (f.TipoObjeto == contrato.IdTipoContrato || f.TipoObjeto == 0)).FirstOrDefault().AvanceEP;

        //        if (db.ASP_epCaratulaContrato.Where(x => x.IdCaratulaEP == idCaratula && x.IdTipoEP == 1 && x.PorcAvanceFinanAcom >= PorcProgramacion).Count() > 0)
        //        {
        //            var eva = db.EVA_EvaluacionContrato.Where(e => e.IdContrato == IdContrato && e.EVA_evaluaciones.IdPeridiocidad == 11).FirstOrDefault();
        //            if (eva != null)
        //            {
        //                ASP_aprobacionesEP evaCreada = new ASP_aprobacionesEP();
        //                evaCreada.FechaAprobacion = eva.EVA_evaluaciones.FechaProgram;
        //                evaCreada.EstadoEP = 90;
        //                evaCreada.NombreAprobador = eva.EVA_evaluaciones.MAE_RespContrato.NombreCompleto;
        //                historico.Add(evaCreada);

        //                if (eva.EVA_evaluaciones.FechaEval != null)
        //                {
        //                    ASP_aprobacionesEP evaEvaluada = new ASP_aprobacionesEP();
        //                    evaEvaluada.FechaAprobacion = eva.EVA_evaluaciones.FechaEval.Value;
        //                    evaEvaluada.EstadoEP = 91;
        //                    evaEvaluada.NombreAprobador = eva.EVA_evaluaciones.MAE_RespContrato.NombreCompleto;
        //                    historico.Add(evaEvaluada);
        //                }

        //                if (eva.EVA_evaluaciones.FH_RespPrev != null)
        //                {
        //                    ASP_aprobacionesEP evaPrev = new ASP_aprobacionesEP();
        //                    evaPrev.FechaAprobacion = eva.EVA_evaluaciones.FH_RespPrev.Value;
        //                    evaPrev.EstadoEP = 92;
        //                    evaPrev.NombreAprobador = eva.EVA_evaluaciones.MAE_RespPrev.NombreCompleto;
        //                    if (eva.EVA_evaluaciones.ObsPrev != "" && eva.EVA_evaluaciones.ObsPrev != null)
        //                    {
        //                        if (eva.EVA_evaluaciones.ObsPrev.Length > 30)
        //                        {
        //                            evaPrev.Mensaje = eva.EVA_evaluaciones.ObsPrev.Substring(0, 30) + "...";
        //                        }
        //                        else
        //                        {
        //                            evaPrev.Mensaje = eva.EVA_evaluaciones.ObsPrev;
        //                        }
        //                    }
        //                    historico.Add(evaPrev);
        //                }
        //                if (eva.EVA_evaluaciones.FH_RespMed != null)
        //                {
        //                    ASP_aprobacionesEP evaMedio = new ASP_aprobacionesEP();
        //                    evaMedio.FechaAprobacion = eva.EVA_evaluaciones.FH_RespMed.Value;
        //                    evaMedio.EstadoEP = 93;
        //                    evaMedio.NombreAprobador = eva.EVA_evaluaciones.MAE_RespMed.NombreCompleto;
        //                    if (eva.EVA_evaluaciones.ObsMed != "" && eva.EVA_evaluaciones.ObsMed != null)
        //                    {
        //                        if (eva.EVA_evaluaciones.ObsMed.Length > 30)
        //                        {
        //                            evaMedio.Mensaje = eva.EVA_evaluaciones.ObsMed.Substring(0, 30) + "...";
        //                        }
        //                        else
        //                        {
        //                            evaMedio.Mensaje = eva.EVA_evaluaciones.ObsMed;
        //                        }
        //                    }
        //                    historico.Add(evaMedio);
        //                }
        //            }
        //        }
        //    }




        //    return PartialView("_GetHistorialEP", historico);
        //}


        //public ActionResult Logs(int? id)
        //{
        //    List<ASP_log> logs = db.ASP_log.
        //        Where(l => l.Objeto == "Contrato" && l.IdObjeto == id).OrderByDescending(l => l.FechaLog).ToList();
        //    if (logs.Count > 0)
        //    {
        //        foreach (ASP_log Log in logs)
        //        {
        //            Log.NombUser = db.Users.Find(Log.UserId).Nombre;
        //        }
        //    }
        //    return PartialView("_getTableLogs", logs);
        //}


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
