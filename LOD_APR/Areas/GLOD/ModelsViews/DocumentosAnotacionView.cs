namespace LOD_APR.Areas.GLOD.ModelsViews
{
    public class DocumentosAnotacionView
    {
        public int IdDocanot { get; set; }
        public int IdAnotacion { get; set; }
        public int IdTipoDoc { get; set; }
        public int IdEstado { get; set; }
        public int TipoClasi { get; set; }

        public bool anotFirmada { get; set; }

        public string TipoDoc { get; set; }
        public string Titulo { get; set; }
        public string Descripcion { get; set; }
        public string Ruta { get; set; }
        public string CreadoPor { get; set; }
        public string CreadoEl { get; set; }
        public string Subtipo { get; set; }
        public string Tipo { get; set; }

        public string UsuarioEvento { get; set; }
        public string FechaEvento { get; set; }
        public string Observaciones { get; set; }

        public bool IsResponsable { get; set; }

        public string CategoriaDoc { get {
                string clasi = string.Empty;
                switch(this.TipoClasi)
                {
                    case 1:
                        clasi = "Formulario";
                        break;
                    case 2:
                        clasi = "Doc.Técnico";
                        break;
                    case 3:
                        clasi = "Doc.Administrativo";
                        break;
                    default:
                        clasi = "Otros";
                        break;
                }
                return clasi;
            }
        }
        public string EstadoDescript
        {
            get
            {
                string estado = string.Empty;
                if (this.IdTipoDoc == 19 && this.IdEstado != 0)
                {
                    estado = "Cargado";
                }
                else if (this.IdEstado == 0)
                {
                    estado = "Pendiente";
                }
                else if (this.IdEstado == 1)
                {
                    estado = "Pend. Aprobación";
                }
                else if (this.IdEstado == 2)
                {
                    estado = "Aprobado";
                }
                else if (this.IdEstado == 3)
                {
                    estado = "Rechazado";
                }

                return estado;
            }
        }
        public string EstadoClass
        {
            get
            {
                string estado = string.Empty;
                if (this.IdTipoDoc == 19 && this.IdEstado != 0)
                {
                    estado = "label label-success";
                }
                else if (this.IdEstado == 0)
                {
                    estado = "label label-warning";
                }
                else if (this.IdEstado == 1)
                {
                    estado = "label label-warning";
                }
                else if (this.IdEstado == 2)
                {
                    estado = "label label-success";
                }
                else if (this.IdEstado == 3)
                {
                    estado = "label label-danger";
                }

                return estado;
            }
        }
    }
}