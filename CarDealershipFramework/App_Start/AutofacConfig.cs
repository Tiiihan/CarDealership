using Autofac;
using Autofac.Integration.Mvc;
using CarDealership.Business.Repositories;
using CarDealership.Business.Services;
using CarDealershipFramework.Models;
using CarDealershipFramework.Repositories;
using System.Web.Mvc;

namespace CarDealershipFramework.App_Start
{
	public static class AutofacConfig
	{
		public static void RegisterComponents()
		{
			var builder = new ContainerBuilder();

			builder.RegisterControllers(typeof(MvcApplication).Assembly);

			builder.RegisterType<CarDealershipDbEntities>().InstancePerRequest();

			builder.RegisterType<CarRepository>().As<ICarRepository>().InstancePerRequest();
			builder.RegisterType<DealershipRepository>().As<IDealershipRepository>().InstancePerRequest();
			builder.RegisterType<CompanyRepository>().As<ICompanyRepository>().InstancePerRequest();

			builder.RegisterType<CarService>().As<ICarService>().InstancePerRequest();
			builder.RegisterType<DealershipService>().As<IDealershipService>().InstancePerRequest();
			builder.RegisterType<CompanyService>().As<ICompanyService>().InstancePerRequest();

			var container = builder.Build();
			DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
		}
	}
}
