using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects.DataClasses;
using System.Linq;
using System.Web;

namespace LOD_APR.Helpers
{

	public static class CheckDelete
	{

		public static bool HasRelation(object entity)
		{
			var allrelatedEnds = ((IEntityWithRelationships)entity).RelationshipManager.GetAllRelatedEnds();
			bool hasRelation = false;
			foreach (var relatedEnd in allrelatedEnds)
			{
				if (relatedEnd.GetEnumerator().MoveNext())
				{
					hasRelation = true;
					break;
				}
			}

			return hasRelation;
			//if (!hasRelation)
			//{
			//	//Delete
			//}


			//var propertiesList = entity.GetType().GetProperties();
			//return (from prop in propertiesList where prop.PropertyType.IsGenericType select prop.GetValue(entity) into propValue select propValue as IList).All(propList => propList == null || propList.Count <= 0);
		}
	}
}