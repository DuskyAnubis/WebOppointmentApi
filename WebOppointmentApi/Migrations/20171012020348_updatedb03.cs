using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace WebOppointmentApi.Migrations
{
    public partial class updatedb03 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "IsSms",
                table: "Schedulings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "SmsDate",
                table: "Schedulings",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsSms",
                table: "Schedulings");

            migrationBuilder.DropColumn(
                name: "SmsDate",
                table: "Schedulings");
        }
    }
}
