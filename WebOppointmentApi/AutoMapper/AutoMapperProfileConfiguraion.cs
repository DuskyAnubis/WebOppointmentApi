using AutoMapper;
using System;
using System.Linq;
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
                .ForMember(output => output.RemainCount, option => option.MapFrom(s => s.MaxCount - s.Registereds.Where(r => r.RegisteredStateCode != "3").ToList().Count));
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
                .ForMember(input => input.Acount, option => option.MapFrom(s => s.MaxCount - s.Registereds.Where(r => r.RegisteredStateCode != "3").ToList().Count))
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

            CreateMap<Registered, SynchronizingStateOrder>()
                .ForMember(input => input.Oid, option => option.MapFrom(r => r.OrderId))
                .ForMember(input => input.State, option => option.MapFrom(r => Convert.ToInt32(r.RegisteredStateCode)));

            CreateMap<Scheduling, SynchronizingsStop>()
                .ForMember(input => input.Endtreat, option => option.MapFrom(s => Convert.ToInt32(s.EndTreatCode)))
                .ForMember(input => input.Deptid, option => option.MapFrom(s => s.User.OrganazitionId.ToString()))
                .ForMember(input => input.Workid, option => option.MapFrom(s => s.Id.ToString()))
                .ForMember(input => input.Issms, option => option.MapFrom(s => s.IsSms))
                .ForMember(input => input.Time, option => option.MapFrom(s => s.SmsDate))
                .ForMember(input => input.Reason, option => option.MapFrom(s => s.EndTreatReason));

            CreateMap<Registered, SynchronizingsOrder>()
                .ForMember(input => input.Atype, option => option.UseValue(0))
                .ForMember(input => input.Otype, option => option.MapFrom(r => r.RegisteredStateCode == "3" ? 2 : 1))
                .ForMember(input => input.Hospid, option => option.Ignore())
                .ForMember(input => input.Hospname, option => option.Ignore())
                .ForMember(input => input.Deptid, option => option.MapFrom(r => r.Scheduling.User.Organazition.Id.ToString()))
                .ForMember(input => input.Deptname, option => option.MapFrom(r => r.Scheduling.User.Organazition.Name))
                .ForMember(input => input.Docid, option => option.MapFrom(r => r.Scheduling.User.Id.ToString()))
                .ForMember(input => input.Docname, option => option.MapFrom(r => r.Scheduling.User.Name))
                .ForMember(input => input.Date, option => option.MapFrom(r => Convert.ToDateTime(r.DoctorDate).ToString("yyyy-MM-dd")))
                .ForMember(input => input.Time, option => option.MapFrom(r => r.DoctorTime))
                .ForMember(input => input.Workid, option => option.MapFrom(r => r.SchedulingId.ToString()))
                .ForMember(input => input.Sourceid, option => option.UseValue(""))
                .ForMember(input => input.Acount, option => option.MapFrom(r => r.Scheduling.MaxCount - r.Scheduling.Registereds.Where(re => re.RegisteredStateCode != "3").ToList().Count))
                .ForMember(input => input.Orderid, option => option.MapFrom(r => r.OrderId))
                .ForMember(input => input.Phone, option => option.MapFrom(r => r.Phone))
                .ForMember(input => input.Card, option => option.MapFrom(r => r.IDCard))
                .ForMember(input => input.Name, option => option.MapFrom(r => r.Name))
                .ForMember(input => input.Addr, option => option.MapFrom(r => r.Address))
                .ForMember(input => input.Birth, option => option.MapFrom(r => r.Birth))
                .ForMember(input => input.Ptype, option => option.MapFrom(r => Convert.ToInt32(r.MedicalInsuranceCode)))
                .ForMember(input => input.Rtype, option => option.MapFrom(r => r.RegisteredTypeCode))
                .ForMember(input => input.Fromtype, option => option.MapFrom(r => r.FromType))
                .ForMember(input => input.Cid, option => option.MapFrom(r => r.CardNo))
                .ForMember(input => input.Ctype, option => option.MapFrom(r => Convert.ToInt32(r.CardTypeCode)));

            CreateMap<Scheduling, SynchronizingsMed>()
                .ForMember(input => input.Workid, option => option.MapFrom(s => s.Id))
                .ForMember(input => input.Acount, option => option.MapFrom(s => s.Registereds.Where(r => r.RegisteredStateCode == "1").ToList().Count));

            CreateMap<Orgnazition, UpdateDept>()
                .ForMember(output => output.Pid, option => option.MapFrom(o => o.Parent.ToString()))
                .ForMember(output => output.Pname, option => option.UseValue(""))
                .ForMember(output => output.Addr, option => option.MapFrom(o => o.Address))
                .ForMember(output => output.Kword, option => option.MapFrom(o => o.KeyWord));

            CreateMap<User, UpdateDoctor>()
                .ForMember(output => output.Did, option => option.MapFrom(u => u.Organazition.Id.ToString()))
                .ForMember(output => output.Dname, option => option.MapFrom(u => u.Organazition.Name))
                .ForMember(output => output.Gender, option => option.MapFrom(u => Convert.ToUInt32(u.GenderCode)))
                .ForMember(output => output.Rank, option => option.MapFrom(u => u.UserRankName))
                .ForMember(output => output.Rankid, option => option.MapFrom(u => Convert.ToUInt32(u.UserRankCode)))
                .ForMember(output => output.Wrank, option => option.MapFrom(u => u.RegisteredRankName))
                .ForMember(output => output.Kword, option => option.MapFrom(u => u.KeyWord));

            CreateMap<Scheduling, UpdateWork>()
                .ForMember(output => output.Endtreat, option => option.MapFrom(s => Convert.ToInt32(s.EndTreatCode)))
                .ForMember(output => output.Hoscode, option => option.UseValue(0))
                .ForMember(output => output.Wtype, option => option.UseValue(0))
                .ForMember(output => output.Docid, option => option.MapFrom(s => s.User.Id.ToString()))
                .ForMember(output => output.Docname, option => option.MapFrom(s => s.User.Name))
                .ForMember(output => output.Mcount, option => option.MapFrom(s => s.MaxCount))
                .ForMember(output => output.Tcount, option => option.MapFrom(s => s.TotalCount))
                .ForMember(output => output.Acount, option => option.MapFrom(s => s.MaxCount - s.Registereds.Where(r => r.RegisteredStateCode != "3").ToList().Count))
                .ForMember(output => output.Wid, option => option.MapFrom(s => s.Id.ToString()))
                .ForMember(output => output.Date, option => option.MapFrom(s => Convert.ToDateTime(s.SurgeryDate).ToString("yyyy-MM-dd")))
                .ForMember(output => output.Pcode, option => option.MapFrom(s => Convert.ToInt32(s.PeriodTypeCode)))
                .ForMember(output => output.Stime, option => option.MapFrom(s => s.StartTime))
                .ForMember(output => output.Etime, option => option.MapFrom(s => s.EndTime))
                .ForMember(output => output.Rankid, option => option.MapFrom(s => Convert.ToInt32(s.User.UserRankCode)))
                .ForMember(output => output.Price, option => option.MapFrom(s => s.Price.ToString()))
                .ForMember(output => output.Ofee, option => option.MapFrom(s => s.TreatPrice.ToString()))
                .ForMember(output => output.Price2, option => option.MapFrom(s => s.PlusPrice.ToString()))
                .ForMember(output => output.Wrank, option => option.MapFrom(s => s.User.RegisteredRankName))
                .ForMember(output => output.Addr, option => option.MapFrom(s => s.Address));

            CreateMap<Registered, UpdateOrderState>()
                .ForMember(output => output.Oid, option => option.MapFrom(r => r.OrderId))
                .ForMember(output => output.State, option => option.MapFrom(r => Convert.ToInt32(r.RegisteredStateCode)));

            CreateMap<OrderParam, Registered>()
                .ForMember(input => input.SchedulingId, option => option.MapFrom(o => Convert.ToInt32(o.Wid)))
                .ForMember(input => input.RegisteredTypeCode, option => option.MapFrom(o => o.Rtype.ToString()))
                .ForMember(input => input.OrderId, option => option.MapFrom(o => o.Oid))
                .ForMember(input => input.Name, option => option.MapFrom(o => o.Name))
                .ForMember(input => input.Phone, option => option.MapFrom(o => o.Tel))
                .ForMember(input => input.DoctorDate, option => option.MapFrom(o => Convert.ToDateTime(Convert.ToDateTime(o.Time).ToString("yyyy-MM-dd"))))
                .ForMember(input => input.DoctorTime, option => option.MapFrom(o => Convert.ToDateTime(o.Time).ToString("HH:mm")))
                .ForMember(input => input.IDCard, option => option.MapFrom(o => o.Card))
                .ForMember(input => input.GenderCode, option => option.MapFrom(o => o.Gender))
                .ForMember(input => input.IDCard, option => option.MapFrom(o => o.Card))
                .ForMember(input => input.Address, option => option.MapFrom(o => o.Addr))
                .ForMember(input => input.IDCard, option => option.MapFrom(o => o.Card))
                .ForMember(input => input.Birth, option => option.MapFrom(o => o.Birth))
                .ForMember(input => input.FromType, option => option.UseValue("健康山西"))
                .ForMember(input => input.MedicalInsuranceCode, option => option.MapFrom(o => o.Ptype.ToString()))
                .ForMember(input => input.CardTypeCode, option => option.MapFrom(o => o.Ctype.ToString()))
                .ForMember(input => input.CardNo, option => option.MapFrom(o => o.Cid))
                .ForMember(input => input.MedicalTypeCode, option => option.MapFrom(o => o.Mtype.ToString()))
                .ForMember(input => input.RegisteredStateCode, option => option.UseValue("0"))
                .ForMember(input => input.RegisteredStateName, option => option.UseValue("待就诊"))
                .ForMember(input => input.Status, option => option.UseValue("同步"));

            CreateMap<Registered, Order>()
                .ForMember(output => output.Wid, option => option.MapFrom(r => r.SchedulingId.ToString()))
                .ForMember(output => output.Iid, option => option.UseValue("0"))
                .ForMember(output => output.Inum, option => option.UseValue("0"))
                .ForMember(output => output.Oid, option => option.MapFrom(r => r.OrderId))
                .ForMember(output => output.Price, option => option.MapFrom(r => r.Scheduling.Price))
                .ForMember(output => output.Ofee, option => option.MapFrom(r => r.Scheduling.TreatPrice))
                .ForMember(output => output.Date, option => option.MapFrom(r => Convert.ToDateTime(r.DoctorDate).ToString("yyyy-MM-dd")))
                .ForMember(output => output.Time, option => option.MapFrom(r => r.DoctorTime))
                .ForMember(output => output.Count, option => option.MapFrom(r => r.Scheduling.MaxCount - r.Scheduling.Registereds.Where(re => re.RegisteredStateCode != "3").ToList().Count))
                .ForMember(output => output.Dcount, option => option.MapFrom(r => r.Scheduling.TotalCount - r.Scheduling.Registereds.Where(re => re.RegisteredStateCode != "3").ToList().Count))
                .ForMember(output => output.Tdate, option => option.MapFrom(r => Convert.ToDateTime(r.TransactionDate).ToString("yyyy-MM-dd HH:mm:ss")))
                .ForMember(output => output.Orderid, option => option.MapFrom(r => r.OrderId));

            CreateMap<Registered, CancelOrder>()
                .ForMember(output => output.Wid, option => option.MapFrom(r => r.SchedulingId.ToString()))
                .ForMember(output => output.Count, option => option.MapFrom(r => r.Scheduling.MaxCount - r.Scheduling.Registereds.Where(re => re.RegisteredStateCode != "3").ToList().Count))
                .ForMember(output => output.Dcount, option => option.MapFrom(r => r.Scheduling.TotalCount - r.Scheduling.Registereds.Where(re => re.RegisteredStateCode != "3").ToList().Count))
                .ForMember(output => output.Tdate, option => option.MapFrom(r => Convert.ToDateTime(r.TransactionDate).ToString("yyyy-MM-dd HH:mm:ss")));

            CreateMap<Registered, UpdateOrder>()
                .ForMember(output => output.Wid, option => option.MapFrom(r => r.SchedulingId.ToString()))
                .ForMember(output => output.Iid, option => option.UseValue("0"))
                .ForMember(output => output.Inum, option => option.MapFrom(r => r.Id.ToString()))
                .ForMember(output => output.Oid, option => option.MapFrom(r => r.OrderId))
                .ForMember(output => output.Price, option => option.MapFrom(r => r.Scheduling.Price))
                .ForMember(output => output.Ofee, option => option.MapFrom(r => r.Scheduling.TreatPrice))
                .ForMember(output => output.Date, option => option.MapFrom(r => Convert.ToDateTime(r.DoctorDate).ToString("yyyy-MM-dd")))
                .ForMember(output => output.Time, option => option.MapFrom(r => r.DoctorTime))
                .ForMember(output => output.Tdate, option => option.MapFrom(r => Convert.ToDateTime(r.TransactionDate).ToString("yyyy-MM-dd HH:mm:ss")))
                .ForMember(output => output.Card, option => option.MapFrom(r => r.IDCard))
                .ForMember(output => output.Cid, option => option.MapFrom(r => r.CardNo))
                .ForMember(output => output.Ctype, option => option.MapFrom(r => Convert.ToInt32(r.CardTypeCode)))
                .ForMember(output => output.Name, option => option.MapFrom(r => r.Name))
                .ForMember(output => output.Tel, option => option.MapFrom(r => r.Phone))
                .ForMember(output => output.State, option => option.MapFrom(r => Convert.ToInt32(r.RegisteredStateCode)))
                .ForMember(output => output.Pcode, option => option.MapFrom(r => Convert.ToInt32(r.Scheduling.PeriodTypeCode)));

            CreateMap<划价临时库, PendingPaymentDetails>()
                .ForMember(output => output.CItemName, option => option.MapFrom(o => o.名称))
                .ForMember(output => output.NPrice, option => option.MapFrom(o => o.单价))
                .ForMember(output => output.NNum, option => option.MapFrom(o => o.数量))
                .ForMember(output => output.CUnit, option => option.MapFrom(o => o.单位))
                .ForMember(output => output.NMoney, option => option.MapFrom(o => o.金额))
                .ForMember(output => output.CStandard, option => option.MapFrom(o => o.规格));
        }
    }
}
