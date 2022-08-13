using LOD_APR.Areas.Admin.Models;
using LOD_APR.Areas.GLOD.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LOD_APR.Areas.GLOD.ModelsViews
{
    public class LiquidacionLibroView
    {
        public int IdContrato { get; set; }
        public CON_Contratos contrato { get; set; }
        public List<LOD_LibroObras> librosPorFirmar { get; set; }
        public List<MAE_TipoDocumento> listadoLiquidacion { get; set; }
        public List<MAE_TipoDocumento> listadoNoAprobados { get; set; }
    }

    public class ItemLiquidacion
    {
        public MAE_TipoDocumento documento { get; set; }
        public MAE_SubtipoComunicacion subtipo { get; set; }
        public MAE_TipoDocumento tipo { get; set; }
    }
}