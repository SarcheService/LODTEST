using LOD_APR.Areas.Admin.Models;
using LOD_APR.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Web;

namespace LOD_APR.Helpers
{
    public class DocumentosHelper
    {
        public bool IsSharepoint { get; set; }
        private LOD_DB db = new LOD_DB();

        public DatosDocumento GetDocumentData(OrigenDocumento _origen, int _primaryKey, int? _idPath, int? _idTipoDoc)
        {
            string path = "~/Files/{0}/{1}/{2}";
            string pre_name = "FileName";
            bool IsSharepoint = false;
            string sharepointPath = string.Empty;
            string Prefijo_SP = string.Empty;
            string pathFolder = PathFolder(_idPath);
            int idContrato = 0;

            switch (_origen)
            {
                case OrigenDocumento.Personal:
                    var per = db.MAE_personal.Find(_primaryKey);
                    pre_name = per.Run.Split('-')[0];
                    path = String.Format(path, "Personal",pre_name.Replace(".",""), pathFolder);
                    break;
                case OrigenDocumento.Sujetos:
                    var suj = db.MAE_sujetoEconomico.Find(_primaryKey);
                    pre_name = suj.Rut.Split('-')[0];
                    path = String.Format(path, "Sujetos", pre_name.Replace(".", ""), pathFolder);
                    break;
                case OrigenDocumento.Proyectos:
                    var proy = db.ASP_proyectos.Find(_primaryKey);
                    pre_name = proy.NombreProyecto;
                    path = String.Format(path, "Proyectos", proy.CodigoProyecto.Replace("/","-"), pathFolder);
                    break;
                case OrigenDocumento.Obras:
                    var obr = db.ASP_obras.Find(_primaryKey);
                    pre_name = obr.CodigoObra.Replace("/", "-");
                    path = String.Format(path, "Obras", obr.CodigoObra.Replace("/", "-"), pathFolder);
                    break;
                case OrigenDocumento.Contratos:
                    var cont = db.ASP_contratos.Find(_primaryKey);
                    pre_name = cont.CodigoContrato.Replace("/", "-");
                    path = String.Format(path, "Contratos", pre_name, pathFolder);
                    Prefijo_SP = GetSharepointRootFolder(_primaryKey);
                    sharepointPath = $"{Prefijo_SP}/{pre_name}/{pathFolder}";
                    idContrato = _primaryKey;
                    if (!String.IsNullOrEmpty(sharepointPath))
                        IsSharepoint = true;
                    break;
                case OrigenDocumento.Bitacoras:
                    var bit = db.ASP_bitacoras.Find(_primaryKey);
                    pre_name = bit.CodigoBitacora.Replace("/", "-");
                    path = String.Format(path, "Bitacoras", pre_name, pathFolder);
                    break;
                case OrigenDocumento.AnotacionLibroObra:
                    var Anot = db.ASP_Anotaciones.Find(_primaryKey);
                    path = String.Format(path, "LibroObra", Anot.ASP_LibroObras.CodigoLObras.Replace("/", "-"), pathFolder);
                    pre_name = "Lod_" + Anot.ASP_LibroObras.IdLibroObra.ToString() + "_Anot_" + Anot.IdAnotacion.ToString();
                    
                    var IdPathLDO = db.ASP_tipoDocumento.Where(d => d.IdTipoDocumento == _idTipoDoc).Select(x=> x.IdPathLod).FirstOrDefault();

                    if (IdPathLDO != null)
                    {
                        _idPath = Convert.ToInt32(IdPathLDO);
                        int IdTipoPath = db.MAE_Path.Where(t => t.IdPath == _idPath).First().IdTipoPath;
                        if (IdTipoPath == 720000)
                        {
                            path = $"~/Files/Contratos/{Anot.ASP_LibroObras.ASP_contratos.CodigoContrato.Replace("/", "-")}/{PathFolder(_idPath)}";
                            Prefijo_SP = GetSharepointRootFolder(Anot.ASP_LibroObras.ASP_contratos.IdContrato);
                            sharepointPath = $"{Prefijo_SP}/{Anot.ASP_LibroObras.ASP_contratos.CodigoContrato.Replace("/", "-")}/{PathFolder(_idPath)}";
                            idContrato = Anot.ASP_LibroObras.ASP_contratos.IdContrato;
                            if (!String.IsNullOrEmpty(sharepointPath))
                                IsSharepoint = true;
                        }
                        else
                        {
                            path = $"~/Files/LibroObra/{Anot.ASP_LibroObras.CodigoLObras.Replace("/", "-")}/{PathFolder(_idPath)}"; 
                        }
                    }
                    break;
                case OrigenDocumento.AnotacionBitacora:
                    var AnotBit = db.ASP_AnotacionesBit.Find(_primaryKey);
                    pre_name = "Bit_" + AnotBit.ASP_bitacoras.IdBitacora.ToString() + "_" + AnotBit.IdAnotacionBit.ToString();
                    path = String.Format(path, "Bitacoras", AnotBit.ASP_bitacoras.CodigoBitacora.Replace("/", "-"), pathFolder);
                    var IdPathBit = db.ASP_tipoDocumento.Where(d => d.IdTipoDocumento == _idTipoDoc).Select(x => x.IdPathBit).FirstOrDefault();

                    if (IdPathBit != null)
                    {
                        _idPath = Convert.ToInt32(IdPathBit);
                        path = $"~/Files/Bitacoras/{AnotBit.ASP_bitacoras.CodigoBitacora.Replace("/", "-")}/{PathFolder(_idPath)}";
                    }

                    break;
                case OrigenDocumento.CaratulaEstadoPago:
                    var ep = db.ASP_epCaratulaContrato.Find(_primaryKey);
                    pre_name = "EP_" + ep.NumeroEP.ToString() + "_Cont_" + ep.ASP_contratos.CodigoContrato.Replace("/", "-");
                    string carpetaEP = $"EP N°{ep.NumeroEP}";

                    int pathTransitorio = 720000;
                    if (ep.ASP_contratos.ASP_tipoContrato.IdPath != null)
                        pathTransitorio = Convert.ToInt32(ep.ASP_contratos.ASP_tipoContrato.IdPath);

                    var mae_path = db.MAE_Path.Where(x => x.IdTipoPath == 720000 && x.Padre == pathTransitorio && x.Path == carpetaEP).FirstOrDefault();
                    if (mae_path==null)
                    {
                        _idPath = CreateNewPath(carpetaEP, pathTransitorio, 720000, false);
                    }
                    else
                    {
                        _idPath = mae_path.IdPath;
                    }

                    path = $"~/Files/Contratos/{ep.ASP_contratos.CodigoContrato.Replace("/", "-")}/{PathFolder(_idPath)}";
                    Prefijo_SP = GetSharepointRootFolder(ep.ASP_contratos.IdContrato);
                    sharepointPath = $"{Prefijo_SP}/{ep.ASP_contratos.CodigoContrato.Replace("/", "-")}/{PathFolder(_idPath)}";
                    idContrato = ep.ASP_contratos.IdContrato;
                    if (!String.IsNullOrEmpty(sharepointPath))
                        IsSharepoint = true;

                    break;
            }

            DatosDocumento data = new DatosDocumento()
            {
                Origen = (Int16)_origen,
                Path = path,
                ServerPath = HttpContext.Current.Server.MapPath(path),
                DataBasePath = DataBasePath(path),
                PreName = pre_name,
                IdPath = _idPath,
                IdContrato = idContrato,
                IsSharepoint = (this.IsSharepoint) ? IsSharepoint : false,
                Prefijo_SP = (this.IsSharepoint)?Prefijo_SP:null,
                SharepointPath = (this.IsSharepoint) ? sharepointPath : null,
            };

            return data;
            
        }
        public Status_Error SaveDocumentData(OrigenDocumento _origen, int _primaryKey, int _idDocument, int? _idTipoDoc)
        {
            Status_Error result = new Status_Error() { Error = false };
            try
            {
                switch (_origen)
                {
                    case OrigenDocumento.Personal:
                        MAE_docPersonal docPer = new MAE_docPersonal() { IdPersonal = Convert.ToInt32(_primaryKey), IdDoc = _idDocument };
                        db.MAE_docPersonal.Add(docPer);
                        db.SaveChanges();
                        break;
                    case OrigenDocumento.Sujetos:
                        MAE_docSujeto docSuj = new MAE_docSujeto() { IdSujEcon = Convert.ToInt32(_primaryKey), IdDoc = _idDocument };
                        db.MAE_docSujeto.Add(docSuj);
                        db.SaveChanges();
                        break;
                    case OrigenDocumento.Proyectos:
                        MAE_docProyectos docProy = new MAE_docProyectos() { IdProyecto = Convert.ToInt32(_primaryKey), IdDoc = _idDocument };
                        db.MAE_docProyectos.Add(docProy);
                        db.SaveChanges();
                        break;
                    case OrigenDocumento.Obras:
                        MAE_docObras docObra = new MAE_docObras() { IdObra = Convert.ToInt32(_primaryKey), IdDoc = _idDocument };
                        db.MAE_docObras.Add(docObra);
                        db.SaveChanges();
                        break;
                    case OrigenDocumento.Contratos:
                        MAE_docContratos docCont = new MAE_docContratos() { IdContrato = Convert.ToInt32(_primaryKey), IdDoc = _idDocument };
                        db.MAE_docContratos.Add(docCont);
                        db.SaveChanges();
                        break;
                    case OrigenDocumento.Bitacoras:
                        MAE_docBitacoras docBit = new MAE_docBitacoras() { IdBitacora = Convert.ToInt32(_primaryKey), IdDoc = _idDocument };
                        db.MAE_docBitacoras.Add(docBit);
                        db.SaveChanges();
                        break;
                    case OrigenDocumento.AnotacionLibroObra:
                        MAE_docAnotacionesLod docAnotDoc = new MAE_docAnotacionesLod()
                        {
                            IdAnotacion = Convert.ToInt32(_primaryKey),
                            IdDoc = _idDocument,
                            IdTipoDocumento = _idTipoDoc

                        };
                        db.MAE_docAnotacionesLod.Add(docAnotDoc);

                        int IdLib = db.ASP_Anotaciones.Where(x => x.IdAnotacion == _primaryKey).First().IdLibroObra;
                        int IdCont = db.ASP_LibroObras.Where(x => x.IdLibroObra == IdLib).First().IdContrato;
                        //FunctionLog(DateTime.Now, mAE_documentos.UserId, 5, docAnotDoc.IdAnotacion, "Subio el archivo :" + mAE_documentos.NombreDoc, "", null, null);

                        MAE_docContratos docContAnotLDO = new MAE_docContratos() { IdContrato = Convert.ToInt32(IdCont), IdDoc = _idDocument };
                        db.MAE_docContratos.Add(docContAnotLDO);
                        db.SaveChanges();
                        break;
                    case OrigenDocumento.AnotacionBitacora:
                        MAE_docAnotacionesBit docAnotDocBit = new MAE_docAnotacionesBit()
                        {
                            IdAnotacionBit = Convert.ToInt32(_primaryKey),
                            IdDoc = _idDocument,
                            IdTipoDocumento = _idTipoDoc

                        };
                        db.MAE_docAnotacionesBit.Add(docAnotDocBit);
                        db.SaveChanges();
                        //FunctionLog(DateTime.Now, mAE_documentos.UserId, 6, docAnotDoc.IdAnotacionBit, "Subio el archivo :" + mAE_documentos.NombreDoc, "", null, null);
                        break;
                    case OrigenDocumento.CaratulaEstadoPago:
                        var userId = (HttpContext.Current.User.Identity as ClaimsIdentity).Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);

                        ASP_docEP docEp = new ASP_docEP()
                        {
                            IdDoc = _idDocument,
                            IdCaratulaEP = Convert.ToInt32(_primaryKey),
                            IdDocReqEp = Convert.ToInt32(_idTipoDoc),
                            UserCarga = userId.Value,
                            FechaCarga = DateTime.Now,
                            Estado = 1,
                            NumRechazos = 0
                        };
                        db.ASP_docEP.Add(docEp);
                        db.SaveChanges();
                        //Aviso Documento Cargados**********************************************************************
                        //await NotificacionesDocEp.AvisoCargaDocCompletos(Convert.ToInt32(mAE_documentos.IdCaratulaEp));
                        //***********************************************************************************************
                        //FunctionLog(DateTime.Now, mAE_documentos.UserId, 6, docAnotDoc.IdAnotacionBit, "Subio el archivo :" + mAE_documentos.NombreDoc, "", null, null);
                        break;
                }
            }
            catch(Exception ex)
            {
                result.Error = true;
                result.ErrorMnsg = ex.Message;
            }
           
