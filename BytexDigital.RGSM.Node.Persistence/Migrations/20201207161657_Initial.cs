using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BytexDigital.RGSM.Node.Persistence.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "NodeSettings",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    Key = table.Column<string>(type: "TEXT", nullable: false),
                    Value = table.Column<string>(type: "TEXT", nullable: false),
                    TimeCreated = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    Version = table.Column<byte[]>(type: "BLOB", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NodeSettings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Servers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    DisplayName = table.Column<string>(type: "TEXT", nullable: false),
                    NodeId = table.Column<string>(type: "TEXT", nullable: false),
                    Type = table.Column<int>(type: "INTEGER", nullable: false),
                    Directory = table.Column<string>(type: "TEXT", nullable: true),
                    TimeCreated = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    Version = table.Column<byte[]>(type: "BLOB", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Servers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Arma3Server",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    ServerId = table.Column<string>(type: "TEXT", nullable: false),
                    IsInstalled = table.Column<bool>(type: "INTEGER", nullable: false),
                    InstalledVersion = table.Column<string>(type: "TEXT", nullable: true),
                    BetaBranch = table.Column<string>(type: "TEXT", nullable: true),
                    ExecutableFileName = table.Column<string>(type: "TEXT", nullable: true),
                    Hostname = table.Column<string>(type: "TEXT", nullable: false),
                    Port = table.Column<int>(type: "INTEGER", nullable: false),
                    Password = table.Column<string>(type: "TEXT", nullable: true),
                    PasswordAdmin = table.Column<string>(type: "TEXT", nullable: true),
                    ServerCommandPassword = table.Column<string>(type: "TEXT", nullable: true),
                    MessageOfTheDay = table.Column<string>(type: "TEXT", nullable: true),
                    MessageOfTheDayInterval = table.Column<int>(type: "INTEGER", nullable: true),
                    MaxPlayers = table.Column<int>(type: "INTEGER", nullable: false),
                    VerifySignatures = table.Column<int>(type: "INTEGER", nullable: false),
                    AllowFilePatching = table.Column<int>(type: "INTEGER", nullable: true),
                    TimeCreated = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    Version = table.Column<byte[]>(type: "BLOB", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Arma3Server", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Arma3Server_Servers_ServerId",
                        column: x => x.ServerId,
                        principalTable: "Servers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Arma3Server_ServerId",
                table: "Arma3Server",
                column: "ServerId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Arma3Server");

            migrationBuilder.DropTable(
                name: "NodeSettings");

            migrationBuilder.DropTable(
                name: "Servers");
        }
    }
}
