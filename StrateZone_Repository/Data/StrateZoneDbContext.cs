using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using StrateZone_Repository.Entities;
using System.Reflection.Emit;
using static StrateZone_Repository.Parameters.PostgreEnums;
using GameExtensionEnum = StrateZone_Repository.Parameters.PostgreEnums.GameExtensionEnum;

namespace StrateZone_Repository.Data;

public partial class StrateZoneDbContext : DbContext
{
    public StrateZoneDbContext()
    {
    }

    public StrateZoneDbContext(DbContextOptions<StrateZoneDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Appointment> Appointments { get; set; }

    public virtual DbSet<Cart> Carts { get; set; }

    public virtual DbSet<CartItem> CartItems { get; set; }

    public virtual DbSet<Comment> Comments { get; set; }

    public virtual DbSet<Course> Courses { get; set; }

    public virtual DbSet<CoursesSlot> CoursesSlots { get; set; }

    public virtual DbSet<Event> Events { get; set; }

    public virtual DbSet<Friendlist> Friendlists { get; set; }

    public virtual DbSet<Appointmentrequest> AppointmentRequests { get; set; }

    public virtual DbSet<Friendrequest> Friendrequests { get; set; }

    public virtual DbSet<Entities.GameExtension> GameExtensions { get; set; }

    public virtual DbSet<Entities.GameType> GameTypes { get; set; }

    public virtual DbSet<Image> Images { get; set; }

    public virtual DbSet<Like> Likes { get; set; }

    public virtual DbSet<Message> Messages { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderDetail> OrderDetails { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<Price> Prices { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<ProductTag> ProductTags { get; set; }

    public virtual DbSet<Room> Rooms { get; set; }

    public virtual DbSet<Table> Tables { get; set; }

    public virtual DbSet<TablesAppointment> TablesAppointments { get; set; }

    public virtual DbSet<Tag> Tags { get; set; }

    public virtual DbSet<Entities.Thread> Threads { get; set; }

    public virtual DbSet<ThreadsTag> ThreadsTags { get; set; }

    public virtual DbSet<Ticket> Tickets { get; set; }

    public virtual DbSet<Tournament> Tournaments { get; set; }

    public virtual DbSet<TournamentsParticipants> TournamentsParticipants { get; set; }

    public virtual DbSet<Transaction> Transactions { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UsersCourse> UsersCourses { get; set; }

    public virtual DbSet<Voucher> Vouchers { get; set; }

    public virtual DbSet<Wallet> Wallets { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<Entities.System> Systems { get; set; }

    public virtual DbSet<AbnormalDay> AbnormalDays { get; set; }

    public virtual DbSet<Expense> Expenses { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder
                .UseNpgsql(
                    "Host=interchange.proxy.rlwy.net;Port=55988;Database=railway;Username=postgres;Password=LkbSuqBdhBaYqQehvnAWHZCLZOHTqaHk;SslMode=Disable",
                    dataSourceBuilder =>
                    {
                        dataSourceBuilder.MapEnum<CourseSlotStatus>("course_slot_status");
                        dataSourceBuilder.MapEnum<CourseStatus>("course_status");
                        dataSourceBuilder.MapEnum<EventStatus>("event_status");
                        dataSourceBuilder.MapEnum<GameExtensionEnum>("game_extension");
                        dataSourceBuilder.MapEnum<GameTypeEnum>("game_type");
                        dataSourceBuilder.MapEnum<Gender>("gender");
                        dataSourceBuilder.MapEnum<MessageStatus>("message_status");
                        dataSourceBuilder.MapEnum<PaymentStatus>("payment_status");
                        dataSourceBuilder.MapEnum<OrderStatus>("order_status");
                        dataSourceBuilder.MapEnum<PaymentType>("payment_type");
                        dataSourceBuilder.MapEnum<ParticipantStatus>("participant_status");
                        dataSourceBuilder.MapEnum<ProductStatus>("product_status");
                        dataSourceBuilder.MapEnum<Ranking>("ranking");
                        dataSourceBuilder.MapEnum<RequestStatus>("request_status");
                        dataSourceBuilder.MapEnum<RoomStatus>("room_status");
                        dataSourceBuilder.MapEnum<RoomType>("room_type");
                        dataSourceBuilder.MapEnum<SkillLevel>("skill_level");
                        dataSourceBuilder.MapEnum<ThreadStatus>("thread_status");
                        dataSourceBuilder.MapEnum<TicketType>("ticket_type");
                        dataSourceBuilder.MapEnum<TransactionType>("transaction_type");
                        dataSourceBuilder.MapEnum<UserCourseResult>("user_course_result");
                        dataSourceBuilder.MapEnum<UserRole>("user_role");
                        dataSourceBuilder.MapEnum<VoucherStatus>("voucher_status");
                        dataSourceBuilder.MapEnum<WalletStatus>("wallet_status");
                        dataSourceBuilder.MapEnum<TournamentStatus>("tournament_status");
                    })
                .LogTo(Console.WriteLine, LogLevel.Information, DbContextLoggerOptions.DefaultWithLocalTime);

    private string GetConnectionString()
    {
        IConfiguration configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", true, true).Build();
        string connection = configuration["ConnectionStrings:DB"];
        return connection;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresEnum<AppointmentStatus>();
        modelBuilder.HasPostgresEnum<CourseSlotStatus>();
        modelBuilder.HasPostgresEnum<CourseStatus>();
        modelBuilder.HasPostgresEnum<EventStatus>();
        modelBuilder.HasPostgresEnum<GameExtensionEnum>();
        modelBuilder.HasPostgresEnum<GameTypeEnum>();
        modelBuilder.HasPostgresEnum<Gender>();
        modelBuilder.HasPostgresEnum<MessageStatus>();
        modelBuilder.HasPostgresEnum<OrderStatus>();
        modelBuilder.HasPostgresEnum<ParticipantStatus>();
        modelBuilder.HasPostgresEnum<PaymentStatus>();
        modelBuilder.HasPostgresEnum<PaymentType>();
        modelBuilder.HasPostgresEnum<ProductStatus>();
        modelBuilder.HasPostgresEnum<Ranking>();
        modelBuilder.HasPostgresEnum<RequestStatus>();
        modelBuilder.HasPostgresEnum<RoomStatus>();
        modelBuilder.HasPostgresEnum<RoomType>();
        modelBuilder.HasPostgresEnum<SkillLevel>();
        modelBuilder.HasPostgresEnum<ThreadStatus>();
        modelBuilder.HasPostgresEnum<TicketType>();
        modelBuilder.HasPostgresEnum<TransactionType>();
        modelBuilder.HasPostgresEnum<UserCourseResult>();
        modelBuilder.HasPostgresEnum<UserRole>();
        modelBuilder.HasPostgresEnum<VoucherStatus>();
        modelBuilder.HasPostgresEnum<WalletStatus>();
        modelBuilder.HasPostgresEnum<TournamentStatus>();

        modelBuilder.Entity<Appointment>(entity =>
        {
            entity.HasKey(e => e.AppointmentId).HasName("appointments_pkey");

            entity.ToTable("appointments");

            entity.Property(e => e.AppointmentId).HasColumnName("appointment_id");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");

            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.Property(e => e.TotalPrice).HasColumnName("total_price");

            entity.Property(e => e.Status)
                  .HasColumnName("status")
                  .HasConversion(
                    v => v.ToString(),
                    v => (AppointmentStatus)Enum.Parse(typeof(AppointmentStatus), v)
                    );

            entity.HasOne(d => d.User).WithMany(p => p.Appointments)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("appointments_user_id_fkey");
        });

        modelBuilder.Entity<AbnormalDay>(entity =>
        {
            entity.ToTable("abnormal_days");

            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id)
                  .HasColumnName("id");

            entity.Property(e => e.SystemId)
                  .HasColumnName("system_id")
                  .IsRequired();

            entity.Property(e => e.Date)
                  .HasColumnName("date")
                  .HasColumnType("date");

            entity.Property(e => e.OpenTime)
                  .HasColumnName("open_time")
                  .HasColumnType("time");

            entity.Property(e => e.CloseTime)
                  .HasColumnName("close_time")
                  .HasColumnType("time");

            entity.Property(e => e.CreatedAt)
                  .HasColumnName("created_at")
                  .HasColumnType("timestamp")
                  .IsRequired();

            entity.HasOne(e => e.System)
                  .WithMany(s => s.AbnormalDays)
                  .HasForeignKey(e => e.SystemId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Cart>(entity =>
        {
            entity.HasKey(e => e.CartId).HasName("carts_pkey");

            entity.ToTable("carts");

            entity.Property(e => e.CartId).HasColumnName("cart_id");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
        });

        modelBuilder.Entity<CartItem>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("cart_items_pkey");

            entity.ToTable("cart_items");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CartId).HasColumnName("cart_id");
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.ProductQuantity)
                .HasDefaultValue(1)
                .HasColumnName("product_quantity");

            entity.HasOne(d => d.Cart).WithMany(p => p.CartItems)
                .HasForeignKey(d => d.CartId)
                .HasConstraintName("cart_items_cart_id_fkey");

            entity.HasOne(d => d.Product).WithMany(p => p.CartItems)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("cart_items_product_id_fkey");
        });

        modelBuilder.Entity<Comment>(entity =>
        {
            entity.HasKey(e => e.CommentId).HasName("comments_pkey");

            entity.ToTable("comments");

            entity.Property(e => e.CommentId).HasColumnName("comment_id");
            entity.Property(e => e.Content).HasColumnName("content");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");
            entity.Property(e => e.Rating)
                .HasDefaultValueSql("0")
                .HasColumnName("rating");
            entity.Property(e => e.ReplyTo).HasColumnName("reply_to");
            entity.Property(e => e.ThreadId).HasColumnName("thread_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.ReplyToNavigation).WithMany(p => p.InverseReplyToNavigation)
                .HasForeignKey(d => d.ReplyTo)
                .HasConstraintName("comments_reply_to_fkey");

            entity.HasOne(d => d.Thread).WithMany(p => p.Comments)
                .HasForeignKey(d => d.ThreadId)
                .HasConstraintName("comments_thread_id_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.Comments)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("comments_user_id_fkey");
        });

        modelBuilder.Entity<Course>(entity =>
        {
            entity.HasKey(e => e.CourseId).HasName("courses_pkey");

            entity.ToTable("courses");

            entity.Property(e => e.CourseId).HasColumnName("course_id");
            entity.Property(e => e.CourseName)
                .HasMaxLength(255)
                .HasColumnName("course_name");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");

            entity.Property(e => e.CourseStatus)
                  .HasColumnName("status")
                  .HasConversion(
                    v => v.ToString(),
                    v => (CourseStatus)Enum.Parse(typeof(CourseStatus), v)
                  );

            entity.Property(e => e.GameType)
                  .HasColumnName("chess_type")
                  .HasConversion(
                    v => v.ToString(),
                    v => (Parameters.PostgreEnums.GameTypeEnum)Enum.Parse(typeof(Parameters.PostgreEnums.GameTypeEnum), v)
                  );

            entity.Property(e => e.SkillLevel).HasColumnName("skill_level")
                  .HasConversion(
                    v => v.ToString(),
                    v => (SkillLevel)Enum.Parse(typeof(SkillLevel), v)
                  );

            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.EndDate).HasColumnName("end_date");
            entity.Property(e => e.InstructorId).HasColumnName("instructor_id");
            entity.Property(e => e.MaxParticipants).HasColumnName("max_participants");
            entity.Property(e => e.StartDate).HasColumnName("start_date");

            entity.HasOne(d => d.Instructor).WithMany(p => p.Courses)
                .HasForeignKey(d => d.InstructorId)
                .HasConstraintName("courses_instructor_id_fkey");
        });

        modelBuilder.Entity<CoursesSlot>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("courses_slot_pkey");

            entity.ToTable("courses_slot");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CourseId).HasColumnName("course_id");
            entity.Property(e => e.EndAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("end_at");
            entity.Property(e => e.InstructorId).HasColumnName("instructor_id");
            entity.Property(e => e.OnDate)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("on_date");
            entity.Property(e => e.RoomId).HasColumnName("room_id");
            entity.Property(e => e.StartAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("start_at");

            entity.Property(e => e.Status)
                  .HasColumnName("status")
                  .HasConversion(
                    v => v.ToString(),
                    v => (CourseSlotStatus)Enum.Parse(typeof(CourseSlotStatus), v)
                    );

            entity.HasOne(d => d.Course).WithMany(p => p.CoursesSlots)
                .HasForeignKey(d => d.CourseId)
                .HasConstraintName("courses_slot_course_id_fkey");

            entity.HasOne(d => d.Instructor).WithMany(p => p.CoursesSlots)
                .HasForeignKey(d => d.InstructorId)
                .HasConstraintName("courses_slot_instructor_id_fkey");

            entity.HasOne(d => d.Room).WithMany(p => p.CoursesSlots)
                .HasForeignKey(d => d.RoomId)
                .HasConstraintName("courses_slot_room_id_fkey");
        });

        modelBuilder.Entity<Event>(entity =>
        {
            entity.HasKey(e => e.EventId).HasName("events_pkey");

            entity.ToTable("events");

            entity.Property(e => e.EventId).HasColumnName("event_id");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.EndDate).HasColumnName("end_date");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
            entity.Property(e => e.StartDate).HasColumnName("start_date");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.Property(e => e.Status).HasColumnName("status").HasConversion(
                    v => v.ToString(),
                    v => (EventStatus)Enum.Parse(typeof(EventStatus), v)
                    );

            entity.HasOne(d => d.User).WithMany(p => p.Events)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("events_user_id_fkey");
        });

        modelBuilder.Entity<Friendlist>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("friendlists_pkey");

            entity.ToTable("friendlists");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.FriendId).HasColumnName("friend_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Friend).WithMany(p => p.FriendlistFriends)
                .HasForeignKey(d => d.FriendId)
                .HasConstraintName("friendlists_friend_id_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.FriendlistUsers)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("friendlists_user_id_fkey");
        });

        modelBuilder.Entity<Friendrequest>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("friendrequests_pkey");

            entity.ToTable("friendrequests");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.FromUser).HasColumnName("from_user");
            entity.Property(e => e.ToUser).HasColumnName("to_user");
            entity.Property(e => e.Status).HasColumnType("status").HasColumnName("status").HasConversion(
                    v => v.ToString(),
                    v => (RequestStatus)Enum.Parse(typeof(RequestStatus), v)
                    );

            entity.HasOne(d => d.FromUserNavigation).WithMany(p => p.FriendrequestFromUserNavigations)
                .HasForeignKey(d => d.FromUser)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("friendrequests_from_user_fkey");

            entity.HasOne(d => d.ToUserNavigation).WithMany(p => p.FriendrequestToUserNavigations)
                .HasForeignKey(d => d.ToUser)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("friendrequests_to_user_fkey");
        });

        modelBuilder.Entity<Appointmentrequest>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("appointment_requests_pkey");

            entity.ToTable("appointment_requests");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");

            entity.Property(e => e.ExpireAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("expire_at");

            entity.Property(e => e.StartTime)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("start_time");

            entity.Property(e => e.EndTime)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("end_time");

            entity.Property(e => e.TotalPrice)
                .HasColumnName("estimated_price");

            entity.Property(e => e.FromUser).HasColumnName("from_user");
            entity.Property(e => e.ToUser).HasColumnName("to_user");
            entity.Property(e => e.AppointmentId).HasColumnName("appointment_id");
            entity.Property(e => e.TableId).HasColumnName("table_id");
            entity.Property(e => e.Status).HasColumnType("request_status").HasColumnName("status").HasConversion(
                    v => v.ToString(),
                    v => (RequestStatus)Enum.Parse(typeof(RequestStatus), v)
                    );

            entity.HasOne(d => d.FromUserNavigation).WithMany(p => p.AppointmentRequestsFromUserNavigations)
                .HasForeignKey(d => d.FromUser)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("appointment_requests_from_user_fkey");

            entity.HasOne(d => d.ToUserNavigation).WithMany(p => p.AppointmentRequestsToUserNavigations)
                .HasForeignKey(d => d.ToUser)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("appointment_requests_to_user_fkey");

            entity.HasOne(d => d.Table).WithMany(p => p.Appointmentrequests)
                .HasForeignKey(d => d.TableId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("appointment_requests_to_table_fkey");

            entity.HasOne(d => d.Appointment).WithMany(p => p.Appointmentrequests)
                .HasForeignKey(d => d.AppointmentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("appointment_requests_to_appointment_fkey");
        });

        modelBuilder.Entity<Entities.GameExtension>(entity =>
        {
            entity.HasKey(e => e.ExtensionId).HasName("gameExtensions_pkey");

            entity.ToTable("gameExtensions");

            entity.Property(e => e.ExtensionId).HasColumnName("extension_id");
            entity.Property(e => e.TypeId).HasColumnName("type_id");

            entity.Property(e => e.ExtensionName).HasColumnName("extension_name").HasColumnType("game_extension").HasConversion(
                    v => v.ToString(),
                    v => (Parameters.PostgreEnums.GameExtensionEnum)Enum.Parse(typeof(Parameters.PostgreEnums.GameExtensionEnum), v)
                    );

            entity.HasOne(d => d.Type).WithMany(p => p.GameExtensions)
                .HasForeignKey(d => d.TypeId)
                .HasConstraintName("gameExtensions_type_id_fkey");
        });

        modelBuilder.Entity<Entities.GameType>(entity =>
        {
            entity.HasKey(e => e.TypeId).HasName("gameTypes_pkey");

            entity.ToTable("gameTypes");

            entity.Property(e => e.TypeId).HasColumnName("type_id");
            entity.Property(e => e.TypeName).HasColumnName("type_name").HasColumnType("game_type").HasConversion(
                    v => v.ToString(),
                    v => (Parameters.PostgreEnums.GameTypeEnum)Enum.Parse(typeof(Parameters.PostgreEnums.GameTypeEnum), v)
                    );
        });

        modelBuilder.Entity<Image>(entity =>
        {
            entity.HasKey(e => e.ImageId).HasName("images_pkey");

            entity.ToTable("images");

            entity.Property(e => e.ImageId).HasColumnName("image_id");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.ThreadId).HasColumnName("thread_id");
            entity.Property(e => e.GameTypeId).HasColumnName("gametype_id");
            entity.Property(e => e.TournamentId).HasColumnName("tournament_id");
            entity.Property(e => e.EventId).HasColumnName("event_id");
            entity.Property(e => e.Url)
                .HasMaxLength(255)
                .HasColumnName("url");

            entity.HasOne(d => d.User).WithOne(p => p.Image)
                .HasForeignKey<User>(d => d.UserId)
                .HasConstraintName("images_user_id_fkey");

            entity.HasOne(d => d.GameType).WithOne(p => p.Image)
                .HasForeignKey<GameType>(d => d.TypeId)
                .HasConstraintName("images_gametype_id_fkey");

            entity.HasOne(d => d.Event).WithOne(p => p.Image)
                .HasForeignKey<Event>(d => d.EventId)
                .HasConstraintName("images_event_id_fkey");

            entity.HasOne(d => d.Tournament).WithOne(p => p.Image)
                .HasForeignKey<Tournament>(d => d.TournamentId)
                .HasConstraintName("images_tournament_id_fkey");

            entity.HasOne(d => d.Product).WithMany(p => p.Images)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("images_product_id_fkey");

            entity.HasOne(d => d.Thread).WithMany(p => p.Images)
                .HasForeignKey(d => d.ThreadId)
                .HasConstraintName("images_thread_id_fkey");
        });

        modelBuilder.Entity<Like>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("likes_pkey");

            entity.ToTable("likes");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CommentId).HasColumnName("comment_id");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.ThreadId).HasColumnName("thread_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Comment).WithMany(p => p.Likes)
                .HasForeignKey(d => d.CommentId)
                .HasConstraintName("likes_comment_id_fkey");

            entity.HasOne(d => d.Thread).WithMany(p => p.Likes)
                .HasForeignKey(d => d.ThreadId)
                .HasConstraintName("likes_thread_id_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.Likes)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("likes_user_id_fkey");
        });

        modelBuilder.Entity<Message>(entity =>
        {
            entity.HasKey(e => e.MessageId).HasName("messages_pkey");

            entity.ToTable("messages");

            entity.Property(e => e.MessageId).HasColumnName("message_id");
            entity.Property(e => e.Content).HasColumnName("content");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.ReceiverId).HasColumnName("receiver_id");
            entity.Property(e => e.SenderId).HasColumnName("sender_id");

            entity.HasOne(d => d.Receiver).WithMany(p => p.MessageReceivers)
                .HasForeignKey(d => d.ReceiverId)
                .HasConstraintName("messages_receiver_id_fkey");

            entity.HasOne(d => d.Sender).WithMany(p => p.MessageSenders)
                .HasForeignKey(d => d.SenderId)
                .HasConstraintName("messages_sender_id_fkey");

            entity.Property(e => e.Status).HasColumnName("status").HasColumnType("message_status").HasConversion(
                    v => v.ToString(),
                    v => (MessageStatus)Enum.Parse(typeof(MessageStatus), v)
                    );
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.OrderId).HasName("orders_pkey");

            entity.ToTable("orders");

            entity.Property(e => e.OrderId).HasColumnName("order_id");
            entity.Property(e => e.OrderDate)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("order_date");
            entity.Property(e => e.TotalAmount)
                .HasPrecision(10, 2)
                .HasColumnName("total_amount");
            entity.Property(e => e.TrackingNumber)
                .HasMaxLength(22)
                .HasDefaultValueSql("NULL::character varying")
                .HasColumnName("tracking_number");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.Orders)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("orders_user_id_fkey");


            entity.Property(e => e.Status).HasColumnName("status").HasConversion(
                    v => v.ToString(),
                    v => (OrderStatus)Enum.Parse(typeof(OrderStatus), v)
                    );
        });

        modelBuilder.Entity<OrderDetail>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("order_details_pkey");

            entity.ToTable("order_details");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.OrderId).HasColumnName("order_id");
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.Quantity)
                .HasDefaultValue(1)
                .HasColumnName("quantity");
            entity.Property(e => e.Subtotal)
                .HasPrecision(10, 2)
                .HasColumnName("subtotal");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderDetails)
                .HasForeignKey(d => d.OrderId)
                .HasConstraintName("order_details_order_id_fkey");

            entity.HasOne(d => d.Product).WithMany(p => p.OrderDetails)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("order_details_product_id_fkey");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("payments_pkey");

            entity.ToTable("payments");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.Description).HasColumnName("description");

            entity.Property(e => e.PaymentType).HasColumnName("payment_type").HasConversion(
                    v => v.ToString(),
                    v => (PaymentType) Enum.Parse(typeof(PaymentType), v)
                );

            entity.Property(e => e.PaymentStatus).HasColumnName("status").HasConversion(
                    v => v.ToString(),
                    v => (PaymentStatus)Enum.Parse(typeof(PaymentStatus), v)
                );

            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.OrderId).HasColumnName("order_id");
            entity.Property(e => e.TablesAppointmentId).HasColumnName("tables_appointment_id");
            entity.Property(e => e.CourseId).HasColumnName("course_id");
            entity.Property(e => e.VoucherId).HasColumnName("voucher_id");

            entity.HasOne(d => d.Order).WithOne(p => p.Payment)
                .HasForeignKey<Order>(d => d.OrderId)
                .HasConstraintName("payments_order_id_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.Payments)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("payments_user_id_fkey");

            entity.HasOne(d => d.Course).WithMany(p => p.Payments)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("payments_course_id_fkey");

            entity.HasOne(d => d.TablesAppointment).WithMany(p => p.Payments)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("payments_tables_appointment_id_fkey");

            entity.HasOne(d => d.Voucher).WithMany(p => p.Payments)
                .HasForeignKey(d => d.VoucherId)
                .HasConstraintName("payment_voucher_id_fkey");
        });

        modelBuilder.Entity<Price>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("prices_pkey");

            entity.ToTable("prices");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CourseId).HasColumnName("course_id");
            entity.Property(e => e.GameTypeId).HasColumnName("game_type_id");
            entity.Property(e => e.MemberFee)
                .HasColumnType("boolean")
                .HasColumnName("member_fee");
            entity.Property(e => e.Price1)
                .HasPrecision(10, 2)
                .HasColumnName("price");
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.TeachingSalary)
                .HasColumnType("boolean")
                .HasColumnName("teaching_salary");
            entity.Property(e => e.RoomType).HasColumnName("room_type")
                .HasConversion(
                    v => v.ToString(),
                    v => (RoomType) Enum.Parse(typeof(RoomType), v)
                );
            entity.Property(e => e.Unit)
                .HasMaxLength(50)
                .HasColumnName("unit");

            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Course).WithMany(p => p.Prices)
                .HasForeignKey(d => d.CourseId)
                .HasConstraintName("prices_course_id_fkey");

            entity.HasOne(d => d.GameType).WithMany(p => p.Prices)
                .HasForeignKey(d => d.GameTypeId)
                .HasConstraintName("prices_game_type_id_fkey");

            entity.HasOne(d => d.Product).WithMany(p => p.Prices)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("prices_product_id_fkey");
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("notifications_pkey");

            entity.ToTable("notifications");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Title).HasColumnName("title");
            entity.Property(e => e.Content).HasColumnName("content");
            entity.Property(e => e.ToUser).HasColumnName("to_user");
            entity.Property(e => e.TablesAppointmentId).HasColumnName("tables_appointment_id");
            entity.Property(e => e.OrderId).HasColumnName("order_id");
            entity.Property(e => e.TournamentId).HasColumnName("tournament_id");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");

            entity.Property(e => e.Status).HasColumnName("status").HasConversion(
                    v => v.ToString(),
                    v => (MessageStatus)Enum.Parse(typeof(MessageStatus), v)
                    );

            entity.Property(e => e.Type).HasColumnName("type").HasConversion(
                    v => v.ToString(),
                    v => (NotificationType)Enum.Parse(typeof(NotificationType), v)
                    );

            entity.HasOne(d => d.ToUserNavigation).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.ToUser)
                .HasConstraintName("notifications_user_id_fkey");

            entity.HasOne(d => d.TablesAppointment).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.TablesAppointmentId)
                .HasConstraintName("notifications_tables_appointment_id_fkey");

            entity.HasOne(d => d.Order).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.OrderId)
                .HasConstraintName("notifications_order_id_fkey");

            entity.HasOne(d => d.Tournament).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.TournamentId)
                .HasConstraintName("notifications_tournament_id_fkey");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.ProductId).HasName("products_pkey");

            entity.ToTable("products");

            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.ImageUrl)
                .HasMaxLength(255)
                .HasColumnName("image_url");
            entity.Property(e => e.InventoryCount).HasColumnName("inventory_count");
            entity.Property(e => e.ProductName)
                .HasMaxLength(100)
                .HasColumnName("product_name");

            entity.Property(e => e.Status).HasColumnName("status").HasConversion(
                    v => v.ToString(),
                    v => (ProductStatus)Enum.Parse(typeof(ProductStatus), v)
                    );
        });

        modelBuilder.Entity<ProductTag>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("product_tags_pkey");

            entity.ToTable("product_tags");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.TagId).HasColumnName("tag_id");

            entity.HasOne(d => d.Product).WithMany(p => p.ProductTags)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("product_tags_product_id_fkey");

            entity.HasOne(d => d.Tag).WithMany(p => p.ProductTags)
                .HasForeignKey(d => d.TagId)
                .HasConstraintName("product_tags_tag_id_fkey");
        });

        modelBuilder.Entity<Room>(entity =>
        {
            entity.HasKey(e => e.RoomId).HasName("rooms_pkey");

            entity.ToTable("rooms");

            entity.Property(e => e.RoomId).HasColumnName("room_id");
            entity.Property(e => e.Capacity).HasColumnName("capacity");
            entity.Property(e => e.RoomName)
                .HasMaxLength(5)
                .HasColumnName("room_name");

            entity.Property(e => e.Type).HasColumnName("room_type").HasColumnType("room_type").HasConversion(
                    v => v.ToString(),
                    v => (RoomType)Enum.Parse(typeof(RoomType), v)
                    );

            entity.Property(e => e.Description).HasColumnName("description")
                .HasMaxLength(200)
                .HasColumnName("description");

            entity.Property(e => e.Status).HasColumnName("status").HasColumnType("room_status").HasConversion(
                    v => v.ToString(),
                    v => (RoomStatus)Enum.Parse(typeof(RoomStatus), v)
                    );
        });

        modelBuilder.Entity<Entities.System>(entity =>
        {
            entity.ToTable("systems");

            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id)
                  .HasColumnName("id");

            entity.Property(e => e.AdminId)
                  .HasColumnName("admin_id")
                  .IsRequired();

            entity.Property(e => e.OpenTime)
                  .HasColumnName("open_time")
                  .HasColumnType("time");

            entity.Property(e => e.CloseTime)
                  .HasColumnName("close_time")
                  .HasColumnType("time");

            entity.Property(e => e.Status)
                  .HasColumnName("status")
                  .HasDefaultValue("active");

            entity.HasOne(e => e.User)
                  .WithMany(u => u.Systems)
                  .HasForeignKey(e => e.AdminId);
        });

        modelBuilder.Entity<Table>(entity =>
        {
            entity.HasKey(e => e.TableId).HasName("tables_pkey");

            entity.ToTable("tables");

            entity.Property(e => e.TableId).HasColumnName("table_id");

            entity.Property(e => e.GameTypeId).HasColumnName("gameType_id");
            entity.Property(e => e.RoomId).HasColumnName("room_id");
            entity.Property(e => e.Status)
            .HasConversion<string>()
            .HasColumnName("status");

            entity.HasOne(d => d.GameType).WithMany(p => p.Tables)
                .HasForeignKey(d => d.GameTypeId)
                .HasConstraintName("tables_gameType_id_fkey");

            entity.HasOne(d => d.Room).WithMany(p => p.Tables)
                .HasForeignKey(d => d.RoomId)
                .HasConstraintName("tables_room_id_fkey");
        });

        modelBuilder.Entity<TablesAppointment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("tables_appointments_pkey");

            entity.ToTable("tables_appointments");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AppointmentId).HasColumnName("appointment_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.TableId).HasColumnName("table_id");
            entity.Property(e => e.Price).HasColumnName("price");

            entity.Property(e => e.ScheduleTime)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("schedule_time");
            entity.Property(e => e.EndTime)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("end_time");

            entity.Property(e => e.Status).HasColumnName("status").HasConversion(
                    v => v.ToString(),
                    v => (AppointmentStatus)Enum.Parse(typeof(AppointmentStatus), v)
                );

            entity.HasOne(d => d.Appointment).WithMany(p => p.TablesAppointments)
                .HasForeignKey(d => d.AppointmentId)
                .HasConstraintName("tables_appointments_appointment_id_fkey");

            entity.HasOne(d => d.Table).WithMany(p => p.TablesAppointments)
                .HasForeignKey(d => d.TableId)
                .HasConstraintName("tables_appointments_table_id_fkey");
        });

        modelBuilder.Entity<Tag>(entity =>
        {
            entity.HasKey(e => e.TagId).HasName("tags_pkey");

            entity.ToTable("tags");

            entity.Property(e => e.TagId).HasColumnName("tag_id");
            entity.Property(e => e.TagName)
                .HasMaxLength(50)
                .HasColumnName("tag_name");

            entity.Property(e => e.Status)
                  .HasConversion(
                    v => v.ToString(),
                    v => (Parameters.PostgreEnums.TagStatus)Enum.Parse(typeof(Parameters.PostgreEnums.TagStatus), v)
                  )
                .HasColumnName("status");

            entity.Property(e => e.TagColor)
                .HasColumnName("tag_color");

            entity.Property(e => e.AllowedRole)
                  .HasConversion(
                    v => v.ToString(),
                    v => (Parameters.PostgreEnums.UserRole)Enum.Parse(typeof(Parameters.PostgreEnums.UserRole), v)
                  )
                .HasColumnName("allowed_role");
        });

        modelBuilder.Entity<Entities.Thread>(entity =>
        {
            entity.HasKey(e => e.ThreadId).HasName("threads_pkey");

            entity.ToTable("threads");

            entity.Property(e => e.ThreadId).HasColumnName("thread_id");
            entity.Property(e => e.Content).HasColumnName("content");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");

            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");

            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.Rating)
                .HasDefaultValueSql("0")
                .HasColumnName("rating");
            entity.Property(e => e.ThumbnailUrl)
                .HasMaxLength(255)
                .HasColumnName("thumbnail_url");
            entity.Property(e => e.Title).HasColumnName("title");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.Threads)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("threads_created_by_fkey");

            entity.Property(e => e.Status).HasColumnName("status").HasConversion(
                    v => v.ToString(),
                    v => (ThreadStatus)Enum.Parse(typeof(ThreadStatus), v)
                    );
        });

        modelBuilder.Entity<ThreadsTag>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("threads_tags_pkey");

            entity.ToTable("threads_tags");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.TagId).HasColumnName("tag_id");
            entity.Property(e => e.ThreadId).HasColumnName("thread_id");

            entity.HasOne(d => d.Tag).WithMany(p => p.ThreadsTags)
                .HasForeignKey(d => d.TagId)
                .HasConstraintName("threads_tags_tag_id_fkey");

            entity.HasOne(d => d.Thread).WithMany(p => p.ThreadsTags)
                .HasForeignKey(d => d.ThreadId)
                .HasConstraintName("threads_tags_thread_id_fkey");
        });

        modelBuilder.Entity<Ticket>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("tickets_pkey");

            entity.ToTable("tickets");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AttachmentUrl)
                .HasMaxLength(255)
                .HasColumnName("attachment_url");
            entity.Property(e => e.Reason).HasColumnName("reason");
            entity.Property(e => e.SenderId).HasColumnName("sender_id");
            entity.Property(e => e.SentAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("sent_at");

            entity.HasOne(d => d.Sender).WithMany(p => p.Tickets)
                .HasForeignKey(d => d.SenderId)
                .HasConstraintName("tickets_sender_id_fkey");

            entity.Property(e => e.TicketType).HasColumnName("ticket_type").HasConversion(
                    v => v.ToString(),
                    v => (TicketType)Enum.Parse(typeof(TicketType), v)
                    );
        });

        modelBuilder.Entity<Tournament>(entity => 
        {
            entity.HasKey(e => e.TournamentId).HasName("tournaments_pkey");

            entity.ToTable("tournaments");

            entity.Property(e => e.TournamentId).HasColumnName("tournament_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
            entity.Property(e => e.StartDate).HasColumnName("start_date");
            entity.Property(e => e.EndDate).HasColumnName("end_date");

            entity.Property(e => e.TargetedRanking).HasColumnName("targeted_ranking").HasConversion(
                    v => v.ToString(),
                    v => (Ranking) Enum.Parse(typeof(Ranking), v)
                );

            entity.Property(e => e.MaxParticipants).HasColumnName("max_participants");

            entity.Property(e => e.Status).HasColumnName("status").HasConversion(
                    v => v.ToString(),
                    v => (TournamentStatus) Enum.Parse(typeof(TournamentStatus), v)
                    );

            entity.HasOne(d => d.User).WithMany(p => p.Tournaments)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("tournaments_user_id_fkey");
        });

        modelBuilder.Entity<TournamentsParticipants>(entity => 
        {
            entity.HasKey(e => e.Id).HasName("tournaments_participants_pkey");

            entity.ToTable("tournaments_participants");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.TournamentId).HasColumnName("tournament_id");
            entity.Property(e => e.ParticipantId).HasColumnName("participant_id");

            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");

            entity.HasOne(d => d.Tournament).WithMany(p => p.TournamentsParticipants)
                .HasForeignKey(d => d.TournamentId)
                .HasConstraintName("tournament_participants_tournament_id_fkey");

            entity.HasOne(d => d.Participant).WithMany(p => p.TournamentsParticipants)
                .HasForeignKey(d => d.ParticipantId)
                .HasConstraintName("tournament_participants_participant_id_fkey");
        });

        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("transactions_pkey");

            entity.ToTable("transactions");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Amount)
                .HasPrecision(10, 2)
                .HasColumnName("amount");
            entity.Property(e => e.Content).HasColumnName("content");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.OfUser).HasColumnName("of_user");
            entity.Property(e => e.ReferenceId)
                .HasMaxLength(50)
                .HasColumnName("reference_id");

            entity.HasOne(d => d.OfUserNavigation).WithMany(p => p.Transactions)
                .HasForeignKey(d => d.OfUser)
                .HasConstraintName("transactions_of_user_fkey");

            entity.Property(e => e.TransactionType).HasColumnName("type").HasConversion(
                    v => v.ToString(),
                    v => (TransactionType)Enum.Parse(typeof(TransactionType), v)
                    );
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("users_pkey");

            entity.ToTable("users");

            entity.HasIndex(e => e.Email, "users_email_key").IsUnique();

            entity.HasIndex(e => e.Phone, "users_phone_key").IsUnique();

            entity.HasIndex(e => e.Username, "users_username_key").IsUnique();

            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.UserRole).HasColumnName("role").HasColumnType("user_role").HasConversion(
                    v => v.ToString(),
                    v => (UserRole)Enum.Parse(typeof(UserRole), v)
                );
            entity.Property(e => e.Address)
                .HasMaxLength(100)
                .HasColumnName("address");
            
            entity.Property(e => e.Bio).HasColumnName("bio");
            
            entity.Property(e => e.FullName).HasColumnName("full_name");

            entity.Property(e => e.Gender).HasColumnName("gender").HasColumnType("gender").HasConversion(
                    v => v.ToString(),
                    v => (Gender)Enum.Parse(typeof(Gender), v)
                );

            entity.Property(e => e.SkillLevel).HasColumnName("skill_level").HasColumnType("skill_level").HasConversion(
                    v => v.ToString(),
                    v => (SkillLevel) Enum.Parse(typeof(SkillLevel), v)
                );

            entity.Property(e => e.Ranking).HasColumnName("ranking").HasColumnType("ranking").HasConversion(
                    v => v.ToString(),
                    v => (Ranking)Enum.Parse(typeof(Ranking), v)
                );

            entity.Property(e => e.CartId).HasColumnName("cart_id");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("email");
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .HasColumnName("password");
            entity.Property(e => e.Phone)
                .HasMaxLength(15)
                .HasColumnName("phone");
            entity.Property(e => e.Points)
                .HasDefaultValue(0)
                .HasColumnName("points");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasComment("Depends on Role")
                .HasColumnName("status");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");
            entity.Property(e => e.RefreshToken)
              .HasMaxLength(100)
              .HasColumnName("refresh_token");
            entity.Property(e => e.RefreshTokenExpiry)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("refresh_token_expiry");
            entity.Property(e => e.OTP)
                .HasMaxLength(100)
                .HasColumnName("otp");
            entity.Property(e => e.OTPExpiry)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("otp_expiry");
            entity.Property(e => e.Username)
                .HasMaxLength(50)
                .HasColumnName("username");

            entity.HasOne(d => d.Cart).WithOne(p => p.User)
                .HasForeignKey<Cart>(d => d.CartId)
                .HasConstraintName("users_cart_id_fkey")
                .OnDelete(DeleteBehavior.Cascade);

        });

        modelBuilder.Entity<UsersCourse>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("users_courses_pkey");

            entity.ToTable("users_courses");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CourseId).HasColumnName("course_id");
            entity.Property(e => e.EnrolledAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("enrolled_at");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.Result).HasColumnName("result").HasConversion(
                    v => v.ToString(),
                    v => (UserCourseResult)Enum.Parse(typeof(UserCourseResult), v)
                    );
            entity.Property(e => e.ParticipantStatus).HasColumnName("participant_status").HasConversion(
                    v => v.ToString(),
                    v => (ParticipantStatus)Enum.Parse(typeof(ParticipantStatus), v)
                    );
            entity.HasOne(d => d.Course).WithMany(p => p.UsersCourses)
                .HasForeignKey(d => d.CourseId)
                .HasConstraintName("users_courses_course_id_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.UsersCourses)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("users_courses_user_id_fkey");
        });

        modelBuilder.Entity<Voucher>(entity =>
        {
            entity.HasKey(e => e.VoucherId).HasName("vouchers_pkey");

            entity.ToTable("vouchers");

            entity.Property(e => e.VoucherId).HasColumnName("voucher_id");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");

            entity.Property(e => e.Value).HasColumnName("value");

            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .HasColumnName("description");
            entity.Property(e => e.ExpireDate).HasColumnName("expire_date");
            entity.Property(e => e.MinPriceCondition)
                .HasPrecision(10, 2)
                .HasDefaultValueSql("0")
                .HasColumnName("min_price_condition");
            entity.Property(e => e.VoucherName)
                .HasMaxLength(50)
                .HasColumnName("voucher_name");

            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.IsSample).HasColumnName("is_sample");
            entity.Property(e => e.PointsCost).HasColumnName("points_cost");

            entity.Property(e => e.Status).HasColumnName("status").HasConversion(
                    v => v.ToString(),
                    v => (VoucherStatus)Enum.Parse(typeof(VoucherStatus), v)
                    );

            entity.HasOne(d => d.User).WithMany(p => p.Vouchers)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("vouchers_user_id_fkey")
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Wallet>(entity =>
        {
            entity.HasKey(e => e.WalletId).HasName("wallet_pkey");

            entity.ToTable("wallet");

            entity.Property(e => e.WalletId).HasColumnName("wallet_id");
            entity.Property(e => e.Balance)
                .HasPrecision(10, 2)
                .HasDefaultValueSql("0")
                .HasColumnName("balance");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.Status).HasColumnName("status").HasConversion(
                    v => v.ToString(),
                    v => (WalletStatus)Enum.Parse(typeof(WalletStatus), v)
                    );

            entity.HasOne(d => d.User).WithOne(p => p.Wallet)
                .HasForeignKey<User>(d => d.UserId)
                .HasConstraintName("wallet_user_id_fkey")
                .OnDelete(DeleteBehavior.Cascade);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
