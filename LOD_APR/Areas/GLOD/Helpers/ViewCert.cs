namespace LOD_APR.Areas.GLOD.Helpers
{
    using iTextSharp.text.pdf;
    using iTextSharp.text.pdf.security;
    using Org.BouncyCastle.Security;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Security.Cryptography;
    using System.Security.Cryptography.X509Certificates;
    using X509Certificate = Org.BouncyCastle.X509.X509Certificate;

    public class CertSelect
    {
        public bool ValidateUserCert(string IdCert)
        {
            X509Certificate2 certClient = GetUserCert();
            if(certClient!=null)
                return (certClient.SerialNumber.ToUpper() == IdCert.ToUpper());

            return false;
        }
        public bool ValidateUserCert(X509Certificate2 certClient, string IdCert)
        {
            if (certClient != null)
                return (certClient.SerialNumber.ToUpper() == IdCert.ToUpper());

            return false;
        }
        static bool VerifyPassword(byte[] fileContent, string password)
        {
            try
            {
                // ReSharper disable once UnusedVariable
                var certificate = new X509Certificate2(fileContent, password);
            }
            catch (CryptographicException ex)
            {
                if ((ex.HResult & 0xFFFF) == 0x56)
                {
                    return false;
                };

                throw;
            }

            return true;
        }
        public X509Certificate2 GetUserCert()
        {
            //X509Store store = new X509Store("MY", StoreLocation.CurrentUser);
            X509Store store = new X509Store(StoreName.My);
            store.Open(OpenFlags.ReadOnly | OpenFlags.OpenExistingOnly);
            //X509Certificate2 cert = new X509Certificate2(Context.Request.ClientCertificate.Certificate);
            X509Certificate2Collection collection = (X509Certificate2Collection)store.Certificates;
            X509Certificate2Collection fcollection = (X509Certificate2Collection)collection.Find(X509FindType.FindByTimeValid, DateTime.Now, true);

            X509Certificate2Collection scollection = X509Certificate2UI.SelectFromCollection(fcollection, "Selección de certificado Digital", "Seleccione uno de los certificados vigentes que se muestran a continuación", X509SelectionFlag.SingleSelection);
            X509Certificate2 certClient = new X509Certificate2();
            //Console.WriteLine("Number of certificates: {0}{1}", scollection.Count, Environment.NewLine);
            if (scollection.Count > 0)
                certClient = scollection[0];
            
            //foreach (X509Certificate2 x509 in scollection)
            //{
            //    try
            //    {
            //        byte[] rawdata = x509.RawData;
            //        Console.WriteLine("Content Type: {0}{1}", X509Certificate2.GetCertContentType(rawdata), Environment.NewLine);
            //        Console.WriteLine("Friendly Name: {0}{1}", x509.FriendlyName, Environment.NewLine);
            //        Console.WriteLine("Certificate Verified?: {0}{1}", x509.Verify(), Environment.NewLine);
            //        Console.WriteLine("Simple Name: {0}{1}", x509.GetNameInfo(X509NameType.SimpleName, true), Environment.NewLine);
            //        Console.WriteLine("Signature Algorithm: {0}{1}", x509.SignatureAlgorithm.FriendlyName, Environment.NewLine);
            //        Console.WriteLine("Public Key: {0}{1}", x509.PublicKey.Key.ToXmlString(false), Environment.NewLine);
            //        Console.WriteLine("Certificate Archived?: {0}{1}", x509.Archived, Environment.NewLine);
            //        Console.WriteLine("Length of Raw Data: {0}{1}", x509.RawData.Length, Environment.NewLine);
            //        X509Certificate2UI.DisplayCertificate(x509);
            //        x509.Reset();
            //    }
            //    catch (CryptographicException)
            //    {
            //        Console.WriteLine("Information could not be written out for this certificate.");
            //    }
            //}
            store.Close();
            return certClient;
        }
        //public List<string> GetAllUserCert()
        //{
        //    import java.security.KeyStore;
        //    import java.security.cert.X509Certificate;
        //    import java.text.SimpleDateFormat;
        //    import java.util.Enumeration;

        //    try {
        //        KeyStore ks = KeyStore.getInstance("Windows-MY");
        //        ks.load(null, null);
        //        Enumeration<string> al = ks.aliases();
        //        while (al.hasMoreElements())
        //        {
        //            String alias = al.nextElement();
        //            if (ks.containsAlias(alias))
        //            {
        //                System.out.println("Emitido para :" + alias);
        //                X509Certificate cert = (X509Certificate)ks.getCertificate(alias);
        //                System.out.println("Subject:" + cert.getSubjectDN().toString());
        //            }
        //        }

        //        System.out.println("Hello, World");
        //    } catch (Exception e)
        //    {

        //    }
        //}
        public bool PutDigitalSign(X509Certificate2 certClient, string pathUnsigned, string pathSigned, bool visible)
        {
            //X509CertificateParser cp = new X509CertificateParser();

            //USUARIO SELECCIONA CERTIFICADO. UI ES CREADA POR EL SO
            //X509Certificate2 certClient = null;
            //X509Store st = new X509Store(StoreName.My, StoreLocation.CurrentUser);
            //st.Open(OpenFlags.MaxAllowed);
            //X509Certificate2Collection collection = X509Certificate2UI.SelectFromCollection(st.Certificates,
            //    "Please choose certificate:", "", X509SelectionFlag.SingleSelection);
            //if (collection.Count > 0)
            //{
            //    certClient = collection[0];
            //}
            //st.Close();
           
            //OBTENER EL CHAIN DEL CERTIFICADO
            IList<X509Certificate> chain = new List<X509Certificate>();
            X509Chain x509Chain = new X509Chain();
            x509Chain.Build(certClient);

            foreach (X509ChainElement x509ChainElement in x509Chain.ChainElements)
            {
                chain.Add(DotNetUtilities.FromX509Certificate(x509ChainElement.Certificate));
            }

            //string rutaPdfSinFirmar = Server.MapPath("~/Files/pdf/para-firmar.pdf");
            PdfReader inputPdf = new PdfReader(pathUnsigned);

            //string rutaPdfFirmado = Server.MapPath($"~/Files/pdf/firmado-{DateTime.Now.ToString("yyyMMddhhmmss")}.pdf");
            FileStream signedPdf = new FileStream(pathSigned, FileMode.Create);

            PdfStamper pdfStamper = PdfStamper.CreateSignature(inputPdf, signedPdf, '\0');

            IExternalSignature externalSignature = new X509Certificate2Signature(certClient,"SHA-1");

            PdfSignatureAppearance signatureAppearance = pdfStamper.SignatureAppearance;
            //A4 page measures 595 by 800 user units 
            //string rutaFirma = Server.MapPath("~/Files/pdf/firma-ejemplo.jpg");
            //signatureAppearance.SignatureGraphic = iTextSharp.text.Image.GetInstance(rutaFirma);
            //signatureAppearance.SetVisibleSignature(new iTextSharp.text.Rectangle(PageSize.A4_LANDSCAPE.Width - 210, 0, PageSize.A4_LANDSCAPE.Width, 150), inputPdf.NumberOfPages, "Signature");

            //signatureAppearance.SetVisibleSignature(new iTextSharp.text.Rectangle(50, 50, 500, 110), 1, "Signature");
            //signatureAppearance.SetVisibleSignature("Rectangle2");
            //Rectangle2 - Image4
            if(visible)
                signatureAppearance.SetVisibleSignature(new iTextSharp.text.Rectangle(50, 50, 500, 110), 1, "Signature");

            signatureAppearance.SignatureRenderingMode = PdfSignatureAppearance.RenderingMode.DESCRIPTION;

            MakeSignature.SignDetached(signatureAppearance, externalSignature, chain, null, null, null, 0,
                CryptoStandard.CMS);

            inputPdf.Close();
            pdfStamper.Close();
            return true;
            //CertSelect gc = new CertSelect();
            //gc.GetCert();
        }
        public bool PutDigitalSign(IList<X509Certificate> chain, IExternalSignature externalSignature, string pathUnsigned, string pathSigned, bool visible)
        {

            PdfReader inputPdf = new PdfReader(pathUnsigned);
            FileStream signedPdf = new FileStream(pathSigned, FileMode.Create);
            PdfStamper pdfStamper = PdfStamper.CreateSignature(inputPdf, signedPdf, '\0');
            PdfSignatureAppearance signatureAppearance = pdfStamper.SignatureAppearance;

            if (visible)
                signatureAppearance.SetVisibleSignature(new iTextSharp.text.Rectangle(50, 50, 500, 110), 1, "Signature");

            signatureAppearance.SignatureRenderingMode = PdfSignatureAppearance.RenderingMode.DESCRIPTION;

            MakeSignature.SignDetached(signatureAppearance, externalSignature, chain, null, null, null, 0,
                CryptoStandard.CMS);

            inputPdf.Close();
            pdfStamper.Close();
            return true;
        }
    }
}