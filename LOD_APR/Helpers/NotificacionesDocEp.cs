using System;
using LOD_APR.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;
using System.Data.Entity;
using static LOD_APR.Helpers.HelperEmails;
using System.IO;
using System.Web;
using LOD_APR.Areas.Admin.Models;

namespace LOD_APR.Helpers
{

    public class NotificacionesDocEp
    {
       
        //ER- Devolvemos True si los documentos están todos cargados y enviaremos un email al administrado del contrato para que realice la Revisión de estos 
        public static bool DocObligCargados(int IdCartulaEp)
        {
            LOD_DB db = new LOD_DB();
            //Buscamos la caratula y los documentos correspondientes que debiera traer.
            ASP_epCaratulaContrato caratulaEp = db.ASP_epCaratulaContrato.Find(IdCartulaEp);
            caratulaEp.ListaDocumentosEp = db.ASP_DocReqEP.
                                           Where(x => x.Activo && x.IdTipoEP == caratulaEp.IdTipoEP).
                                           OrderBy(x => x.Nombre).ToList();
            //Revisamos si existen documentos adjuntos Obligatorios Y Reemplazables Requeridos.

            foreach (ASP_DocReqEP TipoDoc in caratulaEp.ListaDocumentosEp)
            {
                //Aca debemos preguntar por los obligatorios de carga         
                //Condiciones 0:Obligatorio / 1:Reemplazable / 2:No Obligatorio
                if (TipoDoc.Condicion != 2)//Preguntamos si es obligatorio o reemplazable
                {
                    //Preguntamos por la presencia de un obligatorio
                    if (TipoDoc.Condicion == 0 && db.ASP_docEP.Where(x => x.IdCaratulaEP == IdCartulaEp && x.IdDocReqEp == TipoDoc.IdDocReqEp).Count() == 0)
                    {
                        return false;//si no esta el obligatorio devolvemos false
                    }
                    //Preguntamos por si esta cargado el archivo reemplazable
                    else if (db.ASP_docEP.Where(x => x.IdCaratulaEP == IdCartulaEp && x.IdDocReqEp == TipoDoc.IdDocReqEp).Count() == 0)
                    {
                        //si no esta cargado el archivo preguntamos por la presencia de un reemplazable
                        if (db.ASP_docEP.Where(x => x.IdCaratulaEP == IdCartulaEp && x.IdDocReqEp == TipoDoc.IdDocReemplazo).Count() == 0)
                        {
                            return false;//si no esta el reemplazante, devolvemos false
                        }
                    }
                }
            }
            return true;
        }

        public static bool DocObligAprobados(int IdCartulaEp)
        {
            LOD_DB db = new LOD_DB();
            //Preguntamos si estan todos debidamente cargados
            if (!DocObligCargados(IdCartulaEp))
            {
                return false;
            }
            //Consultamos la caratula y traemos los documentos asociados
            List<ASP_docEP> ListaDoc = db.ASP_docEP.Where(x => x.IdCaratulaEP == IdCartulaEp).ToList();
            //Recorremos la lista de documentos cargado y vemos si estan aprobados los obligatorios y reemplazables
            foreach (ASP_docEP Doc in ListaDoc)
            {
                //Estados 0:No cargado, 1:Cargado, 2:Aprobado, 3:Rechazado
                //Condiciones 0:Obligatorio / 1:Reemplazable / 2:No Obligatorio
                if (db.ASP_DocReqEP.Find(Doc.IdDocReqEp).Condicion == 0 && Doc.Estado != 2)
                {
                    return false;
                }
                else if (db.ASP_DocReqEP.Find(Doc.IdDocReqEp).Condicion == 1 && Doc.Estado != 2)
                {
                    int IdReemplazo = Convert.ToInt32(db.ASP_DocReqEP.Find(Doc.IdDocReqEp).IdDocReemplazo);
                    if (db.ASP_DocReqEP.Find(IdReemplazo).Condicion == 1 && Doc.Estado != 2)
                    {
                        return false;
                    }
                }
            }
            CambioEstadoCaratula(IdCartulaEp, 3);
            return true;
        }

