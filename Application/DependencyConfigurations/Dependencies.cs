using Application.Common.FluentValidation;
using Application.Common.TokenService;
using Application.UseCases.Authentication.Commands;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Application.DependencyConfigurations {
	public static class Dependencies {

		public static IServiceCollection AddApplicationLayer(this IServiceCollection services, IConfiguration configuration) {

			services.ConfigureMediatR();
			services.ConfigureFluentValidation();
			services.ConfigureAutoMapper();
			services.ConfigureScopedServices();

			return services;
		}

		private static void ConfigureMediatR(this IServiceCollection services) {
			services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
		}

		private static void ConfigureFluentValidation(this IServiceCollection services) {
			services.AddScoped(typeof(IPipelineBehavior<,>), typeof(FluentValidationPipelineBehaviour<,>));
			services.AddValidatorsFromAssemblyContaining<RegisterCommand>(includeInternalTypes: true);
		}

		private static void ConfigureAutoMapper(this IServiceCollection services) {
			services.AddAutoMapper(Assembly.GetExecutingAssembly());
		}

		private static void ConfigureScopedServices(this IServiceCollection services) {
			services.AddScoped<ITokenService, TokenService>();
		}
	}
}
