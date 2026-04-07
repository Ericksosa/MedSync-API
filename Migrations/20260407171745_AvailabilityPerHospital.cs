using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedSync_API.Migrations
{
    /// <inheritdoc />
    public partial class AvailabilityPerHospital : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "HospitalId",
                table: "DoctorAvailabilities",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_DoctorAvailabilities_HospitalId",
                table: "DoctorAvailabilities",
                column: "HospitalId");

            migrationBuilder.AddForeignKey(
                name: "FK_DoctorAvailabilities_Hospitals_HospitalId",
                table: "DoctorAvailabilities",
                column: "HospitalId",
                principalTable: "Hospitals",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DoctorAvailabilities_Hospitals_HospitalId",
                table: "DoctorAvailabilities");

            migrationBuilder.DropIndex(
                name: "IX_DoctorAvailabilities_HospitalId",
                table: "DoctorAvailabilities");

            migrationBuilder.DropColumn(
                name: "HospitalId",
                table: "DoctorAvailabilities");
        }
    }
}
