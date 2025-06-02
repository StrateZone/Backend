using AutoMapper;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using StrateZone_Repository.DTO;
using StrateZone_Repository.Entities;
using StrateZone_Repository.Interfaces;
using StrateZone_Service.BusinessModels;
using StrateZone_Service.CustomModels.RequestModels;
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
            CreateMap<User, UserModel>()
                .ForMember(ur => ur.AvatarUrl, u => u.MapFrom<UserAvatarResolver>())
                .ForMember(ur => ur.UserRole, u => u.MapFrom(src => src.UserRole.ToString()))
                .ForMember(ur => ur.UserLabel, u => u.MapFrom(src => src.UserLabel.ToString()))
                .ReverseMap();

            CreateMap<User, UserResponse>()
                    .ForMember(ur => ur.UserRole, u => u.MapFrom(src => src.UserRole.ToString()))
                    .ForMember(ur => ur.Gender, u => u.MapFrom(src => src.Gender.ToString()))
                    .ForMember(ur => ur.UserLabel, u => u.MapFrom(src => src.UserLabel.ToString()))
                    .ForMember(ur => ur.Status, u => u.MapFrom(src => src.Status.ToString()))
                    .ForMember(ur => ur.AvatarUrl, u => u.MapFrom<UserResponseAvatarResolver>())
                    .ReverseMap();

            CreateMap<User, UserManagementResponse>()
                    .ForMember(ur => ur.UserRole, u => u.MapFrom(src => src.UserRole.ToString()))
                    .ForMember(ur => ur.UserLabel, u => u.MapFrom(src => src.UserLabel.ToString()))
                    .ForMember(ur => ur.Status, u => u.MapFrom(src => src.Status.ToString()))
                    .ForMember(ur => ur.AvatarUrl, u => u.MapFrom<UserManagementResponseAvatarResolver>())
                    .ReverseMap();

            CreateMap<User, UserDashboardResponse>()
                    .ForMember(ur => ur.UserRole, u => u.MapFrom(src => src.UserRole.ToString()))
                    .ReverseMap();

            CreateMap<UserResponse, OpponentResponse>();
            CreateMap<UserResponse, FriendResponse>();

            CreateMap<UserModel, UserResponse>()
                    .ForMember(ur => ur.Gender, u => u.MapFrom(src => src.Gender.ToString()))
                    .ForMember(ur => ur.Status, u => u.MapFrom(src => src.Status.ToString()))
                .ReverseMap();

            CreateMap<GameType, GameTypeModel>()
                .ForMember(gtm => gtm.TypeName, gt => gt.MapFrom(src => src.TypeName.ToString()))
                .ReverseMap();

            CreateMap<Room, RoomModel>().ReverseMap();

            CreateMap<Image, ImageModel>().ReverseMap();

            CreateMap<Room, RoomResponse>()
                .ForMember(r => r.Type, rs => rs.MapFrom(src => src.Type.ToString()))
                .ForMember(r => r.Status, rs => rs.MapFrom(src => src.Status.ToString()));

            CreateMap<StrateZone_Repository.Entities.Table, TableModel>().ReverseMap();

            CreateMap<StrateZone_Repository.Entities.Table, TableResponse>()
                    .ForMember(tr => tr.RoomName, u => u.MapFrom(src => src.Room.RoomName))
                    .ForMember(tr => tr.RoomDescription, u => u.MapFrom(src => src.Room.Description))
                    .ForMember(tr => tr.IsForMonthlyBooking, u => u.MapFrom(src => src.Room.IsForMonthlyBooking))
                    .ForMember(tr => tr.RoomType, u => u.MapFrom(src => src.Room.Type.ToString()))
                    .ReverseMap();

            CreateMap<Appointment, AppointmentModel>()
                .ForMember(tr => tr.Status, u => u.MapFrom(src => src.Status.ToString()))
                .ReverseMap();

            CreateMap<Appointment, AppointmentResponse>()
                .ForMember(tr => tr.Status, u => u.MapFrom(src => src.Status.ToString()))
                .ReverseMap();

            CreateMap<TablesAppointment, TablesAppointmentModel>()
                .ForMember(tr => tr.Status, u => u.MapFrom(src => src.Status.ToString()))
                .ForMember(tr => tr.AllowExtend, u => u.MapFrom<TablesAppointmentAllowExtendResolver>())
                .ReverseMap();

            CreateMap<TablesAppointment, TablesAppointmentResponse>()
                .ForMember(tr => tr.Status, u => u.MapFrom(src => src.Status.ToString()))
                .ForMember(tr => tr.AllowExtend, u => u.MapFrom<TablesAppointmentRAllowExtendResolver>())
                .ReverseMap();

            CreateMap<TablesAppointmentModel, TablesAppointmentResponse>().ReverseMap();

            CreateMap<TableModel, TableResponse>().ReverseMap();

            CreateMap<Image, ImageModel>().ReverseMap();    
            CreateMap<Price, PriceModel>().ReverseMap();

            CreateMap<Appointmentrequest, AppointmentrequestModel>()
                .ForMember(tr => tr.Status, u => u.MapFrom(src => src.Status.ToString()))
                .ReverseMap();

            CreateMap<Appointmentrequest, AppointmentrequestResponse>();
            CreateMap<AppointmentrequestModel, AppointmentrequestResponse>();

            CreateMap<Friendrequest, FriendrequestModel>()
                .ForMember(tr => tr.Status, u => u.MapFrom(src => src.Status.ToString()))
                .ReverseMap();

            CreateMap<Friendlist, FriendlistModel>().ReverseMap();

            CreateMap<Payment, PaymentModel>()
                .ForMember(mr => mr.PaymentStatus, u => u.MapFrom(src => src.PaymentStatus.ToString()))
                .ForMember(mr => mr.PaymentType, u => u.MapFrom(src => src.PaymentType.ToString()))
                .ReverseMap();

            CreateMap<Notification, NotificationModel>().ReverseMap();
            CreateMap<Voucher, VoucherModel>().ReverseMap();
            CreateMap<Wallet, WalletModel>().ReverseMap();
            CreateMap<Transaction, TransactionModel>().ReverseMap();
            CreateMap<StrateZone_Repository.Entities.System, SystemModel>().ReverseMap();
            CreateMap<AbnormalDay, AbnormalDayModel>().ReverseMap();
            CreateMap<PointsHistory, PointsHistoryModel>().ReverseMap();

            CreateMap<StrateZone_Repository.Entities.Thread, ThreadModel>()
                .ForMember(tr => tr.Status, u => u.MapFrom(src => src.Status.ToString()))
                .ForMember(ur => ur.ThumbnailUrl, u => u.MapFrom<ThreadThumbnailResolver>())
                .ReverseMap();

            CreateMap<ThreadDTO, ThreadModel>()
                 .ForMember(tr => tr.Status, u => u.MapFrom(src => src.Status.ToString()))
                 .ForMember(ur => ur.ThumbnailUrl, u => u.MapFrom<ThreadDTOThumbnailResolver>())
                 .ReverseMap();

            CreateMap<Comment, CommentModel>().ReverseMap();
            CreateMap<Tag, TagModel>().ReverseMap();
            CreateMap<ThreadsTag, ThreadsTagModel>()
                .ReverseMap();
            CreateMap<Like, LikeModel>().ReverseMap();  
            CreateMap<Expense, ExpenseModel>().ReverseMap();
            // add other mappings here
        }
    }
}
