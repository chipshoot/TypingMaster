using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TypingMaster.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class UpdateLoginCredentialsForIdp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "confirmation_token",
                table: "login_credentials");

            migrationBuilder.DropColumn(
                name: "confirmation_token_expiry",
                table: "login_credentials");

            migrationBuilder.DropColumn(
                name: "failed_login_attempts",
                table: "login_credentials");

            migrationBuilder.DropColumn(
                name: "is_email_confirmed",
                table: "login_credentials");

            migrationBuilder.DropColumn(
                name: "lockout_end",
                table: "login_credentials");

            migrationBuilder.DropColumn(
                name: "password_hash",
                table: "login_credentials");

            migrationBuilder.DropColumn(
                name: "password_salt",
                table: "login_credentials");

            migrationBuilder.DropColumn(
                name: "refresh_token",
                table: "login_credentials");

            migrationBuilder.DropColumn(
                name: "refresh_token_expiry",
                table: "login_credentials");

            migrationBuilder.DropColumn(
                name: "reset_password_token",
                table: "login_credentials");

            migrationBuilder.DropColumn(
                name: "reset_password_token_expiry",
                table: "login_credentials");

            migrationBuilder.RenameColumn(
                name: "is_locked",
                table: "login_credentials",
                newName: "is_active");

            migrationBuilder.AlterColumn<DateTime>(
                name: "last_updated",
                table: "login_credentials",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "login_credentials",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AddColumn<DateTime>(
                name: "last_login_at",
                table: "login_credentials",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "last_login_at",
                table: "login_credentials");

            migrationBuilder.RenameColumn(
                name: "is_active",
                table: "login_credentials",
                newName: "is_locked");

            migrationBuilder.AlterColumn<DateTime>(
                name: "last_updated",
                table: "login_credentials",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "login_credentials",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AddColumn<string>(
                name: "confirmation_token",
                table: "login_credentials",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "confirmation_token_expiry",
                table: "login_credentials",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "failed_login_attempts",
                table: "login_credentials",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "is_email_confirmed",
                table: "login_credentials",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "lockout_end",
                table: "login_credentials",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "password_hash",
                table: "login_credentials",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "password_salt",
                table: "login_credentials",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "refresh_token",
                table: "login_credentials",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "refresh_token_expiry",
                table: "login_credentials",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "reset_password_token",
                table: "login_credentials",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "reset_password_token_expiry",
                table: "login_credentials",
                type: "timestamp with time zone",
                nullable: true);
        }
    }
}
