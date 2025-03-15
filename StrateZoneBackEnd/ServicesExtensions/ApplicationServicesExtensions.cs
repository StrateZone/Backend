using StrateZone_Repository.Implements;
using StrateZone_Repository.Interfaces;
using StrateZone_Service.Hubs;
using StrateZone_Service.Implements;
using StrateZone_Service.Interfaces;

namespace StrateZone_APIs.ServiceExtensions
{
    public static class ApplicationServicesExtensions
    {
        const string MyAllowSpecificOrigins = "myAllowSpecificOrigins";

        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddSignalR();

            // Add your application services here
            services
                .AddRepositories()
                .AddServices()
                .AddCorsConfiguration()
                .AddSerControllers()
                ;

			return services;
        }

        private static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            // Add your repositories here
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IGameTypeRepository, GameTypeRepository>();
            services.AddScoped<IGameExtensionRepository, GameExtensionRepository>();
            services.AddScoped<IAppointmentRepository, AppointmentRepository>();
            services.AddScoped<ITablesAppointmentRepository, TablesAppointmentRepository>();
            services.AddScoped<ITableRepository, TableRepository>();
            services.AddScoped<IRoomRepository, RoomRepository>();
            services.AddScoped<IMessageRepository, MessageRepository>();
            services.AddScoped<IPriceRepository, PriceRepository>();
            return services;
        }

        private static IServiceCollection AddServices(this IServiceCollection services)
        {
            // Add your services here
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IGameTypeService, GameTypeService>();
            services.AddScoped<IGameExtensionService, GameExtensionService>();
            services.AddScoped<IAppointmentService, AppointmentService>();
            services.AddScoped<ITablesAppointmentService, TablesAppointmentService>();
            services.AddScoped<ITableService, TableService>();
            services.AddScoped<TokenService>();
            services.AddScoped<IRoomService, RoomService>();
            services.AddScoped<IMessageService, MessageService>();
            services.AddScoped<IPriceService, PriceService>();

            services.AddHttpClient<IGHNService, GHNService>();
            services.AddScoped<IEmailService, EmailService>();
            return services;
        }

        private static IServiceCollection AddCorsConfiguration(this IServiceCollection services)
        {
            // CORS config
            services.AddCors(options =>
            {
                options.AddPolicy(MyAllowSpecificOrigins,
                        policy =>
                        {
                            policy.AllowAnyOrigin()
                            .AllowAnyHeader()
                            .AllowAnyMethod();
                        }
                    );
            });

            return services;
        }

        private static IServiceCollection AddSerControllers(this IServiceCollection services)
        {
			services.AddControllers()
				.AddNewtonsoftJson(options =>
				{
					// Configure JSON options to handle circular references
					options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Error;
				});

			return services;
		}
    }
}
