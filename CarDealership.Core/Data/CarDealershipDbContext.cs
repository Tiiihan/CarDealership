using CarDealership.Core.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace CarDealership.Core.Data;

public class CarDealershipDbContext(DbContextOptions<CarDealershipDbContext> options) : DbContext(options)
{
	public DbSet<CarEntity> Cars { get; set; }
	public DbSet<DealershipEntity> Dealerships { get; set; }
	public DbSet<CompanyEntity> Companies { get; set; }

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<CompanyEntity>()
			.HasKey(c => c.Id);

		modelBuilder.Entity<CompanyEntity>()
			.HasMany(c => c.Dealerships)
			.WithMany(d => d.Companies);

		modelBuilder.Entity<CompanyEntity>()
			.Property(c => c.Name)
			.HasMaxLength(300)
			.IsRequired();

		modelBuilder.Entity<CompanyEntity>()
			.Property(c => c.Country)
			.HasMaxLength(60);

		modelBuilder.Entity<CompanyEntity>()
			.Property(c => c.EstablishDate)
			.HasColumnType("date");

		modelBuilder.Entity<DealershipEntity>()
			.HasKey(d => d.Id);

		modelBuilder.Entity<DealershipEntity>()
			.HasMany(d => d.Cars)
			.WithOne(c => c.Dealership)
			.HasForeignKey(c => c.DealershipId);

		modelBuilder.Entity<DealershipEntity>()
			.Property(d => d.Name)
			.HasMaxLength(150)
			.IsRequired();

		modelBuilder.Entity<DealershipEntity>()
			.Property(d => d.License)
			.HasMaxLength(50)
			.IsRequired();

		modelBuilder.Entity<DealershipEntity>()
			.Property(d => d.City)
			.HasMaxLength(100)
			.IsRequired();

		modelBuilder.Entity<DealershipEntity>()
			.Property(d => d.Phone)
			.HasMaxLength(25);

		modelBuilder.Entity<CarEntity>()
			.Property(d => d.Make)
			.HasMaxLength(100)
			.IsRequired();

		modelBuilder.Entity<CarEntity>()
			.Property(d => d.Model)
			.HasMaxLength(100)
			.IsRequired();

		base.OnModelCreating(modelBuilder);
	}
}
