using CarDealership.Business.Dtos.Dealership;
using CarDealership.Business.Services;
using CarDealershipFramework.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CarDealershipFramework.Controllers
{
	[RoutePrefix("dealerships")]
	public class DealershipController : Controller
	{
		private readonly IDealershipService _dealershipService;
		private readonly ICompanyService _companyService;

		public DealershipController(IDealershipService dealershipService, ICompanyService companyService)
		{
			_dealershipService = dealershipService;
			_companyService = companyService;
		}

		[HttpGet]
		[Route("")]
		public async Task<ActionResult> Index()
		{
			var dealerships = await _dealershipService.GetAllDealerships();
			return View(dealerships.OrderBy(dealership => dealership.Name).Select(MapToViewModel));
		}

		[HttpGet]
		[Route("{dealershipId:int}")]
		public async Task<ActionResult> Details(int dealershipId)
		{
			var dealership = await _dealershipService.GetDealershipById(dealershipId);

			if (dealership is null)
			{
				return HttpNotFound();
			}

			return View(MapToViewModel(dealership));
		}

		[HttpGet]
		[Route("create")]
		public async Task<ActionResult> Create()
		{
			return View(new DealershipViewModel
			{
				CompanyOptions = await GetCompanyOptionsAsync()
			});
		}

		[HttpPost]
		[Route("create")]
		public async Task<ActionResult> Create(DealershipViewModel viewModel)
		{
			if (!ModelState.IsValid)
			{
				viewModel.CompanyOptions = await GetCompanyOptionsAsync();
				return View(viewModel);
			}

			var request = new CreateDealershipRequest
			{
				License = viewModel.License,
				Name = viewModel.Name,
				City = viewModel.City,
				Phone = viewModel.Phone,
				CompanyIds = viewModel.SelectedCompanyIds
			};

			var createdDealership = await _dealershipService.AddDealership(request);
			if (createdDealership is null)
			{
				ModelState.AddModelError(string.Empty, "Unable to create the dealership. Check the entered data, license, and selected companies.");
				viewModel.CompanyOptions = await GetCompanyOptionsAsync();
				return View(viewModel);
			}

			TempData["Success"] = "Dealership created.";
			return RedirectToAction("Index");
		}

		[HttpGet]
		[Route("edit/{dealershipId:int}")]
		public async Task<ActionResult> Edit(int dealershipId)
		{
			var dealership = await _dealershipService.GetDealershipById(dealershipId);

			if (dealership is null)
			{
				return HttpNotFound();
			}

			var viewModel = MapToViewModel(dealership);
			viewModel.CompanyOptions = await GetCompanyOptionsAsync();
			return View(viewModel);
		}

		[HttpPost]
		[Route("edit/{dealershipId:int}")]
		public async Task<ActionResult> Edit(int dealershipId, DealershipViewModel viewModel)
		{
			if (dealershipId != viewModel.Id)
			{
				return HttpNotFound();
			}

			if (!ModelState.IsValid)
			{
				viewModel.CompanyOptions = await GetCompanyOptionsAsync();
				return View(viewModel);
			}

			if (await _dealershipService.GetDealershipById(dealershipId) is null)
			{
				return HttpNotFound();
			}

			var request = new UpdateDealershipRequest
			{
				Id = viewModel.Id,
				License = viewModel.License,
				Name = viewModel.Name,
				City = viewModel.City,
				Phone = viewModel.Phone,
				CompanyIds = viewModel.SelectedCompanyIds
			};

			var isUpdated = await _dealershipService.UpdateDealership(request);
			if (!isUpdated)
			{
				ModelState.AddModelError(string.Empty, "Unable to update the dealership. Check the entered data, license, and selected companies.");
				viewModel.CompanyOptions = await GetCompanyOptionsAsync();
				return View(viewModel);
			}

			TempData["Success"] = "Dealership updated.";
			return RedirectToAction("Details", new { dealershipId });
		}

		[HttpPost]
		[Route("delete/{dealershipId:int}")]
		public async Task<ActionResult> Delete(int dealershipId)
		{
			if (await _dealershipService.GetDealershipById(dealershipId) is null)
			{
				return HttpNotFound();
			}

			if (!await _dealershipService.DeleteDealership(dealershipId))
			{
				TempData["Error"] = "A dealership with cars cannot be deleted.";
				return RedirectToAction("Details", new { dealershipId });
			}

			TempData["Success"] = "Dealership deleted.";
			return RedirectToAction("Index");
		}

		private async Task<IEnumerable<SelectListItem>> GetCompanyOptionsAsync()
		{
			var companies = await _companyService.GetAllCompanies();

			return companies
				.OrderBy(company => company.Name)
				.Select(company => new SelectListItem
				{
					Text = company.Name,
					Value = company.Id.ToString()
				})
				.ToList();
		}

		private static DealershipViewModel MapToViewModel(DealershipDto dealership)
		{
			return new DealershipViewModel
			{
				Id = dealership.Id,
				License = dealership.License,
				Name = dealership.Name,
				City = dealership.City,
				Phone = dealership.Phone,
				SelectedCompanyIds = (dealership.Companies ?? Enumerable.Empty<CarDealership.Business.Dtos.Company.CompanySummaryDto>())
					.Select(company => company.Id)
					.ToList(),
				CompanyNames = (dealership.Companies ?? Enumerable.Empty<CarDealership.Business.Dtos.Company.CompanySummaryDto>())
					.OrderBy(company => company.Name)
					.Select(company => company.Name)
					.ToList(),
				Cars = (dealership.Cars ?? Enumerable.Empty<CarDealership.Business.Dtos.Car.CarSummaryDto>())
					.OrderBy(car => car.Make)
					.ThenBy(car => car.Model)
					.Select(car => new CarSummaryViewModel
					{
						Id = car.Id,
						Make = car.Make,
						Model = car.Model,
						ManufactureYear = car.ManufactureYear
					})
					.ToList()
			};
		}
	}
}
