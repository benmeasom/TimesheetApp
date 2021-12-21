using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DataObjects.Migrations
{
    public partial class TimeSheetTableUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ActivityTypeId",
                table: "TimesheetItems",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Timesheet",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TotalHours",
                table: "Timesheet",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TotalMinutes",
                table: "Timesheet",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "WeekEndingDate",
                table: "Timesheet",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateIndex(
                name: "IX_TimesheetItems_ActivityTypeId",
                table: "TimesheetItems",
                column: "ActivityTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_TimesheetItems_ActivityTypes_ActivityTypeId",
                table: "TimesheetItems",
                column: "ActivityTypeId",
                principalTable: "ActivityTypes",
                principalColumn: "ActivityTypeId",
                onDelete: ReferentialAction.NoAction);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TimesheetItems_ActivityTypes_ActivityTypeId",
                table: "TimesheetItems");

            migrationBuilder.DropIndex(
                name: "IX_TimesheetItems_ActivityTypeId",
                table: "TimesheetItems");

            migrationBuilder.DropColumn(
                name: "ActivityTypeId",
                table: "TimesheetItems");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Timesheet");

            migrationBuilder.DropColumn(
                name: "TotalHours",
                table: "Timesheet");

            migrationBuilder.DropColumn(
                name: "TotalMinutes",
                table: "Timesheet");

            migrationBuilder.DropColumn(
                name: "WeekEndingDate",
                table: "Timesheet");
        }
    }
}
