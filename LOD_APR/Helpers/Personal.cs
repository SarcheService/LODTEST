using LOD_APR.Areas.Admin.Models;
using LOD_APR.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LOD_APR.Helpers
{
	
	public class HelpPersonal
	{
		private LOD_DB db = new LOD_DB();
		public List<MAE_personalCargo> ListadoCargoPersonal(int IdPersonal)
		{
			//SelectHelper select = new SelectHelper();
			var MAE_personalCargo_List = db.MAE_personalCargo.Where(p => p.IdPersonal == IdPersonal).OrderByDescending(o => o.FechaIngreso).OrderByDescending(o => o.Activo).ToList();
			foreach (MAE_personalCargo cargo in MAE_personalCargo_List)
			{
				if (cargo.IdFaena != null)
				{
					cargo.MAE_Faena.Faena = SelectHelper.getNombreFaena(cargo.IdFaena);
				}
			}
			return MAE_personalCargo_List.ToList<MAE_personalCargo>();
		}
		public Cargo_Personal Cargo_Personal(int IdPersonal) {

			//SelectHelper select = new SelectHelper();
			Cargo_Personal cargo = new Cargo_Personal();
			MAE_personalCargo cargoActivo = db.MAE_personalCargo.Where(p => p.IdPersonal == IdPersonal && p.Activo == true).FirstOrDefault();

			if (cargoActivo != null )
			{
				if (cargoActivo.MAE_cargo != null)
				{
					cargo.Cargo = cargoActivo.MAE_cargo.NomCargo;
					if (cargoActivo.IdCargo != null)
					{
						cargo.IdCargo = Convert.ToInt32(cargoActivo.IdCargo);
					}
					else
					{
						cargo.IdCargo = 0;
					}
				}

				if (cargoActivo.IdFaena != null && cargoActivo.IdFaena != 0)
				{
					cargo.Faena = SelectHelper.getNombreFaena(cargoActivo.IdFaena);
					cargo.IdFaena = cargoActivo.IdFaena;
				}
				else
				{
					cargo.Faena = "-";
					cargo.IdFaena = null;
				}

				if (cargoActivo.IdJerarquia != null && cargoActivo.IdJerarquia != 0)
				{
					cargo.Unidad = SelectHelper.getNombreJerarquia(cargoActivo.IdJerarquia);
					cargo.IdUnidad = cargoActivo.IdJerarquia;
				}
				else
				{
					cargo.Unidad = "-";
					cargo.IdUnidad = null;
				}
				
			}
			else
			{
				cargo.Faena = "-";
				cargo.IdFaena = null;
				cargo.Unidad = "-";
				cargo.IdUnidad = null;
			}

			return cargo;

		}
	}
}