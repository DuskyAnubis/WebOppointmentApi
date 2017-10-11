using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace WebOppointmentApi.Dtos
{
    public class OppointmentApiHeaderInput
    {
        public string Token { get; set; }
        public string Version { get; set; }
        public string Fromtype { get; set; }
        public string Sessionid { get; set; }
        public string Time { get; set; }
    }

    public class OppointmentApiBodyOutput
    {
        public int Code { get; set; }
        public string Msg { get; set; }
        public string Result { get; set; }
    }

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
        [Required(ErrorMessage = "请输入科室ID")]
        public int OrgId { get; set; }
        [Required(ErrorMessage = "请输入出诊日期")]
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
        public int Deptid { get; set; }

        public List<SynchronizingWork> Works { get; set; }

        public bool ShouldSerializeWorks()
        {
            return Works.Count > 0;
        }
    }
    #endregion

    #region 同步预约执行情况
    public class SynchronizingOrderParam
    {
        public string Date { get; set; }
    }

    public class SynchronizingOrder
    {
        public string Oid { get; set; }
        public int State { get; set; }
    }

    public class SynchronizingOrderInput
    {
        public string Hospid { get; set; }

        public List<SynchronizingOrder> Orders { get; set; }

        public bool ShouldSerializeOrders()
        {
            return Orders.Count > 0;
        }
    }
    #endregion
}
