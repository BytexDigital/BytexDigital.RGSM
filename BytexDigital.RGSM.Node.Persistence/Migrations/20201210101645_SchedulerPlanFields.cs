using Microsoft.EntityFrameworkCore.Migrations;

namespace BytexDigital.RGSM.Node.Persistence.Migrations
{
    public partial class SchedulerPlanFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ScheduleActions_ScheduleGroups_ScheduleGroupsId",
                table: "ScheduleActions");

            migrationBuilder.DropForeignKey(
                name: "FK_ScheduleGroups_SchedulerPlans_SchedulerPlanId",
                table: "ScheduleGroups");

            migrationBuilder.DropIndex(
                name: "IX_ScheduleActions_ScheduleGroupsId",
                table: "ScheduleActions");

            migrationBuilder.DropColumn(
                name: "SchedulerId",
                table: "ScheduleGroups");

            migrationBuilder.DropColumn(
                name: "ScheduleGroupsId",
                table: "ScheduleActions");

            migrationBuilder.RenameColumn(
                name: "ScheduleEventId",
                table: "ScheduleActions",
                newName: "ScheduleGroupId");

            migrationBuilder.AlterColumn<string>(
                name: "SchedulerPlanId",
                table: "ScheduleGroups",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ScheduleActions_ScheduleGroupId",
                table: "ScheduleActions",
                column: "ScheduleGroupId");

            migrationBuilder.AddForeignKey(
                name: "FK_ScheduleActions_ScheduleGroups_ScheduleGroupId",
                table: "ScheduleActions",
                column: "ScheduleGroupId",
                principalTable: "ScheduleGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ScheduleGroups_SchedulerPlans_SchedulerPlanId",
                table: "ScheduleGroups",
                column: "SchedulerPlanId",
                principalTable: "SchedulerPlans",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ScheduleActions_ScheduleGroups_ScheduleGroupId",
                table: "ScheduleActions");

            migrationBuilder.DropForeignKey(
                name: "FK_ScheduleGroups_SchedulerPlans_SchedulerPlanId",
                table: "ScheduleGroups");

            migrationBuilder.DropIndex(
                name: "IX_ScheduleActions_ScheduleGroupId",
                table: "ScheduleActions");

            migrationBuilder.RenameColumn(
                name: "ScheduleGroupId",
                table: "ScheduleActions",
                newName: "ScheduleEventId");

            migrationBuilder.AlterColumn<string>(
                name: "SchedulerPlanId",
                table: "ScheduleGroups",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AddColumn<string>(
                name: "SchedulerId",
                table: "ScheduleGroups",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ScheduleGroupsId",
                table: "ScheduleActions",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ScheduleActions_ScheduleGroupsId",
                table: "ScheduleActions",
                column: "ScheduleGroupsId");

            migrationBuilder.AddForeignKey(
                name: "FK_ScheduleActions_ScheduleGroups_ScheduleGroupsId",
                table: "ScheduleActions",
                column: "ScheduleGroupsId",
                principalTable: "ScheduleGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ScheduleGroups_SchedulerPlans_SchedulerPlanId",
                table: "ScheduleGroups",
                column: "SchedulerPlanId",
                principalTable: "SchedulerPlans",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
