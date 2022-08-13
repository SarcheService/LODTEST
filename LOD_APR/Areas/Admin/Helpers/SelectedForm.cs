using LOD_APR.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace LOD_APR.Areas.Admin.Helpers
{
    public class SelectedForm
    {
        private LOD_DB db = new LOD_DB();


        public SelectList GetSelectedForm(string IdForm)
        {
            SelectList s = new SelectList(db.FORM_Formularios.Where(f => f.Embebido).Select(m=> new { m.IdForm, m.Titulo }).ToList(), "IdForm", "Titulo", IdForm);
            return s;
        }
        public List<SelectListItem> GetSelectedFormItems(string IdForm)
        {
            List<SelectListItem> list = new List<SelectListItem>();
            var forms = db.FORM_Formularios.Where(f => f.Embebido).Select(m => new { m.IdForm, m.Titulo }).ToList();
            foreach(var f in forms)
            {
                SelectListItem i = new SelectListItem()
                {
                    Value = f.IdForm,
                    Text = f.Titulo,
                    Selected = (f.IdForm == IdForm) ? true : false
                };
                list.Add(i);
            }
            return list;
        }
    }
}