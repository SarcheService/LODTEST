using LOD_APR.Areas.Admin.Models;
using LOD_APR.Helpers;
using LOD_APR.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace LOD_APR.Controllers
{
    [CustomAuthorize]
    public class AccountController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private LOD_DB db = new LOD_DB();
        clsSession se = new clsSession();

        public AccountController()
        {
            var version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            string versionFormat = string.Format("{0}.{1}.{2}", version.Major, version.Minor, version.Build);
            ViewBag.version = versionFormat;

            //RH 23-04-2019[*MODIFICAR*]
            MAE_Empresa MAE_Empresa = db.MAE_Empresa.Find(1);
            ViewBag.Logo = MAE_Empresa.LogoData;
            //FIN RH 23-04-2019
            //Jg 04-01-2020
            List<DropDownList> LogosEmpresas = new List<DropDownList>();
            LogosEmpresas.Add(new DropDownList { Label = "Aguas del Altiplano", Value = "/Images/Empresas/Altiplano.jpg" });
            LogosEmpresas.Add(new DropDownList { Label = "Aguas Araucania", Value = "/Images/Empresas/Araucania.png" });
            LogosEmpresas.Add(new DropDownList { Label = "Aguas Chañar", Value = "/Images/Empresas/76850128-9.png" });
            LogosEmpresas.Add(new DropDownList { Label = "Aguas Nuevas", Value = "/Images/Empresas/AguasNuevas.jpg" });
            LogosEmpresas.Add(new DropDownList { Label = "Ener Nuevas", Value = "/Images/Empresas/EnerNuevas.jpg" });
            LogosEmpresas.Add(new DropDownList { Label = "Aguas Magallanes", Value = "/Images/Empresas/Magallanes.png" });

            ViewBag.LogosEmpresas = new SelectList(LogosEmpresas.OrderBy(x => x.Label), "Value", "Label", LogosEmpresas[2].Value);
            //FIN Jg 04-01-2020
        }

        public class DropDownList
        {
            public string Value { get; set; }
            public string Label { get; set; }
        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
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

        [AllowAnonymous]
        public ActionResult Login(string returnUrl, bool errorADFS = false, int e = 0)
        {
            ViewBag.Empresas = db.MAE_Empresa.OrderByDescending(x => x.EmpresaLiderGrupo).ThenBy(x => x.NomFantasia).ToList();
            ViewBag.CollapseIn = false;
            ViewBag.errorADFS = errorADFS;
            ViewBag.ReturnUrl = returnUrl;
            //if (e == 1)
            //{
            //    ViewBag.ErrorEnGalena = "El personal no esta creado en Galena, o no se encuentra activo";
            //}else if(e==2)
            //{
            //    ViewBag.ErrorEnGalena = "El personal no tiene cuenta asociada";
            //}

            string siteKey = Convert.ToString(ConfigurationManager.AppSettings.Get("CaptchaSiteKey"));
            ViewBag.SiteKey = siteKey;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            ViewBag.errorADFS = false;
            ViewBag.Empresas = db.MAE_Empresa;
            ViewBag.CollapseIn = true;

            ApplicationUser user = new ApplicationUser();
            bool modoTest = Convert.ToBoolean(ConfigurationManager.AppSettings.Get("ModoTest"));

            if (modoTest)
            {
                if (new EmailAddressAttribute().IsValid(model.Usuario))
                {
                    user = UserManager.FindByName(model.Usuario);
                }
                else
                {
                    user = db.Users.Where(x => x.Run.Replace(".", "").Equals(model.Usuario) && x.Activo).FirstOrDefault();
                    if (user != null)
                        model.Usuario = user.UserName;
                }
            }
            else
            {
                user = db.Users.Where(x => x.Run.Replace(".", "").Equals(model.Usuario) && x.Activo).FirstOrDefault();
                if (user != null)
                    model.Usuario = user.UserName;
            }

            var response = Request["g-recaptcha-response"];
            string secretKey = Convert.ToString(ConfigurationManager.AppSettings.Get("CaptchaSecret"));
            var client = new WebClient();
            string webCaptcha = Convert.ToString(ConfigurationManager.AppSettings.Get("webCaptcha"));
            var result1 = "";

            try
            {
                result1 = client.DownloadString(string.Format(webCaptcha, secretKey, response));
            }catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                ModelState.AddModelError(string.Empty, $"Se ha producido un error al validar el reCaptcha.");
                return View(model);
            }

            var obj = JObject.Parse(result1);
            var status = (bool)obj.SelectToken("success");

            string siteKey = Convert.ToString(ConfigurationManager.AppSettings.Get("CaptchaSiteKey"));
            ViewBag.SiteKey = siteKey;

            if (!status)
            {
                {
                    ModelState.AddModelError(string.Empty, $"Debe Completar el reCaptcha.");
                    return View(model);
                }
            }

            if (user == null)
            {
                ModelState.AddModelError(string.Empty, $"Intento de inicio de sesión no válido. Verifique su usuario y Contraseña.");
                return View(model);
            }

            if (!user.Activo)
            {
                ModelState.AddModelError(string.Empty, $"Intento de inicio de sesión no válido. Verifique su usuario y Contraseña.");
                return View(model);
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }


            if (modoTest)
            {
                try
                {
                    SignInManager.SignIn(user, true, true);
                    return RedirectToLocal(returnUrl);
                }
                catch
                {
                    ModelState.AddModelError(string.Empty, $"Ocurrió un problema al tratar de ingresar al sistema.");
                    return View(model);
                }
            }
            else
            {
                var result = await SignInManager.PasswordSignInAsync(model.Usuario, model.Password, model.RememberMe, shouldLockout: true);

                switch (result)
                {
                    case SignInStatus.Success:
                        user.AccessFailedCount = 0;
                        db.Entry(user).State = EntityState.Modified;
                        await db.SaveChangesAsync();
                        return RedirectToLocal(returnUrl);
                    case SignInStatus.LockedOut:
                        return View("LockScreen");
                    case SignInStatus.RequiresVerification:
                        return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                    case SignInStatus.Failure:
                        user.AccessFailedCount = user.AccessFailedCount + 1;
                        db.Entry(user).State = EntityState.Modified;
                        await db.SaveChangesAsync();
                        ModelState.AddModelError("", "Intento de inicio de sesión no válido. Verifique su usuario y Contraseña.");
                        return View(model);
                    default:
                        ModelState.AddModelError("", "Intento de inicio de sesión no válido. Verifique su usuario y Contraseña.");
                        return View(model);
                }
            }

        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie, DefaultAuthenticationTypes.ExternalCookie);
            return RedirectToAction("Login", new { errorADFS = false });
        }

        [AllowAnonymous]
        public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe)
        {
            if (!await SignInManager.HasBeenVerifiedAsync())
            {
                return View("Error");
            }
            return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // El código siguiente protege de los ataques por fuerza bruta a los códigos de dos factores. 
            // Si un usuario introduce códigos incorrectos durante un intervalo especificado de tiempo, la cuenta del usuario 
            // se bloqueará durante un período de tiempo especificado. 
            // Puede configurar el bloqueo de la cuenta en IdentityConfig
            var result = await SignInManager.TwoFactorSignInAsync(model.Provider, model.Code, isPersistent: model.RememberMe, rememberBrowser: model.RememberBrowser);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(model.ReturnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Código no válido.");
                    return View(model);
            }
        }

        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                    // Enviar correo electrónico con este vínculo
                    // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    // var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                    // await UserManager.SendEmailAsync(user.Id, "Confirmar cuenta", "Para confirmar la cuenta, haga clic <a href=\"" + callbackUrl + "\">aquí</a>");
                    return RedirectToAction("Index", "Home");
                }
                AddErrors(result);
            }

            // Si llegamos a este punto, es que se ha producido un error y volvemos a mostrar el formulario
            return View(model);
        }

        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
            var result = await UserManager.ConfirmEmailAsync(userId, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                bool modoTest = Convert.ToBoolean(ConfigurationManager.AppSettings.Get("ModoTest"));
                var user = new ApplicationUser();

                if (modoTest)
                {
                    if (new EmailAddressAttribute().IsValid(model.Email))
                    {
                        user = UserManager.FindByName(model.Email);
                    }
                    else
                    {
                        user = db.Users.Where(x => x.Run.Replace(".", "").Equals(model.Email) && x.Activo).FirstOrDefault();
                        if (user != null)
                            model.Email = user.UserName;
                    }
                }
                else
                {
                    user = db.Users.Where(x => x.Run.Replace(".", "").Equals(model.Email) && x.Activo).FirstOrDefault();
                    if (user != null)
                        model.Email = user.UserName;
                }

                if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
                {
                    // No revelar que el usuario no existe o que no está confirmado
                    return View("ForgotPasswordConfirmation");
                }

                string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                await UserManager.SendEmailAsync(user.Id, "Restablecer contraseña", "Para restablecer la contraseña, haga clic <a href=\"" + callbackUrl + "\">aquí</a>");
                return RedirectToAction("ForgotPasswordConfirmation", "Account");
            }

            // Si llegamos a este punto, es que se ha producido un error y volvemos a mostrar el formulario
            return View(model);
        }

        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult ResetPassword(string code)
        {
            return code == null ? View("Error") : View();
        }



        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            bool modoTest = Convert.ToBoolean(ConfigurationManager.AppSettings.Get("ModoTest"));
            var user = new ApplicationUser();

            if (modoTest)
            {
                if (new EmailAddressAttribute().IsValid(model.Email))
                {
                    user = UserManager.FindByName(model.Email);
                }
                else
                {
                    user = db.Users.Where(x => x.Run.Replace(".", "").Equals(model.Email) && x.Activo).FirstOrDefault();
                    if (user != null)
                        model.Email = user.UserName;
                }
            }
            else
            {
                user = db.Users.Where(x => x.Run.Replace(".", "").Equals(model.Email) && x.Activo).FirstOrDefault();
                if (user != null)
                    model.Email = user.UserName;
            }

            if (user == null)
            {
                // No revelar que el usuario no existe
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }

            AddErrors(result);
            return View();
        }

        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        [AllowAnonymous]
        public async Task<ActionResult> SendCode(string returnUrl, bool rememberMe)
        {
            var userId = await SignInManager.GetVerifiedUserIdAsync();
            if (userId == null)
            {
                return View("Error");
            }
            var userFactors = await UserManager.GetValidTwoFactorProvidersAsync(userId);
            var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
            return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SendCode(SendCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            // Generar el token y enviarlo
            if (!await SignInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
            {
                return View("Error");
            }
            return RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
        }

        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        [HttpPost]
        public async Task<JsonResult> SessionRenew()
        {

            string userId = User.Identity.GetUserId();
            DateTime expiration = new DateTime();

            if (!String.IsNullOrEmpty(userId))
            {
                ApplicationUser user = await UserManager.FindByIdAsync(userId);
                expiration = DateTime.Now.AddMinutes(se.getSessionTime());
                se.set("TimeToExpireSession", expiration);
            }

            SelectListItem item = new SelectListItem()
            {
                Text = "TimeToExpireSession",
                Value = expiration.ToLongDateString(),
            };

            return Json(item, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult EnviarCorreoMA()
        {
            ViewBag.Titulo = "Consulta Mesa de Ayuda";
            ViewBag.ClsModal = "hmodal-success";
            ViewBag.Action = "EnviarCorreoMA";
            CorreoMAView correoMAView = new CorreoMAView();
            return PartialView("CorreoMAView", correoMAView);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EnviarCorreoMA(CorreoMAView correoMAView)
        {
            try
            {
                string correo = "lodapr@aguasaraucania.cl";
                int entero = 0;

                MailServer email = new MailServer($"Consulta a Mesa de Ayuda:", correoMAView.Remitente);
                email.InsertBodyItem("Consulta a Mesa de Ayuda: " + correoMAView.Asunto, MailServer.ItemType.H3, MailServer.Align.Center);
                email.InsertLine();
                email.InsertSpace();
                email.InsertBodyItem($"Se ha realizado una consulta por parte de: {correoMAView.Nombres}", MailServer.ItemType.p, MailServer.Align.Left);
                email.InsertBodyItem($"Consulta: {correoMAView.Cuerpo} .", MailServer.ItemType.p, MailServer.Align.Left);
                        //FLEXBOX METODO 1: CON CLASE FUERTEMENTE TIPADA
                email.InsertFlexBoxTable(correoMAView, s => s.Asunto, s => s.Remitente, s => s.Nombres);              
                email.InsertSpace();

                entero = await email.SendEmail(correo);

                return Content("true");
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
                
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }

            base.Dispose(disposing);
        }

        #region Asistentes
        // Se usa para la protección XSRF al agregar inicios de sesión externos
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion
    }
}