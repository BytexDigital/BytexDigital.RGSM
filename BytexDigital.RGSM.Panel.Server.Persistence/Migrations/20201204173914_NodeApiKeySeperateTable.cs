using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BytexDigital.RGSM.Panel.Server.Persistence.Migrations
{
    public partial class NodeApiKeySeperateTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApiKey",
                table: "Nodes");

            migrationBuilder.CreateTable(
                name: "NodeKeys",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    NodeId = table.Column<string>(type: "TEXT", nullable: false),
                    ApiKey = table.Column<string>(type: "TEXT", nullable: false),
                    TimeCreated = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    Version = table.Column<byte[]>(type: "BLOB", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NodeKeys", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NodeKeys_Nodes_NodeId",
                        column: x => x.NodeId,
                        principalTable: "Nodes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_NodeKeys_NodeId",
                table: "NodeKeys",
                column: "NodeId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NodeKeys");

            migrationBuilder.AddColumn<string>(
                name: "ApiKey",
                table: "Nodes",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }
    }
}
