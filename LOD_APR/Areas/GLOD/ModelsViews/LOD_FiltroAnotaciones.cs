using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LOD_APR.Areas.GLOD.ModelsViews
{
    public class LOD_FiltroAnotaciones
    {
        public int IdLibro { get; set; }
        public int[] IdEstado { get; set; }
        public string[] UserId { get; set; }
        public string searchCuerpo { get; set; }
        public string FDesde { get; set; }
        public string FHasta { get; set; }
        public DateTime? FDesdeDate
        {
            get
            {
                DateTime? dtime = null;
                if (FDesde != null)
                    dtime = DateTime.Parse(FDesde);

                return dtime;
            }
            set { }
        }
        public DateTime? FHastaDate
        {
            get
            {
                DateTime? dtime = null;
                if (FHasta != null)
                    dtime = DateTime.Parse(FHasta);

                return dtime;
            }
            set { }
        }
    }

    public class ASP_FiltroAnotacionesBIT
    {
        public int IdBitacora { get; set; }
        public int[] IdEstado { get; set; }
        public string[] UserId { get; set; }
        public string searchCuerpo { get; set; }
        public string FDesde { get; set; }
        public string FHasta { get; set; }
        public DateTime? FDesdeDate
        {
            get
            {
                DateTime? dtime = null;
                if (FDesde != null)
                    dtime = DateTime.Parse(FDesde);

                return dtime;
            }
            set { }
        }
        public DateTime? FHastaDate
        {
            get
            {
                DateTime? dtime = null;
                if (FHasta != null)
                    dtime = DateTime.Parse(FHasta);

                return dtime;
            }
            set { }
        }
    }
}