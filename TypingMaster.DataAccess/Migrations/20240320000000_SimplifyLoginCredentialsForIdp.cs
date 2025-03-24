using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace TypingMaster.DataAccess.Migrations;

public partial class SimplifyLoginCredentialsForIdp : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        // Drop columns that are no longer needed
        migrationBuilder.DropColumn(
            name: "password_hash",
            table: "login_credentials");

        migrationBuilder.DropColumn(
            name: "password_salt",
            table: "login_credentials");

        migrationBuilder.DropColumn(
            name: "is_email_confirmed",
            table: "login_credentials");

        migrationBuilder.DropColumn(
            name: "confirmation_token",
            table: "login_credentials");

        migrationBuilder.DropColumn(
            name: "confirmation_token_expiry",
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

        migrationBuilder.DropColumn(
            name: "is_locked",
            table: "login_credentials");

        migrationBuilder.DropColumn(
            name: "failed_login_attempts",
            table: "login_credentials");

        migrationBuilder.DropColumn(
            name: "lockout_end",
            table: "login_credentials");

        // Add new columns
        migrationBuilder.AddColumn<DateTime>(
            name: "last_login_at",
            table: "login_credentials",
            type: "timestamp with time zone",
            nullable: false,
            defaultValue: DateTime.UtcNow);

        migrationBuilder.AddColumn<bool>(
            name: "is_active",
            table: "login_credentials",
            type: "boolean",
            nullable: false,
            defaultValue: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        // Restore dropped columns
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

        migrationBuilder.AddColumn<bool>(
            name: "is_email_confirmed",
            table: "login_credentials",
            type: "boolean",
            nullable: false,
            defaultValue: false);

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

        migrationBuilder.AddColumn<bool>(
            name: "is_locked",
            table: "login_credentials",
            type: "boolean",
            nullable: false,
            defaultValue: false);

        migrationBuilder.AddColumn<int>(
            name: "failed_login_attempts",
            table: "login_credentials",
            type: "integer",
            nullable: false,
            defaultValue: 0);

        migrationBuilder.AddColumn<DateTime>(
            name: "lockout_end",
            table: "login_credentials",
            type: "timestamp with time zone",
            nullable: true);

        // Remove new columns
        migrationBuilder.DropColumn(
            name: "last_login_at",
            table: "login_credentials");

        migrationBuilder.DropColumn(
            name: "is_active",
            table: "login_credentials");
    }
}