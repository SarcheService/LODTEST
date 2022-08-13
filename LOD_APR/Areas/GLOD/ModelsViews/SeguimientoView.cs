using LOD_APR.Areas.Admin.Models;
using LOD_APR.Areas.GLOD.Models;
using System.Collections.Generic;

namespace LOD_APR.Areas.GLOD.ModelsViews
{
    public class SeguimientoView
    {
        public CON_Contratos Contrato { get; set; }
        public List<LOD_LibroObras> lista_Libros { get; set; }
        public List<ItemSeguimiento> lista_Items { get; set; }
        public List<LOD_docAnotacion> OtrosDocumentos { get; set; }
    }

    public class ItemSeguimiento { 
        public int IdLod { get; set; }
        public int totalDoc { get; set; }
        public MAE_CodSubCom mAE_CodSubCom { get; set; }
        public MAE_ClassDoc mAE_ClassDoc { get; set; }
        public int Estado { get; set; }
        public bool Alerta { get; set; }
    }

}