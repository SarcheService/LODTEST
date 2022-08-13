using LOD_APR.Areas.GLOD.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using LOD_APR.Models;
using LOD_APR.Areas.Admin.Models;
using LOD_APR.Helpers;
using LOD_APR.Areas.GLOD.Helpers;
using System.IO;
using Microsoft.AspNet.Identity;

namespace LOD_APR.Areas.GLOD.Controllers
{
    [CustomAuthorize]
    public class BibliotecaController : Controller
    {
        private LOD_DB db = new LOD_DB();
        private Log_Helper Log_Helper = new Log_Helper();
        // GET: BIB/Home
        public ActionResult Index()
        {
            List<LOD_docAnotacion> lista = new List<LOD_docAnotacion>();


            List<MAE_DireccionesMOP> mAE_Direcciones = db.MAE_DireccionesMOP.ToList();
            SelectList listDirecciones = new SelectList((from p in mAE_Direcciones.ToList()
                                                  select new
                                                  {
                                                      IdDireccion = p.IdDireccion,
                                                      NombreDireccion = p.NombreDireccion
                                                  }),
                                                       "IdDireccion",
                                                       "NombreDireccion", -1);

            ViewBag.IdDireccion = listDirecciones;


            List<MAE_sujetoEconomico> mAE_SujetoEconomico = db.MAE_sujetoEconomico.Where(x => x.Activo && x.EsContratista == true).ToList();
            ViewBag.IdSujEcon = new SelectList((from p in mAE_SujetoEconomico.ToList()
                                                                             select new
                                                                             {
                                                                                 IdSujEcon = p.IdSujEcon,
                                                                                 NomFantasia = p.NomFantasia
                                                                             }),
                                                       "IdSujEcon",
                                                       "NomFantasia", -1);

            List<MAE_sujetoEconomico> fiscalizadores = db.MAE_sujetoEconomico.Where(x => x.Activo && x.EsMandante == true).ToList();
            ViewBag.IdFiscalizador = new SelectList((from p in fiscalizadores.ToList()
                                                select new
                                                {
                                                    IdFiscalizador = p.IdSujEcon,
                                                    NomFantasia = p.NomFantasia
                                                }),
                                                       "IdFiscalizador",
                                                       "NomFantasia", -1);

            List<LOD_LibroObras> lOD_LibroObras = db.LOD_LibroObras.ToList();
            ViewBag.IdLibroObra = new SelectList((from p in lOD_LibroObras.ToList()
                                                         select new
                                                         {
                                                             IdLOD = p.IdLod,
                                                             NombreLibroObra = p.NombreLibroObra
                                                         }),
                                                       "IdLod",
                                                       "NombreLibroObra", -1);
            List<MAE_TipoLOD> mAE_TipoLOD = db.MAE_TipoLOD.ToList();
            ViewBag.IdTipoLibro = new SelectList((from p in mAE_TipoLOD.ToList()
                                                  select new
                                                  {
                                                      IdTipoLod = p.IdTipoLod,
                                                      Nombre = p.Nombre
                                                  }),
                                                       "IdTipoLod",
                                                       "Nombre", -1);

            List<MAE_TipoComunicacion> mAE_TipoComunicacion = db.MAE_TipoComunicacion.Where(x => x.Activo).ToList();
            ViewBag.IdTipoComunicacion= new SelectList((from p in mAE_TipoComunicacion.ToList()
                                                                            select new
                                                                            {
                                                                                IdTipoCom = p.IdTipoCom,
                                                                                Nombre = p.Nombre
                                                                            }),
                                                       "IdTipoCom",
                                                       "Nombre", -1);

            List<MAE_SubtipoComunicacion> mAE_SubtipoComunicacion = db.MAE_SubtipoComunicacion.Where(x => x.Activo && !x.Nombre.Equals("Comunicación General")).ToList();
            ViewBag.IdSubtipoComunicacion= new SelectList((from p in mAE_SubtipoComunicacion.ToList()
                                                           select new
                                                           {
                                                               IdTipoSub = p.IdTipoSub,
                                                               Nombre = p.Nombre
                                                           }),
                                                       "IdTipoSub",
                                                       "Nombre", -1);

            List<CON_Contratos> cON_Contratos = db.CON_Contratos.Where(x => x.EstadoContrato != 3).ToList();
            ViewBag.IdContrato= new SelectList((from p in cON_Contratos.ToList()
                                                select new
                                                {
                                                    IdContrato = p.IdContrato,
                                                    CodigoContrato = p.CodigoContrato + "-" + p.NombreContrato
                                                }),
                                                       "IdContrato",
                                                       "CodigoContrato", -1);

            List<MAE_TipoDocumento> mAE_TipoDocumentos = db.MAE_TipoDocumento.Where(x => x.Activo).ToList();
            ViewBag.IdTipoDoc = new SelectList((from p in mAE_TipoDocumentos.ToList()
                                             select new
                                             {
                                                 IdTipo = p.IdTipo,
                                                 Tipo = p.Tipo
                                             }),
                                                       "IdTipo",
                                                       "Tipo", -1);

            if (ValidaPermisos.ValidaPermisosEnController("0070000000"))
            {
                return View(lista);
            }
            else
            {
                return RedirectToAction("SinPermiso", "../Pub");
            }

           
        }

