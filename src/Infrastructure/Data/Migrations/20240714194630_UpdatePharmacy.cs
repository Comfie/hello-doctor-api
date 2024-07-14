using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApiBaseTemplate.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdatePharmacy : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Pharmacies",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Pharmacies");
        }
    }
}
