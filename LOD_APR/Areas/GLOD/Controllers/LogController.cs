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
using LOD_APR.Helpers;
using Microsoft.AspNet.Identity;
using System.Linq.Expressions;
using System.Reflection;
using System.ComponentModel;
using ClosedXML.Excel;
using System.IO;

namespace LOD_APR.Areas.GLOD.Controllers
{
    [CustomAuthorize]
    public class LogController : Controller
    {
        private LOD_DB db = new LOD_DB();

        // GET: GLOD/Log
        public async Task<ActionResult> Index(int Id)
        {
            CON_Contratos contrato = await db.CON_Contratos.FindAsync(Id);
            ViewBag.NomContrato = contrato.CodigoContrato+" - "+contrato.NombreContrato;


            List<LOD_LibroObras> libroObra = db.LOD_LibroObras.Where(x => x.IdContrato == contrato.IdContrato).ToList();
            SelectList listDirecciones = new SelectList((from p in libroObra.ToList()
                                                         select new
                                                         {
                                                             IdLod = p.IdLod,
                                                             NombreLibroObra = p.NombreLibroObra
                                                         }),
                                                       "IdLod",
                                                       "NombreLibroObra");

            ViewBag.IdLod = listDirecciones;

            List<LOD_Anotaciones> anotaciones = db.LOD_Anotaciones.Where(x => x.LOD_LibroObras.IdContrato == contrato.IdContrato).ToList();
            SelectList listAnotaciones = new SelectList((from p in anotaciones.ToList()
                                                         select new
                                                         {
                                                             IdAnotacion = p.IdAnotacion,
                                                             TituloAnotacion = p.Correlativo + "-" + p.Titulo
                                                         }),
                                                       "IdAnotacion",
                                                       "TituloAnotacion");

            ViewBag.IdAnotacion = listAnotaciones;


            List<int> listadoLibros = libroObra.Select(x => x.IdLod).ToList();

            List<ApplicationUser> usuarios = db.LOD_UsuariosLod.Where(x => listadoLibros.Contains(x.IdLod)).Select(x => x.ApplicationUser).Distinct().ToList();
            SelectList listUsuarios = new SelectList((from p in usuarios.ToList()
                                                         select new
                                                         {
                                                             UserId = p.Id,
                                                             Nombre = p.NombreCompleto
                                                         }),
                                                       "UserId",
                                                       "Nombre");
            ViewBag.UserId = listUsuarios;
            ViewBag.IdContrato = contrato.IdContrato;

            



            string user = User.Identity.GetUserName();
            clsSession se = new clsSession();
            List<LOD_log> listado = new List<LOD_log>();
            Filtro_Log filtroSesion = (Filtro_Log)se.get("Filtro_Log");
            if (filtroSesion != null)
            {
                ViewBag.FiltroAsignacion = true;
                listado = GetFiltroExport(filtroSesion);
            }

            if (ValidaPermisos.ValidaPermisosEnController("0020070000"))
            {

                return View(listado);
            }
            else
            {
                return RedirectToAction("SinPermiso", "../Pub");
            }

            
        }

        public ActionResult GetFiltro(Filtro_Log filtro)
        {
            clsSession se = new clsSession();
            se.set("Filtro_Log", filtro);
            List<LOD_log> listadoLogs = GetFiltroExport(filtro);
            return PartialView("_GetTable", listadoLogs.OrderByDescending(x => x.IdLog));
        }


        public async Task<ActionResult> GetAnotaciones(int? IdLod, int IdContrato)
        {
            List<LOD_Anotaciones> anotaciones = new List<LOD_Anotaciones>();
            if (IdLod == null)
            {
                anotaciones = await db.LOD_Anotaciones.Where(x => x.LOD_LibroObras.IdContrato == IdContrato).ToListAsync();

            }
            else
            {
                anotaciones = await db.LOD_Anotaciones.Where(x => x.IdLod == IdLod).ToListAsync();

            }

            SelectList listAnotaciones = new SelectList((from p in anotaciones.ToList()
                                                         select new
                                                         {
                                                             IdAnotacion = p.IdAnotacion,
                                                             TituloAnotacion = p.Correlativo + "-" + p.Titulo
                                                         }),
                                                       "IdAnotacion",
                                                       "TituloAnotacion");

            ViewBag.IdAnotacion = listAnotaciones;

            return PartialView("_GetAnotaciones");
        }



