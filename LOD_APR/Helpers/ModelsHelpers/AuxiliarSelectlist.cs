using System.Web.Mvc;

namespace LOD_APR.Helpers.ModelsHelpers
{
    public class ContenedorAuxiliarSelectList
    {
        public AuxiliarSelectList UltimoHistorial { get; set; }
    }

    public class AuxiliarSelectList
    {
        public SelectList IdRegion { get; set; }
        public SelectList IdCiudad { get; set; }
    }
}