using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;

namespace LOD_APR.Areas.GLOD.ModelsViews
{
    public class AnotacionView
    {
        [Required(ErrorMessage = "La ID es obligatorio")]
        public int IdAnotacion { get; set; }

        [DisplayName("Folio")]
        public string Folio { get; set; }

        [Required(ErrorMessage = "El Título es obligatorio")]
        [StringLength(150, ErrorMessage = "El campo no puede exceder los 150 caracteres")]
        public string Titulo { get; set; }

        [Required(ErrorMessage = "El Cuerpo es obligatorio")]
        [AllowHtml]
        [DataType(DataType.MultilineText)]
        public string Cuerpo { get; set; }

        [DisplayName("Libro de Obra Digital")]
        public string LibroObras { get; set; }
        public int IdLibro { get; set; }

        [DisplayName("Tipo Comunicación")]
        public string Tipo { get; set; }

        [DisplayName("Subtipo Comunicación")]
        public string Subtipo { get; set; }

        [DisplayName("Subtipo Comunicación")]
        [ForeignKey("MAE_SubtipoComunicacion")]
        [Required(ErrorMessage = "El Subtipo es obligatorio")]
        [Range(1, 99999, ErrorMessage = "El Subtipo es obligatorio")]
        public int IdTipoSub { get; set; }

        public bool EsEstructurada { get; set; }
        public bool SolicitudRest { get; set; }
        public string FechaCreacion { get; set; }
        public string FechaRespuesta { get; set; }
        public string FechaTopeRespuesta { get; set; }
        public string UserBorrador { get; set; }

        public byte[] QR { get; set; }

        //DATOS CONTRACTUALES   
        public string RutContratista { get; set; }
        public string Contratista { get; set; }
        public string Contrato { get; set; }
        public string LibroDeObras { get; set; }

        public UsuarioActual UsuarioActual { get; set; }

        public Remitente Remitente { get; set; }

        public List<Receptor> Receptores { get; set; }
        
        public EstadoFirma EstadoFirma { get; set; }

        public EstadoAnotacion EstadoAnotacion { get; set; }

        public EstadoRespuesta EstadoRespuesta { get; set; }

        public List<Referencias> Referencias { get; set; }

        public List<Adjuntos> Adjuntos { get; set; }

    }
    public class Remitente
    {
        public string Nombre { get; set; }
        public string ImgRemitente { get; set; }
        public string InicialesRemitente { get; set; }
        public string Cargo { get; set; }
        public string Empresa { get; set; }
    }
    public class Receptor
    {
        public int IdAnotacion { get; set; }
        public int IdReceptor { get; set; }
        public string Nombre { get; set; }
        public string Cargo { get; set; }
        public string Imagen { get; set; }
        public string Iniciales { get; set; }
        public bool RespRespuesta { get; set; }
        public bool EsPrincipal { get; set; }
        public bool ReqVB { get; set; }
        public bool VistoBueno { get; set; }
        public string FechaVB { get; set; }
        public int TipoFirma { get; set; }
        public string TipoFirmaDetalle
        {
            get
            {
                string tipo = string.Empty;
                if (this.TipoFirma == 1)
                {
                    tipo = "Firma Electrónica Avanzada";
                }
                else if (this.TipoFirma == 2)
                {
                    tipo = "Firma MOP";
                }
                else if (this.TipoFirma == 3)
                {
                    tipo = "Firma Electrónica Simple";
                }
                else
                {
                    tipo = "Otro tipo de Firma";
                }
                return tipo;
            }
        }
    }
    public class EstadoFirma
    {
        public bool IsFirmada { get; set; }
        public int IdTipo { get; set; }
        public string UsuarioFirma { get; set; }
        public string FechaFirma { get; set; }
        public string TipoFirma
        {
            get
            {
                string tipo = string.Empty;
                if (this.IdTipo == 1)
                {
                    tipo = "Firma Electrónica Avanzada";
                }
                else if (this.IdTipo == 2)
                {
                    tipo = "Firma MOP";
                }
                else if (this.IdTipo == 3)
                {
                    tipo = "Firma Electrónica Simple";
                }
                else
                {
                    tipo = "Otro tipo de Firma";
                }
                return tipo;
            }
        }
        public string claseFirma
        {
            get
            {
                return (this.IsFirmada) ? "success" : "muted";
            }
        }
    }
    public class EstadoAnotacion
    {
        public int IdEstado { get; set; }
        public string Descripcion
        {
            get
            {
                string tipo = string.Empty;
                if (this.IdEstado == 0)
                {
                    tipo = "Borrador";
                }
                else if (this.IdEstado == 1)
                {
                    tipo = "Solicitud de Firma";
                }
                else if (this.IdEstado == 2)
                {
                    tipo = "Publicada";
                }
                else
                {
                    tipo = "Firma Pendiente";
                }
                return tipo;
            }
        }
        public string claseEstado
        {
            get
            {
                string clase = string.Empty;
                if (this.IdEstado == 0)
                {
                    clase = "muted";
                }
                else if (this.IdEstado == 1)
                {
                    clase = "warning";
                }
                else if (this.IdEstado == 2)
                {
                    clase = "success";
                }
                else
                {
                    clase = "muted";
                }
                return clase;
            }
        }

    }
    public class EstadoRespuesta
    {
        public bool Respondida { get; set; }
        public string FechaRespuesta { get; set; }
        public string FolioRespuesta { get; set; }
        public string UsuarioRespuesta { get; set; }
        public string TituloRespuesta { get; set; }
        public int IdRespuesta { get; set; }
        public string Descripcion
        {
            get
            {
                string desc = string.Empty;
                if (!this.Respondida)
                {
                    desc = "Pendiente Respuesta";
                }
                else
                {
                    desc = "Respondida";
                }
                return desc;
            }
        }
    }
    public class UsuarioActual
    {
        public bool EsDestacada { get; set; }
        public bool DebeResponder { get; set; }
        public bool DebeDarVistoBueno { get; set; }
    }
    public class Referencias
    {
        public int IdRefAnot { get; set; }
        public int IdAnotacion { get; set; }
        public string Libro { get; set; }
        public string Folio { get; set; }
        public string Anotacion { get; set; }
        public string Remitente { get; set; }
        public string Fecha { get; set; }
        public string Observacion { get; set; }
    }
    public class Adjuntos
    {
        public string Tipo { get; set; }
        public string Subtipo { get; set; }
        public string Documento { get; set; }
    }

}