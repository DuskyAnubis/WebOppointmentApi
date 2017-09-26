using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace WebOppointmentApi.Migrations
{
    public partial class updatedb01 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StarTime",
                table: "Schedulings");

            migrationBuilder.AddColumn<string>(
                name: "StartTime",
                table: "Schedulings",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StartTime",
                table: "Schedulings");

            migrationBuilder.AddColumn<string>(
                name: "StarTime",
                table: "Schedulings",
                nullable: true);
        }
    }
}
