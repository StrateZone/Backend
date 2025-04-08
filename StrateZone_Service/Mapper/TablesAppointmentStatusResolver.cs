using AutoMapper;
using StrateZone_Repository.Entities;
using StrateZone_Service.BusinessModels;
using StrateZone_Service.CustomModels.ResponseModels;
using StrateZone_Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrateZone_Service.Mapper
{
    public class TablesAppointmentStatusResolver : IValueResolver<TablesAppointment, TablesAppointmentResponse, string>
    {
        private readonly IAppointmentrequestService _appointmentRequestService;

        public TablesAppointmentStatusResolver(IAppointmentrequestService appointmentrequestService)
        {
            _appointmentRequestService = appointmentrequestService;
        }

        public string? Resolve(TablesAppointment source, TablesAppointmentResponse destination, string destMember, ResolutionContext context)
        {
            var result = _appointmentRequestService.GetTablesAppointmentStatus(source.Id).Result;
            return result;
        }
    }
}
