using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.IO;
using Newtonsoft.Json;
using System.Data.SqlClient;
using LOD_APR.Helpers;
using LOD_APR.Models;
using LOD_APR.Areas.Admin.Models;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity;

namespace LOD_APR.Areas.Admin.Controllers
{
	[CustomAuthorize]
	public class SujetoEconomicoController : Controller
    {
		private ApplicationSignInManager _signInManager;
		private ApplicationUserManager _userManager;
		private ApplicationRoleManager _roleManager;
		private LOD_DB db = new LOD_DB();

		public ApplicationRoleManager RoleManager
		{
			get
			{
				return _roleManager ?? HttpContext.GetOwinContext().Get<ApplicationRoleManager>();
			}
			private set
			{
				_roleManager = value;
			}
		}
		public ApplicationSignInManager SignInManager
		{
			get
			{
				return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
			}
			private set
			{
				_signInManager = value;
			}
		}
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

		public async Task<ActionResult> Index()
        {
            var mAE_sujetoEconomico = db.MAE_sujetoEconomico.Include(m => m.MAE_ciudad).OrderBy(x=>x.RazonSocial);
			//List<MAE_tag> tags = db.MAE_tag.Where(t => t.IdTipoTag == 600000 || t.IdTipoTag == 100000).OrderBy(x => x.IdTipoTag).ToList<MAE_tag>();
			//ViewBag.IdTags = new SelectList(tags, "IdTag", "NomTag", null);

			if (ValidaPermisos.ValidaPermisosEnController("0010000000"))
			{
				return View(await mAE_sujetoEconomico.ToListAsync());
			}
			else
			{
				try
				{
					return RedirectToAction("SinPermiso", "../Pub");
				}catch(Exception ex)
                {
					Console.WriteLine(ex.Message);
					return RedirectToAction("SinPermiso", "../Pub");
				}
			}
        }


		public ActionResult Filtro(Filtro_Sujetos filtro)
		{
			
			var sujetos = db.MAE_sujetoEconomico.AsQueryable();
			sujetos = db.MAE_sujetoEconomico.Where(a => a.Activo == false || a.Activo == true);

			if (filtro.Activo && !filtro.Inactivo)
			{
				sujetos = sujetos.Where(a => a.Activo == true);
			}
			else if (!filtro.Activo && filtro.Inactivo)
			{
				sujetos = sujetos.Where(a => a.Activo == false);
			}

			sujetos = sujetos.Where(a => a.EsGubernamental == filtro.Gubernamental || a.EsContratista== filtro.Contratista || a.EsMandante== filtro.Mandante );

			if (filtro.Gubernamental && !filtro.Contratista && !filtro.Mandante)
			{
				sujetos = sujetos.Where(a => a.EsGubernamental == true);
			}
			else if (filtro.Contratista && !filtro.Gubernamental && !filtro.Mandante)
			{
				sujetos = sujetos.Where(a => a.EsContratista == true);
			}
			else if (filtro.Mandante && !filtro.Gubernamental && !filtro.Contratista)
			{
				sujetos = sujetos.Where(a => a.EsMandante == true);
			}
			else if (!filtro.Contratista && filtro.Gubernamental && filtro.Mandante)
			{
				sujetos = sujetos.Where(a => a.EsGubernamental == true || a.EsMandante == true);
			}
			else if (!filtro.Mandante && filtro.Gubernamental && filtro.Contratista)
			{
				sujetos = sujetos.Where(a => a.EsGubernamental|| a.EsContratista);
			}
			else if ( !filtro.Gubernamental && filtro.Contratista && filtro.Mandante)
			{
				sujetos = sujetos.Where(a => a.EsContratista || a.EsMandante);
			}
		


			//if (filtro.Tags != "null")
			//{
			//	List<int> filtro_tags = JsonConvert.DeserializeObject<List<int>>(filtro.Tags);
			//	List<int> tags_sujetos = db.MAE_tagSujeto.Where(t => filtro_tags.Contains(t.IdTag)).Select(s => s.IdSujEcon).ToList<int>();
			//	sujetos = sujetos.Where(c => tags_sujetos.Contains(c.IdSujEcon));
			//}

			List<MAE_sujetoEconomico> per = sujetos.ToList<MAE_sujetoEconomico>();
			return PartialView("_getTable", per);
		}
		public ActionResult DetailsEmpty()
		{
			return PartialView("_DetailsEmpty");
		}

