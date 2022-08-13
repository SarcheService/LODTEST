using System.Collections.Generic;
using System.Linq;
using static LOD_APR.Models.Auxiliares;

namespace LOD_APR.Areas.Admin.Helpers
{
    public static class TiposPreguntas
    {
        private static List<ConfigPreguntas> LstTipos = new List<ConfigPreguntas>() {
            new ConfigPreguntas(){Id=1,Titulo="Texto Simple",Descripcion="1 Línea de texto hasta 200 caracteres",Clase="text-box",Icono="<i class=\"fa fa-text-width\" aria-hidden=\"true\"></i>", OnEmbebido=true},
            new ConfigPreguntas(){Id=2,Titulo="Texto Largo",Descripcion="Varias líneas hasta 5.000 caracteres",Clase="multi-line",Icono="<i class=\"fa fa-text-height\" aria-hidden=\"true\"></i>", OnEmbebido=true},
            new ConfigPreguntas(){Id=3,Titulo="Número Entero",Descripcion="Separador de miles sin coma",Clase="entero",Icono="N0", OnEmbebido=true},
            new ConfigPreguntas(){Id=4,Titulo="Número Decimal",Descripcion="Separador de miles con 2 dígitos decimales",Clase="decimal",Icono="D2", OnEmbebido=true},
            new ConfigPreguntas(){Id=5,Titulo="Teléfono",Descripcion="Formato chile +56 y 9 dígitos",Clase="telefono",Icono="<i class=\"fa fa-phone\" aria-hidden=\"true\"></i>", OnEmbebido=true},
            new ConfigPreguntas(){Id=6,Titulo="Moneda",Descripcion="Número entero con separador de miles",Clase="moneda",Icono="<i class=\"fa fa-usd\" aria-hidden=\"true\"></i>", OnEmbebido=true},
            new ConfigPreguntas(){Id=7,Titulo="Email",Descripcion="Formato estándar de email",Clase="email",Icono="<i class=\"fa fa-at\" aria-hidden=\"true\"></i>", OnEmbebido=true},
            new ConfigPreguntas(){Id=8,Titulo="RUT",Descripcion="Incluye separador de miles y guión",Clase="rut",Icono="<i class=\"fa fa-user\" aria-hidden=\"true\"></i>", OnEmbebido=true},
            new ConfigPreguntas(){Id=9,Titulo="Fecha",Descripcion="Formato es-CL dd-MM-yyyy",Clase="fecha",Icono="<i class=\"fa fa-calendar\" aria-hidden=\"true\"></i>", OnEmbebido=true},
            new ConfigPreguntas(){Id=901,Titulo="Hora",Descripcion="Formato HH:mm",Clase="hora",Icono="<i class=\"fa fa-clock-o\" aria-hidden=\"true\"></i>", OnEmbebido=true},
            new ConfigPreguntas(){Id=902,Titulo="Fecha y Hora",Descripcion="Formato es-CL dd-MM-yyyy HH:mm",Clase="fechahora",Icono="<i class=\"fa fa-calendar\" aria-hidden=\"true\"></i>", OnEmbebido=true},
            new ConfigPreguntas(){Id=903,Titulo="Dirección IP",Descripcion="Formato estándar 255.255.255.255",Clase="direccionip",Icono="<i class=\"fa fa-desktop\" aria-hidden=\"true\"></i>", OnEmbebido=true},
            new ConfigPreguntas(){Id=913,Titulo="Selección Tipo Combo",Descripcion="Opciones tipo texto",Clase="combo",Icono="<i class=\"fa fa-caret-square-o-down\" aria-hidden=\"true\"></i>", OnEmbebido=true},
            new ConfigPreguntas(){Id=914,Titulo="Selección Múltiple",Descripcion="Opciones tipo texto",Clase="select-multiple",Icono="<i class=\"fa fa-check-square-o\" aria-hidden=\"true\"></i>", OnEmbebido=false},
            new ConfigPreguntas(){Id=915,Titulo="Opción Única",Descripcion="Opciones tipo texto",Clase="select-unica",Icono="<i class=\"fa fa-dot-circle-o\" aria-hidden=\"true\"></i>", OnEmbebido=false},
              new ConfigPreguntas(){Id=904,Titulo="Formulario Incrustado",Descripcion="Insertar un Formulario Básico",Clase="form",Icono="<i class=\"fa fa-dot-circle-o\" aria-hidden=\"true\"></i>", OnEmbebido=false},
        };

        public static ConfigPreguntas GetTipo(int IdTipo)
        {
            return LstTipos.Where(c => c.Id == IdTipo).FirstOrDefault();
        }

        public static List<ComboBoxEstandar> ListTipoPregunta(bool embebido)
        {
            List<ComboBoxEstandar> ListTipoParam = new List<ComboBoxEstandar>();
            List<ConfigPreguntas> opciones = LstTipos;

            if (embebido)
                opciones = LstTipos.Where(x => x.OnEmbebido == true).ToList();

            foreach(ConfigPreguntas p in opciones)
            {
                ListTipoParam.Add(new ComboBoxEstandar { Id = p.Id.ToString(), Value = $"{p.Titulo} | {p.Descripcion}" });
            }
            
            return (ListTipoParam);
        }
    }

    public class ConfigPreguntas
    {
        public int Id { get; set; }
        public string Titulo { get; set; }
        public string Descripcion { get; set; }
        public string Clase { get; set; }
        public string Icono { get; set; }
        public bool OnEmbebido { get; set; }
    }
}