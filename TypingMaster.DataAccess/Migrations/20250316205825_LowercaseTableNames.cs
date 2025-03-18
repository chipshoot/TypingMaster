using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TypingMaster.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class LowercaseTableNames : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Courses_Accounts_AccountId",
                table: "Courses");

            migrationBuilder.DropForeignKey(
                name: "FK_DrillStats_PracticeLogs_PracticeLogId",
                table: "DrillStats");

            migrationBuilder.DropForeignKey(
                name: "FK_PracticeLogs_Accounts_AccountId",
                table: "PracticeLogs");

            migrationBuilder.DropForeignKey(
                name: "FK_UserProfiles_Accounts_AccountId",
                table: "UserProfiles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Accounts",
                table: "Accounts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserProfiles",
                table: "UserProfiles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PracticeLogs",
                table: "PracticeLogs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DrillStats",
                table: "DrillStats");

            migrationBuilder.RenameTable(
                name: "Accounts",
                newName: "accounts");

            migrationBuilder.RenameTable(
                name: "UserProfiles",
                newName: "user_profiles");

            migrationBuilder.RenameTable(
                name: "PracticeLogs",
                newName: "practices");

            migrationBuilder.RenameTable(
                name: "DrillStats",
                newName: "drill_stats");

            migrationBuilder.RenameIndex(
                name: "IX_Accounts_AccountEmail",
                table: "accounts",
                newName: "IX_accounts_AccountEmail");

            migrationBuilder.RenameIndex(
                name: "IX_UserProfiles_AccountId",
                table: "user_profiles",
                newName: "IX_user_profiles_AccountId");

            migrationBuilder.RenameIndex(
                name: "IX_PracticeLogs_AccountId",
                table: "practices",
                newName: "IX_practices_AccountId");

            migrationBuilder.RenameIndex(
                name: "IX_DrillStats_PracticeLogId",
                table: "drill_stats",
                newName: "IX_drill_stats_PracticeLogId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_accounts",
                table: "accounts",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_user_profiles",
                table: "user_profiles",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_practices",
                table: "practices",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_drill_stats",
                table: "drill_stats",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Courses_accounts_AccountId",
                table: "Courses",
                column: "AccountId",
                principalTable: "accounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_drill_stats_practices_PracticeLogId",
                table: "drill_stats",
                column: "PracticeLogId",
                principalTable: "practices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_practices_accounts_AccountId",
                table: "practices",
                column: "AccountId",
                principalTable: "accounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_user_profiles_accounts_AccountId",
                table: "user_profiles",
                column: "AccountId",
                principalTable: "accounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Courses_accounts_AccountId",
                table: "Courses");

            migrationBuilder.DropForeignKey(
                name: "FK_drill_stats_practices_PracticeLogId",
                table: "drill_stats");

            migrationBuilder.DropForeignKey(
                name: "FK_practices_accounts_AccountId",
                table: "practices");

            migrationBuilder.DropForeignKey(
                name: "FK_user_profiles_accounts_AccountId",
                table: "user_profiles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_accounts",
                table: "accounts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_user_profiles",
                table: "user_profiles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_practices",
                table: "practices");

            migrationBuilder.DropPrimaryKey(
                name: "PK_drill_stats",
                table: "drill_stats");

            migrationBuilder.RenameTable(
                name: "accounts",
                newName: "Accounts");

            migrationBuilder.RenameTable(
                name: "user_profiles",
                newName: "UserProfiles");

            migrationBuilder.RenameTable(
                name: "practices",
                newName: "PracticeLogs");

            migrationBuilder.RenameTable(
                name: "drill_stats",
                newName: "DrillStats");

            migrationBuilder.RenameIndex(
                name: "IX_accounts_AccountEmail",
                table: "Accounts",
                newName: "IX_Accounts_AccountEmail");

            migrationBuilder.RenameIndex(
                name: "IX_user_profiles_AccountId",
                table: "UserProfiles",
                newName: "IX_UserProfiles_AccountId");

            migrationBuilder.RenameIndex(
                name: "IX_practices_AccountId",
                table: "PracticeLogs",
                newName: "IX_PracticeLogs_AccountId");

            migrationBuilder.RenameIndex(
                name: "IX_drill_stats_PracticeLogId",
                table: "DrillStats",
                newName: "IX_DrillStats_PracticeLogId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Accounts",
                table: "Accounts",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserProfiles",
                table: "UserProfiles",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PracticeLogs",
                table: "PracticeLogs",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DrillStats",
                table: "DrillStats",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Courses_Accounts_AccountId",
                table: "Courses",
                column: "AccountId",
                principalTable: "Accounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DrillStats_PracticeLogs_PracticeLogId",
                table: "DrillStats",
                column: "PracticeLogId",
                principalTable: "PracticeLogs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PracticeLogs_Accounts_AccountId",
                table: "PracticeLogs",
                column: "AccountId",
                principalTable: "Accounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserProfiles_Accounts_AccountId",
                table: "UserProfiles",
                column: "AccountId",
                principalTable: "Accounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
