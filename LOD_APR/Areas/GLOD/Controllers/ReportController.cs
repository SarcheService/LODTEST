using LOD_APR.Areas.Admin.Models;
using LOD_APR.Areas.GLOD.Helpers;
using LOD_APR.Areas.GLOD.Models;
using LOD_APR.Areas.GLOD.ModelsViews;
using LOD_APR.Helpers;
using LOD_APR.Models;
using MessagingToolkit.QRCode.Codec;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using static LOD_APR.Models.AuxiliaresReport;

namespace LOD_APR.Areas.GLOD.Controllers
{
    [CustomAuthorize]
    public class ReportController : Controller
    {
        private LOD_DB db = new LOD_DB();

        public async Task<ActionResult> VistaPreviaAnotacion(int id)
        {

            List<ReportParameter> parametros = new List<ReportParameter>();
          
            LOD_Anotaciones anotacion = await db.LOD_Anotaciones.FindAsync(id);
            AnotacionHelper anothelp = new AnotacionHelper();
            AnotacionView anot = await anothelp.GetAnotacionData(id);
            anot.Remitente.ImgRemitente =(anot.Remitente.ImgRemitente!="")? Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath.TrimEnd('/') + anot.Remitente.ImgRemitente:"";
            anot.QR = QRBinary(id, anotacion.IdLod, anotacion.LOD_LibroObras.CON_Contratos.IdEmpresaContratista.Value);

            List<AnotacionView> lstAnot = new List<AnotacionView>();
            lstAnot.Add(anot);

            List<Remitente> lstRem = new List<Remitente>();
            lstRem.Add(anot.Remitente);

            List<EstadoFirma> lstFirm = new List<EstadoFirma>();
            lstFirm.Add(anot.EstadoFirma);

            List<EstadoAnotacion> lstEstado = new List<EstadoAnotacion>();
            lstEstado.Add(anot.EstadoAnotacion);

            List<ReportDataSource> dsources = new List<ReportDataSource>();
            dsources.Add(new ReportDataSource("DsAnotacion", lstAnot));
            dsources.Add(new ReportDataSource("DSRemitente", lstRem));
            dsources.Add(new ReportDataSource("DSReceptor", anot.Receptores));
            dsources.Add(new ReportDataSource("DSEstadoFirma", lstFirm));
            dsources.Add(new ReportDataSource("DSEstadoAnotacion", lstEstado));
            dsources.Add(new ReportDataSource("DSEstadoRespuesta", new List<EstadoRespuesta>()));
            dsources.Add(new ReportDataSource("DSReferencias", anot.Referencias));
            dsources.Add(new ReportDataSource("DSAdjuntos", anot.Adjuntos));

            return GenerarReporte("PDF", "rptAnotacionLod.rdlc", dsources, parametros, anotacion.LOD_LibroObras.CON_Contratos.IdEmpresaContratista);
        }
        public FileContentResult GenerarReporte(string tipo, string nombreArchivoRpt, List<ReportDataSource> dataSources, List<ReportParameter> parametros, int? IdEmpresa)
        {

            //************DATOS EMPRESA*****
            //
            var empresa = db.MAE_sujetoEconomico.Find(IdEmpresa);
          
            List<DsEmpresa> lstEmpresa = new List<DsEmpresa>();
            DsEmpresa emp = new DsEmpresa();
            emp.Razon = empresa.RazonSocial;
            emp.Direccion = empresa.Direccion;
            emp.Web = empresa.web;
            emp.Telefono = empresa.Telefono;
            if (empresa.RutaImagen != null)
            {
                emp.Logo = Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath.TrimEnd('/') + "/Images/Sujetos/" + empresa.RutaImagen;
            }
            emp.Email = empresa.email;
            lstEmpresa.Add(emp);

            //LISTADO DE VISTAS DEL INFORME
            //PDF
            //Excel
            //Word
            //Image

            LocalReport lr = new LocalReport();
            lr.EnableExternalImages = true;
            lr.EnableHyperlinks = true;
            string path = Path.Combine(Server.MapPath("~/Areas/GLOD/Reports"), nombreArchivoRpt);

            if (System.IO.File.Exists(path))
            {
                lr.ReportPath = path;
            }
            else
            {
                return null;
            }

            //SE AGREGA EL DATASOURCE DE LOS DATOS DE LA EMPRESA
            lr.DataSources.Add(new ReportDataSource("DSEmpresa", lstEmpresa));
            //SE AGREGAN LOS DATASOURCES ESPECIFICOS DEL REPORTE
            foreach (ReportDataSource ds in dataSources)
                lr.DataSources.Add(ds);
            
            //SE AGREGA EL LISTADO DE PARAMETROS ESPECIFICOS POR CADA REPORTE
            if (parametros != null)
                lr.SetParameters(parametros);
                lr.Refresh();
            
            string reportType = tipo;
            string mimeType;
            string encoding;
            string fileNameExtension;
            string deviceInfo ="<DeviceInfo>" +
            "  <OutputFormat>" + tipo + "</OutputFormat>" +
            "  <PageWidth>8.5in</PageWidth>" +
            "  <PageHeight>11in</PageHeight>" +
            "  <MarginTop>0.3937in</MarginTop>" +
            "  <MarginLeft>0.7874in</MarginLeft>" +
            "  <MarginRight>0.7874in</MarginRight>" +
            "  <MarginBottom>0.3937in</MarginBottom>" +
            "</DeviceInfo>";

            Warning[] warnings;
            string[] streams;
            byte[] renderedBytes;
            try
            {
                renderedBytes = lr.Render(
                               reportType,
                               deviceInfo,
                               out mimeType,
                               out encoding,
                               out fileNameExtension,
                               out streams,
                               out warnings);
                return File(renderedBytes, mimeType);
            }
            catch (Exception Ex)
            {
                Console.WriteLine(Ex);
            }
            renderedBytes = lr.Render(
                               reportType,
                               deviceInfo,
                               out mimeType,
                               out encoding,
                               out fileNameExtension,
                               out streams,
                               out warnings);
            return File(renderedBytes, mimeType);
        }
        public byte[] QRBinary(int Folio, int libro, int cont)
        {
            string ruta = Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath.TrimEnd('/');

            QRCodeEncoder qrCodeEncoder = new QRCodeEncoder();
            String encoding = "Byte";
            if (encoding == "Byte")
            {
                qrCodeEncoder.QRCodeEncodeMode = QRCodeEncoder.ENCODE_MODE.BYTE;
            }
            else if (encoding == "AlphaNumeric")
            {
                qrCodeEncoder.QRCodeEncodeMode = QRCodeEncoder.ENCODE_MODE.ALPHA_NUMERIC;
            }
            else if (encoding == "Numeric")
            {
                qrCodeEncoder.QRCodeEncodeMode = QRCodeEncoder.ENCODE_MODE.NUMERIC;
            }
            try
            {
                int scale = Convert.ToInt16(4);
                qrCodeEncoder.QRCodeScale = scale;
            }
            catch (Exception ex)
            {
                //MessageBox.Show("Invalid size!");
                //return;
            }
            try
            {
                int version = Convert.ToInt16(7);
                qrCodeEncoder.QRCodeVersion = version;
            }
            catch (Exception ex)
            {
                //MessageBox.Show("Invalid version !");
            }

            string errorCorrect = "Q";
            if (errorCorrect == "L")
                qrCodeEncoder.QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.L;
            else if (errorCorrect == "M")
                qrCodeEncoder.QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.M;
            else if (errorCorrect == "Q")
                qrCodeEncoder.QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.Q;
            else if (errorCorrect == "H")
                qrCodeEncoder.QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.H;

            Image image;
            System.Drawing.Imaging.ImageFormat format = System.Drawing.Imaging.ImageFormat.Jpeg;
            String data = String.Format("{0}/Public/Folio?folio={1}&ldo={2}&cont?{3}", ruta, Folio, libro, cont);
            image = qrCodeEncoder.Encode(data);

            using (MemoryStream ms = new MemoryStream())
            {
                // Convert Image to byte[]
                image.Save(ms, format);
                byte[] imageBytes = ms.ToArray();

                return imageBytes;
            }


        }
    }
}