using LOD_APR.Areas.Admin.Models;
using LOD_APR.Areas.GLOD.Helpers;
using LOD_APR.Areas.GLOD.Models;
using LOD_APR.Areas.GLOD.ModelsViews;
using LOD_APR.Helpers;
using LOD_APR.Helpers.ModelsHelpers;
using LOD_APR.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace LOD_APR.Areas.GLOD.Helpers
{

    public class Log_Helper
    {
    
        private LOD_DB db = new LOD_DB();


        public async Task<bool> SetObjectLog(int tipo, Object objeto, string Accion, string idUser)
        {
            try
            {
                LOD_log lOD_Log = new LOD_log()
                {
                    Accion = Accion,
                    FechaLog = DateTime.Now,
                    UserId = idUser,
                };

                switch (tipo)
                {
                    case 0:
                        LOD_Anotaciones anotacion = (LOD_Anotaciones)objeto;
                        lOD_Log.IdObjeto = anotacion.IdAnotacion;
                        lOD_Log.Objeto = "Anotacion";
                        break;
                    case 1:
                        LOD_LibroObras libro = (LOD_LibroObras)objeto;
                        lOD_Log.IdObjeto = libro.IdLod;
                        lOD_Log.Objeto = "Libro";
                        break;
                    case 2:
                        CON_Contratos contrato = (CON_Contratos)objeto;
                        lOD_Log.IdObjeto = contrato.IdContrato;
                        lOD_Log.Objeto = "Contrato";
                        break;
                    case 3:
                        break;
                    default:
                        break;
                }

                db.LOD_log.Add(lOD_Log);
                int count = await db.SaveChangesAsync();
                bool response = false;
                if (count > 0)
                    response = true;

                return response;
            }
            catch { }

            return false;
        }


        public async Task<bool> SetLOGAnotacionAsync(LOD_Anotaciones lOD_Anotaciones, string Accion, string idUser)
        {
            LOD_log lOD_Log = new LOD_log() { 
                Objeto = "Anotacion",
                IdObjeto = lOD_Anotaciones.IdAnotacion,
                Accion = Accion,
                FechaLog = DateTime.Now,
                UserId = idUser,                
            };
            db.LOD_log.Add(lOD_Log);
            int count  = await db.SaveChangesAsync();
            bool response = false;
            if (count > 0)
                response = true;

            return response;
        }


    }

}