        //public async Task<ActionResult> GetFiltroRapido(int id)
        //{

        //    List<LOD_docAnotacion> docs = new List<LOD_docAnotacion>();
        //    switch(id)
        //    {
        //        case 1:
        //            docs = db.LOD_docAnotacion.Where(x => x.MAE_TipoDocumento.TipoClasi == 1).ToList();
        //        break;
        //        case 2:
        //            docs = db.LOD_docAnotacion.Where(x => x.MAE_TipoDocumento.TipoClasi == 2).ToList();
        //            break;
        //        case 3:
        //            docs = db.LOD_docAnotacion.Where(x => x.MAE_TipoDocumento.TipoClasi == 3).ToList();
        //            break;
        //        case 4:
        //            docs = db.LOD_docAnotacion.Where(x => x.MAE_TipoDocumento.TipoClasi == 4).ToList();
        //            break;

        //        default:
        //            break;
        //    }


        //    return PartialView("_GetTable", docs);
        //}


        public ActionResult GetFiltro(Filtro_Documentos filtro)
        {

            var documentos = db.LOD_docAnotacion.AsQueryable();
            string userId = User.Identity.GetUserId();
            List<int> listadoLod = db.LOD_UsuariosLod.Where(x => x.UserId == userId).Select(x => x.IdLod).ToList();


            documentos = db.LOD_docAnotacion.Where(x => x.IdDocAnotacion != 0 && x.LOD_Anotaciones.Estado == 2 && x.MAE_documentos.IdPath != null && listadoLod.Contains(x.LOD_Anotaciones.IdLod));

            if (filtro.IdDireccion != 0)
                documentos = documentos.Where(a => a.LOD_Anotaciones.LOD_LibroObras.CON_Contratos.MAE_Sucursal.IdDireccion == filtro.IdDireccion);
          
            if(filtro.IdSujEcon != 0 && filtro.IdFiscalizador != 0)
                documentos = documentos.Where(a => a.LOD_Anotaciones.LOD_LibroObras.CON_Contratos.IdEmpresaContratista == filtro.IdSujEcon || a.LOD_Anotaciones.LOD_LibroObras.CON_Contratos.IdEmpresaFiscalizadora == filtro.IdFiscalizador);
            else if(filtro.IdSujEcon != 0 && (filtro.IdFiscalizador == 0 || filtro.IdFiscalizador == null))
                documentos = documentos.Where(a => a.LOD_Anotaciones.LOD_LibroObras.CON_Contratos.IdEmpresaContratista == filtro.IdSujEcon);
            else if ((filtro.IdSujEcon == 0 || filtro.IdSujEcon == null) && filtro.IdFiscalizador != 0 )
                documentos = documentos.Where(a => a.LOD_Anotaciones.LOD_LibroObras.CON_Contratos.IdEmpresaFiscalizadora == filtro.IdFiscalizador);


            if (filtro.IdContrato != 0)
                documentos = db.LOD_docAnotacion.Where(a => a.LOD_Anotaciones.LOD_LibroObras.IdContrato == filtro.IdContrato);

            if (filtro.IdTipoLibroObra != 0)
            {
                documentos = documentos.Where(a => a.LOD_Anotaciones.LOD_LibroObras.IdTipoLod == filtro.IdTipoLibroObra);
            }

            if (filtro.IdTipoComunicacion != 0)
            {
                documentos = documentos.Where(a => a.LOD_Anotaciones.MAE_SubtipoComunicacion.IdTipoCom == filtro.IdTipoComunicacion);
            }
            if (filtro.IdSubtipoComunicacion != 0)
            {
                documentos = documentos.Where(a => a.LOD_Anotaciones.IdTipoSub == filtro.IdSubtipoComunicacion);
            }
            if (filtro.IdTipoDoc != 0)
            {
                documentos = documentos.Where(a => a.IdTipoDoc == filtro.IdTipoDoc );
            }

            

            if (filtro.Formulario && !filtro.DocAdmin && !filtro.DocTecnicos && !filtro.Otros)
            {
                documentos = documentos.Where(a => a.MAE_TipoDocumento.TipoClasi == 1);
            }
            else if (!filtro.Formulario && filtro.DocAdmin && !filtro.DocTecnicos && !filtro.Otros)
            {
                documentos = documentos.Where(a => a.MAE_TipoDocumento.TipoClasi == 2);
            }
            else if (!filtro.Formulario && !filtro.DocAdmin && filtro.DocTecnicos && !filtro.Otros)
            {
                documentos = documentos.Where(a => a.MAE_TipoDocumento.TipoClasi == 3);
            }
            else if (!filtro.Formulario && !filtro.DocAdmin && !filtro.DocTecnicos && filtro.Otros)
            {
                documentos = documentos.Where(a => a.MAE_TipoDocumento.TipoClasi == 4);
            }
            else if (filtro.Formulario && filtro.DocAdmin && !filtro.DocTecnicos && !filtro.Otros)
            {
                documentos = documentos.Where(a => a.MAE_TipoDocumento.TipoClasi == 1 || a.MAE_TipoDocumento.TipoClasi == 2);
            }
            else if (filtro.Formulario && !filtro.DocAdmin && filtro.DocTecnicos && !filtro.Otros)
            {
                documentos = documentos.Where(a => a.MAE_TipoDocumento.TipoClasi == 1 || a.MAE_TipoDocumento.TipoClasi == 3);
            }
            else if (filtro.Formulario && !filtro.DocAdmin && !filtro.DocTecnicos && filtro.Otros)
            {
                documentos = documentos.Where(a => a.MAE_TipoDocumento.TipoClasi == 1 || a.MAE_TipoDocumento.TipoClasi == 4);
            }
            else if (!filtro.Formulario && filtro.DocAdmin && filtro.DocTecnicos && !filtro.Otros)
            {
                documentos = documentos.Where(a => a.MAE_TipoDocumento.TipoClasi == 2 || a.MAE_TipoDocumento.TipoClasi == 3);
            }
            else if (!filtro.Formulario && filtro.DocAdmin && !filtro.DocTecnicos && filtro.Otros)
            {
                documentos = documentos.Where(a => a.MAE_TipoDocumento.TipoClasi == 2 || a.MAE_TipoDocumento.TipoClasi == 4);
            }
            else if (filtro.Formulario && !filtro.DocAdmin && filtro.DocTecnicos && filtro.Otros)
            {
                documentos = documentos.Where(a => a.MAE_TipoDocumento.TipoClasi == 3 || a.MAE_TipoDocumento.TipoClasi == 4);
            }
            else if (filtro.Formulario && filtro.DocAdmin && filtro.DocTecnicos && !filtro.Otros)
            {
                documentos = documentos.Where(a => a.MAE_TipoDocumento.TipoClasi == 1 || a.MAE_TipoDocumento.TipoClasi == 2 || a.MAE_TipoDocumento.TipoClasi == 3);
            }
            else if (filtro.Formulario && filtro.DocAdmin && !filtro.DocTecnicos && filtro.Otros)
            {
                documentos = documentos.Where(a => a.MAE_TipoDocumento.TipoClasi == 1 || a.MAE_TipoDocumento.TipoClasi == 2 || a.MAE_TipoDocumento.TipoClasi == 4);
            }
            else if (filtro.Formulario && filtro.DocAdmin && filtro.DocTecnicos && filtro.Otros)
            {
                documentos = documentos.Where(a => a.MAE_TipoDocumento.TipoClasi == 1 || a.MAE_TipoDocumento.TipoClasi == 2 || a.MAE_TipoDocumento.TipoClasi == 3 || a.MAE_TipoDocumento.TipoClasi == 4);
            }


            List<LOD_docAnotacion> docs = documentos.ToList<LOD_docAnotacion>();

            if (!String.IsNullOrEmpty(filtro.FechaCreacion) && docs.Count() != 0)
            {
                string[] fechas = filtro.FechaCreacion.Split('~');
                DateTime first = Convert.ToDateTime(fechas[0]);
                DateTime second = Convert.ToDateTime(fechas[1]);
                docs = docs.Where(x => x.MAE_documentos.FechaCreacion > first && x.MAE_documentos.FechaCreacion < second).ToList();
            }

            return PartialView("_GetTable", docs);
        }


