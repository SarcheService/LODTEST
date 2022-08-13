using System;

namespace LOD_APR.Helpers.ModelsHelpers
{
    public class Filtro_Personal
    {
        public string Texto { get; set; }
        public bool Activo { get; set; }
        public bool Inactivo { get; set; }
        public int Faena { get; set; }
        public int Cargo { get; set; }
        public int Unidad { get; set; }
        public string Tags { get; set; }
    }



    public class Filtro_Solicitudes_Tic
    {
        public bool MisAsignaciones { get; set; }

        public int[] Estados { get; set; }

        public int Categoria { get; set; }
        public int SubCategoria { get; set; }

        public DateTime FDesdeCreacion { get; set; }
        public DateTime FHastaCreacion { get; set; }
        public DateTime FDesdeCierre { get; set; }
        public DateTime FHastaCierre { get; set; }

        public int Personal { get; set; }

        public int UsuarioCreador { get; set; }

        public int UsuarioVb { get; set; }


    }
    public class Filtro_Activos
    {
        public bool Componente { get; set; }
        public bool Asignado { get; set; }
        public int IdPersonal { get; set; }
        public int IdModelo { get; set; }
        public int IdMarca { get; set; }
        public int IdClase { get; set; }

    }
    public class Filtro_Sujetos
    {
        public string Texto { get; set; }
        public bool Activo { get; set; }
        public bool Inactivo { get; set; }
        public bool Cliente { get; set; }
        public bool Proveedor { get; set; }
        public bool Contratista { get; set; }
        public bool Relacionada { get; set; }
        public bool Organizacion { get; set; }
        public string Tags { get; set; }
    }


    public class Filtro_Audiencias
    {
        public string Vista { get; set; }
        public bool Programada { get; set; }
        public bool Cancelada { get; set; }
        public string FDesde { get; set; }
        public string FHasta { get; set; }

        public DateTime? FDesdeDate
        {
            get
            {
                DateTime? dtime = null;
                if (FDesde != null)
                    dtime = DateTime.Parse(FDesde);

                return dtime;
            }
            set { }
        }

        public DateTime? FHastaDate
        {
            get
            {
                DateTime? dtime = null;
                if (FHasta != null)
                    dtime = DateTime.Parse(FHasta);

                return dtime;
            }
            set { }
        }
    }


    public class Filtro_SeguimientoAUD
    {
        public bool Ingresada { get; set; }
        public bool Programada { get; set; }
        public bool Ejecutada { get; set; }
        public bool Finalizada { get; set; }
        public bool Cerrada { get; set; }
        public bool Cancelada { get; set; }
        public string Solicitante { get; set; }
        public int TFecha { get; set; }
        public string FDesde { get; set; }
        public string FHasta { get; set; }
        public DateTime? FDesdeDate
        {
            get
            {
                DateTime? dtime = null;
                if (FDesde != null)
                    dtime = DateTime.Parse(FDesde);

                return dtime;
            }
            set { }
        }

        public DateTime? FHastaDate
        {
            get
            {
                DateTime? dtime = null;
                if (FHasta != null)
                    dtime = DateTime.Parse(FHasta);

                return dtime;
            }
            set { }
        }
    }


    public class Filtro_Solicitudes
    {
        public bool Ingresada { get; set; }
        public bool Entregada { get; set; }
        public int TFecha { get; set; }
        public string FDesde { get; set; }
        public string FHasta { get; set; }

        public DateTime? FDesdeDate
        {
            get
            {
                DateTime? dtime = null;
                if (FDesde != null)
                    dtime = DateTime.Parse(FDesde);

                return dtime;
            }
            set { }
        }

        public DateTime? FHastaDate
        {
            get
            {
                DateTime? dtime = null;
                if (FHasta != null)
                    dtime = DateTime.Parse(FHasta);

                return dtime;
            }
            set { }
        }
    }

    public class Filtro_ActivosSolicitud
    {
       
        public int IdModelo { get; set; }
        public int IdMarca { get; set; }
        public int IdClase { get; set; }
        public int IdFaena { get; set; }
        public string NSerie { get; set; }
        public string NParte { get; set; }
        public string CodigoInterno { get; set; }

    }


    public class Filtro_Adquisiciones
    {
        public bool Ingresada { get; set; }
        public bool Tramitada { get; set; }
        public bool ListaEntrega { get; set; }
        public bool RechazoBeneficio { get; set; }
        public int TFecha { get; set; }
        public string FDesde { get; set; }
        public string FHasta { get; set; }
        public DateTime? FDesdeDate
        {
            get
            {
                DateTime? dtime = null;
                if (FDesde != null)
                    dtime = DateTime.Parse(FDesde);

                return dtime;
            }
            set { }
        }

        public DateTime? FHastaDate
        {
            get
            {
                DateTime? dtime = null;
                if (FHasta != null)
                    dtime = DateTime.Parse(FHasta);

                return dtime;
            }
            set { }
        }
    }


    public class Filtro_Seguimiento
    {
        public bool Ingresada { get; set; }
        public bool Tramitada { get; set; }
        public bool ListaEntrega { get; set; }
        public bool RechazoBeneficio { get; set; }
        public bool Rechazada { get; set; }
        public bool Entregada { get; set; }
        public int TFecha { get; set; }
        public string FDesde { get; set; }
        public string FHasta { get; set; }
        public DateTime? FDesdeDate
        {
            get
            {
                DateTime? dtime = null;
                if (FDesde != null)
                    dtime = DateTime.Parse(FDesde);

                return dtime;
            }
            set { }
        }

