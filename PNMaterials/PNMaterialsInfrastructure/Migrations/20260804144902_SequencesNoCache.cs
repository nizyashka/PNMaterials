using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PNMaterialsInfrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SequencesNoCache : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("ALTER SEQUENCE MaterialCodeSequence NO CACHE;");
            migrationBuilder.Sql("ALTER SEQUENCE RequestNumberSequence NO CACHE;");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("ALTER SEQUENCE MaterialCodeSequence CACHE;");
            migrationBuilder.Sql("ALTER SEQUENCE RequestNumberSequence CACHE;");
        }
    }
}
