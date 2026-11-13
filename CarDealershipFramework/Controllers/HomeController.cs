using System.Web.Mvc;

namespace CarDealershipFramework.Controllers
{
	public class HomeController : Controller
	{
		public ActionResult Index()
		{
			return RedirectToAction("Index", "Car");
		}
	}
}
