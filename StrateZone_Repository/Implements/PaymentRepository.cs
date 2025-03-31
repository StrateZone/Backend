using CloudinaryDotNet.Actions;
using MealHunt_Repositories.Pagination;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using NpgsqlTypes;
using StrateZone_Repository.Data;
using StrateZone_Repository.Entities;
using StrateZone_Repository.Interfaces;
using StrateZone_Repository.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static StrateZone_Repository.Parameters.PostgreEnums;

namespace StrateZone_Repository.Implements
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly StrateZoneDbContext _context;

        public PaymentRepository(StrateZoneDbContext context)
        {
            _context = context;
        }

        public async Task<Payment> CreatePaymentAsync(Payment payment)
        {
            try
            {
                var connection = _context.Database.GetDbConnection();

                if (connection.State != System.Data.ConnectionState.Open) await connection.OpenAsync();

                await using var cmd = connection.CreateCommand();
                cmd.CommandText = @"
                    INSERT INTO payments (user_id, order_id, tables_appointment_id, course_id, description, payment_type, status, created_at) 
                    VALUES (@user_id, @order_id, @appointment_id, @course_id, @description, @payment_type, @status::payment_status, @created_at)
                    RETURNING id;";

                cmd.Parameters.Add(new NpgsqlParameter("@user_id", payment.UserId));
                cmd.Parameters.Add(new NpgsqlParameter("@order_id", payment.OrderId != null ? payment.OrderId : DBNull.Value));
                cmd.Parameters.Add(new NpgsqlParameter("@appointment_id", payment.TablesAppointmentId != null ? payment.TablesAppointmentId : DBNull.Value));
                cmd.Parameters.Add(new NpgsqlParameter("@course_id", payment.CourseId != null ? payment.CourseId : DBNull.Value));
                cmd.Parameters.Add(new NpgsqlParameter("@description", payment.Description != null ? payment.Description : DBNull.Value));
                cmd.Parameters.Add(new NpgsqlParameter("@payment_type", payment.PaymentType.ToString()));
                cmd.Parameters.Add(new NpgsqlParameter("@status", payment.PaymentStatus.ToString()));
                cmd.Parameters.Add(new NpgsqlParameter("@created_at", DateTime.SpecifyKind(DateTime.UtcNow.AddHours(7), DateTimeKind.Unspecified)));

                var newPaymentId = await cmd.ExecuteScalarAsync();
                payment.Id = Convert.ToInt32(newPaymentId);

                return payment;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Payment> UpdatePaymentAsync(Payment payment, int id)
        {
            var existingPayment = await _context.Payments.FindAsync(id) ?? throw new Exception("Appointment request with this ID does not exist");
            existingPayment.Id = id;

            var parameters = new List<NpgsqlParameter>();
            var sql = new StringBuilder("UPDATE payments SET ");

            if (payment.UserId.HasValue)
            {
                sql.Append("user_id = @user_id, ");
                parameters.Add(new NpgsqlParameter("@user_id", payment.UserId.Value));
            }

            if (payment.OrderId.HasValue)
            {
                sql.Append("order_id = @order_id, ");
                parameters.Add(new NpgsqlParameter("@order_id", payment.OrderId.Value));
            }

            if (payment.TablesAppointmentId.HasValue)
            {
                sql.Append("tables_appointment_id = @tables_appointment_id, ");
                parameters.Add(new NpgsqlParameter("@tables_appointment_id", payment.TablesAppointmentId.Value));
            }

            if (payment.CourseId.HasValue)
            {
                sql.Append("course_id = @course_id, ");
                parameters.Add(new NpgsqlParameter("@course_id", payment.CourseId.Value));
            }

            sql.Append("status = @status::payment_status, ");
            parameters.Add(new NpgsqlParameter("@status", payment.PaymentStatus.ToString()));

            if (!string.IsNullOrEmpty(payment.Description))
            {
                sql.Append("description = @description, ");
                parameters.Add(new NpgsqlParameter("@description", payment.Description));
            }

            if (payment.CreatedAt.HasValue)
            {
                sql.Append("created_at = @created_at, ");
                parameters.Add(new NpgsqlParameter("@created_at", payment.CreatedAt.Value));
            }

            sql.Append("payment_type = @type, ");
            parameters.Add(new NpgsqlParameter("@type", payment.PaymentType.ToString()));

            sql.Remove(sql.Length - 2, 2);
            sql.Append(" WHERE id = @id");
            parameters.Add(new NpgsqlParameter("@id", id));

            await _context.Database.ExecuteSqlRawAsync(sql.ToString(), parameters.ToArray());

            var updatedPayment = await _context.Payments.FindAsync(id);
            return updatedPayment;
        }

        public async Task<List<Payment>> GetPaymentsByTablesAppointmentIdAsync(int id)
        {
            try
            {
                return await _context.Payments.Where(p => p.TablesAppointmentId == id).ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<PagedList<Payment>> GetPaymentsByUserIdAsync(int id, PaymentParameters parameters)
        {
            try
            {
                var result = _context.Payments
                                .FromSqlRaw(@"SELECT *  
                                    FROM payments
                                    WHERE user_id = @UserId
                                        AND status = ANY(@PaymentStatuses::public.payment_status[])
                                        AND payment_type = ANY(@PaymentTypes)
                                    ORDER BY created_at DESC",

                                    new NpgsqlParameter("@UserId", id),

                                    new NpgsqlParameter("@PaymentStatuses", NpgsqlDbType.Array | NpgsqlDbType.Text)
                                    { Value = parameters.PaymentStatuses.Select(rt => rt.ToString()).ToArray() ?? (object)DBNull.Value },

                                    new NpgsqlParameter("@PaymentTypes", NpgsqlDbType.Array | NpgsqlDbType.Text)
                                    { Value = parameters.PaymentTypes.Select(rt => rt.ToString()).ToArray() ?? (object)DBNull.Value }
                                )
                                .AsQueryable();

                return await PagedList<Payment>.ToPagedList(result, parameters.PageNumber, parameters.PageSize);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<PagedList<Payment>> GetPaymentsAsync(PaymentParameters parameters)
        {
            try
            {
                var result = _context.Payments
                                .FromSqlRaw(@"SELECT *  
                                    FROM payments
                                    WHERE status = ANY(@PaymentStatuses::public.payment_status[])
                                        AND payment_type = ANY(@PaymentTypes)
                                    ORDER BY created_at DESC",

                                    new NpgsqlParameter("@PaymentStatuses", NpgsqlDbType.Array | NpgsqlDbType.Text)
                                    { Value = parameters.PaymentStatuses.Select(rt => rt.ToString()).ToArray() ?? (object)DBNull.Value },

                                    new NpgsqlParameter("@PaymentTypes", NpgsqlDbType.Array | NpgsqlDbType.Text)
                                    { Value = parameters.PaymentTypes.Select(rt => rt.ToString()).ToArray() ?? (object)DBNull.Value }
                                )
                                .AsQueryable();

                return await PagedList<Payment>.ToPagedList(result, parameters.PageNumber, parameters.PageSize);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
