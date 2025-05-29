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
    public class TablesAppointmentAllowExtendResolver : IValueResolver<TablesAppointment, TablesAppointmentModel, bool>
    {
        private readonly ITablesAppointmentService _tableService;

        public TablesAppointmentAllowExtendResolver(ITablesAppointmentService tableService)
        {
            _tableService = tableService;
        }

        public bool Resolve(TablesAppointment source, TablesAppointmentModel destination, bool destMember, ResolutionContext context)
        {
            return _tableService.CheckAllowTablesAppointmentExtend(source.Id).Result;
        }
    }

    public class TablesAppointmentRAllowExtendResolver : IValueResolver<TablesAppointment, TablesAppointmentResponse, bool>
    {
        private readonly ITablesAppointmentService _tableService;

        public TablesAppointmentRAllowExtendResolver(ITablesAppointmentService tableService)
        {
            _tableService = tableService;
        }

        public bool Resolve(TablesAppointment source, TablesAppointmentResponse destination, bool destMember, ResolutionContext context)
        {
            return _tableService.CheckAllowTablesAppointmentExtend(source.Id).Result;
        }
    }
}
