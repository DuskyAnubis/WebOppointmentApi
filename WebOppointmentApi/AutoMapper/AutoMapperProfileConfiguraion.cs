using AutoMapper;
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
                .ForMember(output => output.RegisteredRankName, option => option.MapFrom(s => s.User.RegisteredRankName));
            CreateMap<SchedulingCreateInput, Scheduling>();
            CreateMap<Scheduling, SchedulingUpdateInput>();
        }
    }
}
