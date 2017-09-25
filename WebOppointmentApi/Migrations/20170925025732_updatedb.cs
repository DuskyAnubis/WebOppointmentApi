using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace WebOppointmentApi.Migrations
{
    public partial class updatedb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Source");

            migrationBuilder.DropColumn(
                name: "OrganazitionId",
                table: "Schedulings");

            migrationBuilder.DropColumn(
                name: "RegisteredRankCode",
                table: "Schedulings");

            migrationBuilder.DropColumn(
                name: "RegisteredRankName",
                table: "Schedulings");

            migrationBuilder.DropColumn(
                name: "UserRankCode",
                table: "Schedulings");

            migrationBuilder.DropColumn(
                name: "UserRankName",
                table: "Schedulings");

            migrationBuilder.DropColumn(
                name: "OrganazitionId",
                table: "Registereds");

            migrationBuilder.DropColumn(
                name: "PlusPrice",
                table: "Registereds");

            migrationBuilder.DropColumn(
                name: "Price",
                table: "Registereds");

            migrationBuilder.DropColumn(
                name: "SourceId",
                table: "Registereds");

            migrationBuilder.DropColumn(
                name: "TreatPrice",
                table: "Registereds");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Registereds");

            migrationBuilder.AlterColumn<DateTime>(
                name: "SurgeryDate",
                table: "Schedulings",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<DateTime>(
                name: "SchedulingDate",
                table: "Schedulings",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<DateTime>(
                name: "RecoveryTreatDate",
                table: "Schedulings",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<DateTime>(
                name: "EndTreatDate",
                table: "Schedulings",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "SurgeryDate",
                table: "Schedulings",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "SchedulingDate",
                table: "Schedulings",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "RecoveryTreatDate",
                table: "Schedulings",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "EndTreatDate",
                table: "Schedulings",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OrganazitionId",
                table: "Schedulings",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "RegisteredRankCode",
                table: "Schedulings",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RegisteredRankName",
                table: "Schedulings",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserRankCode",
                table: "Schedulings",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserRankName",
                table: "Schedulings",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OrganazitionId",
                table: "Registereds",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PlusPrice",
                table: "Registereds",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Price",
                table: "Registereds",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SourceId",
                table: "Registereds",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TreatPrice",
                table: "Registereds",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Registereds",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Source",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Num = table.Column<int>(nullable: false),
                    OppointmentStateCode = table.Column<string>(nullable: true),
                    OppointmentStateName = table.Column<string>(nullable: true),
                    Period = table.Column<string>(nullable: true),
                    SchedulingId = table.Column<int>(nullable: false),
                    Time = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Source", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Source_Schedulings_SchedulingId",
                        column: x => x.SchedulingId,
                        principalTable: "Schedulings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Source_SchedulingId",
                table: "Source",
                column: "SchedulingId");
        }
    }
}
