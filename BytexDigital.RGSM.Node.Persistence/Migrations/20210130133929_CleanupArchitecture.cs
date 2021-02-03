using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BytexDigital.RGSM.Node.Persistence.Migrations
{
    public partial class CleanupArchitecture : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Arma3Server");

            migrationBuilder.DropTable(
                name: "TrackedDepots");

            migrationBuilder.DropTable(
                name: "TrackedWorkshopMods");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Arma3Server",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    AdditionalArguments = table.Column<string>(type: "TEXT", nullable: true),
                    AppId = table.Column<int>(type: "INTEGER", nullable: true),
                    BattlEyePath = table.Column<string>(type: "TEXT", nullable: true),
                    Branch = table.Column<string>(type: "TEXT", nullable: true),
                    ExecutableFileName = table.Column<string>(type: "TEXT", nullable: true),
                    InstalledVersion = table.Column<string>(type: "TEXT", nullable: true),
                    IsInstalled = table.Column<bool>(type: "INTEGER", nullable: false),
                    Port = table.Column<int>(type: "INTEGER", nullable: false),
                    ProfilesPath = table.Column<string>(type: "TEXT", nullable: true),
                    RconIp = table.Column<string>(type: "TEXT", nullable: false),
                    RconPassword = table.Column<string>(type: "TEXT", nullable: false),
                    RconPort = table.Column<int>(type: "INTEGER", nullable: false),
                    ServerId = table.Column<string>(type: "TEXT", nullable: false),
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

            migrationBuilder.CreateTable(
                name: "TrackedDepots",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    DepotId = table.Column<long>(type: "INTEGER", nullable: false),
                    ServerId = table.Column<string>(type: "TEXT", nullable: false),
                    TimeCreated = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    Version = table.Column<byte[]>(type: "BLOB", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrackedDepots", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TrackedDepots_Servers_ServerId",
                        column: x => x.ServerId,
                        principalTable: "Servers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TrackedWorkshopMods",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    Directory = table.Column<string>(type: "TEXT", nullable: true),
                    Load = table.Column<bool>(type: "INTEGER", nullable: false),
                    OfAppId = table.Column<uint>(type: "INTEGER", nullable: true),
                    PublishedFileId = table.Column<ulong>(type: "INTEGER", nullable: false),
                    ServerId = table.Column<string>(type: "TEXT", nullable: false),
                    TimeCreated = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    Version = table.Column<byte[]>(type: "BLOB", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrackedWorkshopMods", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TrackedWorkshopMods_Servers_ServerId",
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

            migrationBuilder.CreateIndex(
                name: "IX_TrackedDepots_ServerId",
                table: "TrackedDepots",
                column: "ServerId");

            migrationBuilder.CreateIndex(
                name: "IX_TrackedWorkshopMods_ServerId",
                table: "TrackedWorkshopMods",
                column: "ServerId");
        }
    }
}
