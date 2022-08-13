using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web.Mvc;
using Newtonsoft.Json;
using LOD_APR.Helpers;
using LOD_APR.Areas.Admin.Models;
using LOD_APR.Models;

namespace LOD_APR.Areas.Admin.Controllers
{
    [CustomAuthorize]
	public class SucursalController : Controller
    {
		private LOD_DB db = new LOD_DB();

		public async Task<ActionResult> Index()
        {
            var mAE_Sucursal = db.MAE_Sucursal.Include(m => m.MAE_ciudad).Include(m => m.MAE_sujetoEconomico);
            return View(await mAE_Sucursal.ToListAsync());
        }
		public async Task<ActionResult> getTable(int IdSujeto)
		{
			var mAE_Sucursal = db.MAE_Sucursal.Where(s=>s.IdSujeto == IdSujeto).Include(m => m.MAE_ciudad).Include(m => m.MAE_sujetoEconomico);
			return PartialView("_getSucursales", await mAE_Sucursal.ToListAsync());
		}
		public ActionResult Create(int IdSujEcon)
		{
			MAE_Sucursal suc = new MAE_Sucursal() { IdSujeto = IdSujEcon };
			MAE_sujetoEconomico mAE_SujetoEconomico = db.MAE_sujetoEconomico.Find(IdSujEcon);
			ViewBag.Titulo = "Nueva Sucursal";
			ViewBag.ClsModal = "hmodal-success";
			ViewBag.Action = "Create";
			ViewBag.IdCiudad = new SelectList(db.MAE_ciudad, "IdCiudad", "Ciudad", null);
			if (mAE_SujetoEconomico.EsGubernamental)
			{
				suc.MAE_sujetoEconomico = mAE_SujetoEconomico;
				ViewBag.IdDireccion = new SelectList(db.MAE_DireccionesMOP, "IdDireccion", "NombreDireccion", suc.IdDireccion);
			}

			if (ValidaPermisos.ValidaPermisosEnController("0010170001"))
			{
				return PartialView("_modalForm", suc);
			}
			else
			{
				return RedirectToAction("SinPermiso", "../Pub");
			}

			
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> Create([Bind(Include = "IdDireccion,Sucursal,Direccion,Encargado,IdSujeto,IdCiudad,Telefono,EsCentral,Email,IdDireccion")] MAE_Sucursal mAE_Sucursal)
		{
			try
			{
				mAE_Sucursal.EsCentral = false;
				db.MAE_Sucursal.Add(mAE_Sucursal);
				await db.SaveChangesAsync();
				return Content(JsonConvert.SerializeObject(new { error = false, idSujeto = mAE_Sucursal.IdSujeto }));
			}
			catch (Exception ex)
			{
				return Content(JsonConvert.SerializeObject(new { error = true, mensaje = ex.Message }));
			}

		}
		public async Task<ActionResult> Edit(int? id)
		{
			//SelectHelper select = new SelectHelper();
			MAE_Sucursal suc = await db.MAE_Sucursal.FindAsync(id);
			ViewBag.Titulo = "Editar Sucursal";
			ViewBag.ClsModal = "hmodal-warning";
			ViewBag.Action = "Edit";
			ViewBag.IdCiudad = new SelectList(db.MAE_ciudad, "IdCiudad", "Ciudad", suc.IdCiudad);
			if (suc.MAE_sujetoEconomico.EsGubernamental)
				ViewBag.IdDireccion = new SelectList(db.MAE_DireccionesMOP, "IdDireccion", "NombreDireccion", suc.IdDireccion);

			if (ValidaPermisos.ValidaPermisosEnController("0010170002"))
			{
				return PartialView("_modalForm", suc);
			}
			else
			{
				return RedirectToAction("SinPermiso", "../Pub");
			}

			
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> Edit([Bind(Include = "IdSucursal,Sucursal,Direccion,Encargado,IdSujeto,IdCiudad,Telefono,EsCentral,Email,IdDireccion")] MAE_Sucursal mAE_Sucursal)
		{
			
			if (ModelState.IsValid)
			{
				MAE_Sucursal sucursal = db.MAE_Sucursal.Find(mAE_Sucursal.IdSucursal);
				sucursal.Sucursal = mAE_Sucursal.Sucursal;
				sucursal.Telefono = mAE_Sucursal.Telefono;
				sucursal.IdSujeto = mAE_Sucursal.IdSujeto;
				sucursal.IdDireccion = mAE_Sucursal.IdDireccion;
				sucursal.IdCiudad = mAE_Sucursal.IdCiudad;
				sucursal.EsCentral = mAE_Sucursal.EsCentral;
				sucursal.Encargado = mAE_Sucursal.Encargado;
				sucursal.Email = mAE_Sucursal.Email;
				sucursal.Direccion = mAE_Sucursal.Direccion;
				db.Entry(sucursal).State = EntityState.Modified;
				await db.SaveChangesAsync();
				return Content(JsonConvert.SerializeObject(new { error = false, idSujeto = mAE_Sucursal.IdSujeto }));
			}
			else
			{
				return Content(JsonConvert.SerializeObject(new { error = true, mensaje = "Ocurrió un problema al tratar de guardar los Datos" }));
			}

		}
		public async Task<ActionResult> Delete(int? id)
		{
			MAE_Sucursal mAE_Sucursal = await db.MAE_Sucursal.FindAsync(id);

			if (ValidaPermisos.ValidaPermisosEnController("0010170003"))
			{
				return PartialView("_Delete", mAE_Sucursal);
			}
			else
			{
				return RedirectToAction("SinPermiso", "../Pub");
			}

			
		}
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> DeleteConfirmed(int IdSucursal)
		{
			try
			{
				MAE_Sucursal mAE_Sucursal = await db.MAE_Sucursal.FindAsync(IdSucursal);
				db.MAE_Sucursal.Remove(mAE_Sucursal);
				await db.SaveChangesAsync();
				return Content(JsonConvert.SerializeObject(new { error = false, idSujeto = mAE_Sucursal.IdSujeto }));
			}
			catch (Exception err)
			{
				return Content(JsonConvert.SerializeObject(new { error = true, mensaje = err.Message }));
			}

		}
		public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MAE_Sucursal mAE_Sucursal = await db.MAE_Sucursal.FindAsync(id);
            if (mAE_Sucursal == null)
            {
                return HttpNotFound();
            }
            return View(mAE_Sucursal);
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
