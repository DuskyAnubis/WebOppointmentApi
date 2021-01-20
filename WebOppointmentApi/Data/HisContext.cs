using Microsoft.EntityFrameworkCore;
using WebOppointmentApi.Models;

namespace WebOppointmentApi.Data
{
    public class HisContext : DbContext
    {
        public HisContext(DbContextOptions<HisContext> options) : base(options)
        {

        }

        public DbSet<工作人员> 工作人员 { get; set; }
        public DbSet<划价临时库> 划价临时库 { get; set; }
        public DbSet<划价流水帐> 划价流水帐 { get; set; }
        public DbSet<门诊收费> 门诊收费 { get; set; }
        public DbSet<门诊收费流水帐> 门诊收费流水帐 { get; set; }
        public DbSet<医师代码> 医师代码 { get; set; }
        public DbSet<Zy病案库> Zy病案库 { get; set; }
        public DbSet<Zy病区床位> Zy病区床位 { get; set; }
        public DbSet<Zy记帐临时库> Zy记帐临时库 { get; set; }
        public DbSet<Zy记帐流水帐> Zy记帐流水帐 { get; set; }
        public DbSet<Zy预交金> Zy预交金 { get; set; }
        public DbSet<InpatientPrepayment> InpatientPrepayments { get; set; }
        public DbSet<门诊就诊信息> 门诊就诊信息 { get; set; }
        public DbSet<Yf> Yf { get; set; }
        public DbQuery<V_LisReport> V_LisReport { get; set; }
        public DbQuery<V_LisReportDetail> V_LisReportDetail { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<工作人员>(entity =>
            {
                entity.HasKey(e => e.代码);

                entity.Property(e => e.代码)
                    .HasMaxLength(50)
                    .ValueGeneratedNever();

                entity.Property(e => e.Dw_Id)
                    .HasColumnName("Dw_Id")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.Ip)
                    .HasColumnName("IP")
                    .HasMaxLength(25)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.KsId).HasColumnName("KS_ID");

                entity.Property(e => e.入库审核)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.入库录入)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.入网许可)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.出库审核)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.出库录入)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.姓名)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.密码)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.性别)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.报损审核)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.报损录入)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.日期)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.机器名)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.流水号).HasDefaultValueSql("((1))");

                entity.Property(e => e.科室)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.调价审核)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.调价录入)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.采购计划权限)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");
            });

            modelBuilder.Entity<划价临时库>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CzyId).HasColumnName("Czy_Id");

                entity.Property(e => e.DwId)
                    .HasColumnName("Dw_Id")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.YfId)
                    .HasColumnName("YF_ID")
                    .HasColumnType("numeric(18, 0)");

                entity.Property(e => e.YsId).HasColumnName("ys_Id");

                entity.Property(e => e.一付量).HasColumnType("money");

                entity.Property(e => e.代码)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.使用频率)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.农合卡号)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.分组标识)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.划价号)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.医保比例).HasColumnType("money");

                entity.Property(e => e.医保码)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.医保金额).HasColumnType("money");

                entity.Property(e => e.医师)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.单价).HasColumnType("money");

                entity.Property(e => e.单位)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.卡号)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.发药日期).HasColumnType("datetime");

                entity.Property(e => e.合疗分类)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.名称)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.地址)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.处方类别)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.套餐名称)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.年龄)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.库存量).HasColumnType("money");

                entity.Property(e => e.性别)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.批号)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.接口码1)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.接口码2)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.操作员)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.收费科室)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.政府采购价).HasColumnType("money");

                entity.Property(e => e.数量).HasColumnType("money");

                entity.Property(e => e.日期)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.日结帐日期).HasColumnType("smalldatetime");

                entity.Property(e => e.月结帐日期).HasColumnType("smalldatetime");

                entity.Property(e => e.材质分类)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.物理分类)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.用法)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.用量)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.疾病诊断)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.病人姓名)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.禁忌)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.科室id).HasColumnName("科室ID");

                entity.Property(e => e.药品付数).HasDefaultValueSql("((1))");

                entity.Property(e => e.规格)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.货号)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.金额).HasColumnType("money");

                entity.Property(e => e.CateId)
                    .HasColumnName("CateId");

                entity.Property(e => e.CateName)
                   .HasColumnName("CateName");
            });

            modelBuilder.Entity<划价流水帐>(entity =>
            {
                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .ValueGeneratedNever();

                entity.Property(e => e.CzyId).HasColumnName("Czy_Id");

                entity.Property(e => e.DwId)
                    .HasColumnName("Dw_Id")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.YfId)
                    .HasColumnName("YF_ID")
                    .HasColumnType("numeric(18, 0)")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.YsId).HasColumnName("ys_Id");

                entity.Property(e => e.一付量).HasColumnType("money");

                entity.Property(e => e.代码)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.使用频率)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.农合卡号)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.分组标识)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.划价号)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.医保比例).HasColumnType("money");

                entity.Property(e => e.医保码)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.医保金额).HasColumnType("money");

                entity.Property(e => e.医师)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.单价)
                    .HasColumnType("money")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.单位)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.卡号)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.发票流水号).HasDefaultValueSql("((0))");

                entity.Property(e => e.发药日期).HasColumnType("datetime");

                entity.Property(e => e.发药标志).HasDefaultValueSql("((0))");

                entity.Property(e => e.合疗分类)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.名称)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.地址)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.处方类别)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.套餐名称)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.年龄)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.库存量)
                    .HasColumnType("money")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.性别)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.批号)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.接口码1)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.接口码2)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.操作员)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.收费科室)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.政府采购价).HasColumnType("money");

                entity.Property(e => e.数量)
                    .HasColumnType("money")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.日期).HasColumnType("datetime");

                entity.Property(e => e.日结帐日期).HasColumnType("smalldatetime");

                entity.Property(e => e.月结帐日期).HasColumnType("smalldatetime");

                entity.Property(e => e.材质分类)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.物理分类)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.特殊药品).HasDefaultValueSql("((0))");

                entity.Property(e => e.用法)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.用量)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.疾病诊断)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.病人姓名)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.禁忌)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.科室id)
                    .HasColumnName("科室ID")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.药品付数).HasDefaultValueSql("((0))");

                entity.Property(e => e.规格)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.货号)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.金额)
                    .HasColumnType("money")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.CateId)
                    .HasColumnName("CateId");

                entity.Property(e => e.CateName)
                   .HasColumnName("CateName");
            });

            modelBuilder.Entity<门诊收费>(entity =>
            {
                entity.HasKey(e => e.收费id);

                entity.Property(e => e.收费id).HasColumnName("收费ID");

                entity.Property(e => e.CzyId).HasColumnName("Czy_ID");

                entity.Property(e => e.DwId).HasColumnName("Dw_ID");

                entity.Property(e => e.PInfo)
                    .HasColumnName("pInfo")
                    .HasMaxLength(512)
                    .IsUnicode(false);

                entity.Property(e => e.优惠额).HasColumnType("money");

                entity.Property(e => e.公补基金支付).HasColumnType("money");

                entity.Property(e => e.医保)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.医疗保险号)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.单据流水号)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.卡号)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.发票号)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.基金支付额).HasColumnType("money");

                entity.Property(e => e.帐户余额).HasColumnType("money");

                entity.Property(e => e.性别)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.总金额).HasColumnType("money");

                entity.Property(e => e.折扣率).HasColumnType("money");

                entity.Property(e => e.操作员)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.日期)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.现金支付).HasColumnType("money");

                entity.Property(e => e.病人姓名)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.结帐日期).HasColumnType("datetime");

                entity.Property(e => e.统筹支付).HasColumnType("money");

                entity.Property(e => e.补助金).HasColumnType("money");

                entity.Property(e => e.账户支付).HasColumnType("money");

                entity.Property(e => e.费别)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.门诊号)
                    .HasMaxLength(20)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<门诊收费流水帐>(entity =>
            {
                entity.HasKey(e => e.收费id);

                entity.Property(e => e.收费id)
                    .HasColumnName("收费ID")
                    .ValueGeneratedNever();

                entity.Property(e => e.CzyId).HasColumnName("Czy_Id");

                entity.Property(e => e.DwId).HasColumnName("Dw_Id");

                entity.Property(e => e.PInfo)
                    .HasColumnName("pInfo")
                    .HasMaxLength(512)
                    .IsUnicode(false);

                entity.Property(e => e.优惠额).HasColumnType("money");

                entity.Property(e => e.公补基金支付).HasColumnType("money");

                entity.Property(e => e.医保)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.医疗保险号)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.单据流水号)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.卡号)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.发票号)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.基金支付额).HasColumnType("money");

                entity.Property(e => e.帐户余额).HasColumnType("money");

                entity.Property(e => e.性别)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.总金额).HasColumnType("money");

                entity.Property(e => e.折扣率).HasColumnType("money");

                entity.Property(e => e.操作员)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.日期).HasColumnType("datetime");

                entity.Property(e => e.现金支付).HasColumnType("money");

                entity.Property(e => e.病人姓名)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.结帐日期).HasColumnType("datetime");

                entity.Property(e => e.统筹支付).HasColumnType("money");

                entity.Property(e => e.补助金).HasColumnType("money");

                entity.Property(e => e.账户支付).HasColumnType("money");

                entity.Property(e => e.费别)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.门诊号)
                    .HasMaxLength(20)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<医师代码>(entity =>
            {
                entity.HasKey(e => e.医师代码1);

                entity.Property(e => e.医师代码1)
                    .HasColumnName("医师代码")
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .ValueGeneratedNever();

                entity.Property(e => e.DwId).HasColumnName("Dw_ID");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.Id)
                .UseSqlServerIdentityColumn();

                entity.Property(e => e.Id)
                    .Metadata.AfterSaveBehavior = Microsoft.EntityFrameworkCore.Metadata.PropertySaveBehavior.Ignore;

                entity.Property(e => e.Ip)
                    .HasColumnName("IP")
                    .HasMaxLength(25)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.KsId).HasColumnName("KS_ID");

                entity.Property(e => e.医保医师编码)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.医师姓名)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.密码)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.所在科室)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.挂号科室)
                    .IsRequired()
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.日期)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.机器名)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.权限).HasDefaultValueSql("((2))");

                entity.Property(e => e.流水号).HasDefaultValueSql("((1))");

                entity.Property(e => e.职称).HasDefaultValueSql("((1))");
            });

            modelBuilder.Entity<Zy病案库>(entity =>
            {
                entity.HasKey(e => e.病人编号);

                entity.ToTable("ZY_病案库");

                entity.Property(e => e.DwId).HasColumnName("Dw_Id");

                entity.Property(e => e.PInfo)
                    .HasColumnName("pInfo")
                    .HasMaxLength(512)
                    .IsUnicode(false);

                entity.Property(e => e.一卡通)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.个人全自费支付).HasColumnType("money");

                entity.Property(e => e.个人帐户支付).HasColumnType("money");

                entity.Property(e => e.主诉)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.企业补充基金支付).HasColumnType("money");

                entity.Property(e => e.优惠额).HasColumnType("money");

                entity.Property(e => e.住院天数).HasDefaultValueSql("((1))");

                entity.Property(e => e.住院次数).HasDefaultValueSql("((1))");

                entity.Property(e => e.入院方式)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.入院日期)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.入院诊断)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.入院途径)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.公务员床补).HasColumnType("money");

                entity.Property(e => e.公补基金支付).HasColumnType("money");

                entity.Property(e => e.关系)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.出生地)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.出生地县区)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.出生地市)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.出生地省)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.出生日期).HasColumnType("datetime");

                entity.Property(e => e.出院交退款).HasColumnType("money");

                entity.Property(e => e.出院日期).HasColumnType("datetime");

                entity.Property(e => e.医保类型)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.医师代码)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.医院代付金额).HasColumnType("money");

                entity.Property(e => e.单位)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.卡号)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.发票号)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.国籍)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.大额医疗基金支付).HasColumnType("money");

                entity.Property(e => e.姓名).HasMaxLength(50);

                entity.Property(e => e.婚否)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.审核人)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.家床标志)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.家庭住址)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.就诊号)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.工伤基金支付).HasColumnType("money");

                entity.Property(e => e.工作单位电话)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.工作单位邮编)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.差额).HasColumnType("money");

                entity.Property(e => e.常规费用)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.年龄)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.床位)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.性别)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.总费用).HasColumnType("money");

                entity.Property(e => e.户口住址县区)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.户口住址市)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.户口住址省)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.户口住址邮编)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.手术情况)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.担保人)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.支付类型)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.支票支付).HasColumnType("money");

                entity.Property(e => e.核算医师)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.欠款).HasColumnType("money");

                entity.Property(e => e.民族)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.治疗情况)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.现住址县区)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.现住址市)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.现住址省)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.现金支付).HasColumnType("money");

                entity.Property(e => e.生育基金支付).HasColumnType("money");

                entity.Property(e => e.电话)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.病室)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.病情)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.确诊icd)
                    .HasColumnName("确诊ICD")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.确诊医师)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.确诊日期).HasColumnType("datetime");

                entity.Property(e => e.确诊诊断)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.社会保障号)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.离休基金支付).HasColumnType("money");

                entity.Property(e => e.科室)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.籍贯)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.籍贯市)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.经办人)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.结算日期).HasColumnType("datetime");

                entity.Property(e => e.统筹支付).HasColumnType("money");

                entity.Property(e => e.职业)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.联系人)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.联系人地址)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.联系电话)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.补助金).HasColumnType("money");

                entity.Property(e => e.记帐).HasColumnType("money");

                entity.Property(e => e.诊断1)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.诊断2)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.诊断3)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.诊断4)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.诊断5)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.起始日期).HasColumnType("datetime");

                entity.Property(e => e.身份证号)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.邮政编码)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.门急诊诊断)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.预交金).HasColumnType("money");

                entity.Property(e => e.预交金余额).HasColumnType("money");
            });

            modelBuilder.Entity<Zy病区床位>(entity =>
            {
                entity.HasKey(e => new { e.病区名, e.病室名, e.床位号, e.KsId });

                entity.ToTable("ZY_病区床位");

                entity.Property(e => e.病区名)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.病室名)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.KsId).HasColumnName("KS_ID");

                entity.Property(e => e.DwId)
                    .HasColumnName("Dw_Id")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.冷暖yfId).HasColumnName("冷暖YF_ID");

                entity.Property(e => e.冷暖代码)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.冷暖医保分类)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.冷暖医保码)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.冷暖名称)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.冷暖接口码1)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.冷暖接口码2)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.冷暖材质分类)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.冷暖费).HasColumnType("money");

                entity.Property(e => e.医保比例).HasColumnType("money");

                entity.Property(e => e.合疗分类)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.合疗比例).HasColumnType("money");

                entity.Property(e => e.床位yfId).HasColumnName("床位YF_ID");

                entity.Property(e => e.床位代码)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.床位医保分类)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.床位医保码)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.床位名称)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.床位接口码1)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.床位接口码2)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.床位材质分类)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.床位费).HasColumnType("money");

                entity.Property(e => e.护理yfId).HasColumnName("护理YF_ID");

                entity.Property(e => e.护理医保分类)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.护理医保码)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.护理名称)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.护理接口码1)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.护理接口码2)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.护理标准).HasColumnType("money");

                entity.Property(e => e.是否记冷暖)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.是否记床位)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.是否记护理)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.是否记诊疗)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.诊疗yfId).HasColumnName("诊疗YF_ID");

                entity.Property(e => e.诊疗代码)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.诊疗医保分类)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.诊疗医保码)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.诊疗名称)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.诊疗接口码1)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.诊疗接口码2)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.诊疗材质分类)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.诊疗费).HasColumnType("money");
            });

            modelBuilder.Entity<Zy记帐临时库>(entity =>
            {
                entity.ToTable("ZY_记帐临时库");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CzyId).HasColumnName("Czy_Id");

                entity.Property(e => e.FId).HasColumnName("F_ID");

                entity.Property(e => e.YfId)
                    .HasColumnName("YF_ID")
                    .HasColumnType("numeric(18, 0)");

                entity.Property(e => e.上传日期).HasColumnType("datetime");

                entity.Property(e => e.中药付数).HasDefaultValueSql("((1))");

                entity.Property(e => e.代码)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.医保分类)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.医保比例).HasColumnType("money");

                entity.Property(e => e.医保码)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.医嘱类别)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.医师编码)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.单价).HasColumnType("money");

                entity.Property(e => e.单位)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.发药日期).HasColumnType("smalldatetime");

                entity.Property(e => e.司药人)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.合疗分类)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.合疗比例).HasColumnType("money");

                entity.Property(e => e.名称)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.备注)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.套餐名称)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.实交金额).HasColumnType("money");

                entity.Property(e => e.实施标志).HasDefaultValueSql("((0))");

                entity.Property(e => e.库存量).HasColumnType("money");

                entity.Property(e => e.应交金额).HasColumnType("money");

                entity.Property(e => e.所在科室)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.执行频率)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.批号)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.接口码1)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.接口码2)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.操作员)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.收费科室)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.政府采购价).HasColumnType("money");

                entity.Property(e => e.数量).HasColumnType("money");

                entity.Property(e => e.日期)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.材质分类)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.物理分类)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.用法)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.用量)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.科室id).HasColumnName("科室ID");

                entity.Property(e => e.组别)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.规格)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.长天上传时间).HasColumnType("datetime");
            });

            modelBuilder.Entity<Zy记帐流水帐>(entity =>
            {
                entity.ToTable("ZY_记帐流水帐");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CzyId).HasColumnName("Czy_Id");

                entity.Property(e => e.DwId)
                    .HasColumnName("Dw_Id")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.FId).HasColumnName("F_ID");

                entity.Property(e => e.LskId).HasColumnName("LSK_ID");

                entity.Property(e => e.YfId)
                    .HasColumnName("YF_ID")
                    .HasColumnType("numeric(18, 0)")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.上传日期).HasColumnType("datetime");

                entity.Property(e => e.中药付数).HasDefaultValueSql("((1))");

                entity.Property(e => e.代码)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.医保分类)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.医保比例).HasColumnType("money");

                entity.Property(e => e.医保码)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.医嘱类别)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.医师编码)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.单价).HasColumnType("money");

                entity.Property(e => e.单位)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.发药日期).HasColumnType("smalldatetime");

                entity.Property(e => e.司药人)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.合疗分类)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.合疗比例).HasColumnType("money");

                entity.Property(e => e.名称)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.备注)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.套餐名称)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.实交金额).HasColumnType("money");

                entity.Property(e => e.实施标志).HasDefaultValueSql("((0))");

                entity.Property(e => e.库存量).HasColumnType("money");

                entity.Property(e => e.应交金额).HasColumnType("money");

                entity.Property(e => e.所在科室)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.执行频率)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.批号)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.接口码1)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.接口码2)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.操作员)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.收费科室)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.政府采购价).HasColumnType("money");

                entity.Property(e => e.数量).HasColumnType("money");

                entity.Property(e => e.日期)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.材质分类)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.物理分类)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.用法)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.用量)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.科室id).HasColumnName("科室ID");

                entity.Property(e => e.组别)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.规格)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.长天上传时间).HasColumnType("datetime");
            });

            modelBuilder.Entity<Zy预交金>(entity =>
            {
                entity.HasKey(e => e.预交金编号);

                entity.ToTable("ZY_预交金");

                entity.Property(e => e.CzyId).HasColumnName("Czy_Id");

                entity.Property(e => e.DwId).HasColumnName("Dw_Id");

                entity.Property(e => e.备注)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.操作员)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.操作员科室)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.日期)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.日结帐日期).HasColumnType("smalldatetime");

                entity.Property(e => e.月结帐日期).HasColumnType("smalldatetime");

                entity.Property(e => e.金额)
                    .HasColumnType("money")
                    .HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<门诊就诊信息>(entity =>
            {
                entity.HasKey(e => e.就诊序号);

                entity.Property(e => e.CzyId).HasColumnName("Czy_Id");

                entity.Property(e => e.DwId).HasColumnName("Dw_Id");

                entity.Property(e => e.出生日期).HasColumnType("datetime");

                entity.Property(e => e.医师)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.卡号)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.姓名)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.婚姻)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.年龄单位)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.性别)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.挂号类别)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.接诊医师id).HasColumnName("接诊医师ID");

                entity.Property(e => e.操作员)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.日期)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.民族)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.电话)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.社保卡)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.科室)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.籍贯)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.职业)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.身份证)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.过敏史)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.通信地址)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Yf>(entity =>
            {
                entity.ToTable("YF");

                entity.HasIndex(e => e.代码)
                    .HasName("dm");

                entity.HasIndex(e => e.拼音码)
                    .HasName("pym");

                entity.HasIndex(e => e.材质分类)
                    .HasName("czfl");

                entity.HasIndex(e => e.科室id)
                    .HasName("ksid");

                entity.HasIndex(e => new { e.名称, e.规格, e.单位, e.单价 })
                    .HasName("zh");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.DwId).HasColumnName("Dw_Id");

                entity.Property(e => e.FId)
                    .HasColumnName("F_ID")
                    .HasDefaultValueSql("(0)");

                entity.Property(e => e.YkId).HasColumnName("YK_ID");

                entity.Property(e => e.YyJb).HasColumnName("Yy_Jb");

                entity.Property(e => e.一次用量)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.五笔码)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.产地)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.代码)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.包装单位)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.医保分类)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.医保比例).HasDefaultValueSql("(0)");

                entity.Property(e => e.医保码)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.单价)
                    .HasColumnType("money")
                    .HasDefaultValueSql("(0)");

                entity.Property(e => e.单位)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('/')");

                entity.Property(e => e.合疗分类)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.合疗比例)
                    .HasColumnType("money")
                    .HasDefaultValueSql("(0)");

                entity.Property(e => e.名称)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('/')");

                entity.Property(e => e.失效期).HasColumnType("smalldatetime");

                entity.Property(e => e.存放位置)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.库存下限).HasDefaultValueSql("(0)");

                entity.Property(e => e.库存量).HasDefaultValueSql("(0)");

                entity.Property(e => e.执行频率)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.批发价)
                    .HasColumnType("money")
                    .HasDefaultValueSql("(0)");

                entity.Property(e => e.批号)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.拼音码)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.接口码1)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.接口码2)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.收费科室)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.政府采购价)
                    .HasColumnType("money")
                    .HasDefaultValueSql("(0)");

                entity.Property(e => e.散装数).HasDefaultValueSql("(1)");

                entity.Property(e => e.材质分类)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('/')");

                entity.Property(e => e.物理分类)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.特殊药品).HasDefaultValueSql("(0)");

                entity.Property(e => e.用法)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.科室id)
                    .HasColumnName("科室ID")
                    .HasDefaultValueSql("(0)");

                entity.Property(e => e.类别)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.统计内容)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.统计内容1)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.药品极限).HasDefaultValueSql("(0)");

                entity.Property(e => e.药品目录)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.药房库存).HasDefaultValueSql("(0)");

                entity.Property(e => e.药理作用)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.规格)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('/')");

                entity.Property(e => e.计价).HasDefaultValueSql("((1))");

                entity.Property(e => e.货号)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.进价)
                    .HasColumnType("money")
                    .HasDefaultValueSql("(0)");

                entity.Property(e => e.锁定)
                    .IsRequired()
                    .HasDefaultValueSql("(0)");
            });

            modelBuilder.Query<V_LisReport>(v =>
            {
                v.ToView("V_LisReport");
            });

            modelBuilder.Query<V_LisReportDetail>(v =>
            {
                v.ToView("V_LisReportDetail");
            });

            modelBuilder.Entity<工作人员>().HasQueryFilter(m => m.Dw_Id == 1);
            modelBuilder.Entity<划价临时库>().HasQueryFilter(m => m.DwId == 1);
            modelBuilder.Entity<划价流水帐>().HasQueryFilter(m => m.DwId == 1);
            modelBuilder.Entity<门诊收费>().HasQueryFilter(m => m.DwId == 1);
            modelBuilder.Entity<门诊收费流水帐>().HasQueryFilter(m => m.DwId == 1);
            modelBuilder.Entity<医师代码>().HasQueryFilter(m => m.DwId == 1);
            modelBuilder.Entity<Zy病案库>().HasQueryFilter(m => m.DwId == 1);
            modelBuilder.Entity<Zy病区床位>().HasQueryFilter(m => m.DwId == 1);
            modelBuilder.Entity<Zy记帐流水帐>().HasQueryFilter(m => m.DwId == 1);
            modelBuilder.Entity<Zy预交金>().HasQueryFilter(m => m.DwId == 1);
            modelBuilder.Entity<InpatientPrepayment>().HasQueryFilter(m => m.Dw_Id == 1);
            modelBuilder.Entity<门诊就诊信息>().HasQueryFilter(m => m.DwId == 1);
            modelBuilder.Entity<Yf>().HasQueryFilter(m => m.DwId == 1);
        }
    }
}
