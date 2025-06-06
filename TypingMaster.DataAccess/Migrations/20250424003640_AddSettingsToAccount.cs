﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TypingMaster.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddSettingsToAccount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "settings_json",
                table: "accounts",
                type: "jsonb",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "settings_json",
                table: "accounts");
        }
    }
}
