using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace HelloDoctorApi.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddAudit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "AssignedPharmacyId",
                table: "Prescriptions",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "DefaultPharmacyId",
                table: "MainMembers",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AuditLogs",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Action = table.Column<string>(type: "text", nullable: false),
                    Details = table.Column<string>(type: "text", nullable: true),
                    ActorUserId = table.Column<string>(type: "text", nullable: true),
                    PrescriptionId = table.Column<long>(type: "bigint", nullable: true),
                    PharmacyId = table.Column<long>(type: "bigint", nullable: true),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    LastModified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditLogs", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Prescriptions_AssignedPharmacyId",
                table: "Prescriptions",
                column: "AssignedPharmacyId");

            migrationBuilder.AddForeignKey(
                name: "FK_Prescriptions_Pharmacies_AssignedPharmacyId",
                table: "Prescriptions",
                column: "AssignedPharmacyId",
                principalTable: "Pharmacies",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Prescriptions_Pharmacies_AssignedPharmacyId",
                table: "Prescriptions");

            migrationBuilder.DropTable(
                name: "AuditLogs");

            migrationBuilder.DropIndex(
                name: "IX_Prescriptions_AssignedPharmacyId",
                table: "Prescriptions");

            migrationBuilder.DropColumn(
                name: "AssignedPharmacyId",
                table: "Prescriptions");

            migrationBuilder.DropColumn(
                name: "DefaultPharmacyId",
                table: "MainMembers");
        }
    }
}
