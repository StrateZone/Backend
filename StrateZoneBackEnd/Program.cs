using CloudinaryDotNet;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Net.payOS;
using StrateZone_APIs.ServiceExtensions;
using StrateZone_Repository.Data;
using StrateZone_Service.Hubs;
using StrateZone_Service.Mapper;
using System.Text;
using System.Text.Json.Serialization;

var MyAllowSpecificOrigins = "myAllowSpecificOrigins";

IConfiguration configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

PayOS payOS = new PayOS(configuration["Environment:PAYOS_CLIENT_ID"] ?? throw new Exception("Cannot find environment"),
                    configuration["Environment:PAYOS_API_KEY"] ?? throw new Exception("Cannot find environment"),
                    configuration["Environment:PAYOS_CHECKSUM_KEY"] ?? throw new Exception("Cannot find environment"));

var builder = WebApplication.CreateBuilder(args);

//*****************************************************************************************************************
// Add services to the container.
//*****************************************************************************************************************

//Config for Railway
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
builder.WebHost.UseUrls($"http://*:{port}");

//Add health check
builder.Services.AddHealthChecks();

builder.Services.AddControllers()
     .AddJsonOptions(options =>
     {
         options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
     });
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add dbcontext
builder.Services.AddDbContext<StrateZoneDbContext>();

//Http Context Accessor
builder.Services.AddHttpContextAccessor();

// Auto Mapper
builder.Services.AddAutoMapper(typeof(MappingProfiles).Assembly);

// Load Cloudinary settings
var cloudinarySettings = builder.Configuration.GetSection("Cloudinary");
var account = new Account(
    cloudinarySettings["CloudName"],
    cloudinarySettings["ApiKey"],
    cloudinarySettings["ApiSecret"]
);
builder.Services.AddSingleton(new Cloudinary(account));

builder.Services.AddSingleton(payOS);

// Seed data
//builder.Services.AddTransient<Seed>();

// Swagger configuration for security
builder.Services.AddSwaggerGen(option =>
{
    option.SwaggerDoc("v1", new OpenApiInfo { Title = "StrateZone_APIs", Version = "v1" });
    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });
    option.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            new string[]{}
        }
    });
});

// Identity, authentication and authorization

var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
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
            IssuerSigningKey = new SymmetricSecurityKey(secretKey)
        };
    });
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    options.AddPolicy("StaffOnly", policy => policy.RequireRole("Staff"));
    options.AddPolicy("InstructorOrAbove", policy => policy.RequireRole("Instructor", "Staff", "Admin"));
});
// Service Extensions
builder.Services.AddApplicationServices();
var app = builder.Build();

app.UseHealthChecks("/health");

app.UseSwagger();
app.UseSwaggerUI();

app.UseCors(MyAllowSpecificOrigins);

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapHub<ChatHub>("/chatHub");
app.MapControllers();

app.Run();
