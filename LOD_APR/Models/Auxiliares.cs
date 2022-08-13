
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LOD_APR.Models
{
    public class Auxiliares
    {
        public class ComboBoxEstandar
        {
            public string Id { get; set; }
            public string Value { get; set; }
        }

        public class ComboBoxEstandar2
        {
            public int Id { get; set; }
            public string Value { get; set; }
        }

        public class DataPanelLO
        {
            public int numTodos { get; set; }
            public int numMias { get; set; }
            public int numBorr { get; set; }
            public int numDest { get; set; }
            public int numNombEn { get; set; }
            public int numPend { get; set; }

        }

        public class UserSelect
        {
            public int PersonalId { get; set; }
            public string Nombre { get; set; }
            public string RutaImg { get; set; }
        }

        //public class LibYBit  //Envía vista de Libros y Bitacoras
        //{
        //    public List<ASP_LibroObras> Libros { get; set; }
        //    public List<ASP_bitacoras> Bitacoras { get; set; }
        //}

        public class LobrasSelect
        {
            public int IdLibroObra { get; set; }
            public string Nombre { get; set; }
            public string NombreContrato { get; set; }
            public string NombreCompuesto {
                get {
                    return Nombre + "/" + NombreContrato;
                }
            }
        }

        public class UserRemitente //Usado para mostrar el remitente de la anotacion en Detail 
        {
            public string Cargo { get; set; }
            public string Nombre { get; set; }
            public bool IsPersOrCont { get; set; } //Para saber si es personal o contacto
            public string RutaImg { get; set; }
            public string Empresa { get; set; }
        }

        public class DocZip // No se me ocurre algun nombre bueno
        {
            public int IdDoc { get; set; }
            public string PathToRead { get; set; }
        }
    }
    
}