        //public async Task<JsonResult> GetStats()
        //{

        //    BibUserStats stats = new BibUserStats()
        //    {
        //        Formularios = db.LOD_docAnotacion.Where(x => x.MAE_TipoDocumento.TipoClasi == 1).ToList().Count(),
        //        DocTecnicos = db.LOD_docAnotacion.Where(x => x.MAE_TipoDocumento.TipoClasi == 2).ToList().Count(),
        //        DocAdmin = db.LOD_docAnotacion.Where(x => x.MAE_TipoDocumento.TipoClasi == 3).ToList().Count(),
        //        Otros = db.LOD_docAnotacion.Where(x => x.MAE_TipoDocumento.TipoClasi == 4).ToList().Count()

        //    };
        //    return Json(stats, JsonRequestBehavior.AllowGet);
        //}

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

        public ApplicationUser GetUserInSession()
        {
            string userName = User.Identity.GetUserId();
            var user = db.Users.Find(userName);
            return user;
        }

        public async Task<ActionResult> GetSujEcon(int? IdDireccion)
        {
            List<MAE_sujetoEconomico> mAE_SujetoEconomico = new List<MAE_sujetoEconomico>();
            if (IdDireccion == null || IdDireccion == 0)
            {
                mAE_SujetoEconomico = db.MAE_sujetoEconomico.Where(x => x.Activo && x.EsContratista == true).ToList();
            }
            else
            {
                List<int> auxContratos = db.CON_Contratos.Where(x => x.MAE_Sucursal.IdDireccion == IdDireccion).Select(x => x.IdEmpresaContratista.Value).ToList();
                mAE_SujetoEconomico = db.MAE_sujetoEconomico.Where(x => x.Activo && auxContratos.Contains(x.IdSujEcon)).ToList();
               
            }

            ViewBag.IdSujEcon = new SelectList((from p in mAE_SujetoEconomico.ToList()
                                                select new
                                                {
                                                    IdSujEcon = p.IdSujEcon,
                                                    NomFantasia = p.NomFantasia
                                                }),
                                                          "IdSujEcon",
                                                          "NomFantasia");

            return PartialView("_GetSujetos");
        }

