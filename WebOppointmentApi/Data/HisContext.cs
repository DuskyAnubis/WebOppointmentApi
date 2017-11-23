﻿using Microsoft.EntityFrameworkCore;
using WebOppointmentApi.Models;


namespace WebOppointmentApi.Data
{
    public class HisContext : DbContext
    {

        public HisContext(DbContextOptions<HisContext> options) : base(options)
        {

        }

        public DbSet<门诊挂号> GH { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<门诊挂号>(entity =>
            {
                entity.HasKey(e => e.门诊号);

                entity.Property(e => e.Password)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.交班).HasDefaultValueSql("((0))");

                entity.Property(e => e.交班日期).HasColumnType("datetime");

                entity.Property(e => e.出生日期).HasColumnType("datetime");

                entity.Property(e => e.初诊).HasDefaultValueSql("((0))");

                entity.Property(e => e.医师)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.卡号)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.复诊).HasDefaultValueSql("((0))");

                entity.Property(e => e.姓名).HasColumnType("char(10)");

                entity.Property(e => e.婚姻)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.就诊标志).HasDefaultValueSql("((0))");

                entity.Property(e => e.工本费).HasColumnType("money");

                entity.Property(e => e.年龄单位)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.急诊).HasDefaultValueSql("((0))");

                entity.Property(e => e.性别).HasColumnType("char(10)");

                entity.Property(e => e.总费用)
                    .HasColumnType("money")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.总预存款)
                    .HasColumnType("money")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.挂号类别).HasMaxLength(20);

                entity.Property(e => e.挂号费).HasColumnType("money");

                entity.Property(e => e.接诊医师id)
                    .HasColumnName("接诊医师ID")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.操作员)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.日期)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.日结帐日期).HasColumnType("datetime");

                entity.Property(e => e.来源)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.民族)
                    .HasColumnType("char(20)")
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.状态).HasDefaultValueSql("((0))");

                entity.Property(e => e.现金支付)
                    .HasColumnType("money")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.电话)
                    .HasColumnType("char(15)")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.科室).HasMaxLength(50);

                entity.Property(e => e.籍贯).HasColumnType("char(30)");

                entity.Property(e => e.职业).HasColumnType("char(10)");

                entity.Property(e => e.身份证)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.过敏史)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.退款)
                    .HasColumnType("money")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.退票号)
                    .HasColumnType("numeric(18, 0)")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.通信地址).HasColumnType("char(30)");

                entity.Property(e => e.金额).HasColumnType("money");

                entity.Property(e => e.预存款余额)
                    .HasColumnType("money")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.预存款支付)
                    .HasColumnType("money")
                    .HasDefaultValueSql("((0))");
            });
        }
    }
}
