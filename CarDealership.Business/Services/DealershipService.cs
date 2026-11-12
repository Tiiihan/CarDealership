using CarDealership.Business.Domain;
using CarDealership.Business.Dtos.Car;
using CarDealership.Business.Dtos.Company;
using CarDealership.Business.Dtos.Dealership;
using CarDealership.Business.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CarDealership.Business.Services
{
	public class DealershipService : IDealershipService
	{
		private const int MaxLicenseLength = 50;
		private const int MaxNameLength = 150;
		private const int MaxCityLength = 100;
		private const int MaxPhoneLength = 25;

		private readonly IDealershipRepository _dealershipRepository;
		private readonly ICompanyRepository _companyRepository;
		private readonly ICarRepository _carRepository;

		public DealershipService(
			IDealershipRepository dealershipRepository,
			ICompanyRepository companyRepository,
			ICarRepository carRepository)
		{
			_dealershipRepository = dealershipRepository;
			_companyRepository = companyRepository;
			_carRepository = carRepository;
		}

		public async Task<DealershipDto> AddDealership(CreateDealershipRequest request)
		{
			if (request is null || !IsRequestValid(request.License, request.Name, request.City, request.Phone))
			{
				return null;
			}

			if (await _dealershipRepository.GetByLicense(request.License) != null)
			{
				return null;
			}

			var companyIds = GetDistinctCompanyIds(request.CompanyIds);
			var companies = await GetCompaniesIfValid(companyIds);

			if (companies is null)
			{
				return null;
			}

			var dealership = await _dealershipRepository.Create(new Dealership
			{
				License = request.License,
				Name = request.Name,
				City = request.City,
				Phone = request.Phone
			});

			await _dealershipRepository.ReplaceCompanies(dealership.Id, companyIds);
			return MapDealership(dealership, Enumerable.Empty<Car>(), companies);
		}

		public async Task<bool> DeleteDealership(int id)
		{
			var dealership = await _dealershipRepository.GetById(id);
			if (dealership is null || await _carRepository.HasAnyForDealership(id))
			{
				return false;
			}

			await _dealershipRepository.ReplaceCompanies(id, Enumerable.Empty<int>());
			await _dealershipRepository.Delete(dealership);
			return true;
		}

		public async Task<IEnumerable<DealershipDto>> GetAllDealerships()
		{
			var dealerships = await _dealershipRepository.GetAll();
			if (dealerships is null)
			{
				return Enumerable.Empty<DealershipDto>();
			}

			return dealerships.Select(dealership => MapDealership(
				dealership,
				Enumerable.Empty<Car>(),
				Enumerable.Empty<Company>()));
		}

		public async Task<DealershipDto> GetDealershipById(int id)
		{
			var dealership = await _dealershipRepository.GetById(id);
			if (dealership is null)
			{
				return null;
			}

			var cars = await _carRepository.GetByDealershipId(id);
			var companies = await _companyRepository.GetByDealershipId(id);

			return MapDealership(
				dealership,
				cars ?? Enumerable.Empty<Car>(),
				companies ?? Enumerable.Empty<Company>());
		}

		public async Task<bool> UpdateDealership(UpdateDealershipRequest request)
		{
			if (request is null || !IsRequestValid(request.License, request.Name, request.City, request.Phone))
			{
				return false;
			}

			var dealership = await _dealershipRepository.GetById(request.Id);
			if (dealership is null)
			{
				return false;
			}

			var license = request.License;
			var dealershipWithLicense = await _dealershipRepository.GetByLicense(license);
			if (dealershipWithLicense is object && dealershipWithLicense.Id != request.Id)
			{
				return false;
			}

			var companyIds = GetDistinctCompanyIds(request.CompanyIds);
			if (await GetCompaniesIfValid(companyIds) is null)
			{
				return false;
			}

			dealership.License = license;
			dealership.Name = request.Name;
			dealership.City = request.City;
			dealership.Phone = request.Phone;

			await _dealershipRepository.Update(dealership);
			await _dealershipRepository.ReplaceCompanies(dealership.Id, companyIds);
			return true;
		}

		private async Task<IEnumerable<Company>> GetCompaniesIfValid(IEnumerable<int> companyIds)
		{
			if (!companyIds.Any())
			{
				return Enumerable.Empty<Company>();
			}

			if (companyIds.Any(id => id <= 0))
			{
				return null;
			}

			var companies = await _companyRepository.GetByIds(companyIds);
			return companies.Count() == companyIds.Count() ? companies : null;
		}

		private static IEnumerable<int> GetDistinctCompanyIds(IEnumerable<int> companyIds)
		{
			return (companyIds ?? Enumerable.Empty<int>()).Distinct();
		}

		private static bool IsRequestValid(string license, string name, string city, string phone)
		{
			return !string.IsNullOrWhiteSpace(license)
				&& license.Length <= MaxLicenseLength
				&& !string.IsNullOrWhiteSpace(name)
				&& name.Length <= MaxNameLength
				&& !string.IsNullOrWhiteSpace(city)
				&& city.Length <= MaxCityLength
				&& (string.IsNullOrWhiteSpace(phone) || phone.Length <= MaxPhoneLength);
		}

		private static DealershipDto MapDealership(Dealership dealership, IEnumerable<Car> cars, IEnumerable<Company> companies)
		{
			return new DealershipDto
			{
				Id = dealership.Id,
				License = dealership.License,
				Name = dealership.Name,
				City = dealership.City,
				Phone = dealership.Phone,
				Cars = cars.Select(car => new CarSummaryDto
				{
					Id = car.Id,
					Make = car.Make,
					Model = car.Model,
					ManufactureYear = car.ManufactureYear
				}),
				Companies = companies.Select(company => new CompanySummaryDto
				{
					Id = company.Id,
					Name = company.Name
				})
			};
		}
	}
}
