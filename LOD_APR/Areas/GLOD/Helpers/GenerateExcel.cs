using ClosedXML.Excel;
using LOD_APR.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace LOD_APR.Areas.GLOD.Helpers
{
    public static class ACA_ExportarExcelWeb
    {

        public static string ExportarExcelOnMemory(DataTable dt, string nombreArchivo, string rutaGuardado)
        {

            if (dt == null || dt.Columns.Count == 0)
                return string.Empty;

            try
            {
                var wb = new XLWorkbook();
                wb.Worksheets.Add(dt);

                HttpResponse response = HttpContext.Current.Response;
                response.Clear();
                response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                response.AddHeader("content-disposition", "attachment;filename=" + nombreArchivo);

                FileStream file;

                using (MemoryStream MyMemoryStream = new MemoryStream())
                {
                    wb.SaveAs(MyMemoryStream);
                    file = new FileStream(HttpContext.Current.Server.MapPath("~/" + rutaGuardado + nombreArchivo), FileMode.Create, FileAccess.Write);
                    MyMemoryStream.WriteTo(file);

                    dt.Clear();
                    dt.Dispose();
                    file.Flush();
                    file.Close();
                    file.Dispose();
                    MyMemoryStream.Flush();
                    MyMemoryStream.Close();
                    MyMemoryStream.Dispose();
                }
                wb.Dispose();
                GC.Collect();

                return rutaGuardado + nombreArchivo;
            }
            catch {
                return string.Empty;
            }
          
        }

    }
}