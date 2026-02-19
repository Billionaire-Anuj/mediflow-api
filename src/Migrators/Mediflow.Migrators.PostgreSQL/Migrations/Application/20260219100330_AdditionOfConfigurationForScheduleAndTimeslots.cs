using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mediflow.Migrators.PostgreSQL.Migrations.Application
{
    /// <inheritdoc />
    public partial class AdditionOfConfigurationForScheduleAndTimeslots : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ScheduleId1",
                table: "Timeslot",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Timeslot_ScheduleId1",
                table: "Timeslot",
                column: "ScheduleId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Timeslot_Schedules_ScheduleId1",
                table: "Timeslot",
                column: "ScheduleId1",
                principalTable: "Schedules",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Timeslot_Schedules_ScheduleId1",
                table: "Timeslot");

            migrationBuilder.DropIndex(
                name: "IX_Timeslot_ScheduleId1",
                table: "Timeslot");

            migrationBuilder.DropColumn(
                name: "ScheduleId1",
                table: "Timeslot");
        }
    }
}
