using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Middleware {
	public class GlobalExceptionHandler {

		private readonly RequestDelegate _next;
		private readonly IHostEnvironment _environment;
		private readonly ILogger<GlobalExceptionHandler> _logger;

		public GlobalExceptionHandler(RequestDelegate next,
									  IHostEnvironment environment,
									  ILogger<GlobalExceptionHandler> logger) {
			_next = next;
			_environment = environment;
			_logger = logger;
		}

		// Method is invoked on every HTTP request in the pipeline
		public async Task InvokeAsync(HttpContext context) {
			try {
				// If no exception, pass control to the next middleware
				await _next(context);
			}
			catch (Exception ex) {
				// If there is an exception process them in an uniform manner
				await HandleExceptionAsync(context, ex);
			}
		}

		private async Task HandleExceptionAsync(HttpContext context, Exception exception) {

			var problemDetails = new ProblemDetails();

			// Include stack trace only in development & docker environments
			if (_environment.IsDevelopment() || _environment.EnvironmentName == "Docker") {
				problemDetails.Extensions["StackTrace"] = exception.StackTrace;
				problemDetails.Detail = exception.Message;
			}

			switch (exception) {
				case ValidationException:
				problemDetails.Status = StatusCodes.Status400BadRequest;
				context.Response.StatusCode = StatusCodes.Status400BadRequest;
				problemDetails.Title = "Validation Failure";
				_logger.LogWarning(exception, "Validation failure occurred.");
				break;

				case UnauthorizedAccessException:
				problemDetails.Status = StatusCodes.Status401Unauthorized;
				context.Response.StatusCode = StatusCodes.Status401Unauthorized;
				problemDetails.Title = "Unauthorized Access";
				_logger.LogWarning(exception, "Unauthorized access attempt.");
				break;

				default:
				problemDetails.Status = StatusCodes.Status500InternalServerError;
				context.Response.StatusCode = StatusCodes.Status500InternalServerError;
				problemDetails.Title = "Internal Server Error";
				_logger.LogError(exception, "Unhandled exception occurred.");
				break;
			}

			// Write as json response
			await context.Response.WriteAsJsonAsync(problemDetails);
		}
	}
}