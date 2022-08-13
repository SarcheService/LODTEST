using LOD_APR.Areas.ASP.Models;
using LOD_APR.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

//CREADO ER 14-05-2019
namespace LOD_APR.Helpers
{
    public class HelperBitacoras
    {
        //Recupera el Correlativo de Bitacora cuando corresponde a una Libro
        public static  int EnumerarAnotacionesBit(int IdBit, int IdAnotacion)
        {
            LOD_DB db = new LOD_DB();
            int CoutReturn = db.ASP_AnotacionesBit.Where(x => x.IdBitacora == IdBit && x.Estado != 1).Count();
            List<tablaNumeracionBit> ListaAnotBit = db.ASP_AnotacionesBit.Where(x => x.IdBitacora == IdBit && x.Estado != 1)
                .Select(x=> new tablaNumeracionBit(){IdAnotLod=0,IdAnotBit=x.IdAnotacionBit, FechaPub=x.FechaPub,Correlativo=x.CorrelativoBit}).ToList();           
            //Generar Numeración
            List<ASP_bitacorasLObrasAsoc> LibrosAsoc = db.ASP_bitacorasLObrasAsoc.Where(l => l.IdBitacora == IdBit).ToList();
            foreach (ASP_bitacorasLObrasAsoc asoc in LibrosAsoc)//Por cada libro asociado
            {

                if (asoc.FechaDesasoc == null)
                {
                    CoutReturn = CoutReturn + db.ASP_Anotaciones
                    .Where(l => l.IdLibroObra == asoc.IdLibroObra && l.FechaPub > asoc.FechaAsoc && l.Estado != 1).Count();
                    List<tablaNumeracionBit> ListaAnotLod = db.ASP_Anotaciones.Where(l => l.IdLibroObra == asoc.IdLibroObra && l.FechaPub > asoc.FechaAsoc && l.Estado != 1)
                    .Select(x => new tablaNumeracionBit() { IdAnotLod = x.IdAnotacion, IdAnotBit = 0, FechaPub = x.FechaPub, Correlativo = 0}).ToList();
                    ListaAnotBit.AddRange(ListaAnotLod);

                }
                else
                {
                    CoutReturn = CoutReturn + db.ASP_Anotaciones
                    .Where(l => l.IdLibroObra == asoc.IdLibroObra && l.FechaPub > asoc.FechaAsoc && l.FechaPub < asoc.FechaDesasoc && l.Estado != 1).Count();
                    List<tablaNumeracionBit> ListaAnotLod = db.ASP_Anotaciones.Where(l => l.IdLibroObra == asoc.IdLibroObra && l.FechaPub > asoc.FechaAsoc && l.FechaPub < asoc.FechaDesasoc && l.Estado != 1)
                   .Select(x => new tablaNumeracionBit() { IdAnotLod = x.IdAnotacion, IdAnotBit = 0, FechaPub = x.FechaPub, Correlativo = 0 }).ToList();
                    ListaAnotBit.AddRange(ListaAnotLod);
                }
            }
            ListaAnotBit = ListaAnotBit.OrderBy(a => a.FechaPub).ToList();
            for (int i = 0; i < CoutReturn; i++)
            {
                ListaAnotBit[i].Correlativo = i+1;
            }
            //Buscar Correlativo
            var corrBitprev = ListaAnotBit.Where(x => x.IdAnotLod == IdAnotacion).ToList();
            int CorrelativoBit = 0;
            if (corrBitprev.Count()>0) CorrelativoBit = Convert.ToInt16(corrBitprev[0].Correlativo);
            return (CorrelativoBit);
        }

        public static ASP_AnotacionesBit ConvertToAnotBit(ASP_Anotaciones AnotRefLod, int Bitacora)
        {
            LOD_DB db = new LOD_DB();
            ASP_AnotacionesBit Anotacion = new ASP_AnotacionesBit()
            {
                IdAnotacionBit = AnotRefLod.IdAnotacion,
                CorrelativoBit = HelperBitacoras.EnumerarAnotacionesBit(Convert.ToInt32(Bitacora), AnotRefLod.IdAnotacion),
                Titulo = AnotRefLod.ASP_LibroObras.NomLibroObra + ": Anot N°" + AnotRefLod.Correlativo + "- " + AnotRefLod.Titulo,
                Cuerpo = AnotRefLod.Cuerpo,
                Estado = AnotRefLod.Estado,
                FechaIngreso = AnotRefLod.FechaIngreso,
                FechaPub = AnotRefLod.FechaPub,
                SolicitudResp = AnotRefLod.SolicitudRest,
                FechaResp = AnotRefLod.FechaResp,
                IdTipoAnotacion = AnotRefLod.IdTipoAnotacion,
                IdAnotLod = AnotRefLod.IdAnotacion,
                UserId = AnotRefLod.UserId,
                IdReferencia = AnotRefLod.IdReferencia,
                SolicitudVB = AnotRefLod.SolicitudVB,
                TieneRespuesta = db.ASP_Anotaciones.Where(x => x.IdReferencia == AnotRefLod.IdAnotacion && x.Estado > 1).Count(),
                VistosBuenosPendientes = db.ASP_UsuarioAnotacion.Where(x => x.RespVB == true && x.IdAnotacion == AnotRefLod.IdAnotacion && x.VistoBueno == false).Count()
            };
            return Anotacion;
        }

    }
    public class tablaNumeracionBit
    {
        public int? Correlativo { get; set; }
        public int IdAnotBit { get; set; }
        public int IdAnotLod { get; set; }
        public DateTime? FechaPub { get; set; }
    }
}