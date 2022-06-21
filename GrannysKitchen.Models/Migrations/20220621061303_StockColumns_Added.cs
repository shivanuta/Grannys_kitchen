using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GrannysKitchen.Models.Migrations
{
    public partial class StockColumns_Added : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "ActualPrice",
                table: "FoodItems",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "AvailableStock",
                table: "FoodItems",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "DeliveryCharges",
                table: "FoodItems",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DiscountPercentage",
                table: "FoodItems",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TotalStock",
                table: "FoodItems",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ActualPrice",
                table: "FoodItems");

            migrationBuilder.DropColumn(
                name: "AvailableStock",
                table: "FoodItems");

            migrationBuilder.DropColumn(
                name: "DeliveryCharges",
                table: "FoodItems");

            migrationBuilder.DropColumn(
                name: "DiscountPercentage",
                table: "FoodItems");

            migrationBuilder.DropColumn(
                name: "TotalStock",
                table: "FoodItems");
        }
    }
}
