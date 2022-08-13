using LOD_APR.Areas.Admin.Models;
using LOD_APR.Areas.GLOD.Models;
using LOD_APR.Helpers.Validadores;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;

namespace LOD_APR.Models
{
    public class ApplicationUser : IdentityUser
    {

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Tenga en cuenta que el valor de authenticationType debe coincidir con el definido en CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Agregar aquí notificaciones personalizadas de usuario
            return userIdentity;
        }
        public bool Activo { get; set; }

        [ForeignKey("MAE_Sucursal")]
        public int IdSucursal { get; set; }
        public virtual MAE_Sucursal MAE_Sucursal { get; set; }
        public string Run { get; set; }
        public string Nombres { get; set; }
        public string Apellidos { get; set; }
        public string Telefono { get; set; }
        public string Movil { get; set; }
        public string AnexoEmpresa { get; set; }
        public string CargoContacto { get; set; }
        public string DataLetters { get; set; }
        public string RutaImagen { get; set; }
        public string IdCertificado { get; set; }
        public DateTime? FechaCreacion { get; set; }
        [NotMapped]
        public string RutaImg
        {
            get
            {
                if (RutaImagen == null)
                {
                    return string.Empty;
                }
                else
                {
                    return "/Images/Contactos/" + RutaImagen;
                }
            }
        }

        [NotMapped]
        [DisplayName("Nombre Usuario Firmante")]
        public string NombreCompleto
        {
            get
            {
                return Nombres.Split(' ')[0] + " " + Apellidos;
            }
        }