            return result;

        }
        private int CreateNewPath(string newpath, int Padre, int IdTipoPath, bool Visible)
        {
            try
            {
                MAE_Path newPath = new MAE_Path()
                {
                    Path = newpath,
                    IdTipoPath = IdTipoPath,
                    Padre = Padre,
                    VisibilidadTree = Visible
                };
                db.MAE_Path.Add(newPath);
                db.SaveChanges();
                return newPath.IdPath;
            }
            catch (Exception ex)
            {
                return 0;
            }

        }
        public string GetDocumentDeletePath(OrigenDocumento _origen, string _path, int? _idPath)
        {
            string path = "~/Files/{0}/" + _path;
            
            switch (_origen)
            {
                case OrigenDocumento.Personal:
                    path = String.Format(path, "Personal");
                    break;
                case OrigenDocumento.Sujetos:
                    path = String.Format(path, "Sujetos");
                    break;
                case OrigenDocumento.Proyectos:
                    path = String.Format(path, "Proyectos");
                    break;
                case OrigenDocumento.Obras:
                    path = String.Format(path, "Obras");
                    break;
                case OrigenDocumento.Contratos:
                    path = String.Format(path, "Contratos");
                    break;
                case OrigenDocumento.Bitacoras:
                    path = String.Format(path, "Bitacoras");
                    break;
                case OrigenDocumento.AnotacionLibroObra:
                    var pathLDO = db.MAE_Path.Where(t => t.IdPath == _idPath).FirstOrDefault();
                    if (pathLDO != null)
                    {
                        if (pathLDO.IdTipoPath == 720000)
                        {
                            path = String.Format(path, "Contratos");
                        }
                        else
                        {
                            path = String.Format(path, "LibroObra");
                        }
                    }
                    else
                    {
                        path = String.Format(path, "LibroObra");
                    }
                    break;
                case OrigenDocumento.AnotacionBitacora:
                    path = String.Format(path, "Bitacoras");
                    break;
                case OrigenDocumento.CaratulaEstadoPago:
                    path = String.Format(path, "Contratos");
                    break;
            }
           
            return path;

        }
        private string DataBasePath(string _path)
        {
            string[] palabras = Array.ConvertAll(_path.Split('/'), p => p.Trim());
            string[] PalabrasLimpias = palabras.Where(p => p != "/").ToArray();
            List<string> final = new List<string>(PalabrasLimpias);
            final.RemoveRange(0, 3);
            return String.Join("/", final.ToArray());

        }
        public string PathFolder(int? IdPath, string PFolder = null)
        {
            var folder = db.MAE_Path.Find(IdPath);
            
            if (folder != null)
            {
                if(folder.GuardarSharepoint)
                    this.IsSharepoint = true;

                string Path = folder.Path + "/" + PFolder;
                PFolder = PathFolder(folder.Padre, Path);
            }
           
            return PFolder;
        }
        public string GetSharepointRootFolder(int IdContrato)
        {
            var contrato = db.ASP_contratos.Where(c => c.IdContrato == IdContrato).Include(e => e.MAE_Empresa).Include(o => o.ASP_obras).FirstOrDefault();
            string SP_Path = string.Empty;

            if (!String.IsNullOrEmpty(contrato.MAE_Empresa.SharepointUrl))
            {
                if (contrato.ASP_obras == null)
                {
                    SP_Path = SP_Path + $"{contrato.MAE_Empresa.SPDefault}";
                }
                else
                {
                    SP_Path = SP_Path + $"{(contrato.ASP_obras.MAE_Faena == null ? "[PROYECTOS REGIONALES]" : contrato.ASP_obras.MAE_Faena.MAE_ciudad.Ciudad.ToUpper())}/{contrato.ASP_obras.ASP_tipoObra.AbrevCodif}/{contrato.ASP_obras.ASP_tipoObra.NombreTipoObra}";
                }
            }

            return SP_Path;

        }
        public string GetSharepointLibraryID(int IdContrato)
        {
            var contrato = db.ASP_contratos.Where(c => c.IdContrato == IdContrato).Include(e => e.MAE_Empresa).Include(o => o.ASP_obras).FirstOrDefault();
            string SP_Path = string.Empty;

            if (!String.IsNullOrEmpty(contrato.MAE_Empresa.SharepointUrl))
                SP_Path = contrato.MAE_Empresa.SPLibrary;

            return SP_Path;
        }
        public Status_Error SaveDirectory(string path)
        {
            Status_Error result = new Status_Error();
            if (!Directory.Exists(path))
                try
                {
                    Directory.CreateDirectory(path);
                    result.Error = false;
                }
                catch (Exception ex)
                {
                    result.Error=true;
                    result.ErrorMnsg = ex.Message;
                }

            return result;
        }

    }
    public class DatosDocumento
    {
        public int Origen { get; set; }
        public string Path { get; set; }
        public string ServerPath { get; set; }
        public string DataBasePath { get; set; }
        public string SharepointPath { get; set; }
        public string PreName { get; set; }
        public int? IdPath { get; set; }
        public bool IsSharepoint { get; set; }
        public int IdContrato { get; set; }
        public string Prefijo_SP { get; set; }
    }
    public enum OrigenDocumento: Int16
    {
        Personal = 0,
        Sujetos = 1,
        Proyectos = 2,
        Obras = 3,
        Contratos = 4,
        Bitacoras = 5,
        AnotacionLibroObra = 6,
        AnotacionBitacora = 7,
        CaratulaEstadoPago = 8
    }

}