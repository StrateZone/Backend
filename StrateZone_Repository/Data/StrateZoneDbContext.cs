using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Npgsql;
using StrateZone_Repository.Entities;
using static StrateZone_Repository.Parameters.PostgreEnums;

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

    public virtual DbSet<Transaction> Transactions { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UsersCourse> UsersCourses { get; set; }

    public virtual DbSet<Voucher> Vouchers { get; set; }

    public virtual DbSet<Wallet> Wallets { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql(GetConnectionString());

    private string GetConnectionString()
    {
        IConfiguration configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", true, true).Build();
        var connection = configuration["ConnectionStrings:DB"];
        return "Host=switchyard.proxy.rlwy.net;Port=35707;Database=railway;Username=postgres;Password=fqLsUMeFmmCJNzTcjKiqGPVswmwmjIOj;SslMode=Disable";
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresEnum<CourseSlotStatus>("course_slot_status");
        modelBuilder.HasPostgresEnum<CourseStatus>("course_status");
        modelBuilder.HasPostgresEnum<EventStatus>("event_status");
        modelBuilder.HasPostgresEnum<EventType>("event_type");
        modelBuilder.HasPostgresEnum<Parameters.PostgreEnums.GameExtension>("game_extension");
        modelBuilder.HasPostgresEnum<Parameters.PostgreEnums.GameType>("game_type");
        modelBuilder.HasPostgresEnum<Gender>("gender");
        modelBuilder.HasPostgresEnum<MessageStatus>("message_status");
        modelBuilder.HasPostgresEnum<OrderStatus>("order_status");
        modelBuilder.HasPostgresEnum<ParticipantStatus>("participant_status");
        modelBuilder.HasPostgresEnum<ProductStatus>("product_status");
        modelBuilder.HasPostgresEnum<Ranking>("ranking");
        modelBuilder.HasPostgresEnum<RequestStatus>("request_status");
        modelBuilder.HasPostgresEnum<RoomStatus>("room_status");
        modelBuilder.HasPostgresEnum<RoomType>("room_type");
        modelBuilder.HasPostgresEnum<SkillLevel>("skill_level");
        modelBuilder.HasPostgresEnum<ThreadStatus>("thread_status");
        modelBuilder.HasPostgresEnum<TicketType>("ticket_type");
        modelBuilder.HasPostgresEnum<TransactionType>("transaction_type");
        modelBuilder.HasPostgresEnum<UserCourseResult>("user_course_result");
        modelBuilder.HasPostgresEnum<UserRole>("user_role");
        modelBuilder.HasPostgresEnum<VoucherStatus>("voucher_status");
        modelBuilder.HasPostgresEnum<WalletStatus>("wallet_status");

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            foreach (var property in entityType.GetProperties())
            {
                if (property.ClrType.IsEnum)
                {
                    property.SetColumnType("text"); // Store enums as string
                    property.SetProviderClrType(typeof(string)); // Ensure EF treats it as string
                }
            }
        }

        modelBuilder.Entity<Appointment>(entity =>
        {
            entity.HasKey(e => e.AppointmentId).HasName("appointments_pkey");

            entity.ToTable("appointments");

            entity.Property(e => e.AppointmentId).HasColumnName("appointment_id");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.ScheduleTime)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("schedule_time");
            entity.Property(e => e.EndTime)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("end_time");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.Status).HasColumnName("status");

            entity.HasOne(d => d.User).WithMany(p => p.Appointments)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("appointments_user_id_fkey");
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
            entity.Property(e => e.CourseStatus).HasColumnName("status");
            entity.Property(e => e.GameType).HasColumnName("chess_type");
            entity.Property(e => e.SkillLevel).HasColumnName("skill_level");
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
            entity.Property(e => e.Status).HasColumnName("status");
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
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.EndDate).HasColumnName("end_date");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
            entity.Property(e => e.StartDate).HasColumnName("start_date");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.Property(e => e.EventType).HasColumnName("type");
            entity.Property(e => e.Status).HasColumnName("status");

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
            entity.Property(e => e.Status).HasColumnType("status");

            entity.HasOne(d => d.FromUserNavigation).WithMany(p => p.FriendrequestFromUserNavigations)
                .HasForeignKey(d => d.FromUser)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("friendrequests_from_user_fkey");

            entity.HasOne(d => d.ToUserNavigation).WithMany(p => p.FriendrequestToUserNavigations)
                .HasForeignKey(d => d.ToUser)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("friendrequests_to_user_fkey");
        });

        modelBuilder.Entity<Entities.GameExtension>(entity =>
        {
            entity.HasKey(e => e.ExtensionId).HasName("gameExtensions_pkey");

            entity.ToTable("gameExtensions");

            entity.Property(e => e.ExtensionId).HasColumnName("extension_id");
            entity.Property(e => e.TypeId).HasColumnName("type_id");
            entity.Property(e => e.ExtensionName).HasColumnName("extension_name").HasColumnType("game_extension");

            entity.HasOne(d => d.Type).WithMany(p => p.GameExtensions)
                .HasForeignKey(d => d.TypeId)
                .HasConstraintName("gameExtensions_type_id_fkey");
        });

        modelBuilder.Entity<Entities.GameType>(entity =>
        {
            entity.HasKey(e => e.TypeId).HasName("gameTypes_pkey");

            entity.ToTable("gameTypes");

            entity.Property(e => e.TypeId).HasColumnName("type_id");
            entity.Property(e => e.TypeName).HasColumnName("type_name").HasColumnType("game_type");
        });

        modelBuilder.Entity<Image>(entity =>
        {
            entity.HasKey(e => e.ImageId).HasName("images_pkey");

            entity.ToTable("images");

            entity.Property(e => e.ImageId).HasColumnName("image_id");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.ThreadId).HasColumnName("thread_id");
            entity.Property(e => e.Url)
                .HasMaxLength(255)
                .HasColumnName("url");

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

            entity.Property(e => e.Status).HasColumnName("status");
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
            entity.Property(e => e.VoucherId).HasColumnName("voucher_id");

            entity.HasOne(d => d.User).WithMany(p => p.Orders)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("orders_user_id_fkey");

            entity.HasOne(d => d.Voucher).WithMany(p => p.Orders)
                .HasForeignKey(d => d.VoucherId)
                .HasConstraintName("orders_voucher_id_fkey");

            entity.Property(e => e.Status).HasColumnName("status");
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
            entity.Property(e => e.OrderId).HasColumnName("order_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Order).WithMany(p => p.Payments)
                .HasForeignKey(d => d.OrderId)
                .HasConstraintName("payments_order_id_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.Payments)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("payments_user_id_fkey");
        });

        modelBuilder.Entity<Price>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("prices_pkey");

            entity.ToTable("prices");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CourseId).HasColumnName("course_id");
            entity.Property(e => e.GameTypeId).HasColumnName("game_type_id");
            entity.Property(e => e.MemberFee)
                .HasColumnType("bit(1)")
                .HasColumnName("member_fee");
            entity.Property(e => e.Price1)
                .HasPrecision(10, 2)
                .HasColumnName("price");
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.TeachingSalary)
                .HasColumnType("bit(1)")
                .HasColumnName("teaching_salary");
            entity.Property(e => e.Unit)
                .HasMaxLength(50)
                .HasColumnName("unit");

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

            entity.Property(e => e.Status).HasColumnName("status");
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

            entity.Property(e => e.Type).HasColumnName("room_type");
            entity.Property(e => e.Status).HasColumnName("status");
        });

        modelBuilder.Entity<Table>(entity =>
        {
            entity.HasKey(e => e.TableId).HasName("tables_pkey");

            entity.ToTable("tables");

            entity.Property(e => e.TableId).HasColumnName("table_id");
            entity.Property(e => e.Fee)
                .HasPrecision(10, 2)
                .HasColumnName("fee");
            entity.Property(e => e.GameExtensionId).HasColumnName("gameExtension_id");
            entity.Property(e => e.RoomId).HasColumnName("room_id");

            entity.HasOne(d => d.GameExtension).WithMany(p => p.Tables)
                .HasForeignKey(d => d.GameExtensionId)
                .HasConstraintName("tables_gameExtension_id_fkey");

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

            entity.Property(e => e.Status).HasColumnName("status");
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

            entity.Property(e => e.TicketType).HasColumnName("ticket_type");
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

            entity.Property(e => e.TransactionType).HasColumnName("type");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("users_pkey");

            entity.ToTable("users");

            entity.HasIndex(e => e.Email, "users_email_key").IsUnique();

            entity.HasIndex(e => e.Phone, "users_phone_key").IsUnique();

            entity.HasIndex(e => e.Username, "users_username_key").IsUnique();

            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.UserRole).HasColumnName("role");
            entity.Property(e => e.Address)
                .HasMaxLength(100)
                .HasColumnName("address");
            entity.Property(e => e.AvatarUrl)
                .HasMaxLength(255)
                .HasColumnName("avatar_url");
            entity.Property(e => e.Bio).HasColumnName("bio");
            entity.Property(e => e.Gender).HasColumnName("gender");
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
            entity.Property(e => e.Username)
                .HasMaxLength(50)
                .HasColumnName("username");

            entity.HasOne(d => d.Cart).WithMany(p => p.Users)
                .HasForeignKey(d => d.CartId)
                .HasConstraintName("users_cart_id_fkey");
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
            entity.Property(e => e.Result).HasColumnName("result");
            entity.Property(e => e.ParticipantStatus).HasColumnName("participant_status");
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
            entity.Property(e => e.Status).HasColumnName("status");
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
            entity.Property(e => e.Status).HasColumnName("status");
            entity.HasOne(d => d.User).WithMany(p => p.Wallets)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("wallet_user_id_fkey");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
