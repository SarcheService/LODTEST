using LOD_APR.Areas.Admin.Models;
using LOD_APR.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LOD_APR.Helpers
{
    public class MailServer
    {
        private StringBuilder EmailBodyItems;
        private string subject;
        private string user;
        public MailServer(string Subject,string UserName)
        {
            EmailBodyItems = new StringBuilder();
            subject = Subject;
            user = UserName;
        }
        public void InsertBodyItem(string Item, ItemType ItemType ,Align ItemAlignment)
        {
            string type = string.Empty;
            string align = string.Empty;

            if (ItemType == ItemType.p)
            {
                type = "p";
            }
            else if (ItemType == ItemType.H1)
            {
                type = "h1";
            }
            else if (ItemType == ItemType.H2)
            {
                type = "h2";
            }
            else if (ItemType == ItemType.H3)
            {
                type = "h3";
            }
            else if (ItemType == ItemType.H4)
            {
                type = "h4";
            }
            else if (ItemType == ItemType.H5)
            {
                type = "h5";
            }

            if (ItemAlignment == Align.Left)
            {
                align = "left";
            }
            else if (ItemAlignment == Align.Right)
            {
                align = "right";
            }
            else if (ItemAlignment == Align.Center)
            {
                align = "center";
            }

            string NewItem = String.Format("<{0} align=\"{2}\">{1}</{0}>", type,Item,align);
            EmailBodyItems.Append(NewItem);
            
        }
        public void InsertHtmlTable<T>(IEnumerable<T> list, params Expression<Func<T, object>>[] fxns)
        {
            string htmlTable = GetHtmlTable(list, fxns);
            EmailBodyItems.Append(htmlTable);
        }
        public void InsertFlexBoxTable<T>(T TableObject, params Expression<Func<T, object>>[] Fields)
        {
            EmailBodyItems.Append(GetFlexBoxTable(TableObject,Fields));
        }
        public void InsertFlexBoxTable(List<FlexBoxItem> Items)
        {
            
            StringBuilder sb = new StringBuilder();
            sb.Append("<table class=\"flex\">\n");

            foreach (FlexBoxItem item in Items)
            {
                sb.Append("<tr>\n");
                sb.Append("<th>");
                sb.Append(item.item);
                sb.Append("</th>");
                sb.Append("<td>");
                sb.Append(item.description);
                sb.Append("</td>");
                sb.Append("</tr>\n");
            }
            sb.Append("</table>");
            EmailBodyItems.Append(sb.ToString());
        }
        public void InsertSpace()
        {
            EmailBodyItems.Append("<br>");
        }
        public void InsertLine()
        {
            EmailBodyItems.Append("<hr>");
        }




        public async Task<int> SendEmail(string To)
        {
            LOD_DB db = new LOD_DB();
            MAE_Empresa empresa = db.MAE_Empresa.Find(12);
            string ruta_logo = System.Web.HttpContext.Current.Request.Url.Scheme + "://" + System.Web.HttpContext.Current.Request.Url.Authority + System.Web.HttpContext.Current.Request.ApplicationPath.TrimEnd('/') + empresa.LogoData.Substring(1);
            string BodyEmail = Properties.Resources.Email.ToString();
            BodyEmail = BodyEmail.Replace("{cuerpo_email}", EmailBodyItems.ToString()).Replace("{ruta_logo}",ruta_logo);
            MAIL_Envio email = new MAIL_Envio()
            {
                FH_IngresoBD = DateTime.Now,
                Body = BodyEmail,
                EstadoEnvio = false,
                Email = To,
                Usuario = user,
                Asunto = subject
            };
            db.MAIL_Envio.Add(email);
            return await db.SaveChangesAsync();
        }


        private string GetHtmlTable<T>(IEnumerable<T> list, params Expression<Func<T, object>>[] fxns)
        {

            StringBuilder sb = new StringBuilder();
            sb.Append("<table>\n");

            sb.Append("<tr>\n");
            foreach (var fxn in fxns)
            {
                sb.Append("<th>");
                sb.Append(GetName(fxn));
                sb.Append("</th>");
            }
            sb.Append("</tr> <!-- HEADER -->\n");
            
            foreach (var item in list)
            {
                try
                {
                    sb.Append("<tr>\n");
                    foreach (var fxn in fxns)
                    {
                        sb.Append("<td>");
                        sb.Append(fxn.Compile()(item));
                        sb.Append("</td>");
                    }
                    sb.Append("</tr>\n");
                }
                catch { }
            }
            sb.Append("</table>");

            return sb.ToString();
        }
        private string GetFlexBoxTable<T>(T tableObject, params Expression<Func<T, object>>[] fxns)
        {

            StringBuilder sb = new StringBuilder();
            sb.Append("<table class=\"flex\">\n");

            foreach (var fxn in fxns)
            {
                try
                {
                    sb.Append("<tr>\n");
                    sb.Append("<th>");
                    sb.Append(GetName(fxn));
                    sb.Append("</th>");
                    sb.Append("<td>");
                    sb.Append(fxn.Compile()(tableObject));
                    sb.Append("</td>");
                    sb.Append("</tr>\n");
                }
                catch { }
            }            
            sb.Append("</table>");

            return sb.ToString();
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
       
        public struct FlexBoxItem
        {
            public string item { get; set; }
            public string description { get; set; }
            FlexBoxItem(string Item, string Description)
            {
                item = Item;
                description = Description;
            }
        }
        public enum ItemType
        {
            p = 0,
            H1 = 1,
            H2 = 2,
            H3 = 3,
            H4 = 4,
            H5 = 5
        }
        public enum Align
        {
            Left = 0,
            Right = 1,
            Center = 2
        }
    }
}