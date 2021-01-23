using Microsoft.EntityFrameworkCore.Migrations;

namespace NetScape.Modules.DAL.Migrations
{
    public partial class RemoveAppearanceFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Players_AppearanceId",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "PlayerId",
                table: "Appearances");

            migrationBuilder.CreateIndex(
                name: "IX_Players_AppearanceId",
                table: "Players",
                column: "AppearanceId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Players_AppearanceId",
                table: "Players");

            migrationBuilder.AddColumn<int>(
                name: "PlayerId",
                table: "Appearances",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Players_AppearanceId",
                table: "Players",
                column: "AppearanceId",
                unique: true);
        }
    }
}
