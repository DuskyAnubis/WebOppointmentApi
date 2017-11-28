using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using WebOppointmentApi.Common;

namespace WebOppointmentApi.Dtos
{
    public class OppointmentApiQuery
    {
        public string Head { get; set; }
        public string Body { get; set; }
    }

    public class OppointmentApiResult
    {
        public string Head { get; set; }
        public string Body { get; set; }
    }

    public class OppointmentApiHeader
    {
        public string Token { get; set; }
        public string Version { get; set; }
        public string Fromtype { get; set; }
        public string Sessionid { get; set; }
        public string Time { get; set; }
    }

    public class OppointmentApiBody
    {
        public int Code { get; set; }
        public string Msg { get; set; }
        public string Result { get; set; }
    }
    #region 平台提供接口

    #region 同步医院信息
    public class SynchronizingHospitalInput
    {
        public string Id { get; set; }
        public int Atype { get; set; }
        public string Name { get; set; }
        public string Addr { get; set; }
        public string Info { get; set; }
        public string Logourl { get; set; }
        public string Picurl { get; set; }
        public string Hcodedic { get; set; }
    }
    #endregion

    #region 同步科室信息
    public class SynchronizingDeptParam
    {
        [Required(ErrorMessage = "请输入Opcode")]
        public int Opcode { get; set; }
        public int[] Ids { get; set; }
    }

