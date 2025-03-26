using Microsoft.EntityFrameworkCore;
using Npgsql;
using StrateZone_Repository.Data;
using StrateZone_Repository.Entities;
using StrateZone_Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrateZone_Repository.Implements
{
    public class PaymentRepository: IPaymentRepository
    {
        private readonly StrateZoneDbContext _context;

        public PaymentRepository(StrateZoneDbContext context)
        {
            _context = context;
        }
    
        public async Task<Payment> CreatePaymentAsync(Payment payment)
        {
            var connection = _context.Database.GetDbConnection();
            await connection.OpenAsync();

            await using var cmd = connection.CreateCommand();
            cmd.CommandText = @"
                    INSERT INTO payments (user_id, order_id, appointment_id, course_id, description, payment_type, status, created_at) 
                    VALUES (@user_id, @order_id, @appointment_id, @course_id, @description, @payment_type, @status::payment_status, @created_at)
                    RETURNING room_id;";

            cmd.Parameters.Add(new NpgsqlParameter("@user_id", payment.UserId));
            cmd.Parameters.Add(new NpgsqlParameter("@order_id", payment.OrderId != null ? payment.OrderId : DBNull.Value));
            cmd.Parameters.Add(new NpgsqlParameter("@appointment_id", payment.AppointmentId != null ? payment.AppointmentId : DBNull.Value));
            cmd.Parameters.Add(new NpgsqlParameter("@course_id", payment.CourseId != null ? payment.CourseId : DBNull.Value));
            cmd.Parameters.Add(new NpgsqlParameter("@description", payment.Description));
            cmd.Parameters.Add(new NpgsqlParameter("@payment_type", payment.PaymentType.ToString()));
            cmd.Parameters.Add(new NpgsqlParameter("@status", payment.PaymentStatus.ToString()));
            cmd.Parameters.Add(new NpgsqlParameter("@created_at", DateTime.UtcNow.AddHours(7)));

            var newPaymentId = await cmd.ExecuteScalarAsync();
            payment.Id = Convert.ToInt32(newPaymentId);

            return payment;
        }

        public Task<List<Payment>> GetPaymentsByTablesAppointmentIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<List<Payment>> GetPaymentsByUserIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<Payment> UpdatePaymentAsync(Payment payment, int id)
        {
            throw new NotImplementedException();
        }
    }
}
