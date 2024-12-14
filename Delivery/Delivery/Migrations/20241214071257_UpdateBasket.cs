using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Delivery.Migrations
{
    /// <inheritdoc />
    public partial class UpdateBasket : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Dishes_Baskets_BasketId",
                table: "Dishes");

            migrationBuilder.DropIndex(
                name: "IX_Dishes_BasketId",
                table: "Dishes");

            migrationBuilder.DropColumn(
                name: "BasketId",
                table: "Dishes");

            migrationBuilder.CreateTable(
                name: "BasketDishes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    BasketId = table.Column<int>(type: "integer", nullable: false),
                    DishId = table.Column<int>(type: "integer", nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BasketDishes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BasketDishes_Baskets_BasketId",
                        column: x => x.BasketId,
                        principalTable: "Baskets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BasketDishes_Dishes_DishId",
                        column: x => x.DishId,
                        principalTable: "Dishes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BasketDishes_BasketId",
                table: "BasketDishes",
                column: "BasketId");

            migrationBuilder.CreateIndex(
                name: "IX_BasketDishes_DishId",
                table: "BasketDishes",
                column: "DishId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BasketDishes");

            migrationBuilder.AddColumn<int>(
                name: "BasketId",
                table: "Dishes",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Dishes_BasketId",
                table: "Dishes",
                column: "BasketId");

            migrationBuilder.AddForeignKey(
                name: "FK_Dishes_Baskets_BasketId",
                table: "Dishes",
                column: "BasketId",
                principalTable: "Baskets",
                principalColumn: "Id");
        }
    }
}
