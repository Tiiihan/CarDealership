using CarDealershipFramework.Models;
using CarDealershipFramework.ViewModels;
using System;
using System.Data.Common;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CarDealershipFramework.Controllers
{
	[RoutePrefix("cars")]
	public class CarController : Controller
	{
		private readonly CarDealershipDbEntities _dbContext;

		public CarController()
		{
			_dbContext = new CarDealershipDbEntities();
		}

		[HttpGet]
		[Route("", Name = "CarIndex")]
		public ActionResult Index()
		{
			var cars = _dbContext.Cars;

			if (cars == null || !cars.Any())
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
			var car = await _dbContext.Cars.FirstOrDefaultAsync(c => c.Car_id == carId);

			if (car == null)
			{
				return HttpNotFound();
			}

			return View(new CarViewModel { Car_model = car.Car_model, Car_make = car.Car_make, Manufacture_year = car.Manufacture_year });
		}

		[HttpGet]
		[Route("create", Name = "CarCreate")]
		public ActionResult Create()
		{
			var dealerships = _dbContext.Dealerships.Select(d => new SelectListItem
			{
				Text = d.Name,
				Value = d.Dealership_id.ToString(),
			});

			if (dealerships is null)
			{
				return View();
			}

			return View(new CarViewModel { Dealerships = dealerships });
		}

		[HttpPost]
		[Route("create", Name = "CarCreatePost")]
		public async Task<ActionResult> Create(CarViewModel viewModel)
		{
			var isDealerExist = !(_dbContext.Dealerships.FirstOrDefault(d => d.Dealership_id == viewModel.Dealership_id) is null);

			if (!ModelState.IsValid || !isDealerExist)
			{
				var dealerships = _dbContext.Dealerships.Select(d => new SelectListItem
				{
					Text = d.Name,
					Value = d.Dealership_id.ToString(),
				});

				viewModel.Dealerships = dealerships;

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
				_dbContext.Cars.Add(newCar);
				await _dbContext.SaveChangesAsync();
			}
			catch (DbException ex)
			{
				Console.WriteLine(ex.Message);

				ModelState.AddModelError("", "Could not add car");

				viewModel.Dealerships = _dbContext.Dealerships.Select(d => new SelectListItem
				{
					Text = d.Name,
					Value = d.Dealership_id.ToString(),
				});

				return View(viewModel);
			}

			return RedirectToRoute("CarIndex");
		}

		[HttpGet]
		[Route("edit/{carId:int}", Name = "CarEdit")]
		public async Task<ActionResult> Edit(int carId)
		{
			var car = await _dbContext.Cars.FirstOrDefaultAsync(c => c.Car_id == carId);

			if (car is null)
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
				Dealerships = _dbContext.Dealerships.Select(d => new SelectListItem
				{
					Text = d.Name,
					Value = d.Dealership_id.ToString(),
				}),
			});
		}

		[HttpPost]
		[Route("edit/{carId:int}", Name = "CarEditPost")]
		public async Task<ActionResult> Edit(int carId, CarViewModel viewModel)
		{
			if (carId != viewModel.Car_id)
			{
				return HttpNotFound();
			}

			if (!ModelState.IsValid)
			{
				viewModel.Dealerships = _dbContext.Dealerships.Select(d => new SelectListItem
				{
					Text = d.Name,
					Value = d.Dealership_id.ToString(),
				});

				return View(viewModel);
			}

			var carToUpdate = await _dbContext.Cars.FirstOrDefaultAsync(c => c.Car_id == viewModel.Car_id);

			if (carToUpdate is null)
			{
				return HttpNotFound();
			}

			carToUpdate.Car_make = viewModel.Car_make;
			carToUpdate.Car_model = viewModel.Car_model;
			carToUpdate.Manufacture_year = viewModel.Manufacture_year;
			carToUpdate.Dealership_id = viewModel.Dealership_id;

			await _dbContext.SaveChangesAsync();

			return RedirectToRoute("CarIndex");
		}

		[HttpPost]
		[Route("delete/{carId:int}", Name = "CarDelete")]
		public async Task<ActionResult> Delete(int carId)
		{
			var carToDelete = await _dbContext.Cars.FirstOrDefaultAsync(c => c.Car_id == carId);

			if (carToDelete is null)
			{
				return HttpNotFound();
			}

			_dbContext.Cars.Remove(carToDelete);
			await _dbContext.SaveChangesAsync();

			return RedirectToAction("Index");
		}
	}
}
