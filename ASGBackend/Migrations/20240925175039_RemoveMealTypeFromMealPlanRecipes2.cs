using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ASGBackend.Migrations
{
    /// <inheritdoc />
    public partial class RemoveMealTypeFromMealPlanRecipes2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MealType",
                table: "MealPlanRecipes");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MealType",
                table: "MealPlanRecipes",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