        public DateTime? FHastaDate
        {
            get
            {
                DateTime? dtime = null;
                if (FHasta != null)
                    dtime = DateTime.Parse(FHasta);

                return dtime;
            }
            set { }
        }
    }


    public class Filtro_RepBenRes
    {
        public bool Ingresada { get; set; }
        public bool Tramitada { get; set; }
        public bool ListaEntrega { get; set; }
        public bool RechazoBeneficio { get; set; }
        public bool Rechazada { get; set; }
        public bool Entregada { get; set; }


        public int TipoBeneficios { get; set; }
        public int Beneficios { get; set; }
        public int LogicaAdquisicion { get; set; }
        public int REtario { get; set; }

        public int TFecha { get; set; }
        public string FDesde { get; set; }
        public string FHasta { get; set; }
        public DateTime? FDesdeDate
        {
            get
            {
                DateTime? dtime = null;
                if (FDesde != null)
                    dtime = DateTime.Parse(FDesde);

                return dtime;
            }
            set { }
        }

        public DateTime? FHastaDate
        {
            get
            {
                DateTime? dtime = null;
                if (FHasta != null)
                    dtime = DateTime.Parse(FHasta);

                return dtime;
            }
            set { }
        }
    }


    public class Filtro_Derivaciones
    {

        public bool Derivada { get; set; }
        public bool RyP { get; set; }
        public bool Retrasada { get; set; }
        public bool Finalizada { get; set; }
        public bool FyR { get; set; }
        public bool Cancelada { get; set; }
        public int TFecha { get; set; }
        public string FDesde { get; set; }
        public string FHasta { get; set; }

        public DateTime? FDesdeDate
        {
            get
            {
                DateTime? dtime = null;
                if (FDesde != null)
                    dtime = DateTime.Parse(FDesde);

                return dtime;
            }
            set { }
        }

        public DateTime? FHastaDate
        {
            get
            {
                DateTime? dtime = null;
                if (FHasta != null)
                    dtime = DateTime.Parse(FHasta);

                return dtime;
            }
            set { }
        }
    }
    public class Filtro_BibliotecaDoc
    {
        public string Texto { get; set; }
        public bool UA { get; set; }
        public string TDoc { get; set; }
        public string CatDoc { get; set; }
        public bool Publico { get; set; }
        public bool Corporativo { get; set; }
        public bool Interno { get; set; }
        public bool Privado { get; set; }
        public int IdJerarquia { get; set; }
        public int Padre { get; set; }
        public int TFecha { get; set; }
        public string FDesde { get; set; }
        public string FHasta { get; set; }

        public DateTime? FDesdeDate
        {
            get
            {
                DateTime? dtime = null;
                if (FDesde != null)
                    dtime = DateTime.Parse(FDesde);

                return dtime;
            }
            set { }
        }

        public DateTime? FHastaDate
        {
            get
            {
                DateTime? dtime = null;
                if (FHasta != null)
                    dtime = DateTime.Parse(FHasta);

                return dtime;
            }
            set { }
        }




    }





    public class BIE_Filtro_Socios
    {
        public int Areas { get; set; }
        public int SubAreas { get; set; }
        public int DeBaja { get; set; }
      
        public int EsJubilado { get; set; }
        public int Sexo { get; set; }
        public int IdBanco { get; set; }

    }

    public class BIE_Filtro_Cargas
    {
        public int DeBaja { get; set; }
        public int IdSocio { get; set; }
        public int Sexo { get; set; }

    }

    public class BIE_Filtro_Solicitudes
    {
        public int Estado { get; set; }
        public int IdSocio { get; set; }
        public string FDesde { get; set; }
        public string FHasta { get; set; }

        public DateTime? FDesdeDate
        {
            get
            {
                DateTime? dtime = null;
                if (FDesde != null)
                    dtime = DateTime.Parse(FDesde);

                return dtime;
            }
            set { }
        }

        public DateTime? FHastaDate
        {
            get
            {
                DateTime? dtime = null;
                if (FHasta != null)
                    dtime = DateTime.Parse(FHasta);

                return dtime;
            }
            set { }
        }

    }
    public class BIE_Filtro_Voucher
    {
        public int Estado { get; set; }
        public string FDesde { get; set; }
        public string FHasta { get; set; }

        public DateTime? FDesdeDate
        {
            get
            {
                DateTime? dtime = null;
                if (FDesde != null)
                    dtime = DateTime.Parse(FDesde);

                return dtime;
            }
            set { }
        }

        public DateTime? FHastaDate
        {
            get
            {
                DateTime? dtime = null;
                if (FHasta != null)
                    dtime = DateTime.Parse(FHasta);

                return dtime;
            }
            set { }
        }

    }
    public class BIE_Filtro_Historico
    {
        public int IdSocio { get; set; }
        public int IdTipoBeneficio { get; set; }
        public int IdBeneficio { get; set; }

    }


    public class TIC_Filtro_Evaluaciones
    {
        public int IdCategoria { get; set; }
        public int IdSubcategoria { get; set; }        
        public int IdPersonal { get; set; }
        public int[] Estado { get; set; }
        public DateTime FDesde { get; set; }
        public DateTime FHasta { get; set; }

    }

}