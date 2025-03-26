using StrateZone_Repository.Entities;
using StrateZone_Repository.Implements;
using StrateZone_Repository.Interfaces;
using StrateZone_Service.CustomModels.RequestModels;
using StrateZone_Service.CustomModels.ResponseModels;
using StrateZone_Service.Interfaces;
using StrateZone_Repository.Parameters;
﻿using AutoMapper;
using StrateZone_Repository.Entities;
using StrateZone_Repository.Interfaces;
using StrateZone_Service.BusinessModels;
using StrateZone_Service.Interfaces;
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
        private readonly IMapper _mapper;

        public PaymentService(IAppointmentRepository appointmentRepository,
            ITablesAppointmentRepository tablesAppointmentRepository,
            IAppointmentrequestRepository appointmentrequestRepository,
            IPaymentRepository paymentRepository,
            IWalletRepository walletRepository,
            ITransactionRepository transactionRepository,
            IMapper mapper)
        {
            _appointmentRepository = appointmentRepository;
            _tablesAppointmentRepository = tablesAppointmentRepository;
            _appointmentrequestRepository = appointmentrequestRepository;
            _paymentRepository = paymentRepository;
            _walletRepository = walletRepository;
            _transactionRepository = transactionRepository;
            _mapper = mapper;
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
                foreach (var tablesAppointment in appointment.TablesAppointments)
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
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        

        public async Task<PaymentModel> CreatePaymentAsync(PaymentModel paymentModel)
        {
            try
            {
                var payment = _mapper.Map<Payment>(paymentModel);
                var result = await _paymentRepository.CreatePaymentAsync(payment);
                return _mapper.Map<PaymentModel>(result);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<PaymentModel>> GetPaymentsByTablesAppointmentIdAsync(int id)
        {
            try
            {
                var result = await _paymentRepository.GetPaymentsByTablesAppointmentIdAsync(id);
                return _mapper.Map<List<PaymentModel>>(result);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<PaymentModel>> GetPaymentsByUserIdAsync(int id)
        {
            try
            {
                var result = await _paymentRepository.GetPaymentsByUserIdAsync(id);
                return _mapper.Map<List<PaymentModel>>(result);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<PaymentModel> UpdatePaymentAsync(PaymentModel paymentModel, int id)
        {
            try
            {
                var payment = _mapper.Map<Payment>(paymentModel);
                var result = await _paymentRepository.UpdatePaymentAsync(payment, id);
                return _mapper.Map<PaymentModel>(result);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
