using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Delivery.Migrations
{
    /// <inheritdoc />
    public partial class UpdateContext : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Dishes_Establishments_EstablishmentId",
                table: "Dishes");

            migrationBuilder.AddForeignKey(
                name: "FK_Dishes_Establishments_EstablishmentId",
                table: "Dishes",
                column: "EstablishmentId",
                principalTable: "Establishments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Dishes_Establishments_EstablishmentId",
                table: "Dishes");

            migrationBuilder.AddForeignKey(
                name: "FK_Dishes_Establishments_EstablishmentId",
                table: "Dishes",
                column: "EstablishmentId",
                principalTable: "Establishments",
                principalColumn: "Id");
        }
    }
}
