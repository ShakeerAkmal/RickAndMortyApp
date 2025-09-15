using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RickAndMortyApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class ChangeOnDelete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Characters_Locations_OriginId",
                table: "Characters");

            migrationBuilder.AddForeignKey(
                name: "FK_Characters_Locations_OriginId",
                table: "Characters",
                column: "OriginId",
                principalTable: "Locations",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Characters_Locations_OriginId",
                table: "Characters");

            migrationBuilder.AddForeignKey(
                name: "FK_Characters_Locations_OriginId",
                table: "Characters",
                column: "OriginId",
                principalTable: "Locations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
