using CarDealership.Business.Dtos.Car;
using CarDealership.Business.Services;
using CarDealership.Core.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CarDealership.Core.Controllers;

public class CarController(ICarService carService, IDealershipService dealershipService) : Controller
{
	[HttpGet]
	public async Task<IActionResult> Index()
	{
		var cars = await carService.GetAllCars();
		return View(cars.Select(MapToViewModel).ToList());
	}

	[HttpGet]
	public async Task<IActionResult> Statistics()
	{
		var statistics = await carService.GetCityCarStatistics();
		return View(statistics.Select(MapToViewModel).ToList());
	}

	[HttpGet]
	public async Task<IActionResult> Details(int id)
	{
		var car = await carService.GetCarById(id);
		if (car is null)
		{
			return NotFound();
		}

		var dealership = await dealershipService.GetDealershipById(car.DealershipId);
		var viewModel = MapToViewModel(car);
		viewModel.DealershipName = dealership?.Name;
		viewModel.DealershipCity = dealership?.City;

		return View(viewModel);
	}

	[HttpGet]
	public async Task<IActionResult> Create()
	{
		return View(new CarViewModel
		{
			DealershipOptions = await GetDealershipOptionsAsync()
		});
	}

	[HttpPost]
	public async Task<IActionResult> Create(CarViewModel viewModel)
	{
		if (!ModelState.IsValid)
		{
			viewModel.DealershipOptions = await GetDealershipOptionsAsync();
			return View(viewModel);
		}

		var createdCar = await carService.AddCar(new CreateCarRequest
		{
			Make = viewModel.Make,
			Model = viewModel.Model,
			ManufactureYear = viewModel.ManufactureYear,
			DealershipId = viewModel.DealershipId
		});

		if (createdCar is null)
		{
			ModelState.AddModelError(string.Empty, "Unable to create the car. Check the entered data and dealership.");
			viewModel.DealershipOptions = await GetDealershipOptionsAsync();
			return View(viewModel);
		}

		TempData["Success"] = "Car created.";
		return RedirectToAction(nameof(Index));
	}

	[HttpGet]
	public async Task<IActionResult> Edit(int id)
	{
		var car = await carService.GetCarById(id);
		if (car is null)
		{
			return NotFound();
		}

		var viewModel = MapToViewModel(car);
		viewModel.DealershipOptions = await GetDealershipOptionsAsync();
		return View(viewModel);
	}

	[HttpPost]
	public async Task<IActionResult> Edit(int id, CarViewModel viewModel)
	{
		if (id != viewModel.Id)
		{
			return NotFound();
		}

		if (!ModelState.IsValid)
		{
			viewModel.DealershipOptions = await GetDealershipOptionsAsync();
			return View(viewModel);
		}

		if (await carService.GetCarById(id) is null)
		{
			return NotFound();
		}

		var isUpdated = await carService.UpdateCar(new UpdateCarRequest
		{
			Id = viewModel.Id,
			Make = viewModel.Make,
			Model = viewModel.Model,
			ManufactureYear = viewModel.ManufactureYear,
			DealershipId = viewModel.DealershipId
		});

		if (!isUpdated)
		{
			ModelState.AddModelError(string.Empty, "Unable to update the car. Check the entered data and dealership.");
			viewModel.DealershipOptions = await GetDealershipOptionsAsync();
			return View(viewModel);
		}

		TempData["Success"] = "Car updated.";
		return RedirectToAction(nameof(Details), new { id });
	}

	[HttpPost]
	public async Task<IActionResult> Delete(int id)
	{
		if (!await carService.DeleteCar(id))
		{
			return NotFound();
		}

		TempData["Success"] = "Car deleted.";
		return RedirectToAction(nameof(Index));
	}

	private async Task<IEnumerable<SelectListItem>> GetDealershipOptionsAsync()
	{
		var dealerships = await dealershipService.GetAllDealerships();

		return [.. dealerships
			.OrderBy(dealership => dealership.Name)
			.Select(dealership => new SelectListItem
			{
				Text = dealership.Name,
				Value = dealership.Id.ToString()
			})];
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
