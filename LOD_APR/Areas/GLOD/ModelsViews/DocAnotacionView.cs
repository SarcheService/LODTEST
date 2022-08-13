using LOD_APR.Areas.Admin.Models;
using LOD_APR.Areas.GLOD.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LOD_APR.Areas.GLOD.ModelsViews
{
    public class DocAnotacionView
    {
        public LOD_Anotaciones lOD_Anotacion = new LOD_Anotaciones();
        public List<MAE_CodSubCom> list_Cod = new List<MAE_CodSubCom>();
        public List<LOD_docAnotacion> list_doc = new List<LOD_docAnotacion>();
    }

}