        [NotMapped]
        public string RunToken
        {
            get
            {
                string temp = Run.Replace(".", "").Replace("-", "");
                return temp.Remove(temp.Length - 1, 1);
            }
        }
    }

    public class ApplicationRole : IdentityRole    {
        public ApplicationRole() : base() { }
        public ApplicationRole(string name) : base(name) { }
        public bool IsGubernamental { get; set; }
        public bool IsFiscalizador { get; set; }
        public bool IsContratista { get; set; }

        //[ForeignKey("MAE_sistema")]
        //public int? IdSistema { get; set; }
        //public virtual MAE_sistema MAE_sistema { get; set; }

        //public int TipoPerfil { get; set; }
        //public int IdEmpresa { get; set; }

        [NotMapped]
        public string AppName { get; set; }    }

    public class LOD_DB : IdentityDbContext<ApplicationUser>
    {
        public LOD_DB() : base("LOD_DB", throwIfV1Schema: false)
        {
        }

        public static LOD_DB Create()
        {
            return new LOD_DB();
        }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<IdentityUser>().ToTable("SEG_Users", "dbo").Property(p => p.Id).HasColumnName("UserId");
            modelBuilder.Entity<ApplicationUser>().ToTable("SEG_Users", "dbo").Property(p => p.Id).HasColumnName("UserId");
            modelBuilder.Entity<IdentityUserRole>().ToTable("SEG_UserRoles", "dbo");
            modelBuilder.Entity<IdentityUserLogin>().ToTable("SEG_UserLogins", "dbo");
            modelBuilder.Entity<IdentityUserClaim>().ToTable("SEG_UserClaims", "dbo");
            modelBuilder.Entity<IdentityRole>().ToTable("SEG_Roles", "dbo");
            
        }
        /*PARA UTILIZAR EL NUEVO MODEL DE ROLES*/
        new public DbSet<ApplicationRole> Roles { get; set; }
        public DbSet<IdentityUserRole> UserRoles { get; set; }

        ////TABLAS LOCALIZACIÓN
        public virtual DbSet<MAE_ciudad> MAE_ciudad { get; set; }
        public virtual DbSet<MAE_region> MAE_region { get; set; }

        //****************
        //TABLAS AUXILIARES
        public virtual DbSet<MAE_tipoMedida> MAE_tipoMedida { get; set; }
        public virtual DbSet<MAE_Umedida> MAE_Umedida { get; set; }
        public virtual DbSet<MAE_moneda> MAE_moneda { get; set; }
        public virtual DbSet<MAE_nacionalidad> MAE_nacionalidad { get; set; }
        public virtual DbSet<MAE_TipoPath> MAE_TipoPath { get; set; }
        public virtual DbSet<MAE_Path> MAE_Path { get; set; }
        public virtual DbSet<MAE_RolesContrato> MAE_RolesContrato { get; set; }
        public virtual DbSet<LOD_RolesCttosContrato> LOD_RolesCttosContrato { get; set; }
        //****************
        //TABLAS PERSONAL

        //****************
        //TABLAS SUJETOS ECONOMICOS
        public virtual DbSet<MAE_sujetoEconomico> MAE_sujetoEconomico { get; set; }
        //TABLAS DOCUMENTOS
        public virtual DbSet<MAE_documentos> MAE_documentos { get; set; }
        public DbSet<MAE_Contactos> MAE_Contactos { get; set; }
        public DbSet<MAE_TipoDocumento> MAE_TipoDocumento { get; set; }


        //SEGURIDAD EN ASP****
        public DbSet<SEG_UserContacto> SEG_UserContacto { get; set; }
        public DbSet<MAE_OpcionesRoles> MAE_OpcionesRoles { get; set; }
        public DbSet<MAE_modulos> MAE_modulos { get; set; }
        public DbSet<MAE_opcionesMenu> MAE_opcionesMenu { get; set; }
        public DbSet<MAE_Empresa> MAE_Empresa { get; set; }
        public DbSet<MAE_ParametrosModulo> MAE_ParametrosModulo { get; set; }
        public DbSet<MAE_ConfigEmail> MAE_ConfigEmail { get; set; }
        public DbSet<MAE_ConfigSecurityEmail> MAE_ConfigSecurityEmail { get; set; }
        //public DbSet<MAE_ConfigEmail> MAE_ConfigEmail { get; set; }
        public DbSet<MAE_tipoCtaEmails> MAE_tipoCtaEmails { get; set; }
        public DbSet<MAE_sistema> MAE_sistema { get; set; }

        public DbSet<MAIL_Envio> MAIL_Envio { get; set; }

        public DbSet<MAE_TipoLOD> MAE_TipoLOD { get; set; }

        public DbSet<MAE_TipoComunicacion> MAE_TipoComunicacion { get; set; }
        public DbSet<MAE_DireccionesMOP> MAE_DireccionesMOP { get; set; }
        public DbSet<MAE_CodSubCom> MAE_CodSubCom { get; set; }

        public DbSet<MAE_SubtipoComunicacion> MAE_SubtipoComunicacion { get; set; }

        public DbSet<MAE_Sucursal> MAE_Sucursal { get; set; }
        public DbSet<MAE_ClassDoc> MAE_ClassDoc { get; set; }

        public DbSet<MAE_ClassOne> MAE_ClassOne { get; set; }

        public DbSet<MAE_ClassTwo> MAE_ClassTwo { get; set; }

        //FORMS
        public DbSet<FORM_Formularios> FORM_Formularios { get; set; }
        public DbSet<FORM_Ejecucion> FORM_Ejecucion { get; set; }
        public DbSet<FORM_FormItems> FORM_FormItems { get; set; }
        public DbSet<FORM_FormPreguntas> FORM_FormPreguntas { get; set; }
        public DbSet<FORM_FormAlternativa> FORM_FormAlternativa { get; set; }

        //REPORTES MENSUALES
        public DbSet<CON_Contratos> CON_Contratos { get; set; }
        public DbSet<FORM_Informes> FORM_Informes { get; set; }
        public DbSet<FORM_InformesItems> FORM_InformesItems { get; set; }
        public DbSet<FORM_InformesItemsData> FORM_InformesItemsData { get; set; }
        //**********************************************************************************

        public DbSet<LOD_Anotaciones> LOD_Anotaciones { get; set; }
        public DbSet<LOD_UserAnotacion> LOD_UserAnotacion { get; set; }
        public DbSet<LOD_UsuariosLod> LOD_UsuariosLod { get; set; }
        public DbSet<LOD_ReferenciasAnot> LOD_ReferenciasAnot { get; set; }
        public DbSet<LOD_LibroObras> LOD_LibroObras { get; set; }
        public DbSet<LOD_docAnotacion> LOD_docAnotacion { get; set; }
        public DbSet<LOD_Carpetas> LOD_Carpetas { get; set; }
        public DbSet<LOD_log> LOD_log { get; set; }
        public DbSet<LOD_PermisosRolesContrato> LOD_PermisosRolesContrato { get; set; }

    }


}