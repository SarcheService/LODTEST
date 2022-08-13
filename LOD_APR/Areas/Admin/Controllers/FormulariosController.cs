using LOD_APR.Areas.Admin.Models;
using LOD_APR.Helpers;
using LOD_APR.Models;
using MessagingToolkit.QRCode.Codec;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace LOD_APR.Areas.Admin.Controllers
{
    [CustomAuthorize]
    public class FormulariosController : Controller
    {
        private LOD_DB db = new LOD_DB();

        public ActionResult Index()
        {
            //PENDIENTE FILTRO SEGÚN CONFIGURACIÓN DEL FORM
            var modelo = db.FORM_Formularios.Where(f=> f.Embebido==false).OrderBy(x => x.Titulo).ToList();
            return View(modelo);
        }

        //public ActionResult EjecutarForm()
        //{
        //    var modelo = db.FORM_Formularios.Where(w=> w.Activa==true).OrderBy(x => x.Titulo).ToList();
        //    List<FORM_Formularios> fauth = new List<FORM_Formularios>();

        //    string _User = User.Identity.GetUserId();
        //    int idPersonal = db.SEG_UserPersonal.Where(x => x.UserId == _User).FirstOrDefault().IdPersonal;
        //    HelpPersonal hlp = new HelpPersonal();
        //    Cargo_Personal cargo = hlp.Cargo_Personal(idPersonal);

        //    foreach (FORM_Formularios form in modelo)
        //    {
        //        if (form.Visibilidad == 0)
        //        {
        //            if(form.UserId == User.Identity.GetUserName())
        //                fauth.Add(form);

        //        }else if (form.Visibilidad == 1)
        //        {
        //            if (cargo.IdUnidad != null)
        //            {
        //                if (form.Dependencias)
        //                {
        //                    List<TreeNode> general = JsTrees.getTreeJerarquico(true);
        //                    List<TreeNode> dependencias = JsTrees.FlatToHierarchy(general, Convert.ToInt32(form.IdJerarquia));
        //                    List<int> listado_unidades = dependencias.Traverse(x => x.children).Select(s => s.data.db_id).ToList();
        //                    listado_unidades.Add(Convert.ToInt32(form.IdJerarquia));

        //                    if (listado_unidades.Contains(Convert.ToInt32(cargo.IdUnidad)))
        //                        fauth.Add(form);
        //                }
        //                else
        //                {
        //                    if(form.IdJerarquia== Convert.ToInt32(cargo.IdUnidad))
        //                        fauth.Add(form);
        //                }
        //            }
        //        }
        //        else
        //        {
        //            fauth.Add(form);
        //        }
        //    }
        //    return View(fauth);
        //}

        public async Task<ActionResult> GetTable()
        {
            //PENDIENTE FILTRO SEGÚN CONFIGURACIÓN DEL FORM
            var modelo = await db.FORM_Formularios.Where(f => f.Embebido == false).OrderBy(x => x.Titulo).ToListAsync();
            return PartialView("_GetTabla", modelo);
        }

        public ActionResult Create()
        {
            FORM_Formularios model = new FORM_Formularios();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IdForm,Titulo,Descripcion")] FORM_Formularios eVA_formularioEvaluacion)
        {
            
            try
            {
                eVA_formularioEvaluacion.IdForm = RandomString.GetRandomString(10);
                eVA_formularioEvaluacion.UserId = User.Identity.GetUserName();
                eVA_formularioEvaluacion.FechaCreacion = DateTime.Now;
                eVA_formularioEvaluacion.Activa = false;
                eVA_formularioEvaluacion.Embebido = false;
              
                db.FORM_Formularios.Add(eVA_formularioEvaluacion);
                db.SaveChanges();
                return RedirectToAction("Edit", new { id = eVA_formularioEvaluacion.IdForm });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(eVA_formularioEvaluacion);
            }
        }

        public ActionResult Edit(string id)
        {
            var eVA_formularioEvaluacion = db.FORM_Formularios.Find(id);
            if (eVA_formularioEvaluacion == null)
                return HttpNotFound();

            //ViewBag.Link = Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath.TrimEnd('/') + "/FORMS/Pub/Ejecutar?id=";
            ViewBag.FormsEmbebidos = new SelectList(db.FORM_Formularios.Where(f => f.Embebido).ToList(), "IdForm", "Titulo");

            //List<ComboBoxEstandar2> ListVisibilidad = Visibilidad.GetTipoVisibilidad();
            //ViewBag.Visibilidad = new SelectList(ListVisibilidad.OrderBy(x => x.Id), "Id", "Value", eVA_formularioEvaluacion.Visibilidad);

            //List<ComboBoxEstandar2> ListaccionPOST = AccionPOST.GetAccionPOST();
            //ViewBag.AccionPost = new SelectList(ListaccionPOST.OrderBy(x => x.Id), "Id", "Value", eVA_formularioEvaluacion.AccionPost);
            //if (eVA_formularioEvaluacion.IdJerarquia != null)
            //    ViewBag.lblJerarquia = eVA_formularioEvaluacion.MAE_jerarquia.Nombre; //db.MAE_jerarquia.Find(eVA_formularioEvaluacion.IdJerarquia).Nombre;

            return View(eVA_formularioEvaluacion);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "IdForm,Titulo,Descripcion")] FORM_Formularios eVA_formularioEvaluacion)
        {
            FORM_Formularios modelanterior = db.FORM_Formularios.AsNoTracking().Where(x => x.IdForm == eVA_formularioEvaluacion.IdForm).First();

            try
            {
                modelanterior.Titulo = eVA_formularioEvaluacion.Titulo;
                modelanterior.Descripcion = eVA_formularioEvaluacion.Descripcion;
               
                db.Entry(modelanterior).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return Content("true");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return Content(ex.Message);
            }
        }

        public ActionResult Details(string id)
        {
            FORM_Formularios model = db.FORM_Formularios.Find(id);
            model.FORM_FormItems = db.FORM_FormItems.Where(i => i.IdForm == id).OrderBy(i => i.Indice).OrderBy(x => x.Indice).ToList();
            ViewBag.EvaluacionActiva = model.Activa;
            ViewBag.FormsEmbebidos = new SelectList(db.FORM_Formularios.Where(f => f.Embebido).ToList(), "IdForm", "Titulo");

            foreach (var item in model.FORM_FormItems)
            {
                item.ErrorList = new List<string>();
                if (item.FORM_FormPreguntas.Count() == 0)
                {
                    item.Errores = true;
                    item.ErrorList.Add("Ítem sin preguntas");
                }
                
                foreach (var param in item.FORM_FormPreguntas)
                {
                    
                    param.ErrorList = new List<string>();
                    
                    if (param.TipoParam > 912)
                    {
                        if (param.FORM_FormAlternativa.Count() == 0)
                        {
                            param.ErrorList.Add("Alternativas Pendientes");
                            param.Errores = true;

                            item.Errores = true;
                            item.ErrorList.Add("Pregunta con errores");
                        }
                       
                    }else if (param.TipoParam == 904)
                    {
                        if(String.IsNullOrEmpty(param.IdForm))
                            param.ErrorList.Add("Selección de Formulario Pendiente");
                            param.Errores = true;
                    }
                   
                }

            }
            
            return PartialView("_getDetails", model);
        }

        public async Task<ActionResult> ListParams(string id)
        {
            List<FORM_FormPreguntas> Lista = await db.FORM_FormPreguntas.Where(l=>l.IdItem==id).ToListAsync();
            FORM_FormItems itemFormulario = db.FORM_FormItems.Find(id);

            ViewBag.EvaluacionActiva = itemFormulario.FORM_Formularios.Activa;

            foreach (var param in Lista)
            {
                param.ErrorList = new List<string>();
                
                if (param.TipoParam > 912)
                {
                    if (param.FORM_FormAlternativa.Count() == 0)
                    {
                        param.ErrorList.Add("Alternativas Pendientes");
                        param.Errores = true;
                    }
                }
                else if (param.TipoParam == 904)
                {
                    if (String.IsNullOrEmpty(param.IdForm))
                        param.ErrorList.Add("Selección de Formulario Pendiente");
                        param.Errores = true;
                }

            }

            return PartialView("_ListParametros", Lista);
        }
        
        public async Task<ActionResult> Delete(string id)
        {
            FORM_Formularios formularioEvaluacion = await db.FORM_Formularios.FindAsync(id);
            return PartialView("_Delete", formularioEvaluacion);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string id)
        {
            try
            {
                FORM_Formularios formularioEvaluacion = await db.FORM_Formularios.FindAsync(id);
                db.FORM_Formularios.Remove(formularioEvaluacion);
                await db.SaveChangesAsync();
                return Content("true");
            }
            catch (Exception err)
            {
                return Content(err.Message);
            }
        }

        public async Task<ActionResult> ValidarFormulario(string id)
        {
            var eVA_formularioEvaluacion = db.FORM_Formularios.Find(id);
            if (eVA_formularioEvaluacion == null)
                return HttpNotFound();

            eVA_formularioEvaluacion.FORM_FormItems = db.FORM_FormItems.Where(i => i.IdForm == id).OrderBy(i => i.Indice).OrderBy(x => x.Indice).ToList();

            foreach (var item in eVA_formularioEvaluacion.FORM_FormItems)
            {
                item.ErrorList = new List<string>();
                if (item.FORM_FormPreguntas.Count() == 0)
                {
                    item.Errores = true;
                    item.ErrorList.Add("Ítem sin preguntas");
                }

                foreach (var param in item.FORM_FormPreguntas)
                {

                    param.ErrorList = new List<string>();

                    if (param.TipoParam > 912)
                    {
                        if (param.FORM_FormAlternativa.Count() == 0)
                        {
                            param.ErrorList.Add("Alternativas Pendientes");
                            param.Errores = true;

                            item.Errores = true;
                            item.ErrorList.Add("Pregunta con errores");
                        }

                    }
                    else if (param.TipoParam == 904)
                    {
                        if (String.IsNullOrEmpty(param.IdForm))
                        {
                            param.ErrorList.Add("Selección de Formulario Pendiente");
                            param.Errores = true;
                            item.Errores = true;
                            item.ErrorList.Add("Pregunta con errores");
                        }
                           
                    }
                }

            }



            return PartialView("_modalValidacion", eVA_formularioEvaluacion);

        }

        [HttpPost, ActionName("Activar")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Activar(string IdForm)
        {
            try
            {
                FORM_Formularios formularioEvaluacion = db.FORM_Formularios.Find(IdForm);
                formularioEvaluacion.Activa = true;
                db.Entry(formularioEvaluacion).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return Content($"true;{IdForm}");
            }
            catch (Exception err)
            {
                return Content(err.Message);
            }
        }

        public async Task<ActionResult> Desactivar(string id)
        {
            FORM_Formularios formularioEvaluacion = await db.FORM_Formularios.FindAsync(id);
            return PartialView("_Desactivar", formularioEvaluacion);
        }

        [HttpPost, ActionName("Desactivar")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DesactivarConfirm(string IdForm)
        {
            try
            {
                FORM_Formularios formularioEvaluacion = db.FORM_Formularios.Find(IdForm);
                formularioEvaluacion.Activa = false;
                db.Entry(formularioEvaluacion).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return Content($"true;{IdForm}");
            }
            catch (Exception err)
            {
                return Content(err.Message);
            }
        }

        public ActionResult Test(string id)
        {
            FORM_Formularios formularioEvaluacion = db.FORM_Formularios.Find(id);
            if (formularioEvaluacion == null || !formularioEvaluacion.Activa)
                return HttpNotFound();

            MAE_Empresa MAE_Empresa = db.MAE_Empresa.Find(1);
            ViewBag.Logo = MAE_Empresa.LogoData;

            ViewBag.QR = QRstring(id);

            return View(formularioEvaluacion);
        }
        
        [HttpPost]
        public async Task<ActionResult> UpdateFormEmb(string FormsEmbebidos, string IdPregunta)
        {
            try
            {
                FORM_FormPreguntas pre = await db.FORM_FormPreguntas.FindAsync(IdPregunta);
                pre.IdForm = FormsEmbebidos;

                db.Entry(pre).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return Content("true;" + pre.IdItem);

            }
            catch (Exception err)
            {
                return Content("false;" + err.Message);
            }
        }

        private string QRstring(string Folio)
        {
            QRCodeEncoder qrCodeEncoder = new QRCodeEncoder();
            String encoding = "Byte";
            if (encoding == "Byte")
            {
                qrCodeEncoder.QRCodeEncodeMode = QRCodeEncoder.ENCODE_MODE.BYTE;
            }
            else if (encoding == "AlphaNumeric")
            {
                qrCodeEncoder.QRCodeEncodeMode = QRCodeEncoder.ENCODE_MODE.ALPHA_NUMERIC;
            }
            else if (encoding == "Numeric")
            {
                qrCodeEncoder.QRCodeEncodeMode = QRCodeEncoder.ENCODE_MODE.NUMERIC;
            }
            try
            {
                int scale = Convert.ToInt16(4);
                qrCodeEncoder.QRCodeScale = scale;
            }
            catch (Exception ex)
            {
                //MessageBox.Show("Invalid size!");
                //return;
            }
            try
            {
                int version = Convert.ToInt16(7);
                qrCodeEncoder.QRCodeVersion = version;
            }
            catch (Exception ex)
            {
                //MessageBox.Show("Invalid version !");
            }

            string errorCorrect = "Q";
            if (errorCorrect == "L")
                qrCodeEncoder.QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.L;
            else if (errorCorrect == "M")
                qrCodeEncoder.QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.M;
            else if (errorCorrect == "Q")
                qrCodeEncoder.QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.Q;
            else if (errorCorrect == "H")
                qrCodeEncoder.QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.H;

            string ruta = Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath.TrimEnd('/');

            Image image;
            System.Drawing.Imaging.ImageFormat format = System.Drawing.Imaging.ImageFormat.Jpeg;
            String data = String.Format("{0}/FORMS/Pub/Ejecutar?id={1}", ruta, Folio);
            image = qrCodeEncoder.Encode(data);

            using (MemoryStream ms = new MemoryStream())
            {
                // Convert Image to byte[]
                image.Save(ms, format);
                byte[] imageBytes = ms.ToArray();

                // Convert byte[] to Base64 String
                string base64String = Convert.ToBase64String(imageBytes);
                return base64String;
            }


        }
        private byte[] QRBinary(string Folio)
        {
            QRCodeEncoder qrCodeEncoder = new QRCodeEncoder();
            String encoding = "Byte";
            if (encoding == "Byte")
            {
                qrCodeEncoder.QRCodeEncodeMode = QRCodeEncoder.ENCODE_MODE.BYTE;
            }
            else if (encoding == "AlphaNumeric")
            {
                qrCodeEncoder.QRCodeEncodeMode = QRCodeEncoder.ENCODE_MODE.ALPHA_NUMERIC;
            }
            else if (encoding == "Numeric")
            {
                qrCodeEncoder.QRCodeEncodeMode = QRCodeEncoder.ENCODE_MODE.NUMERIC;
            }
            try
            {
                int scale = Convert.ToInt16(4);
                qrCodeEncoder.QRCodeScale = scale;
            }
            catch (Exception ex)
            {
                //MessageBox.Show("Invalid size!");
                //return;
            }
            try
            {
                int version = Convert.ToInt16(7);
                qrCodeEncoder.QRCodeVersion = version;
            }
            catch (Exception ex)
            {
                //MessageBox.Show("Invalid version !");
            }

            string errorCorrect = "Q";
            if (errorCorrect == "L")
                qrCodeEncoder.QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.L;
            else if (errorCorrect == "M")
                qrCodeEncoder.QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.M;
            else if (errorCorrect == "Q")
                qrCodeEncoder.QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.Q;
            else if (errorCorrect == "H")
                qrCodeEncoder.QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.H;

            string ruta = Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath.TrimEnd('/');

            Image image;
            System.Drawing.Imaging.ImageFormat format = System.Drawing.Imaging.ImageFormat.Jpeg;
            String data = String.Format("{0}/FORMS/Pub/Ejecutar?id={1}", ruta, Folio);
            image = qrCodeEncoder.Encode(data);

            using (MemoryStream ms = new MemoryStream())
            {
                // Convert Image to byte[]
                image.Save(ms, format);
                byte[] imageBytes = ms.ToArray();

                return imageBytes;
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
