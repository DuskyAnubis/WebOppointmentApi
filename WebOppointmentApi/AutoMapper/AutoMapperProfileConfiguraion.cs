﻿using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebOppointmentApi.Models;
using WebOppointmentApi.Dtos;
using WebOppointmentApi.Common;

namespace WebOppointmentApi.AutoMapper
{
    public class AutoMapperProfileConfiguraion : Profile
    {
        public AutoMapperProfileConfiguraion()
            : this("MyProfile")
        {

        }

        protected AutoMapperProfileConfiguraion(string profileName) : base(profileName)
        {
            CreateMap<User, UserOutput>();
            CreateMap<UserCreateInput, User>()
                .ForMember(user => user.PassWord, option => option.MapFrom(input => Encrypt.Md5Encrypt(input.PassWord)));
            CreateMap<User, UserUpdateInput>();
            CreateMap<User, UserSelectOutput>();

            CreateMap<Role, RoleOutput>();
            CreateMap<RoleCreateInput, Role>();
            CreateMap<Role, RoleUpdateInput>();

            CreateMap<Orgnazition, OrgOutput>();
            CreateMap<OrgCreateInput, Orgnazition>();
            CreateMap<Orgnazition, OrgUpdateInput>();
            CreateMap<Orgnazition, OrgTreeOutput>();

            CreateMap<Permission, PermissionOutput>();
            CreateMap<PermissionCreateInput, Permission>();
            CreateMap<Permission, PermissionUpdateInput>();
            CreateMap<Permission, PermissionTreeOutput>();

            CreateMap<RolePermission, PermissionMenuOutput>();

            CreateMap<Dictionary, DictionaryOutput>();
            CreateMap<DictionaryCreateInput, Dictionary>();
            CreateMap<Dictionary, DictonaryUpdateInput>();
            CreateMap<Dictionary, DictionarySelectOutput>();

            CreateMap<Hospital, HosptialOutput>();
            CreateMap<HosptialCreateInput, Hospital>();
            CreateMap<Hospital, HosptialUpdateInput>();

            CreateMap<Scheduling, SchedulingOutput>()
                .ForMember(output => output.OrganazitionId, option => option.MapFrom(s => s.User.OrganazitionId))
                .ForMember(output => output.OrganazitionCode, option => option.MapFrom(s => s.User.Organazition.Code))
                .ForMember(output => output.OrganazitionName, option => option.MapFrom(s => s.User.Organazition.Name))
                .ForMember(output => output.UserRankCode, option => option.MapFrom(s => s.User.UserRankCode))
                .ForMember(output => output.UserRankName, option => option.MapFrom(s => s.User.UserRankName))
                .ForMember(output => output.RegisteredRankCode, option => option.MapFrom(s => s.User.RegisteredRankCode))
                .ForMember(output => output.RegisteredRankName, option => option.MapFrom(s => s.User.RegisteredRankName))
                .ForMember(output => output.RemainCount, option => option.MapFrom(s => s.MaxCount - s.Registereds.Count));
            CreateMap<SchedulingCreateInput, Scheduling>();
            CreateMap<Scheduling, SchedulingUpdateInput>();

            CreateMap<Registered, RegisteredOutput>()
                .ForMember(output => output.OrganazitionId, option => option.MapFrom(r => r.Scheduling.User.OrganazitionId))
                .ForMember(output => output.OrganazitionCode, option => option.MapFrom(r => r.Scheduling.User.Organazition.Code))
                .ForMember(output => output.OrganazitionName, option => option.MapFrom(r => r.Scheduling.User.Organazition.Name))
                .ForMember(output => output.UserId, option => option.MapFrom(r => r.Scheduling.User.Id))
                .ForMember(output => output.UserCode, option => option.MapFrom(r => r.Scheduling.User.Code))
                .ForMember(output => output.UserName, option => option.MapFrom(r => r.Scheduling.User.Name))
                .ForMember(output => output.Price, option => option.MapFrom(r => r.Scheduling.Price))
                .ForMember(output => output.TreatPrice, option => option.MapFrom(r => r.Scheduling.TreatPrice))
                .ForMember(output => output.PlusPrice, option => option.MapFrom(r => r.Scheduling.PlusPrice));
            CreateMap<RegisteredCreateInput, Registered>();
            CreateMap<Registered, RegisteredUpdateInput>();

            CreateMap<Hospital, SynchronizingHospitalInput>()
                .ForMember(input => input.Atype, option => option.MapFrom(h => Convert.ToInt32(h.AccessTypeCode)))
                .ForMember(input => input.Addr, option => option.MapFrom(h => h.Address))
                .ForMember(input => input.Hcodedic, option => option.UseValue("0"));

            CreateMap<Orgnazition, SynchronizingDept>()
                .ForMember(input => input.Pid, option => option.MapFrom(o => o.Parent.ToString()))
                .ForMember(input => input.Pname, option => option.UseValue(""))
                .ForMember(input => input.Addr, option => option.MapFrom(o => o.Address))
                .ForMember(input => input.Kword, option => option.MapFrom(o => o.KeyWord));

            CreateMap<User, SynchronizingDoctor>()
                .ForMember(input => input.Did, option => option.MapFrom(u => u.Organazition.Id.ToString()))
                .ForMember(input => input.Dname, option => option.MapFrom(u => u.Organazition.Name))
                .ForMember(input => input.Gender, option => option.MapFrom(u => Convert.ToUInt32(u.GenderCode)))
                .ForMember(input => input.Rank, option => option.MapFrom(u => u.UserRankName))
                .ForMember(input => input.Rankid, option => option.MapFrom(u => Convert.ToUInt32(u.UserRankCode)))
                .ForMember(input => input.Wrank, option => option.MapFrom(u => u.RegisteredRankName))
                .ForMember(input => input.Kword, option => option.MapFrom(u => u.KeyWord));

            CreateMap<Scheduling, SynchronizingWork>()
                .ForMember(input => input.Endtreat, option => option.MapFrom(s => Convert.ToInt32(s.EndTreatCode)))
                .ForMember(input => input.Hoscode, option => option.UseValue(0))
                .ForMember(input => input.Wtype, option => option.UseValue(0))
                .ForMember(input => input.Docid, option => option.MapFrom(s => s.User.Id.ToString()))
                .ForMember(input => input.Docname, option => option.MapFrom(s => s.User.Name))
                .ForMember(input => input.Mcount, option => option.MapFrom(s => s.MaxCount))
                .ForMember(input => input.Tcount, option => option.MapFrom(s => s.TotalCount))
                .ForMember(input => input.Acount, option => option.MapFrom(s => s.MaxCount - s.Registereds.Count))
                .ForMember(input => input.Wid, option => option.MapFrom(s => s.Id.ToString()))
                .ForMember(input => input.Date, option => option.MapFrom(s => Convert.ToDateTime(s.SurgeryDate).ToString("yyyy-MM-dd")))
                .ForMember(input => input.Pcode, option => option.MapFrom(s => Convert.ToInt32(s.PeriodTypeCode)))
                .ForMember(input => input.Stime, option => option.MapFrom(s => s.StartTime))
                .ForMember(input => input.Etime, option => option.MapFrom(s => s.EndTime))
                .ForMember(input => input.Rankid, option => option.MapFrom(s => Convert.ToInt32(s.User.UserRankCode)))
                .ForMember(input => input.Price, option => option.MapFrom(s => s.Price.ToString()))
                .ForMember(input => input.Ofee, option => option.MapFrom(s => s.TreatPrice.ToString()))
                .ForMember(input => input.Price2, option => option.MapFrom(s => s.PlusPrice.ToString()))
                .ForMember(input => input.Wrank, option => option.MapFrom(s => s.User.RegisteredRankName))
                .ForMember(input => input.Addr, option => option.MapFrom(s => s.Address));

            CreateMap<Registered, SynchronizingOrder>()
                .ForMember(input => input.Oid, option => option.MapFrom(r => r.OrderId))
                .ForMember(input => input.State, option => option.MapFrom(r => Convert.ToInt32(r.RegisteredStateCode)));
        }
    }
}
