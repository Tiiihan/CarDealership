using System.Web.Mvc;
using System.Web.Routing;

namespace CarDealershipFramework
{
	public class RouteConfig
	{
		public static void RegisterRoutes(RouteCollection routes)
		{
			routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

			routes.MapMvcAttributeRoutes();

			routes.MapRoute(
				name: "Root",
				url: "",
				defaults: new { controller = "Home", action = "Index" }
			);

			routes.MapRoute(
				name: "Default",
				url: "{controller}/{action}/{id}",
				defaults: new { controller = "Car", action = "Index", id = UrlParameter.Optional }
			);
		}
	}
}
