using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PNMaterialsInfrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRequestNumberSequence : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateSequence(
                name: "RequestNumberSequence",
                startValue: 5000000000L);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropSequence(
                name: "RequestNumberSequence");
        }
    }
}
