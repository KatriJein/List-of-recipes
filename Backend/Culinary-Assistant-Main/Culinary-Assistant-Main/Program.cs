using Core.Minio;
using Core.Serilog;
using Culinary_Assistant.Core.Constants;
using Culinary_Assistant.Core.Options;
using Culinary_Assistant.Core.Shared.Middlewares;
using Culinary_Assistant_Main.Infrastructure;
using Culinary_Assistant_Main.Infrastructure.Mappers;
using Culinary_Assistant_Main.Infrastructure.Startups;
using Culinary_Assistant_Main.Services.Receipts;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Microsoft.IdentityModel.Tokens;
using System.Reflection;
using System.Text;
using Culinary_Assistant.Core.Redis;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<ConnectionOptions>(builder.Configuration.GetSection(ConfigurationConstants.PostgreSQL));
builder.Services.Configure<AmazonS3Options>(builder.Configuration.GetSection(ConfigurationConstants.AmazonS3));

builder.Services.AddCors(setup =>
{
	setup.AddPolicy(ConfigurationConstants.FrontendPolicy, config =>
	{
		config
	   .WithOrigins(builder.Configuration[ConfigurationConstants.FrontendHost]!)
	   .AllowAnyHeader()
	   .AllowAnyMethod()
	   .AllowCredentials();
	});
});

builder.Host.AddSerilog();
builder.Services.AddDbContext<CulinaryAppContext>();
builder.Services.AddAutoMapper(typeof(CulinaryAppMapper).Assembly);
builder.Services.AddDomain();
builder.Services.AddCustomServices();
builder.Services.AddProblemDetails();
builder.Services.AddControllers().AddNewtonsoftJson(config =>
{
	config.SerializerSettings.Converters.Add(new StringEnumConverter(typeof(CamelCaseNamingStrategy)));
	config.SerializerSettings.Formatting = Newtonsoft.Json.Formatting.Indented;
	config.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(setup =>
{
	var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
	var xmlFilePath = Path.Combine(AppContext.BaseDirectory, xmlFile);
	setup.IncludeXmlComments(xmlFilePath);
	setup.SupportNonNullableReferenceTypes();
});
builder.Services.AddSwaggerGenNewtonsoftSupport();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseCors(ConfigurationConstants.FrontendPolicy);

app.UseMiddleware<ExceptionMiddleware>();

app.UseAuthorization();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
	var dbContext = scope.ServiceProvider.GetRequiredService<CulinaryAppContext>();
	await dbContext.Database.MigrateAsync();
}

app.Run();
