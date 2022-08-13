using LOD_APR.Helpers.Validadores;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LOD_APR.Models
{
    public class ExternalLoginConfirmationViewModel
    {
        [Required]
        [Display(Name = "Correo electrónico")]
        public string Email { get; set; }
    }

    public class ExternalLoginListViewModel
    {
        public string ReturnUrl { get; set; }
        //[Required(ErrorMessage = "Dato obligatorio")]
        //[Display(Name ="Rut Usuario")]
        //[RutValido]
        //public string RutUsuario { get; set; }
    }
    public class VerifyCodeViewModel
    {
        [Required]
        public string Provider { get; set; }

        [Required]
        [Display(Name = "Código")]
        public string Code { get; set; }
        public string ReturnUrl { get; set; }

        [Display(Name = "¿Recordar este explorador?")]
        public bool RememberBrowser { get; set; }

        public bool RememberMe { get; set; }
    }
    public class ForgotViewModel
    {
        [Required]
        [Display(Name = "Correo electrónico")]
        public string Email { get; set; }
    }

    public class LoginViewModel
    {
        [Required]
        [Display(Name = "Usuario")]
        public string Usuario { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string Password { get; set; }

        [Display(Name = "¿Recordar cuenta?")]
        public bool RememberMe { get; set; }

        public string ReturnUrl { get; set; }
    }

    public class ADLoginViewModel
    {
        [Required]
        [Display(Name = "Email")]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string Password { get; set; }

        public string ReturnUrl { get; set; }
    }

    public class UserViewModel
    {
        public string Id { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Correo electrónico")]
        public string Email { get; set; }

        [Display(Name = "Usuario API")]
        public bool ApiUser { get; set; }

        [Display(Name = "Super Usuario")]
        public bool Admin { get; set; }

        [Display(Name = "Perfil Usuario")]
        public string RoleName { get; set; }

        [Display(Name = "Usuario AD")]
        [StringLength(200, ErrorMessage = "El número de caracteres de {0} debe ser al menos {1}.")]
        public string ActiveDirectoryAccount { get; set; }

        //Agregado 12-11-2018 No mapped para tipo de Usuario

        public int IdSucursal { get; set; }

        [StringLength(13, ErrorMessage = "Máx. 13 caracteres")]
        [Required(ErrorMessage = "Dato obligatorio")]
        [RutValido]
        public string Run { get; set; }

        [Required(ErrorMessage = "Dato obligatorio")]
        public string Nombres { get; set; }

        [Required(ErrorMessage = "Dato obligatorio")]
        public string Apellidos { get; set; }
        public string Telefono { get; set; }
        public string Movil { get; set; }
        public string AnexoEmpresa { get; set; }

        [Required(ErrorMessage = "Dato obligatorio")]
        public string CargoContacto { get; set; }
        public string DataLetters { get; set; }
        public string RutaImagen { get; set; }
        public bool Activo { get; set; }

        public string IdCertificado { get; set; }

        [Required(ErrorMessage = "Debe seleccionar al menos 1 rol para el usuario")]
        public List<string> IdRoles { get; set; }

        [NotMapped]
        public string back { get; set; }
    }

    public class RegisterViewModel
    {

        [Required]
        [EmailAddress]
        [Display(Name = "Correo electrónico")]
        public string Email { get; set; }

        [Display(Name = "Usuario API")]
        public bool ApiUser { get; set; }

        [Display(Name = "Super Usuario")]
        public bool Admin { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "El número de caracteres de {0} debe ser al menos {2}.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirmar contraseña")]
        [Compare("Password", ErrorMessage = "La contraseña y la contraseña de confirmación no coinciden.")]
        public string ConfirmPassword { get; set; }
        [Display(Name = "Perfil Usuario")]
        public string RoleName { get; set; }

        [Display(Name = "Usuario AD")]
        [StringLength(200, ErrorMessage = "El número de caracteres de {0} debe ser al menos {1}.")]
        public string ActiveDirectoryAccount { get; set; }

        //Agregado 12-11-2018 No mapped para tipo de Usuario

        public int IdSucursal { get; set; }

        [StringLength(13, ErrorMessage = "Máx. 13 caracteres")]
        [Required(ErrorMessage = "Dato obligatorio")]
        [RutValido]
        public string Run { get; set; }

        [Required(ErrorMessage = "Dato obligatorio")]
        public string Nombres { get; set; }

        [Required(ErrorMessage = "Dato obligatorio")]
        public string Apellidos { get; set; }
        public string Telefono { get; set; }
        public string Movil { get; set; }
        public string AnexoEmpresa { get; set; }

        [Required(ErrorMessage = "Dato obligatorio")]
        public string CargoContacto { get; set; }
        public string DataLetters { get; set; }
        public string RutaImagen { get; set; }
        public bool Activo { get; set; }
        public string IdCertificado { get; set; }

        [Required(ErrorMessage = "Debe seleccionar al menos 1 rol para el usuario")]
        public List<string> IdRoles { get; set; }
    }

    public class ResetPasswordViewModel
    {
        [Required]
        [Display(Name = "Rut")] //se cambia el acceso por Email a Rut, por lo tanto se solicita en el Forgot y Reset, utilizar el Rut
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "El número de caracteres de {0} debe ser al menos {2}.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirmar contraseña")]
        [Compare("Password", ErrorMessage = "La contraseña y la contraseña de confirmación no coinciden.")]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }
    }

    public class ForgotPasswordViewModel
    {
        [Required]
        [Display(Name = "Rut")]
        public string Email { get; set; }
    }


    public class SendCodeViewModel
    {
        public string SelectedProvider { get; set; }
        public ICollection<System.Web.Mvc.SelectListItem> Providers { get; set; }
        public string ReturnUrl { get; set; }
        public bool RememberMe { get; set; }
    }


    public class CorreoMAView
    {
        [DisplayName("Remitente")]
        [Required]
        public string Remitente { get; set; }

        [DisplayName("Nombres Rem.")]
        [Required]
        public string Nombres { get; set; }

        [DisplayName("Destinatario")]
        public string Destinatario { get; set; }

        [DisplayName("Consulta")]
        [Required]
        public string Cuerpo { get; set; }


        [DisplayName("Asunto")]
        public string Asunto { get; set; }
    }
}
