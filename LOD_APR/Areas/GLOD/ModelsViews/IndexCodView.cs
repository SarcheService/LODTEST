using LOD_APR.Areas.GLOD.Models;
using System.Collections.Generic;

namespace LOD_APR.Areas.GLOD.ModelsViews
{
    public class IndexCodView
    {      
       public List<ItemIndexCod> list_Item { get; set; }
    }

    public class ItemIndexCod
    {
        public CON_Contratos CON_Contratos { get; set; }
        public int Estado { get; set; }
    }
}