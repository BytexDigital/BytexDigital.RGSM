using Microsoft.EntityFrameworkCore.Migrations;

namespace BytexDigital.RGSM.Panel.Server.Persistence.Migrations
{
    public partial class RemoveSteamCredentialsRef : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SteamCredentialId",
                table: "SteamLoginSupportedApps");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SteamCredentialId",
                table: "SteamLoginSupportedApps",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }
    }
}
