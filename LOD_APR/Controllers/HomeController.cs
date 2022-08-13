using ACAMicroFramework.SqlDB;
using LOD_APR.Areas.Admin.Models;
using LOD_APR.Helpers;
using LOD_APR.Models;
using Microsoft.AspNet.Identity;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;

namespace LOD_APR.Controllers
{
    public class HomeController : Controller
    {
        private LOD_DB db = new LOD_DB();
         
        public ActionResult Index()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login","Account", new { errorADFS = false });
            }
            else
            {
                string UserId = User.Identity.GetUserId();
                ApplicationUser user = db.Users.Find(UserId);
                //SEG_UserContacto uc = db.SEG_UserContacto.Where(u => u.UserId == UserId).FirstOrDefault();

                if (user != null /*|| uc != null*/)
                {
                    if (user.EmailConfirmed == false || !user.Activo)
                    {
                        ModelState.AddModelError("Usuario", "El usuario no se encuentra Activo");
                        ViewBag.Empresas = db.MAE_Empresa;
                        return RedirectToAction("Login", "Account", new { errorADFS = true });
                    }
                }

                bool EsPersonal = false;
                string Cargo = "-";
                string RutaImg = "";
                string Empresa = string.Empty;
                int IdEmpresa = user.MAE_Sucursal.IdSujeto;
                if (user.RutaImagen != null)
                        RutaImg = "/Images/Contactos/" + user.RutaImagen;

                Cargo = user.CargoContacto;
                Empresa = user.MAE_Sucursal.MAE_sujetoEconomico.RazonSocial;
                

                HttpContext.Session.Add("Contacto", !EsPersonal);
                HttpContext.Session.Add("EmpresaUser", Empresa);
                HttpContext.Session.Add("CargoUser", Cargo);
                HttpContext.Session.Add("RutaImgUser", RutaImg);
                HttpContext.Session.Add("NombreUser", user.NombreCompleto);
                HttpContext.Session.Add("ApellidosUser", user.Apellidos);
                HttpContext.Session.Add("IdEmpresa", IdEmpresa);

                //Obtener Roles del Usuario 20-05-2019
                string con = ACA_SqlServer.generaConexion("LOD_DB");
                List<string> IdsRoles = new List<string>();
                DataTable rolesIds = ACA_SqlServer.SelectTable(con, "SELECT [RoleId] FROM [dbo].[SEG_UserRoles] where [UserId]='" + UserId + "'");
                foreach (DataRow role in rolesIds.Rows)
                {
                    IdsRoles.Add(role[0].ToString());
                };

                List<string> permisos = new List<string>();
                //HttpContext.Session.Add("TipoPerfil", db.Roles.Find(IdsRoles[0]).TipoPerfil);
                permisos = db.MAE_OpcionesRoles.Where(p => IdsRoles.Contains(p.IdRol)).Select(s => s.IdOpcion + ";" + s.IdEmpresa).Distinct().ToList();

                clsSession se = new clsSession();
                se.set("List_permisos", permisos);

            }

            return RedirectToAction("InicioRapido", "Contratos", new { area = "GLOD" });

        }
    }
}