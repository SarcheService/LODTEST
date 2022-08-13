using System.Web.Mvc;

namespace LOD_APR.Areas.GLOD
{
    public class GLODAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "GLOD";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "GLOD_default",
                "GLOD/{controller}/{action}/{id}",
                new { Controller="Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}