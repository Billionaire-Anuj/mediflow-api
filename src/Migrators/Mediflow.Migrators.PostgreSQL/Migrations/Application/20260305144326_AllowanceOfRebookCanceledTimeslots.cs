using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mediflow.Migrators.PostgreSQL.Migrations.Application
{
    /// <inheritdoc />
    public partial class AllowanceOfRebookCanceledTimeslots : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Appointments_TimeslotId",
                table: "Appointments");

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_TimeslotId",
                table: "Appointments",
                column: "TimeslotId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Appointments_TimeslotId",
                table: "Appointments");

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_TimeslotId",
                table: "Appointments",
                column: "TimeslotId",
                unique: true);
        }
    }
}
