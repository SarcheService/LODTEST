using LOD_APR.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LOD_APR.Areas.Admin.Controllers
{
    [CustomAuthorize]
    public class HomeController : Controller
    {

        public ActionResult Index()
        {
            return RedirectToAction("Index","SujetoEconomico");
        }

    }
}