using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LOD_APR.Areas.GLOD.ModelsViews
{
    public class LogView
    {
        public int IdLog { get; set; }
        public string Objeto { get; set; }
        public int IdObjeto { get; set; }
        public string FechaLog { get; set; }
        public string UserId { get; set; }
        public string Usuario { get; set; }
        public string Accion { get; set; }
        public string Campo { get; set; }
        public string ValorAnterior { get; set; }
        public string ValorActualizado { get; set; }
    }
}