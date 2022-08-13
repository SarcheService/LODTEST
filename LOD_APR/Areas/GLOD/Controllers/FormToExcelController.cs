using ClosedXML.Excel;
using LOD_APR.Areas.GLOD.Helpers;
using LOD_APR.Helpers;
using LOD_APR.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace LOD_APR.Areas.GLOD.Controllers
{
    [CustomAuthorize]
    public class FormToExcelController : Controller
    {
        private LOD_DB db = new LOD_DB();

        public async Task<FileResult> ITemToExcel(int id)
        {
            var item = await db.FORM_InformesItems.FindAsync(id);
            if (item == null)
                return null;

            string fileName = $"Informe_Ejecutivo_Contrato_{item.FORM_Informes.CON_Contratos.CodigoContrato.Replace(" ", "_")}_{item.Titulo.Replace(" ", "_")}_{item.FORM_Informes.MesInformado.Replace(" ", "_")}.xlsx";

            InformeToDataTable infToDt = new InformeToDataTable();
            DataTable dt = await infToDt.GenerateDataTable(item);

            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(dt);
                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
                }
            }
        }
        public async Task<FileResult> ITemIncidenteToExcel(int id)
        {
            var item = await db.FORM_InformesItems.FindAsync(id);
            if (item == null)
                return null;

            string fileName = $"Informe_Incidentes_Contrato_{item.CON_Contratos.CodigoContrato.Replace(" ","_")}_{item.Titulo.Replace(" ", "_")}.xlsx";

            InformeToDataTable infToDt = new InformeToDataTable();
            DataTable dt = await infToDt.GenerateDataTableIncidentes(item);

            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(dt);
                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
                }
            }
        }
        private DataTable GetTable<T>(string DtName, IEnumerable<T> list, params Expression<Func<T, object>>[] fxns)
        {
            DataTable dt = new DataTable(DtName);

            foreach (var fxn in fxns)
            {
                dt.Columns.Add(new DataColumn(GetName(fxn)));
            }
            foreach (var item in list)
            {
                List<object> columnData = new List<object>();
                foreach (var fxn in fxns)
                {
                    try
                    {
                        columnData.Add(fxn.Compile()(item));
                    }
                    catch (Exception ex)
                    {

                        columnData.Add("-");
                    }
                }
                dt.Rows.Add(columnData.ToArray());
            }

            return dt;
        }
        static string GetName<T>(Expression<Func<T, object>> expr)
        {
            var member = expr.Body as MemberExpression;
            if (member != null)
                return GetName2(member);

            var unary = expr.Body as UnaryExpression;
            if (unary != null)
                return GetName2((MemberExpression)unary.Operand);

            return "?+?";
        }
        static string GetName2(MemberExpression member)
        {
            var fieldInfo = member.Member as FieldInfo;
            if (fieldInfo != null)
            {
                var d = fieldInfo.GetCustomAttribute(typeof(DescriptionAttribute)) as DescriptionAttribute;
                if (d != null) return d.Description;
                return fieldInfo.Name;
            }

            var propertInfo = member.Member as PropertyInfo;
            if (propertInfo != null)
            {
                if (propertInfo.GetCustomAttribute(typeof(DisplayNameAttribute), true) is DisplayNameAttribute d) return d.DisplayName;
                return propertInfo.Name;
            }

            return "?-?";
        }
    }
}
