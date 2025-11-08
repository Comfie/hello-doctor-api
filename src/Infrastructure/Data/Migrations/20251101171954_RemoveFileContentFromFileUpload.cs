using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HelloDoctorApi.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemoveFileContentFromFileUpload : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FileContent",
                table: "FileUploads");

            migrationBuilder.AddColumn<long>(
                name: "FileSize",
                table: "FileUploads",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FileSize",
                table: "FileUploads");

            migrationBuilder.AddColumn<byte[]>(
                name: "FileContent",
                table: "FileUploads",
                type: "bytea",
                nullable: false,
                defaultValue: new byte[0]);
        }
    }
}
