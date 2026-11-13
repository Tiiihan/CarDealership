using CarDealership.Business.Dtos.Company;
using CarDealership.Business.Services;
using CarDealership.Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace CarDealership.Core.Controllers;

public class CompanyController(ICompanyService companyService) : Controller
{
	[HttpGet]
	public async Task<IActionResult> Index()
	{
		var companies = await companyService.GetAllCompanies();
		return View(companies.OrderBy(company => company.Name).Select(MapToViewModel).ToList());
	}

	[HttpGet]
	public async Task<IActionResult> Details(int id)
	{
		var company = await companyService.GetCompanyById(id);
		if (company is null)
		{
			return NotFound();
		}

		return View(MapToViewModel(company));
	}

	[HttpGet]
	public IActionResult Create()
	{
		return View(new CompanyViewModel
		{
			EstablishDate = DateTime.Today
		});
	}

	[HttpPost]
	public async Task<IActionResult> Create(CompanyViewModel viewModel)
	{
		if (!ModelState.IsValid)
		{
			return View(viewModel);
		}

		var createdCompany = await companyService.CreateCompany(new CreateCompanyRequest
		{
			Name = viewModel.Name,
			EstablishDate = viewModel.EstablishDate,
			Country = viewModel.Country
		});

		if (createdCompany is null)
		{
			ModelState.AddModelError(string.Empty, "Unable to create the company. Check the entered data.");
			return View(viewModel);
		}

		TempData["Success"] = "Company created.";
		return RedirectToAction(nameof(Index));
	}

	[HttpGet]
	public async Task<IActionResult> Edit(int id)
	{
		var company = await companyService.GetCompanyById(id);
		if (company is null)
		{
			return NotFound();
		}

		return View(MapToViewModel(company));
	}

	[HttpPost]
	public async Task<IActionResult> Edit(int id, CompanyViewModel viewModel)
	{
		if (id != viewModel.Id)
		{
			return NotFound();
		}

		if (!ModelState.IsValid)
		{
			return View(viewModel);
		}

		if (await companyService.GetCompanyById(id) is null)
		{
			return NotFound();
		}

		var isUpdated = await companyService.UpdateCompany(new UpdateCompanyRequest
		{
			Id = viewModel.Id,
			Name = viewModel.Name,
			EstablishDate = viewModel.EstablishDate,
			Country = viewModel.Country
		});

		if (!isUpdated)
		{
			ModelState.AddModelError(string.Empty, "Unable to update the company. Check the entered data.");
			return View(viewModel);
		}

		TempData["Success"] = "Company updated.";
		return RedirectToAction(nameof(Details), new { id });
	}

	[HttpPost]
	public async Task<IActionResult> Delete(int id)
	{
		if (!await companyService.DeleteCompany(id))
		{
			return NotFound();
		}

		TempData["Success"] = "Company deleted.";
		return RedirectToAction(nameof(Index));
	}

	private static CompanyViewModel MapToViewModel(CompanyDto company)
	{
		return new CompanyViewModel
		{
			Id = company.Id,
			Name = company.Name,
			EstablishDate = company.EstablishDate,
			Country = company.Country,
			DealershipNames = [.. (company.Dealerships ?? [])
				.OrderBy(dealership => dealership.Name)
				.Select(dealership => dealership.Name)]
		};
	}
}
