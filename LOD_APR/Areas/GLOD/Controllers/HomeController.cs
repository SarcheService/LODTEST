using LOD_APR.Helpers;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Web.Mvc;

namespace LOD_APR.Areas.GLOD.Controllers
{
    [CustomAuthorize]
    public class HomeController : Controller
    {
        // GET: GLOD/Home
        public ActionResult Index()
        {
            clsSession se = new clsSession();
            int tiempo = HttpContext.Session.Timeout;

            return RedirectToAction("InicioRapido", "Contratos", new { area = "GLOD" });
        }

        public ActionResult Admin(int? id, string tipo)
        {
            //if (ValidaPermisos.ValidaPermisosEnController("00600100"))
            if (true)
            {
                if (id != null)
                {
                    ViewBag.Tipo = tipo;
                    ViewBag.Id = id;
                }
                else
                {
                    ViewBag.Tipo = "t_";
                    ViewBag.Id = 0;
                }

                if (ValidaPermisos.ValidaPermisosEnController("0020000000"))
                {
                    return View();
                }
                else
                {
                    return RedirectToAction("SinPermiso", "../Pub");
                }

                
            }
            else
            {
                //return RedirectToAction("LibroIndex", "LibroObras", new { area = "ASP" });
            }
        
        }
        public ActionResult getTree()
        {
            string userid = User.Identity.GetUserId();
            List<TreeNode> TreeAdmin = JsTrees.getTreeContratos(userid);
            return Content(JsonConvert.SerializeObject(TreeAdmin));
        }

        public ActionResult Details(int? id)
        {
            //Thread.Sleep(1000);
            return PartialView("_getDetailsPortafolio");
        }

    }
}