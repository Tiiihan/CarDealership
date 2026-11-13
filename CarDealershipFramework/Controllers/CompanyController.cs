using CarDealership.Business.Dtos.Company;
using CarDealership.Business.Services;
using CarDealershipFramework.ViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CarDealershipFramework.Controllers
{
	[RoutePrefix("companies")]
	public class CompanyController : Controller
	{
		private readonly ICompanyService _companyService;

		public CompanyController(ICompanyService companyService)
		{
			_companyService = companyService;
		}

		[HttpGet]
		[Route("")]
		public async Task<ActionResult> Index()
		{
			var companies = await _companyService.GetAllCompanies();
			return View(companies.OrderBy(company => company.Name).Select(MapToViewModel));
		}

		[HttpGet]
		[Route("{companyId:int}")]
		public Task<ActionResult> Details(int companyId)
		{
			return GetCompanyViewAsync(companyId, "Details");
		}

		[HttpGet]
		[Route("create")]
		public ActionResult Create()
		{
			return View(new CompanyViewModel { EstablishDate = DateTime.Today });
		}

		[HttpPost]
		[Route("create")]
		public async Task<ActionResult> Create(CompanyViewModel viewModel)
		{
			if (!ModelState.IsValid)
			{
				return View(viewModel);
			}

			var request = new CreateCompanyRequest
			{
				Name = viewModel.Name,
				EstablishDate = viewModel.EstablishDate,
				Country = viewModel.Country
			};

			var createdCompany = await _companyService.CreateCompany(request);
			if (createdCompany is null)
			{
				ModelState.AddModelError(string.Empty, "Unable to create the company. Check the entered data.");
				return View(viewModel);
			}

			TempData["Success"] = "Company created.";
			return RedirectToAction("Index");
		}

		[HttpGet]
		[Route("edit/{companyId:int}")]
		public Task<ActionResult> Edit(int companyId)
		{
			return GetCompanyViewAsync(companyId, "Edit");
		}

		[HttpPost]
		[Route("edit/{companyId:int}")]
		public async Task<ActionResult> Edit(int companyId, CompanyViewModel viewModel)
		{
			if (companyId != viewModel.Id)
			{
				return HttpNotFound();
			}

			if (!ModelState.IsValid)
			{
				return View(viewModel);
			}

			if (await _companyService.GetCompanyById(companyId) is null)
			{
				return HttpNotFound();
			}

			var request = new UpdateCompanyRequest
			{
				Id = viewModel.Id,
				Name = viewModel.Name,
				EstablishDate = viewModel.EstablishDate,
				Country = viewModel.Country
			};

			if (!await _companyService.UpdateCompany(request))
			{
				ModelState.AddModelError(string.Empty, "Unable to update the company. Check the entered data.");
				return View(viewModel);
			}

			TempData["Success"] = "Company updated.";
			return RedirectToAction("Details", new { companyId });
		}

		[HttpPost]
		[Route("delete/{companyId:int}")]
		public async Task<ActionResult> Delete(int companyId)
		{
			if (await _companyService.GetCompanyById(companyId) is null)
			{
				return HttpNotFound();
			}

			if (!await _companyService.DeleteCompany(companyId))
			{
				return HttpNotFound();
			}

			TempData["Success"] = "Company deleted.";
			return RedirectToAction("Index");
		}

		private static CompanyViewModel MapToViewModel(CompanyDto company)
		{
			return new CompanyViewModel
			{
				Id = company.Id,
				Name = company.Name,
				EstablishDate = company.EstablishDate,
				Country = company.Country,
				DealershipNames = (company.Dealerships ?? Enumerable.Empty<CarDealership.Business.Dtos.Dealership.DealershipSummaryDto>())
					.OrderBy(dealership => dealership.Name)
					.Select(dealership => dealership.Name)
					.ToList()
			};
		}

		private async Task<ActionResult> GetCompanyViewAsync(int companyId, string viewName)
		{
			var company = await _companyService.GetCompanyById(companyId);
			if (company is null)
			{
				return HttpNotFound();
			}

			return View(viewName, MapToViewModel(company));
		}
	}
}
