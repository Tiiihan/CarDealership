using CarDealership.Business.Dtos.Dealership;
using CarDealership.Business.Services;
using CarDealership.Core.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CarDealership.Core.Controllers;

public class DealershipController(IDealershipService dealershipService, ICompanyService companyService) : Controller
{
	[HttpGet]
	public async Task<IActionResult> Index()
	{
		var dealerships = await dealershipService.GetAllDealerships();
		return View(dealerships.OrderBy(dealership => dealership.Name).Select(MapToViewModel).ToList());
	}

	[HttpGet]
	public async Task<IActionResult> Details(int id)
	{
		var dealership = await dealershipService.GetDealershipById(id);
		if (dealership is null)
		{
			return NotFound();
		}

		return View(MapToViewModel(dealership));
	}

	[HttpGet]
	public async Task<IActionResult> Create()
	{
		return View(new DealershipViewModel
		{
			CompanyOptions = await GetCompanyOptionsAsync()
		});
	}

	[HttpPost]
	public async Task<IActionResult> Create(DealershipViewModel viewModel)
	{
		if (!ModelState.IsValid)
		{
			viewModel.CompanyOptions = await GetCompanyOptionsAsync();
			return View(viewModel);
		}

		var createdDealership = await dealershipService.AddDealership(new CreateDealershipRequest
		{
			License = viewModel.License,
			Name = viewModel.Name,
			City = viewModel.City,
			Phone = viewModel.Phone,
			CompanyIds = viewModel.SelectedCompanyIds
		});

		if (createdDealership is null)
		{
			ModelState.AddModelError(string.Empty, "Unable to create the dealership. Check the entered data, license, and selected companies.");
			viewModel.CompanyOptions = await GetCompanyOptionsAsync();
			return View(viewModel);
		}

		TempData["Success"] = "Dealership created.";
		return RedirectToAction(nameof(Index));
	}

	[HttpGet]
	public async Task<IActionResult> Edit(int id)
	{
		var dealership = await dealershipService.GetDealershipById(id);
		if (dealership is null)
		{
			return NotFound();
		}

		var viewModel = MapToViewModel(dealership);
		viewModel.CompanyOptions = await GetCompanyOptionsAsync();
		return View(viewModel);
	}

	[HttpPost]
	public async Task<IActionResult> Edit(int id, DealershipViewModel viewModel)
	{
		if (id != viewModel.Id)
		{
			return NotFound();
		}

		if (!ModelState.IsValid)
		{
			viewModel.CompanyOptions = await GetCompanyOptionsAsync();
			return View(viewModel);
		}

		if (await dealershipService.GetDealershipById(id) is null)
		{
			return NotFound();
		}

		var isUpdated = await dealershipService.UpdateDealership(new UpdateDealershipRequest
		{
			Id = viewModel.Id,
			License = viewModel.License,
			Name = viewModel.Name,
			City = viewModel.City,
			Phone = viewModel.Phone,
			CompanyIds = viewModel.SelectedCompanyIds
		});

		if (!isUpdated)
		{
			ModelState.AddModelError(string.Empty, "Unable to update the dealership. Check the entered data, license, and selected companies.");
			viewModel.CompanyOptions = await GetCompanyOptionsAsync();
			return View(viewModel);
		}

		TempData["Success"] = "Dealership updated.";
		return RedirectToAction(nameof(Details), new { id });
	}

	[HttpPost]
	public async Task<IActionResult> Delete(int id)
	{
		if (await dealershipService.GetDealershipById(id) is null)
		{
			return NotFound();
		}

		if (!await dealershipService.DeleteDealership(id))
		{
			TempData["Error"] = "A dealership with cars cannot be deleted.";
			return RedirectToAction(nameof(Details), new { id });
		}

		TempData["Success"] = "Dealership deleted.";
		return RedirectToAction(nameof(Index));
	}

	private async Task<IEnumerable<SelectListItem>> GetCompanyOptionsAsync()
	{
		var companies = await companyService.GetAllCompanies();

		return [.. companies
			.OrderBy(company => company.Name)
			.Select(company => new SelectListItem
			{
				Text = company.Name,
				Value = company.Id.ToString()
			})];
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
			SelectedCompanyIds = (dealership.Companies ?? [])
				.Select(company => company.Id)
				.ToList(),
			CompanyNames = (dealership.Companies ?? [])
				.OrderBy(company => company.Name)
				.Select(company => company.Name)
				.ToList(),
			Cars = (dealership.Cars ?? [])
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
