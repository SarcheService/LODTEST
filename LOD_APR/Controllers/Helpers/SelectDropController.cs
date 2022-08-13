using LOD_APR.Helpers;
using LOD_APR.Models;
using LOD_APR.ModelsView;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace LOD_APR.Controllers
{
    [CustomAuthorize]
    public class SelectDropController : Controller
    {
        private LOD_DB db = new LOD_DB();

        public async Task<JsonResult> GetPersonal(string param)
        {
            string[] Words = param.Split(' ');

            List<SelectDropdown> resultado = new List<SelectDropdown>();
            var list = await db.MAE_Contactos
                                             .Where(x => (x.Nombre.ToUpper().Contains(param.ToUpper())
                                             && x.Activo)).Take(5).ToListAsync();
            resultado = list.Select(s => s.SelectDropdown).ToList();

            return Json(resultado, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> GetSujeto(string param)
        {
            string[] Words = param.Split(' ');

            List<SelectDropdown> resultado = new List<SelectDropdown>();
            var list = await db.MAE_sujetoEconomico
                                             .Where(x => (x.RazonSocial.ToUpper().Contains(param.ToUpper())
                                             || x.Rut.ToUpper().Contains(param.ToUpper())
                                             || x.Rut.Replace(".", "").Contains(param.ToUpper())
                                             && x.Activo)).Take(5).ToListAsync();

            resultado = list.Select(s => s.SelectDropdown).ToList();
            return Json(resultado, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> GetCiudad(string param)
        {
            string[] Words = param.Split(' ');

            List<SelectDropdown> resultado = new List<SelectDropdown>();
            var list = await db.MAE_ciudad.Where(x => (x.Ciudad.ToUpper().Contains(param.ToUpper()))).Take(5).ToListAsync();

            resultado = list.Select(s => s.SelectDropdown).ToList();
            return Json(resultado, JsonRequestBehavior.AllowGet);
        }
    }
}