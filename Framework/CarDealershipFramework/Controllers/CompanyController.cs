using CarDealershipFramework.Models;
using CarDealershipFramework.Repositories;
using CarDealershipFramework.ViewModels;
using System;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CarDealershipFramework.Controllers
{
	[RoutePrefix("companies")]
	public class CompanyController : Controller
	{
		private readonly IGenericRepository<Company> _companyRepository;

		public CompanyController()
		{
			_companyRepository = new GenericRepository<Company>(new CarDealershipDbEntities());
		}

		[HttpGet]
		[Route("", Name = "CompanyIndex")]
		public async Task<ActionResult> Index()
		{
			var companies = await _companyRepository.GetAllAsync();
			return View(companies.OrderBy(c => c.Company_name).Select(MapCompany));
		}

		[HttpGet]
		[Route("{companyId:int}", Name = "CompanyDetails")]
		public async Task<ActionResult> Details(int companyId)
		{
			var company = await _companyRepository.GetByIdAsync(companyId);

			if (company == null)
			{
				return HttpNotFound();
			}

			return View(MapCompany(company));
		}

		[HttpGet]
		[Route("create", Name = "CompanyCreate")]
		public ActionResult Create()
		{
			return View(new CompanyViewModel { Establish_date = DateTime.Today });
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[Route("create", Name = "CompanyCreatePost")]
		public async Task<ActionResult> Create(CompanyViewModel viewModel)
		{
			if (!ModelState.IsValid)
			{
				return View(viewModel);
			}

			var company = new Company
			{
				Company_name = viewModel.Company_name,
				Establish_date = viewModel.Establish_date,
				Country = viewModel.Country
			};

			try
			{
				_companyRepository.Add(company);
				await _companyRepository.SaveChangesAsync();
			}
			catch (DbException)
			{
				ModelState.AddModelError("", "Could not create the company.");
				return View(viewModel);
			}

			TempData["Success"] = "Company created.";
			return RedirectToRoute("CompanyIndex");
		}

		[HttpGet]
		[Route("edit/{companyId:int}", Name = "CompanyEdit")]
		public async Task<ActionResult> Edit(int companyId)
		{
			var company = await _companyRepository.GetByIdAsync(companyId);

			if (company == null)
			{
				return HttpNotFound();
			}

			return View(MapCompany(company));
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[Route("edit/{companyId:int}", Name = "CompanyEditPost")]
		public async Task<ActionResult> Edit(int companyId, CompanyViewModel viewModel)
		{
			if (companyId != viewModel.Company_id)
			{
				return HttpNotFound();
			}

			if (!ModelState.IsValid)
			{
				return View(viewModel);
			}

			var company = await _companyRepository.GetByIdAsync(companyId);

			if (company == null)
			{
				return HttpNotFound();
			}

			company.Company_name = viewModel.Company_name;
			company.Establish_date = viewModel.Establish_date;
			company.Country = viewModel.Country;

			try
			{
				await _companyRepository.SaveChangesAsync();
			}
			catch (DbException)
			{
				ModelState.AddModelError("", "Could not update the company.");
				return View(viewModel);
			}

			TempData["Success"] = "Company updated.";
			return RedirectToRoute("CompanyDetails", new { companyId });
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[Route("delete/{companyId:int}", Name = "CompanyDelete")]
		public async Task<ActionResult> Delete(int companyId)
		{
			var company = await _companyRepository.GetByIdAsync(companyId);

			if (company == null)
			{
				return HttpNotFound();
			}

			company.Dealerships.Clear();
			_companyRepository.Remove(company);
			await _companyRepository.SaveChangesAsync();

			TempData["Success"] = "Company deleted.";
			return RedirectToRoute("CompanyIndex");
		}

		private static CompanyViewModel MapCompany(Company company)
		{
			return new CompanyViewModel
			{
				Company_id = company.Company_id,
				Company_name = company.Company_name,
				Establish_date = company.Establish_date,
				Country = company.Country,
				DealershipNames = company.Dealerships
					.OrderBy(d => d.Name)
					.Select(d => d.Name)
					.ToList()
			};
		}
	}
}
