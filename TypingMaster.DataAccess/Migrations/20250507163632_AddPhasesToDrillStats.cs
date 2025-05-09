using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TypingMaster.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddPhasesToDrillStats : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "phases",
                table: "drill_stats",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "phases",
                table: "drill_stats");
        }
    }
}
