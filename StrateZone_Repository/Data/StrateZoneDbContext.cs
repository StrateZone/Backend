using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using StrateZone_Repository.Entities;
using System.Reflection.Emit;
using static StrateZone_Repository.Parameters.PostgreEnums;
using Thread = StrateZone_Repository.Entities.Thread;

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

    public virtual DbSet<Comment> Comments { get; set; }

    public virtual DbSet<Friendlist> Friendlists { get; set; }

    public virtual DbSet<Appointmentrequest> AppointmentRequests { get; set; }

    public virtual DbSet<Friendrequest> Friendrequests { get; set; }

    public virtual DbSet<Entities.GameType> GameTypes { get; set; }

    public virtual DbSet<Image> Images { get; set; }

    public virtual DbSet<Like> Likes { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<Price> Prices { get; set; }

    public virtual DbSet<Room> Rooms { get; set; }

    public virtual DbSet<Table> Tables { get; set; }

    public virtual DbSet<TablesAppointment> TablesAppointments { get; set; }

    public virtual DbSet<Tag> Tags { get; set; }

    public virtual DbSet<Entities.Thread> Threads { get; set; }

    public virtual DbSet<ThreadsTag> ThreadsTags { get; set; }

    public virtual DbSet<Transaction> Transactions { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<Voucher> Vouchers { get; set; }

    public virtual DbSet<Wallet> Wallets { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<Entities.System> Systems { get; set; }

    public virtual DbSet<AbnormalDay> AbnormalDays { get; set; }

    public virtual DbSet<Expense> Expenses { get; set; }

    public virtual DbSet<Profanity> Profanities { get; set; }

    public virtual DbSet<PointsHistory> PointsHistories { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder
                .UseNpgsql(
                    GetConnectionString(),
                    dataSourceBuilder =>
                    {
                        dataSourceBuilder.MapEnum<CourseSlotStatus>("course_slot_status");
                        dataSourceBuilder.MapEnum<CourseStatus>("course_status");
                        dataSourceBuilder.MapEnum<EventStatus>("event_status");
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
        return "Host=centerbeam.proxy.rlwy.net;Port=16477;Database=railway;Username=postgres;Password=FSRdUxlJFvAfhkdvLNdgzEeNDZGiIrWx;SslMode=Disable;Timeout=30;Command Timeout=30;Pooling=true;Minimum Pool Size=1;Maximum Pool Size=20;";
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresEnum<AppointmentStatus>();
        modelBuilder.HasPostgresEnum<CourseSlotStatus>();
        modelBuilder.HasPostgresEnum<CourseStatus>();
        modelBuilder.HasPostgresEnum<EventStatus>();
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

            entity.Property(e => e.IsPaid)
                .HasColumnName("is_paid");

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

        modelBuilder.Entity<Entities.GameType>(entity =>
        {
            entity.HasKey(e => e.TypeId).HasName("gameTypes_pkey");

            entity.ToTable("gameTypes");

            entity.Property(e => e.TypeId).HasColumnName("type_id");
            entity.Property(e => e.TypeName).HasColumnName("type_name");
            entity.Property(e => e.Status).HasColumnName("status");
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

            entity.HasOne(d => d.User).WithMany(p => p.Payments)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("payments_user_id_fkey");

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
            entity.Property(e => e.RoomType).HasColumnName("room_type");
            entity.Property(e => e.Unit)
                .HasMaxLength(50)
                .HasColumnName("unit");

            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");

            entity.Property(e => e.Type).HasColumnName("type");

            entity.HasOne(d => d.GameType).WithMany(p => p.Prices)
                .HasForeignKey(d => d.GameTypeId)
                .HasConstraintName("prices_game_type_id_fkey");
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

            entity.Property(e => e.Type).HasColumnName("room_type").HasColumnType("room_type");

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

            entity.Property(e => e.Appointment_Refund100_HoursFromScheduleTime)
                  .HasColumnName("appointment_refund100_hoursfromscheduletime");

            entity.Property(e => e.Appointment_Incoming_HoursFromScheduleTime)
                    .HasColumnName("appointment_incoming_hoursfromscheduletime");

            entity.Property(e => e.Appointment_Checkin_MinutesFromScheduleTime)
                    .HasColumnName("appointment_checkin_minutesfromscheduletime");

            entity.Property(e => e.Max_NumberOfTables_CancelPerWeek)
                    .HasColumnName("number_of_tables_cancel_perweek");

            entity.Property(e => e.ContributionPoints_PerThread)
                    .HasColumnName("contribution_points_per_thread");

            entity.Property(e => e.ContributionPoints_PerComment)
                    .HasColumnName("contribution_points_per_comment");

            entity.Property(e => e.UserPoints_PerCheckinTable_ByPercentageOfTablesPrice)
                    .HasColumnName("user_points_per_checked_in_appointment_by_percentage_of_price");

            entity.Property(e => e.AppointmentRequest_MaxHours_UntilExpiration)
                    .HasColumnName("appointmentrequests_maxhours_untilexpiration");

            entity.Property(e => e.AppointmentRequest_MinHours_UntilExpiration)
                    .HasColumnName("appointmentrequests_minhours_untilexpiration");

            entity.Property(e => e.Numberof_TopContributors_PerWeek)
                    .HasColumnName("numberof_topcontributors_per_week");

            entity.Property(e => e.Max_NumberOfUsers_InvitedToTable)
                    .HasColumnName("max_users_invited_to_table");

            entity.Property(e => e.PercentageRefund_IfNot100)
                    .HasColumnName("percentage_refund_ifnot100");
            
            entity.Property(e => e.PercentageTimeRange_UntilRequestExpiration)
                    .HasColumnName("percentage_timerange_untilrequestexpiration");

            entity.Property(e => e.Verification_OTP_Duration)
                    .HasColumnName("verification_otp_duration_inminutes");

            entity.Property(e => e.ExtendCancel_BeforeMinutes_FromPlayTime)
                    .HasColumnName("extend_cancellation_minutes_before_playtime");

            entity.Property(e => e.Min_Minutes_For_TablesExtend)
                   .HasColumnName("min_minutes_for_tables_extend");

            entity.Property(e => e.Max_Minutes_For_TablesExtend)
                    .HasColumnName("max_minutes_for_tables_extend");

            entity.Property(e => e.ExtendAllow_BeforeMinutes_FromTableComplete)
                    .HasColumnName("extend_allow_minutes_before_table_complete");

            entity.Property(e => e.Percentage_Refund_On_ExtendedTables)
                    .HasColumnName("percentage_refund_on_extended_tables");

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

            entity.Property(e => e.Note).HasColumnName("note");

            entity.Property(e => e.PaidForOpponent).HasColumnName("paid_for_opponent");

            entity.Property(e => e.IsExtended).HasColumnName("is_extended");

            entity.Property(e => e.ExtendedOf).HasColumnName("extended_of_id");

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

            entity.Property(e => e.UpdateOfThread).HasColumnName("update_of_thread");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.Threads)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("threads_created_by_fkey");

            entity.HasOne(d => d.UpdateOfThreadNavigation).WithOne(p => p.InverseUpdateOfThreadNavigation)
                .HasForeignKey<Thread>(d => d.UpdateOfThread)
                .HasConstraintName("threads_update_of_thread_fkey");

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

            entity.Property(e => e.UserLabel).HasColumnName("label").HasConversion(
                    v => v.ToString(),
                    v => (UserLabel)Enum.Parse(typeof(UserLabel), v)
                );

            entity.Property(e => e.Address)
                .HasMaxLength(100)
                .HasColumnName("address");

            entity.Property(e => e.IsPasswordHashed)
                .HasColumnName("is_hashed_password");

            entity.Property(e => e.Bio).HasColumnName("bio");
            
            entity.Property(e => e.FullName).HasColumnName("full_name");

            entity.Property(e => e.Gender).HasColumnName("gender").HasColumnType("gender").HasConversion(
                    v => v.ToString(),
                    v => (Gender)Enum.Parse(typeof(Gender), v)
                );


            entity.Property(e => e.Status).HasColumnName("status").HasConversion(
                    v => v.ToString(),
                    v => (UserStatus)Enum.Parse(typeof(UserStatus), v)
                );

            entity.Property(e => e.AvatarUrl).HasColumnName("avatar_url");

            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.MembershipExpiry)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("membership_expiry");
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
            entity.Property(e => e.ContributionPoints)
                .HasDefaultValue(0)
                .HasColumnName("contribution_points");
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
            entity.Property(e => e.DayOfUsage).HasColumnName("day_of_usage");
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
            entity.Property(e => e.ContributionPointsCost).HasColumnName("contributor_points_cost");

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

        modelBuilder.Entity<Expense>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("expenses_pkey");

            entity.ToTable("expenses");

            entity.Property(e => e.Id).HasColumnName("id");

            entity.Property(e => e.Amount)
                .HasPrecision(10, 2)
                .HasDefaultValueSql("0")
                .HasColumnName("amount");

            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.Type).HasColumnName("type");
            
            entity.Property(e => e.Description).HasColumnName("description");
            
            entity.Property(e => e.TransactionDate).HasColumnName("transaction_date").HasColumnType("timestamp without time zone");

            entity.Property(e => e.CreatedAt).HasColumnName("created_at").HasColumnType("timestamp without time zone");

            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at").HasColumnType("timestamp without time zone");

            entity.Property(e => e.SystemId).HasColumnName("system_id");

            entity.HasOne(d => d.User).WithMany(p => p.Expenses)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("expenses_user_id_fkey")
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Profanity>(entity =>
        {
            entity.ToTable("profanities");
            entity.HasKey(p => p.Id).HasName("e_pkey"); ;

            entity.Property(p => p.Id).HasColumnName("id");
            entity.Property(p => p.Word).IsRequired().HasColumnName("word");
        });

        modelBuilder.Entity<PointsHistory>(entity =>
        {
            entity.ToTable("points_history");

            entity.HasKey(e => e.Id).HasName("points_history_pkey");

            entity.Property(e => e.Id)
                  .HasColumnName("id");

            entity.Property(e => e.OfUser)
                  .HasColumnName("of_user");

            entity.Property(e => e.Description)
                  .HasColumnName("description");

            entity.Property(e => e.Amount)
                  .HasColumnName("amount");

            entity.Property(e => e.Content)
                  .HasColumnName("content");

            entity.Property(e => e.PointType)
                  .HasColumnName("point_type")
                  .IsRequired();

            entity.Property(e => e.CreatedAt)
                  .HasColumnName("created_at")
                  .HasColumnType("timestamp");

            entity.HasOne(d => d.OfUserNavigation)
                  .WithMany(p => p.PointsHistories)
                  .HasForeignKey(d => d.OfUser)
                  .HasConstraintName("points_history_of_user_fkey");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
