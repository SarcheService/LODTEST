using LOD_APR.Areas.GLOD.Helpers;
using LOD_APR.Areas.GLOD.Models;
using LOD_APR.Helpers.ModelsHelpers;
using LOD_APR.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace LOD_APR.Controllers
{
    [AllowAnonymous]
    public class PubController : Controller

    {
        private LOD_DB db = new LOD_DB();
        private Log_Helper Log_Helper = new Log_Helper();

        public async Task<string> validateCodigo(string codigo)
        {
            string result = string.Empty;

            if (codigo == null)
                return result;

            var anotacion = await db.LOD_Anotaciones.Where(a => a.EstadoFirma == false && a.TempCode == codigo).FirstOrDefaultAsync();
            if (anotacion != null)
                result = db.Users.Find(anotacion.UserId).IdCertificado;

            return result;

        }

        public async Task<string> validateCodigoVB(string codigo)
        {
            string result = string.Empty;

            if (codigo == null)
                return result;

            var anotacion = await db.LOD_UserAnotacion.Where(a => a.LOD_Anotaciones.EstadoFirma && a.TempCode == codigo && a.FechaVB == null).FirstOrDefaultAsync();
            if (anotacion != null)
                result = db.Users.Find(anotacion.LOD_UsuarioLod.UserId).IdCertificado;

            return result;

        }

        public async Task<string> validateCodigoActivacion(string codigo)
        {
            string result = string.Empty;

            if (codigo == null)
                return result;

            var libro = await db.LOD_LibroObras.Where(a => a.Estado == 0 && a.OTP == codigo && a.FechaApertura == null).FirstOrDefaultAsync();
            if (libro != null)
                result = db.Users.Find(libro.UsuarioApertura).IdCertificado;

            return result;

        }

        public async Task<JsonResult> RollBackSign(string codigo)
        {
            Form_Validation val = new Form_Validation() { Status = true, ErrorMessage = new List<string>() };

            if (codigo == null)
            {
                val.ErrorMessage.Add("El Código es inválido");
                val.Status = false;
                return Json(val, JsonRequestBehavior.AllowGet);
            }

            var anotacion = await db.LOD_Anotaciones.Where(a => a.EstadoFirma && a.TempCode == codigo).FirstOrDefaultAsync();
            if (anotacion != null)
            {
                FirmarAnotacion firm = new FirmarAnotacion();
                await firm.QuitarFirmaAnotacionDB(anotacion.IdAnotacion);
            }

            return Json(val, JsonRequestBehavior.AllowGet);

        }

        public async Task<JsonResult> GetPdfParaFirmar(string codigo)
        {
            Form_Validation val = new Form_Validation() { Status = false, ErrorMessage = new List<string>() };
            FirmarAnotacion firm = new FirmarAnotacion();

            if (codigo == null)
            {
                val.ErrorMessage.Add("El código ingresado No es válido.");
                return Json(val, JsonRequestBehavior.AllowGet);
            }

            var anotacion = await db.LOD_Anotaciones.Where(a => a.EstadoFirma == false && a.TempCode == codigo).FirstOrDefaultAsync();

            if (anotacion == null)
            {
                val.ErrorMessage.Add("Ocurrió un Problema al tratar de buscar Anotación a Firmar.");
                return Json(val, JsonRequestBehavior.AllowGet);
            }

            string filePath = string.Empty;

            if (await firm.FirmarAnotacionDB(anotacion.IdAnotacion, 1, anotacion.UserId))
            {
                filePath = await firm.GeneratePDF(anotacion.IdAnotacion);
                if (String.IsNullOrEmpty(filePath))
                {
                    val.ErrorMessage.Add("Ocurrió un Problema al tratar de guardar el archivo pdf de la Anotación.");
                    await firm.QuitarFirmaAnotacionDB(anotacion.IdAnotacion);
                    return Json(val, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    string fileName = db.LOD_Anotaciones.Find(anotacion.IdAnotacion).RutaPdfConFirma;
                    val.Status = true;
                    val.Parametros = await firm.AnotacionPDFToBase64(filePath);
                }

                GLOD_Notificaciones notificaciones = new GLOD_Notificaciones();
                string userid = User.Identity.GetUserId();
                LOD_ReferenciasAnot referencia = db.LOD_ReferenciasAnot.Where(x => x.IdAnotacion == anotacion.IdAnotacion).FirstOrDefault();
                int resu = 0;
                if (referencia == null)  //NOTIFICAR PUBLICACION O NOTIFICAR RESPUESTA
                    resu = await notificaciones.NotificarPublicacion(anotacion, userid);
                else
                    resu = await notificaciones.NotificarRespuesta(anotacion, userid);
            }
            else
            {
                val.ErrorMessage.Add("Ocurrió un Problema al tratar de guardar los datos de firma de la Anotación.");
                return Json(val, JsonRequestBehavior.AllowGet);
            }

            return Json(val, JsonRequestBehavior.AllowGet);

        }

        public async Task<JsonResult> GuardarPdfFirmado(Post_Firma firma)
        {
            Form_Validation val = new Form_Validation() { Status = false, ErrorMessage = new List<string>() };
            FirmarAnotacion firm = new FirmarAnotacion();

            try
            {
                var anotacion = await db.LOD_Anotaciones.Where(a => a.EstadoFirma && a.TempCode == firma.Codigo).FirstOrDefaultAsync();
                if (anotacion == null)
                {
                    val.ErrorMessage.Add("Ocurrió un Problema al tratar de buscar Anotación a Firmar.");
                    await firm.QuitarFirmaAnotacionDB(anotacion.IdAnotacion);
                    return Json(val, JsonRequestBehavior.AllowGet);
                }

                if (String.IsNullOrEmpty(firma.PdfBase64))
                {
                    val.ErrorMessage.Add("El pdf enviado no es válido.");
                    await firm.QuitarFirmaAnotacionDB(anotacion.IdAnotacion);
                    return Json(val, JsonRequestBehavior.AllowGet);
                }

                MemoryStream ms = new MemoryStream(Convert.FromBase64String(firma.PdfBase64));
                string tempPath = Path.Combine(Server.MapPath("~/"), anotacion.RutaCarpetaPdf);

                if (!Directory.Exists(tempPath))
                    Directory.CreateDirectory(tempPath);

                string informeTempPath = Path.Combine(Server.MapPath("~/"), anotacion.RutaPdfConFirma);

                FileStream newFile = new FileStream(informeTempPath, FileMode.Create, FileAccess.Write);
                ms.WriteTo(newFile);
                newFile.Close();

                //MOVER LOS DOCUMENTOS ADJUNTOS AL DIRECTORIO FINAL
                try
                {
                    List<LOD_docAnotacion> docsAnotacion = await db.LOD_docAnotacion.Where(d => d.IdAnotacion == anotacion.IdAnotacion).ToListAsync();

                    foreach (var doc in docsAnotacion)
                    {
                        string borrador = Path.Combine(Server.MapPath("~/"), doc.MAE_documentos.Ruta);
                        string destino = Path.Combine(Server.MapPath("~/"), anotacion.RutaCarpetaPdf, doc.MAE_documentos.NombreDoc);
                        System.IO.File.Move(borrador, destino);

                        doc.MAE_documentos.Ruta = $"{anotacion.RutaCarpetaPdf}/{doc.MAE_documentos.NombreDoc}";
                        db.Entry(doc).State = EntityState.Modified;
                        await db.SaveChangesAsync();
                    }
                    Directory.Delete(Path.Combine(Server.MapPath("~/"), anotacion.RutaCarpetaBorradores));
                }
                catch { }


                string pdfSinFirmaTempPath = Path.Combine(Server.MapPath("~/"), anotacion.RutaPdfSinFirma);
                System.IO.File.Delete(pdfSinFirmaTempPath);
                val.Status = true;
                val.Parametros = anotacion.Correlativo.ToString().PadLeft(6, '0');

                anotacion.TempCode = null;
                db.Entry(anotacion).State = EntityState.Modified;
                await db.SaveChangesAsync();

                string accion = "Anotación Firmada Correctamente";
                bool response = await Log_Helper.SetLOGAnotacionAsync(anotacion, accion, anotacion.UserId);

                return Json(val, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                val.ErrorMessage.Add(ex.Message);
                return Json(val, JsonRequestBehavior.AllowGet);
            }

        }

        public async Task<JsonResult> GetPdfParaVB(string codigo)
        {
            Form_Validation val = new Form_Validation() { Status = false, ErrorMessage = new List<string>() };
            FirmarAnotacion firm = new FirmarAnotacion();

            if (codigo == null)
            {
                val.ErrorMessage.Add("El código ingresado No es válido.");
                return Json(val, JsonRequestBehavior.AllowGet);
            }

            var anotacion = await db.LOD_UserAnotacion.Where(a => a.LOD_Anotaciones.EstadoFirma && a.TempCode == codigo && a.FechaVB == null).Select(an => an.LOD_Anotaciones).FirstOrDefaultAsync();
            if (anotacion == null)
            {
                val.ErrorMessage.Add("Ocurrió un Problema al tratar de buscar Anotación a Firmar.");
                return Json(val, JsonRequestBehavior.AllowGet);
            }

            try
            {
                string tempPath = Path.Combine(Server.MapPath("~/"), anotacion.RutaPdfConFirma);
                val.Status = true;
                val.Parametros = await firm.AnotacionPDFToBase64(tempPath);
            }
            catch (Exception ex)
            {
                val.ErrorMessage.Add("Ocurrió un Problema al tratar de guardar los datos de firma de la Anotación.");
                return Json(val, JsonRequestBehavior.AllowGet);
            }

            return Json(val, JsonRequestBehavior.AllowGet);

        }

        public async Task<JsonResult> GuardarPdfFirmadoVB(Post_Firma firma)
        {
            Form_Validation val = new Form_Validation() { Status = false, ErrorMessage = new List<string>() };
            FirmarAnotacion firm = new FirmarAnotacion();

            try
            {
                var anotacion = await db.LOD_UserAnotacion.Where(a => a.LOD_Anotaciones.EstadoFirma && a.TempCode == firma.Codigo).Select(an => an.LOD_Anotaciones).FirstOrDefaultAsync();
                if (anotacion == null)
                {
                    val.ErrorMessage.Add("Ocurrió un Problema al tratar de buscar Anotación a Firmar.");
                    return Json(val, JsonRequestBehavior.AllowGet);
                }

                if (String.IsNullOrEmpty(firma.PdfBase64))
                {
                    val.ErrorMessage.Add("El pdf enviado no es válido.");
                    return Json(val, JsonRequestBehavior.AllowGet);
                }

                MemoryStream ms = new MemoryStream(Convert.FromBase64String(firma.PdfBase64));
                string tempPath = Path.Combine(Server.MapPath("~/"), anotacion.RutaCarpetaPdf);

                if (!Directory.Exists(tempPath))
                    Directory.CreateDirectory(tempPath);

                string informeTempPath = Path.Combine(Server.MapPath("~/"), anotacion.RutaPdfConFirma);

                FileStream newFile = new FileStream(informeTempPath, FileMode.Create, FileAccess.Write);
                ms.WriteTo(newFile);
                newFile.Close();

                //MOVER LOS DOCUMENTOS ADJUNTOS AL DIRECTORIO FINAL
                try
                {
                    var userAnotacionActual = db.LOD_UserAnotacion.Where(x => x.IdAnotacion == anotacion.IdAnotacion && x.TempCode == firma.Codigo).FirstOrDefault();
                    if (userAnotacionActual != null)
                    {
                        userAnotacionActual.FechaVB = DateTime.Now;
                        userAnotacionActual.VistoBueno = true;
                        userAnotacionActual.TipoVB = 1;
                        db.Entry(userAnotacionActual).State = EntityState.Modified;
                        await db.SaveChangesAsync();
                    };

                    val.Status = true;
                    val.Parametros = anotacion.Correlativo.ToString().PadLeft(6, '0');
                    string accion = $"{userAnotacionActual.LOD_UsuarioLod.ApplicationUser.NombreCompleto} confirma Toma de Conocimiento correctamente";
                    bool response = await Log_Helper.SetLOGAnotacionAsync(anotacion, accion, anotacion.UserId);
                }
                catch { }

                return Json(val, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                val.ErrorMessage.Add(ex.Message);
                return Json(val, JsonRequestBehavior.AllowGet);
            }

        }
        public async Task<JsonResult> ActivarLibro(Post_Firma otp)
        {
            Form_Validation val = new Form_Validation() { Status = false, ErrorMessage = new List<string>() };

            try
            {

                var libro = await db.LOD_LibroObras.Where(a => a.Estado == 0 && a.OTP == otp.Codigo && a.FechaApertura == null).FirstOrDefaultAsync();
                if (libro == null)
                {
                    val.ErrorMessage.Add("Ocurrió un Problema al tratar de buscar el Libro para Activar.");
                    return Json(val, JsonRequestBehavior.AllowGet);
                }
                var user = await db.Users.Where(u => u.IdCertificado == otp.PdfBase64 && u.Id == libro.UsuarioApertura).FirstOrDefaultAsync();

                if (user == null)
                {
                    val.ErrorMessage.Add("El Certificado no coincide con el usuario que está Activando el Libro.");
                    return Json(val, JsonRequestBehavior.AllowGet);
                }

                try
                {
                    libro.FechaApertura = DateTime.Now;
                    libro.Estado = 1;
                    libro.OTP = null;
                    db.Entry(libro).State = EntityState.Modified;
                    await db.SaveChangesAsync();

                    val.Status = true;
                    //string accion = $"Usuario Activa {libro.NombreLibroObra} correctamente";
                    //bool response = await Log_Helper.SetLOGAnotacionAsync(anotacion, accion, anotacion.UserId);
                }
                catch { }

                return Json(val, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                val.ErrorMessage.Add(ex.Message);
                return Json(val, JsonRequestBehavior.AllowGet);
            }

        }
        public async Task<ActionResult> ValidarFolio(int folio, int ldo, int cont)
        {
            FirmarAnotacion firm = new FirmarAnotacion();
            var anot = await db.LOD_Anotaciones.Where(a => a.Correlativo == folio && a.EstadoFirma && a.IdLod == ldo && a.LOD_LibroObras.IdContrato == cont).FirstOrDefaultAsync();
            if (anot == null)
                return View("Error");

            string tempPath = await firm.PathDescargarAnotacion(anot.IdAnotacion);

            var memory = new MemoryStream();
            using (var stream = new FileStream(tempPath, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;
            return File(memory, "application/pdf", Path.GetFileName(tempPath));
        }

        public ViewResult Error()
        {
            var version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            string versionFormat = string.Format("{0}.{1}.{2}", version.Major, version.Minor, version.Build);
            ViewBag.version = versionFormat;
            return View("Error");
        }


        public ActionResult SinPermiso()
        {
            var version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            string versionFormat = string.Format("{0}.{1}.{2}", version.Major, version.Minor, version.Build);
            ViewBag.version = versionFormat;
            return View();
        }


        public class Post_Firma
        {
            public string Codigo { get; set; }
            public string PdfBase64 { get; set; }
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