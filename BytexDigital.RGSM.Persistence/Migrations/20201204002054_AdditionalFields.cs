using Microsoft.EntityFrameworkCore.Migrations;

namespace BytexDigital.RGSM.Persistence.Migrations
{
    public partial class AdditionalFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Servers_Nodes_NodeId",
                table: "Servers");

            migrationBuilder.AlterColumn<string>(
                name: "NodeId",
                table: "Servers",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AllowFilePatching",
                table: "Arma3Servers",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BetaBranch",
                table: "Arma3Servers",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Hostname",
                table: "Arma3Servers",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "MaxPlayers",
                table: "Arma3Servers",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "MessageOfTheDay",
                table: "Arma3Servers",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MessageOfTheDayInterval",
                table: "Arma3Servers",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Password",
                table: "Arma3Servers",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PasswordAdmin",
                table: "Arma3Servers",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ServerCommandPassword",
                table: "Arma3Servers",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ServerId",
                table: "Arma3Servers",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "VerifySignatures",
                table: "Arma3Servers",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_ServerId",
                table: "Tasks",
                column: "ServerId");

            migrationBuilder.CreateIndex(
                name: "IX_Arma3Servers_ServerId",
                table: "Arma3Servers",
                column: "ServerId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Arma3Servers_Servers_ServerId",
                table: "Arma3Servers",
                column: "ServerId",
                principalTable: "Servers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Servers_Nodes_NodeId",
                table: "Servers",
                column: "NodeId",
                principalTable: "Nodes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_Servers_ServerId",
                table: "Tasks",
                column: "ServerId",
                principalTable: "Servers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Arma3Servers_Servers_ServerId",
                table: "Arma3Servers");

            migrationBuilder.DropForeignKey(
                name: "FK_Servers_Nodes_NodeId",
                table: "Servers");

            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_Servers_ServerId",
                table: "Tasks");

            migrationBuilder.DropIndex(
                name: "IX_Tasks_ServerId",
                table: "Tasks");

            migrationBuilder.DropIndex(
                name: "IX_Arma3Servers_ServerId",
                table: "Arma3Servers");

            migrationBuilder.DropColumn(
                name: "AllowFilePatching",
                table: "Arma3Servers");

            migrationBuilder.DropColumn(
                name: "BetaBranch",
                table: "Arma3Servers");

            migrationBuilder.DropColumn(
                name: "Hostname",
                table: "Arma3Servers");

            migrationBuilder.DropColumn(
                name: "MaxPlayers",
                table: "Arma3Servers");

            migrationBuilder.DropColumn(
                name: "MessageOfTheDay",
                table: "Arma3Servers");

            migrationBuilder.DropColumn(
                name: "MessageOfTheDayInterval",
                table: "Arma3Servers");

            migrationBuilder.DropColumn(
                name: "Password",
                table: "Arma3Servers");

            migrationBuilder.DropColumn(
                name: "PasswordAdmin",
                table: "Arma3Servers");

            migrationBuilder.DropColumn(
                name: "ServerCommandPassword",
                table: "Arma3Servers");

            migrationBuilder.DropColumn(
                name: "ServerId",
                table: "Arma3Servers");

            migrationBuilder.DropColumn(
                name: "VerifySignatures",
                table: "Arma3Servers");

            migrationBuilder.AlterColumn<string>(
                name: "NodeId",
                table: "Servers",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AddForeignKey(
                name: "FK_Servers_Nodes_NodeId",
                table: "Servers",
                column: "NodeId",
                principalTable: "Nodes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
