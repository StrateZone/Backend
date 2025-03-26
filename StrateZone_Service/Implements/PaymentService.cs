using AutoMapper;
using StrateZone_Repository.Entities;
using StrateZone_Repository.Interfaces;
using StrateZone_Service.BusinessModels;
using StrateZone_Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrateZone_Service.Implements
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IMapper _mapper;

        public PaymentService(IPaymentRepository paymentRepository, IMapper mapper)
        {
            _paymentRepository = paymentRepository;
            _mapper = mapper;
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
