using CarDealershipFramework.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CarDealershipFramework.App_Start
{
	public static class DatabaseSeeder
	{
		public static void Seed()
		{
			using (var dbContext = new CarDealershipDbEntities())
			{
				if (dbContext.Companies.Any() || dbContext.Dealerships.Any() || dbContext.Cars.Any())
				{
					return;
				}

				var toyota = new Company
				{
					Company_name = "Toyota Motor Corporation",
					Establish_date = new DateTime(1937, 8, 28),
					Country = "Japan"
				};

				var volkswagen = new Company
				{
					Company_name = "Volkswagen AG",
					Establish_date = new DateTime(1937, 5, 28),
					Country = "Germany"
				};

				var hyundai = new Company
				{
					Company_name = "Hyundai Motor Company",
					Establish_date = new DateTime(1967, 12, 29),
					Country = "South Korea"
				};

				var kia = new Company
				{
					Company_name = "Kia Corporation",
					Establish_date = new DateTime(1944, 12, 11),
					Country = "South Korea"
				};

				var skoda = new Company
				{
					Company_name = "Skoda Auto",
					Establish_date = new DateTime(1895, 12, 18),
					Country = "Czech Republic"
				};

				var kyivDealership = new Dealership
				{
					License = "KYIV-001",
					Name = "Auto City Kyiv",
					City = "Kyiv",
					Phone = "+380441112233",
					Companies = new List<Company> { toyota, volkswagen },
					Cars = new List<Car>
					{
						new Car { Car_make = "Toyota", Car_model = "Camry", Manufacture_year = 2023 },
						new Car { Car_make = "Volkswagen", Car_model = "Tiguan", Manufacture_year = 2022 },
						new Car { Car_make = "Toyota", Car_model = "Corolla", Manufacture_year = 2024 }
					}
				};

				var capitalMotors = new Dealership
				{
					License = "KYIV-002",
					Name = "Capital Motors",
					City = "Kyiv",
					Phone = "+380445556677",
					Companies = new List<Company> { toyota, kia },
					Cars = new List<Car>
					{
						new Car { Car_make = "Kia", Car_model = "Sportage", Manufacture_year = 2024 },
						new Car { Car_make = "Toyota", Car_model = "RAV4", Manufacture_year = 2023 }
					}
				};

				var lvivDealership = new Dealership
				{
					License = "LVIV-001",
					Name = "West Auto",
					City = "Lviv",
					Phone = "+380322223344",
					Companies = new List<Company> { hyundai, toyota, skoda },
					Cars = new List<Car>
					{
						new Car { Car_make = "Hyundai", Car_model = "Tucson", Manufacture_year = 2024 },
						new Car { Car_make = "Toyota", Car_model = "Corolla", Manufacture_year = 2021 },
						new Car { Car_make = "Skoda", Car_model = "Kodiaq", Manufacture_year = 2023 }
					}
				};

				var odesaDealership = new Dealership
				{
					License = "ODESA-001",
					Name = "South Drive",
					City = "Odesa",
					Phone = "+380487778899",
					Companies = new List<Company> { volkswagen, skoda, kia },
					Cars = new List<Car>
					{
						new Car { Car_make = "Volkswagen", Car_model = "Golf", Manufacture_year = 2020 },
						new Car { Car_make = "Kia", Car_model = "Sportage", Manufacture_year = 2023 },
						new Car { Car_make = "Skoda", Car_model = "Octavia", Manufacture_year = 2022 }
					}
				};

				var dniproDealership = new Dealership
				{
					License = "DNIPRO-001",
					Name = "East Auto",
					City = "Dnipro",
					Phone = "+380562223344",
					Companies = new List<Company> { hyundai, kia },
					Cars = new List<Car>
					{
						new Car { Car_make = "Hyundai", Car_model = "Elantra", Manufacture_year = 2023 },
						new Car { Car_make = "Kia", Car_model = "Sorento", Manufacture_year = 2022 },
						new Car { Car_make = "Hyundai", Car_model = "Tucson", Manufacture_year = 2021 }
					}
				};

				var kharkivDealership = new Dealership
				{
					License = "KHARKIV-001",
					Name = "Drive Center",
					City = "Kharkiv",
					Phone = "+380577778899",
					Companies = new List<Company> { volkswagen, skoda },
					Cars = new List<Car>
					{
						new Car { Car_make = "Volkswagen", Car_model = "Passat", Manufacture_year = 2021 },
						new Car { Car_make = "Skoda", Car_model = "Scala", Manufacture_year = 2024 }
					}
				};

				dbContext.Dealerships.Add(kyivDealership);
				dbContext.Dealerships.Add(capitalMotors);
				dbContext.Dealerships.Add(lvivDealership);
				dbContext.Dealerships.Add(odesaDealership);
				dbContext.Dealerships.Add(dniproDealership);
				dbContext.Dealerships.Add(kharkivDealership);
				dbContext.SaveChanges();
			}
		}
	}
}
