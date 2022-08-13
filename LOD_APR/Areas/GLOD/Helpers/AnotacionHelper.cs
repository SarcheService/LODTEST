using LOD_APR.Areas.GLOD.Models;
using LOD_APR.Areas.GLOD.ModelsViews;
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
    public class AnotacionHelper
    {
        private LOD_DB db = new LOD_DB();

        public async Task<AnotacionView> GetAnotacionData(int id)
        {
            LOD_Anotaciones anotacion = await db.LOD_Anotaciones.FindAsync(id);
                    
            AnotacionView anot = new AnotacionView()
            {
                IdAnotacion = anotacion.IdAnotacion,
                Folio = (anotacion.Correlativo==0)?"-":anotacion.Correlativo.ToString().PadLeft(6,'0'),
                Titulo = anotacion.Titulo,
                Cuerpo = anotacion.Cuerpo,
                LibroObras = anotacion.LOD_LibroObras.NombreLibroObra,
                IdLibro = anotacion.IdLod,
                Contrato = anotacion.LOD_LibroObras.CON_Contratos.CodigoContrato + " - " + anotacion.LOD_LibroObras.CON_Contratos.NombreContrato,
                RutContratista = anotacion.LOD_LibroObras.CON_Contratos.Empresa_Contratista.Rut,
                Contratista = anotacion.LOD_LibroObras.CON_Contratos.Empresa_Contratista.RazonSocial,
                LibroDeObras = anotacion.LOD_LibroObras.NombreLibroObra,
                Tipo = anotacion.MAE_SubtipoComunicacion.MAE_TipoComunicacion.Nombre,
                Subtipo = anotacion.MAE_SubtipoComunicacion.Nombre,
                IdTipoSub = anotacion.IdTipoSub, 
                EsEstructurada = false, //POR AHORA EN FALSE
                SolicitudRest = anotacion.SolicitudRest,
                FechaRespuesta = anotacion.FechaResp.ToString(),
                FechaTopeRespuesta = anotacion.FechaTopeRespuesta.ToString(),
                FechaCreacion = anotacion.FechaIngreso.Value.ToLongDateString() + " " + anotacion.FechaIngreso.Value.ToShortTimeString(),
                UserBorrador = anotacion.UserIdBorrador,
                EstadoFirma = new EstadoFirma()
                {
                    FechaFirma = (!anotacion.FechaFirma.HasValue)?"":anotacion.FechaFirma.Value.ToLongDateString() + " " + anotacion.FechaFirma.Value.ToShortTimeString(),
                    IdTipo = anotacion.TipoFirma, //DEPENDE DEL TIPO DE LIBRO: 1 - FEA | 2 - MINSEGPRES | 3 - FIRMA ELECTRONICA SIMPLE
                    IsFirmada = anotacion.EstadoFirma,
                    UsuarioFirma = anotacion.UserId
                },
                EstadoAnotacion = new EstadoAnotacion() { IdEstado = anotacion.Estado },//0-BORRADOR |1-SOLICITUD FIRMA | 2 - PUBLICADA
                Adjuntos = new List<Adjuntos>(),
                Referencias = new List<Referencias>()
            };

            //RESPUESTA
            EstadoRespuesta res = new EstadoRespuesta() { Respondida = false};
            var referenciaAnot = db.LOD_ReferenciasAnot.Where(x => x.IdAnontacionRef == id && x.EsRepuesta && x.AnotacionOrigen.EstadoFirma).FirstOrDefault();
            if (referenciaAnot != null)
            {
                
                res = new EstadoRespuesta()//SE OBTIENE DE LAS ANOTACIONES REFERENCIDAS EN TABLA LOD_ReferenciasAnot
                {
                    IdRespuesta = referenciaAnot.IdAnotacion,//ID DE LA ANOTACION RESPUESTA
                    FechaRespuesta = referenciaAnot.AnotacionOrigen.FechaFirma.Value.ToLongDateString(),
                    FolioRespuesta = referenciaAnot.AnotacionOrigen.Correlativo.ToString(), //FOLIO DE LA ANOTACION DE RESPUESTA
                    TituloRespuesta = referenciaAnot.AnotacionOrigen.Titulo,
                    Respondida = true, // ES TRUE SI EL REGISTRO EN TABLA LOD_ReferenciasAnot EXISTE
                    UsuarioRespuesta = referenciaAnot.AnotacionOrigen.UsuarioRemitente.NombreCompleto //USERID LOD_ANOTACION ANOTACION DE RESPUESTA
                };
            }
            anot.EstadoRespuesta = res;
            //REMITENTE
            Remitente rem = new Remitente();
            if (anotacion.UserId != null)
            {
                var remitente = await db.Users.Where(u => u.Id == anotacion.UserId).FirstOrDefaultAsync();//NO USAR USERMANAGER
                rem = new Remitente()
                {//LOD_ANOTACION => USERID (FIRMANTE)
                    ImgRemitente = remitente.RutaImg,//VACIO SI NO TIENE IMAGEN
                    InicialesRemitente = Data_Letters.ImageLetter(remitente.Nombres,remitente.Apellidos),
                    Nombre = remitente.Nombres.Split(' ')[0] + " " + remitente.Apellidos,
                    Cargo = remitente.CargoContacto,
                    Empresa = remitente.MAE_Sucursal.MAE_sujetoEconomico.RazonSocial
                };
            }
            anot.Remitente = rem;
            
            anot.Receptores = await GetReceptoresData(id);
            anot.Referencias = await GetReferencias(id);
            anot.Adjuntos = await GetAdjuntos(id);
            return anot;

        }
        public async Task<List<Receptor>> GetReceptoresData(int id)
        {
            List<LOD_UserAnotacion> listaUserRecep = await db.LOD_UserAnotacion.Where(x => x.IdAnotacion == id && x.RespVB).ToListAsync();
            List<Receptor> receptores = new List<Receptor>();
            foreach (var item in listaUserRecep)
            {
                try
                {
                    Receptor r = new Receptor()
                    {
                        Nombre = item.Nombre,
                        Imagen = item.RutaImg,
                        Iniciales = item.LOD_UsuarioLod.ApplicationUser.Nombres.Substring(0, 1) + item.LOD_UsuarioLod.ApplicationUser.Apellidos.Substring(0, 1),
                        ReqVB = item.RespVB,
                        RespRespuesta = item.EsRespRespuesta,
                        EsPrincipal = item.EsPrincipal,
                        Cargo = item.LOD_UsuarioLod.ApplicationUser.CargoContacto,
                        IdAnotacion = item.IdAnotacion,
                        IdReceptor = item.IdUsLod,
                        VistoBueno = item.VistoBueno,
                        FechaVB = (item.FechaVB.HasValue) ? item.FechaVB.Value.ToLongDateString() + " " + item.FechaVB.Value.ToShortTimeString() : "",
                        TipoFirma = (item.TipoVB.HasValue)?item.TipoVB.Value:0
                    };
                    receptores.Add(r);
                }
                catch(Exception ex){}
            }
            return receptores;
        }

        public async Task<List<Referencias>> GetReferencias(int id)
        {
            List<Referencias> listadoReferencias = new List<Referencias>();
            List<LOD_ReferenciasAnot> referencias = await db.LOD_ReferenciasAnot.Where(r => r.IdAnotacion == id).ToListAsync();
            foreach(LOD_ReferenciasAnot r in referencias)
            {
                Referencias re = new Referencias()
                {
                    IdRefAnot = r.IdRefAnot,
                    IdAnotacion = r.IdAnontacionRef,
                    Anotacion = r.AnotacionReferencia.Titulo,
                    Fecha = r.AnotacionReferencia.FechaFirma.ToString(),
                    Folio = r.AnotacionReferencia.Correlativo.ToString().PadLeft(6, '0'),
                    Libro = r.AnotacionReferencia.LOD_LibroObras.NombreLibroObra,
                    Remitente = r.AnotacionReferencia.UsuarioRemitente.NombreCompleto,
                    Observacion = r.Observacion
                };
                listadoReferencias.Add(re);
            }
            return listadoReferencias;
        }

        public async Task<List<Adjuntos>> GetAdjuntos(int id)
        {
            List<Adjuntos> adjuntos = new List<Adjuntos>();
            List<LOD_docAnotacion> referencias = await db.LOD_docAnotacion.Where(r => r.IdAnotacion == id).ToListAsync();
            foreach (LOD_docAnotacion d in referencias)
            {
                Adjuntos re = new Adjuntos()
                {
                  Documento = d.MAE_documentos.NombreDoc,
                  Tipo = d.MAE_TipoDocumento.Tipo
                };
                adjuntos.Add(re);
            }
            return adjuntos;
        }
    }
}