using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PNMaterialsInfrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueMaterialGroupNameIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_MaterialGroups_Name",
                table: "MaterialGroups",
                column: "Name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_MaterialGroups_Name",
                table: "MaterialGroups");
        }
    }
}
