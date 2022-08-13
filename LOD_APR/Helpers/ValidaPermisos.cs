using ACAMicroFramework.SqlDB;
using LOD_APR.Models;
using Microsoft.AspNet.Identity;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace LOD_APR.Helpers
{
    public class ValidaPermisos
    {
        private LOD_DB db = new LOD_DB();

        private string con = ACA_SqlServer.generaConexion("LOD_DB");
        private string userId = HttpContext.Current.User.Identity.GetUserId();
        public List<string> permisos = new List<string>();


        public ValidaPermisos()//no aplica
        {
            string roleId = ACA_SqlServer.ScalarQuery(con, "SELECT [RoleId] FROM [dbo].[SEG_UserRoles] where [UserId]='" + userId + "'").ToString();
            permisos = db.MAE_OpcionesRoles.Where(p => p.IdRol == roleId).Select(s => s.IdOpcion).ToList();
        }

        public ValidaPermisos(string userId)
        {

            string roleId = ACA_SqlServer.ScalarQuery(con, "SELECT [RoleId] FROM [dbo].[SEG_UserRoles] where [UserId]='" + userId + "'").ToString();
            permisos = db.MAE_OpcionesRoles.Where(p => p.IdRol == roleId).Select(s => s.IdOpcion).ToList();
        }

        public static bool ValidaPermisosEnController(string IdOpcion)
        {

            ///ACA_Sesion se = new ACA_Sesion();
            clsSession se = new clsSession();
            List<string> p = (List<string>)se.get("List_permisos");
            if (p != null)
            {
                if (p.Contains("MDTECH"))
                {
                    return true;
                }
                if (!IdOpcion.Contains(";"))
                {
                    return (0 < p.Where(x => x.Split(';')[0] == IdOpcion).Count());
                }
                else
                {
                    return p.Contains(IdOpcion);
                }

            }else
            {
                return false;
            }
        }

        //public  bool EnSesionItoMandante(int idContrato)
        //{
        //    ASP_contratos contrato = db.ASP_contratos.Find(idContrato);
        //    if (db.SEG_UserPersonal.Where(x => x.UserId == userId).Count()==0) { return true; }
        //    var IdItoMandante = db.SEG_UserPersonal.Where(x => x.UserId == userId).FirstOrDefault().IdPersonal;
        //    if (contrato.IdRespMandante != null)
        //    {
        //        if (IdItoMandante == contrato.IdRespMandante)
        //        {
        //            return true;
        //        }
        //    }
        //    return false;
        //}
    }
          
}