        public static async Task CambioEstadoCaratula(int IdEpCaratula, int estado)
        {
            LOD_DB db = new LOD_DB();
            ASP_epCaratulaContrato caratulaEditEstado = db.ASP_epCaratulaContrato.Find(IdEpCaratula);
            if (estado == 3)
            {
                if (caratulaEditEstado.EstadoEP == 1)
                {
                    caratulaEditEstado.EstadoEP = estado;
                }
                else if (caratulaEditEstado.EstadoEP == 2)
                {
                    caratulaEditEstado.EstadoEP = 4;

                    ////Arreglar este problema
                    //Helper_Evaluacion HE = new Helper_Evaluacion();
                    ////HE Estados 1: No corresponde Evaluar;  
                    ////          -1: No existe Programación;
                    ////           2: Evaluación Creada; 
                    ////          -2: Corresponde pero Faltan Roles ; 
                    ////           3: Evaluación Realizada y pasen los 2 dias;
                    ////          -3: Evaluación sin firma de Prev y M.A.; 
                    //int RespHE = HE.GetEstadoEvaluacion(caratulaEditEstado.IdContrato, Convert.ToInt32(Math.Round(caratulaEditEstado.PorcAvanceFinanAcom, 0)));

                    ////Ver logica de Notificaciones y alertas
                    //switch (RespHE)
                    //{
                    //    case -1:
                    //        //return Content("No existe programación de evaluación por porcentaje de avance");
                    //    case -2:
                    //        //return Content("Roles Faltantes en el Contrato");
                    //    case 2:
                    //        //Se gatillan Notificaciones para evaluar; 
                    //        break;
                    //}
                    ASP_aprobacionesEP aSP_AprobacionesEP = new ASP_aprobacionesEP()
                    {
                        IdAprobador = Convert.ToInt32(caratulaEditEstado.ASP_contratos.IdRespMandante),
                        IdCaratulaEP = caratulaEditEstado.IdCaratulaEP,
                        FechaAprobacion = DateTime.Now,
                        Activo = true,
                        EstadoEP = caratulaEditEstado.EstadoEP,
                        ResolucionAprueba = false
                    };

                    if (db.ASP_aprobacionesEP.Where(x => x.IdCaratulaEP == caratulaEditEstado.IdCaratulaEP && x.Activo).Count() > 0)
                    {
                        ASP_aprobacionesEP ActivoAprob = db.ASP_aprobacionesEP.Where(x => x.IdCaratulaEP == caratulaEditEstado.IdCaratulaEP && x.Activo).First();
                        ActivoAprob.Activo = false;
                        db.Entry(ActivoAprob).State = EntityState.Modified;
                    }
                    db.ASP_aprobacionesEP.Add(aSP_AprobacionesEP);
                }
                db.Entry(caratulaEditEstado).State = EntityState.Modified;
                await db.SaveChangesAsync();
            }
        }

       

        public static async Task  AvisoDocRechazado(int IdEpCaratula, int IdDocReqEP)
        {
            LOD_DB db = new LOD_DB();
            //Obtenemos el nombre del Estado de Pago
            int NroEP = db.ASP_epCaratulaContrato.Find(IdEpCaratula).NumeroEP;
            string NomRef = db.ASP_epCaratulaContrato.Find(IdEpCaratula).RefEP;
            string NombreEP = "EP N°" + NroEP.ToString();
            if (NomRef != "" || NomRef != null)
            {
                NombreEP = NomRef;
            }

            //Buscamos codigo y nombre del contrato
            ASP_contratos Contrato = db.ASP_contratos.Find(db.ASP_epCaratulaContrato.Find(IdEpCaratula).IdContrato);

            //Buscar Email del Administrador de la contratista
            string idAdminContrato = db.SEG_UserContacto.Where(x => x.IdContacto == Contrato.IdAdminContrato).First().UserId;
            string EmailAdminContrato = db.Users.Find(idAdminContrato).Email;
            //Mandamos el los datos del documento rechazado y motivo de rechazo
            ASP_docEP docEpRechazado = db.ASP_docEP.Where(x => x.IdCaratulaEP == IdEpCaratula && x.IdDocReqEp == IdDocReqEP).First();

            //Formateamos el Email a Mandar
            //Envío Detalle
            List<ItemCorreo> items = new List<ItemCorreo>();
            if (Contrato.CodigoContrato != null && Contrato.CodigoContrato != "")
            {
                Contrato.NombreContrato = Contrato.CodigoContrato + "- " + Contrato.NombreContrato;
            }
            items.Add(new ItemCorreo() { Item = "Contrato", Descripcion = Contrato.NombreContrato });
            items.Add(new ItemCorreo() { Item = "Caratula Estado de Pago                   ", Descripcion = NombreEP });
            items.Add(new ItemCorreo() { Item = "Fecha de Rechazo", Descripcion = docEpRechazado.FechaRechazo.ToString() });
            items.Add(new ItemCorreo() { Item = "Documento Rechazado", Descripcion = docEpRechazado.ASP_DocReqEP.Nombre });
             items.Add(new ItemCorreo() { Item = "Motivo Rechazado", Descripcion = docEpRechazado.MotivoRechazo });
            items.Add(new ItemCorreo() { Item = "N° de Rechazos", Descripcion = docEpRechazado.NumRechazos.ToString() });

            //Obtengo el Cuerpo del Correo
            string bodyFormateado = formatearCorreo(3, items, "Rechazo Documento EP");

            //A quien Envío el Correo
            List<Email> Correo = new List<Email>();

            if (EmailAdminContrato != null || EmailAdminContrato != string.Empty)
            {
                Correo.Add(new Email { Correo = EmailAdminContrato, Subject = "Rechazo Documento cargado a Estado de Pago", Body = bodyFormateado });
            }
            await sendEmail(Correo);
        }


