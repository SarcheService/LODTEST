using System.Collections.Generic;

namespace Galena.Helpers
{
    public class Meses
    {
        public int IdMes { get; set; }
        public string Nombre { get; set; }
        
        /*Utilizado en Proyecciones*/
        public decimal TotalColumna { get; set; }

        public static List<Meses> MAE_Meses()
        {
            List<Meses> ListaMeses = new List<Meses>() {
                new Meses { IdMes = 1, Nombre = "Enero" },
                new Meses { IdMes = 2, Nombre = "Febrero" },
                new Meses { IdMes = 3, Nombre = "Marzo" },
                new Meses { IdMes = 4, Nombre = "Abril" },
                new Meses { IdMes = 5, Nombre = "Mayo" },
                new Meses { IdMes = 6, Nombre = "Junio" },
                new Meses { IdMes = 7, Nombre = "Julio" },
                new Meses { IdMes = 8, Nombre = "Agosto" },
                new Meses { IdMes = 9, Nombre = "Septiembre" },
                new Meses { IdMes = 10, Nombre = "Octubre" },
                new Meses { IdMes = 11, Nombre = "Noviembre" },
                new Meses { IdMes = 12, Nombre = "Diciembre" }
            };

            return ListaMeses;
        }
    }
}