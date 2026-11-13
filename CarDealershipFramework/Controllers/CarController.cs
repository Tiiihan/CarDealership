using CarDealership.Business.Dtos.Car;
using CarDealership.Business.Services;
using CarDealershipFramework.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CarDealershipFramework.Controllers
{
	[RoutePrefix("cars")]
	public class CarController : Controller
	{
		private readonly ICarService _carService;
		private readonly IDealershipService _dealershipService;

		public CarController(ICarService carService, IDealershipService dealershipService)
		{
			_carService = carService;
			_dealershipService = dealershipService;
		}

		[HttpGet]
		[Route("")]
		public async Task<ActionResult> Index()
		{
			var cars = await _carService.GetAllCars();

			return View(cars.Select(MapToViewModel));
		}

		[HttpGet]
		[Route("statistics")]
		public async Task<ActionResult> Statistics()
		{
			var statistics = await _carService.GetCityCarStatistics();
			return View(statistics.Select(MapToViewModel));
		}

		[HttpGet]
		[Route("{carId:int}")]
		public async Task<ActionResult> Details(int carId)
		{
			var car = await _carService.GetCarById(carId);

			if (car is null)
			{
				return HttpNotFound();
			}

			var dealership = await _dealershipService.GetDealershipById(car.DealershipId);

			var viewModel = MapToViewModel(car);
			viewModel.DealershipName = dealership?.Name;
			viewModel.DealershipCity = dealership?.City;

			return View(viewModel);
		}

		[HttpGet]
		[Route("create")]
		public async Task<ActionResult> Create()
		{
			return View(new CarViewModel
			{
				DealershipOptions = await GetDealershipOptionsAsync()
			});
		}

		[HttpPost]
		[Route("create")]
		public async Task<ActionResult> Create(CarViewModel viewModel)
		{
			if (!ModelState.IsValid)
			{
				viewModel.DealershipOptions = await GetDealershipOptionsAsync();
				return View(viewModel);
			}

			var newCar = new CreateCarRequest
			{
				Make = viewModel.Make,
				Model = viewModel.Model,
				ManufactureYear = viewModel.ManufactureYear,
				DealershipId = viewModel.DealershipId
			};

			var createdCar = await _carService.AddCar(newCar);
			if (createdCar is null)
			{
				ModelState.AddModelError(string.Empty, "Unable to create the car. Check the entered data and dealership.");
				viewModel.DealershipOptions = await GetDealershipOptionsAsync();
				return View(viewModel);
			}

			return RedirectToAction("Index");
		}

		[HttpGet]
		[Route("edit/{carId:int}")]
		public async Task<ActionResult> Edit(int carId)
		{
			var car = await _carService.GetCarById(carId);

			if (car is null)
			{
				return HttpNotFound();
			}

			var viewModel = MapToViewModel(car);
			viewModel.DealershipOptions = await GetDealershipOptionsAsync();

			return View(viewModel);
		}

		[HttpPost]
		[Route("edit/{carId:int}")]
		public async Task<ActionResult> Edit(int carId, CarViewModel viewModel)
		{
			if (carId != viewModel.Id)
			{
				return HttpNotFound();
			}

			if (!ModelState.IsValid)
			{
				viewModel.DealershipOptions = await GetDealershipOptionsAsync();
				return View(viewModel);
			}

			var existingCar = await _carService.GetCarById(viewModel.Id);

			if (existingCar is null)
			{
				return HttpNotFound();
			}

			var request = new UpdateCarRequest
			{
				Id = viewModel.Id,
				Make = viewModel.Make,
				Model = viewModel.Model,
				ManufactureYear = viewModel.ManufactureYear,
				DealershipId = viewModel.DealershipId
			};

			var isUpdated = await _carService.UpdateCar(request);
			if (!isUpdated)
			{
				ModelState.AddModelError(string.Empty, "Unable to update the car. Check the entered data and dealership.");
				viewModel.DealershipOptions = await GetDealershipOptionsAsync();
				return View(viewModel);
			}

			return RedirectToAction("Index");
		}

		[HttpPost]
		[Route("delete/{carId:int}")]
		public async Task<ActionResult> Delete(int carId)
		{
			await _carService.DeleteCar(carId);

			return RedirectToAction("Index");
		}

		private async Task<IEnumerable<SelectListItem>> GetDealershipOptionsAsync()
		{
			var dealerships = await _dealershipService.GetAllDealerships();

			return dealerships.OrderBy(d => d.Name)
							  .Select(d => new SelectListItem
							  {
								  Text = d.Name,
								  Value = d.Id.ToString()
							  })
							  .ToList();
		}

		private static CarViewModel MapToViewModel(CarDto car)
		{
			return new CarViewModel
			{
				Id = car.Id,
				Make = car.Make,
				Model = car.Model,
				ManufactureYear = car.ManufactureYear,
				DealershipId = car.DealershipId
			};
		}

		private static CityCarStatisticsViewModel MapToViewModel(CityCarStatisticsDto statistics)
		{
			return new CityCarStatisticsViewModel
			{
				City = statistics.City,
				DealershipCount = statistics.DealershipCount,
				CarCount = statistics.CarCount,
				AverageManufactureYear = statistics.AverageManufactureYear,
				NewestManufactureYear = statistics.NewestManufactureYear,
				OldestManufactureYear = statistics.OldestManufactureYear,
				MostPopularMake = statistics.MostPopularMake
			};
		}
	}
}
