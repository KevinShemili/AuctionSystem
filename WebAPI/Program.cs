using Application.DependencyConfigurations;
using Application.UseCases.AutomaticExpiry;
using FluentValidation.AspNetCore;
using Hangfire;
using Infrastructure.DependencyConfigurations;
using Infrastructure.Hangfire;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Exceptions;
using Serilog.Sinks.Elasticsearch;
using System.Reflection;
using System.Text;
using WebAPI.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
	.AddJsonFile("appsettings.json", optional: true)
	.AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
	.AddEnvironmentVariables();

builder.Services
	.AddApplicationLayer(builder.Configuration)
	.AddInfrastructureLayer(builder.Configuration);

builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpClient();
builder.Services.AddControllers();
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(config => {
	config.EnableAnnotations();

	config.SwaggerDoc("v1", new OpenApiInfo {
		Title = "Auction System",
		Version = "v1"
	});

	config.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme {
		In = ParameterLocation.Header,
		Description = "JWT Authorization",
		Name = "Authorization",
		Type = SecuritySchemeType.Http,
		Scheme = "bearer",
		BearerFormat = "JWT"
	});

	config.AddSecurityRequirement(new OpenApiSecurityRequirement
	{
		{
			new OpenApiSecurityScheme {
				Reference = new OpenApiReference
				{
					Type = ReferenceType.SecurityScheme,
					Id = "Bearer"
				}
			},
			new List<string>()
		}
	});
});

builder.Services.AddAuthorization();
builder.Services.AddAuthentication(x => {
	x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
	x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
	x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(x => {
	x.RequireHttpsMetadata = false;
	x.SaveToken = true;
	x.TokenValidationParameters = new TokenValidationParameters {
		ValidateIssuer = false,
		ValidateAudience = false,
		ValidateLifetime = true,
		ValidateIssuerSigningKey = true,
		ValidIssuer = builder.Configuration["JWTSettings:Issuer"],
		ValidAudience = builder.Configuration["JWTSettings:Audience"],
		IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(builder.Configuration["JWTSettings:Secret"]!)),
		ClockSkew = TimeSpan.Zero,
		RequireExpirationTime = true
	};
});

builder.Services.AddProblemDetails();

ConfigureLogging();

builder.Host.UseSerilog();

var app = builder.Build();
ApplyMigration();

app.UseMiddleware<GlobalExceptionHandler>();

ConfigureSwagger(app);

if (builder.Environment.EnvironmentName != "Testing") {

	app.UseHangfireDashboard("/hangfire", new DashboardOptions {
		Authorization = new[] { new AllowAllDashboardAuthorizationFilter() }
	});

	var recurringJobManager = app.Services.GetRequiredService<IRecurringJobManager>();
	recurringJobManager.AddOrUpdate<IAuctionCloser>(
		"Close Expired Auctions",
		job => job.AutomaticClose(),
		Cron.Minutely()
	);
}

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseStaticFiles();
app.MapControllers();
app.Run();

static void ConfigureSwagger(WebApplication app) {
	app.UseSwagger();
	app.UseSwaggerUI();
}

static void ConfigureLogging() {

	var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

	if (environment == "Docker") {
		var config = new ConfigurationBuilder()
			  .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
			  .AddJsonFile($"appsettings.{environment}.json", optional: true)
			  .Build();

		Log.Logger = new LoggerConfiguration()
			.Enrich.FromLogContext()
			.Enrich.WithEnvironmentName()
			.Enrich.WithMachineName()
			.Enrich.WithExceptionDetails()
			.WriteTo.Debug()
			.WriteTo.Console()
			.WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(config["ElasticConfiguration:Uri"])) {
				AutoRegisterTemplate = true,
				IndexFormat = $"{Assembly.GetExecutingAssembly()?.GetName()?.Name?.ToLower().Replace(".", "-")}-{environment?.ToLower()}-{DateTime.UtcNow:yyyy-MM-dd}",
				NumberOfReplicas = 1,
				NumberOfShards = 2
			})
			.Enrich.WithProperty("Environment", environment)
			.ReadFrom.Configuration(config)
			.CreateLogger();
	}
}

void ApplyMigration() {
	using var scope = app.Services.CreateScope();
	var db = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
	db.Database.Migrate();
}

public partial class Program { }