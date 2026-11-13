using CarDealership.Core.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace CarDealership.Core.Data;

public static class DbInitializer
{
	public static async Task SeedAsync(CarDealershipDbContext dbContext)
	{
		if (await dbContext.Companies.AnyAsync()
			|| await dbContext.Dealerships.AnyAsync()
			|| await dbContext.Cars.AnyAsync())
		{
			return;
		}

		var toyota = new CompanyEntity
		{
			Name = "Toyota Motor Corporation",
			EstablishDate = new DateTime(1937, 8, 28),
			Country = "Japan"
		};

		var volkswagen = new CompanyEntity
		{
			Name = "Volkswagen AG",
			EstablishDate = new DateTime(1937, 5, 28),
			Country = "Germany"
		};

		var hyundai = new CompanyEntity
		{
			Name = "Hyundai Motor Company",
			EstablishDate = new DateTime(1967, 12, 29),
			Country = "South Korea"
		};

		var kia = new CompanyEntity
		{
			Name = "Kia Corporation",
			EstablishDate = new DateTime(1944, 12, 11),
			Country = "South Korea"
		};

		var skoda = new CompanyEntity
		{
			Name = "Skoda Auto",
			EstablishDate = new DateTime(1895, 12, 18),
			Country = "Czech Republic"
		};

		var kyivDealership = new DealershipEntity
		{
			License = "KYIV-001",
			Name = "Auto City Kyiv",
			City = "Kyiv",
			Phone = "+380441112233",
			Companies = [toyota, volkswagen],
			Cars =
			[
				new CarEntity { Make = "Toyota", Model = "Camry", ManufactureYear = 2023 },
				new CarEntity { Make = "Volkswagen", Model = "Tiguan", ManufactureYear = 2022 },
				new CarEntity { Make = "Toyota", Model = "Corolla", ManufactureYear = 2024 }
			]
		};

		var capitalMotors = new DealershipEntity
		{
			License = "KYIV-002",
			Name = "Capital Motors",
			City = "Kyiv",
			Phone = "+380445556677",
			Companies = [toyota, kia],
			Cars =
			[
				new CarEntity { Make = "Kia", Model = "Sportage", ManufactureYear = 2024 },
				new CarEntity { Make = "Toyota", Model = "RAV4", ManufactureYear = 2023 }
			]
		};

		var lvivDealership = new DealershipEntity
		{
			License = "LVIV-001",
			Name = "West Auto",
			City = "Lviv",
			Phone = "+380322223344",
			Companies = [hyundai, toyota, skoda],
			Cars =
			[
				new CarEntity { Make = "Hyundai", Model = "Tucson", ManufactureYear = 2024 },
				new CarEntity { Make = "Toyota", Model = "Corolla", ManufactureYear = 2021 },
				new CarEntity { Make = "Skoda", Model = "Kodiaq", ManufactureYear = 2023 }
			]
		};

		var odesaDealership = new DealershipEntity
		{
			License = "ODESA-001",
			Name = "South Drive",
			City = "Odesa",
			Phone = "+380487778899",
			Companies = [volkswagen, skoda, kia],
			Cars =
			[
				new CarEntity { Make = "Volkswagen", Model = "Golf", ManufactureYear = 2020 },
				new CarEntity { Make = "Kia", Model = "Sportage", ManufactureYear = 2023 },
				new CarEntity { Make = "Skoda", Model = "Octavia", ManufactureYear = 2022 }
			]
		};

		var dniproDealership = new DealershipEntity
		{
			License = "DNIPRO-001",
			Name = "East Auto",
			City = "Dnipro",
			Phone = "+380562223344",
			Companies = [hyundai, kia],
			Cars =
			[
				new CarEntity { Make = "Hyundai", Model = "Elantra", ManufactureYear = 2023 },
				new CarEntity { Make = "Kia", Model = "Sorento", ManufactureYear = 2022 },
				new CarEntity { Make = "Hyundai", Model = "Tucson", ManufactureYear = 2021 }
			]
		};

		var kharkivDealership = new DealershipEntity
		{
			License = "KHARKIV-001",
			Name = "Drive Center",
			City = "Kharkiv",
			Phone = "+380577778899",
			Companies = [volkswagen, skoda],
			Cars =
			[
				new CarEntity { Make = "Volkswagen", Model = "Passat", ManufactureYear = 2021 },
				new CarEntity { Make = "Skoda", Model = "Scala", ManufactureYear = 2024 }
			]
		};

		dbContext.Dealerships.AddRange(
			kyivDealership,
			capitalMotors,
			lvivDealership,
			odesaDealership,
			dniproDealership,
			kharkivDealership);
		await dbContext.SaveChangesAsync();
	}
}
