using CarDealershipFramework.Models;
using CarDealershipFramework.Repositories;
using CarDealershipFramework.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CarDealershipFramework.Controllers
{
	[RoutePrefix("dealerships")]
	public class DealershipController : Controller
	{
		private readonly IGenericRepository<Dealership> _dealershipRepository;
		private readonly IGenericRepository<Company> _companyRepository;

		public DealershipController()
		{
			var dbContext = new CarDealershipDbEntities();
			_dealershipRepository = new GenericRepository<Dealership>(dbContext);
			_companyRepository = new GenericRepository<Company>(dbContext);
		}

		[HttpGet]
		[Route("", Name = "DealershipIndex")]
		public async Task<ActionResult> Index()
		{
			var dealerships = await _dealershipRepository.GetAllAsync();
			return View(dealerships.OrderBy(d => d.Name).Select(MapDealership));
		}

		[HttpGet]
		[Route("{dealershipId:int}", Name = "DealershipDetails")]
		public async Task<ActionResult> Details(int dealershipId)
		{
			var dealership = await _dealershipRepository.GetByIdAsync(dealershipId);

			if (dealership == null)
			{
				return HttpNotFound();
			}

			return View(MapDealership(dealership));
		}

		[HttpGet]
		[Route("create", Name = "DealershipCreate")]
		public async Task<ActionResult> Create()
		{
			var viewModel = new DealershipViewModel();
			await PopulateCompanyOptionsAsync(viewModel);
			return View(viewModel);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[Route("create", Name = "DealershipCreatePost")]
		public async Task<ActionResult> Create(DealershipViewModel viewModel)
		{
			var dealerships = await _dealershipRepository.GetAllAsync();
			if (dealerships.Any(d => d.License == viewModel.License))
			{
				ModelState.AddModelError("License", "This license already exists.");
			}

			if (!await HasValidCompanyIdsAsync(viewModel.SelectedCompanyIds))
			{
				ModelState.AddModelError("SelectedCompanyIds", "One or more selected companies do not exist.");
			}

			if (!ModelState.IsValid)
			{
				await PopulateCompanyOptionsAsync(viewModel);
				return View(viewModel);
			}

			var dealership = new Dealership
			{
				License = viewModel.License,
				Name = viewModel.Name,
				City = viewModel.City,
				Phone = viewModel.Phone
			};

			var companyIds = GetDistinctCompanyIds(viewModel.SelectedCompanyIds);
			var companies = (await _companyRepository.GetAllAsync())
				.Where(c => companyIds.Contains(c.Company_id))
				.ToList();

			foreach (var company in companies)
			{
				dealership.Companies.Add(company);
			}

			try
			{
				_dealershipRepository.Add(dealership);
				await _dealershipRepository.SaveChangesAsync();
			}
			catch (DbException)
			{
				ModelState.AddModelError("", "Could not create the dealership.");
				await PopulateCompanyOptionsAsync(viewModel);
				return View(viewModel);
			}

			TempData["Success"] = "Dealership created.";
			return RedirectToRoute("DealershipIndex");
		}

		[HttpGet]
		[Route("edit/{dealershipId:int}", Name = "DealershipEdit")]
		public async Task<ActionResult> Edit(int dealershipId)
		{
			var dealership = await _dealershipRepository.GetByIdAsync(dealershipId);

			if (dealership == null)
			{
				return HttpNotFound();
			}

			var viewModel = MapDealership(dealership);
			viewModel.SelectedCompanyIds = dealership.Companies.Select(c => c.Company_id).ToList();
			await PopulateCompanyOptionsAsync(viewModel);
			return View(viewModel);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[Route("edit/{dealershipId:int}", Name = "DealershipEditPost")]
		public async Task<ActionResult> Edit(int dealershipId, DealershipViewModel viewModel)
		{
			if (dealershipId != viewModel.Dealership_id)
			{
				return HttpNotFound();
			}

			var dealerships = await _dealershipRepository.GetAllAsync();
			if (dealerships.Any(d => d.License == viewModel.License && d.Dealership_id != dealershipId))
			{
				ModelState.AddModelError("License", "This license already exists.");
			}

			if (!await HasValidCompanyIdsAsync(viewModel.SelectedCompanyIds))
			{
				ModelState.AddModelError("SelectedCompanyIds", "One or more selected companies do not exist.");
			}

			if (!ModelState.IsValid)
			{
				await PopulateCompanyOptionsAsync(viewModel);
				return View(viewModel);
			}

			var dealership = await _dealershipRepository.GetByIdAsync(dealershipId);

			if (dealership == null)
			{
				return HttpNotFound();
			}

			dealership.License = viewModel.License;
			dealership.Name = viewModel.Name;
			dealership.City = viewModel.City;
			dealership.Phone = viewModel.Phone;

			dealership.Companies.Clear();
			var companyIds = GetDistinctCompanyIds(viewModel.SelectedCompanyIds);
			var companies = (await _companyRepository.GetAllAsync())
				.Where(c => companyIds.Contains(c.Company_id))
				.ToList();

			foreach (var company in companies)
			{
				dealership.Companies.Add(company);
			}

			try
			{
				await _dealershipRepository.SaveChangesAsync();
			}
			catch (DbException)
			{
				ModelState.AddModelError("", "Could not update the dealership.");
				await PopulateCompanyOptionsAsync(viewModel);
				return View(viewModel);
			}

			TempData["Success"] = "Dealership updated.";
			return RedirectToRoute("DealershipDetails", new { dealershipId });
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[Route("delete/{dealershipId:int}", Name = "DealershipDelete")]
		public async Task<ActionResult> Delete(int dealershipId)
		{
			var dealership = await _dealershipRepository.GetByIdAsync(dealershipId);

			if (dealership == null)
			{
				return HttpNotFound();
			}

			if (dealership.Cars.Any())
			{
				TempData["Error"] = "A dealership with cars cannot be deleted.";
				return RedirectToRoute("DealershipDetails", new { dealershipId });
			}

			dealership.Companies.Clear();
			_dealershipRepository.Remove(dealership);
			await _dealershipRepository.SaveChangesAsync();

			TempData["Success"] = "Dealership deleted.";
			return RedirectToRoute("DealershipIndex");
		}

		private async Task PopulateCompanyOptionsAsync(DealershipViewModel viewModel)
		{
			var selectedCompanyIds = GetDistinctCompanyIds(viewModel.SelectedCompanyIds);
			var companies = await _companyRepository.GetAllAsync();

			viewModel.CompanyOptions = companies
				.OrderBy(c => c.Company_name)
				.Select(c => new SelectListItem
				{
					Text = c.Company_name,
					Value = c.Company_id.ToString(),
					Selected = selectedCompanyIds.Contains(c.Company_id)
				})
				.ToList();
		}

		private async Task<bool> HasValidCompanyIdsAsync(IEnumerable<int> companyIds)
		{
			var ids = GetDistinctCompanyIds(companyIds);

			if (!ids.Any())
			{
				return true;
			}

			var companies = await _companyRepository.GetAllAsync();
			return ids.All(id => companies.Any(c => c.Company_id == id));
		}

		private static List<int> GetDistinctCompanyIds(IEnumerable<int> companyIds)
		{
			return (companyIds ?? Enumerable.Empty<int>()).Distinct().ToList();
		}

		private static DealershipViewModel MapDealership(Dealership dealership)
		{
			return new DealershipViewModel
			{
				Dealership_id = dealership.Dealership_id,
				License = dealership.License,
				Name = dealership.Name,
				City = dealership.City,
				Phone = dealership.Phone,
				CompanyNames = dealership.Companies
					.OrderBy(c => c.Company_name)
					.Select(c => c.Company_name)
					.ToList(),
				Cars = dealership.Cars
					.OrderBy(c => c.Car_make)
					.ThenBy(c => c.Car_model)
					.Select(c => new CarSummaryViewModel
					{
						Car_id = c.Car_id,
						Car_make = c.Car_make,
						Car_model = c.Car_model,
						Manufacture_year = c.Manufacture_year
					})
					.ToList()
			};
		}
	}
}
