using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace LOD_APR.Areas.Admin.Models
{
    [Table("MAE_RolesContrato")]
    public class MAE_RolesContrato
    {
        [Key]
        public int IdRolCtto { get; set; }

        [DisplayName("Nombres")]
        [Required(ErrorMessage = "Dato obligatorio")]
        public string NombreRol { get; set; }
        [DisplayName("Descripción")]
        [DataType(DataType.MultilineText)]
        public string Descripcion { get; set; }
        [DisplayName("Activo")]
        public bool Activo { get; set; } 

        //Segun Tipo de Empresa
        public bool EsGubernamental { get; set; }
        public bool EsFiscalizador { get; set; }
        public bool EsContratista { get; set; }

        //Opciones del Libro especifico al cual pertenece
        [ForeignKey("MAE_TipoLOD")]
        public int IdTipoLod { get; set; }
        public virtual MAE_TipoLOD MAE_TipoLOD { get; set; }
        public bool Lectura { get; set; }
        public bool Escritura { get; set; }
        public bool FirmaGob { get; set; }
        public bool FirmaFea { get; set; }
        public bool FirmaSimple { get; set; }

        //Opciones del Libro Obra Mestro
        public bool Lectura1 { get; set; }
        public bool Escritura1 { get; set; }
        public bool FirmaGob1 { get; set; }
        public bool FirmaFea1 { get; set; }
        public bool FirmaSimple1 { get; set; }

        //Opciones del Libro Comunicaciones
        public bool Lectura2 { get; set; }
        public bool Escritura2 { get; set; }
        public bool FirmaGob2 { get; set; }
        public bool FirmaFea2 { get; set; }
        public bool FirmaSimple2 { get; set; }

        //Opciones de Libros de Comunicaciones
        public bool Lectura3 { get; set; }
        public bool Escritura3 { get; set; }
        public bool FirmaGob3 { get; set; }
        public bool FirmaFea3 { get; set; }
        public bool FirmaSimple3 { get; set; }

        //Opciones de Libros de Auxiliares o complementarios
        public bool Lectura4 { get; set; }
        public bool Escritura4 { get; set; }
        public bool FirmaGob4 { get; set; }
        public bool FirmaFea4 { get; set; }
        public bool FirmaSimple4 { get; set; }


        //Definición Deacuerdo al ROL

        public bool RolGubernamental { get; set; }
        public bool RolFiscalizador { get; set; }
        public bool RolContratista { get; set; }

        public int Jerarquia { get; set; }

    }
}