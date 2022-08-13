using LOD_APR.Helpers;
using LOD_APR.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace LOD_APR.Controllers
{
    [CustomAuthorize]
    public class FichasController : Controller
    {
        private LOD_DB db = new LOD_DB();

        public async Task<JsonResult> FichaPersonal(int id)
        {
            var personal = await db.MAE_Contactos.FindAsync(id);
            var cargo = db.MAE_Contactos.Where(p => p.IdContacto == id && p.Activo == true).FirstOrDefault();
            FichaPersonal ficha = new FichaPersonal();

            try
            {
                ficha = new FichaPersonal()
                {
                    nombre = personal.Nombre,
                    estado = personal.Activo,
                    foto = personal.RutaImagen,
                    dataletters = personal.Iniciales,
                    dlclass = personal.DataLetters,
                    urlfacebook = personal.UrlFacebook,
                    urllinkedin = personal.UrlLinkedin,
                    urltwitter = personal.UrlTwitter,
                    isfacebook = !String.IsNullOrEmpty(personal.UrlFacebook),
                    istwitter= !String.IsNullOrEmpty(personal.UrlTwitter),
                    islinkedin= !String.IsNullOrEmpty(personal.UrlLinkedin),
                    email= personal.Email,
                    telefono = personal.Telefono,
                    anexo = personal.AnexoEmpresa,
                    cargo= personal.CargoContacto,
                    faena="-",
                    unidad="-"
                };
                              
            }
            catch{}
           
            return Json(ficha, JsonRequestBehavior.AllowGet);
        }
        public async Task<JsonResult> FichaSujeto(int id)
        {
            var sujeto = await db.MAE_sujetoEconomico.FindAsync(id);
            FichaSujeto ficha = new FichaSujeto();

            try
            {
                ficha = new FichaSujeto
                {
                    rut = sujeto.Rut,
                    razon = sujeto.RazonSocial,
                    fantasia = sujeto.NomFantasia,
                    giro = sujeto.Giro,
                    estado = sujeto.Activo,
                    foto = sujeto.RutaImagen,
                    dataletters = sujeto.Iniciales,
                    dlclass = sujeto.DataLetters,
                    urlfacebook = sujeto.UrlFacebook,
                    urllinkedin = sujeto.UrlLinkedin,
                    urltwitter = sujeto.UrlTwitter,
                    isfacebook = !String.IsNullOrEmpty(sujeto.UrlFacebook),
                    istwitter = !String.IsNullOrEmpty(sujeto.UrlTwitter),
                    islinkedin = !String.IsNullOrEmpty(sujeto.UrlLinkedin),
                    email = sujeto.email,
                    telefono = sujeto.Telefono,
                    ciudad = sujeto.GetCiudad,
                    direccion = sujeto.Direccion,
                    web = sujeto.web,
                    escliente = sujeto.EsGubernamental,
                    escontratista = sujeto.EsContratista,
                    esorganizacion = sujeto.EsGubernamental,
                    esrelacionada = sujeto.EsMandante
                };
                
            }
            catch { }

            return Json(ficha, JsonRequestBehavior.AllowGet);
        }

    }
    public class FichaPersonal
    {
        public string nombre { get; set; }
        public string nombres { get; set; }
        public string apellido { get; set; }
        public string apellidos { get; set; }
        public bool estado { get; set; }
        public string foto { get; set; }
        public string dataletters { get; set; }
        public string dlclass { get; set; }
        public bool isfacebook { get; set; }
        public bool istwitter { get; set; }
        public bool islinkedin { get; set; }
        public string urlfacebook { get; set; }
        public string urltwitter { get; set; }
        public string urllinkedin { get; set; }
        public string cargo { get; set; }
        public string unidad { get; set; }
        public string faena { get; set; }
        public string email { get; set; }
        public string telefono { get; set; }
        public string anexo { get; set; }
    }
    public class FichaSujeto
    {
        public string rut { get; set; }
        public string razon { get; set; }
        public string fantasia { get; set; }
        public string giro { get; set; }
        public string direccion { get; set; }
        public string ciudad { get; set; }
        public string email { get; set; }
        public string telefono { get; set; }
        public string web { get; set; }
        public bool estado { get; set; }
        public string foto { get; set; }
        public string dataletters { get; set; }
        public string dlclass { get; set; }
        public bool isfacebook { get; set; }
        public bool istwitter { get; set; }
        public bool islinkedin { get; set; }
        public string urlfacebook { get; set; }
        public string urltwitter { get; set; }
        public string urllinkedin { get; set; }
        public bool escliente { get; set; }
        public bool esproveedor { get; set; }
        public bool escontratista { get; set; }
        public bool esorganizacion { get; set; }
        public bool esrelacionada { get; set; }
    }

}