        public static async Task AvisoCargaDocCompletos(int IdEpCaratula)
        {
            LOD_DB db = new LOD_DB();
            //Verificamos si el corresponde Envíar email
            if (DocObligCargados(IdEpCaratula))
            {
                //Obtenemos el nombre del Estado de Pago
                int NroEP = db.ASP_epCaratulaContrato.Find(IdEpCaratula).NumeroEP;
                string NomRef = db.ASP_epCaratulaContrato.Find(IdEpCaratula).RefEP;
                string NombreEP = "EP N°" + NroEP.ToString();
                if (NomRef != "" || NomRef != null)
                {
                    NombreEP = NomRef;
                }

                //Buscamos codigo y nombre del contrato
                ASP_contratos Contrato = db.ASP_contratos.Find(db.ASP_epCaratulaContrato.Find(IdEpCaratula).IdContrato);

                //Buscar Email del Administrador de la contratista
                string idAdminContrato = db.SEG_UserContacto.Where(x => x.IdContacto == Contrato.IdAdminContrato).First().UserId;
                string EmailAdminContrato = db.Users.Find(idAdminContrato).Email;
                //Buscamos Email del ITO encargado del contrato
                string idIto = db.SEG_UserPersonal.Where(x => x.IdPersonal == Contrato.IdRespMandante).First().UserId;
                string EmailIto = db.Users.Find(idIto).Email;



                //Formateamos el Email a Mandar
                //Envío Detalle
                List<ItemCorreo> items = new List<ItemCorreo>();
                if (Contrato.CodigoContrato != null && Contrato.CodigoContrato != "")
                {
                    Contrato.NombreContrato = Contrato.CodigoContrato + "- " + Contrato.NombreContrato;
                }
                items.Add(new ItemCorreo() { Item = "Contrato", Descripcion = Contrato.NombreContrato });
                items.Add(new ItemCorreo() { Item = "Caratula Estado de Pago                   ", Descripcion = NombreEP });

                //Obtengo el Cuerpo del Correo
                string bodyFormateado = formatearCorreo(1, items, "Documentos obligatorios cargados a EP");

                //A quien Envío el Correo
                List<Email> Correo = new List<Email>();

                if (EmailAdminContrato != null || EmailAdminContrato != string.Empty)
                {
                    Correo.Add(new Email { Correo = EmailAdminContrato, Subject = "Documentos obligatorios cargados a Estado de Pago para Revisión", Body = bodyFormateado });
                }
                if (EmailIto != null || EmailIto != string.Empty)
                {
                    Correo.Add(new Email { Correo = EmailIto, Subject = "Documentos obligatorios cargados a Estado de Pago para Revisión", Body = bodyFormateado });
                }
                await sendEmail(Correo);
            }
        }

