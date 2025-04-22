using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrateZone_Service.CustomModels.ResponseModels
{
    public class TransactionMonthResponse
    {
        public string Month { get; set; }

        public int TotalDays { get; set; }

        public decimal Deposit => transactionDayResponses.Select(t => t.Deposit).Sum();

        public decimal Booking => transactionDayResponses.Select(t => t.Booking).Sum();

        public decimal MemberShip => transactionDayResponses.Select(t => t.MemberShip).Sum();

        public decimal Spending => transactionDayResponses.Select(t => t.Spending).Sum();

        public decimal Refund => transactionDayResponses.Select(t => t.Refund).Sum();

        public decimal Voucher => transactionDayResponses.Select(t => t.Voucher).Sum();

        public List<TransactionDayResponse> transactionDayResponses { get; set; } = new();
    }

    public class TransactionDayResponse
    {
        public int DayOfMonth { get; set; }

        public decimal Deposit { get; set; } = 0;

        public decimal Booking { get; set; }

        public decimal MemberShip { get; set; }

        public decimal Spending { get; set; }

        public decimal Refund { get; set; }

        public decimal Voucher { get; set; }
    }

    public class ProfitMonthResponse
    {
        public string Month { get; set; }
        public int TotalDays { get; set; }
        public decimal Profit => ProfitDailyResponses.Select(p => p.Profit).Sum();
        public List<ProfitDailyResponse> ProfitDailyResponses { get; set; } = new();
    }

    public class ProfitDailyResponse
    {
        public int DayOfMonth { get; set; }
        public decimal Profit { get; set; }
    }

    public class UserMonthResponse
    {
        public string Month { get; set; }

        public int TotalDays { get; set; }

        public int UsersJoined => UserDailyResponses.Select(u => u.UsersJoined).Sum();

        public List<UserDailyResponse> UserDailyResponses { get; set; } = new();
    }

    public class UserDailyResponse
    {
        public int DayOfMonth { get; set; }

        public int UsersJoined { get; set; }
    }

    public class ThreadMonthResponse
    {
        public string Month { get; set; }

        public int TotalDays { get; set; }

        public int ThreadsCreated => ThreadDailyResponse.Select(u => u.ThreadsCreated).Sum();

        public int PublishedCount => ThreadDailyResponse.Select(u => u.PublishedCount).Sum();

        public int PendingCount => ThreadDailyResponse.Select(u => u.PendingCount).Sum();

        public int RejectedCount => ThreadDailyResponse.Select(u => u.RejectedCount).Sum();

        public int HiddenCount => ThreadDailyResponse.Select(u => u.HiddenCount).Sum();

        public int DeletedCount => ThreadDailyResponse.Select(u => u.DeletedCount).Sum();

        public List<ThreadDailyResponse> ThreadDailyResponse { get; set; } = new();
    }

    public class ThreadDailyResponse
    {
        public int DayOfMonth { get; set; }

        public int ThreadsCreated { get; set; }

        public int PublishedCount { get; set; }

        public int PendingCount { get; set; }

        public int RejectedCount { get; set; }

        public int HiddenCount { get; set; }

        public int DeletedCount { get; set; }
    }

    public class TablesAppointmentsMonthResponse
    {
        public string Month { get; set; }

        public int TotalDays { get; set; }

        public int TablesAppointmentBooked => TablesAppointmentsDailyResponse.Select(u => u.TablesAppointmentBooked).Sum();

        public int CompletedCount => TablesAppointmentsDailyResponse.Select(u => u.CompletedCount).Sum();

        public int CancelledCount => TablesAppointmentsDailyResponse.Select(u => u.CancelledCount).Sum();

        public int ExpiredCount => TablesAppointmentsDailyResponse.Select(u => u.ExpiredCount).Sum();

        public int FutureCount => TablesAppointmentsDailyResponse.Select(u => u.FutureCount).Sum();

        public List<TablesAppointmentsDailyResponse> TablesAppointmentsDailyResponse { get; set; } = new();
    }

    public class TablesAppointmentsDailyResponse
    {
        public int DayOfMonth { get; set; }

        public int TablesAppointmentBooked { get; set; }

        public int CompletedCount { get; set; }

        public int CancelledCount { get; set; }

        public int ExpiredCount { get; set; }

        public int FutureCount { get; set; }
    }

    public class MembershipMonthResponse
    {
        public string Month { get; set; }

        public int TotalDays { get; set; }

        public int MembershipsPurchased => MembershipDailyResponse.Select(u => u.MembershipsPurchased).Sum();

        public List<MembershipDailyResponse> MembershipDailyResponse { get; set; } = new();
    }

    public class MembershipDailyResponse
    {
        public int DayOfMonth { get; set; }

        public int MembershipsPurchased { get; set; }
    }
}
