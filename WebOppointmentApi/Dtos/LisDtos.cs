using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebOppointmentApi.Dtos
{
    #region 平台提供接口

    #endregion

    #region HIS提供接口

    #region 门诊患者查询
    public class PatientParam
    {
        public string Oid { get; set; }
        public string PatientName { get; set; }
        public string Tel { get; set; }
        public string IdentityCard { get; set; }
    }

    public class PatientResult
    {
        [JsonProperty("PatientCode")]
        public string PatientCode { get; set; }
        [JsonProperty("PatientName")]
        public string PatientName { get; set; }
        [JsonProperty("Sex")]
        public string Sex { get; set; }
        [JsonProperty("identityCard")]
        public string IdentityCard { get; set; }
        [JsonProperty("tel")]
        public string Tel { get; set; }
    }

    public class PatientOutput
    {
        [JsonProperty("code")]
        public int Code { get; set; }
        [JsonProperty("msg")]
        public string Msg { get; set; }
        [JsonProperty("results")]
        public List<PatientResult> Results { get; set; }
    }
    #endregion

    #region 检验报告列表
    public class ReportParam
    {
        public string Hospid { get; set; }
        public string Cpatientcode { get; set; }
        public string Barcode { get; set; }
        public string Startdate { get; set; }
        public string Enddate { get; set; }
        public string Name { get; set; }
        public string Idcard { get; set; }
        public string Phone { get; set; }
    }

    public class ReportResult
    {
        [JsonProperty("ApplyNo")]
        public string ApplyNo { get; set; }
        [JsonProperty("ReportTypeName")]
        public string ReportTypeName { get; set; }
        [JsonProperty("TestSample")]
        public string TestSample { get; set; }
        [JsonProperty("OrderNo")]
        public string OrderNo { get; set; }
        [JsonProperty("cjgsj")]
        public string Cjgsj { get; set; }
        [JsonProperty("MainID")]
        public string MainID { get; set; }
    }

    public class ReportOutput
    {
        [JsonProperty("code")]
        public int Code { get; set; }
        [JsonProperty("msg")]
        public string Msg { get; set; }
        [JsonProperty("reportlist")]
        public List<ReportResult> Reportlist { get; set; }
    }
    #endregion

    #region 检验报告明细
    public class ReportDetailParam
    {
        public string Mainid { get; set; }
    }

    public class ReportDetail
    {
        [JsonProperty("TestItemChi")]
        public string TestItemChi { get; set; }
        [JsonProperty("TestResult")]
        public string TestResult { get; set; }
        [JsonProperty("Unit")]
        public string Unit { get; set; }
        [JsonProperty("ConsultChar")]
        public string ConsultChar { get; set; }
        [JsonProperty("TestResultSign")]
        public string TestResultSign { get; set; }
        [JsonProperty("testmeans")]
        public string Testmeans { get; set; }
        [JsonProperty("mutualvalue")]
        public string Mutualvalue { get; set; }
    }

    public class ReportDetailOutput
    {
        [JsonProperty("code")]
        public int Code { get; set; }
        [JsonProperty("msg")]
        public string Msg { get; set; }
        [JsonProperty("mainid")]
        public string Mainid { get; set; }
        [JsonProperty("StandardSampleNo")]
        public string StandardSampleNo { get; set; }
        [JsonProperty("SectionOffice")]
        public string SectionOffice { get; set; }
        [JsonProperty("jbsj")]
        public string Jbsj { get; set; }
        [JsonProperty("Name")]
        public string Name { get; set; }
        [JsonProperty("TestSample")]
        public string TestSample { get; set; }
        [JsonProperty("BedNo")]
        public string BedNo { get; set; }
        [JsonProperty("jysj")]
        public string Jysj { get; set; }
        [JsonProperty("sex")]
        public string Sex { get; set; }
        [JsonProperty("SampleStatus")]
        public string SampleStatus { get; set; }
        [JsonProperty("kdr")]
        public string Kdr { get; set; }
        [JsonProperty("cjgsj")]
        public string Cjgsj { get; set; }
        [JsonProperty("Age")]
        public string Age { get; set; }
        [JsonProperty("jyr")]
        public string Jyr { get; set; }
        [JsonProperty("Diagnose")]
        public string Diagnose { get; set; }
        [JsonProperty("cbsj")]
        public string Cbsj { get; set; }
        [JsonProperty("hdr")]
        public string Hdr { get; set; }
        [JsonProperty("cbr")]
        public string Cbr { get; set; }
        [JsonProperty("ReportFormat")]
        public string ReportFormat { get; set; }
        [JsonProperty("ReportTypeName")]
        public string ReportTypeName { get; set; }
        [JsonProperty("MzZyNo")]
        public string MzZyNo { get; set; }
        [JsonProperty("kdsj")]
        public string Kdsj { get; set; }
        [JsonProperty("detail")]
        public List<ReportDetail> Details { get; set; }
    }

    #endregion

    #endregion
}
