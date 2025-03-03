using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using static StrateZone_Repository.Parameters.PostgreEnums;

namespace StrateZone_Repository.Entities;

public partial class User
{
    public int UserId { get; set; }

    public int? CartId { get; set; }

    public string? Username { get; set; }

    [Column(TypeName = "user_role")]
    public UserRole UserRole { get; set; }

    public string? Email { get; set; }

    public string? Phone { get; set; }

    public string? Password { get; set; }

    /// <summary>
    /// Depends on Role
    /// </summary>
    public string Status { get; set; } = null!;

    [Column(TypeName = "gender")]
    public Gender Gender { get; set; }

    public string? Address { get; set; }

    public string? AvatarUrl { get; set; }

    public string? Bio { get; set; }

    public int? Points { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

    public virtual Cart? Cart { get; set; }

    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

    public virtual ICollection<Course> Courses { get; set; } = new List<Course>();

    public virtual ICollection<CoursesSlot> CoursesSlots { get; set; } = new List<CoursesSlot>();

    public virtual ICollection<Event> Events { get; set; } = new List<Event>();

    public virtual ICollection<Friendlist> FriendlistFriends { get; set; } = new List<Friendlist>();

    public virtual ICollection<Friendlist> FriendlistUsers { get; set; } = new List<Friendlist>();

    public virtual ICollection<Friendrequest> FriendrequestFromUserNavigations { get; set; } = new List<Friendrequest>();

    public virtual ICollection<Friendrequest> FriendrequestToUserNavigations { get; set; } = new List<Friendrequest>();

    public virtual ICollection<Like> Likes { get; set; } = new List<Like>();

    public virtual ICollection<Message> MessageReceivers { get; set; } = new List<Message>();

    public virtual ICollection<Message> MessageSenders { get; set; } = new List<Message>();

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual ICollection<Thread> Threads { get; set; } = new List<Thread>();

    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();

    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();

    public virtual ICollection<UsersCourse> UsersCourses { get; set; } = new List<UsersCourse>();

    public virtual ICollection<Wallet> Wallets { get; set; } = new List<Wallet>();
}
