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

}
