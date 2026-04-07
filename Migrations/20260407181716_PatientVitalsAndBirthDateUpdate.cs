using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedSync_API.Migrations
{
    /// <inheritdoc />
    public partial class PatientVitalsAndBirthDateUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BloodPressure",
                table: "MedicalRecords",
                type: "varchar(20)",
                maxLength: 20,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "HeartRate",
                table: "MedicalRecords",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Temperature",
                table: "MedicalRecords",
                type: "decimal(65,30)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BloodPressure",
                table: "MedicalRecords");

            migrationBuilder.DropColumn(
                name: "HeartRate",
                table: "MedicalRecords");

            migrationBuilder.DropColumn(
                name: "Temperature",
                table: "MedicalRecords");
        }
    }
}
