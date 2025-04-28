using Microsoft.EntityFrameworkCore.Migrations;

namespace TypingMaster.DataAccess.Migrations;

public partial class AddSettingsToAccounts : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "settings_json",
            table: "accounts",
            type: "jsonb",
            nullable: false,
            defaultValue: "{}");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "settings_json",
            table: "accounts");
    }
}