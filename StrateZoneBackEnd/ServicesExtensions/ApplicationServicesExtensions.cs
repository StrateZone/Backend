using StrateZone_Repository.Implements;
using StrateZone_Repository.Interfaces;
using StrateZone_Service.Hubs;
using StrateZone_Service.Implements;
using StrateZone_Service.Interfaces;
using StrateZone_Service.Utils;

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
            services.AddScoped<IAppointmentrequestRepository, AppointmentrequestRepository>();
            services.AddScoped<IFriendrequestRepository, FriendrequestRepository>();
            services.AddScoped<IImageRepository, ImageRepository>();
            services.AddScoped<ITransactionRepository, TransactionRepository>();
            services.AddScoped<IPaymentRepository, PaymentRepository>();
            services.AddScoped<IWalletRepository, WalletRepository>();
            services.AddScoped<ITournamentRepository, TournamentRepository>();
            services.AddScoped<IVoucherRepository, VoucherRepository>();
            services.AddScoped<ISystemRepository, SystemRepository>();
            services.AddScoped<INotificationRepository, NotificationRepository>();
            services.AddScoped<IThreadRepository, ThreadRepository>();
            services.AddScoped<ICommentRepository, CommentRepository>();
            services.AddScoped<IFriendlistRepository, FriendlistRepository>();
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
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IRoomService, RoomService>();
            services.AddScoped<IMessageService, MessageService>();
            services.AddScoped<IPriceService, PriceService>();
            services.AddScoped<IAppointmentrequestService, AppointmentrequestService>();
            services.AddScoped<IFriendrequestService, FriendrequestService>();
            services.AddScoped<IImageService, ImageService>();
            services.AddScoped<ITransactionService, TransactionService>();
            services.AddScoped<IPaymentService, PaymentService>();
            services.AddScoped<IZaloPayService, ZaloPayService>();
            services.AddScoped<IWalletService, WalletService>();
            services.AddScoped<ITournamentService, TournamentService>();
            services.AddScoped<ITransactionService, TransactionService>();
            services.AddScoped<IVoucherService, VoucherService>();
            services.AddScoped<ISystemService, SystemService>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<IThreadService, ThreadService>();
            services.AddScoped<ICommentService,  CommentService>();
            services.AddScoped<IFriendlistService, FriendlistService>();

            services.AddScoped<ICloudinaryService, CloudinaryService>();
            services.AddHttpClient<IGHNService, GHNService>();
            services.AddScoped<IEmailService, EmailService>();

            services.AddHostedService<TimedHostedService>();
            services.AddScoped<ScheduleTimeValidator>();
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
					options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Error;
				});

			return services;
		}
    }
}
