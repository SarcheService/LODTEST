using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LOD_APR.Areas.GLOD.ModelsViews
{
    public class FiltroAvanzado
    {
        public int IdLibro { get; set; }
        public string IdRemitente { get; set; }
        public string IdDestinatario { get; set; }
        public string searchCuerpo { get; set; }
        public string FechaPub { get; set; }
    }
}