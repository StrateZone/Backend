using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrateZone_Repository.Parameters
{
    [JsonConverter(typeof(StringEnumConverter))]
    public class PostgreEnums
    {
        public enum CourseSlotStatus { Upcoming, InProgress, Completed, Cancelled }
        public enum CourseStatus { Open, Closed, InProgress, Completed, Cancelled }
        public enum EventStatus { Upcoming, Ongoing, Completed, Cancelled }
        public enum EventType { Tournament, Promotion }
        public enum GameExtension { Bullet, Lightning, Flip, Traditional }
        public enum GameType { Chess, Xiangqi, Go }
        public enum Gender { Male, Female }
        public enum MessageStatus { Read, Unread }
        public enum OrderStatus { Pending, Shipped, Delivered, Cancelled }
        public enum ParticipantStatus { Enrolled, DropOut, InProgress, Completed }
        public enum ProductStatus { Available, OutOfStock, Discontinued }
        public enum Ranking { Basic, Silver, Gold, Platinum }
        public enum RequestStatus { Pending, Accepted, Rejected, Cancelled }
        public enum RoomStatus { Available, Unavailable, Closed }
        public enum RoomType { Study, Appointment }
        public enum SkillLevel { Beginner, Intermediate, Advanced }
        public enum ThreadStatus { Published, Rejected, Pending, Deleted }
        public enum TicketType { Withdrawal, Feedback, Other }
        public enum TransactionType { Deposit, Withdrawal, Refund }
        public enum UserCourseResult { PASSED, FAILED }
        public enum UserRole { RegisteredUser, Member, Instructor, Staff, Admin }
        public enum VoucherStatus { Active, Expired }
        public enum WalletStatus { Active, Closed }

    }
}
