using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TypingMaster.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class ApplySnakeCaseNamingConvention : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_courses_accounts_AccountId",
                table: "courses");

            migrationBuilder.DropForeignKey(
                name: "FK_drill_stats_practices_PracticeLogId",
                table: "drill_stats");

            migrationBuilder.DropForeignKey(
                name: "FK_practices_accounts_AccountId",
                table: "practices");

            migrationBuilder.DropForeignKey(
                name: "FK_user_profiles_accounts_AccountId",
                table: "user_profiles");

            migrationBuilder.RenameColumn(
                name: "Title",
                table: "user_profiles",
                newName: "title");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "user_profiles",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "PhoneNumber",
                table: "user_profiles",
                newName: "phone_number");

            migrationBuilder.RenameColumn(
                name: "LastName",
                table: "user_profiles",
                newName: "last_name");

            migrationBuilder.RenameColumn(
                name: "FirstName",
                table: "user_profiles",
                newName: "first_name");

            migrationBuilder.RenameColumn(
                name: "AvatarUrl",
                table: "user_profiles",
                newName: "avatar_url");

            migrationBuilder.RenameColumn(
                name: "AccountId",
                table: "user_profiles",
                newName: "account_id");

            migrationBuilder.RenameIndex(
                name: "IX_user_profiles_AccountId",
                table: "user_profiles",
                newName: "IX_user_profiles_account_id");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "practices",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "PracticeDuration",
                table: "practices",
                newName: "practice_duration");

            migrationBuilder.RenameColumn(
                name: "KeyStatsJson",
                table: "practices",
                newName: "key_stats_json");

            migrationBuilder.RenameColumn(
                name: "CurrentLessonId",
                table: "practices",
                newName: "current_lesson_id");

            migrationBuilder.RenameColumn(
                name: "CurrentCourseId",
                table: "practices",
                newName: "current_course_id");

            migrationBuilder.RenameColumn(
                name: "AccountId",
                table: "practices",
                newName: "account_id");

            migrationBuilder.RenameIndex(
                name: "IX_practices_AccountId",
                table: "practices",
                newName: "IX_practices_account_id");

            migrationBuilder.RenameColumn(
                name: "Wpm",
                table: "drill_stats",
                newName: "wpm");

            migrationBuilder.RenameColumn(
                name: "Accuracy",
                table: "drill_stats",
                newName: "accuracy");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "drill_stats",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "TypedText",
                table: "drill_stats",
                newName: "typed_text");

            migrationBuilder.RenameColumn(
                name: "TrainingType",
                table: "drill_stats",
                newName: "training_type");

            migrationBuilder.RenameColumn(
                name: "StartTime",
                table: "drill_stats",
                newName: "start_time");

            migrationBuilder.RenameColumn(
                name: "PracticeText",
                table: "drill_stats",
                newName: "practice_text");

            migrationBuilder.RenameColumn(
                name: "PracticeLogId",
                table: "drill_stats",
                newName: "practice_log_id");

            migrationBuilder.RenameColumn(
                name: "LessonId",
                table: "drill_stats",
                newName: "lesson_id");

            migrationBuilder.RenameColumn(
                name: "KeyEventsJson",
                table: "drill_stats",
                newName: "key_events_json");

            migrationBuilder.RenameColumn(
                name: "FinishTime",
                table: "drill_stats",
                newName: "finish_time");

            migrationBuilder.RenameColumn(
                name: "CourseId",
                table: "drill_stats",
                newName: "course_id");

            migrationBuilder.RenameIndex(
                name: "IX_drill_stats_PracticeLogId",
                table: "drill_stats",
                newName: "IX_drill_stats_practice_log_id");

            migrationBuilder.RenameColumn(
                name: "Type",
                table: "courses",
                newName: "type");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "courses",
                newName: "name");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "courses",
                newName: "description");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "courses",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "SettingsJson",
                table: "courses",
                newName: "settings_json");

            migrationBuilder.RenameColumn(
                name: "LessonDataUrl",
                table: "courses",
                newName: "lesson_data_url");

            migrationBuilder.RenameColumn(
                name: "AccountId",
                table: "courses",
                newName: "account_id");

            migrationBuilder.RenameIndex(
                name: "IX_courses_AccountId",
                table: "courses",
                newName: "IX_courses_account_id");

            migrationBuilder.RenameColumn(
                name: "Version",
                table: "accounts",
                newName: "version");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "accounts",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "IsDeleted",
                table: "accounts",
                newName: "is_deleted");

            migrationBuilder.RenameColumn(
                name: "GoalStats",
                table: "accounts",
                newName: "goal_stats");

            migrationBuilder.RenameColumn(
                name: "DeletedAt",
                table: "accounts",
                newName: "deleted_at");

            migrationBuilder.RenameColumn(
                name: "AccountName",
                table: "accounts",
                newName: "account_name");

            migrationBuilder.RenameColumn(
                name: "AccountEmail",
                table: "accounts",
                newName: "account_email");

            migrationBuilder.RenameIndex(
                name: "IX_accounts_AccountEmail",
                table: "accounts",
                newName: "IX_accounts_account_email");

            migrationBuilder.AddForeignKey(
                name: "FK_courses_accounts_account_id",
                table: "courses",
                column: "account_id",
                principalTable: "accounts",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_drill_stats_practices_practice_log_id",
                table: "drill_stats",
                column: "practice_log_id",
                principalTable: "practices",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_practices_accounts_account_id",
                table: "practices",
                column: "account_id",
                principalTable: "accounts",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_user_profiles_accounts_account_id",
                table: "user_profiles",
                column: "account_id",
                principalTable: "accounts",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_courses_accounts_account_id",
                table: "courses");

            migrationBuilder.DropForeignKey(
                name: "FK_drill_stats_practices_practice_log_id",
                table: "drill_stats");

            migrationBuilder.DropForeignKey(
                name: "FK_practices_accounts_account_id",
                table: "practices");

            migrationBuilder.DropForeignKey(
                name: "FK_user_profiles_accounts_account_id",
                table: "user_profiles");

            migrationBuilder.RenameColumn(
                name: "title",
                table: "user_profiles",
                newName: "Title");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "user_profiles",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "phone_number",
                table: "user_profiles",
                newName: "PhoneNumber");

            migrationBuilder.RenameColumn(
                name: "last_name",
                table: "user_profiles",
                newName: "LastName");

            migrationBuilder.RenameColumn(
                name: "first_name",
                table: "user_profiles",
                newName: "FirstName");

            migrationBuilder.RenameColumn(
                name: "avatar_url",
                table: "user_profiles",
                newName: "AvatarUrl");

            migrationBuilder.RenameColumn(
                name: "account_id",
                table: "user_profiles",
                newName: "AccountId");

            migrationBuilder.RenameIndex(
                name: "IX_user_profiles_account_id",
                table: "user_profiles",
                newName: "IX_user_profiles_AccountId");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "practices",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "practice_duration",
                table: "practices",
                newName: "PracticeDuration");

            migrationBuilder.RenameColumn(
                name: "key_stats_json",
                table: "practices",
                newName: "KeyStatsJson");

            migrationBuilder.RenameColumn(
                name: "current_lesson_id",
                table: "practices",
                newName: "CurrentLessonId");

            migrationBuilder.RenameColumn(
                name: "current_course_id",
                table: "practices",
                newName: "CurrentCourseId");

            migrationBuilder.RenameColumn(
                name: "account_id",
                table: "practices",
                newName: "AccountId");

            migrationBuilder.RenameIndex(
                name: "IX_practices_account_id",
                table: "practices",
                newName: "IX_practices_AccountId");

            migrationBuilder.RenameColumn(
                name: "wpm",
                table: "drill_stats",
                newName: "Wpm");

            migrationBuilder.RenameColumn(
                name: "accuracy",
                table: "drill_stats",
                newName: "Accuracy");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "drill_stats",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "typed_text",
                table: "drill_stats",
                newName: "TypedText");

            migrationBuilder.RenameColumn(
                name: "training_type",
                table: "drill_stats",
                newName: "TrainingType");

            migrationBuilder.RenameColumn(
                name: "start_time",
                table: "drill_stats",
                newName: "StartTime");

            migrationBuilder.RenameColumn(
                name: "practice_text",
                table: "drill_stats",
                newName: "PracticeText");

            migrationBuilder.RenameColumn(
                name: "practice_log_id",
                table: "drill_stats",
                newName: "PracticeLogId");

            migrationBuilder.RenameColumn(
                name: "lesson_id",
                table: "drill_stats",
                newName: "LessonId");

            migrationBuilder.RenameColumn(
                name: "key_events_json",
                table: "drill_stats",
                newName: "KeyEventsJson");

            migrationBuilder.RenameColumn(
                name: "finish_time",
                table: "drill_stats",
                newName: "FinishTime");

            migrationBuilder.RenameColumn(
                name: "course_id",
                table: "drill_stats",
                newName: "CourseId");

            migrationBuilder.RenameIndex(
                name: "IX_drill_stats_practice_log_id",
                table: "drill_stats",
                newName: "IX_drill_stats_PracticeLogId");

            migrationBuilder.RenameColumn(
                name: "type",
                table: "courses",
                newName: "Type");

            migrationBuilder.RenameColumn(
                name: "name",
                table: "courses",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "description",
                table: "courses",
                newName: "Description");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "courses",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "settings_json",
                table: "courses",
                newName: "SettingsJson");

            migrationBuilder.RenameColumn(
                name: "lesson_data_url",
                table: "courses",
                newName: "LessonDataUrl");

            migrationBuilder.RenameColumn(
                name: "account_id",
                table: "courses",
                newName: "AccountId");

            migrationBuilder.RenameIndex(
                name: "IX_courses_account_id",
                table: "courses",
                newName: "IX_courses_AccountId");

            migrationBuilder.RenameColumn(
                name: "version",
                table: "accounts",
                newName: "Version");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "accounts",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "is_deleted",
                table: "accounts",
                newName: "IsDeleted");

            migrationBuilder.RenameColumn(
                name: "goal_stats",
                table: "accounts",
                newName: "GoalStats");

            migrationBuilder.RenameColumn(
                name: "deleted_at",
                table: "accounts",
                newName: "DeletedAt");

            migrationBuilder.RenameColumn(
                name: "account_name",
                table: "accounts",
                newName: "AccountName");

            migrationBuilder.RenameColumn(
                name: "account_email",
                table: "accounts",
                newName: "AccountEmail");

            migrationBuilder.RenameIndex(
                name: "IX_accounts_account_email",
                table: "accounts",
                newName: "IX_accounts_AccountEmail");

            migrationBuilder.AddForeignKey(
                name: "FK_courses_accounts_AccountId",
                table: "courses",
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
    }
}
