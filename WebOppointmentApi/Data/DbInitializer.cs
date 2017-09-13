using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WebOppointmentApi.Models;
using WebOppointmentApi.Common;

namespace WebOppointmentApi.Data
{
    public static class DbInitializer
    {
        public static void Initialize(ApiContext context)
        {
            //context.Database.MigrateAsync();

            //if (context.Users.Any())
            //{
            //    return;
            //}

            //var dictionaries = new List<Dictionary>
            //{
            //    new Dictionary{TypeCode="AccessType",TypeName="接入类型",Code="0",Name="序号预约",RemarkName="",RemarkValue="" },
            //    new Dictionary{TypeCode="AccessType",TypeName="接入类型",Code="1",Name="分时预约",RemarkName="",RemarkValue="" },
            //    new Dictionary{TypeCode="AccessType",TypeName="接入类型",Code="2",Name="分时段预约",RemarkName="",RemarkValue="" },

            //    new Dictionary{TypeCode="OrgType",TypeName="部门类型",Code="01",Name="临床科室",RemarkName="",RemarkValue="" },
            //    new Dictionary{TypeCode="OrgType",TypeName="部门类型",Code="02",Name="医务医技",RemarkName="",RemarkValue="" },
            //    new Dictionary{TypeCode="OrgType",TypeName="部门类型",Code="03",Name="其他部门",RemarkName="",RemarkValue="" },

            //    new Dictionary{TypeCode="UserType",TypeName="用户类型",Code="01",Name="医生",RemarkName="",RemarkValue="" },
            //    new Dictionary{TypeCode="UserType",TypeName="用户类型",Code="02",Name="护士",RemarkName="",RemarkValue="" },
            //    new Dictionary{TypeCode="UserType",TypeName="用户类型",Code="03",Name="医务",RemarkName="",RemarkValue="" },
            //    new Dictionary{TypeCode="UserType",TypeName="用户类型",Code="04",Name="其他",RemarkName="",RemarkValue="" },

            //    new Dictionary{TypeCode="Gender",TypeName="性别",Code="1",Name="男",RemarkName="",RemarkValue="" },
            //    new Dictionary{TypeCode="Gender",TypeName="性别",Code="2",Name="女",RemarkName="",RemarkValue="" },

            //    new Dictionary{TypeCode="UserRank",TypeName="职称",Code="01",Name="执业医师",RemarkName="",RemarkValue="" },
            //    new Dictionary{TypeCode="UserRank",TypeName="职称",Code="02",Name="主治医师",RemarkName="",RemarkValue="" },
            //    new Dictionary{TypeCode="UserRank",TypeName="职称",Code="03",Name="副主任医师",RemarkName="",RemarkValue="" },
            //    new Dictionary{TypeCode="UserRank",TypeName="职称",Code="04",Name="主任医师",RemarkName="",RemarkValue="" },
            //    new Dictionary{TypeCode="UserRank",TypeName="职称",Code="11",Name="护士",RemarkName="",RemarkValue="" },
            //    new Dictionary{TypeCode="UserRank",TypeName="职称",Code="12",Name="护士长",RemarkName="",RemarkValue="" },
            //    new Dictionary{TypeCode="UserRank",TypeName="职称",Code="99",Name="其他",RemarkName="",RemarkValue="" },

            //    new Dictionary{TypeCode="RegisteredRank",TypeName="挂号级别",Code="01",Name="执业医师号",RemarkName="挂号费",RemarkValue="0" },
            //    new Dictionary{TypeCode="RegisteredRank",TypeName="挂号级别",Code="02",Name="主治医师号",RemarkName="挂号费",RemarkValue="0" },
            //    new Dictionary{TypeCode="RegisteredRank",TypeName="挂号级别",Code="03",Name="副主任医师号",RemarkName="挂号费",RemarkValue="0" },
            //    new Dictionary{TypeCode="RegisteredRank",TypeName="挂号级别",Code="04",Name="主任医师号",RemarkName="挂号费",RemarkValue="0" },
            //    new Dictionary{TypeCode="RegisteredRank",TypeName="挂号级别",Code="05",Name="特邀专家号",RemarkName="挂号费",RemarkValue="0" },
            //    new Dictionary{TypeCode="RegisteredRank",TypeName="挂号级别",Code="06",Name="其他",RemarkName="挂号费",RemarkValue="0" },

            //    new Dictionary{TypeCode="SchedulingType",TypeName="排班类别",Code="1",Name="医生排班",RemarkName="",RemarkValue="" },
            //    new Dictionary{TypeCode="SchedulingType",TypeName="排班类别",Code="2",Name="职称排班",RemarkName="",RemarkValue="" },

            //    new Dictionary{TypeCode="EndTreat",TypeName="是否停诊",Code="0",Name="未停诊",RemarkName="",RemarkValue="" },
            //    new Dictionary{TypeCode="EndTreat",TypeName="是否停诊",Code="1",Name="已停诊",RemarkName="",RemarkValue="" },

            //    new Dictionary{TypeCode="PeriodType",TypeName="班次时段",Code="10",Name="上午",RemarkName="",RemarkValue="" },
            //    new Dictionary{TypeCode="PeriodType",TypeName="班次时段",Code="20",Name="中午",RemarkName="",RemarkValue="" },
            //    new Dictionary{TypeCode="PeriodType",TypeName="班次时段",Code="30",Name="下午",RemarkName="",RemarkValue="" },
            //    new Dictionary{TypeCode="PeriodType",TypeName="班次时段",Code="40",Name="夜班",RemarkName="",RemarkValue="" },
            //    new Dictionary{TypeCode="PeriodType",TypeName="班次时段",Code="50",Name="全天",RemarkName="",RemarkValue="" },
            //    new Dictionary{TypeCode="PeriodType",TypeName="班次时段",Code="60",Name="急诊上午",RemarkName="",RemarkValue="" },
            //    new Dictionary{TypeCode="PeriodType",TypeName="班次时段",Code="70",Name="急诊下午",RemarkName="",RemarkValue="" },
            //    new Dictionary{TypeCode="PeriodType",TypeName="班次时段",Code="80",Name="夏令",RemarkName="",RemarkValue="" },
            //    new Dictionary{TypeCode="PeriodType",TypeName="班次时段",Code="90",Name="绿色通道",RemarkName="",RemarkValue="" },

            //    new Dictionary{TypeCode="OppointmentState",TypeName="预约状态",Code="0",Name="未预约",RemarkName="",RemarkValue="" },
            //    new Dictionary{TypeCode="OppointmentState",TypeName="预约状态",Code="1",Name="已预约",RemarkName="",RemarkValue="" },

            //    new Dictionary{TypeCode="RegisteredType",TypeName="挂号类别",Code="1",Name="初诊",RemarkName="",RemarkValue="" },
            //    new Dictionary{TypeCode="RegisteredType",TypeName="挂号类别",Code="2",Name="本地复诊",RemarkName="",RemarkValue="" },
            //    new Dictionary{TypeCode="RegisteredType",TypeName="挂号类别",Code="3",Name="本地术后复诊",RemarkName="",RemarkValue="" },
            //    new Dictionary{TypeCode="RegisteredType",TypeName="挂号类别",Code="4",Name="外地复诊",RemarkName="",RemarkValue="" },
            //    new Dictionary{TypeCode="RegisteredType",TypeName="挂号类别",Code="5",Name="外地术后复诊",RemarkName="",RemarkValue="" },

            //    new Dictionary{TypeCode="MedicalInsurance",TypeName="医保类别",Code="3",Name="普通",RemarkName="",RemarkValue="" },
            //    new Dictionary{TypeCode="MedicalInsurance",TypeName="医保类别",Code="801",Name="省医保",RemarkName="",RemarkValue="" },
            //    new Dictionary{TypeCode="MedicalInsurance",TypeName="医保类别",Code="851",Name="市医保",RemarkName="",RemarkValue="" },
            //    new Dictionary{TypeCode="MedicalInsurance",TypeName="医保类别",Code="856",Name="居民医保",RemarkName="",RemarkValue="" },
            //    new Dictionary{TypeCode="MedicalInsurance",TypeName="医保类别",Code="701",Name="铁路医保",RemarkName="",RemarkValue="" },
            //    new Dictionary{TypeCode="MedicalInsurance",TypeName="医保类别",Code="2000",Name="新农合",RemarkName="",RemarkValue="" },
            //    new Dictionary{TypeCode="MedicalInsurance",TypeName="医保类别",Code="9999",Name="其他",RemarkName="",RemarkValue="" },

            //    new Dictionary{TypeCode="CardType",TypeName="患者卡类别",Code="1",Name="社保卡",RemarkName="",RemarkValue="" },
            //    new Dictionary{TypeCode="CardType",TypeName="患者卡类别",Code="3",Name="诊疗卡",RemarkName="",RemarkValue="" },

            //    new Dictionary{TypeCode="MedicalType",TypeName="患者类别",Code="1",Name="成人",RemarkName="",RemarkValue="" },
            //    new Dictionary{TypeCode="MedicalType",TypeName="患者类别",Code="2",Name="儿童",RemarkName="",RemarkValue="" },

            //    new Dictionary{TypeCode="RegisteredState",TypeName="挂号状态",Code="0",Name="待就诊",RemarkName="",RemarkValue="" },
            //    new Dictionary{TypeCode="RegisteredState",TypeName="挂号状态",Code="1",Name="取号",RemarkName="",RemarkValue="" },
            //    new Dictionary{TypeCode="RegisteredState",TypeName="挂号状态",Code="2",Name="未取号（爽约）",RemarkName="",RemarkValue="" },
            //    new Dictionary{TypeCode="RegisteredState",TypeName="挂号状态",Code="3",Name="已取消",RemarkName="",RemarkValue="" }
            //};
            //dictionaries.ForEach(dictionary => context.Add(dictionary));
            //context.SaveChanges();

            //var organazitions = new List<Organazition>
            //{
            //    new Organazition{ Name="预约诊疗平台",Code="System",OrgTypeCode="",OrgTypeName="", Parent=0,Tel="",Address="",Info="部门根目录", KeyWord="",LogoUrl="",PicUrl="", Status ="正常"},
            //    new Organazition{ Name="信息中心",Code="InfoCenter",OrgTypeCode="03",OrgTypeName="其他部门", Parent=1,Tel="",Address="",Info="信息中心", KeyWord="",LogoUrl="",PicUrl="", Status ="正常"}
            //};
            //organazitions.ForEach(organazition => context.Add(organazition));
            //context.SaveChanges();

            //var roles = new List<Role>
            //{
            //    new Role{ Code="admin", Name="系统管理员",Description="拥有系统所有权限",Status="正常"},
            //    new Role{ Code="data", Name="数据管理员",Description="管理医院部门人员排班等",Status="正常"},
            //    new Role{ Code="user", Name="普通用户",Description="系统默认权限",Status="正常"}
            //};
            //roles.ForEach(role => context.Add(role));
            //context.SaveChanges();

            //var permissions = new List<Permission>
            //{
            //    new Permission{Name="预约诊疗平台",Code="System", Parent=0,Path="",Property="目录",Description="系统功能根目录",Icon="",Order=1,Status="正常" },
            //    new Permission{Name="系统设置",Code="SystemSetting",Parent=1,Path="",Property="目录",Description="系统设置",Icon="",Order=2,Status="正常" },
            //    new Permission{Name="挂号平台",Code="BaseSetting",Parent=1,Path="",Property="目录",Description="挂号平台",Icon="",Order=1,Status="正常" },
            //    new Permission{Name="数据字典",Code="DictionaryManager",Parent=2,Path="",Property="菜单",Description="数据字典",Icon="",Order=1,Status="正常" },
            //    new Permission{Name="医院信息",Code="HospitalInfo",Parent=2,Path="",Property="菜单",Description="医院信息",Icon="",Order=2,Status="正常" },
            //    new Permission{Name="部门管理",Code="OrganazitionManager",Parent=2,Path="",Property="菜单",Description="部门管理",Icon="",Order=3,Status="正常" },
            //    new Permission{Name="功能管理",Code="PowerManager",Parent=2,Path="",Property="菜单",Description="功能管理",Icon="",Order=4,Status="正常" },
            //    new Permission{Name="角色管理",Code="RoleManager",Parent=2,Path="",Property="菜单",Description="角色管理",Icon="",Order=5,Status="正常" },
            //    new Permission{Name="用户管理",Code="UserManager",Parent=2,Path="",Property="菜单",Description="用户管理",Icon="",Order=6,Status="正常" }
            //};
            //permissions.ForEach(power => context.Add(power));
            //context.SaveChanges();

            //var rolePermissions = new List<RolePermission>
            //{
            //    new RolePermission{ RoleId=1,PermissionId=2},
            //    new RolePermission{ RoleId=1,PermissionId=3},
            //    new RolePermission{ RoleId=1,PermissionId=4},
            //    new RolePermission{ RoleId=1,PermissionId=5},
            //    new RolePermission{ RoleId=1,PermissionId=6},
            //    new RolePermission{ RoleId=1,PermissionId=7},
            //    new RolePermission{ RoleId=1,PermissionId=8},
            //    new RolePermission{ RoleId=1,PermissionId=9}
            //};
            //rolePermissions.ForEach(rolePermission => context.Add(rolePermission));
            //context.SaveChanges();

            //var users = new List<User>
            //{
            //    new User{Code="admin", Name="admin",PassWord=Encrypt.Md5Encrypt("123"),UserTypeCode="04",UserTypeName="其他",RoleId=1,OrganazitionId=2,GenderCode="1",GenderName="男",UserRankCode="99",UserRankName="其他",RegisteredRankCode="06",RegisteredRankName="其他",Tel="",Info="",KeyWord="",PicUrl="", Status="正常"}
            //};
            //users.ForEach(user => context.Add(user));
            //context.SaveChanges();
        }
    }
}