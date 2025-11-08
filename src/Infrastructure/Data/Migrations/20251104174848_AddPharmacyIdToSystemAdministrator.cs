using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HelloDoctorApi.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddPharmacyIdToSystemAdministrator : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "PharmacyId",
                table: "SystemAdministrators",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SystemAdministrators_PharmacyId",
                table: "SystemAdministrators",
                column: "PharmacyId");

            migrationBuilder.AddForeignKey(
                name: "FK_SystemAdministrators_Pharmacies_PharmacyId",
                table: "SystemAdministrators",
                column: "PharmacyId",
                principalTable: "Pharmacies",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SystemAdministrators_Pharmacies_PharmacyId",
                table: "SystemAdministrators");

            migrationBuilder.DropIndex(
                name: "IX_SystemAdministrators_PharmacyId",
                table: "SystemAdministrators");

            migrationBuilder.DropColumn(
                name: "PharmacyId",
                table: "SystemAdministrators");
        }
    }
}