        public async Task<ActionResult> GetFiscalizador(int? IdDireccion)
        {
            List<MAE_sujetoEconomico> mAE_SujetoEconomico = new List<MAE_sujetoEconomico>();
            if (IdDireccion == null || IdDireccion == 0)
            {
                mAE_SujetoEconomico = await db.MAE_sujetoEconomico.Where(x => x.Activo && x.EsMandante == true).ToListAsync();
            }
            else
            {
                List<int> auxContratos = db.CON_Contratos.Where(x => x.MAE_Sucursal.IdDireccion == IdDireccion).Select(x => x.IdEmpresaFiscalizadora.Value).ToList();
                mAE_SujetoEconomico = db.MAE_sujetoEconomico.Where(x => x.Activo && auxContratos.Contains(x.IdSujEcon)).ToList();

            }

            ViewBag.IdFiscalizador = new SelectList((from p in mAE_SujetoEconomico.ToList()
                                                select new
                                                {
                                                    IdFiscalizador = p.IdSujEcon,
                                                    NomFantasia = p.NomFantasia
                                                }),
                                                          "IdFiscalizador",
                                                          "NomFantasia");

            return PartialView("_GetFiscalizador");
        }


        public async Task<ActionResult> GetContratos(int? IdSujEcon, int? IdFiscalizador, int? IdDireccion)
        {
            List<CON_Contratos> cON_Contratos = new List<CON_Contratos>();
            if ((IdSujEcon == null || IdSujEcon == 0) && (IdFiscalizador == null || IdFiscalizador == 0) && (IdDireccion == null || IdDireccion == 0))
            {
                cON_Contratos = await db.CON_Contratos.ToListAsync();           
            }
            else
            {
                List<int> maeSucursal = new List<int>();
                if (IdDireccion != 0 && IdDireccion != null)
                    maeSucursal = db.MAE_Sucursal.Where(x => x.IdDireccion == IdDireccion).Select(x => x.IdSucursal).ToList();

                if ((IdSujEcon != null && IdSujEcon != 0) && (IdFiscalizador != null && IdFiscalizador != 0) && (IdDireccion != null && IdDireccion != 0))
                    cON_Contratos = db.CON_Contratos.Where(x => x.IdEmpresaContratista == IdSujEcon && x.IdEmpresaFiscalizadora == IdFiscalizador && maeSucursal.Contains(x.IdDireccionContrato.Value)).ToList();
                else if ((IdSujEcon != null && IdSujEcon != 0) && (IdFiscalizador == null || IdFiscalizador == 0) && (IdDireccion != null && IdDireccion != 0))
                    cON_Contratos = db.CON_Contratos.Where(x => x.IdEmpresaContratista == IdSujEcon && maeSucursal.Contains(x.IdDireccionContrato.Value)).ToList();
                else if ((IdSujEcon != null && IdSujEcon != 0) && (IdFiscalizador == null || IdFiscalizador == 0) && (IdDireccion == null || IdDireccion == 0))
                    cON_Contratos = db.CON_Contratos.Where(x => x.IdEmpresaContratista == IdSujEcon).ToList();
                else if ((IdSujEcon == null || IdSujEcon == 0) && (IdFiscalizador != null && IdFiscalizador != 0) && (IdDireccion != null && IdDireccion != 0))
                    cON_Contratos = db.CON_Contratos.Where(x => x.IdEmpresaFiscalizadora == IdFiscalizador && maeSucursal.Contains(x.IdDireccionContrato.Value)).ToList();
                else if ((IdSujEcon == null || IdSujEcon == 0) && (IdFiscalizador != null && IdFiscalizador != 0) && (IdDireccion == null || IdDireccion == 0))
                    cON_Contratos = db.CON_Contratos.Where(x => x.IdEmpresaFiscalizadora == IdFiscalizador).ToList();
                else if ((IdSujEcon == null || IdSujEcon == 0) && (IdFiscalizador == null || IdFiscalizador == 0) && (IdDireccion != null && IdDireccion != 0))
                    cON_Contratos = db.CON_Contratos.Where(x => maeSucursal.Contains(x.IdDireccionContrato.Value)).ToList();
                else
                    cON_Contratos = new List<CON_Contratos>();
                
            }

            ViewBag.IdContrato = new SelectList((from p in cON_Contratos.ToList()
                                                 select new
                                                 {
                                                     IdContrato = p.IdContrato,
                                                     CodigoContrato = p.CodigoContrato + "-" + p.NombreContrato
                                                 }),
                                                           "IdContrato",
                                                           "CodigoContrato");

            return PartialView("_GetContratos");
        }

