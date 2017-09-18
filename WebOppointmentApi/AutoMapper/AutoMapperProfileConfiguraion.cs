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
        }
    }
}
