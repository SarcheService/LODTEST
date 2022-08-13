using LOD_APR.Areas.Admin.Models;
using LOD_APR.Helpers;
using LOD_APR.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using static LOD_APR.Helpers.HelperEmails;

namespace LOD_APR.Areas.Admin.Controllers
{
    [CustomAuthorize]
    public class UsuariosController : Controller
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

        public ActionResult Index()
        {
            List<MAE_sujetoEconomico> mAE_SujetoEconomico = db.MAE_sujetoEconomico.Where(x => x.Activo).ToList();
            ViewBag.IdSujEcon = new SelectList((from p in mAE_SujetoEconomico
                                            select new
                                            {
                                                IdSujEcon = p.IdSujEcon,
                                                NomFantasia = p.NomFantasia
                                            }),
                                                       "IdSujEcon",
                                                       "NomFantasia");
            if (ValidaPermisos.ValidaPermisosEnController("0010010000"))
            {
                return View(getUsuarios());
            }else
            {
                return RedirectToAction("SinPermiso", "../Pub");
            }
            
        }


        public ActionResult Filtro(Filtro_Usuarios filtro)
        {

            var usuarios = db.Users.AsQueryable();
            if(filtro.IdSujetoEconomico != 0)
                usuarios = db.Users.Where(a => a.Activo == false || a.Activo == true && a.MAE_Sucursal.IdSujeto == filtro.IdSujetoEconomico);
            else
                usuarios = db.Users.Where(a => a.Activo == false || a.Activo == true);

            if (filtro.Activo && !filtro.Inactivo)
            {
                usuarios = usuarios.Where(a => a.Activo == true);
            }
            else if (!filtro.Activo && filtro.Inactivo)
            {
                usuarios = usuarios.Where(a => a.Activo == false);
            }

            usuarios = usuarios.Where(a => a.MAE_Sucursal.MAE_sujetoEconomico.EsGubernamental == filtro.Gubernamental || a.MAE_Sucursal.MAE_sujetoEconomico.EsContratista == filtro.Contratista || a.MAE_Sucursal.MAE_sujetoEconomico.EsMandante == filtro.Mandante);

            if (filtro.Gubernamental && !filtro.Contratista && !filtro.Mandante)
            {
                usuarios = usuarios.Where(a => a.MAE_Sucursal.MAE_sujetoEconomico.EsGubernamental == true);
            }
            else if (filtro.Contratista && !filtro.Gubernamental && !filtro.Mandante)
            {
                usuarios = usuarios.Where(a => a.MAE_Sucursal.MAE_sujetoEconomico.EsContratista == true);
            }
            else if (filtro.Mandante && !filtro.Gubernamental && !filtro.Contratista)
            {
                usuarios = usuarios.Where(a => a.MAE_Sucursal.MAE_sujetoEconomico.EsMandante == true);
            }
            else if (!filtro.Contratista && filtro.Gubernamental && filtro.Mandante)
            {
                usuarios = usuarios.Where(a => a.MAE_Sucursal.MAE_sujetoEconomico.EsGubernamental == true || a.MAE_Sucursal.MAE_sujetoEconomico.EsMandante == true);
            }
            else if (!filtro.Mandante && filtro.Gubernamental && filtro.Contratista)
            {
                usuarios = usuarios.Where(a => a.MAE_Sucursal.MAE_sujetoEconomico.EsGubernamental || a.MAE_Sucursal.MAE_sujetoEconomico.EsContratista);
            }
            else if (!filtro.Gubernamental && filtro.Contratista && filtro.Mandante)
            {
                usuarios = usuarios.Where(a => a.MAE_Sucursal.MAE_sujetoEconomico.EsContratista || a.MAE_Sucursal.MAE_sujetoEconomico.EsMandante);
            }



            //if (filtro.Tags != "null")
            //{
            //	List<int> filtro_tags = JsonConvert.DeserializeObject<List<int>>(filtro.Tags);
            //	List<int> tags_sujetos = db.MAE_tagSujeto.Where(t => filtro_tags.Contains(t.IdTag)).Select(s => s.IdSujEcon).ToList<int>();
            //	usuarios = usuarios.Where(c => tags_sujetos.Contains(c.IdSujEcon));
            //}

            List<ApplicationUser> per = usuarios.ToList<ApplicationUser>();
            return PartialView("_getTableUsers", per);
        }


