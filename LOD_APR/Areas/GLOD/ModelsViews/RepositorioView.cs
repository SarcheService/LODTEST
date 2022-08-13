using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LOD_APR.Areas.GLOD.ModelsViews
{
    public class RepositorioView
    {
        public int IdElemento { get; set; }
        /*
         tipo 1 = Carpeta;
         Tipo 2 = Documento;         
         */
        public int TipoElemento { get; set; }
        public string NombreElemento { get; set; }
        public int EstadoElemento { get; set; }
        public bool Obligatorio { get; set; }
        public int? IdTipoDocumento { get; set; }
        public int? IdLibro { get; set; }
        public int? IdAnotacion { get; set; }
        public int? totalArchivos { get; set; }
    }
}