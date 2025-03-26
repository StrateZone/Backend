using StrateZone_Repository.Entities;
using StrateZone_Repository.Implements;
using StrateZone_Repository.Interfaces;
using StrateZone_Service.CustomModels.RequestModels;
using StrateZone_Service.CustomModels.ResponseModels;
using StrateZone_Service.Interfaces;
using StrateZone_Repository.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StrateZone_Service.BusinessModels;

namespace StrateZone_Service.Implements
{
    public class PaymentService : IPaymentService
    {
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly ITablesAppointmentRepository _tablesAppointmentRepository;
        private readonly IAppointmentrequestRepository _appointmentrequestRepository;
        private readonly IPaymentRepository _paymentRepository;
        private readonly IWalletRepository _walletRepository;
        private readonly ITransactionRepository _transactionRepository;

        public PaymentService(IAppointmentRepository appointmentRepository,
            ITablesAppointmentRepository tablesAppointmentRepository,
            IAppointmentrequestRepository appointmentrequestRepository,
            IPaymentRepository paymentRepository,
            IWalletRepository walletRepository,
            ITransactionRepository transactionRepository)
        {
            _appointmentRepository = appointmentRepository;
            _tablesAppointmentRepository = tablesAppointmentRepository;
            _appointmentrequestRepository = appointmentrequestRepository;
            _paymentRepository = paymentRepository;
            _walletRepository = walletRepository;
            _transactionRepository = transactionRepository;
        }
        public async Task<ApiResponse<AppointmentModel>> CreatePaymentBooking(AppointmentModel appointment)
        {
            try
            {
                var userWallet = await _walletRepository.GetWalletByUserIdAsync(appointment.UserId);
                if (userWallet.Balance < appointment.TotalPrice)
                {
                    return new ApiResponse<AppointmentModel>
                    {
                        Success = true,
                        StatusCode = 500,
                        Message = "Payment failed due to not enough in balance",
                        Data = null
                    };
                }
                userWallet.Balance -= appointment.TotalPrice;
                await _walletRepository.UpdateWalletAsync(userWallet, userWallet.WalletId);
                foreach(var tablesAppointment in appointment.TablesAppointments)
                {
                    var updatingPayment = (await _paymentRepository.GetPaymentsByTablesAppointmentIdAsync(tablesAppointment.Id)).SingleOrDefault(p => p.UserId == appointment.UserId);
                    updatingPayment.PaymentStatus = PostgreEnums.PaymentStatus.paid;
                    await _paymentRepository.UpdatePaymentAsync(updatingPayment, updatingPayment.Id);
                }

                var result = new ApiResponse<AppointmentModel>
                {
                    Success = true,
                    StatusCode = 201,
                    Message = "Payment success",
                    Data = appointment
                };

                return result;
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
