using LOD_APR.Areas.GLOD.Models;
using LOD_APR.Helpers;
using LOD_APR.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace LOD_APR.Areas.GLOD.Helpers
{
    public class GLOD_Notificaciones
    {

        private LOD_DB db = new LOD_DB();

        public async Task<int> NotificarPublicacion(LOD_Anotaciones anotacion, string userId)
        {

            List<LOD_UserAnotacion> listadoUser = db.LOD_UserAnotacion.Where(x => x.IdAnotacion == anotacion.IdAnotacion).Include(x => x.LOD_UsuarioLod).ToList();
            int entero = 0;
            foreach (var item in listadoUser)
            {
                if (item.LOD_UsuarioLod.UserId == userId)
                {
                    MailServer email = new MailServer($"Notificación de Anotación: {anotacion.Correlativo} - {anotacion.Titulo}", anotacion.UsuarioRemitente.UserName);
                    email.InsertBodyItem("Notificación Anotación: " + anotacion.Correlativo, MailServer.ItemType.H3, MailServer.Align.Center);
                    email.InsertLine();
                    email.InsertSpace();
                    email.InsertBodyItem($"Estimado usuario, le informamos que la anotación: {anotacion.Correlativo} - {anotacion.Titulo}, Asociada al Libro de Obra {anotacion.LOD_LibroObras.NombreLibroObra} ha sido publicada", MailServer.ItemType.p, MailServer.Align.Left);
                    //FLEXBOX METODO 1: CON CLASE FUERTEMENTE TIPADA
                    email.InsertFlexBoxTable(anotacion, s => s.Correlativo, s => s.Titulo, s => s.UsuarioRemitente.NombreCompleto,
                        s => s.FechaPub, s => s.MAE_SubtipoComunicacion.Nombre, s => s.LOD_LibroObras.CON_Contratos.NombreContrato);
                    email.InsertSpace();

                    entero = await email.SendEmail(item.LOD_UsuarioLod.ApplicationUser.UserName);
                } else if (item.EsPrincipal)
                {
                    MailServer email = new MailServer($"Receptor Principal de Anotación: {anotacion.Correlativo} - {anotacion.Titulo}", anotacion.UsuarioRemitente.UserName);
                    email.InsertBodyItem("Recepción Principal Anotación: " + anotacion.Correlativo, MailServer.ItemType.H3, MailServer.Align.Center);
                    email.InsertLine();
                    email.InsertSpace();
                    email.InsertBodyItem($"Estimado usuario, le informamos ha sido seleccionado como receptor principal para la anotación {anotacion.Correlativo} - {anotacion.Titulo}, Asociada al Libro de Obra {anotacion.LOD_LibroObras.NombreLibroObra}.", MailServer.ItemType.p, MailServer.Align.Left);
                    //FLEXBOX METODO 1: CON CLASE FUERTEMENTE TIPADA
                    email.InsertFlexBoxTable(anotacion, s => s.Correlativo, s => s.Titulo, s => s.UsuarioRemitente.NombreCompleto,
                        s => s.FechaPub, s => s.MAE_SubtipoComunicacion.Nombre, s => s.LOD_LibroObras.CON_Contratos.NombreContrato);
                    email.InsertSpace();

                    entero = await email.SendEmail(item.LOD_UsuarioLod.ApplicationUser.UserName);
                }
                else if (item.EsRespRespuesta)
                {
                    MailServer email = new MailServer($"Receptor de Anotación: {anotacion.Correlativo} - {anotacion.Titulo}", anotacion.UsuarioRemitente.UserName);
                    email.InsertBodyItem("Recepción Anotación: " + anotacion.Correlativo, MailServer.ItemType.H3, MailServer.Align.Center);
                    email.InsertLine();
                    email.InsertSpace();
                    email.InsertBodyItem($"Estimado usuario, le informamos ha sido seleccionado como responsable de respuesta para la anotación {anotacion.Correlativo} - {anotacion.Titulo}, Asociada al Libro de Obra {anotacion.LOD_LibroObras.NombreLibroObra}.", MailServer.ItemType.p, MailServer.Align.Left);
                    //FLEXBOX METODO 1: CON CLASE FUERTEMENTE TIPADA
                    email.InsertFlexBoxTable(anotacion, s => s.Correlativo, s => s.Titulo, s => s.UsuarioRemitente.NombreCompleto,
                        s => s.FechaPub, s => s.MAE_SubtipoComunicacion.Nombre, s => s.LOD_LibroObras.CON_Contratos.NombreContrato);
                    email.InsertSpace();

                    entero = await email.SendEmail(item.LOD_UsuarioLod.ApplicationUser.UserName);
                }
                else
                {
                    MailServer email = new MailServer($"Receptor de Anotación: {anotacion.Correlativo} - {anotacion.Titulo}", anotacion.UsuarioRemitente.UserName);
                    email.InsertBodyItem("Recepción Anotación: " + anotacion.Correlativo, MailServer.ItemType.H3, MailServer.Align.Center);
                    email.InsertLine();
                    email.InsertSpace();
                    email.InsertBodyItem($"Estimado usuario, le informamos ha sido seleccionado como receptor para la anotación {anotacion.Correlativo} - {anotacion.Titulo}, Asociada al Libro de Obra {anotacion.LOD_LibroObras.NombreLibroObra}.", MailServer.ItemType.p, MailServer.Align.Left);
                    //FLEXBOX METODO 1: CON CLASE FUERTEMENTE TIPADA
                    email.InsertFlexBoxTable(anotacion, s => s.Correlativo, s => s.Titulo, s => s.UsuarioRemitente.NombreCompleto,
                        s => s.FechaPub, s => s.MAE_SubtipoComunicacion.Nombre, s => s.LOD_LibroObras.CON_Contratos.NombreContrato);
                    email.InsertSpace();

                    entero = await email.SendEmail(item.LOD_UsuarioLod.ApplicationUser.UserName);
                }

            }

            return entero;
        }

        public async Task<int> NotificarRespuesta(LOD_Anotaciones anotacion, string userId)
        {

            List<LOD_UserAnotacion> listadoUser = db.LOD_UserAnotacion.Where(x => x.IdAnotacion == anotacion.IdAnotacion).Include(x => x.LOD_UsuarioLod).ToList();
            LOD_ReferenciasAnot referenciasAnot = db.LOD_ReferenciasAnot.Where(x => x.IdAnotacion == anotacion.IdAnotacion).FirstOrDefault();
            LOD_Anotaciones anotacionReferenciada = db.LOD_Anotaciones.Find(referenciasAnot.IdAnontacionRef);
            LOD_UserAnotacion userAnotacion = db.LOD_UserAnotacion.Where(x => x.LOD_UsuarioLod.UserId == anotacionReferenciada.UserId && x.IdAnotacion == anotacionReferenciada.IdAnotacion).FirstOrDefault();
            if(!listadoUser.Contains(userAnotacion))
                listadoUser.Add(userAnotacion);
            int entero = 0;
            foreach (var item in listadoUser)
            {
                if (item.LOD_UsuarioLod.UserId == userId)
                {
                    MailServer email = new MailServer($"Notificación de Respuesta: {anotacion.Correlativo} - {anotacion.Titulo}", anotacion.UsuarioRemitente.UserName);
                    email.InsertBodyItem("Notificación Respuesta: " + anotacion.Correlativo, MailServer.ItemType.H3, MailServer.Align.Center);
                    email.InsertLine();
                    email.InsertSpace(); 
                    email.InsertBodyItem($"Estimado usuario, le informamos que la anotación: {anotacion.Correlativo} - {anotacion.Titulo}, Asociada al Libro de Obra {anotacion.LOD_LibroObras.NombreLibroObra} ha sido publicada", MailServer.ItemType.p, MailServer.Align.Left);
                    //FLEXBOX METODO 1: CON CLASE FUERTEMENTE TIPADA
                    email.InsertFlexBoxTable(anotacion, s => s.Correlativo, s => s.Titulo, s => s.UsuarioRemitente.NombreCompleto,
                        s => s.FechaPub, s => s.MAE_SubtipoComunicacion.Nombre, s => s.LOD_LibroObras.CON_Contratos.NombreContrato);
                    email.InsertSpace();

                    entero = await email.SendEmail(item.LOD_UsuarioLod.ApplicationUser.UserName);
                }
                else if (item.LOD_UsuarioLod.UserId == anotacionReferenciada.UserId && item.IdAnotacion == anotacionReferenciada.IdAnotacion)
                {
                    MailServer email = new MailServer($"Responsable de Anotación: {anotacionReferenciada.Correlativo}", anotacion.UsuarioRemitente.UserName);
                    email.InsertBodyItem("Responsable de Anotación: " + anotacionReferenciada, MailServer.ItemType.H3, MailServer.Align.Center);
                    email.InsertLine();
                    email.InsertSpace();
                    email.InsertBodyItem($"Estimado usuario, le informamos que la anotación {anotacionReferenciada.Correlativo} - {anotacionReferenciada.Titulo}, " +
                        $"Asociada al Libro de Obra {anotacionReferenciada.LOD_LibroObras.NombreLibroObra} ha sido respondida en la Anotación: {anotacion.Correlativo}.", MailServer.ItemType.p, MailServer.Align.Left);
                    //FLEXBOX METODO 1: CON CLASE FUERTEMENTE TIPADA
                    email.InsertFlexBoxTable(anotacionReferenciada, s => s.Correlativo, s => s.Titulo, s => s.UsuarioRemitente.NombreCompleto,
                       s => s.FechaPub, s => s.MAE_SubtipoComunicacion.Nombre, s => s.LOD_LibroObras.CON_Contratos.CodigoContrato);
                    email.InsertFlexBoxTable(anotacion, s => s.Correlativo, s => s.Titulo, s => s.UsuarioRemitente.NombreCompleto,
                       s => s.FechaPub, s => s.MAE_SubtipoComunicacion.Nombre, s => s.LOD_LibroObras.CON_Contratos.CodigoContrato);
                    email.InsertSpace();

                    entero = await email.SendEmail(item.LOD_UsuarioLod.ApplicationUser.UserName);
                }
                else if (item.EsPrincipal)
                {
                    MailServer email = new MailServer($"Receptor Principal de Anotación: {anotacion.Correlativo}", anotacion.UsuarioRemitente.UserName);
                    email.InsertBodyItem("Recepción Principal Anotación: " + anotacion.Correlativo, MailServer.ItemType.H3, MailServer.Align.Center);
                    email.InsertLine();
                    email.InsertSpace();
                    email.InsertBodyItem($"Estimado usuario, le informamos ha sido seleccionado como receptor principal para la anotación {anotacion.Correlativo} - {anotacion.Titulo}, Asociada al Libro de Obra {anotacion.LOD_LibroObras.NombreLibroObra}.", MailServer.ItemType.p, MailServer.Align.Left);
                    //FLEXBOX METODO 1: CON CLASE FUERTEMENTE TIPADA
                    email.InsertFlexBoxTable(anotacion, s => s.Correlativo, s => s.Titulo, s => s.UsuarioRemitente.NombreCompleto,
                        s => s.FechaPub, s => s.MAE_SubtipoComunicacion.Nombre, s => s.LOD_LibroObras.CON_Contratos.NombreContrato);
                    email.InsertSpace();

                    entero = await email.SendEmail(item.LOD_UsuarioLod.ApplicationUser.UserName);
                }
                else if (item.EsRespRespuesta)
                {
                    MailServer email = new MailServer($"Receptor de Anotación: {anotacion.Correlativo}", anotacion.UsuarioRemitente.UserName);
                    email.InsertBodyItem("Recepción Anotación: " + anotacion.Correlativo, MailServer.ItemType.H3, MailServer.Align.Center);
                    email.InsertLine();
                    email.InsertSpace();
                    email.InsertBodyItem($"Estimado usuario, le informamos ha sido seleccionado como responsable de respuesta para la anotación {anotacion.Correlativo} - {anotacion.Titulo}, Asociada al Libro de Obra {anotacion.LOD_LibroObras.NombreLibroObra}.", MailServer.ItemType.p, MailServer.Align.Left);
                    //FLEXBOX METODO 1: CON CLASE FUERTEMENTE TIPADA
                    email.InsertFlexBoxTable(anotacion, s => s.Correlativo, s => s.Titulo, s => s.UsuarioRemitente.NombreCompleto,
                        s => s.FechaPub, s => s.MAE_SubtipoComunicacion.Nombre, s => s.LOD_LibroObras.CON_Contratos.NombreContrato);
                    email.InsertSpace();

                    entero = await email.SendEmail(item.LOD_UsuarioLod.ApplicationUser.UserName);
                }
                else
                {
                    MailServer email = new MailServer($"Receptor de Anotación: {anotacion.Correlativo}", anotacion.UsuarioRemitente.UserName);
                    email.InsertBodyItem("Recepción Anotación: " + anotacion.Correlativo, MailServer.ItemType.H3, MailServer.Align.Center);
                    email.InsertLine();
                    email.InsertSpace();
                    email.InsertBodyItem($"Estimado usuario, le informamos ha sido seleccionado como receptor para la anotación {anotacion.Correlativo} - {anotacion.Titulo}, Asociada al Libro de Obra {anotacion.LOD_LibroObras.NombreLibroObra}.", MailServer.ItemType.p, MailServer.Align.Left);
                    //FLEXBOX METODO 1: CON CLASE FUERTEMENTE TIPADA
                    email.InsertFlexBoxTable(anotacion, s => s.Correlativo, s => s.Titulo, s => s.UsuarioRemitente.NombreCompleto,
                        s => s.FechaPub, s => s.MAE_SubtipoComunicacion.Nombre, s => s.LOD_LibroObras.CON_Contratos.NombreContrato);
                    email.InsertSpace();

                    entero = await email.SendEmail(item.LOD_UsuarioLod.ApplicationUser.UserName);
                }

            }
            return entero;
        }

        public async Task<int> NotificarVB(LOD_Anotaciones anotacion, string userId)
        {

            List<LOD_UserAnotacion> listadoUser = db.LOD_UserAnotacion.Where(x => x.IdAnotacion == anotacion.IdAnotacion).Include(x => x.LOD_UsuarioLod).ToList();
            int entero = 0;
            foreach (var item in listadoUser)
            {
                if (item.LOD_UsuarioLod.UserId == userId)
                {
                    MailServer email = new MailServer($"Notificación de VB para: {anotacion.Correlativo}", anotacion.UsuarioRemitente.UserName);
                    email.InsertBodyItem("Notificación VB para: " + anotacion.Correlativo, MailServer.ItemType.H3, MailServer.Align.Center);
                    email.InsertLine();
                    email.InsertSpace();
                    email.InsertBodyItem($"Estimado usuario, le informamos que la anotación: {anotacion.Correlativo} - {anotacion.Titulo}, Asociada al Libro de Obra {anotacion.LOD_LibroObras.NombreLibroObra} ha recibido el VB°", MailServer.ItemType.p, MailServer.Align.Left);
                    //FLEXBOX METODO 1: CON CLASE FUERTEMENTE TIPADA
                    email.InsertFlexBoxTable(anotacion, s => s.Correlativo, s => s.Titulo, s => s.UsuarioRemitente.NombreCompleto,
                        s => s.FechaPub, s => s.MAE_SubtipoComunicacion.Nombre, s => s.LOD_LibroObras.CON_Contratos.NombreContrato);
                    email.InsertSpace();

                    entero = await email.SendEmail(item.LOD_UsuarioLod.ApplicationUser.UserName);
                }
                else if (item.EsPrincipal)
                {
                    MailServer email = new MailServer($"Notificación de VB para: {anotacion.Correlativo} - {anotacion.Titulo}", anotacion.UsuarioRemitente.UserName);
                    email.InsertBodyItem("Notificación VB para: " + anotacion.Correlativo, MailServer.ItemType.H3, MailServer.Align.Center);
                    email.InsertLine();
                    email.InsertSpace();
                    email.InsertBodyItem($"Estimado usuario, le informamos como receptor principal de la anotación {anotacion.Correlativo} - {anotacion.Titulo}, Asociada al Libro de Obra {anotacion.LOD_LibroObras.NombreLibroObra}, que esta anotación ha recibido el VB°.", MailServer.ItemType.p, MailServer.Align.Left);
                    //FLEXBOX METODO 1: CON CLASE FUERTEMENTE TIPADA
                    email.InsertFlexBoxTable(anotacion, s => s.Correlativo, s => s.Titulo, s => s.UsuarioRemitente.NombreCompleto,
                        s => s.FechaPub, s => s.MAE_SubtipoComunicacion.Nombre, s => s.LOD_LibroObras.CON_Contratos.NombreContrato);
                    email.InsertSpace();

                    entero = await email.SendEmail(item.LOD_UsuarioLod.ApplicationUser.UserName);
                }
                else if (item.EsRespRespuesta)
                {
                    MailServer email = new MailServer($"Notificación de VB para: {anotacion.Correlativo} - {anotacion.Titulo}", anotacion.UsuarioRemitente.UserName);
                    email.InsertBodyItem("Notificación VB para: " + anotacion.Correlativo, MailServer.ItemType.H3, MailServer.Align.Center);
                    email.InsertLine();
                    email.InsertSpace();
                    email.InsertBodyItem($"Estimado usuario, le informamos como responsable de respuesta de la anotación {anotacion.Correlativo} - {anotacion.Titulo}, Asociada al Libro de Obra {anotacion.LOD_LibroObras.NombreLibroObra}, que esta anotación ha recibido el VB°.", MailServer.ItemType.p, MailServer.Align.Left);
                    //FLEXBOX METODO 1: CON CLASE FUERTEMENTE TIPADA
                    email.InsertFlexBoxTable(anotacion, s => s.Correlativo, s => s.Titulo, s => s.UsuarioRemitente.NombreCompleto,
                        s => s.FechaPub, s => s.MAE_SubtipoComunicacion.Nombre, s => s.LOD_LibroObras.CON_Contratos.NombreContrato);
                    email.InsertSpace();

                    entero = await email.SendEmail(item.LOD_UsuarioLod.ApplicationUser.UserName);
                }
                else
                {
                    MailServer email = new MailServer($"Notificación de VB para: {anotacion.Correlativo} - {anotacion.Titulo}", anotacion.UsuarioRemitente.UserName);
                    email.InsertBodyItem("Notificación VB para: " + anotacion.Correlativo, MailServer.ItemType.H3, MailServer.Align.Center);
                    email.InsertLine();
                    email.InsertSpace();
                    email.InsertBodyItem($"Estimado usuario, le informamos como receptor para la anotación {anotacion.Correlativo} - {anotacion.Titulo}, Asociada al Libro de Obra {anotacion.LOD_LibroObras.NombreLibroObra}, que esta anotación ha recibido el VB°.", MailServer.ItemType.p, MailServer.Align.Left);
                    //FLEXBOX METODO 1: CON CLASE FUERTEMENTE TIPADA
                    email.InsertFlexBoxTable(anotacion, s => s.Correlativo, s => s.Titulo, s => s.UsuarioRemitente.NombreCompleto,
                        s => s.FechaPub, s => s.MAE_SubtipoComunicacion.Nombre, s => s.LOD_LibroObras.CON_Contratos.NombreContrato);
                    email.InsertSpace();

                    entero = await email.SendEmail(item.LOD_UsuarioLod.ApplicationUser.UserName);
                }

            }
            return entero;

        }

        public async Task<int> NotificarAprobacionDoc(LOD_Anotaciones anotacion, LOD_docAnotacion docAnotacion, string userId)
        {

            List<LOD_UserAnotacion> listadoUser = db.LOD_UserAnotacion.Where(x => x.IdAnotacion == anotacion.IdAnotacion).Include(x => x.LOD_UsuarioLod).ToList();
            int entero = 0;
            foreach (var item in listadoUser)
            {
                if (item.LOD_UsuarioLod.UserId == userId)
                {
                    MailServer email = new MailServer($"Notificación de Aprobación de documento en: {anotacion.Correlativo} - {anotacion.Titulo}", anotacion.UsuarioRemitente.UserName);
                    email.InsertBodyItem("Notificación de Aprobación de documento en: " + anotacion.Correlativo + "-"+anotacion.Titulo, MailServer.ItemType.H3, MailServer.Align.Center);
                    email.InsertLine();
                    email.InsertSpace();
                    email.InsertBodyItem($"Estimado usuario, le informamos que ha sido aprobado el documento: {docAnotacion.MAE_TipoDocumento.Tipo} en la anotación: {anotacion.Correlativo} - {anotacion.Titulo}, Asociada al Libro de Obra {anotacion.LOD_LibroObras.NombreLibroObra}", MailServer.ItemType.p, MailServer.Align.Left);
                    email.InsertBodyItem($"Observaciones: {docAnotacion.Observaciones} .", MailServer.ItemType.p, MailServer.Align.Left);
                    //FLEXBOX METODO 1: CON CLASE FUERTEMENTE TIPADA
                    email.InsertFlexBoxTable(anotacion, s => s.Correlativo, s => s.Titulo, s => s.UsuarioRemitente.NombreCompleto,
                        s => s.FechaPub, s => s.MAE_SubtipoComunicacion.Nombre, s => s.LOD_LibroObras.CON_Contratos.NombreContrato);
                    email.InsertFlexBoxTable(docAnotacion, s => s.MAE_documentos.NombreDoc, s => s.MAE_TipoDocumento.Tipo, s => s.MAE_documentos.FechaCreacion);
                    email.InsertSpace();

                    entero = await email.SendEmail(item.LOD_UsuarioLod.ApplicationUser.UserName);
                    return entero;
                }
            }
            return entero;
        }

        public async Task<int> NotificarRechazoDoc(LOD_Anotaciones anotacion, LOD_docAnotacion docAnotacion, string userId)
        {

            List<LOD_UserAnotacion> listadoUser = db.LOD_UserAnotacion.Where(x => x.IdAnotacion == anotacion.IdAnotacion).Include(x => x.LOD_UsuarioLod).ToList();
            int entero = 0;
            foreach (var item in listadoUser)
            {
                if (item.LOD_UsuarioLod.UserId == userId)
                {
                    MailServer email = new MailServer($"Notificación de rechazo de documento en: {anotacion.Correlativo} - {anotacion.Titulo}", anotacion.UsuarioRemitente.UserName);
                    email.InsertBodyItem("Notificación de rechazo de documento en: " + anotacion.Correlativo + "-" + anotacion.Titulo, MailServer.ItemType.H3, MailServer.Align.Center);
                    email.InsertLine();
                    email.InsertSpace();
                    email.InsertBodyItem($"Estimado usuario, le informamos que ha sido rechazado el documento: {docAnotacion.MAE_TipoDocumento.Tipo} en la anotación: {anotacion.Correlativo}  - {anotacion.Titulo}, Asociada al Libro de Obra {anotacion.LOD_LibroObras.NombreLibroObra}", MailServer.ItemType.p, MailServer.Align.Left);
                    email.InsertBodyItem($"Observaciones: {docAnotacion.Observaciones} .", MailServer.ItemType.p, MailServer.Align.Left);
                    //FLEXBOX METODO 1: CON CLASE FUERTEMENTE TIPADA
                    email.InsertFlexBoxTable(anotacion, s => s.Correlativo, s => s.Titulo, s => s.UsuarioRemitente.NombreCompleto,
                        s => s.FechaPub, s => s.MAE_SubtipoComunicacion.Nombre, s => s.LOD_LibroObras.CON_Contratos.NombreContrato);
                    email.InsertFlexBoxTable(docAnotacion, s => s.MAE_documentos.NombreDoc, s => s.MAE_TipoDocumento.Tipo, s => s.MAE_documentos.FechaCreacion);
                    email.InsertSpace();

                    entero = await email.SendEmail(item.LOD_UsuarioLod.ApplicationUser.UserName);

                    return entero;
                }
            }

            return entero;
        }


        public async Task<int> NotificarSolicitudFirma(LOD_Anotaciones anotacion, string userId, string remitente)
        {

            ApplicationUser user = db.Users.Find(userId);
            LOD_UsuariosLod userRem = db.LOD_UsuariosLod.Where(x => x.UserId == remitente).FirstOrDefault();

            MailServer email = new MailServer($"Notificación de Solicitud de Firma: {anotacion.Titulo}", anotacion.UsuarioRemitente.UserName);
            email.InsertBodyItem("Notificación Solicitud de Firma para: " + anotacion.Titulo, MailServer.ItemType.H3, MailServer.Align.Center);
            email.InsertLine();
            email.InsertSpace();
            email.InsertBodyItem($"Estimado usuario, se le ha solicitado firma para la anotación: {anotacion.Titulo} al usuario {userRem.ApplicationUser.NombreCompleto}, Asociada al Libro de Obra {anotacion.LOD_LibroObras.NombreLibroObra}", MailServer.ItemType.p, MailServer.Align.Left);
            //FLEXBOX METODO 1: CON CLASE FUERTEMENTE TIPADA
            email.InsertFlexBoxTable(anotacion, s => s.Titulo, s => s.UsuarioRemitente.NombreCompleto,
                        s => s.FechaPub, s => s.MAE_SubtipoComunicacion.Nombre, s => s.LOD_LibroObras.CON_Contratos.NombreContrato);
            email.InsertSpace();

            var entero = await email.SendEmail(user.UserName);


            MailServer email2 = new MailServer($"Notificación de Solicitud de Firma: {anotacion.Titulo}", anotacion.UsuarioRemitente.UserName);
            email2.InsertBodyItem("Notificación Solicitud de Firma para: " + anotacion.Titulo, MailServer.ItemType.H3, MailServer.Align.Center);
            email2.InsertLine();
            email2.InsertSpace();
            email2.InsertBodyItem($"Estimado usuario, le informamos que se le ha solicitado su firma para la anotación: {anotacion.Correlativo} - {anotacion.Titulo}, Asociada al Libro de Obra {anotacion.LOD_LibroObras.NombreLibroObra}", MailServer.ItemType.p, MailServer.Align.Left);
            //FLEXBOX METODO 1: CON CLASE FUERTEMENTE TIPADA
            email2.InsertFlexBoxTable(anotacion, s => s.Titulo, s => s.UsuarioRemitente.NombreCompleto,
                        s => s.FechaPub, s => s.MAE_SubtipoComunicacion.Nombre, s => s.LOD_LibroObras.CON_Contratos.NombreContrato);
            email2.InsertSpace();

            var entero2 = await email2.SendEmail(userRem.ApplicationUser.UserName);

            return entero2;
        }

        public async Task<int> NotificarReenvio(LOD_Anotaciones anotacion, string userId, string remitente)
        {

            ApplicationUser user = db.Users.Find(userId);
            LOD_UsuariosLod userRem = db.LOD_UsuariosLod.Where(x => x.UserId == remitente).FirstOrDefault();

            MailServer email = new MailServer($"Notificación de Solicitud de Firma: {anotacion.Correlativo} - {anotacion.Titulo}", anotacion.UsuarioRemitente.UserName);
            email.InsertBodyItem("Notificación reenvío para: " + anotacion.Correlativo, MailServer.ItemType.H3, MailServer.Align.Center);
            email.InsertLine();
            email.InsertSpace();
            email.InsertBodyItem($"Estimado usuario, le informamos que se le ha reenviado la anotación: {anotacion.Correlativo} - {anotacion.Titulo}, Asociada al Libro de Obra {anotacion.LOD_LibroObras.NombreLibroObra}", MailServer.ItemType.p, MailServer.Align.Left);
            //FLEXBOX METODO 1: CON CLASE FUERTEMENTE TIPADA
            email.InsertFlexBoxTable(anotacion, s => s.Correlativo, s => s.Titulo, s => s.UsuarioRemitente.NombreCompleto,
                        s => s.FechaPub, s => s.MAE_SubtipoComunicacion.Nombre, s => s.LOD_LibroObras.CON_Contratos.NombreContrato);
            email.InsertSpace();

            var entero2 = await email.SendEmail(userRem.ApplicationUser.UserName);
            return entero2;
        }
    }
}