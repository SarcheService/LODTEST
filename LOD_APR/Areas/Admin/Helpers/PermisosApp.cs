using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LOD_APR.Areas.Admin.Helpers
{
    public class PermisosUsuariosApp
    {
        public string IdPermiso { get; set; }
        public string IdApp { get; set; }
        public int NumPermiso { get; set; }
        public string Nombre { get; set; }

        public static List<PermisosUsuariosApp> SEG_PermisosUsuariosApp()
        {
            List<PermisosUsuariosApp> ListPermisosUA = new List<PermisosUsuariosApp>() {
              
                //ADMIN
                new PermisosUsuariosApp { IdPermiso = "00100901", IdApp = "Admin", NumPermiso = 1, Nombre = "Crear Usuario" },
                new PermisosUsuariosApp { IdPermiso = "00100902", IdApp = "Admin", NumPermiso = 2, Nombre = "Editar Usuario" },
                new PermisosUsuariosApp { IdPermiso = "00100903", IdApp = "Admin", NumPermiso = 3, Nombre = "Eliminar Perfiles" },

                //AUD
                new PermisosUsuariosApp { IdPermiso = "0130100001", IdApp = "AUD", NumPermiso = 1, Nombre = "Crear Usuario" },
                new PermisosUsuariosApp { IdPermiso = "0130100002", IdApp = "AUD", NumPermiso = 2, Nombre = "Editar Usuario" },
                new PermisosUsuariosApp { IdPermiso = "0130100003", IdApp = "AUD", NumPermiso = 3, Nombre = "Eliminar Perfiles" },
                //TIC
                new PermisosUsuariosApp { IdPermiso = "0200060001", IdApp = "TIC", NumPermiso = 1, Nombre = "Agrega Usuario" },
                new PermisosUsuariosApp { IdPermiso = "0200060002", IdApp = "TIC", NumPermiso = 2, Nombre = "Editar Usuario" },
                new PermisosUsuariosApp { IdPermiso = "0200060003", IdApp = "TIC", NumPermiso = 3, Nombre = "Eliminar Usuario" },
                
               //PER
                new PermisosUsuariosApp { IdPermiso = "00500701", IdApp = "PER", NumPermiso = 1, Nombre = "Agrega Usuario" },
                new PermisosUsuariosApp { IdPermiso = "00500702", IdApp = "PER", NumPermiso = 2, Nombre = "Editar Usuario" },
                new PermisosUsuariosApp { IdPermiso = "00500703", IdApp = "PER", NumPermiso = 3, Nombre = "Eliminar Usuario" },
                
               //GAE
                new PermisosUsuariosApp { IdPermiso = "0020050001", IdApp = "GAE", NumPermiso = 1, Nombre = "Agrega Usuario" },
                new PermisosUsuariosApp { IdPermiso = "0020050002", IdApp = "GAE", NumPermiso = 2, Nombre = "Editar Usuario" },
                new PermisosUsuariosApp { IdPermiso = "0020050003", IdApp = "GAE", NumPermiso = 3, Nombre = "Eliminar Usuario" },
              
                //Biblioteca
                new PermisosUsuariosApp { IdPermiso = "00700601", IdApp = "BIB", NumPermiso = 1, Nombre = "Agrega Usuario" },
                new PermisosUsuariosApp { IdPermiso = "00700602", IdApp = "BIB", NumPermiso = 2, Nombre = "Editar Usuario" },
                new PermisosUsuariosApp { IdPermiso = "00700603", IdApp = "BIB", NumPermiso = 3, Nombre = "Eliminar Usuario" },
            
                //BSD 
                new PermisosUsuariosApp { IdPermiso = "0110180001", IdApp = "BSD", NumPermiso = 1, Nombre = "Agrega Usuario" },
                new PermisosUsuariosApp { IdPermiso = "0110180002", IdApp = "BSD", NumPermiso = 2, Nombre = "Editar Usuario" },
                new PermisosUsuariosApp { IdPermiso = "0110180003", IdApp = "BSD", NumPermiso = 3, Nombre = "Eliminar Usuario" },

                //BIE
                new PermisosUsuariosApp { IdPermiso = "0210160001", IdApp = "BIE", NumPermiso = 1, Nombre = "Agrega Usuario" },
                new PermisosUsuariosApp { IdPermiso = "0210160002", IdApp = "BIE", NumPermiso = 2, Nombre = "Editar Usuario" },
                new PermisosUsuariosApp { IdPermiso = "0210160003", IdApp = "BIE", NumPermiso = 3, Nombre = "Eliminar Usuario" },
                
                //OIRS
                new PermisosUsuariosApp { IdPermiso = "0220130001", IdApp = "OIRS", NumPermiso = 1, Nombre = "Agrega Usuario" },
                new PermisosUsuariosApp { IdPermiso = "0220130002", IdApp = "OIRS", NumPermiso = 2, Nombre = "Editar Usuario" },
                new PermisosUsuariosApp { IdPermiso = "0220130003", IdApp = "OIRS", NumPermiso = 3, Nombre = "Eliminar Usuario" },

                //FORMS
                new PermisosUsuariosApp { IdPermiso = "0080010001", IdApp = "FORMS", NumPermiso = 1, Nombre = "Agrega Usuario" },
                new PermisosUsuariosApp { IdPermiso = "0080010002", IdApp = "FORMS", NumPermiso = 2, Nombre = "Editar Usuario" },
                new PermisosUsuariosApp { IdPermiso = "0080010003", IdApp = "FORMS", NumPermiso = 3, Nombre = "Eliminar Usuario" },

                //EVA
                new PermisosUsuariosApp { IdPermiso = "0100030001", IdApp = "EVA", NumPermiso = 1, Nombre = "Agrega Usuario" },
                new PermisosUsuariosApp { IdPermiso = "0100030002", IdApp = "EVA", NumPermiso = 2, Nombre = "Editar Usuario" },
                new PermisosUsuariosApp { IdPermiso = "0100030003", IdApp = "EVA", NumPermiso = 3, Nombre = "Eliminar Usuario" }


        };
            return ListPermisosUA;
        }
    }

    public class PermisosPerfilesApp
    {
        public string IdPermiso { get; set; }
        public string IdApp { get; set; }
        public int NumPermiso { get; set; }
        public string Nombre { get; set; }

        public static List<PermisosPerfilesApp> SEG_PermisosPerfilesApp()
        {
            List<PermisosPerfilesApp> ListPermisosUA = new List<PermisosPerfilesApp>() {
               
                  //ADMIN
                new PermisosPerfilesApp { IdPermiso = "00101001", IdApp = "Admin", NumPermiso = 1, Nombre = "Crear Usuario" },
                new PermisosPerfilesApp { IdPermiso = "00101002", IdApp = "Admin", NumPermiso = 2, Nombre = "Editar Usuario" },
                new PermisosPerfilesApp { IdPermiso = "00101003", IdApp = "Admin", NumPermiso = 3, Nombre = "Eliminar Perfiles" },

                //AUD
                new PermisosPerfilesApp { IdPermiso = "0130110001", IdApp = "AUD", NumPermiso = 1, Nombre = "Crear Perfiles" },
                new PermisosPerfilesApp { IdPermiso = "0130110002", IdApp = "AUD", NumPermiso = 2, Nombre = "Editar Perfiles" },
                new PermisosPerfilesApp { IdPermiso = "0130110003", IdApp = "AUD", NumPermiso = 3, Nombre = "Eliminar Perfiles" },

                  //TIC
                new PermisosPerfilesApp { IdPermiso = "0200050001", IdApp = "TIC", NumPermiso = 1, Nombre = "Crear Perfiles" },
                new PermisosPerfilesApp { IdPermiso = "0200050002", IdApp = "TIC", NumPermiso = 2, Nombre = "Editar Perfiles" },
                new PermisosPerfilesApp { IdPermiso = "0200050003", IdApp = "TIC", NumPermiso = 3, Nombre = "Eliminar Perfiles" },


                //PER                                 
                new PermisosPerfilesApp { IdPermiso = "00500801", IdApp = "PER", NumPermiso = 1, Nombre = "Crear Perfiles" },
                new PermisosPerfilesApp { IdPermiso = "00500802", IdApp = "PER", NumPermiso = 2, Nombre = "Editar Perfiles" },
                new PermisosPerfilesApp { IdPermiso = "00500803", IdApp = "PER", NumPermiso = 3, Nombre = "Eliminar Perfiles" },
                //GAE                                   
                new PermisosPerfilesApp { IdPermiso = "0020060001", IdApp = "GAE", NumPermiso = 1, Nombre = "Crear Perfiles" },
                new PermisosPerfilesApp { IdPermiso = "0020060002", IdApp = "GAE", NumPermiso = 2, Nombre = "Editar Perfiles" },
                new PermisosPerfilesApp { IdPermiso = "0020060003", IdApp = "GAE", NumPermiso = 3, Nombre = "Eliminar Perfiles" },
                //Biblioteca 
                new PermisosPerfilesApp { IdPermiso = "00700701", IdApp = "BIB", NumPermiso = 1, Nombre = "Crear Perfiles" },
                new PermisosPerfilesApp { IdPermiso = "00700702", IdApp = "BIB", NumPermiso = 2, Nombre = "Editar Perfiles" },
                new PermisosPerfilesApp { IdPermiso = "00700703", IdApp = "BIB", NumPermiso = 3, Nombre = "Eliminar Perfiles" },
                //BSD 
                new PermisosPerfilesApp { IdPermiso = "0110190001", IdApp = "BSD", NumPermiso = 1, Nombre = "Agrega Perfiles" },
                new PermisosPerfilesApp { IdPermiso = "0110190002", IdApp = "BSD", NumPermiso = 2, Nombre = "Editar Perfiles" },
                new PermisosPerfilesApp { IdPermiso = "0110190003", IdApp = "BSD", NumPermiso = 3, Nombre = "Eliminar Perfiles" },
                //BIE 
                new PermisosPerfilesApp { IdPermiso = "0210170001", IdApp = "BIE", NumPermiso = 1, Nombre = "Agrega Perfiles" },
                new PermisosPerfilesApp { IdPermiso = "0210170002", IdApp = "BIE", NumPermiso = 2, Nombre = "Editar Perfiles" },
                new PermisosPerfilesApp { IdPermiso = "0210170003", IdApp = "BIE", NumPermiso = 3, Nombre = "Eliminar Perfiles" },
                 //OIRS
                new PermisosPerfilesApp { IdPermiso = "0220120001", IdApp = "OIRS", NumPermiso = 1, Nombre = "Agrega Perfiles" },
                new PermisosPerfilesApp { IdPermiso = "0220120002", IdApp = "OIRS", NumPermiso = 2, Nombre = "Editar Perfiles" },
                new PermisosPerfilesApp { IdPermiso = "0220120003", IdApp = "OIRS", NumPermiso = 3, Nombre = "Eliminar Perfiles" },

                //FORMS
                new PermisosPerfilesApp { IdPermiso = "0080020001", IdApp = "FORMS", NumPermiso = 1, Nombre = "Agrega Perfiles" },
                new PermisosPerfilesApp { IdPermiso = "0080020002", IdApp = "FORMS", NumPermiso = 2, Nombre = "Editar Perfiles" },
                new PermisosPerfilesApp { IdPermiso = "0080020003", IdApp = "FORMS", NumPermiso = 3, Nombre = "Eliminar Perfiles" },

                //EVA
                new PermisosPerfilesApp { IdPermiso = "0100040001", IdApp = "EVA", NumPermiso = 1, Nombre = "Agrega Perfiles" },
                new PermisosPerfilesApp { IdPermiso = "0100040002", IdApp = "EVA", NumPermiso = 2, Nombre = "Editar Perfiles" },
                new PermisosPerfilesApp { IdPermiso = "0100040003", IdApp = "EVA", NumPermiso = 3, Nombre = "Eliminar Perfiles" }


        };
            return ListPermisosUA;
        }
    }
}