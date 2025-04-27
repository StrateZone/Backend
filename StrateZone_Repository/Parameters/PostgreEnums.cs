namespace StrateZone_Repository.Parameters
{
    public class PostgreEnums
    {
        public enum AppointmentStatus { pending, confirmed, cancelled, completed, expired, unpaid, refunded, checked_in, incoming, incompleted, unfinished }
        public enum CourseSlotStatus { upcoming, in_progress, completed, cancelled }
        public enum CourseStatus { open, closed, in_progress, completed, cancelled }
        public enum EventStatus { upcoming, ongoing, completed, cancelled }
        public enum TournamentStatus { upcoming, enrolling, ongoing, completed, cancelled }
        public enum EventType { tournament, promotion }
        public enum GameExtensionEnum { bullet, lightning, flip, traditional }
        public enum GameTypeEnum { chess, xiangqi, go }
        public enum Gender { male, female }
        public enum MessageStatus { read, unread }
        public enum NotificationType { appointment, order, tables_appointment, appointment_request_from, appointment_request_to, thread, friend, friend_request, community, tables_appointment_invitations_timedout, tables_appointment_ongoing }
        public enum OrderStatus { pending, shipped, delivered, cancelled }
        public enum ParticipantStatus { enrolled, drop_out, in_progress, completed }
        public enum ProductStatus { available, out_of_stock, discontinued }
        public enum Ranking { basic, silver, gold, platinum }
        public enum RequestStatus { pending, accepted, rejected, cancelled, expired, accepted_by_others, table_cancelled }
        public enum RoomStatus { available, unavailable, closed }
        public enum RoomType { study, premium, basic, openspaced }
        public enum SkillLevel { beginner, intermediate, advanced }
        public enum ThreadStatus { published, rejected, pending, deleted, drafted, hidden, edit_pending }
        public enum TicketType { withdrawal, feedback, other }
        public enum TransactionType { deposit, withdrawal, refund, payment }
        public enum PaymentStatus { unpaid, paid }
        public enum PaymentType { order, appointment, course, membership }
        public enum UserCourseResult { passed, failed }
        public enum UserRole { RegisteredUser, Member, Instructor, Staff, Admin }
        public enum VoucherStatus { active, expired }
        public enum WalletStatus { active, closed }
        public enum TagStatus { active, hidden }
        public enum TableStatus { active, out_of_service }
        public enum UserLabel { none, top_contributor }
        public enum UserStatus { Active, Unactivated, Suspended }
    }
}
