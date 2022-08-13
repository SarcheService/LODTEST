using LOD_APR.Areas.Admin.Models;
using LOD_APR.Models;
using System.Linq;

namespace LOD_APR.Helpers
{
    public static class SelectHelper
	{
		private static LOD_DB db = new LOD_DB();
		//public static string getNombreFaena(MAE_personalCargo Cargo)
		//{

		//	if (Cargo == null)
		//		return "-";
		//	if (Cargo.MAE_Faena == null)
		//		return "-";

		//	string lblFanea = string.Empty;
		//	MAE_Faena faena = db.MAE_Faena.Find(Cargo.MAE_Faena.IdFaena);

		//	if (faena.Root != null)
		//	{
		//		string faenaPadre = db.MAE_Faena.Where(f => f.IdFaena == faena.Root).First().Faena;
		//		return faenaPadre + "/" + faena.Faena;
		//	}
		//	else
		//	{
		//		return faena.Faena;
		//	}


		//}
		//public static string getNombreCargo(MAE_personalCargo Cargo)
		//{

		//	if (Cargo == null)
		//		return "-";
		//	if (Cargo.MAE_cargo == null)
		//		return "-";

		//	return Cargo.MAE_cargo.NomCargo;


		//}
		//public static string getNombreFaena(GAE_activoFaena activoFaena)
		//{

		//	if (activoFaena == null)
		//		return "-";
		//	if (activoFaena.MAE_Faena == null)
		//		return "-";

		//	string lblFanea = string.Empty;
		//	MAE_Faena faena = db.MAE_Faena.Find(activoFaena.MAE_Faena.IdFaena);

		//	if (faena.Root != null)
		//	{
		//		string faenaPadre = db.MAE_Faena.Where(f => f.IdFaena == faena.Root).First().Faena;
		//		return faenaPadre + "/" + faena.Faena;
		//	}
		//	else
		//	{
		//		return faena.Faena;
		//	}

		//}
		//public static string getNombreJerarquia(GAE_activoFaena activoFaena)
		//{

		//	if (activoFaena == null)
		//		return "-";
		//	if (activoFaena.MAE_jerarquia == null)
		//		return "-";

		//	string lblJerarquia = string.Empty;
		//	MAE_jerarquia jerarquia = db.MAE_jerarquia.Find(activoFaena.MAE_jerarquia.IdJerarquia);

		//	if (jerarquia.Leaft)
		//	{
		//		string Padre = db.MAE_jerarquia.Where(f => f.IdJerarquia == jerarquia.Padre).First().Nombre;
		//		return jerarquia.Nombre;
		//		//return Padre + "/" + jerarquia.Nombre;
		//	}
		//	else
		//	{
		//		return jerarquia.Nombre;
		//	}


		//}

		//public static string getNombreFaena(int? IdFaena)
		//{

		//	if (IdFaena == null)
		//		return "-";

		//	string lblFanea = string.Empty;
		//	MAE_Faena faena = db.MAE_Faena.Find(IdFaena);

		//	if (faena.Root != null)
		//	{
		//		string faenaPadre = db.MAE_Faena.Where(f => f.IdFaena == faena.Root).First().Faena;
		//		return faenaPadre + "/" + faena.Faena;
		//	}
		//	else
		//	{
		//		return faena.Faena;
		//	}

		//}
		//public static int getFaenaRootId(int? IdFaena)
		//{

		//	if (IdFaena == null)
		//		return 0;

		//	MAE_Faena faena = db.MAE_Faena.Find(IdFaena);

		//	if (faena.Root != null)
		//	{
		//		return db.MAE_Faena.Where(f => f.IdFaena == faena.Root).First().IdFaena;
		//	}
		//	else
		//	{
		//		return faena.IdFaena;
		//	}


		//}
		//public static string getNombreJerarquia(int? IdJerarquia)
		//{

		//	if (IdJerarquia == null)
		//		return "-";

		//	string lblJerarquia = string.Empty;
		//	MAE_jerarquia jerarquia = db.MAE_jerarquia.Find(IdJerarquia);

		//	if (jerarquia.Leaft)
		//	{
		//		string Padre = db.MAE_jerarquia.Where(f => f.IdJerarquia == jerarquia.Padre).First().Nombre;
		//		return jerarquia.Nombre;
		//		//return Padre + "/" + jerarquia.Nombre;
		//	}
		//	else
		//	{
		//		return jerarquia.Nombre;
		//	}


		//}    
    }
	public class FaenaCargoPersonal{

		private static LOD_DB db = new LOD_DB();
		//private MAE_personalCargo mae_Cargo { get; set; }
		public string Faena { get; set; }
		public string Jerarquia { get; set; }
		public string Cargo { get; set; }

		//public FaenaCargoPersonal(MAE_personalCargo cargo) {
		//	mae_Cargo = cargo;
		//	Faena = getNombreFaena();
		//	Cargo = getNombreCargo();
		//	Jerarquia = getNombreJerarquia();
		//}
		
		//private string getNombreFaena()
		//{
		//	if (mae_Cargo == null)
		//		return "-";
		//	if (mae_Cargo.MAE_Faena == null)
		//		return "-";

		//	string lblFanea = string.Empty;
		//	MAE_Faena faena = db.MAE_Faena.Find(mae_Cargo.MAE_Faena.IdFaena);

		//	if (faena.Root != null)
		//	{
		//		string faenaPadre = db.MAE_Faena.Where(f => f.IdFaena == faena.Root).First().Faena;
		//		return faenaPadre + "/" + faena.Faena;
		//	}
		//	else
		//	{
		//		return faena.Faena;
		//	}


		//}
		//private string getNombreCargo()
		//{
		//	if (mae_Cargo == null)
		//		return "-";
		//	if (mae_Cargo.MAE_cargo == null)
		//		return "-";

		//	return mae_Cargo.MAE_cargo.NomCargo;
		//}
		//private string getNombreJerarquia()
		//{

		//	if (mae_Cargo == null)
		//		return "-";

		//	string lblJerarquia = string.Empty;
		//	MAE_jerarquia jerarquia = db.MAE_jerarquia.Find(mae_Cargo.MAE_jerarquia.IdJerarquia);

		//	if (jerarquia.Leaft)
		//	{
		//		string Padre = db.MAE_jerarquia.Where(f => f.IdJerarquia == jerarquia.Padre).First().Nombre;
		//		return jerarquia.Nombre;
		//	}
		//	else
		//	{
		//		return jerarquia.Nombre;
		//	}


		//}

	}
    
    //avisar a Enrique
    public class AutoSearch
    {
        public string id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
    }
}