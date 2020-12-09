using Microsoft.EntityFrameworkCore.Migrations;

namespace BytexDigital.RGSM.Node.Persistence.Migrations
{
    public partial class Rcon : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AdditionalArguments",
                table: "Arma3Server",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BattlEyePath",
                table: "Arma3Server",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RconIp",
                table: "Arma3Server",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "RconPassword",
                table: "Arma3Server",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "RconPort",
                table: "Arma3Server",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdditionalArguments",
                table: "Arma3Server");

            migrationBuilder.DropColumn(
                name: "BattlEyePath",
                table: "Arma3Server");

            migrationBuilder.DropColumn(
                name: "RconIp",
                table: "Arma3Server");

            migrationBuilder.DropColumn(
                name: "RconPassword",
                table: "Arma3Server");

            migrationBuilder.DropColumn(
                name: "RconPort",
                table: "Arma3Server");
        }
    }
}
