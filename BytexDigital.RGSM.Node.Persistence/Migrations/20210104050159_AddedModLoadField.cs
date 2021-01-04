using Microsoft.EntityFrameworkCore.Migrations;

namespace BytexDigital.RGSM.Node.Persistence.Migrations
{
    public partial class AddedModLoadField : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsLoaded",
                table: "TrackedWorkshopMods",
                newName: "Load");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Load",
                table: "TrackedWorkshopMods",
                newName: "IsLoaded");
        }
    }
}
