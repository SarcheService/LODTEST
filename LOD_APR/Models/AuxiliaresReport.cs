using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LOD_APR.Models
{
    public class AuxiliaresReport
    {

        public class ASP_logView
        {
            public DateTime FechaLog { get; set; }
            public int IdUsuario { get; set; }
            public string Accion { get; set; }
            public string Campo { get; set; }
            public string ValorAnterior { get; set; }
            public string ValorActualizado { get; set; }
        }
        //Mejora Jorge Tito 24/01/2018 Se agrega clase auxiliar para las dependecias de proyectos y obras
        public class ASP_dependeciasView
        {
            public int IdEstado { get; set; }
            public string Objeto { get; set; }
            public string Codigo { get; set; }
            public string Nombre { get; set; }
            public string Tipo { get; set; }
            public string Monto { get; set; }//Se deja como string para poder colocar la abreviatura de la moneda
            public DateTime FechaCreacion { get; set; }
            public DateTime FechaProgInicio { get; set; }
            public DateTime FechaProgTermino { get; set; }
            public string Estado { get; set; }
        }

        public class DsEmpresa
        {
            public string Razon { get; set; }
            public string Direccion { get; set; }
            public string Web { get; set; }
            public string Telefono { get; set; }
            public string Logo { get; set; }
            public string Email { get; set; }
        }

        public class DsObra
        {
            public string Titulo { get; set; }
            public string Descripcion { get; set; }
            public string Codigo { get; set; }
            public string Area { get; set; }
            public string Tipo { get; set; }
            public string FInicio { get; set; }
            public string FTermino { get; set; }
            public string Monto { get; set; }
        }

        public class DsContrato
        {
            public string Codigo { get; set; }
            public string FInicio { get; set; }
            public string FTermino { get; set; }
            public string Plazo { get; set; }
            public string Tipo { get; set; }
            public string Responsable { get; set; }
            public string Empresa { get; set; }
            public string Monto { get; set; }
        }

        public class DsAnotacion
        {
            public string Titulo { get; set; }
            public string Cuerpo { get; set; }
            public string Estado { get; set; }
            public string Usuario { get; set; }
            public string Tipo { get; set; }
            public string FPublicacion { get; set; }
            public string SRespuesta { get; set; }// Solicita resp
            public string Plazo { get; set; }
            public int Folio { get; set; }
            public string RutaImagen { get; set; }
            public string Remitente { get; set; }
            public string CargoRemitente { get; set; }
            public string Empresa { get; set; }
            public string Archivos { get; set; }
            public string Iniciales { get; set; }
            public string AnotRef { get; set; }
            public string AnotResp { get; set; }
            public int NDoc { get; set; }
            public int NRecep { get; set; }
            public int NVB { get; set; }
        }

        public class DsDocumentos
        {
            public string Usuario { get; set; }
            public string Tipo { get; set; }
            public string Fecha { get; set; }
            public string Nombre { get; set; }
            public string Ruta { get; set; }
            public string Folio { get; set; }
        }

        public class DsReceptor
        {
            public string Usuario { get; set; }
            public string Responsable { get; set; }
            public string VB { get; set; }

        }
        public class DsVB
        {
            public string Usuario { get; set; }
            public string Cargo { get; set; }
            public string Empresa { get; set; }

        }


        public class DsActividad
        {
            public string Usuario { get; set; }
            public string Empresa { get; set; }
            public string Accion { get; set; }
            //public string Observacion { get; set; }
            public string Fecha { get; set; }
        }

        public class DsEvaluacion
        {
            public string Item { get; set; }
            public string Pregunta { get; set; }
            public string Tipo { get; set; }
            public string Puntaje { get; set; }
            public string Ponderacion { get; set; }
            public string Respuesta { get; set; }  
        }

        public class DsEpCaratula
        {
            public string NumeroEP { get; set; }
            public decimal AvanceFisico { get; set; }
            public decimal AvanceFinanciero { get; set; }
            public decimal Retenciones { get; set; }
            public decimal DevAnticipo { get; set; }
            public decimal MontoEP { get; set; }
        }
        public class DsFirmas
        {
            public string Aprobador { get; set; }
            public string Cargo { get; set; }
            public string Fecha { get; set; }
            public string Estado { get; set; }
        }
        public class DsEpDetalle
        {
            public string Item { get; set; }
            public string Descripcion { get; set; }
            public decimal Cantidad { get; set; }
            public string Unidad { get; set; }
            public decimal PrecioUnitario { get; set; }
            public decimal Total { get; set; }
            public decimal PorcAvance { get; set; }
            public decimal TotalAcomulado { get; set; }
            public decimal TotalAcomAnterior { get; set; }
            public decimal PresenteEp { get; set; }

            public int IdModificacion { get; set; }

            public string EsTitulo { get; set; }

            public decimal PorcGGP { get; set; }
            public decimal PorcUP { get; set; }

            public string DescripcionMod { get; set; }
        }

        public class DsHistorialContrato
        {
          
            public string Partidas { get; set; }
            public decimal PorcAvaFisAcum { get; set; }
            public decimal PorcAvaFinAcum { get; set; }
            public decimal MontoTotal { get; set; }
            public decimal MontoAcumulado { get; set; }
            public decimal MontoPendiente { get; set; }

        }

        //Se agrega 21/11/2018
        public class DsAnotacionBit
        {
            public string Titulo { get; set; }
            public string Cuerpo { get; set; }
            public string Estado { get; set; }
            public string Usuario { get; set; }
            public string Tipo { get; set; }
            public string FPublicacion { get; set; }
            public string SRespuesta { get; set; }// Solicita resp
            public string Plazo { get; set; }
            public int Folio { get; set; }
            public string RutaImagen { get; set; }
            public string Remitente { get; set; }
            public string CargoRemitente { get; set; }
            public string Empresa { get; set; }            
            public string Iniciales { get; set; }
            public string Archivos { get; set; }
            public string NombreArchivos { get; set; }
            public string TipoArchivos { get; set; }
           
        }

        public class DsBitacora
        {
            public string Nombre { get; set; }
            public string Codigo { get; set; }
            public string FechaCreacion { get; set; }
            public string CreadoPor { get; set; }
        }
        public class DsProyecto
        {
            public string Nombre { get; set; }
            public string Codigo { get; set; }
            public string Descripcion { get; set; }
        }

        public class DsLibrosAsociados
        {
            public string CodigoLibro { get; set; }
            public string NombreLibro { get; set; }
            public string CodigoContrato { get; set; }
            public string NombreContrato { get; set; }
            public string UnidadMedida { get; set; }
            public string Monto { get; set; }
            public string Empresa { get; set; }
        }

        public class DsDocumentos2
        {
            public string Usuario { get; set; }
            public string Tipo { get; set; }
            public string Fecha { get; set; }
            public string Nombre { get; set; }
            public string Ruta { get; set; }
            public int Folio { get; set; }
        }

        //********
        public class DsUno
        {
            public string IdUno { get; set; }
            public string Nombre { get; set; }
            public string Descricion { get; set; }
        }
        public class DsVarios1
        {
            public string IdUno { get; set; }
            public string NombreVarios1 { get; set; }
        }
        public class DsVarios2
        {
            public string IdUno { get; set; }
            public string NombreVarios2 { get; set; }
        }

        public class DsModificaciones
        {
            public DateTime Fecha { get; set; }
            public string Modificacion { get; set; }
            public string NModif { get; set; }
            public decimal MontoCttoNeto { get; set; }
        }


    }
}