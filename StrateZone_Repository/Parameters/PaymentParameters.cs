using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static StrateZone_Repository.Parameters.PostgreEnums;

namespace StrateZone_Repository.Parameters
{
    public class PaymentParameters : PagedListParameters
    {
        public PaymentStatus[] PaymentStatuses { get; set; } = [PaymentStatus.unpaid, PaymentStatus.paid];
        public PaymentType[] PaymentTypes { get; set; } = [PaymentType.order, PaymentType.appointment, PaymentType.course, PaymentType.membership];
    }
}
