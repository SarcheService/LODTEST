using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace LOD_APR.Areas.GLOD.Models
{
    [Table("FORM_Informes")]
    public class FORM_Informes
    {
        [Key]
        [DisplayName("ID Envío")]
        public int IdEnvio { get; set; }

        [ForeignKey("CON_Contratos")]
        public int IdContrato { get; set; }
        public virtual CON_Contratos CON_Contratos { get; set; }

        [DisplayName("Mes Informado")]
        public string MesInformado { get; set; }

        [DisplayName("Año Informado")]
        [MaxLength(4,ErrorMessage ="Máximo 4 números")]
        public string Anio { get; set; }

        [DisplayName("Mes")]
        public string Mes { get; set; }

        [DisplayName("Creado por")]
        public string Usuario { get; set; }

        [DisplayName("Creado el")]
        public DateTime FechaCreacion { get; set; }

        [DisplayName("Despachado el")]
        public DateTime? FechaDespacho { get; set; }

        [DisplayName("Estado")]
        public bool Estado { get; set; }

        [DisplayName("Tipo")]
        public int Tipo { get; set; }

        [NotMapped]
        public string EstadoDescript {
            get {
                return (!this.Estado ? "<label class='label label-warning'>Pendiente Envío</label>" : "<label class='label label-success'>Enviado</label>");
            }
        }

        [NotMapped]
        public string FechaDespachoDescript
        {
            get
            {
                return ((this.FechaDespacho!=null) ? FechaDespacho.ToString() : "");
            }
        }

        [NotMapped]
        public bool PermiteCerrar
        {
            get
            {
                if (this.FORM_InformesItems != null)
                {
                    int count = this.FORM_InformesItems.Where(i => i.FORM_Formularios.Obligatorio && (i.Estado < 2 || i.Estado == 4)).Count();
                    return ((count > 0) ? false : true);
                }
                else
                {
                    return false;
                }
            }
        }

        [NotMapped]
        public bool PermiteEliminar
        {
            get
            {
                if (this.FORM_InformesItems != null)
                {
                    int count = this.FORM_InformesItems.Where(i => i.Estado > 0).Count();
                    return ((count > 0) ? false : true);
                }
                else
                {
                    return true;
                }
            }
        }
        public virtual List<FORM_InformesItems> FORM_InformesItems { get; set; }

    }
}