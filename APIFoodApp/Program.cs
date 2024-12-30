using APIFoodApp.Models;
using CloudinaryDotNet;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
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
// C?u hình d?ch v? JSON cho các controller
builder.Services.AddControllers().AddJsonOptions(options =>
{
	options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
	options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
});
// C?u hình chu?i k?t n?i ??n c? s? d? li?u
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<FoodAppContext>(options =>
	options.UseSqlServer(connectionString));
// Cấu hình JWT
var jwtSettings = builder.Configuration.GetSection("JWTSettings");
var key = Encoding.UTF8.GetBytes(jwtSettings["Secret"]);

builder.Services.AddAuthentication(options =>
{
	options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
	options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
	options.TokenValidationParameters = new TokenValidationParameters
	{
		ValidateIssuer = true,
		ValidateAudience = true,
		ValidateLifetime = true,
		ValidateIssuerSigningKey = true,
		ValidIssuer = jwtSettings["Issuer"],
		ValidAudience = jwtSettings["Audience"],
		IssuerSigningKey = new SymmetricSecurityKey(key)
	};
});

builder.Services.AddAuthorization();
// C?u hình Swagger
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
	// Cấu hình JWT trong Swagger
	options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
	{
		Name = "Authorization",
		Type = SecuritySchemeType.Http,
		Scheme = "Bearer",
		BearerFormat = "JWT",
		In = ParameterLocation.Header,
		Description = "Nhập 'Bearer {token}' để xác thực API."
	});

	options.AddSecurityRequirement(new OpenApiSecurityRequirement
	{
		{
			new OpenApiSecurityScheme
			{
				Reference = new OpenApiReference
				{
					Type = ReferenceType.SecurityScheme,
					Id = "Bearer"
				}
			},
			new string[] {}
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

// C?u hình ?? s? d?ng Swagger và Swagger UI
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI(c =>
	{
		c.SwaggerEndpoint("/swagger/v1/swagger.json", "My APIQLFoodApp");
		// c.RoutePrefix = string.Empty; // ??t Swagger UI là trang chính khi truy c?p vào root c?a ?ng d?ng
	});
}
// Configure the HTTP request pipeline.

app.UseAuthorization();

app.MapControllers();

app.Run();
