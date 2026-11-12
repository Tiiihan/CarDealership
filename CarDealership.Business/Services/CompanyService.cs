using CarDealership.Business.Domain;
using CarDealership.Business.Dtos.Company;
using CarDealership.Business.Dtos.Dealership;
using CarDealership.Business.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CarDealership.Business.Services
{
	public class CompanyService : ICompanyService
	{
		private const int MaxNameLength = 300;
		private const int MaxCountryLength = 60;

		private readonly ICompanyRepository _companyRepository;

		public CompanyService(ICompanyRepository companyRepository)
		{
			_companyRepository = companyRepository;
		}

		public async Task<CompanyDto> CreateCompany(CreateCompanyRequest request)
		{
			if (request is null || !IsRequestValid(request.Name, request.EstablishDate, request.Country))
			{
				return null;
			}

			var company = await _companyRepository.Create(new Company
			{
				Name = request.Name,
				EstablishDate = request.EstablishDate,
				Country = request.Country
			});

			return MapCompany(company, Enumerable.Empty<Dealership>());
		}

		public async Task<bool> DeleteCompany(int id)
		{
			var company = await _companyRepository.GetById(id);
			if (company is null)
			{
				return false;
			}

			await _companyRepository.RemoveFromDealerships(id);
			await _companyRepository.Delete(company);
			return true;
		}

		public async Task<IEnumerable<CompanyDto>> GetAllCompanies()
		{
			var companies = await _companyRepository.GetAll();
			if (companies is null)
			{
				return Enumerable.Empty<CompanyDto>();
			}

			return companies.Select(company => MapCompany(company, Enumerable.Empty<Dealership>()));
		}

		public async Task<CompanyDto> GetCompanyById(int id)
		{
			var company = await _companyRepository.GetById(id);
			if (company is null)
			{
				return null;
			}

			var dealerships = await _companyRepository.GetDealershipsByCompanyId(id);
			return MapCompany(company, dealerships ?? Enumerable.Empty<Dealership>());
		}

		public async Task<bool> UpdateCompany(UpdateCompanyRequest request)
		{
			if (request is null || !IsRequestValid(request.Name, request.EstablishDate, request.Country))
			{
				return false;
			}

			var company = await _companyRepository.GetById(request.Id);
			if (company is null)
			{
				return false;
			}

			company.Name = request.Name;
			company.EstablishDate = request.EstablishDate;
			company.Country = request.Country;

			await _companyRepository.Update(company);
			return true;
		}

		private static bool IsRequestValid(string name, DateTime establishDate, string country)
		{
			return !string.IsNullOrWhiteSpace(name)
				&& name.Length <= MaxNameLength
				&& establishDate != default
				&& establishDate.Date <= DateTime.Today
				&& (string.IsNullOrWhiteSpace(country) || country.Length <= MaxCountryLength);
		}

		private static CompanyDto MapCompany(Company company, IEnumerable<Dealership> dealerships)
		{
			return new CompanyDto
			{
				Id = company.Id,
				Name = company.Name,
				EstablishDate = company.EstablishDate,
				Country = company.Country,
				Dealerships = dealerships.Select(dealership => new DealershipSummaryDto
				{
					Id = dealership.Id,
					License = dealership.License,
					Name = dealership.Name
				})
			};
		}
	}
}
