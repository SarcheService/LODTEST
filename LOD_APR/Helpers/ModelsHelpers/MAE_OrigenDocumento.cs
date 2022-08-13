using System;

namespace LOD_APR.Helpers
{
    public class DatosDocumento
    {
        public int Origen { get; set; }
        public string Path { get; set; }
        public string ServerPath { get; set; }
        public string DataBasePath { get; set; }
        public string SharepointPath { get; set; }
        public string PreName { get; set; }
        public int? IdPath { get; set; }
        public bool IsSharepoint { get; set; }
        public int IdContrato { get; set; }
        public int? IdTipoDocumento { get; set; }
        public string Prefijo_SP { get; set; }
        public SPMetadata Metadata { get; set; }
    }
    public class SPMetadata
    {
        public int Empresa { get; set; }
        public string Categoria { get; set; }
        public string Localidad { get; set; }
        public string Tipo { get; set; }
    }
    public enum OrigenDocumento : Int32
    {
        Personal = 400000,
        Activos = 500000,
        Sujetos = 600000,
        Proyectos = 700000,
        Obras = 710000,
        Contratos = 720000,
        AnotacionLibroObra = 730000,
        Bitacoras = 740000,
        AnotacionBitacora = 750000,
        CaratulaEstadoPago = 760000,
        Soporte = 800000,
        EventoSoporte = 810000,
        Formularios = 900000
    }
    public enum TipoVistaDocumento : Int16
    {
        Paths = 0,
        Icons = 1,
    }
}