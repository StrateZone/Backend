using AutoMapper;
using StrateZone_Repository.Entities;
using StrateZone_Service.BusinessModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrateZone_Service.Mapper
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<User, UserModel>().ReverseMap();
            CreateMap<GameType, GameTypeModel>().ForMember(gtm => gtm.TypeName, gt => gt.MapFrom(src => src.TypeName.ToString()));
            CreateMap<GameExtension, GameExtensionModel>().ForMember(gtm => gtm.ExtensionName, gt => gt.MapFrom(src => src.ExtensionName.ToString()));
            CreateMap<Appointment, AppointmentModel>().ReverseMap();

            // add other mapping here
        }
    }
}
