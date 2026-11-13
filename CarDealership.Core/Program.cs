using CarDealership.Business.Repositories;
using CarDealership.Business.Services;
using CarDealership.Core.Data;
using CarDealership.Core.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

var connectionString = builder.Configuration.GetConnectionString("CarDealershipDb");

if (string.IsNullOrWhiteSpace(connectionString))
{
	throw new InvalidOperationException("Connection string 'CarDealershipDb' is not configured.");
}

builder.Services.AddDbContext<CarDealershipDbContext>(options =>
	options.UseSqlServer(connectionString));

builder.Services.AddScoped<ICarRepository, CarRepository>();
builder.Services.AddScoped<IDealershipRepository, DealershipRepository>();
builder.Services.AddScoped<ICompanyRepository, CompanyRepository>();

builder.Services.AddScoped<ICarService, CarService>();
builder.Services.AddScoped<IDealershipService, DealershipService>();
builder.Services.AddScoped<ICompanyService, CompanyService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
	var dbContext = scope.ServiceProvider.GetRequiredService<CarDealershipDbContext>();
	await DbInitializer.SeedAsync(dbContext);
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Home/Error");
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Car}/{action=Index}/{id?}")
	.WithStaticAssets();


app.Run();
