using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PNMaterialsInfrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddIsArchived : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsArchived",
                table: "Materials",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsArchived",
                table: "MaterialGroups",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsArchived",
                table: "Materials");

            migrationBuilder.DropColumn(
                name: "IsArchived",
                table: "MaterialGroups");
        }
    }
}
