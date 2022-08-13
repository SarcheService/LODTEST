using LinqKit;
using LOD_APR.Areas.Admin.Models;
using LOD_APR.Helpers;
using LOD_APR.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace LOD_APR.Areas.Admin.Controllers
{
    [CustomAuthorize]
    public class TipoDocController : Controller
    {
        private LOD_DB db = new LOD_DB();
        private GoogleReCaptchaHelper reCaptchaHelper = new GoogleReCaptchaHelper();
        public async Task<ActionResult> Index()
        {
            List<MAE_TipoDocumento> list = await db.MAE_TipoDocumento.Where(x => x.Activo == true).ToListAsync();

            if (ValidaPermisos.ValidaPermisosEnController("0010110000"))
            {
                return View(list);
            }
            else
            {
                return RedirectToAction("SinPermiso", "../Pub");
            }


            
        }

        public ActionResult Create()
        {
            ViewBag.Titulo = "Nuevo Tipo";
            ViewBag.ClsModal = "hmodal-success";
            ViewBag.Action = "Create";
            MAE_TipoDocumento mae_tipodoc = new MAE_TipoDocumento();
            mae_tipodoc.IdTipo = 0;
            mae_tipodoc.Tipo = "";
            mae_tipodoc.Activo = true;

            List<ClasiBIB> clasi = new List<ClasiBIB>();
            ClasiBIB clasiForm = new ClasiBIB()
            {
                id = 1,
                Clasi = "Formulario"
            };
            ClasiBIB clasiTec = new ClasiBIB()
            {
                id = 2,
                Clasi = "Documento Técnico"
            };
            ClasiBIB clasiAdmin = new ClasiBIB()
            {
                id = 3,
                Clasi = "Documento Administrativo"
            };
            ClasiBIB clasiOtros = new ClasiBIB()
            {
                id = 1,
                Clasi = "Otros"
            };

            clasi.Add(clasiForm); clasi.Add(clasiTec); clasi.Add(clasiAdmin); clasi.Add(clasiOtros);

            ViewBag.TipoClasi = new SelectList((from p in clasi.ToList()
                                                select new
                                                {
                                                    TipoClasi = p.id,
                                                    Clasi = p.Clasi
                                                }),
                                                       "TipoClasi",
                                                       "Clasi");

            if (ValidaPermisos.ValidaPermisosEnController("0010110001"))
            {
                return PartialView("_modalForm", mae_tipodoc);
            }
            else
            {
                return RedirectToAction("SinPermiso", "../Pub");
            }


           
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "IdTipo,Tipo,Descripcion,Activo,TipoClasi")] MAE_TipoDocumento mae_tipodoc)
        {
            try
            {                
                if (ModelState.IsValid)
                {
                    var mAE_TipoDocumento = db.MAE_TipoDocumento.Where(x => x.IdTipo.Equals(mae_tipodoc.IdTipo)).FirstOrDefault();

                    if (!reCaptchaHelper.IsCaptchaValid(Request.Form["foo"]))
                    {
                        var response = "Error en el captcha, Intente nuevamente.";
                        return Content(response);
                    }
                    if (mAE_TipoDocumento != null)
                    {
                        db.Entry(mAE_TipoDocumento).State = EntityState.Modified;
                        await db.SaveChangesAsync();
                        return Content("true");
                    }
                    else
                    {
                        db.MAE_TipoDocumento.Add(mae_tipodoc);
                        await db.SaveChangesAsync();
                        return Content("true");
                    }                    
                }
                else
                {
                    return Content("Ocurrió un error al tratar de guardar los datos");
                }
                
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
        }
        public async Task<ActionResult> Edit(int? id)
        {
            ViewBag.Titulo = "Editar Tipo";
            ViewBag.ClsModal = "hmodal-warning";
            ViewBag.Action = "Edit";
            MAE_TipoDocumento mAE_TipoDoc = await db.MAE_TipoDocumento.FindAsync(id);

            List<ClasiBIB> clasi = new List<ClasiBIB>();
            ClasiBIB clasiForm = new ClasiBIB()
            {
                id = 1,
                Clasi = "Formulario"
            };
            ClasiBIB clasiTec = new ClasiBIB()
            {
                id = 2,
                Clasi = "Documento Técnico"
            };
            ClasiBIB clasiAdmin = new ClasiBIB()
            {
                id = 3,
                Clasi = "Documento Adminsitrativo"
            };
            ClasiBIB clasiOtros = new ClasiBIB()
            {
                id = 1,
                Clasi = "Otros"
            };

            clasi.Add(clasiForm); clasi.Add(clasiTec); clasi.Add(clasiAdmin); clasi.Add(clasiOtros);

            ViewBag.TipoClasi = new SelectList((from p in clasi.ToList()
                                                select new
                                                {
                                                    TipoClasi = p.id,
                                                    Clasi = p.Clasi
                                                }),
                                                       "TipoClasi",
                                                       "Clasi");


            if (ValidaPermisos.ValidaPermisosEnController("0010110002"))
            {
                return PartialView("_modalFormEdit", mAE_TipoDoc);
            }
            else
            {
                return RedirectToAction("SinPermiso", "../Pub");
            }

            
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "IdTipo,Tipo,Descripcion,Vencimiento,Activo,TipoClasi")] MAE_TipoDocumento mae_tipodoc)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (!reCaptchaHelper.IsCaptchaValid(Request.Form["foo"]))
                    {
                        var response = "Error en el captcha, Intente nuevamente.";
                        return Content(response);
                    }
                    var docExistente = db.MAE_TipoDocumento.Find(mae_tipodoc.IdTipo);
                    docExistente.Descripcion = mae_tipodoc.Descripcion;
                    docExistente.Tipo = mae_tipodoc.Tipo;
                    docExistente.Activo = mae_tipodoc.Activo;
                    db.Entry(docExistente).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                    return Content("true");
                }
                catch (Exception ex)
                {
                    return Content(ex.Message);
                }
            }
            else
            {
                return Content("Ocurrió un error al tratar de guardar los datos");
            }
        }
        public async Task<ActionResult> Delete(int? id)
        {
            MAE_TipoDocumento mAE_TipoDocumento = await db.MAE_TipoDocumento.FindAsync(id);

            if (ValidaPermisos.ValidaPermisosEnController("0010110003"))
            {
                return PartialView("_Delete", mAE_TipoDocumento);
            }
            else
            {
                return RedirectToAction("SinPermiso", "../Pub");
            }

           
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int IdTipo)
        {
            try
            {
                MAE_TipoDocumento mae_tipodoc = await db.MAE_TipoDocumento.FindAsync(IdTipo);
                db.MAE_TipoDocumento.Remove(mae_tipodoc);
                await db.SaveChangesAsync();
                return Content("true");
            }
            catch (Exception err)
            {
                return Content(err.Message);
            }

        }

        public async Task<ActionResult> getTable(string id)
        {
            List<MAE_TipoDocumento> list = new List<MAE_TipoDocumento>();
            int idN = Convert.ToInt32(id);
            list = await db.MAE_TipoDocumento.Where(x => x.Activo == true).ToListAsync();
            return PartialView("_getTable", list);
        }

        public ActionResult getJsonTipos(string q)
        {

            string[] Words = q.Split(' ');

            var predicate = PredicateBuilder.True<MAE_TipoDocumento>();

            foreach (var Word in Words)
            {
                if (!string.IsNullOrEmpty(Word))
                {
                    predicate = predicate.And(x => x.Tipo.ToUpper().Contains(Word.ToUpper()));
                }
            }


            IQueryable<Select2Tipo> IQ_Tipo = db.MAE_TipoDocumento.AsExpandable().Where(predicate).Select(x => new Select2Tipo { id = x.IdTipo, Etiqueda = x.Tipo });

            Select2Tipo aux = new Select2Tipo()
            {
                id= 0,
                Etiqueda = ""
            };
            
            var tipos = IQ_Tipo.ToList();

            tipos.Add(aux);

            List<AutoSearch> search = new List<AutoSearch>();
            foreach (var item in tipos)
            {
                search.Add(new AutoSearch()
                {
                    id = item.id.ToString(),
                    name = item.Etiqueda.ToString()
                });
            }

            string json = JsonConvert.SerializeObject(search);
            return Content(json);

        }

        public class ClasiBIB
        {
            public int id { get; set; }
            public string Clasi { get; set; }
        }

        public class docType
        {
            public int id { get; set; }
            public string Tipo { get; set; }
        }
        public class Select2Tipo
        {
            public int id { get; set; }
            public string Etiqueda { get; set; }
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