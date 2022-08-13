using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LOD_APR.Areas.GLOD.ModelsViews
{
    public class LodUserStats
    {
        public int Principal { get; set; }
        public int MisPub { get; set; }
        public int Borradores { get; set; }
        public int Destacadas { get; set; }
        public int Nombrado { get; set; }
        public int FirmasPendientes { get; set; }
        public int RespuestasPendientes { get; set; }
    }
}