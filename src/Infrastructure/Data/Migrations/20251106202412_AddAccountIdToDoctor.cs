using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HelloDoctorApi.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddAccountIdToDoctor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Doctors_DoctorId",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_DoctorId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "DoctorId",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<string>(
                name: "AccountId",
                table: "Doctors",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Doctors_AccountId",
                table: "Doctors",
                column: "AccountId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Doctors_AspNetUsers_AccountId",
                table: "Doctors",
                column: "AccountId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Doctors_AspNetUsers_AccountId",
                table: "Doctors");

            migrationBuilder.DropIndex(
                name: "IX_Doctors_AccountId",
                table: "Doctors");

            migrationBuilder.DropColumn(
                name: "AccountId",
                table: "Doctors");

            migrationBuilder.AddColumn<long>(
                name: "DoctorId",
                table: "AspNetUsers",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_DoctorId",
                table: "AspNetUsers",
                column: "DoctorId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Doctors_DoctorId",
                table: "AspNetUsers",
                column: "DoctorId",
                principalTable: "Doctors",
                principalColumn: "Id");
        }
    }
}
