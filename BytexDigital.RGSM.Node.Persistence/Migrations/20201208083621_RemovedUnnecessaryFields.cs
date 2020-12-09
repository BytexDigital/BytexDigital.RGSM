using Microsoft.EntityFrameworkCore.Migrations;

namespace BytexDigital.RGSM.Node.Persistence.Migrations
{
    public partial class RemovedUnnecessaryFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AllowFilePatching",
                table: "Arma3Server");

            migrationBuilder.DropColumn(
                name: "Hostname",
                table: "Arma3Server");

            migrationBuilder.DropColumn(
                name: "MaxPlayers",
                table: "Arma3Server");

            migrationBuilder.DropColumn(
                name: "MessageOfTheDay",
                table: "Arma3Server");

            migrationBuilder.DropColumn(
                name: "MessageOfTheDayInterval",
                table: "Arma3Server");

            migrationBuilder.DropColumn(
                name: "Password",
                table: "Arma3Server");

            migrationBuilder.DropColumn(
                name: "PasswordAdmin",
                table: "Arma3Server");

            migrationBuilder.DropColumn(
                name: "VerifySignatures",
                table: "Arma3Server");

            migrationBuilder.RenameColumn(
                name: "ServerCommandPassword",
                table: "Arma3Server",
                newName: "ProfilesPath");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ProfilesPath",
                table: "Arma3Server",
                newName: "ServerCommandPassword");

            migrationBuilder.AddColumn<int>(
                name: "AllowFilePatching",
                table: "Arma3Server",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Hostname",
                table: "Arma3Server",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "MaxPlayers",
                table: "Arma3Server",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "MessageOfTheDay",
                table: "Arma3Server",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MessageOfTheDayInterval",
                table: "Arma3Server",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Password",
                table: "Arma3Server",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PasswordAdmin",
                table: "Arma3Server",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "VerifySignatures",
                table: "Arma3Server",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }
    }
}
