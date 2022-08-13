using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Web;
using System.DirectoryServices;

namespace LOD_APR.Helpers
{
	public class clsSession
	{
		public void set(string key, object value)
		{
			HttpContext.Current.Session.Add(key, value);
		}
		public object get(string key)
		{
			object value = new object();

			try
			{
				value = HttpContext.Current.Session[key];
			}
			catch
			{
				value = null;
			}

			return value;

		}
		public bool del(string key)
		{
			bool isDel = false;

			try
			{
				HttpContext.Current.Session.Remove(key);
				isDel = true;
			}
			catch
			{
				isDel = false;
			}

			return isDel;

		}
		public bool clear()
		{
			bool isDel = false;

			try
			{
				HttpContext.Current.Session.RemoveAll();
				isDel = true;
			}
			catch
			{
				isDel = false;
			}

			return isDel;
		}
		public bool find(string key)
		{
			bool isKey = false;

			foreach (string k in HttpContext.Current.Session.Keys)
			{
				if (k == key)
				{
					isKey = true;
				}
			}

			return isKey;

		}
		public List<string> list()
		{
			NameObjectCollectionBase.KeysCollection list;
			list = HttpContext.Current.Session.Keys;

			return list.Cast<string>().ToList<string>();

		}
        public int getSessionTime()
        {
            return HttpContext.Current.Session.Timeout;
        }
        public void logout()
		{
			HttpContext.Current.Session.Clear();
			HttpContext.Current.Session.Abandon();
		}

        #region "ACTIVE DIRECTORY"
        //VERIFICAMOS SI EL USUARIO EXISTE
        public bool existeUsuario(string usuario, string clave)
        {

            bool existe = false;
            string ipServer = ConfigurationManager.AppSettings["ipServer"];
            string[] dom = ConfigurationManager.AppSettings["dominio"].Split('.');
            string path = "";
            path = "LDAP://" + ipServer + "/" + "DC=" + dom[0] + ",DC=" + dom[1];
            // Dominio y usuario
            string dominiousuario = ipServer + @"\" + usuario;
            // Creamos un objeto DirectoryEntry al cual le pasamos el URL, dominio\usuario y contraseña
            DirectoryEntry entry = new DirectoryEntry(path, dominiousuario, clave);

            try
            {
                DirectorySearcher buscar = new DirectorySearcher(entry);

                buscar.Filter = "(SAMAccountName=" + usuario + ")";

                buscar.PropertiesToLoad.Add("cn");

                // Verificamos existan los datos ingresados
                SearchResult result = buscar.FindOne();

                if (result == null)
                {
                    existe = false;
                }
                else
                {
                    existe = true;
                }
            }
            catch (Exception e)
            {
                existe = false;
            }

            return existe;
        }
        //BUSCAMOS LOS GRUPOS QUE EL USUARIO TIENE CONFIGURADOS
        //public ArrayList userGroups(string usuario, string clave)
        //{
        //    ArrayList grupos = new ArrayList();

        //    string ipServer = ConfigurationManager.AppSettings["ipServer"];
        //    string[] dom = ConfigurationManager.AppSettings["dominio"].Split('.');
        //    string path = "";
        //    path = "LDAP://" + ipServer + "/" + "DC=" + dom[0] + ",DC=" + dom[1];

        //    // Dominio y usuario
        //    string dominiousuario = ipServer + @"\" + usuario;

        //    // Creamos un objeto DirectoryEntry al cual le pasamos el URL, dominio\usuario y contraseña
        //    DirectoryEntry entry = new DirectoryEntry(path, dominiousuario, clave);

        //    try
        //    {
        //        DirectorySearcher buscar = new DirectorySearcher(entry);

        //        buscar.Filter = "(SAMAccountName=" + usuario + ")";
        //        buscar.PropertiesToLoad.Add("memberOf");

        //        // Verificamos existan los datos ingresados
        //        SearchResult result = buscar.FindOne();

        //        int propertyCount = result.Properties["memberOf"].Count;
        //        String dn;
        //        int equalsIndex, commaIndex;

        //        for (int propertyCounter = 0; propertyCounter < propertyCount;
        //            propertyCounter++)
        //        {
        //            dn = (String)result.Properties["memberOf"][propertyCounter];

        //            equalsIndex = dn.IndexOf("=", 1);
        //            commaIndex = dn.IndexOf(",", 1);
        //            if (-1 == equalsIndex)
        //            {
        //                return grupos;
        //            }
        //            grupos.Add(dn.Substring((equalsIndex + 1), (commaIndex - equalsIndex) - 1));
        //        }

        //    }
        //    catch (Exception e)
        //    {

        //    }


        //    return grupos;

        //}

        //public List<string> usuariosEnGrupo(string grupo, string usuario, string clave)
        //{
        //    List<string> usuarios = new List<string>();

        //    string ipServer = ConfigurationManager.AppSettings["ipServer"];
        //    string[] dom = ConfigurationManager.AppSettings["dominio"].Split('.');
        //    string path = "";
        //    path = "LDAP://" + ipServer + "/" + "DC=" + dom[0] + ",DC=" + dom[1];
        //    DirectoryEntry entry = new DirectoryEntry(path, usuario, clave);

        //    DirectorySearcher srch = new DirectorySearcher(entry, "(CN=" + grupo + ")");
        //    SearchResultCollection coll = srch.FindAll();
        //    foreach (SearchResult rs in coll)
        //    {
        //        ResultPropertyCollection resultPropColl = rs.Properties;
        //        foreach (Object memberColl in resultPropColl["member"])
        //        {
        //            DirectoryEntry gpMemberEntry = new DirectoryEntry("LDAP://" + ipServer + "/" + memberColl, usuario, clave);
        //            System.DirectoryServices.PropertyCollection userProps = gpMemberEntry.Properties;
        //            object obVal = userProps["sAMAccountName"].Value;
        //            if (null != obVal)
        //            {
        //                usuarios.Add(obVal.ToString());
        //            }
        //        }
        //    }

        //    return usuarios;

        //}
        #endregion


    }
}