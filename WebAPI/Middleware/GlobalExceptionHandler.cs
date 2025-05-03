using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Middleware {
	public class GlobalExceptionHandler {

		private readonly RequestDelegate _next;
		private readonly IHostEnvironment _environment;

		//private readonly ILogger<ExceptionHandlingMiddleware> _logger;

		public GlobalExceptionHandler(RequestDelegate next,
								IHostEnvironment environment) {
			_next = next;
			_environment = environment;
		}

		public async Task InvokeAsync(HttpContext context) {
			try {
				await _next(context);
			}
			catch (Exception ex) {
				await HandleExceptionAsync(context, ex);
			}
		}

		private async Task HandleExceptionAsync(HttpContext context, Exception exception) {

			var problemDetails = new ProblemDetails();

			// Include stack trace only in development environment
			if (_environment.IsDevelopment() || _environment.EnvironmentName == "Docker") {
				problemDetails.Extensions["StackTrace"] = exception.StackTrace;
				problemDetails.Detail = exception.Message;
			}

			switch (exception) {
				case ValidationException:
				problemDetails.Status = StatusCodes.Status400BadRequest;
				context.Response.StatusCode = StatusCodes.Status400BadRequest;
				problemDetails.Title = "Validation Failure";
				// Log 
				break;

				case UnauthorizedAccessException:
				problemDetails.Status = StatusCodes.Status401Unauthorized;
				context.Response.StatusCode = StatusCodes.Status401Unauthorized;
				problemDetails.Title = "Unauthorized Access";
				// Log 
				break;

				default:
				problemDetails.Status = StatusCodes.Status500InternalServerError;
				context.Response.StatusCode = StatusCodes.Status500InternalServerError;
				problemDetails.Title = "Internal Server Error";
				// Log
				break;
			}

			await context.Response.WriteAsJsonAsync(problemDetails);
		}
	}
}