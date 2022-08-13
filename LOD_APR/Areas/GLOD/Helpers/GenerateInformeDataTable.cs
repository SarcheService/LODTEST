using LOD_APR.Areas.GLOD.Models;
using LOD_APR.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace LOD_APR.Areas.GLOD.Helpers
{
    public class InformeToDataTable
    {
        private LOD_DB db = new LOD_DB();

        public async Task<DataTable> GenerateDataTable(FORM_InformesItems item)
        {
            var items = await db.FORM_InformesItems.Where(i => i.IdForm == item.IdForm && i.FORM_Informes.IdContrato == item.FORM_Informes.IdContrato).ToListAsync();

            DataTable dt = new DataTable(item.Titulo);
            dt.Columns.Add(new DataColumn("RUT Mandante"));
            dt.Columns.Add(new DataColumn("Mandante"));
            dt.Columns.Add(new DataColumn("RUT Contratista"));
            dt.Columns.Add(new DataColumn("Contratista"));
            dt.Columns.Add(new DataColumn("Cod. Contrato"));
            dt.Columns.Add(new DataColumn("Mes Informado."));
            dt.Columns.Add(new DataColumn("Periodo Informe"));
            dt.Columns.Add(new DataColumn("Folio Informe"));
            dt.Columns.Add(new DataColumn("Ingresado el"));
            dt.Columns.Add(new DataColumn("Ingresado Por"));
            string[] datosBase = new string[10]{
                item.FORM_Informes.CON_Contratos.MAE_Sucursal.MAE_sujetoEconomico.Rut,
                item.FORM_Informes.CON_Contratos.MAE_Sucursal.Sucursal,
                item.FORM_Informes.CON_Contratos.Empresa_Contratista.Rut,
                item.FORM_Informes.CON_Contratos.Empresa_Contratista.RazonSocial,
                item.FORM_Informes.CON_Contratos.CodigoContrato,
                item.FORM_Informes.MesInformado,
                item.FORM_Informes.Mes + "-" + item.FORM_Informes.Anio,
                item.FORM_Informes.IdEnvio.ToString().PadLeft(6,'0'),
                item.FORM_Informes.FechaCreacion.ToShortDateString(),
                item.FORM_Informes.Usuario
             };

            foreach (var p in item.FORM_InformesItemsData.OrderBy(o => o.IdCampo))
            {
                DataColumn col = new DataColumn(p.Pregunta);
                col.DataType = System.Type.GetType(p.DataType);
                dt.Columns.Add(col);
            }

            foreach (var i in items)
            {
                DataRow row = dt.NewRow();
                int index = 10;
                for (int ind = 0; ind < index; ind++)
                {
                    row[ind] = datosBase[ind];
                }
               
                foreach (var p in i.FORM_InformesItemsData.OrderBy(o => o.IdCampo))
                {
                    //PARSEAR SEGÚN EL TIPO DE DATO DE LA COLUMNA
                    if (p.FORM_FormPreguntas.TipoParam == 3) // "System.Int32
                    {
                        row[index] = Convert.ToInt32(p.Respuesta.Replace(".", ""));
                    }
                    else if (p.FORM_FormPreguntas.TipoParam == 4)//"System.Decimal"
                    {
                        row[index] = Convert.ToDecimal(p.Respuesta.Replace(".", ""));
                    }
                    else if (p.FORM_FormPreguntas.TipoParam == 9 || p.FORM_FormPreguntas.TipoParam == 902)//"System.DateTime"
                    {
                        row[index] = Convert.ToDateTime(p.Respuesta);
                    }
                    else //"System.String"
                    {
                        row[index] = p.Respuesta;
                    }
                    index++;
                }

                dt.Rows.Add(row);
            }

            return dt;
        }
        public async Task<DataTable> GenerateDataTableIncidentes(FORM_InformesItems item)
        {
            var items = await db.FORM_InformesItems.Where(i => i.IdForm == item.IdForm && i.IdContrato == item.IdContrato).ToListAsync();

            DataTable dt = new DataTable(item.Titulo);
            dt.Columns.Add(new DataColumn("RUT Mandante"));
            dt.Columns.Add(new DataColumn("Mandante"));
            dt.Columns.Add(new DataColumn("RUT Contratista"));
            dt.Columns.Add(new DataColumn("Contratista"));
            dt.Columns.Add(new DataColumn("Cod. Contrato"));
            dt.Columns.Add(new DataColumn("Ingresado el"));
            dt.Columns.Add(new DataColumn("Ingresado Por"));
            dt.Columns.Add(new DataColumn("Folio Informe"));
            string[] datosBase = new string[7]{
                item.CON_Contratos.MAE_Sucursal.MAE_sujetoEconomico.Rut,
                item.CON_Contratos.MAE_Sucursal.Sucursal,
                item.CON_Contratos.Empresa_Contratista.Rut,
                item.CON_Contratos.Empresa_Contratista.RazonSocial,
                item.CON_Contratos.CodigoContrato,
                item.FechaDespacho.Value.ToShortDateString(),
                item.Usuario
            };

            foreach (var p in item.FORM_InformesItemsData.OrderBy(o => o.IdCampo))
            {
                DataColumn col = new DataColumn(p.Pregunta);
                col.DataType = System.Type.GetType(p.DataType);
                dt.Columns.Add(col);
            }

            foreach (var i in items)
            {
                DataRow row = dt.NewRow();
                int index = 7;
                for (int ind = 0; ind < index; ind++)
                {
                    row[ind] = datosBase[ind];
                }
                row[7] = i.IdItem.ToString().PadLeft(6, '0');
                index = 8;

                foreach (var p in i.FORM_InformesItemsData.OrderBy(o => o.IdCampo))
                {
                    //PARSEAR SEGÚN EL TIPO DE DATO DE LA COLUMNA
                    if (p.FORM_FormPreguntas.TipoParam == 3) // "System.Int32
                    {
                        row[index] = Convert.ToInt32(p.Respuesta.Replace(".", ""));
                    }
                    else if (p.FORM_FormPreguntas.TipoParam == 4)//"System.Decimal"
                    {
                        row[index] = Convert.ToDecimal(p.Respuesta.Replace(".", ""));
                    }
                    else if (p.FORM_FormPreguntas.TipoParam == 9 || p.FORM_FormPreguntas.TipoParam == 902)//"System.DateTime"
                    {
                        row[index] = Convert.ToDateTime(p.Respuesta);
                    }
                    else //"System.String"
                    {
                        row[index] = p.Respuesta;
                    }
                    index++;
                }

                dt.Rows.Add(row);
            }

            return dt;
        }
    }
}