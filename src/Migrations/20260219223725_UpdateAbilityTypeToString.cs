using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace api.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAbilityTypeToString : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Type",
                table: "Ability");

            migrationBuilder.RenameColumn(
                name: "Type",
                table: "Pokemon",
                newName: "AbilityType");

            migrationBuilder.AddColumn<string>(
                name: "AbilityType",
                table: "Ability",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AbilityType",
                table: "Ability");

            migrationBuilder.RenameColumn(
                name: "AbilityType",
                table: "Pokemon",
                newName: "Type");

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "Ability",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
