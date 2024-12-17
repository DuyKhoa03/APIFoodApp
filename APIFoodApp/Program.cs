using APIFoodApp.Models;
using CloudinaryDotNet;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.OpenApi.Models;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton(provider =>
{
	var config = provider.GetRequiredService<IConfiguration>();
	var cloudName = config["Cloudinary:CloudName"];
	var apiKey = config["Cloudinary:ApiKey"];
	var apiSecret = config["Cloudinary:ApiSecret"];

	return new Cloudinary(new Account(cloudName, apiKey, apiSecret));
});
// Add services to the container.
// C?u h�nh d?ch v? JSON cho c�c controller
builder.Services.AddControllers().AddJsonOptions(options =>
{
	options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
	options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
});
// C?u h�nh chu?i k?t n?i ??n c? s? d? li?u
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<FoodAppContext>(options =>
	options.UseSqlServer(connectionString));

// C?u h�nh Swagger
builder.Services.AddSwaggerGen(options =>
{
	options.SwaggerDoc("v1", new OpenApiInfo
	{
		Version = "v1",
		Title = "My API",
		Description = "An ASP.NET Core Web API for managing resources",
		TermsOfService = new Uri("https://example.com/terms"),
		Contact = new OpenApiContact
		{
			Name = "Your Name",
			Email = "youremail@example.com",
			Url = new Uri("https://example.com/contact"),
		},
		License = new OpenApiLicense
		{
			Name = "Use under LICX",
			Url = new Uri("https://example.com/license"),
		}
	});
});
builder.Services.AddCors(options =>
{
	options.AddPolicy("AllowAll", policy =>
	{
		policy.AllowAnyOrigin()
			  .AllowAnyMethod()
			  .AllowAnyHeader();
	});
});


var app = builder.Build();
app.UseCors("AllowAll");

// C?u h�nh ?? s? d?ng Swagger v� Swagger UI
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI(c =>
	{
		c.SwaggerEndpoint("/swagger/v1/swagger.json", "My APIQLFoodApp");
		// c.RoutePrefix = string.Empty; // ??t Swagger UI l� trang ch�nh khi truy c?p v�o root c?a ?ng d?ng
	});
}
// Configure the HTTP request pipeline.

app.UseAuthorization();

app.MapControllers();

app.Run();