		//public async Task<ActionResult> getTable(int IdSujeto)
		//{
		//	var mAE_sujetoEconomico = db.Users.Include(m => m.MAE_ciudad);
		//	return PartialView("_getTable",await mAE_sujetoEconomico.ToListAsync());
		//}

		public async Task<ActionResult> getTableUser(int IdSujeto)
		{
			List<MAE_Sucursal> sucursales = await db.MAE_Sucursal.Where(x => x.IdSujeto == IdSujeto).ToListAsync();
			return PartialView("_getTableUsuarios", sucursales);
		}

		public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MAE_sujetoEconomico mAE_sujetoEconomico = await db.MAE_sujetoEconomico.FindAsync(id);
            if (mAE_sujetoEconomico == null)
            {
                return HttpNotFound();
            }
			return PartialView("_Details", mAE_sujetoEconomico);
        }

		public async Task<ActionResult> DetailsUser(string id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			ApplicationUser User = await db.Users.Where(x => x.Id == id).FirstOrDefaultAsync();
			if (User == null)
			{
				return HttpNotFound();
			}
			return PartialView("_DetailsUser", User);
		}

		public ActionResult Create()
        {
            ViewBag.IdCiudad = new SelectList(db.MAE_ciudad, "IdCiudad", "Ciudad");
			MAE_sujetoEconomico sujeto = new MAE_sujetoEconomico();
			sujeto.EsContratista = true;

			if (ValidaPermisos.ValidaPermisosEnController("0010000001"))
			{
				return View(sujeto);
			}
			else
			{
				return RedirectToAction("SinPermiso", "../Pub");
			}

			
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "IdSujEcon,Rut,RazonSocial,NomFantasia" +
			",Giro,Direccion,Telefono,email,web,UrlFacebook,UrlTwitter,UrlLinkedin,IdCiudad,EsMandante," +
			"EsContratista,EsGubernamental,FechaCreacion,RutaImagen,Activo")] MAE_sujetoEconomico mAE_sujetoEconomico, HttpPostedFileBase fileImage)
        {
			string imageError = string.Empty;
			string regError = string.Empty;

			if (ModelState.IsValid)
			{
				var existeRut = db.MAE_sujetoEconomico.Where(s => s.Rut == mAE_sujetoEconomico.Rut).FirstOrDefault();
				if (existeRut != null)
				{
					ModelState.AddModelError("Rut", "El Rut ya se encuentra ingresado");
					goto error;
				}

				if (fileImage != null && fileImage.ContentLength > 0)
				{
					try
					{
						string fileExt = Path.GetExtension(fileImage.FileName);
						string fileName = String.Format("{0}_logo{1}", mAE_sujetoEconomico.Rut.Split('-')[0], fileExt);
						string filePath = Path.Combine(Server.MapPath("~/Images/Sujetos/"), fileName);

						if (System.IO.File.Exists(filePath))
							System.IO.File.Delete(filePath);

						fileImage.SaveAs(filePath);
						mAE_sujetoEconomico.RutaImagen = fileName;
					}
					catch (Exception ex)
					{
						imageError = String.Format("Ocurrió un problema al tratar de guardar la imagen: {0}", ex.Message);
						//nlog
					}

				}

				try
				{
					mAE_sujetoEconomico.DataLetters = Data_Letters.getRandomDataLetter();
					db.MAE_sujetoEconomico.Add(mAE_sujetoEconomico);
					db.SaveChanges();
					
					MAE_Sucursal suc = new MAE_Sucursal()
					{
						EsCentral=true,
						Sucursal="Casa Matriz",
						IdCiudad= Convert.ToInt32(mAE_sujetoEconomico.IdCiudad),
						Direccion=mAE_sujetoEconomico.Direccion,
						IdSujeto= mAE_sujetoEconomico.IdSujEcon,
						Telefono=mAE_sujetoEconomico.Telefono,
						Email= mAE_sujetoEconomico.email
					};
					db.MAE_Sucursal.Add(suc);
					await db.SaveChangesAsync();
					return RedirectToAction("Edit", new { id = mAE_sujetoEconomico.IdSujEcon, fromCreate = true });
				}
				catch (Exception ex)
				{
					regError = String.Format("Ocurrió un problema al tratar de guardar el registro: {0}", ex.Message);
					//nlog
				}

			}
			if (regError != string.Empty)
				ModelState.AddModelError(string.Empty, regError);
			error:
			ViewBag.IdCiudad = new SelectList(db.MAE_ciudad, "IdCiudad", "Ciudad", mAE_sujetoEconomico.IdCiudad);
			return View(mAE_sujetoEconomico);

        }
		public ActionResult Edit(int id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			MAE_sujetoEconomico mAE_sujetoEconomico = db.MAE_sujetoEconomico.Include(t => t.Sucursales).Where(x=>x.IdSujEcon==id).FirstOrDefault();
            if (mAE_sujetoEconomico == null)
            {
                return HttpNotFound();
            }
            ViewBag.IdCiudad = new SelectList(db.MAE_ciudad, "IdCiudad", "Ciudad", mAE_sujetoEconomico.IdCiudad);

			if (ValidaPermisos.ValidaPermisosEnController("0010000002"))
			{
				return View(mAE_sujetoEconomico);
			}
			else
			{
				return RedirectToAction("SinPermiso", "../Pub");
			}

        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "IdSujEcon,Rut,RazonSocial,NomFantasia,Giro,Direccion,Telefono,email,web,UrlFacebook," +
			"UrlTwitter,UrlLinkedin,IdCiudad,EsMandante,EsContratista,EsGubernamental,FechaCreacion,RutaImagen,Activo,DataLetters")] MAE_sujetoEconomico mAE_sujetoEconomico, HttpPostedFileBase fileImage)
        {
			string imageError = string.Empty;
			string regError = string.Empty;
			if (ModelState.IsValid)
			{

				if (fileImage != null && fileImage.ContentLength > 0)
				{
					try
					{
						string fileExt = Path.GetExtension(fileImage.FileName);
						string fileName = String.Format("{0}_logo{1}", mAE_sujetoEconomico.IdSujEcon, fileExt);
						string filePath = Path.Combine(Server.MapPath("~/Images/Sujetos/"), fileName);

						if (System.IO.File.Exists(filePath))
							System.IO.File.Delete(filePath);

						fileImage.SaveAs(filePath);
						mAE_sujetoEconomico.RutaImagen = fileName;
					}
					catch (Exception ex)
					{
						imageError = String.Format("Ocurrió un problema al tratar de guardar la imagen: {0}", ex.Message);
						//nlog
					}

				}

				try
				{
					db.Entry(mAE_sujetoEconomico).State = EntityState.Modified;
					await db.SaveChangesAsync();
					return RedirectToAction("Edit", new { id = mAE_sujetoEconomico.IdSujEcon, fromCreate = true });

				}
				catch (Exception ex)
				{
					regError = String.Format("Ocurrió un problema al tratar de guardar el registro: {0}", ex.Message);
					//nlog
				}

			}
			if (regError != string.Empty)
				ModelState.AddModelError(string.Empty, regError);

			//ViewBag.ContactosSujeto = db.MAE_Contactos.Where(c => c.IdSujeto == mAE_sujetoEconomico.IdSujEcon).ToList();

			//ViewBag.Sucursales = db.MAE_Sucursal.Where(c => c.IdSujeto == mAE_sujetoEconomico.IdSujEcon).Include(c => c.MAE_Contactos).ToList();
			ViewBag.IdCiudad = new SelectList(db.MAE_ciudad, "IdCiudad", "Ciudad", mAE_sujetoEconomico.IdCiudad);
            //ViewBag.Evaluaciones = db.EVA_EvaluacionContrato.Where(e => e.ASP_contratos.IdSujEcon == mAE_sujetoEconomico.IdSujEcon && e.EVA_evaluaciones.EstadosEval == 1).ToList();

            return View(mAE_sujetoEconomico);
        }
		public async Task<ActionResult> Delete(int? id)
		{

			MAE_sujetoEconomico mAE_sujetoEconomico = await db.MAE_sujetoEconomico.FindAsync(id);
			//if (mAE_sujetoEconomico.GAE_activosDue.Any() || !mAE_sujetoEconomico.GAE_activosProv.Any())
			//	return Content(JsonConvert.SerializeObject(new { error = true, msg = Properties.Resources.ERR_Msg_CheckDelete }));

			if (ValidaPermisos.ValidaPermisosEnController("0010000003"))
			{
				return PartialView("_Delete", mAE_sujetoEconomico);
			}
			else
			{
				return RedirectToAction("SinPermiso", "../Pub");
			}

			
		}

		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> DeleteConfirmed(int IdSujEcon)
		{
			try
			{
				MAE_sujetoEconomico mAE_sujetoEconomico = await db.MAE_sujetoEconomico.FindAsync(IdSujEcon);
				if (mAE_sujetoEconomico.RutaImagen != null)
				{
					string filePath = Path.Combine(Server.MapPath("~/Images/Sujetos/"), mAE_sujetoEconomico.RutaImagen);

					if (System.IO.File.Exists(filePath))
						System.IO.File.Delete(filePath);
				}

				db.MAE_sujetoEconomico.Remove(mAE_sujetoEconomico);
				await db.SaveChangesAsync();
				return Content("true");
			}
			catch (SqlException err)
			{
				return Content(err.InnerException.Message);
			}

		}
		public ActionResult ExisteRut(string RUT)
		{

			string existe = "false";
			if (RUT == "") existe = "true";
			if (db.MAE_sujetoEconomico.Where(x => x.Rut == RUT && x.EsGubernamental == false).Count() > 0)
			{
				existe = "true";
			}
			return Content(existe);
		}

		public ActionResult getSujetoEconomicoJson(string q)
        {
            List<MAE_sujetoEconomico> Sujetos = db.MAE_sujetoEconomico.Where(x => x.RazonSocial.ToUpper().Contains(q.ToUpper()) && x.EsContratista == true).ToList();
            List<AutoSearch> search = new List<AutoSearch>();
            foreach (var item in Sujetos)
            {
                search.Add(new AutoSearch()
                {
                    id = item.IdSujEcon.ToString(),
                    name = item.RazonSocial.ToString()
                });
            }
            string json = JsonConvert.SerializeObject(search);
            return Content(json);
        }

		//PREVISUALIZACIÓN DE IMAGENES*************************************
		[HttpPost]
		public ActionResult AddImagenPreview(HttpPostedFileBase fileImage)
		{
			string UrlFoto = string.Empty;
			if (fileImage != null && fileImage.ContentLength > 0)
			{
				if (!fileImage.ContentType.StartsWith("image/"))
				{
					throw new InvalidOperationException("Invalid MIME content type.");
				}

				var extension = Path.GetExtension(fileImage.FileName.ToLowerInvariant());
				string[] extensions = { ".gif", ".jpg", ".png", ".svg", ".webp",".jfif" };
				if (!extensions.Contains(extension))
				{
					throw new InvalidOperationException("Invalid file extension.");
				}
				try
				{
					string fileExt = Path.GetExtension(fileImage.FileName);
					string Name = "ImagenTemporal" + DateTime.Now.Ticks.ToString() + fileExt;

					string filePath = Path.Combine(Server.MapPath("~/Files/Temp/"), Name);
					fileImage.SaveAs(filePath);
					UrlFoto = String.Format("~/Files/Temp/{0}", Name);
				}
				catch (Exception ex)
				{
					return Content(String.Format("Ocurrió un problema al tratar de guardar el archivo: {0}", ex.Message));
				}
			}
			return Content("true;" + UrlFoto);
		}
		public ActionResult getImagenPreview(string url)
		{
			ViewBag.RutaImagenProyecto = url;
			return PartialView("_getImagen");
		}

		public ActionResult getImagenPreviewPersona(string url)
		{
			ViewBag.RutaImagenProyecto = url;
			return PartialView("_getImagenPersona");
		}

		public ActionResult DeleteUser(string Id)
		{			
			ApplicationUser model = UserManager.FindById(Id);

			UserViewModel viewModel = new UserViewModel()
			{
				Id = Id,
				Nombres = model.Nombres,
				Email = model.UserName,
				Apellidos = model.Apellidos

			};
			return PartialView("_DeleteUser", viewModel);
		}

		[HttpPost, ActionName("DeleteUser")]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> DeleteConfirmed(string Id)
		{
			try
			{
				ApplicationUser user = UserManager.FindById(Id);
				int IdSujetoEconomico = await db.MAE_Sucursal.Where(x => x.IdSucursal == user.IdSucursal).Select(x => x.IdSujeto).FirstOrDefaultAsync();
				var roles = user.Roles;
				var listadoUserRoles = await db.UserRoles.Where(x => x.UserId == user.Id).ToListAsync();
				bool resultRol = true;
                foreach (var item in listadoUserRoles)
                {
					db.UserRoles.Remove(item);
					await db.SaveChangesAsync();
                }

				IdentityResult result = await UserManager.DeleteAsync(user);
				if (result.Succeeded)
				{
					return Content($"true;{IdSujetoEconomico}");
				}
				else
				{
					return Content(result.Errors.First().ToString());
				}
			}
			catch (Exception err)
			{
				return Content(err.Message);
			}

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