        public async Task<ActionResult> GetTipoLOD(int? IdContrato)
        {
            List<MAE_TipoLOD> mAE_TipoLOD = new List<MAE_TipoLOD>();
            if (IdContrato == null || IdContrato == 0)
            {
                mAE_TipoLOD = db.MAE_TipoLOD.ToList();
            }
            else
            {
                mAE_TipoLOD = await db.LOD_LibroObras.Where(x => x.IdContrato == IdContrato).Select(x => x.MAE_TipoLOD).ToListAsync();
            }

            ViewBag.IdTipoLibro = new SelectList((from p in mAE_TipoLOD.ToList()
                                                  select new
                                                  {
                                                      IdTipoLod = p.IdTipoLod,
                                                      Nombre = p.Nombre
                                                  }),
                                                       "IdTipoLod",
                                                       "Nombre");

            return PartialView("_GetTipoLOD");
        }

        public async Task<ActionResult> GetTipoCom(int? IdTipoLod)
        {
            List<MAE_TipoComunicacion> mAE_TipoComunicacion = new List<MAE_TipoComunicacion>();

            if (IdTipoLod == null || IdTipoLod == 0 )
            {
                mAE_TipoComunicacion = db.MAE_TipoComunicacion.Where(x => x.Activo ).ToList();
            }
            else
            {
                mAE_TipoComunicacion = await db.MAE_TipoComunicacion.Where(x => x.Activo && x.IdTipoLod == IdTipoLod).ToListAsync();
                
            }

            ViewBag.IdTipoComunicacion = new SelectList((from p in mAE_TipoComunicacion.ToList()
                                                         select new
                                                         {
                                                             IdTipoCom = p.IdTipoCom,
                                                             Nombre = p.Nombre
                                                         }),
                                                       "IdTipoCom",
                                                       "Nombre");

            return PartialView("_GetTipoCOM");
        }

