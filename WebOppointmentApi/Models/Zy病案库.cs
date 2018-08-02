﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebOppointmentApi.Models
{
    public class Zy病案库
    {
        public int 病人编号 { get; set; }
        public int 住院号 { get; set; }
        public string 姓名 { get; set; }
        public DateTime 入院日期 { get; set; }
        public bool? 家床标志 { get; set; }
        public string 医保类型 { get; set; }
        public string 性别 { get; set; }
        public string 身份证号 { get; set; }
        public string 年龄 { get; set; }
        public DateTime? 出生日期 { get; set; }
        public string 出生地 { get; set; }
        public string 民族 { get; set; }
        public string 婚否 { get; set; }
        public string 职业 { get; set; }
        public string 单位 { get; set; }
        public string 家庭住址 { get; set; }
        public string 电话 { get; set; }
        public string 邮政编码 { get; set; }
        public string 国籍 { get; set; }
        public short 保密等级 { get; set; }
        public string 联系人地址 { get; set; }
        public string 联系人 { get; set; }
        public string 关系 { get; set; }
        public string 联系电话 { get; set; }
        public string 入院诊断 { get; set; }
        public string 病情 { get; set; }
        public string 确诊诊断 { get; set; }
        public string 手术情况 { get; set; }
        public string 治疗情况 { get; set; }
        public DateTime? 起始日期 { get; set; }
        public decimal 预交金 { get; set; }
        public decimal 预交金余额 { get; set; }
        public decimal 总费用 { get; set; }
        public short 出院标志 { get; set; }
        public DateTime? 出院日期 { get; set; }
        public bool 长期医嘱 { get; set; }
        public string 医师代码 { get; set; }
        public bool? 常规费用 { get; set; }
        public int 住院天数 { get; set; }
        public string 科室 { get; set; }
        public decimal 出院交退款 { get; set; }
        public int 区间天数 { get; set; }
        public int? 分组 { get; set; }
        public decimal 差额 { get; set; }
        public DateTime? 确诊日期 { get; set; }
        public string 确诊icd { get; set; }
        public string 确诊医师 { get; set; }
        public string 卡号 { get; set; }
        public decimal 个人帐户支付 { get; set; }
        public decimal 统筹支付 { get; set; }
        public decimal 现金支付 { get; set; }
        public decimal 支票支付 { get; set; }
        public decimal 公务员床补 { get; set; }
        public decimal 补助金 { get; set; }
        public decimal 欠款 { get; set; }
        public decimal 记帐 { get; set; }
        public int? 封锁信息 { get; set; }
        public string PInfo { get; set; }
        public string 核算医师 { get; set; }
        public string 床位 { get; set; }
        public string 就诊号 { get; set; }
        public string 病室 { get; set; }
        public decimal 优惠额 { get; set; }
        public string 社会保障号 { get; set; }
        public decimal 个人全自费支付 { get; set; }
        public decimal 大额医疗基金支付 { get; set; }
        public decimal 公补基金支付 { get; set; }
        public decimal 企业补充基金支付 { get; set; }
        public decimal 离休基金支付 { get; set; }
        public decimal 工伤基金支付 { get; set; }
        public decimal 生育基金支付 { get; set; }
        public decimal 医院代付金额 { get; set; }
        public int 病历状态 { get; set; }
        public string 发票号 { get; set; }
        public DateTime? 结算日期 { get; set; }
        public string 担保人 { get; set; }
        public string 审核人 { get; set; }
        public string 经办人 { get; set; }
        public string 一卡通 { get; set; }
        public bool 欠费记账 { get; set; }
        public string 出生地省 { get; set; }
        public string 出生地市 { get; set; }
        public string 出生地县区 { get; set; }
        public string 籍贯 { get; set; }
        public string 籍贯市 { get; set; }
        public string 现住址省 { get; set; }
        public string 现住址市 { get; set; }
        public string 现住址县区 { get; set; }
        public string 入院途径 { get; set; }
        public string 户口住址省 { get; set; }
        public string 户口住址市 { get; set; }
        public string 户口住址县区 { get; set; }
        public string 户口住址邮编 { get; set; }
        public string 工作单位电话 { get; set; }
        public string 工作单位邮编 { get; set; }
        public string 门急诊诊断 { get; set; }
        public string 诊断1 { get; set; }
        public string 诊断2 { get; set; }
        public string 诊断3 { get; set; }
        public string 诊断4 { get; set; }
        public string 诊断5 { get; set; }
        public string 主诉 { get; set; }
        public string 支付类型 { get; set; }
        public string 入院方式 { get; set; }
        public int 住院次数 { get; set; }
        public int DwId { get; set; }
    }
}