        public ActionResult getTableUsers()
        {
            return PartialView("_getTableUsers", getUsuarios());
        }
        public List<ApplicationUser> getUsuarios()
        {
            List<ApplicationUser> usuarios = new List<ApplicationUser>();
            string userId = User.Identity.GetUserId();
            //string empresaUser = db.Users.Find(userId).IdEmpresa;
            var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
            var isInRole = UserManager.IsInRole(User.Identity.GetUserId(), "SuperAdmin");
            if (isInRole)
            {
                usuarios = db.Users.ToList();
            }
            else
            {
                usuarios = db.Users.ToList();
                // List<string> usuariosList = db.LoginEmpresa.Where(u => u.IdEmpresa == empresaUser).Select(s => s.IdUser).ToList();
                // usuarios = db.Users.Where(u => usuariosList.Contains(u.Id)).ToList();
            }

            return usuarios;
        }



        //ER**
        public ActionResult ExisteRut(string RUN, int IdSujeto)
        {
            string existe = "false";
            if (RUN == "") existe = "true";
            if (db.Users.Where(x => x.Run == RUN && x.MAE_Sucursal.IdSujeto == IdSujeto || (x.Run == RUN && x.Activo)).Count() > 0)
            {
                existe = "true";
            }
            return Content(existe);
        }

        public ActionResult ExisteCorreo(string Correo)
        {
            string existe = "false";
            if (Correo == "") existe = "true";
            if (db.Users.Where(x => x.Email == Correo).Count() > 0)
            {
                existe = "true";
            }
            return Content(existe);
        }

        //ER**
        public ActionResult Register(int IdSujEcon)
        {
            RegisterViewModel model = new RegisterViewModel();
            MAE_sujetoEconomico Suj = db.MAE_sujetoEconomico.Find(IdSujEcon);
            List<ApplicationRole> roles = new List<ApplicationRole>(); ;
            if (Suj.EsContratista)
            { 
               roles = db.Roles.Where(x => x.IsContratista == true).OrderBy(x => x.Name).ToList();
            }
            else if (Suj.EsMandante) 
            {
               roles = db.Roles.Where(x => x.IsFiscalizador == true).OrderBy(x => x.Name).ToList();
            } 
            else 
            {
                roles = db.Roles.Where(x => x.IsGubernamental == true).OrderBy(x => x.Name).ToList();
            }

            ViewBag.IdRoles = new SelectList(roles, "Id", "Name", null);

            //Preguntar a Jhon donde dejara la configuración por defecto de la aplicación
            MAE_Empresa empresa = db.MAE_Empresa.Find(1);
            //En este caso lo sacamos de empresa 1

            string pass= HelperPassword.GenerateRandomPassword(empresa);
            model.Password = pass;
            model.ConfirmPassword= pass;
            int IdSucMatriz= Convert.ToInt32(db.MAE_Sucursal.Where(x=>x.IdSujeto==IdSujEcon && x.EsCentral).FirstOrDefault().IdSucursal);
            ViewBag.Sujeto = Suj;
            ViewBag.IdSucursal = new SelectList(db.MAE_Sucursal.Where(s => s.IdSujeto == IdSujEcon).ToList(), "IdSucursal", "Sucursal", IdSucMatriz);

            if (ValidaPermisos.ValidaPermisosEnController("0010010001"))
            {
                return View(model);
            }
            else
            {
                return RedirectToAction("SinPermiso", "../Pub");
            }
 
        }

