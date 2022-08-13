using Galena.Helpers;
using LOD_APR.Areas.Admin.Helpers;
using LOD_APR.Areas.Admin.Models;
using LOD_APR.Areas.GLOD.Models;
using LOD_APR.Areas.GLOD.ModelsViews;
using LOD_APR.Helpers;
using LOD_APR.Models;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace LOD_APR.Areas.GLOD.Controllers
{
    [CustomAuthorize]
    public class FormInformesController : Controller
    {
        private LOD_DB db = new LOD_DB();

        public async Task<ActionResult> IndexIncidentes(int id)
        {
            ViewBag.IdContrato = id;
            ViewBag.EstadoContrato = db.CON_Contratos.Find(id).EstadoContrato;
            ViewBag.Contrato = $"{db.CON_Contratos.Find(id).CodigoContrato} - {db.CON_Contratos.Find(id).NombreContrato}";
            List<FORM_InformesItems> inf = await db.FORM_InformesItems.Where(f => f.FORM_Formularios.Tipo == 1 && f.IdContrato==id).OrderBy(o => o.IdItem).ToListAsync();

            if (ValidaPermisos.ValidaPermisosEnController("0020080000"))
            {

                return View(inf);
            }
            else
            {
                return RedirectToAction("SinPermiso", "../Pub");
            }

        }
        public async Task<ActionResult> GetTableIncidentes(int id)
        {
            List<FORM_InformesItems> inf = await db.FORM_InformesItems.Where(f => f.FORM_Formularios.Tipo == 1 && f.IdContrato == id).OrderBy(o => o.IdItem).ToListAsync();
            return PartialView("_GetTableReportIncidentes", inf);
        }

        public async Task<ActionResult> IndexEjecutivo(int id)
        {
            ViewBag.IdContrato = id;
            ViewBag.EstadoContrato = db.CON_Contratos.Find(id).EstadoContrato;
            ViewBag.Contrato = $"{db.CON_Contratos.Find(id).CodigoContrato} - {db.CON_Contratos.Find(id).NombreContrato}";
            List<FORM_Informes> inf = await db.FORM_Informes.Where(i => i.IdContrato == id && i.Tipo > 1).ToListAsync();
            ViewBag.AddInforme = (inf.Where(i => i.Estado == false).Count() > 0) ? false : true;

            if (ValidaPermisos.ValidaPermisosEnController("0020080000"))
            {

                return View(inf);
            }
            else
            {
                return RedirectToAction("SinPermiso", "../Pub");
            }

            return View(inf);
        }
        public async Task<ActionResult> GetTableEjecutivo(int id)
        {
            List<FORM_Informes> inf = await db.FORM_Informes.Where(i => i.IdContrato == id && i.Tipo > 1).ToListAsync();
            return PartialView("_GetTableEjecutivo", inf);
        }
        public async Task<ActionResult> ViewReport(int id)
        {
            var inf = await db.FORM_Informes.Include(i => i.FORM_InformesItems).Where(w => w.IdEnvio == id).FirstOrDefaultAsync();
            if (inf == null)
                return HttpNotFound();

            return View(inf);
        }

        public async Task<ActionResult> Ingresar(int id)
        {
            FORM_InformesItems inf = await db.FORM_InformesItems.FindAsync(id);
            ViewBag.IdItemInforme = id;
            ViewBag.Titulo = inf.Titulo;

            return PartialView("_FormAddItem", inf.FORM_Formularios);
        }
        public async Task<ActionResult> IngresarIncidente(int idContrato, string formId)
        {
            FORM_Formularios form = await db.FORM_Formularios.FindAsync(formId);
            ViewBag.IdContrato = idContrato;
            ViewBag.IdItemInforme = 0;
            ViewBag.Titulo = form.Titulo;

            return PartialView("_FormAddItem", form);
        }
        public async Task<ActionResult> VerIngreso(int id)
        {
            FORM_InformesItems inf = await db.FORM_InformesItems.FindAsync(id);
            ViewBag.IdItemInforme = id;
            ViewBag.Titulo = inf.Titulo;

            return PartialView("_FormViewItem", inf);
        }

        public async Task<ActionResult> EditIngreso(int id)
        {
            FORM_InformesItems inf = await db.FORM_InformesItems.FindAsync(id);
            if(inf.IdContrato != null)
            {
                ViewBag.IdContrato = inf.IdContrato;
            }else if(inf.FORM_Informes != null)
            {
                ViewBag.IdContrato = null;
            }
            else
            {
                ViewBag.IdContrato = null;
            }
            
            ViewBag.IdItemInforme = id;
            ViewBag.Titulo = inf.Titulo;

            return PartialView("_FormEditItem", inf);
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> PostFormEjecutivo(string __Ejecucion, int IdItemInforme, int? IdContrato)
        {

            if (IdContrato.HasValue && IdItemInforme == 0)
                IdItemInforme = await CreateItemIncidente(IdContrato.Value);

            var itemInforme = await db.FORM_InformesItems.FindAsync(IdItemInforme);
            if (itemInforme == null)
                return Content("false");

            string Username = User.Identity.GetUserName();
 
            try
            {
                EjecucionView obj = JsonConvert.DeserializeObject<EjecucionView>(__Ejecucion);
                obj.ExecutionTime = DateTime.Now;
                obj.UserName = Username;
                obj.Form = new Form() { FormID = itemInforme.IdForm, FormName = itemInforme.Titulo };
                obj.Folio = 0;
                obj.ID = RandomString.GetRandomIDString(10);

                foreach (var o in obj.ItemsData)
                {
                    o.ItemName = db.FORM_FormItems.Find(o.ItemID).Titulo;
                    foreach (var p in o.Fields)
                    {
                        p.FieldName = db.FORM_FormPreguntas.Find(p.FieldID).Titulo;
                        p.Type.TypeName = TiposPreguntas.GetTipo(p.Type.Type).Titulo;
                        p.Type.TypeWidth = db.FORM_FormPreguntas.Find(p.FieldID).Largo;
                        foreach (var a in p.Options)
                        {
                            a.OptionName = db.FORM_FormAlternativa.Find(a.OptionID).Titulo;
                        }
                    }
                }

                foreach(var field in obj.ItemsData[0].Fields)
                {
                    FORM_InformesItemsData data = new FORM_InformesItemsData()
                    {
                        IdItem = IdItemInforme,
                        IdPregunta = field.FieldID,
                        Pregunta = field.FieldName,
                        Respuesta = field.FieldValue
                    };

                    db.FORM_InformesItemsData.Add(data);
                    await db.SaveChangesAsync();
                }

                itemInforme.Usuario = Username;
                itemInforme.Estado = 1;
                itemInforme.FechaDespacho = DateTime.Now;
                db.Entry(itemInforme).State = EntityState.Modified;
                await db.SaveChangesAsync();

               //VERIFICAR SI TODOS LOS FORMULARIOS DEL REPORTE EJECUTIVO ESTAN CERRADOS PARA CERRAR EL REPORTE EN SÍ
                return Content($"true;{itemInforme.IdInforme};{itemInforme.FORM_Formularios.Tipo};{IdContrato}");

            }
            catch (Exception ex)
            {
                return Content("false;" + ex.Message);
            }

        }


        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> PostFormEditEjecutivo(string __Ejecucion, int IdItemInforme, int? IdContrato)
        {

            if (IdContrato.HasValue && IdItemInforme == 0)
                IdItemInforme = await CreateItemIncidente(IdContrato.Value);

            var itemInforme = await db.FORM_InformesItems.FindAsync(IdItemInforme);
            if (itemInforme == null)
                return Content("false");

            string Username = User.Identity.GetUserName();

            try
            {
                EjecucionView obj = JsonConvert.DeserializeObject<EjecucionView>(__Ejecucion);
                obj.ExecutionTime = DateTime.Now;
                obj.UserName = Username;
                obj.Form = new Form() { FormID = itemInforme.IdForm, FormName = itemInforme.Titulo };
                obj.Folio = 0;
                obj.ID = RandomString.GetRandomIDString(10);

                foreach (var o in obj.ItemsData)
                {
                    o.ItemName = db.FORM_FormItems.Find(o.ItemID).Titulo;
                    foreach (var p in o.Fields)
                    {
                        p.FieldName = db.FORM_FormPreguntas.Find(p.FieldID).Titulo;
                        p.Type.TypeName = TiposPreguntas.GetTipo(p.Type.Type).Titulo;
                        p.Type.TypeWidth = db.FORM_FormPreguntas.Find(p.FieldID).Largo;
                        foreach (var a in p.Options)
                        {
                            a.OptionName = db.FORM_FormAlternativa.Find(a.OptionID).Titulo;
                        }
                    }
                }

                List<FORM_InformesItemsData> dataList = db.FORM_InformesItemsData.Where(x => x.IdItem == IdItemInforme).ToList();
                foreach (var item in dataList)
                {
                    foreach (var field in obj.ItemsData[0].Fields)
                    {
                        if (item.IdPregunta.Equals(field.FieldID))
                            item.Respuesta = field.FieldValue;

                        db.Entry(item).State = EntityState.Modified;
                        await db.SaveChangesAsync();
                    }
                }


                itemInforme.Usuario = Username;
                itemInforme.Estado = 1;
                itemInforme.FechaDespacho = DateTime.Now;
                db.Entry(itemInforme).State = EntityState.Modified;
                await db.SaveChangesAsync();

                //VERIFICAR SI TODOS LOS FORMULARIOS DEL REPORTE EJECUTIVO ESTAN CERRADOS PARA CERRAR EL REPORTE EN SÍ
                return Content($"true;{itemInforme.IdInforme};{itemInforme.FORM_Formularios.Tipo};{IdContrato}");

            }
            catch (Exception ex)
            {
                return Content("false;" + ex.Message);
            }

        }


        public async Task<ActionResult> GetTableFormItem(int id, int tipo)
        {
            List<FORM_InformesItems> inf = await db.FORM_InformesItems.Where(f => f.FORM_Formularios.Tipo == tipo && f.IdInforme==id).OrderBy(o => o.FORM_Formularios.Titulo).ToListAsync();
            return PartialView("_GetTableReportItem", inf);
        }


        public ActionResult Create(int IdContrato, int IdTipo)
        {
            FORM_Informes inf = new FORM_Informes()
            {
                IdContrato = IdContrato,
                Tipo = IdTipo,
                Anio = DateTime.Now.Year.ToString()
            };
            ViewBag.Mes = new SelectList(Meses.MAE_Meses().OrderBy(x => x.IdMes), "IdMes", "Nombre", DateTime.Now.Month);

            if (ValidaPermisos.ValidaPermisosEnController("0020080001"))
            {

                return PartialView("_Create", inf);
            }
            else
            {
                return RedirectToAction("SinPermiso", "../Pub");
            }

            
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "IdContrato,Tipo,MesInformado,Mes,Anio")] FORM_Informes formulario)
        {
            try
            {
                formulario.Usuario = User.Identity.GetUserName();
                formulario.Estado = false;
                formulario.FechaCreacion = DateTime.Now;
                //formulario.Mes = Meses.MAE_Meses().Where(m => m.IdMes == Convert.ToInt32(formulario.Mes)).Select(s => s.Nombre).FirstOrDefault();

                db.FORM_Informes.Add(formulario);
                int ok = db.SaveChanges();

                if (ok > 0)
                {
                    List<FORM_Formularios> forms = new List<FORM_Formularios>();
                    if (formulario.Tipo==2)
                        forms = await db.FORM_Formularios.Where(f => f.Tipo > 1).ToListAsync();
                    else
                        forms = await db.FORM_Formularios.Where(f => f.Tipo == 1).ToListAsync();

                    foreach (FORM_Formularios form in forms)
                    {
                        foreach (FORM_FormItems item in form.FORM_FormItems)
                        {
                            FORM_InformesItems itemInf = new FORM_InformesItems()
                            {
                                IdInforme = formulario.IdEnvio,
                                IdForm = form.IdForm,
                                Estado = 0,
                                Titulo = item.Titulo
                            };
                            db.FORM_InformesItems.Add(itemInf);
                            await db.SaveChangesAsync();
                        }
                    }
                }

                return Content("true;" + formulario.IdEnvio);
            }
            catch (Exception ex)
            {
                return Content("false;" + ex.Message);
            }
        }
        public async Task<int> CreateItemIncidente(int IdContrato)
        {
            FORM_Formularios form = await db.FORM_Formularios.FindAsync("bDivYVz1sS");
            int result = 0;
            foreach (FORM_FormItems item in form.FORM_FormItems)
            {
                FORM_InformesItems itemInf = new FORM_InformesItems()
                {
                    IdForm = form.IdForm,
                    Estado = 0,
                    Titulo = item.Titulo,
                    IdContrato = IdContrato
                };
                db.FORM_InformesItems.Add(itemInf);
                await db.SaveChangesAsync();
                result = itemInf.IdItem;
            }

            return result;
        }
        public async Task<ActionResult> Delete(int id)
        {
            FORM_Informes formulario = await db.FORM_Informes.FindAsync(id);

            if (ValidaPermisos.ValidaPermisosEnController("0020080003"))
            {

                return PartialView("_Delete", formulario);
            }
            else
            {
                return RedirectToAction("SinPermiso", "../Pub");
            }

            
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int IdEnvio)
        {
            try
            {
                FORM_Informes formulario = await db.FORM_Informes.FindAsync(IdEnvio);
                int idcontrato = formulario.IdContrato;
                db.FORM_Informes.Remove(formulario);
                await db.SaveChangesAsync();
                return Content("true;" + idcontrato);
            }
            catch (Exception err)
            {
                return Content("false;" + err.Message);
            }
        }

        public async Task<ActionResult> Cerrar(int id)
        {
            FORM_Informes formulario = await db.FORM_Informes.FindAsync(id);
            return PartialView("_Cerrar", formulario);
        }

        [HttpPost, ActionName("Cerrar")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CerrarConfirmed(int IdEnvio)
        {
            try
            {
                FORM_Informes formulario = await db.FORM_Informes.FindAsync(IdEnvio);
                int idcontrato = formulario.IdContrato;
                formulario.Estado = true;
                formulario.FechaDespacho = DateTime.Now;
                db.Entry(formulario).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return Content("true;" + IdEnvio);
            }
            catch (Exception err)
            {
                return Content("false;" + err.Message);
            }
        }

        public JsonResult GetTipoInforme()
        {
            List<SelectListItem> tipo = new List<SelectListItem>();
            tipo.Add(new SelectListItem() { Text = "Reporte de Incidentes", Value = "1" });
            tipo.Add(new SelectListItem() { Text = "Estado del Contrato", Value ="2" });
            tipo.Add(new SelectListItem() { Text = "Prevención de Riesgos", Value = "3" });
            tipo.Add(new SelectListItem() { Text = "Comité Paritario", Value = "4" });
            tipo.Add(new SelectListItem() { Text = "Gestión de Calidad", Value = "6" });
            tipo.Add(new SelectListItem() { Text = "Medio Ambiente", Value = "8" });
            tipo.Add(new SelectListItem() { Text = "Participación Ciudadana", Value = "9" });

            return Json(tipo, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> GetItemInforme(int IdInforme, int IdTipo)
        {
            List<SelectListItem> subtipo = new List<SelectListItem>();
            if (IdTipo > 1)
            {
                List<FORM_InformesItems> informes = await db.FORM_InformesItems.Where(f => f.FORM_Formularios.Tipo == IdTipo && f.IdInforme == IdInforme && f.Estado == 1).OrderBy(o => o.FORM_Formularios.Titulo).ToListAsync();
                foreach (var item in informes)
                    subtipo.Add(new SelectListItem() { Text = item.Titulo, Value = item.IdItem.ToString() });
            }
            else
            {
                var informe = await db.FORM_Informes.FindAsync(IdInforme);
                List<FORM_InformesItems> informes = await db.FORM_InformesItems.Where(f => f.FORM_Formularios.Tipo == 1 && f.IdContrato == informe.IdContrato && f.Estado == 1).OrderBy(o => o.FORM_Formularios.Titulo).ToListAsync();
                foreach (var item in informes)
                    subtipo.Add(new SelectListItem() { Text = $"{item.IdItem} -{item.Titulo} del {item.FechaDespacho.Value.ToShortDateString()}" , Value = item.IdItem.ToString() });
            }
           

            return Json(subtipo, JsonRequestBehavior.AllowGet);
        }

    }
}