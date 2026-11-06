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
	[RoutePrefix("cars")]
	public class CarController : Controller
	{
		private readonly IGenericRepository<Car> _carRepository;
		private readonly IGenericRepository<Dealership> _dealershipRepository;

		public CarController()
		{
			var dbContext = new CarDealershipDbEntities();
			_carRepository = new GenericRepository<Car>(dbContext);
			_dealershipRepository = new GenericRepository<Dealership>(dbContext);
		}

		[HttpGet]
		[Route("", Name = "CarIndex")]
		public async Task<ActionResult> Index()
		{
			var cars = await _carRepository.GetAllAsync();

			if (!cars.Any())
			{
				return HttpNotFound();
			}

			return View(cars.Select(car => new CarViewModel
			{
				Car_id = car.Car_id,
				Car_model = car.Car_model,
				Car_make = car.Car_make,
				Manufacture_year = car.Manufacture_year
			}));
		}

		[HttpGet]
		[Route("{carId:int}", Name = "CarDetails")]
		public async Task<ActionResult> Details(int carId)
		{
			var car = await _carRepository.GetByIdAsync(carId);

			if (car == null)
			{
				return HttpNotFound();
			}

			return View(new CarViewModel
			{
				Car_id = car.Car_id,
				Car_model = car.Car_model,
				Car_make = car.Car_make,
				Manufacture_year = car.Manufacture_year,
				Dealership_id = car.Dealership_id,
				DealershipName = car.Dealership.Name,
				DealershipCity = car.Dealership.City
			});
		}

		[HttpGet]
		[Route("create", Name = "CarCreate")]
		public async Task<ActionResult> Create()
		{
			return View(new CarViewModel
			{
				Dealerships = await GetDealershipOptionsAsync()
			});
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[Route("create", Name = "CarCreatePost")]
		public async Task<ActionResult> Create(CarViewModel viewModel)
		{
			var dealershipExists = await _dealershipRepository.GetByIdAsync(viewModel.Dealership_id) != null;

			if (!ModelState.IsValid || !dealershipExists)
			{
				if (!dealershipExists)
				{
					ModelState.AddModelError("Dealership_id", "The selected dealership does not exist.");
				}

				viewModel.Dealerships = await GetDealershipOptionsAsync();
				return View(viewModel);
			}

			var newCar = new Car
			{
				Car_make = viewModel.Car_make,
				Car_model = viewModel.Car_model,
				Manufacture_year = viewModel.Manufacture_year,
				Dealership_id = viewModel.Dealership_id
			};

			try
			{
				_carRepository.Add(newCar);
				await _carRepository.SaveChangesAsync();
			}
			catch (DbException ex)
			{
				Console.WriteLine(ex.Message);
				ModelState.AddModelError("", "Could not add car");
				viewModel.Dealerships = await GetDealershipOptionsAsync();
				return View(viewModel);
			}

			return RedirectToRoute("CarIndex");
		}

		[HttpGet]
		[Route("edit/{carId:int}", Name = "CarEdit")]
		public async Task<ActionResult> Edit(int carId)
		{
			var car = await _carRepository.GetByIdAsync(carId);

			if (car == null)
			{
				return HttpNotFound();
			}

			return View(new CarViewModel
			{
				Car_id = car.Car_id,
				Car_make = car.Car_make,
				Car_model = car.Car_model,
				Manufacture_year = car.Manufacture_year,
				Dealership_id = car.Dealership_id,
				Dealerships = await GetDealershipOptionsAsync()
			});
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[Route("edit/{carId:int}", Name = "CarEditPost")]
		public async Task<ActionResult> Edit(int carId, CarViewModel viewModel)
		{
			if (carId != viewModel.Car_id)
			{
				return HttpNotFound();
			}

			if (!ModelState.IsValid)
			{
				viewModel.Dealerships = await GetDealershipOptionsAsync();
				return View(viewModel);
			}

			var carToUpdate = await _carRepository.GetByIdAsync(viewModel.Car_id);

			if (carToUpdate is null)
			{
				return HttpNotFound();
			}

			carToUpdate.Car_make = viewModel.Car_make;
			carToUpdate.Car_model = viewModel.Car_model;
			carToUpdate.Manufacture_year = viewModel.Manufacture_year;
			carToUpdate.Dealership_id = viewModel.Dealership_id;

			await _carRepository.SaveChangesAsync();
			return RedirectToRoute("CarIndex");
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[Route("delete/{carId:int}", Name = "CarDelete")]
		public async Task<ActionResult> Delete(int carId)
		{
			var carToDelete = await _carRepository.GetByIdAsync(carId);

			if (carToDelete is null)
			{
				return HttpNotFound();
			}

			_carRepository.Remove(carToDelete);
			await _carRepository.SaveChangesAsync();
			return RedirectToRoute("CarIndex");
		}

		private async Task<IEnumerable<SelectListItem>> GetDealershipOptionsAsync()
		{
			var dealerships = await _dealershipRepository.GetAllAsync();

			return dealerships
				.OrderBy(d => d.Name)
				.Select(d => new SelectListItem
				{
					Text = d.Name,
					Value = d.Dealership_id.ToString()
				})
				.ToList();
		}
	}
}
