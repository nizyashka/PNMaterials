using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PNMaterialsInfrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddMaterialCodeSequence : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateSequence(
                name: "MaterialCodeSequence",
                startValue: 10000000L);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropSequence(
                name: "MaterialCodeSequence");
        }
    }
}
