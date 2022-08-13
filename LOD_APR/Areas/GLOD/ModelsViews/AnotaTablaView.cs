using System;

namespace LOD_APR.Areas.GLOD.ModelsViews
{
    public class AnotaTablaView
    {
        public AnotaTablaView(bool isfirmada, int tipofirma)
        {
            EstadoFirma = new EstadoFirma() { IsFirmada = isfirmada, IdTipo = tipofirma };
            DatosRemitente = new Remitente();
        }
        public int IdAnotacion { get; set; }
        public string Titulo { get; set; }
        public string Correlativo { get; set; } = "-";
        public EstadoFirma EstadoFirma { get; set; }
        public Remitente DatosRemitente { get; set; }
        public string FechaPublicacion { get; set; } = "-";
        public bool IsReferencias { get; set; }
        public bool IsLeida { get; set; }
        public string claseLeida
        {
            get
            {
                return (this.IsLeida) ? "read" : "active unread";
            }
        }
        public bool IsDestacada { get; set; }
        public string claseDestacada
        {
            get
            {
                return (this.IsDestacada) ? "fa fa-star text-warning" : "fa fa-star-o";
            }
        }

        public int correlativoNum { 
            get
            {
                try
                {
                    int correlativo = Convert.ToInt32(Correlativo);
                    return correlativo;
                }catch(Exception ex)
                {
                    return 0;
                }
            } 
        }

    }


    public class Firma
    {
        public bool IsFirmada { get; set; }
        public int IdTipo { get; set; }
        public Firma(bool isfirmada, int tipo)
        {
            IsFirmada = isfirmada;
            IdTipo = tipo;
        }
        public string TipoFirma
        {
            get
            {
                string tipo = string.Empty;
                if (this.IdTipo == 1)
                {
                    tipo = "Firma Avanzada";
                }
                else if (this.IdTipo == 2)
                {
                    tipo = "Firma MINSEGPRE";
                }
                else if (this.IdTipo == 3)
                {
                    tipo = "Firma Básica";
                }
                else
                {
                    tipo = "Firma Pendiente";
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
}