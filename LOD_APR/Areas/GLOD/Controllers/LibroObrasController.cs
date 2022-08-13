using ACAMicroFramework.Archivos;
using ACAMicroFramework.SqlDB;
using LinqKit;
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
using System.Configuration;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace LOD_APR.Areas.GLOD.Controllers
{
    [CustomAuthorize]
    public class LibroObrasController : Controller
    {
        private LOD_DB db = new LOD_DB();
        private GoogleReCaptchaHelper reCaptchaHelper = new GoogleReCaptchaHelper();
        public async Task<JsonResult> GetLibrosContrato(int id)
        {
            List<SelectListItem> tipo = new List<SelectListItem>();
            string idUser = User.Identity.GetUserId();
            List<int> libros = db.LOD_UsuariosLod.Where(x => x.Activo && x.UserId == idUser).Select(x => x.IdLod).Distinct().ToList();
            List<LOD_LibroObras> librosContrato = await db.LOD_LibroObras.Where(x => x.IdContrato == id && x.Estado == 1 && libros.Contains(x.IdLod)).ToListAsync();

            foreach (var item in librosContrato)
                tipo.Add(new SelectListItem() { Text = item.NombreLibroObra, Value = item.IdLod.ToString() });

            return Json(tipo, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> GetAnotaciones(int id)
        {
            List<SelectListItem> subtipo = new List<SelectListItem>();
            List<LOD_Anotaciones> anotacionesLibro = await db.LOD_Anotaciones.Where(x => x.IdLod == id && x.EstadoFirma).ToListAsync();
            foreach (var item in anotacionesLibro)
                subtipo.Add(new SelectListItem() { Text = item.Correlativo.ToString().PadLeft(6, '0') + "; " + item.Titulo, Value = item.IdAnotacion.ToString() });

            return Json(subtipo, JsonRequestBehavior.AllowGet);
        }



        public ActionResult Index(int id)
        {
            string IdUsuario = User.Identity.GetUserId();
            int IdUsLod = db.LOD_UsuariosLod.Where(x => x.IdLod == id && x.UserId == IdUsuario && x.Activo).FirstOrDefault().IdUsLod;
            ViewBag.TituloBandeja = "Bandeja Principal";

            LOD_LibroObras LibroObras = db.LOD_LibroObras.Find(id);

            clsSession se = new clsSession();
            LOD_FiltroAnotaciones filtroSesion = (LOD_FiltroAnotaciones)se.get("LOD_FiltroAnotaciones");

            if (filtroSesion != null)
            {
                ViewBag.FiltroEvaluacion = true;
                LibroObras.LstAnotaciones = GetFiltro(filtroSesion);
            }
            else
            {
                ViewBag.FiltroEvaluacion = false;
                LibroObras.LstAnotaciones = db.LOD_Anotaciones.Where(a => a.IdLod == id && a.Estado != 1).OrderByDescending(a => a.FechaPub).ToList();
            }

            LibroObras.LstAnotaciones = ProcesarAnotaciones(LibroObras.LstAnotaciones);
            LibroObras.LstLeidas = db.LOD_UserAnotacion.Where(a => a.Leido == true && a.IdUsLod == IdUsLod).Select(i => i.IdAnotacion).ToList();
            LibroObras.LstDestacadas = db.LOD_UserAnotacion.Where(a => a.Destacado == true && a.IdUsLod == IdUsLod).Select(i => i.IdAnotacion).ToList();

            LibroObras.DataPanelLO = new Auxiliares.DataPanelLO();
            LibroObras.DataPanelLO.numTodos = LibroObras.LstAnotaciones.Where(a => a.IdLod == id && a.Estado != 1).Count();
            LibroObras.DataPanelLO.numBorr = db.LOD_Anotaciones.Where(a => a.IdLod == id && a.UserId == IdUsuario && a.Estado == 1).Count();
            LibroObras.DataPanelLO.numMias = LibroObras.LstAnotaciones.Where(a => a.IdLod == id && a.UserId == IdUsuario && a.Estado != 1).Count();
            LibroObras.DataPanelLO.numDest = db.LOD_UserAnotacion.AsNoTracking().Where(a => a.IdUsLod == IdUsLod && a.Destacado == true && a.LOD_Anotaciones.Estado != 1).Count();
            LibroObras.DataPanelLO.numNombEn = db.LOD_UserAnotacion.AsNoTracking().Where(a => a.IdUsLod == IdUsLod && a.EsPrincipal == true && a.LOD_Anotaciones.Estado != 1).Count();
            LibroObras.DataPanelLO.numPend = db.LOD_UserAnotacion.AsNoTracking().Where(a => a.IdUsLod == IdUsLod && a.LOD_Anotaciones.Estado == 2 && a.EsRespRespuesta == true).Count();

            ViewBag.Cs = 0;

            List<SelectListItem> estados = new List<SelectListItem>();
            estados.Add(new SelectListItem() { Text = "Pendientes de Respuesta", Value = "2" });
            estados.Add(new SelectListItem() { Text = "Pendiente de Cierre", Value = "3" });
            estados.Add(new SelectListItem() { Text = "Cerradas", Value = "4" });

            ViewBag.IdEstado = new SelectList(estados.ToList(), "Value", "Text");
            List<string> usuariosLibro = db.LOD_Anotaciones.Where(w => w.IdLod == id).Select(u => u.UserId).Distinct().ToList();

            ViewBag.UserId = new SelectList(db.Users.Where(u => usuariosLibro.Contains(u.Id)).ToList(), "Id", "Nombre");

            if (ValidaPermisos.ValidaPermisosEnController("0020100000"))
            {
                return View(LibroObras);
            }
            else
            {
                return RedirectToAction("SinPermiso", "../Pub");
            }

            
        }


        public ActionResult Filtro(LOD_FiltroAnotaciones filtro)
        {
            string IdUsuario = User.Identity.GetUserId();
            int IdUsLod = db.LOD_UsuariosLod.Where(x => x.IdLod == filtro.IdLibro && x.UserId == IdUsuario && x.Activo).FirstOrDefault().IdUsLod;
            clsSession se = new clsSession();
            se.set("ASP_FiltroAnotaciones", filtro);
            ViewBag.FiltroEvaluacion = true;
            ViewBag.TituloBandeja = "Filtro Avanzado";

            LOD_LibroObras libro = db.LOD_LibroObras.Find(filtro.IdLibro);
            libro.LstAnotaciones = GetFiltro(filtro);

            if (libro.LstAnotaciones != null)
                libro.LstAnotaciones = ProcesarAnotaciones(libro.LstAnotaciones);

            libro.LstLeidas = db.LOD_UserAnotacion.Where(a => a.Leido == true && a.IdUsLod == IdUsLod).Select(i => i.IdAnotacion).ToList();
            libro.LstDestacadas = db.LOD_UserAnotacion.Where(a => a.Destacado == true && a.IdUsLod == IdUsLod).Select(i => i.IdAnotacion).ToList();

            return PartialView("_GetFiltroRapido", libro);
        }
        public ActionResult RemoveFiltro(int IdLibro)
        {
            clsSession se = new clsSession();
            se.del("ASP_FiltroAnotaciones");
            ViewBag.FiltroEvaluacion = false;
            string IdUsuario = User.Identity.GetUserId();
            int IdUsLod = db.LOD_UsuariosLod.Where(x => x.IdLod == IdLibro && x.UserId == IdUsuario && x.Activo).FirstOrDefault().IdUsLod;
            ViewBag.TituloBandeja = "Bandeja Principal";
            LOD_LibroObras LibroObras = db.LOD_LibroObras.Find(IdLibro);
            LibroObras.LstAnotaciones = db.LOD_Anotaciones.Where(a => a.IdLod == IdLibro && a.Estado != 1).OrderByDescending(a => a.FechaPub).ToList();

            if (LibroObras.LstAnotaciones != null)
                LibroObras.LstAnotaciones = ProcesarAnotaciones(LibroObras.LstAnotaciones);

            LibroObras.LstLeidas = db.LOD_UserAnotacion.Where(a => a.Leido == true && a.IdUsLod == IdUsLod).Select(i => i.IdAnotacion).ToList();
            LibroObras.LstDestacadas = db.LOD_UserAnotacion.Where(a => a.Destacado == true && a.IdUsLod == IdUsLod).Select(i => i.IdAnotacion).ToList();

            ViewBag.Cs = 0;

            return PartialView("_GetFiltroRapido", LibroObras);
        }
        private List<LOD_Anotaciones> GetFiltro(LOD_FiltroAnotaciones filtro)
        {
            string IdUsuario = User.Identity.GetUserId();

            var predicate = PredicateBuilder.New<LOD_Anotaciones>();

            if (filtro.IdEstado != null && filtro.IdEstado.Length > 0)
                predicate = predicate.And(x => filtro.IdEstado.Contains(x.Estado));

            if (filtro.UserId != null && filtro.UserId.Length > 0)
                predicate = predicate.And(x => filtro.UserId.Contains(x.UserId));



            if (!string.IsNullOrEmpty(filtro.FDesde))
                predicate = predicate.And(x => x.FechaPub >= filtro.FDesdeDate);

            if (!string.IsNullOrEmpty(filtro.FHasta))
            {
                DateTime FechaHasta = filtro.FHastaDate.Value.Add(new TimeSpan(23, 59, 59));
                predicate = predicate.And(x => x.FechaPub <= FechaHasta);
            }

            predicate = predicate.And(l => l.IdLod == filtro.IdLibro).And(l => l.Correlativo != null);

            IQueryable<LOD_Anotaciones> IQ_Solicituds = db.LOD_Anotaciones.AsExpandable().Where(predicate);

            var lista = IQ_Solicituds.ToList().OrderByDescending(x => x.Correlativo).ToList();

            if (!string.IsNullOrEmpty(filtro.searchCuerpo))
                lista = lista.Where(x => x.Cuerpo.Contains(filtro.searchCuerpo)).ToList().OrderByDescending(x => x.Correlativo).ToList();

            return lista;
        }

        public ActionResult LibroIndex()
        {
            string IdUsuario = User.Identity.GetUserId();

            List<LOD_LibroObras> libros = new List<LOD_LibroObras>();
            string con = ACA_SqlServer.generaConexion("LOD_DB");
            string roleId = ACA_SqlServer.ScalarQuery(con, "SELECT [RoleId] FROM [dbo].[SEG_UserRoles] where [UserId]='" + User.Identity.GetUserId() + "'").ToString();

            List<int> LstLib = db.LOD_UsuariosLod.Where(l => l.UserId == IdUsuario && l.Activo == true).Select(l => l.IdLod).ToList();

            libros = db.LOD_LibroObras.Where(x => x.Estado == 1 && LstLib.Contains(x.IdLod)).OrderBy(x => x.NombreLibroObra).ToList();

            //ViewBag.TipoVista = "IndexUsr";
            return View("IndexUsr", libros);
        }

        public ActionResult LibroIndexBuscar(string buscar, string TipoVista)
        {
            string IdUsuario = User.Identity.GetUserId();
            List<int> LstLib = db.LOD_UsuariosLod.Where(l => l.UserId == IdUsuario && l.Activo == true).Select(l => l.IdLod).ToList();
            List<LOD_LibroObras> Libros = new List<LOD_LibroObras>();
            Libros = db.LOD_LibroObras
            .Where(l => (((l.NombreLibroObra.ToUpper().Contains(buscar.ToUpper()))
            || (l.CON_Contratos.CodigoContrato.ToUpper().Contains(buscar.ToUpper()))
            && (l.Estado == 1 && LstLib.Contains(l.IdLod))))).ToList();

            return PartialView("_" + TipoVista, Libros);
        }

        public ActionResult LibroIndexRecargar(int? IdEmpresa, string TipoVista)
        {

            List<LOD_LibroObras> Libros = new List<LOD_LibroObras>();
            string IdUsuario = User.Identity.GetUserId();
            List<int> LstLib = db.LOD_UsuariosLod.Where(l => l.UserId == IdUsuario && l.Activo == true).Select(l => l.IdLod).ToList();
            Libros = db.LOD_LibroObras.Where(x => x.Estado == 1 && LstLib.Contains(x.IdLod)).ToList();
            return PartialView("_" + TipoVista, Libros);
        }


        public ActionResult ActivarLibroMOP(int IdLod)
        {

            if (db.LOD_UsuariosLod.Where(x => x.IdLod == IdLod && x.Activo).Count() > 0)
            {
                ViewBag.PermiteActivar = true;
            }
            else
            {
                ViewBag.PermiteActivar = false;
            }
            LOD_LibroObras libro = db.LOD_LibroObras.Find(IdLod);
            return PartialView("_modalFormActivarLibroMOP", libro);

        }

        public async Task<ActionResult> ActivarLibroFEA(int IdLod)
        {
            if (db.LOD_UsuariosLod.Where(x => x.IdLod == IdLod && x.Activo).Count() > 0)
            {
                ViewBag.PermiteActivar = true;
            }
            else
            {
                ViewBag.PermiteActivar = false;
            }

            Random rnd = new Random();
            int first = rnd.Next(0, 9);
            int second = rnd.Next(0, 99);
            string ambiente = ConfigurationManager.AppSettings.Get("Ambiente").ToString();

            string userid = User.Identity.GetUserId();
            LOD_LibroObras libro = db.LOD_LibroObras.Find(IdLod);
            libro.OTP = $"{ambiente}A{first}-{second.ToString().PadLeft(2, '0')}";
            libro.UsuarioApertura = userid;
            ViewBag.OTP = libro.OTP;
            db.Entry(libro).State = EntityState.Modified;
            await db.SaveChangesAsync();

            

            return PartialView("_modalFormActivarLibroFEA", libro);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ActivarLibro(LOD_LibroObras libro, string otp)
        {
            try
            {
                LOD_LibroObras libroobra = await db.LOD_LibroObras.FindAsync(libro.IdLod);
                FirmarAnotacion firm = new FirmarAnotacion();
                string response = string.Empty;
                string pdfBase64 = await firm.AnotacionPDFToBase64(Server.MapPath("~/Files/System/apertura-libros.pdf"));
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
                        otp = otp
                    };
                    ModelsViews.File pdf = new ModelsViews.File()
                    {
                        contentType = "application/pdf",
                        checksum = "",
                        content = pdfBase64,
                        description = "Apertura de Libro " + libroobra.NombreLibroObra + " del contrato " + libroobra.ContratoNombre
                    };
                    mop.files.Add(pdf);

                    MOP_Post_Response result = await Mop_API.API_postListSMS(mop);

                    if (String.IsNullOrEmpty(result.error))
                    {
                        libroobra.UsuarioApertura = User.Identity.GetUserId();
                        libroobra.FechaApertura = DateTime.Now;
                        libroobra.Estado = 1;
                        libroobra.TipoApertura = true;
                        db.Entry(libroobra).State = EntityState.Modified;
                        db.SaveChanges();
                        response = "true;" + libro.IdLod;

                        Log_Helper log_Helper = new Log_Helper();
                        string accion = "Se ha Activado el Libro: " + libro.NombreLibroObra;
                        bool res = await log_Helper.SetObjectLog(1, libro, accion, User.Identity.GetUserId());

                        return Content(response);


                    }
                    else
                    {
                        response = result.error;
                    }
                }
                else
                {
                    response = "Ocurrió un Problema al tratar de Convertir a Base 64 el archivo pdf de la Apertura de Libro.";
                }

                return Content(response);
            }
            catch (Exception e)
            {
                return Content(e.Message);
            }
        }

        public ActionResult getLibro(int id)
        {
            LOD_LibroObras libro = db.LOD_LibroObras.Find(id);
            ViewBag.IdLod = libro.IdLod;
            ViewBag.IdUserActual = User.Identity.GetUserId();

            ViewBag.EsInspectorFiscal = false;
            string userId = User.Identity.GetUserId();
            List<LOD_RolesCttosContrato> rolesUsuario = db.LOD_UsuariosLod.Where(x => x.UserId == userId && x.LOD_LibroObras.IdContrato == libro.IdContrato && x.Activo).Select(x => x.LOD_RolesCttosContrato).ToList();
            if (rolesUsuario.Select(x => x.IdRolCtto).Contains(2)) //2 es el rol Inspector Fiscal en la Tabla MAE_RolesContratos
                ViewBag.EsInspectorFiscal = true;

            return PartialView("_GetInfoLibro", libro);
        }

        public ActionResult Details(int? id)
        {
            LOD_LibroObras libro = db.LOD_LibroObras.Find(id);
            libro.Creador = db.Users.Find(libro.UserId).NombreCompleto;
            ViewBag.IdLod = libro.IdLod;
            ViewBag.IdUserActual = User.Identity.GetUserId();

            ViewBag.EsInspectorFiscal = false;
            string userId = User.Identity.GetUserId();
            List<LOD_RolesCttosContrato> rolesUsuario = db.LOD_UsuariosLod.Where(x => x.UserId == userId && x.LOD_LibroObras.IdContrato == libro.IdContrato && x.Activo).Select(x => x.LOD_RolesCttosContrato).ToList();
            if (rolesUsuario.Select(x => x.IdRolCtto).Contains(2)) //2 es el rol Inspector Fiscal en la Tabla MAE_RolesContratos
                ViewBag.EsInspectorFiscal = true;

            return PartialView("_getDetailsLibroObras", libro);

        }


        public ActionResult GetLibrosObras(int? id)
        {
            List<LOD_LibroObras> LObras = new List<LOD_LibroObras>();
            if (id != null)
            {
                LObras = db.LOD_LibroObras.Where(l => l.IdContrato == id).ToList();
            }
            else
            {
                LObras = db.LOD_LibroObras.ToList();
            }

            return PartialView("_getTableLObras", LObras);
        }

        public ActionResult Create(int Padre, int Tipo, int IdContrato) // Tipo: Complementarios = 3 y Comunicaciones = 2
        {
            if (db.LOD_LibroObras.Where(x => x.IdContrato == IdContrato && x.IdTipoLod == 1).FirstOrDefault().Estado == 0 || db.LOD_LibroObras.Where(x => x.IdContrato == IdContrato && x.IdTipoLod == 2).FirstOrDefault().Estado == 0)
            {
                ViewBag.PermiteCrear = false;
                ViewBag.Titulo = "Nuevo Libro";
                ViewBag.ClsModal = "hmodal-warning";
                ViewBag.Color = "warning";
                ViewBag.Action = "Create";
            }
            else
            {
                ViewBag.PermiteCrear = true;
                ViewBag.Titulo = "Nuevo Libro";
                ViewBag.ClsModal = "hmodal-success";
                ViewBag.Color = "success";
                ViewBag.Action = "Create";
            }
                
            //Parametros del Modal
            
            //Preparación de combos

            LOD_LibroObras libro = new LOD_LibroObras();
            libro.IdContrato = IdContrato;
            libro.IdCarpeta = Padre;
            if (Tipo == 2)
            {
                //Mandar ViewBag. con tipos de lod de com no creados
                List<int> IdsTipoLod = db.MAE_TipoLOD.Where(x => x.EsObligatorio == false && x.TipoLodJer == 2).Select(x => x.IdTipoLod).ToList();
                List<int> IdsTipoLodCreados = db.LOD_LibroObras.Where(x => x.IdContrato == IdContrato).Select(x => x.IdTipoLod).Distinct().ToList();
                IdsTipoLodCreados = IdsTipoLod.Except(IdsTipoLodCreados).ToList();
                List<MAE_TipoLOD> tipolods = db.MAE_TipoLOD.Where(x => IdsTipoLodCreados.Contains(x.IdTipoLod)).ToList();
                //search.Add(new AutoSearch()
                //{
                //    id = "0",
                //    name = "<Seleccione Tipo Libro>"
                //});


                List<AutoSearch> search = new List<AutoSearch>();
                search.Add(new AutoSearch()
                {
                    id = "0",
                    name = "<Seleccione Tipo de Libro>"
                });
                foreach (var item in tipolods)
                {
                    search.Add(new AutoSearch()
                    {
                        id = item.IdTipoLod.ToString(),
                        name = item.Nombre
                    });
                }
                ViewBag.IdTipoLod = new SelectList(search, "id", "name", "0");
            }

            libro.tipo = Tipo;
            libro.HerImgPadre = false;
            return PartialView("_modalForm", libro);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(LOD_LibroObras libroObra)
            //[Bind(Include = "CodigoLObras,NomLibroObra,DescripcionLObra,FechaCreacion,IdEstado,fileImage,HerImgPadre,IdLibro")]
        {
            try
            {
                if (!reCaptchaHelper.IsCaptchaValid(Request.Form["foo"]))
                {
                    var responseRe = "Error en el captcha, Intente nuevamente.";
                    return Content(responseRe);
                }
                if (libroObra.tipo == 2 && libroObra.IdTipoLod == 0)
                {
                    return Content("Debe Seleccionar un Tipo de Libro");
                }
                else if(libroObra.tipo == 2)
                {
                    int totalTipos = db.LOD_LibroObras.Where(x => x.IdContrato == libroObra.IdContrato && x.MAE_TipoLOD.TipoLodJer == 2).Count();
                    libroObra.NombreLibroObra ="2."+(totalTipos +1).ToString()+"-"+ db.MAE_TipoLOD.Where(x => x.IdTipoLod == libroObra.IdTipoLod).FirstOrDefault().Nombre;
                }
                else
                {
                    int totalTipos = db.LOD_LibroObras.Where(x => x.IdContrato == libroObra.IdContrato && x.MAE_TipoLOD.TipoLodJer == 3).Count();
                    libroObra.NombreLibroObra = "3." + (totalTipos + 1).ToString() + "-" + libroObra.NombreLibroObra;
                    libroObra.IdTipoLod = 12;
                }


                libroObra.UserId = User.Identity.GetUserId();
                if (libroObra.HerImgPadre == true)
                {
                    string RutaImg = db.CON_Contratos.Find(libroObra.IdContrato).RutaImagenContrato;
                    if (RutaImg != null)
                    {
                        libroObra.RutaImagenLObras = RutaImg;
                    }
                }
                else if (libroObra.fileImage != null)
                {
                    //Ruta imagen
                    string filePath = Server.MapPath("~/Images/LibroObras/");
                    string fileExt = Path.GetExtension(libroObra.fileImage.FileName);
                    string nombreImage = libroObra.IdContrato.ToString()+ "_LOD_" + libroObra.NombreLibroObra + fileExt;
                    //ACA_Archivos aca_archivos = new ACA_Archivos();
                    File_Result save_file = ACA_Archivos.saveFile(libroObra.fileImage, filePath, nombreImage, true);
                    libroObra.RutaImagenLObras = "~/Images/LibroObras/" + nombreImage;
                    //*************************************//
                }
                


                

                libroObra.FechaCreacion = DateTime.Now.Date;
                libroObra.Estado = 0;
                libroObra.UserId = User.Identity.GetUserId();
               
                db.LOD_LibroObras.Add(libroObra);
                db.SaveChanges();


                //LOD_UsuariosLod usuarioLibro = new LOD_UsuariosLod();
                //usuarioLibro.UserId = User.Identity.GetUserId();
                //usuarioLibro.IdLod = Convert.ToInt32(db.LOD_LibroObras.OrderByDescending(l => l.IdLod).Select(l => l.IdLod).First());
                //db.LOD_UsuariosLod.Add(usuarioLibro);
                bool response = FuncionRolesContrato(libroObra.IdLod);
                await db.SaveChangesAsync();

                Log_Helper log_Helper = new Log_Helper();
                string accion = "Se ha creado el Libro: " + libroObra.NombreLibroObra;
                bool res = await log_Helper.SetObjectLog(1, libroObra, accion, User.Identity.GetUserId());

                return Content("true;" + libroObra.IdLod);
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
        }

        public bool FuncionRolesContrato(int IdLibro)
        {
            try
            {
                LOD_LibroObras libro = db.LOD_LibroObras.Find(IdLibro);
                List<LOD_RolesCttosContrato> rolesCreados = db.LOD_RolesCttosContrato.Where(x => x.IdContrato == libro.IdContrato && x.IdRolCtto != null).ToList();
                if (libro.IdTipoLod != 12)
                {
                    bool response = FuncionRolesNuevasComunicaciones(libro.IdContrato, libro.IdLod);
                }

                //Revisamos Casos
                foreach (LOD_RolesCttosContrato RC in rolesCreados)
                        {
                            MAE_RolesContrato RolPC = db.MAE_RolesContrato.Find(RC.IdRolCtto);
                            //Creamos permisos del ROL
                            List<LOD_PermisosRolesContrato> Permisos = new List<LOD_PermisosRolesContrato>();
                            if(libro.IdTipoLod != 12) {

                                if (RolPC.Lectura3)
                                {
                                    LOD_PermisosRolesContrato Permiso = new LOD_PermisosRolesContrato()
                                    {
                                        IdRCContrato = RC.IdRCContrato,
                                        IdLod = libro.IdLod,
                                        Lectura = RolPC.Lectura3,
                                        Escritura = RolPC.Escritura3,
                                        FirmaGob = RolPC.FirmaGob3,
                                        FirmaFea = RolPC.FirmaFea3,
                                        FirmaSimple = RolPC.FirmaSimple3,
                                    };
                                    Permisos.Add(Permiso);

                                    bool activo = AsignarUsuarioALod(RC.IdRCContrato, libro.IdLod);
                                }
                            
                            }else{
                                if (RolPC.Lectura4)
                                {
                                    LOD_PermisosRolesContrato Permiso = new LOD_PermisosRolesContrato()
                                    {
                                        IdRCContrato = RC.IdRCContrato,
                                        IdLod = libro.IdLod,
                                        Lectura = RolPC.Lectura4,
                                        Escritura = RolPC.Escritura4,
                                        FirmaGob = RolPC.FirmaGob4,
                                        FirmaFea = RolPC.FirmaFea4,
                                        FirmaSimple = RolPC.FirmaSimple4,
                                    };
                                    Permisos.Add(Permiso);

                                    bool activo = AsignarUsuarioALod(RC.IdRCContrato, libro.IdLod);
                                }
                            
                            }

                            db.LOD_PermisosRolesContrato.AddRange(Permisos);
                            db.SaveChanges();
                        }


            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }

        public bool AsignarUsuarioALod( int IdRol, int IdLod)
        {
            List<string> usuarios = db.LOD_UsuariosLod.Where(x => x.IdRCContrato == IdRol).Select(x => x.UserId).Distinct().ToList();
            foreach (var item in usuarios)
            {
                int verdaderos = db.LOD_UsuariosLod.Where(x => x.IdRCContrato == IdRol && x.UserId == item && x.Activo).Count();
                bool activo = false;
                if (verdaderos > 0) activo = true;

                LOD_UsuariosLod user = new LOD_UsuariosLod()
                {
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

        public bool FuncionRolesNuevasComunicaciones(int IdContrato, int IdLibro)
        {
            try
            {
                int IdTipoLod = db.LOD_LibroObras.Find(IdLibro).IdTipoLod;
                MAE_RolesContrato maeRol = db.MAE_RolesContrato.Where(x => x.IdTipoLod == IdTipoLod).FirstOrDefault();
                //1.-Crear Rol en el contrato
                LOD_RolesCttosContrato RolCtto = new LOD_RolesCttosContrato()
                {
                    IdContrato = IdContrato,
                    IdRolCtto = maeRol.IdRolCtto,
                    NombreRol = maeRol.NombreRol,
                    Descripcion = maeRol.Descripcion
                };
                db.LOD_RolesCttosContrato.Add(RolCtto);
                db.SaveChanges();

                //Buscamos los tipos de libro
                List<int> IdsTipoLod = db.LOD_LibroObras.Where(x => x.IdContrato == IdContrato).Select(x => x.MAE_TipoLOD.IdTipoLod).ToList();

                        MAE_RolesContrato RolPC = db.LOD_RolesCttosContrato.Find(RolCtto.IdRCContrato).MAE_RolesContrato;
                        //Revisamos Casos
                       
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
                            //4.-En Funcion de los Libros Comunicaciones excepto idtipolod 2
                            List<int> IdsLodCom = db.MAE_TipoLOD.Where(x => x.TipoLodJer == 2 && x.IdTipoLod != 2).Select(x => x.IdTipoLod).ToList();
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

                        //5.- En funcion de los Libros complementarios
                        List<int> IdsLodComP = db.MAE_TipoLOD.Where(x => x.TipoLodJer == 3).Select(x => x.IdTipoLod).ToList();
                        foreach (int IdTLod in IdsLodComP)
                        {
                            if (RolPC.Lectura3)
                            {
                                LOD_PermisosRolesContrato Permiso = new LOD_PermisosRolesContrato()
                                {
                                    IdRCContrato = RolCtto.IdRCContrato,
                                    IdLod = db.LOD_LibroObras.Where(x => x.IdTipoLod == IdTLod && x.IdContrato == IdContrato).FirstOrDefault().IdLod,
                                    Lectura = RolPC.Lectura4,
                                    Escritura = RolPC.Escritura4,
                                    FirmaGob = RolPC.FirmaGob4,
                                    FirmaFea = RolPC.FirmaFea4,
                                    FirmaSimple = RolPC.FirmaSimple4,
                                };
                                Permisos.Add(Permiso);
                            }
                        }

                        db.LOD_PermisosRolesContrato.AddRange(Permisos);
                        db.SaveChanges();
                    
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }



        public ActionResult Edit(int id)
        {
            LOD_LibroObras LibroObras = db.LOD_LibroObras.Find(id);
            //Parametros del Modal
            ViewBag.Titulo = "Editar Libro de Obras";
            ViewBag.ClsModal = "hmodal-warning";
            ViewBag.Color = "warning";
            ViewBag.Action = "Edit";
            ViewBag.PermiteCrear = true;
            //Arbol Jerarquía
            // ViewBag.UserContratista = new SelectList(db.Users/*.Where(u => u.Empresa.EsContratista && u.Empresa.IdEmpresa == libroObra.Contrato.IdEmpresa).OrderBy(x => x.NomUsuario)*/, "Id", "Nombre");
            //ViewBag.UserMandante = new SelectList(db.Users/*Where(u => !u.Empresa.EsContratista && u.IdUsuario != IdUsuario)*/ , "Id", "Nombre"); //Usuarios que pertecen a la empresa ralacionada con el contrato
            return PartialView("_modalForm", LibroObras);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(LOD_LibroObras ModelActual)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (ModelActual.HerImgPadre == true)
                    {
                        string RutaImg = db.CON_Contratos.Find(ModelActual.IdContrato).RutaImagenContrato;
                        if (RutaImg != null)
                        {
                            ModelActual.RutaImagenLObras = RutaImg;
                        }
                    }
                    else if(ModelActual.RutaImagenLObras != null && ModelActual.fileImage != null)
                    {
                        HttpPostedFileBase ImagenActual = ModelActual.fileImage;
                        byte[] ImgAct = new byte[ModelActual.fileImage.ContentLength];
                        string ImgAnt64 = TransImgString(ModelActual.RutaImagenLObras);
                        string ImgAct64 = TransImgString(ModelActual.fileImage);
                        string nombreImage = ModelActual.IdContrato.ToString() + "_LOD_" + ModelActual.NombreLibroObra;
                        //fin Imagen 2

                        //Comparamos Imagenes
                        if ((ImgAnt64 != ImgAct64) && ImgAnt64 != string.Empty && ImgAct64 != string.Empty)
                        {
                            ModelActual.RutaImagenLObras = GuardarImagenEnCarpeta("~/Images/LibroObras/", ModelActual.fileImage, ModelActual.NombreLibroObra);
                        }
                    }
                    else
                    {
                        if (ModelActual.fileImage != null)
                        {
                            ModelActual.RutaImagenLObras = GuardarImagenEnCarpeta("~/Images/LibroObras/", ModelActual.fileImage, ModelActual.NombreLibroObra);
                        }
                    }

                    LOD_LibroObras editLibro = db.LOD_LibroObras.Find(ModelActual.IdLod);
                    editLibro.DescripcionLObra = ModelActual.DescripcionLObra;
                    editLibro.CodigoLObras = ModelActual.CodigoLObras;
                    editLibro.RutaImagenLObras = ModelActual.RutaImagenLObras;
                    editLibro.HerImgPadre = ModelActual.HerImgPadre;

                    db.Entry(editLibro).State = EntityState.Modified;
                    db.SaveChanges();

                }
                return Content("true;" + ModelActual.IdLod);
            }
            catch (Exception ex)
            {
                return Content("false;" + ex.Message);
            }
        }

        public ActionResult Delete(int id)
        {
            LOD_LibroObras lOD_LibroObras = db.LOD_LibroObras.Find(id);
            return PartialView("_Delete", lOD_LibroObras);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(LOD_LibroObras libro)
        {
            try
            {
                LOD_LibroObras libroDelete = await db.LOD_LibroObras.FindAsync(libro.IdLod);
                int IdContrato = libroDelete.IdContrato;
                //Se quitan los usuarios asociados a esta Bitacora
                List<LOD_PermisosRolesContrato> permisos = db.LOD_PermisosRolesContrato.Where(x => x.IdLod == libroDelete.IdLod).ToList();

                foreach (var item in permisos)
                {
                    db.LOD_PermisosRolesContrato.Remove(item);
                    db.SaveChanges();
                }

                List<LOD_UsuariosLod> usuarios = db.LOD_UsuariosLod.Where(x => x.IdLod == libroDelete.IdLod).ToList();
                foreach (var item in usuarios)
                {
                    db.LOD_UsuariosLod.Remove(item);
                    db.SaveChanges();
                }

                db.LOD_LibroObras.Remove(libroDelete);
                db.SaveChanges();

                await db.SaveChangesAsync();
                return Content("delete;" + IdContrato);
            }
            catch (Exception err)
            {
                return Content(err.Message);
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
        public LOD_log FunctionLog(DateTime FechaLog, string IdUsuario, int Objeto, int IdObjeto,
                                 string Accion, string Campo, string ValorAnterior, string ValorActualizado)
        {
            string StringObjeto = "";
            switch (Objeto)
            {
                case 1:
                    StringObjeto = "Proyecto";
                    break;
                case 2:
                    StringObjeto = "Obra";
                    break;
                case 3:
                    StringObjeto = "Contrato";
                    break;
                case 4:
                    StringObjeto = "LibroObras";
                    break;
            }

            try
            {
                LOD_log log = new LOD_log()
                {
                    FechaLog = FechaLog,
                    UserId = IdUsuario,
                    Objeto = StringObjeto,
                    IdObjeto = IdObjeto,
                    Accion = Accion,
                    Campo = Campo,
                    ValorAnterior = ValorAnterior,
                    ValorActualizado = ValorActualizado
                };
                //db.ASP_log.Add(log);
                //db.SaveChanges();
                return log;
            }
            catch (Exception ex)
            {
                return null;
            }

        }

        public string GuardarImagenEnCarpeta(string ruta, HttpPostedFileBase archivo, string nombreProy)
        {
            //Ruta imagen
            string RutaImg = string.Empty;
            string filePath = Server.MapPath(ruta);
            string fileExt = Path.GetExtension(archivo.FileName);
            string nombreImage = nombreProy + fileExt;
            //ACA_Archivos aca_archivos = new ACA_Archivos();
            File_Result save_file = ACA_Archivos.saveFile(archivo, filePath, nombreImage, true);
            RutaImg = "~/Images/LibroObras/" + nombreImage;
            //*************************************//
            return RutaImg;
        }


        public ActionResult GetFiltroRapido(int cs, int idlib)
        {

            string IdUsuario = User.Identity.GetUserId();
            int IdUsLod = db.LOD_UsuariosLod.Where(x => x.IdLod == idlib && x.UserId == IdUsuario && x.Activo).FirstOrDefault().IdUsLod;
            int UsLib = db.LOD_UsuariosLod.Where(x => x.UserId == IdUsuario && x.Activo == true && x.IdLod == idlib).Count();
            string con = ACA_SqlServer.generaConexion("LOD_DB");
            string roleId = ACA_SqlServer.ScalarQuery(con, "SELECT [RoleId] FROM [dbo].[SEG_UserRoles] where [UserId]='" + User.Identity.GetUserId() + "'").ToString();
            if (roleId == "183a055c-7f92-4537-a455-dad99a5c4987")
            {
                UsLib = 1;
            }
            if (UsLib == 0)
            {
                return RedirectToAction("LibroIndex", "LibroObras");
            }
            
            LOD_LibroObras Libro = new LOD_LibroObras();
            Libro.LstLeidas = db.LOD_UserAnotacion.Where(a => a.Leido == true && a.IdUsLod == IdUsLod).Select(i => i.IdAnotacion).ToList();
            Libro.LstDestacadas = db.LOD_UserAnotacion.Where(a => a.Destacado == true && a.IdUsLod == IdUsLod).Select(i => i.IdAnotacion).ToList();
            Libro.IdLod= idlib;
            ViewBag.Cs = cs;
            switch (cs)
            {
                case 0:
                    ViewBag.TituloBandeja = "Baneja Principal";
                    Libro.LstAnotaciones = db.LOD_Anotaciones.Include(a => a.LOD_LibroObras).Include(a => a.MAE_SubtipoComunicacion)/*.Include(a => a.Usuario)*/.Where(a => a.IdLod == idlib && a.Estado != 1).OrderByDescending(a => a.FechaPub).ToList();
                    break;
                case 1:
                    ViewBag.TituloBandeja = "Mis Publicaciones";
                    Libro.LstAnotaciones = db.LOD_Anotaciones.Include(a => a.LOD_LibroObras).Include(a => a.MAE_SubtipoComunicacion)/*.Include(a => a.Usuario)*/.Where(a => a.IdLod == idlib && a.Estado != 1 && a.UserId == IdUsuario).OrderByDescending(a => a.FechaPub).ToList();
                    break;
                case 2:
                    ViewBag.TituloBandeja = "Mis Borradores";
                    Libro.LstAnotaciones = db.LOD_Anotaciones.Include(a => a.LOD_LibroObras).Include(a => a.MAE_SubtipoComunicacion)/*.Include(a => a.Usuario)*/.Where(a => a.IdLod == idlib && a.Estado == 1 && a.UserId == IdUsuario).OrderByDescending(a => a.FechaIngreso).ToList();
                    break;
                case 3:
                    ViewBag.TituloBandeja = "Mis Destacadas";
                    Libro.LstAnotaciones = db.LOD_Anotaciones.Include(a => a.LOD_LibroObras).Include(a => a.MAE_SubtipoComunicacion)/*.Include(a => a.Usuario)*/.Where(a => Libro.LstDestacadas.Contains(a.IdAnotacion) && a.Estado != 1).OrderByDescending(a => a.FechaPub).ToList();
                    break;
                case 4:
                    ViewBag.TituloBandeja = "Nombrado en";
                    List<int> receptor = db.LOD_UserAnotacion.Where(a => a.EsPrincipal == true && a.IdUsLod == IdUsLod).Select(i => i.IdAnotacion).ToList();
                    Libro.LstAnotaciones = db.LOD_Anotaciones.Include(a => a.LOD_LibroObras).Include(a => a.MAE_SubtipoComunicacion)/*.Include(a => a.Usuario)*/.Where(a => receptor.Contains(a.IdAnotacion) && a.Estado != 1).OrderByDescending(a => a.FechaPub).ToList();
                    break;
                case 5:
                    ViewBag.TituloBandeja = "Mis Pendientes";
                    List<int> responsable = db.LOD_UserAnotacion.Where(a => a.EsRespRespuesta == true && a.IdUsLod == IdUsLod).Select(i => i.IdAnotacion).ToList();
                    Libro.LstAnotaciones = db.LOD_Anotaciones.Include(a => a.LOD_LibroObras).Include(a => a.MAE_SubtipoComunicacion)/*.Include(a => a.Usuario)*/.Where(a => responsable.Contains(a.IdAnotacion) && a.Estado == 2).OrderByDescending(a => a.FechaPub).ToList();
                    break;

                case 7:
                    ViewBag.TituloBandeja = "Pendiente de Respuesta";
                    Libro.LstAnotaciones = db.LOD_Anotaciones.Include(a => a.LOD_LibroObras).Include(a => a.MAE_SubtipoComunicacion)/*.Include(a => a.Usuario)*/.Where(a => a.IdLod == idlib && a.Estado == 2).OrderByDescending(a => a.FechaPub).ToList();
                    break;
                case 8:
                    ViewBag.TituloBandeja = "Pendiente de Cierre";
                    Libro.LstAnotaciones = db.LOD_Anotaciones.Include(a => a.LOD_LibroObras).Include(a => a.MAE_SubtipoComunicacion)/*.Include(a => a.Usuario)*/.Where(a => a.IdLod == idlib && a.Estado == 3).OrderByDescending(a => a.FechaPub).ToList();
                    break;
                case 9:
                    ViewBag.TituloBandeja = "Cerradas";
                    Libro.LstAnotaciones = db.LOD_Anotaciones.Include(a => a.LOD_LibroObras).Include(a => a.MAE_SubtipoComunicacion)/*.Include(a => a.Usuario)*/.Where(a => a.IdLod == idlib && a.Estado == 4).OrderByDescending(a => a.FechaPub).ToList();
                    break;
            }

            if (Libro.LstAnotaciones != null)
                Libro.LstAnotaciones = ProcesarAnotaciones(Libro.LstAnotaciones);

            ViewBag.FiltroEvaluacion = false;
            return PartialView("_GetFiltroRapido", Libro);

        }

        private List<LOD_Anotaciones> ProcesarAnotaciones(List<LOD_Anotaciones> Base)
        {
            //foreach (LOD_Anotaciones Anotacion in Base)
            //{
            //    var uSER = db.Users.Find(Anotacion.UserId);
            //    Anotacion.SEG_Users = uSER.Nombres + " " + uSER.Apellidos;
            //    Anotacion.RutImgRemitente = uSER.RutaImagen;

            //    List<LOD_ReferenciasAnot> Referencias = db.LOD_ReferenciasAnot.Where(x => x.IdAnotacion == Anotacion.IdAnotacion).ToList();

            //    foreach (LOD_ReferenciasAnot Ref in Referencias)
            //    {
            //        LOD_Anotaciones AnotRef = db.LOD_Anotaciones.Find(Ref.IdRefAnot);
            //        var uSERRef = db.Users.Find(Ref.LOD_Anotaciones.UserId);
            //        AnotRef.SEG_Users = uSER.Nombres + " " + uSER.Apellidos;
            //        AnotRef.RutImgRemitente = uSER.RutaImagen;
            //        Anotacion.LOD_AnotacionesRef.Add(AnotRef);
            //    }
            //}
            return Base;
        }

        public ActionResult GetOpcionesFiltro(int id)
        {
            string IdUsuario = User.Identity.GetUserId();
            int IdUsLod = db.LOD_UsuariosLod.Where(x => x.IdLod == id && x.UserId == IdUsuario && x.Activo).FirstOrDefault().IdUsLod;
            LOD_LibroObras libroObra = db.LOD_LibroObras.AsNoTracking().Where(a => a.IdLod == id).SingleOrDefault();  
            libroObra.DataPanelLO = new Auxiliares.DataPanelLO();
            libroObra.DataPanelLO.numTodos = db.LOD_Anotaciones.AsNoTracking().Where(a => a.IdLod == id && a.Estado != 1).Count();
            libroObra.DataPanelLO.numMias = db.LOD_Anotaciones.AsNoTracking().Where(a => a.IdLod == id && a.UserId == IdUsuario && a.Estado != 1).Count();
            libroObra.DataPanelLO.numBorr = db.LOD_Anotaciones.AsNoTracking().Where(a => a.IdLod == id && a.UserId == IdUsuario && a.Estado == 1).Count();
            libroObra.DataPanelLO.numDest = db.LOD_UserAnotacion.AsNoTracking().Where(a => a.IdUsLod == IdUsLod && a.Destacado == true && a.LOD_Anotaciones.Estado != 1).Count();
            libroObra.DataPanelLO.numNombEn = db.LOD_UserAnotacion.AsNoTracking().Where(a => a.IdUsLod == IdUsLod && a.EsPrincipal == true && a.LOD_Anotaciones.Estado != 1).Count();
            libroObra.DataPanelLO.numPend = db.LOD_UserAnotacion.AsNoTracking().Where(a => a.IdUsLod == IdUsLod && a.LOD_Anotaciones.Estado == 2 && a.EsRespRespuesta == true).Count();
            return PartialView("_GetOpcionesFiltro", libroObra);
        }

        public ActionResult selLibro(int id)
        {
            try
            {
                string IdUsuario = User.Identity.GetUserId();//*Convert.ToInt32(se.get("IdUsuario"));
                CON_Contratos contrato = db.CON_Contratos.Find(id);

                ViewBag.CodContrato = contrato.CodigoContrato;
                List<int> accesos = db.LOD_UsuariosLod.Where(ul => ul.UserId == IdUsuario).Select(ul => ul.IdLod).ToList();

                var libros = db.LOD_LibroObras.Where(l => l.IdContrato == id && accesos.Contains(l.IdLod)).ToList();

                return PartialView("_SelLibro", libros);
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }

        }


        public async Task<ActionResult> ArchivarLibro(int IdLibro)
        {
            var libro = db.LOD_LibroObras.Find(IdLibro);
            if (libro == null)
                return HttpNotFound();

            return PartialView("_ArchivarLibro", libro);
        }

        [HttpPost, ActionName("ArchivarLibro")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ArchivarLibroConfirm(int IdLibroObra)
        {
            try
            {
                
                LOD_LibroObras libroobra = await db.LOD_LibroObras.FindAsync(IdLibroObra);
                libroobra.FechaCierre = DateTime.Now;
                libroobra.Estado = 0;
                db.Entry(libroobra).State = EntityState.Modified;
                db.SaveChanges();
                return Content("true;" + IdLibroObra);
            }
            catch (Exception e)
            {
                return Content(e.Message);
            }
        }

        public async Task<ActionResult> AperturarLibro(int IdLibro)
        {
            var libro = db.LOD_LibroObras.Find(IdLibro);
            if (libro == null)
                return HttpNotFound();

            return PartialView("_AperturarLibro", libro);
        }

        [HttpPost, ActionName("AperturarLibro")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AperturarLibroConfirm(int IdLibroObra)
        {
            try
            {
                LOD_LibroObras libroobra = await db.LOD_LibroObras.FindAsync(IdLibroObra);
                libroobra.FechaCierre = null;
                libroobra.Estado = 1;
                db.Entry(libroobra).State = EntityState.Modified;
                db.SaveChanges();

                Log_Helper log_Helper = new Log_Helper();
                string accion = "Se ha Aperturado el Libro: " + libroobra.NombreLibroObra;
                bool response = await log_Helper.SetObjectLog(1, libroobra, accion, User.Identity.GetUserId());

                return Content("true;" + IdLibroObra);
            }
            catch (Exception e)
            {
                return Content(e.Message);
            }
        }

        //Add UsuarioLibro
        public async Task<ActionResult> AddUserLib(int idLib, string idUs)
        {
            try
            {
                LOD_UsuariosLod usuarioLibro = await db.LOD_UsuariosLod.Where(ul => ul.IdLod == idLib && ul.UserId == idUs).FirstOrDefaultAsync();
                if (usuarioLibro != null)
                {
                    return Content("exist");
                }
                else
                {
                    LOD_UsuariosLod newUsuarioLibro = new LOD_UsuariosLod();
                    newUsuarioLibro.IdLod = idLib;
                    newUsuarioLibro.UserId = idUs;
                    db.LOD_UsuariosLod.Add(newUsuarioLibro);
                    await db.SaveChangesAsync();
                }
            }
            catch (Exception e)
            {
                return Content(e.Message);
            }
            return Content("true");
        }

        //Quitar UsuarioLibro
        public async Task<ActionResult> QuitarUserLib(int idLib, string idUs)
        {
            try
            {
                LOD_UsuariosLod usuarioLibro = await db.LOD_UsuariosLod.Where(ul => ul.IdLod == idLib && ul.UserId == idUs).FirstOrDefaultAsync();
                if (usuarioLibro == null)
                {
                    return Content("false");
                }
                db.LOD_UsuariosLod.Remove(usuarioLibro);
                await db.SaveChangesAsync();
            }
            catch (Exception e)
            {
                return Content(e.Message);
            }
            return Content("true");
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

            ViewBag.PermiteCierre = true;
            if ((cierreView.listadoLiquidacion.Count > 0 || cierreView.listadoNoAprobados.Count > 0) && libro.IdTipoLod == 2 ) //IdTipoLod == 2, libro de comunicaciones
                ViewBag.PermiteCierre = false;

            return PartialView("_modalFormCierre", cierreView);
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
                string response = "true;" + libroobra.IdLod;

                Log_Helper log_Helper = new Log_Helper();
                string accion = "Se ha cerrado el Libro: " + libroobra.NombreLibroObra + " del contrato " + libroobra.CON_Contratos.NombreContrato;
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


        public async Task<JsonResult> GetActivateState(int id)
        {
            Form_Validation val = new Form_Validation() { Status = false, ErrorMessage = new List<string>() };
            try
            {
                var lod = await db.LOD_LibroObras.Where(a => a.Estado == 1 && a.IdLod == id && a.OTP == null).FirstOrDefaultAsync();
                if (lod != null)
                {
                    val.Status = true;
                    val.Parametros = id.ToString();
                }

            }
            catch (Exception ex)
            {
                val.Status = false;
                val.ErrorMessage.Add(ex.Message);
            }

            return Json(val, JsonRequestBehavior.AllowGet);
        }


        public ActionResult Maqueta()
        {
            return View();
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
