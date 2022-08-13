using LOD_APR.Areas.Admin.Models;
using LOD_APR.Areas.GLOD.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LOD_APR.Areas.GLOD.ModelsViews
{
    public class CierreLibroView
    {
        public int IdLibro { get; set; }
        public LOD_LibroObras LibroObra { get; set; }
        public List<LOD_Anotaciones> listadoAnotaciones { get; set; }
        public List<MAE_TipoDocumento> listadoLiquidacion { get; set; }
        public List<MAE_TipoDocumento> listadoNoAprobados { get; set; }
    }
}