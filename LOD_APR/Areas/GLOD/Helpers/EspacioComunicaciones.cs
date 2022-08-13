using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LOD_APR.Areas.GLOD.Helpers
{
    public class EspacioComunicaciones
    {
        public string NombreEspacio { get; set; }
        public EspacioComunicaciones(int IdTipoLibro)
        {
            if (IdTipoLibro == 1)
            {
                NombreEspacio = "Libro de Obras";
            }
            else if (IdTipoLibro == 2)
            {
                NombreEspacio = "Libro de Comunicaciones";
            }
            else
            {
                NombreEspacio = "Comunicacion Complementaria";
            }
        }
    }
}