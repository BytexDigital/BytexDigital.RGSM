using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BytexDigital.RGSM.Panel.Server.Persistence.Migrations
{
    public partial class SteamLogins : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SteamCredentialSupportedApps");

            migrationBuilder.DropTable(
                name: "SteamCredentials");

            migrationBuilder.CreateTable(
                name: "SteamLogins",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    Username = table.Column<string>(type: "TEXT", nullable: false),
                    Password = table.Column<string>(type: "TEXT", nullable: false),
                    LoginKey = table.Column<string>(type: "TEXT", nullable: true),
                    Sentry = table.Column<string>(type: "TEXT", nullable: true),
                    WebApiKey = table.Column<string>(type: "TEXT", nullable: true),
                    TimeCreated = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    Version = table.Column<byte[]>(type: "BLOB", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SteamLogins", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SteamLoginSupportedApps",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    SteamCredentialId = table.Column<string>(type: "TEXT", nullable: false),
                    AppId = table.Column<long>(type: "INTEGER", nullable: false),
                    SupportsWorkshop = table.Column<bool>(type: "INTEGER", nullable: false),
                    SteamLoginId = table.Column<string>(type: "TEXT", nullable: true),
                    TimeCreated = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    Version = table.Column<byte[]>(type: "BLOB", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SteamLoginSupportedApps", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SteamLoginSupportedApps_SteamLogins_SteamLoginId",
                        column: x => x.SteamLoginId,
                        principalTable: "SteamLogins",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SteamLoginSupportedApps_SteamLoginId",
                table: "SteamLoginSupportedApps",
                column: "SteamLoginId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SteamLoginSupportedApps");

            migrationBuilder.DropTable(
                name: "SteamLogins");

            migrationBuilder.CreateTable(
                name: "SteamCredentials",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    LoginKey = table.Column<string>(type: "TEXT", nullable: true),
                    Password = table.Column<string>(type: "TEXT", nullable: false),
                    Sentry = table.Column<string>(type: "TEXT", nullable: true),
                    TimeCreated = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    Username = table.Column<string>(type: "TEXT", nullable: false),
                    Version = table.Column<byte[]>(type: "BLOB", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SteamCredentials", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SteamCredentialSupportedApps",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    AppId = table.Column<long>(type: "INTEGER", nullable: false),
                    SteamCredentialId = table.Column<string>(type: "TEXT", nullable: false),
                    SupportsWorkshop = table.Column<bool>(type: "INTEGER", nullable: false),
                    TimeCreated = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    Version = table.Column<byte[]>(type: "BLOB", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SteamCredentialSupportedApps", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SteamCredentialSupportedApps_SteamCredentials_SteamCredentialId",
                        column: x => x.SteamCredentialId,
                        principalTable: "SteamCredentials",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SteamCredentialSupportedApps_SteamCredentialId",
                table: "SteamCredentialSupportedApps",
                column: "SteamCredentialId");
        }
    }
}
