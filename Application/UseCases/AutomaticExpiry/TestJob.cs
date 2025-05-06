using Microsoft.Extensions.Logging;

namespace Application.UseCases.AutomaticExpiry {
	public class TestJob : ITestJob {

		private readonly ILogger<TestJob> _logger;
		public TestJob(ILogger<TestJob> logger) => _logger = logger;

		public Task ExecuteAsync() {
			_logger.LogInformation("TEST JOB RAN AT {TIME}", DateTime.UtcNow);
			return Task.CompletedTask;
		}
	}
}
