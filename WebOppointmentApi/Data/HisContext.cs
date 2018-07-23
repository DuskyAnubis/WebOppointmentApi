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
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .ValueGeneratedNever();

                entity.Property(e => e.DwId).HasColumnName("Dw_ID");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.Ip)
                    .HasColumnName("IP")
                    .HasMaxLength(25)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.KsId).HasColumnName("KS_ID");

                entity.Property(e => e.划价号).HasColumnType("numeric(18, 0)");

                entity.Property(e => e.医保医师编码)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.医师姓名)
                    .HasMaxLength(8)
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

            modelBuilder.Entity<工作人员>().HasQueryFilter(m => m.Dw_Id == 1);
            modelBuilder.Entity<划价临时库>().HasQueryFilter(m => m.DwId == 1);
            modelBuilder.Entity<划价流水帐>().HasQueryFilter(m => m.DwId == 1);
            modelBuilder.Entity<门诊收费>().HasQueryFilter(m => m.DwId == 1);
            modelBuilder.Entity<门诊收费流水帐>().HasQueryFilter(m => m.DwId == 1);
            modelBuilder.Entity<医师代码>().HasQueryFilter(m => m.DwId == 1);
        }
    }
}
