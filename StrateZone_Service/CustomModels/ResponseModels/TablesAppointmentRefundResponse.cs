using StrateZone_Service.BusinessModels;

namespace StrateZone_Service.CustomModels.ResponseModels
{
    public enum RefundStatus { cancellation_fail, no_refund, no_refund_while_refund_for_invited_user, refund_50_percentage_of_total, refund_100_percentage_of_total };

    public class TablesAppointmentRefundResponse
    {
        public TablesAppointmentModel TablesAppointmentModel { get; set; }

        public RefundStatus RefundStatus { get; set; }

        public string Message { get; set; }

        public decimal RefundAmount { get; set; }

        public int NumerOfTablesCancelledThisWeek { get; set; } = 0;

        public DateTime? CancellationTime { get; set; }

        public DateTime? Cancellation_Block_TimeGate { get; set; }

        public DateTime? Cancellation_PartialRefund_TimeGate { get; set; }

    }
}