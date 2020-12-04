using Microsoft.EntityFrameworkCore.Migrations;

namespace BytexDigital.RGSM.Panel.Server.Persistence.Migrations
{
    public partial class NodeApiKey : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ApiKey",
                table: "Nodes",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApiKey",
                table: "Nodes");
        }
    }
}