        public FileResult Excel_Log()
        {
            //AGREGAR FILTRO EN VERSIÓN FINAL
            string user = User.Identity.GetUserName();
            List<LOD_log> ListaLog = new List<LOD_log>();
            clsSession se = new clsSession();
            Filtro_Log filtroSesion = (Filtro_Log)se.get("Filtro_Log");

            ListaLog = GetFiltroExport(filtroSesion); //Usuario común

            DataTable dt = GetTable("Anotaciones_Log", ListaLog,
                s => s.anotacion.LOD_LibroObras.NombreLibroObra,
                s => s.IdObjeto,
                s => s.anotacion.Titulo,
                s => s.ApplicationUser.NombreCompleto,
                s => s.FechaLog,
                s => s.Accion
                );
            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(dt);
                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Anotaciones_Log.xlsx");
                }
            }
        }



        public List<LOD_log> GetFiltroExport(Filtro_Log filtro)
        {
            List<LOD_log> listadoLogs = new List<LOD_log>();
            var logs = db.LOD_log.AsQueryable();
            List<int> IdsLibros = db.LOD_LibroObras.Where(x => x.IdContrato == filtro.IdContrato).Select(x => x.IdLod).ToList();
            List<int> IdsAnotaciones = db.LOD_Anotaciones.Where(x => x.Estado == 2 && IdsLibros.Contains(x.IdLod)).Select(x => x.IdAnotacion).ToList();
            logs = db.LOD_log.Where(x => (x.Objeto.Equals("Contrato") && x.IdObjeto == filtro.IdContrato) || (x.Objeto.Equals("Anotacion") && IdsAnotaciones.Contains(x.IdObjeto)) || (x.Objeto.Equals("Libro") && IdsLibros.Contains(x.IdObjeto))  );
            if(filtro != null) {

                if(filtro.IdLod == 0 && filtro.IdAnotacion == 0 && (filtro.UserId == "" || filtro.UserId == null || filtro.UserId == "0"))
                {
                    logs = db.LOD_log.Where(x => x.Objeto.Equals("Contrato") && x.IdObjeto == filtro.IdContrato);
                    listadoLogs = logs.ToList<LOD_log>();
                }
                else
                {
                    if (filtro.IdLod != 0)
                    {
                        List<int> listadoAnot = db.LOD_Anotaciones.Where(x => x.IdLod == filtro.IdLod).Select(x => x.IdAnotacion).ToList();
                        LOD_LibroObras libro = db.LOD_LibroObras.Where(x => x.IdLod == filtro.IdLod).FirstOrDefault();
                        logs = logs.Where(a => listadoAnot.Contains(a.IdObjeto) && a.Objeto.Equals("Anotacion") || (a.Objeto.Equals("Libro") && a.IdObjeto == filtro.IdLod || (a.Accion.Contains(libro.NombreLibroObra) && a.Objeto.Equals("Contrato"))));
                    }

                    if (filtro.IdAnotacion != 0)
                        logs = logs.Where(a => a.IdObjeto == filtro.IdAnotacion && a.Objeto.Equals("Anotacion"));

                    if (!String.IsNullOrEmpty(filtro.UserId) && !filtro.UserId.Equals("0"))
                        logs = logs.Where(a => a.UserId == filtro.UserId);


                    listadoLogs = logs.ToList<LOD_log>();

                    if (!String.IsNullOrEmpty(filtro.FechaLog) && listadoLogs.Count() != 0)
                    {
                        string[] fechas = filtro.FechaLog.Split('~');
                        DateTime first = Convert.ToDateTime(fechas[0]);
                        DateTime second = Convert.ToDateTime(fechas[1]);
                        listadoLogs = listadoLogs.Where(x => x.FechaLog > first && x.FechaLog < second).ToList();
                    }

                    foreach (var item in listadoLogs)
                    {
                        item.anotacion = db.LOD_Anotaciones.Where(x => x.IdAnotacion == item.IdObjeto).FirstOrDefault();
                    }
                }

            }
            else
            {
                listadoLogs = logs.ToList<LOD_log>();
            }
         
            return listadoLogs.OrderByDescending(x => x.IdLog).ToList();
        }

        private DataTable GetTable<T>(string DtName, IEnumerable<T> list, params Expression<Func<T, object>>[] fxns)
        {
            DataTable dt = new DataTable(DtName);

            foreach (var fxn in fxns)
            {
                dt.Columns.Add(new DataColumn(GetName(fxn)));
            }
            foreach (var item in list)
            {
                List<object> columnData = new List<object>();
                foreach (var fxn in fxns)
                {
                    try
                    {
                        columnData.Add(fxn.Compile()(item));
                    }
                    catch (Exception ex)
                    {

                        columnData.Add("-");
                    }
                }
                dt.Rows.Add(columnData.ToArray());
            }

            return dt;
        }

        static string GetName<T>(Expression<Func<T, object>> expr)
        {
            var member = expr.Body as MemberExpression;
            if (member != null)
                return GetName2(member);

            var unary = expr.Body as UnaryExpression;
            if (unary != null)
                return GetName2((MemberExpression)unary.Operand);

            return "?+?";
        }

        static string GetName2(MemberExpression member)
        {
            var fieldInfo = member.Member as FieldInfo;
            if (fieldInfo != null)
            {
                var d = fieldInfo.GetCustomAttribute(typeof(DescriptionAttribute)) as DescriptionAttribute;
                if (d != null) return d.Description;
                return fieldInfo.Name;
            }

            var propertInfo = member.Member as PropertyInfo;
            if (propertInfo != null)
            {
                if (propertInfo.GetCustomAttribute(typeof(DisplayNameAttribute), true) is DisplayNameAttribute d) return d.DisplayName;
                return propertInfo.Name;
            }

            return "?-?";
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
