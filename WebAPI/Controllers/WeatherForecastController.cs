using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers {

	[ApiController]
	[Route("[controller]")]
	public class WeatherForecastController : ControllerBase {

		private readonly DatabaseContext _dbContext;

		private static readonly string[] Summaries = new[]
		{
			"Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
		};

		private readonly ILogger<WeatherForecastController> _logger;

		public WeatherForecastController(ILogger<WeatherForecastController> logger, DatabaseContext dbContext) {
			_logger = logger;
			_dbContext = dbContext;
		}

		[HttpGet(Name = "GetWeatherForecast")]
		public IEnumerable<WeatherForecast> Get() {

			_dbContext.Permissions.Add(new Permission {
				Key = "Key",
				Name = "Test",
				Description = "Test",
			});

			_dbContext.SaveChanges();


			return Enumerable.Range(1, 5).Select(index => new WeatherForecast {
				Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
				TemperatureC = Random.Shared.Next(-20, 55),
				Summary = Summaries[Random.Shared.Next(Summaries.Length)]
			})
			.ToArray();
		}
	}
}