        public async Task<ActionResult> GetSubTipo(int? IdTipoCom)
        {
            List<MAE_SubtipoComunicacion> mAE_SubtipoComunicacion = new List<MAE_SubtipoComunicacion>();
            if (IdTipoCom == null || IdTipoCom == 0 )
            {
                mAE_SubtipoComunicacion = db.MAE_SubtipoComunicacion.Where(x => x.Activo).ToList();
            }
            else
            {
                mAE_SubtipoComunicacion = db.MAE_SubtipoComunicacion.Where(x => x.Activo && x.IdTipoCom == IdTipoCom).ToList();
            }
            
            ViewBag.IdSubtipoComunicacion = new SelectList((from p in mAE_SubtipoComunicacion.ToList()
                                                            select new
                                                            {
                                                                IdTipoSub = p.IdTipoSub,
                                                                Nombre = p.Nombre
                                                            }),
                                                       "IdTipoSub",
                                                       "Nombre");

            return PartialView("_GetSubtipo");
        }

        public async Task<ActionResult> GetTipoDoc(int? IdTipoSub)
        {

            List<MAE_TipoDocumento> mAE_TipoDocumentos = new List<MAE_TipoDocumento>();

            if (IdTipoSub == null || IdTipoSub == 0 )
            {
                mAE_TipoDocumentos = db.MAE_TipoDocumento.Where(x => x.Activo).ToList();
            }
            else
            {
                mAE_TipoDocumentos = db.MAE_CodSubCom.Where(x => x.Activo && x.IdTipoSub == IdTipoSub).Select(x => x.MAE_TipoDocumento).ToList();
            }

            ViewBag.IdTipoDoc = new SelectList((from p in mAE_TipoDocumentos.ToList()
                                                select new
                                                {
                                                    IdTipo = p.IdTipo,
                                                    Tipo = p.Tipo
                                                }),
                                                       "IdTipo",
                                                       "Tipo");

            return PartialView("_GetTipoDoc");
        }

    }
}