        public static async Task AvisoAprobDocCompletos(int IdEpCaratula)
        {
            LOD_DB db = new LOD_DB();
            //Verificamos si el corresponde Envíar email
            if (DocObligAprobados(IdEpCaratula))
            {
                //Obtenemos el nombre del Estado de Pago
                int NroEP = db.ASP_epCaratulaContrato.Find(IdEpCaratula).NumeroEP;
                string NomRef = db.ASP_epCaratulaContrato.Find(IdEpCaratula).RefEP;
                string NombreEP = "EP N°" + NroEP.ToString();
                if (NomRef != "" || NomRef != null)
                {
                    NombreEP = NomRef;
                }

                //Buscamos codigo y nombre del contrato
                ASP_contratos Contrato = db.ASP_contratos.Find(db.ASP_epCaratulaContrato.Find(IdEpCaratula).IdContrato);

                //Buscar Email del Administrador de la contratista
                string idAdminContrato = db.SEG_UserContacto.Where(x => x.IdContacto == Contrato.IdAdminContrato).First().UserId;
                string EmailAdminContrato = db.Users.Find(idAdminContrato).Email;
                //Buscamos Email del ITO encargado del contrato
                string idIto = db.SEG_UserPersonal.Where(x => x.IdPersonal == Contrato.IdRespMandante).First().UserId;
                string EmailIto = db.Users.Find(idIto).Email;

                //Formateamos el Email a Mandar
                //Envío Detalle
                List<ItemCorreo> items = new List<ItemCorreo>();
                if (Contrato.CodigoContrato != null && Contrato.CodigoContrato != "")
                {
                    Contrato.NombreContrato = Contrato.CodigoContrato + "- " + Contrato.NombreContrato;
                }
                items.Add(new ItemCorreo() { Item = "Contrato", Descripcion = Contrato.NombreContrato });
                items.Add(new ItemCorreo() { Item = "Caratula Estado de Pago                   ", Descripcion = NombreEP });

                //Obtengo el Cuerpo del Correo
                string bodyFormateado = formatearCorreo(2, items, "Documentos obligatorios aprobados");

                //A quien Envío el Correo
                List<Email> Correo = new List<Email>();

                if (EmailAdminContrato != null || EmailAdminContrato != string.Empty)
                {
                    Correo.Add(new Email { Correo = EmailAdminContrato, Subject = "Documentos Aprobados del Estado de Pago", Body = bodyFormateado });
                }
                if (EmailIto != null || EmailIto != string.Empty)
                {
                    Correo.Add(new Email { Correo = EmailIto, Subject = "Documentos Aprobados del Estado de Pago", Body = bodyFormateado });
                }
                await sendEmail(Correo);
            }
        }


        public static string formatearCorreo(int tipo, List<ItemCorreo> items, string titulo)
        {
            LOD_DB db = new LOD_DB();
            MAE_Empresa empresa = db.MAE_Empresa.Find(1);
            string rutaImg = HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Authority + HttpContext.Current.Request.ApplicationPath.TrimEnd('/') + empresa.LogoData.Substring(1);
            string encabezado = string.Empty;
            string comentario = "Esta notificación le ha llega a usted por ser responsable de cargar o revisar la documentación.";

            switch (tipo)
            {
                case 1: //Aviso de Archivos Cargados
                    encabezado = "Se informa que se encuentran cargados todos los documentos requeridos para el Estado de Pago, por favor proceder a su revisión.";
                    break;
                case 2: //Aviso de Archivos Aprobados
                    encabezado = "Se informa que se encuentran Aprobados todos los documentos requeridos, para el Estado de Pago, por lo cual ya se puede crear un nuevo Estado de Pago de ser Requerido.";
                    break;
                case 3: //Aviso de Archivos Rechazados
                    encabezado = "Se informa que ha sido Rechazado uno de los documentos cargados en el Estado de Pago.";
                    break;
            }

            try
            {
                if (tipo == 3)
                {
                    string html = File.ReadAllText(HttpContext.Current.Server.MapPath("/Helpers/CorreoRechazo.html"));
                    //string html = PartialView("_CorreoRechazo").RenderToString();
                    string htmlFormateado =
                        String.Format(html,
                                      rutaImg,
                                      titulo,
                                      encabezado,
                                      items[0].Item.ToString(),
                                      items[0].Descripcion.ToString(),
                                      items[1].Item.ToString(),
                                      items[1].Descripcion.ToString(),
                                      items[2].Item.ToString(),
                                      items[2].Descripcion.ToString(),
                                      items[3].Item.ToString(),
                                      items[3].Descripcion.ToString(),
                                      items[4].Item.ToString(),
                                      items[4].Descripcion.ToString(),
                                      items[5].Item.ToString(),
                                      items[5].Descripcion.ToString(),
                                      comentario);
                    return htmlFormateado;
                }
                else
                {
                    string html = File.ReadAllText(HttpContext.Current.Server.MapPath("/Helpers/Correo.html"));
                    string htmlFormateado =
                        String.Format(html,
                                      rutaImg,
                                      titulo,
                                      encabezado,
                                      items[0].Item.ToString(),
                                      items[0].Descripcion.ToString(),
                                      items[1].Item.ToString(),
                                      items[1].Descripcion.ToString(),
                                      comentario);
                    return htmlFormateado;
                }

            }
            catch (Exception ex)
            {
                return ("Error de Email");
            }

        }
    }
}