        //ER**
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model, HttpPostedFileBase fileImage)
        {
            string imageError = string.Empty;
            if (ModelState.IsValid && model.IdRoles != null)
            {
                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    Activo = model.Activo,
                    EmailConfirmed = true,
                    LockoutEnabled = true,
                    Run = model.Run,
                    Nombres = model.Nombres,
                    Apellidos = model.Apellidos,
                    CargoContacto = model.CargoContacto,
                    Telefono = model.Telefono,
                    Movil = model.Movil,
                    AnexoEmpresa = model.AnexoEmpresa,
                    DataLetters = Data_Letters.getRandomDataLetter(),
                    IdSucursal=model.IdSucursal,
                    IdCertificado = model.IdCertificado,
                    FechaCreacion = DateTime.Now
                };
                //Cargar La imagen//
                if (fileImage != null && fileImage.ContentLength > 0)
                {
                    try
                    {
                        string fileExt = Path.GetExtension(fileImage.FileName);
                        string fileName = String.Format("{0}_logo{1}", model.Run.Split('-')[0], fileExt);
                        string filePath = Path.Combine(Server.MapPath("~/Images/Contactos/"), fileName);
                        
                        if (System.IO.File.Exists(filePath))
                            System.IO.File.Delete(filePath);

                        fileImage.SaveAs(filePath);
                        user.RutaImagen = fileName;
                    }
                    catch (Exception ex)
                    {
                        imageError = String.Format("Ocurrió un problema al tratar de guardar la imagen: {0}", ex.Message);
                        //nlog
                    }

                }                                                                         

                bool modoTest = Convert.ToBoolean(ConfigurationManager.AppSettings.Get("ModoTest"));
                if (modoTest)
                {
                    user.EmailConfirmed = true;
                    user.Activo = true;
                    model.Password = "Atacama.2022";
                }

                var result = await UserManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    foreach (var IdRol in model.IdRoles)        
                    {
                        string NameRol= db.Roles.Find(IdRol).Name;
                        result = await UserManager.AddToRoleAsync(user.Id, NameRol);

                    }

                    string codeReset = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                    var callbackUrl = Url.Action("ResetPassword", "Account", new { area = "",code = codeReset, tipo = 1, IdUser = user.Id }, protocol: Request.Url.Scheme);

                    //Preparar Email para Con Claves y activación                   
                    string correoformateado = formatearCorreo(0, model.Nombres, model.Email, 
                        "Para confirmar la cuenta y crear contraseña, haga clic <a href=\"" + callbackUrl + "\">aquí</a>");

                    await UserManager.SendEmailAsync(user.Id, "Confirmar cuenta", correoformateado);

                    if (ValidaPermisos.ValidaPermisosEnController("0010010001"))
                    {
                        return RedirectToAction("Edit", "Usuarios", new { Id = user.Id, back = "2" });
                    }
                    else
                    {
                        return RedirectToAction("SinPermiso", "../Pub");
                    }

                    
                }
                AddErrors(result);
            }

            //List<SelectListItem> roles = new List<SelectListItem>();
            //List<SelectListItem> roles = new List<SelectListItem>();
            //var user_roles = db.Roles.ToList();
            //var isInRole = UserManager.IsInRole(User.Identity.GetUserId(), "SuperAdmin");
            //foreach (var role in user_roles)
            //{
            //    if (role.Name == "SuperAdmin")
            //    {
            //        if (isInRole)
            //            roles.Add(new SelectListItem() { Value = role.Name, Text = role.Name });
            //    }
            //    else
            //    {
            //        roles.Add(new SelectListItem() { Value = role.Name, Text = role.Name });
            //    }
            //}

            MAE_Sucursal mAE_Sucursal = db.MAE_Sucursal.Find(model.IdSucursal);
            MAE_sujetoEconomico Suj = db.MAE_sujetoEconomico.Find(mAE_Sucursal.IdSujeto);
            List<ApplicationRole> roles = new List<ApplicationRole>(); ;
            if (Suj.EsContratista)
            {
                roles = db.Roles.Where(x => x.IsContratista == true).OrderBy(x => x.Name).ToList();
            }
            else if (Suj.EsMandante)
            {
                roles = db.Roles.Where(x => x.IsFiscalizador == true).OrderBy(x => x.Name).ToList();
            }
            else
            {
                roles = db.Roles.Where(x => x.IsGubernamental == true).OrderBy(x => x.Name).ToList();
            }

            ViewBag.IdRoles = new SelectList(roles, "Id", "Name", null);

            ViewBag.Roles = roles;


            MAE_Empresa empresa = db.MAE_Empresa.Find(1);
            string pass = HelperPassword.GenerateRandomPassword(empresa);
            model.Password = pass;
            model.ConfirmPassword = pass;
            int IdSucMatriz = Convert.ToInt32(db.MAE_Sucursal.Where(x => x.IdSujeto == Suj.IdSujEcon && x.EsCentral).FirstOrDefault().IdSucursal);
            ViewBag.Sujeto = Suj;
            ViewBag.IdSucursal = new SelectList(db.MAE_Sucursal.Where(s => s.IdSujeto == Suj.IdSujEcon).ToList(), "IdSucursal", "Sucursal", IdSucMatriz);

            // Si llegamos a este punto, es que se ha producido un error y volvemos a mostrar el formulario

            if (ValidaPermisos.ValidaPermisosEnController("0010010001"))
            {
                return View(model);
            }
            else
            {
                return RedirectToAction("SinPermiso", "../Pub");
            }

        }


        private bool ErolarRegistro(int tipo, int IdOrigen,string UserId)
        {
            try 
            { 
            //{
            //        SEG_UserContacto userContacto = new SEG_UserContacto()
            //        {
            //            IdContacto = IdOrigen,
            //            UserId = UserId
            //        };
            //        db.SEG_UserContacto.Add(userContacto);
                
                db.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public ActionResult Edit(string Id, string back)
        {
            ApplicationUser model = UserManager.FindById(Id);
            MAE_Sucursal sucursal = db.MAE_Sucursal.Find(model.IdSucursal);
            MAE_sujetoEconomico Suj = db.MAE_sujetoEconomico.Find(sucursal.IdSujeto);
            ViewBag.Sujeto = Suj;
            var userStore = new UserStore<ApplicationUser>(db);
            var userManager = new UserManager<ApplicationUser>(userStore);
            List<string> roles1 = userManager.GetRoles(Id).ToList();
            List<ApplicationRole> roles = new List<ApplicationRole>();
            if (Suj.EsContratista)
            {
                roles = db.Roles.Where(x => x.IsContratista == true).OrderBy(x => x.Name).ToList();
            }
            else if (Suj.EsMandante)
            {
                roles = db.Roles.Where(x => x.IsFiscalizador == true).OrderBy(x => x.Name).ToList();
            }
            else
            {
                roles = db.Roles.Where(x => x.IsGubernamental == true).OrderBy(x => x.Name).ToList();
            }

            UserViewModel viewModel = new UserViewModel()
            {
                Id = Id,
                Email = model.Email,
                Activo = model.Activo,
                Run = model.Run,
                Nombres = model.Nombres,
                Apellidos = model.Apellidos,
                CargoContacto = model.CargoContacto,
                Telefono = model.Telefono,
                Movil = model.Movil,
                AnexoEmpresa = model.AnexoEmpresa,
                DataLetters = Data_Letters.getRandomDataLetter(),
                RutaImagen = model.RutaImagen,
                IdSucursal = model.IdSucursal,
                back = back,
                IdCertificado = model.IdCertificado
                //Password = model.PasswordHash
                
               
                //sis

            };
            

            ViewBag.IdSucursal = new SelectList(db.MAE_Sucursal.Where(s => s.IdSujeto == Suj.IdSujEcon).ToList(), "IdSucursal", "Sucursal", model.IdSucursal);
            var IdRolesAux = new List<ApplicationRole>();
            foreach (var item in roles1)
            {
                IdRolesAux.Add(db.Roles.Where(x => x.Name.Equals(item)).FirstOrDefault());
            }
            string[] arrRoles = IdRolesAux.Select(x => x.Id).ToArray();

            ViewBag.IdRoles = new MultiSelectList(roles, "Id", "Name", arrRoles); 
            //ViewBag para Volver Donde Corresponda*******
            
            ViewBag.back = back;
            ViewBag.Tipo = 0;
            ViewBag.IdRegreso = viewModel.Id;
            if (back.Equals("2"))
                ViewBag.IdRegreso = sucursal.IdSujeto;


            //*******************************************************************************


            if (ValidaPermisos.ValidaPermisosEnController("0010010002"))
            {
                return View(viewModel);
            }
            else
            {
                return RedirectToAction("SinPermiso", "../Pub");
            }
            

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(UserViewModel model, HttpPostedFileBase fileImage)
        {
            ApplicationUser user = UserManager.FindById(model.Id);

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            MAE_Sucursal sucursal = db.MAE_Sucursal.Find(model.IdSucursal);
            MAE_sujetoEconomico Suj = db.MAE_sujetoEconomico.Find(sucursal.IdSujeto);
            ViewBag.Sujeto = Suj;
            //ViewBag para Volver Donde Corresponda*******
            ViewBag.back = model.back;
            ViewBag.Tipo = 0;
            ViewBag.IdRegreso = user.Id;
            if (model.back.Equals("2"))
                ViewBag.IdRegreso = sucursal.IdSujeto;


            var userStore = new UserStore<ApplicationUser>(db);
            var userManager = new UserManager<ApplicationUser>(userStore);
            List<string> roles1 = userManager.GetRoles(user.Id).ToList();
            List<ApplicationRole> roles = new List<ApplicationRole>();
            if (Suj.EsContratista)
            {
                roles = db.Roles.Where(x => x.IsContratista == true).OrderBy(x => x.Name).ToList();
            }
            else if (Suj.EsMandante)
            {
                roles = db.Roles.Where(x => x.IsFiscalizador == true).OrderBy(x => x.Name).ToList();
            }
            else
            {
                roles = db.Roles.Where(x => x.IsGubernamental == true).OrderBy(x => x.Name).ToList();
            }

            ViewBag.IdSucursal = new SelectList(db.MAE_Sucursal.Where(s => s.IdSujeto == Suj.IdSujEcon).ToList(), "IdDireccion", "Sucursal", model.IdSucursal);
            var IdRolesAux = new List<ApplicationRole>();
            foreach (var item in roles1)
            {
                IdRolesAux.Add(db.Roles.Where(x => x.Name.Equals(item)).FirstOrDefault());
            }
            string[] arrRoles = IdRolesAux.Select(x => x.Id).ToArray();

            ViewBag.IdRoles = new MultiSelectList(roles, "Id", "Name", arrRoles);


            

            string imageError = string.Empty;


           

            if (model.Email != user.Email)
                user.EmailConfirmed = false;

            user.Email = model.Email;
            user.Nombres = model.Nombres;
            user.Activo = model.Activo;
            user.PhoneNumber = model.Telefono;
            user.Apellidos = model.Apellidos;
            user.CargoContacto = model.CargoContacto;
            user.Movil = model.Movil;
            user.Telefono = model.Telefono;
            user.IdSucursal = model.IdSucursal;
            user.IdCertificado = model.IdCertificado;


            //Cargar La imagen//
            if (fileImage != null && fileImage.ContentLength > 0)
            {
                try
                {
                    string fileExt = Path.GetExtension(fileImage.FileName);
                    string fileName = String.Format("{0}_logo{1}", model.Run.Split('-')[0], fileExt);
                    string filePath = Path.Combine(Server.MapPath("~/Images/Contactos/"), fileName);

                    if (System.IO.File.Exists(filePath))
                        System.IO.File.Delete(filePath);

                    fileImage.SaveAs(filePath);
                    user.RutaImagen = fileName;
                }
                catch (Exception ex)
                {
                    imageError = String.Format("Ocurrió un problema al tratar de guardar la imagen: {0}", ex.Message);
                    //nlog
                }

            }
            else
            {
                user.RutaImagen = user.RutaImagen;
            }

            string[] allUserRoles = UserManager.GetRoles(user.Id).ToArray();
            UserManager.RemoveFromRoles(user.Id, allUserRoles);
            foreach (var IdRol in model.IdRoles)
            {
                string NameRol = db.Roles.Find(IdRol).Name;
                var infoResult = await UserManager.AddToRoleAsync(user.Id, NameRol);
            }

            IdentityResult result = await UserManager.UpdateAsync(user);

            

            return RedirectToAction("Edit", "Usuarios", new { Id = user.Id, back = "2" });

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
                string[] extensions = { ".gif", ".jpg", ".png", ".svg", ".webp", ".jfif" };
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

        public ActionResult Delete(string Id)
        {
            ApplicationUser model = UserManager.FindById(Id);

            UserViewModel viewModel = new UserViewModel()
            {
                Id = Id,
                Nombres = model.Nombres,
                Email = model.UserName,
                Apellidos = model.Apellidos

            };

            if (ValidaPermisos.ValidaPermisosEnController("0010010003"))
            {
                return PartialView("_Delete", viewModel);
            }
            else
            {
                return RedirectToAction("SinPermiso", "../Pub");
            }

            
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string Id)
        {
            try
            {
                ApplicationUser user = UserManager.FindById(Id);
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
                    return Content("true");
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

        //SET NEW PASSWORD********************************
        public async Task<ActionResult> SetPassword(string userId)
        {
            SetPasswordViewModel model = new SetPasswordViewModel() { Id = userId };
            var user = await UserManager.FindByIdAsync(userId);
            ViewBag.Usuario = user.Nombres;
            MAE_Empresa empresa = db.MAE_Empresa.Find(1);
            string pass = HelperPassword.GenerateRandomPassword(empresa);
            model.NewPassword = pass;
            model.ConfirmPassword = pass;
            return PartialView("SetPassword", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SetPassword(SetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var eliminacion = await UserManager.RemovePasswordAsync(model.Id);
                if (eliminacion.Succeeded)
                {
                    var result = await UserManager.AddPasswordAsync(model.Id, model.NewPassword);
                    if (result.Succeeded)
                    {
                        //Preparar Email para Con Claves y activación
                        //*****
                        string codeReset = await UserManager.GeneratePasswordResetTokenAsync(model.Id);
                        var callbackUrl = Url.Action("ResetPassword", "Account", new { area = "", code = codeReset, tipo = 0, IdUser = model.Id }, protocol: Request.Url.Scheme);
                        //var Url = Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath.TrimEnd('/');
                        var user = db.Users.Find(model.Id);
                        string correoformateado = formatearCorreo(1, user.Nombres, user.Email,  
                            "Para completar la operación debe crear nueva contraseña, haciendo clic <a href=\"" + callbackUrl + "\">aquí</a>");

                        await UserManager.SendEmailAsync(user.Id, "Desbloqueo de cuenta", correoformateado);
                        //var user = await UserManager.FindByIdAsync(model.Id);
                        //if (user != null)
                        //{
                        //	await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                        //}



                        return Content("true");
                    }
                    return Content(result.Errors.First());
                }
                return Content(eliminacion.Errors.First());
            }

            // Si llegamos a este punto, es que se ha producido un error, volvemos a mostrar el formulario
            return Content("Ocurrio un error inesperado");
        }
        //**********************************************

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        // Formato Correo Para Password
        public string formatearCorreo(int tipo, string nombre, string mail, string cuerpo)
        {
            string titulo = ConfigurationManager.AppSettings["AsuntoEmail"];
            string subtitulo = string.Empty;
            string encabezado = string.Empty;
            List<ItemCorreo> items = new List<ItemCorreo>();
            switch (tipo)
            {
                case 0: //Creación de Nueva Cuenta
                       subtitulo = "Confirmación de Cuenta de Usuario";
                       encabezado = "Estimado "+ nombre.ToUpper() + ", se le ha creado la siguiente cuenta en el sistema:";
                        items.Add(new ItemCorreo() { Item = "Usuario     :", Descripcion = mail });
                    break;
                case 1: //Creación de Nueva Cuenta
                    subtitulo = "Desbloqueo de Cuenta de Usuario";
                    encabezado = "Estimado " + nombre.ToUpper() + ", se le ha desbloqueado su cuenta del sistema:";
                    items.Add(new ItemCorreo() { Item = "Usuario     :", Descripcion = mail });
                    break;
            }

            ViewBag.Ruta = Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath.TrimEnd('/') + db.MAE_Empresa.Find(12).LogoData.Substring(1);
            ViewBag.Titulo = titulo;
            ViewBag.Subtitulo = subtitulo;
            ViewBag.Encabezado = encabezado;
            ViewBag.Items = items;
            ViewBag.Cuerpo = cuerpo; 

            try
            {
                var html = PartialView("_Correos").RenderToString();
                return html;
            }
            catch
            {
                return "";
            }


        }

        public async Task<ActionResult> DesactivarUsuario (string RUN)
        {
            ApplicationUser user = await db.Users.Where(x => x.Run.Equals(RUN)).FirstOrDefaultAsync();
            return PartialView("_DesactivarUsuario", user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DesactivarUsuario (ApplicationUser user)
        {
            try
            {
                ApplicationUser userDesactivado = await db.Users.Where(x => x.Id == user.Id).FirstOrDefaultAsync();
                userDesactivado.Activo = false;
                db.Entry(userDesactivado).State = EntityState.Modified;
                await db.SaveChangesAsync();

                return Content("true");
            }catch(Exception ex)
            {
                return Content(ex.Message);
            }
        }

        public async Task<ActionResult> ExisteUsuarioActivo(string RUN,string IdUser, string check)
        {
            try
            {
                ApplicationUser user = db.Users.Find(IdUser);
                ApplicationUser usuarioActivo = db.Users.Where(x => x.Run.Equals(RUN) && x.Id != user.Id && x.Activo).FirstOrDefault();
                if (usuarioActivo != null && check.Equals("False"))
                {
                    return Content("true");
                }
                else if(usuarioActivo == null && check.Equals("False"))
                {
                    ApplicationUser userActivado = await db.Users.Where(x => x.Id == IdUser).FirstOrDefaultAsync();
                    userActivado.Activo = true;
                    db.Entry(userActivado).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                    return Content("Usuario Activo");
                }
                else
                {
                    return Content("false");
                }
            }catch(Exception ex)
            {
                return Content(ex.Message);
            }
            
        }

        public async Task<ActionResult> ActivarUsuario(string IdUser)
        {
            ApplicationUser user = await db.Users.Where(x => x.Id == IdUser).FirstOrDefaultAsync();
            return PartialView("_ActivarUsuario", user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ActivarUsuario(ApplicationUser user)
        {
            ApplicationUser userActivado = await db.Users.Where(x => x.Id == user.Id).FirstOrDefaultAsync();

            try
            {
                List<ApplicationUser> usuarios = await db.Users.Where(x => x.Run.Equals(userActivado.Run) && x.Activo).ToListAsync();
                foreach (var item in usuarios)
                {
                    item.Activo = false;
                    db.Entry(item).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                }

                userActivado.Activo = true;
                db.Entry(userActivado).State = EntityState.Modified;

                await db.SaveChangesAsync();

                return Content("true");
            }catch(Exception ex)
            {
                return Content(ex.Message);
            }
        }


        public async Task<ActionResult> DesUsuario(string IdUser)
        {
            try
            {
                ApplicationUser userDesactivado = await db.Users.Where(x => x.Id == IdUser).FirstOrDefaultAsync();
                userDesactivado.Activo = false;
                db.Entry(userDesactivado).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return Content("true");
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
        }

        public async Task<ActionResult> DetailsUser(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ApplicationUser User = db.Users.Where(x => x.Id == id).FirstOrDefault();
            if (User == null)
            {
                return HttpNotFound();
            }
            return PartialView("_DetailsUser", User);
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