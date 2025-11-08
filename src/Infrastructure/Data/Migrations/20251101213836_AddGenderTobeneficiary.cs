using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HelloDoctorApi.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddGenderTobeneficiary : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DateOfBirth",
                table: "Beneficiaries",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Gender",
                table: "Beneficiaries",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DateOfBirth",
                table: "Beneficiaries");

            migrationBuilder.DropColumn(
                name: "Gender",
                table: "Beneficiaries");
        }
    }
}
