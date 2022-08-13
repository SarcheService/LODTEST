using LOD_APR.Areas.GLOD.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LOD_APR.Areas.GLOD.ModelsViews
{
    public class CreateRolesView
    {
        public string NombreRol { get; set; }
        public int IdContrato { get; set; }
        public string Descripcion { get; set; }
        public List<PermisosLibroView> PermisosLibros { get; set; }
    }

    public class PermisosLibroView
    {
        public LOD_LibroObras libro { get; set; }
        public int IdLod { get; set; }
        
        public bool Lectura { get; set; }
        public bool Escritura { get; set; }
        public bool FirmaGobierno { get; set; }
        public bool FirmaFEA { get; set; }
        public bool FirmaSimple { get; set; }
    }
}