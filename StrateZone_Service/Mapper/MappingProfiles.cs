using AutoMapper;
using StrateZone_Repository.Entities;
using StrateZone_Repository.Interfaces;
using StrateZone_Service.BusinessModels;
using StrateZone_Service.CustomModels.ResponseModels;
using StrateZone_Service.Interfaces;
using System;
using System.Collections.Generic;

namespace StrateZone_Service.Mapper
{
    public class MappingProfiles : Profile
    {

        public MappingProfiles()
        {   
            CreateMap<User, UserModel>().ReverseMap();

            CreateMap<User, UserResponse>()
                    .ForMember(ur => ur.UserRole, u => u.MapFrom(src => src.UserRole.ToString()))
                    .ForMember(ur => ur.Gender, u => u.MapFrom(src => src.Gender.ToString()))
                    .ForMember(ur => ur.SkillLevel, u => u.MapFrom(src => src.SkillLevel.ToString()))
                    .ForMember(ur => ur.Ranking, u => u.MapFrom(src => src.Ranking.ToString()))
                    .ForMember(ur => ur.AvatarUrl, u => u.MapFrom<UserAvatarResolver>());

            CreateMap<UserModel, UserResponse>()
                    .ForMember(ur => ur.UserRole, u => u.MapFrom(src => src.UserRole.ToString()))
                    .ForMember(ur => ur.Gender, u => u.MapFrom(src => src.Gender.ToString()))
                    .ForMember(ur => ur.SkillLevel, u => u.MapFrom(src => src.SkillLevel.ToString()))
                    .ForMember(ur => ur.Ranking, u => u.MapFrom(src => src.Ranking.ToString()))
                .ReverseMap();

            CreateMap<GameType, GameTypeModel>()
                .ForMember(gtm => gtm.TypeName, gt => gt.MapFrom(src => src.TypeName.ToString()))
                .ReverseMap();

            CreateMap<GameExtension, GameExtensionModel>()
                .ForMember(gtm => gtm.ExtensionName, gt => gt.MapFrom(src => src.ExtensionName.ToString()))
                .ReverseMap();

            CreateMap<Room, RoomModel>().ReverseMap();

            CreateMap<Room, RoomResponse>()
                .ForMember(r => r.Type, rs => rs.MapFrom(src => src.Type.ToString()))
                .ForMember(r => r.Status, rs => rs.MapFrom(src => src.Status.ToString()));

            CreateMap<Table, TableModel>().ReverseMap();
            CreateMap<Appointment, AppointmentModel>().ReverseMap();
            CreateMap<TablesAppointment, TablesAppointmentModel>().ReverseMap();
            
            CreateMap<Message, MessageModel>().ReverseMap();
            CreateMap<Message, MessageResponse>()
                .ForMember(mr => mr.SenderName, u => u.MapFrom(src => src.Sender.Username))
                .ForMember(mr => mr.ReceiverName, u => u.MapFrom(src => src.Receiver.Username));

            CreateMap<Image, ImageModel>().ReverseMap();    
            CreateMap<Price, PriceModel>().ReverseMap();
            CreateMap<Appointmentrequest, AppointmentrequestModel>().ReverseMap();
            CreateMap<Friendrequest, FriendrequestModel>().ReverseMap();
            // add other mappings here
        }
    }
}
