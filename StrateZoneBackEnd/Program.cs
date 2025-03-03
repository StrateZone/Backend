using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Net.payOS;
using Npgsql;
using StrateZone_APIs.ServiceExtensions;
using StrateZone_Repository.Data;
using StrateZone_Service.Implements;
using StrateZone_Service.Mapper;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;
using static StrateZone_Repository.Parameters.PostgreEnums;

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

//Config GHN
builder.Services.AddHttpClient<GHNService>();

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

// Auto Mapper
builder.Services.AddAutoMapper(typeof(MappingProfiles).Assembly);

// Add payOS
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
/*
builder.Services.AddIdentity<User, Role>(
        options =>
        {
            options.Password.RequireDigit = false;
            options.Password.RequireUppercase = false;
            options.Password.RequireLowercase = false;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequiredLength = 3;
        }
    )
    .AddEntityFrameworkStores<MealHuntContext>()
    .AddDefaultTokenProviders();
*/

//builder.Services.AddAuthentication(options =>
//{
//    options.DefaultAuthenticateScheme =
//    options.DefaultChallengeScheme =
//    options.DefaultForbidScheme =
//    options.DefaultScheme =
//    options.DefaultSignInScheme =
//    options.DefaultSignOutScheme = JwtBearerDefaults.AuthenticationScheme;
//})
//    .AddJwtBearer(options =>
//    {
//        options.TokenValidationParameters = new TokenValidationParameters
//        {
//            ValidateIssuer = false,
//            //ValidIssuer = builder.Configuration["JWT:Issuer"],
//            ValidateAudience = false,
//            //ValidAudience = builder.Configuration["JWT:Audience"],
//            RequireExpirationTime = true,
//            ValidateIssuerSigningKey = true,
//            ClockSkew = TimeSpan.Zero,
//            IssuerSigningKey = new SymmetricSecurityKey(
//                        Encoding.UTF8.GetBytes(builder.Configuration["JWT:SigningKey"])
//                    )
//        };
//    });

// Service Extensions
builder.Services.AddApplicationServices();

var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}

app.UseHealthChecks("/health");

app.UseSwagger();
app.UseSwaggerUI();

app.UseCors(MyAllowSpecificOrigins);

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
