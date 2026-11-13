namespace CarDealershipFramework.ViewModels
{
	public class CityCarStatisticsViewModel
	{
		public string City { get; set; } = string.Empty;
		public int DealershipCount { get; set; }
		public int CarCount { get; set; }
		public int AverageManufactureYear { get; set; }
		public int NewestManufactureYear { get; set; }
		public int OldestManufactureYear { get; set; }
		public string MostPopularMake { get; set; } = string.Empty;
	}
}
