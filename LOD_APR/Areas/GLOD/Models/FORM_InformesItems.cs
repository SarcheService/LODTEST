using LOD_APR.Areas.Admin.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LOD_APR.Areas.GLOD.Models
{
    [Table("FORM_InformesItems")]
    public class FORM_InformesItems
    {
        [Key]
        [DisplayName("ID Ítem")]
        public int IdItem { get; set; }

        [ForeignKey("FORM_Informes")]
        public int? IdInforme { get; set; }
        public virtual FORM_Informes FORM_Informes { get; set; }

        [ForeignKey("FORM_Formularios")]
        public string IdForm { get; set; }
        public virtual FORM_Formularios FORM_Formularios { get; set; }

        [DisplayName("Título")]
        public string Titulo { get; set; }
        
        [DisplayName("Creado por")]
        public string Usuario { get; set; }

        [DisplayName("Despachado el")]
        public DateTime? FechaDespacho { get; set; }

        [DisplayName("Estado")]
        public int Estado { get; set; }

        [ForeignKey("LOD_Anotacion")]
        public int? IdAnotacion { get; set; }
        public virtual LOD_Anotaciones LOD_Anotacion { get; set; }

        [ForeignKey("CON_Contratos")]
        public int? IdContrato { get; set; }
        public virtual CON_Contratos CON_Contratos { get; set; }

        [NotMapped]
        public string EstadoDescript
        {
            get
            {
                string estado = string.Empty;
                if (this.Estado == 0)
                {
                    estado = "<label class='label label-warning'>Ingreso Pendiente</label>";
                }
                else if (this.Estado == 1)
                {
                    estado = "<label class='label label-info'>Envio Pendiente</label>";
                }
                else if(this.Estado == 2)
                {
                    estado = "<label class='label label-success'>Enviado</label>";
                }else if(this.Estado == 3)
                {
                    estado = "<label class='label label-success'>Aprobado</label>";
                }else if(this.Estado == 4)
                {
                    estado = "<label class='label label-danger'>Rechazado</label>";
                }
                return estado;
            }
        }

        [NotMapped]
        public string FechaDespachoDescript
        {
            get
            {
                return ((this.FechaDespacho != null) ? FechaDespacho.ToString() : "");
            }
        }

        [NotMapped]
        public string FolioEnvio
        {
            get
            {
                return ((this.IdAnotacion != null) ? this.LOD_Anotacion.Correlativo.ToString().PadLeft(6,'0') : "");
            }
        }
        [NotMapped]
        public string LibroEnvio
        {
            get
            {
                return ((this.IdAnotacion != null) ? this.LOD_Anotacion.LOD_LibroObras.NombreLibroObra : "");
            }
        }

        [NotMapped]
        public string ObligatorioDescript
        {
            get
            {
                return ((this.FORM_Formularios.Obligatorio) ? "Sí": "No");
            }
        }

        public virtual List<FORM_InformesItemsData> FORM_InformesItemsData { get; set; }
    }
}