    public class SynchronizingDept
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Pid { get; set; }
        public string Pname { get; set; }
        public string Tel { get; set; }
        public string Addr { get; set; }
        public string Info { get; set; }
        public string Kword { get; set; }
        public string Logourl { get; set; }
        public string Picurl { get; set; }

    }

    public class SynchronizingDeptInput
    {
        public string Hospid { get; set; }
        public int Opcode { get; set; }

        public List<SynchronizingDept> Depts { get; set; }

        public bool ShouldSerializeDepts()
        {
            return Depts.Count > 0;
        }
    }
    #endregion

    #region 同步医生信息
    public class SynchronizingDoctorParam
    {
        public SynchronizingDoctorParam()
        {
            OrgId = 0;
        }

        [Required(ErrorMessage = "请输入Opcode")]
        public int Opcode { get; set; }
        public int OrgId { get; set; }
        public int[] Ids { get; set; }
    }

    public class SynchronizingDoctor
    {
        public string Id { get; set; }
        public string Did { get; set; }
        public string Dname { get; set; }
        public string Name { get; set; }
        public int Gender { get; set; }
        public string Rank { get; set; }
        public int Rankid { get; set; }
        public string Wrank { get; set; }
        public string Tel { get; set; }
        public string Info { get; set; }
        public string Kword { get; set; }
        public string Picurl { get; set; }
    }

    public class SynchronizingDoctorInput
    {
        public string Hospid { get; set; }
        public int Opcode { get; set; }

        public List<SynchronizingDoctor> Doctors { get; set; }

        public bool ShouldSerializeDoctors()
        {
            return Doctors.Count > 0;
        }
    }
    #endregion

    #region 同步排班信息
    public class SynchronizingWorkParam
    {
        [Required(ErrorMessage = "请输入Opcode")]
        public int Opcode { get; set; }
        public int OrgId { get; set; }
        public string Date { get; set; }
        public int[] Ids { get; set; }
    }

    public class SynchronizingWork
    {
        public int Endtreat { get; set; }
        public int Hoscode { get; set; }
        public int Wtype { get; set; }
        public string Docid { get; set; }
        public string Docname { get; set; }
        public int Mcount { get; set; }
        public int Tcount { get; set; }
        public int Acount { get; set; }
        public string Wid { get; set; }
        public string Date { get; set; }
        public int Pcode { get; set; }
        public string Stime { get; set; }
        public string Etime { get; set; }
        public int Rankid { get; set; }
        public string Price { get; set; }
        public string Ofee { get; set; }
        public string Price2 { get; set; }
        public string Wrank { get; set; }
        public string Addr { get; set; }
    }

    public class SynchronizingWorkInput
    {
        public string Hospid { get; set; }
        public int Opcode { get; set; }
        public int Atype { get; set; }
        public string Deptid { get; set; }

        public List<SynchronizingWork> Works { get; set; }

        public bool ShouldSerializeWorks()
        {
            return Works.Count > 0;
        }
    }
    #endregion

    #region 同步预约执行情况
    public class SynchronizingOrderStateParam
    {
        [Required(ErrorMessage = "请输入就诊日期")]
        public string Date { get; set; }
    }

    public class SynchronizingStateOrder
    {
        public string Oid { get; set; }
        public int State { get; set; }
    }

    public class SynchronizingOrderStateInput
    {
        public string Hospid { get; set; }

        public List<SynchronizingStateOrder> Orders { get; set; }

        public bool ShouldSerializeOrders()
        {
            return Orders.Count > 0;
        }
    }
    #endregion

    #region 同步停诊情况
    public class SynchronizingsStopParam
    {
        [Required(ErrorMessage = "Id不能为空")]
        public int Id { get; set; }
    }

    public class SynchronizingsStop
    {
        public int Endtreat { get; set; }
        public string Deptid { get; set; }
        public string Workid { get; set; }
        public int Issms { get; set; }
        public string Time { get; set; }
        public string Reason { get; set; }
    }

    public class SynchronizingsStopInput
    {
        public string Hospid { get; set; }

        public List<SynchronizingsStop> Values { get; set; }

        public bool ShouldSerializeValues()
        {
            return Values.Count > 0;
        }
    }
    #endregion

    #region 同步医院预约挂号
    public class SynchronizingsOrderParam
    {
        [Required(ErrorMessage = "Id不能为空")]
        public int Id { get; set; }
    }

    public class SynchronizingsOrder
    {
        public int Atype { get; set; }
        public int Otype { get; set; }
        public string Hospid { get; set; }
        public string Hospname { get; set; }
        public string Deptid { get; set; }
        public string Deptname { get; set; }
        public string Docid { get; set; }
        public string Docname { get; set; }
        public string Date { get; set; }
        public string Time { get; set; }
        public string Workid { get; set; }
        public string Sourceid { get; set; }
        public int Acount { get; set; }
        public string Orderid { get; set; }
        public string Phone { get; set; }
        public string Card { get; set; }
        public string Name { get; set; }
        public string Addr { get; set; }
        public string Birth { get; set; }
        public int Ptype { get; set; }
        public string Rtype { get; set; }
        public string Fromtype { get; set; }
        public string Cid { get; set; }
        public int Ctype { get; set; }
    }
    #endregion

    #region 同步实际取号量
    public class SynchronizingsMedParam
    {
        [Required(ErrorMessage = "请输入出诊日期")]
        public string Date { get; set; }
    }

    public class SynchronizingsMed
    {
        public string Workid { get; set; }
        public int Acount { get; set; }
    }

    public class SynchronizingsMedInput
    {
        public string Hospid { get; set; }

        public List<SynchronizingsMed> Works { get; set; }

        public bool ShouldSerializeWorks()
        {
            return Works.Count > 0;
        }
    }
    #endregion

    #endregion

    #region HIS提供接口

    #region 更新科室信息
    public class UpdateDeptParam
    {
        public string Hospid { get; set; }
        public string Id { get; set; }
    }

    public class UpdateDept
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Pid { get; set; }
        public string Pname { get; set; }
        public string Tel { get; set; }
        public string Addr { get; set; }
        public string Info { get; set; }
        public string Kword { get; set; }
        public string Logourl { get; set; }
        public string Picurl { get; set; }
    }

    public class UpdateDeptOutput
    {
        public string Hospid { get; set; }

        public List<UpdateDept> Depts { get; set; }

        public bool ShouldSerializeDepts()
        {
            return Depts.Count > 0;
        }
    }
    #endregion

    #region 更新医生信息
    public class UpdateDoctorParam
    {
        public string Hospid { get; set; }
        public string Deptid { get; set; }
        public string Id { get; set; }
    }

    public class UpdateDoctor
    {
        public string Id { get; set; }
        public string Did { get; set; }
        public string Dname { get; set; }
        public string Name { get; set; }
        public int Gender { get; set; }
        public string Rank { get; set; }
        public int Rankid { get; set; }
        public string Wrank { get; set; }
        public string Tel { get; set; }
        public string Info { get; set; }
        public string Kword { get; set; }
        public string Picurl { get; set; }
    }

    public class UpdateDoctorOutput
    {
        public string Hospid { get; set; }

        public List<UpdateDoctor> Doctors { get; set; }

        public bool ShouldSerializeDoctors()
        {
            return Doctors.Count > 0;
        }
    }
    #endregion

    #region 更新排班信息
    public class UpdateWorkParam
    {
        public string Hospid { get; set; }
        public int Optype { get; set; }
        public int Atype { get; set; }
        public string Deptid { get; set; }
        public string Docid { get; set; }
        public string Dates { get; set; }
        public string Ids { get; set; }
    }

    public class UpdateWork
    {
        public int Endtreat { get; set; }
        public int Hoscode { get; set; }
        public int Wtype { get; set; }
        public string Docid { get; set; }
        public string Docname { get; set; }
        public int Mcount { get; set; }
        public int Tcount { get; set; }
        public int Acount { get; set; }
        public string Wid { get; set; }
        public string Date { get; set; }
        public int Pcode { get; set; }
        public string Stime { get; set; }
        public string Etime { get; set; }
        public int Rankid { get; set; }
        public string Price { get; set; }
        public string Ofee { get; set; }
        public string Price2 { get; set; }
        public string Wrank { get; set; }
        public string Addr { get; set; }
    }

    public class UpdateWorkOutput
    {
        public string Hospid { get; set; }
        public int Opcode { get; set; }
        public int Atype { get; set; }
        public string Deptid { get; set; }

        public List<UpdateWork> Works { get; set; }

        public bool ShouldSerializeWorks()
        {
            return Works.Count > 0;
        }
    }
    #endregion

    #region 更新预约执行情况
    public class UpdateOrderStateParam
    {
        public string Hospid { get; set; }
        public int Optype { get; set; }
        public int Atype { get; set; }
        public string Deptid { get; set; }
        public string Docid { get; set; }
        public string Dates { get; set; }
        public string Ids { get; set; }
    }

    public class UpdateOrderState
    {
        public string Oid { get; set; }
        public int State { get; set; }
    }

    public class UpdateOrderStateOutput
    {
        public string Hospid { get; set; }

        public List<UpdateOrderState> Orders { get; set; }

        public bool ShouldSerializeOrders()
        {
            return Orders.Count > 0;
        }
    }
    #endregion

    #region 平台预约挂号
    public class OrderParam
    {
        public int Atype { get; set; }
        public string Wid { get; set; }
        public string Iid { get; set; }
        public string Inum { get; set; }
        public string Oid { get; set; }
        public int Ptype { get; set; }
        public int Rtype { get; set; }
        public string Name { get; set; }
        public string Tel { get; set; }
        public string Time { get; set; }
        public string Card { get; set; }
        public string Gender { get; set; }
        public string Addr { get; set; }
        public string Birth { get; set; }
        public string Cid { get; set; }
        public int Ctype { get; set; }
        public int Mtype { get; set; }
    }

    public class Order
    {
        public string Wid { get; set; }
        public string Iid { get; set; }
        public string Inum { get; set; }
        public string Oid { get; set; }
        public int Price { get; set; }
        public int Ofee { get; set; }
        public string Date { get; set; }
        public string Time { get; set; }
        public int Count { get; set; }
        public int Dcount { get; set; }
        public string Tdate { get; set; }
        public string Orderid { get; set; }
    }

    public class OrderOutput
    {
        public int Code { get; set; }
        public string Msg { get; set; }
        public Order Result { get; set; }
    }
    #endregion

    #region 平台取消预约挂号
    public class CancelOrderParam
    {
        public int Atype { get; set; }
        public string Wid { get; set; }
        public string Iid { get; set; }
        public string Inum { get; set; }
        public string Oid { get; set; }
        public string Name { get; set; }
        public string Tel { get; set; }
        public string Cid { get; set; }
        public int Ctype { get; set; }
    }

    public class CancelOrder
    {
        public string Wid { get; set; }
        public int Count { get; set; }
        public int Dcount { get; set; }
        public string Tdate { get; set; }
    }

    public class CancelOrderOutput
    {
        public int Code { get; set; }
        public string Msg { get; set; }
        public CancelOrder Result { get; set; }
    }
    #endregion

    #region 更新实际取号量
    public class UpdateMedParam
    {
        public string Hospid { get; set; }
        public string Workid { get; set; }
    }

    public class UpdateMed
    {
        public int Acount { get; set; }
    }
    #endregion

    #region 绑定诊疗卡校验
    public class BindCardParam
    {
        public string ZlkCardNum { get; set; }
        public string PatientName { get; set; }
        public string IdentityCard { get; set; }
        public string Tel { get; set; }
    }

    public class BindCard
    {
        public string Code { get; set; }
        public string Msg { get; set; }
        public string ZlkCardNum { get; set; }
        public string PatientName { get; set; }
        public string IdentityCard { get; set; }
        public string Tel { get; set; }
    }
    #endregion

    #region 心跳接口
    public class HeartBeatParam
    {
        public string Time { get; set; }
    }

    public class HeartBeatOutput
    {
        public int Code { get; set; }
        public string Msg { get; set; }
        public string Time { get; set; }
    }
    #endregion

    #region 获取订单信息
    public class UpdateOrderParam
    {
        public string Card { get; set; }
        public string Date { get; set; }
    }

    public class UpdateOrder
    {
        public string Wid { get; set; }
        public string Iid { get; set; }
        public string Inum { get; set; }
        public string Oid { get; set; }
        public int Price { get; set; }
        public int Ofee { get; set; }
        public string Date { get; set; }
        public string Time { get; set; }
        public string Tdate { get; set; }
        public string Card { get; set; }
        public string Cid { get; set; }
        public int Ctype { get; set; }
        public string Name { get; set; }
        public string Tel { get; set; }
        public int State { get; set; }
        public int Pcode { get; set; }
    }

    public class UpdateOrderOutput
    {
        public string Hospid { get; set; }
        public List<UpdateOrder> Orders { get; set; }

        public bool ShouldSerializeOrders()
        {
            return Orders.Count > 0;
        }
    }
    #endregion

    #endregion
}
