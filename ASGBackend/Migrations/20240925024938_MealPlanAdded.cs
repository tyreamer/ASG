using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ASGBackend.Migrations
{
    /// <inheritdoc />
    public partial class MealPlanAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_UserPreferences_PreferencesId",
                table: "Users");

            migrationBuilder.DropTable(
                name: "UserPreferences");

            migrationBuilder.DropTable(
                name: "DietaryRestrictions");

            migrationBuilder.DropTable(
                name: "NutritionalGoals");

            migrationBuilder.DropIndex(
                name: "IX_Users_PreferencesId",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "PreferencesId",
                table: "Users",
                newName: "Preferences_UserId");

            migrationBuilder.AddColumn<string>(
                name: "Preferences_Allergies",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Preferences_CalorieTarget",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Preferences_DietaryRestrictions_Id",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "Preferences_DietaryRestrictions_IsGlutenFree",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Preferences_DietaryRestrictions_IsPescatarian",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Preferences_DietaryRestrictions_IsVegan",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Preferences_DietaryRestrictions_IsVegetarian",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Preferences_DislikedFoods",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Preferences_FavoriteCuisines",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Preferences_Id",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "Preferences_NutritionalGoals_HighProtein",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Preferences_NutritionalGoals_Id",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "Preferences_NutritionalGoals_LowCarb",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Preferences_NutritionalGoals_LowFat",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Preferences_TotalMealsPerWeek",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "MealPlans",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    WeekStartDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MealPlans", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Recipes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Ingredients = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Instructions = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CuisineType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Calories = table.Column<int>(type: "int", nullable: false),
                    PrepTime = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Recipes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MealPlanRecipes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RecipeId = table.Column<int>(type: "int", nullable: false),
                    MealPlanId = table.Column<int>(type: "int", nullable: false),
                    DayOfWeek = table.Column<int>(type: "int", nullable: false),
                    MealType = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MealPlanRecipes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MealPlanRecipes_MealPlans_MealPlanId",
                        column: x => x.MealPlanId,
                        principalTable: "MealPlans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MealPlanRecipes_Recipes_RecipeId",
                        column: x => x.RecipeId,
                        principalTable: "Recipes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MealPlanRecipes_MealPlanId",
                table: "MealPlanRecipes",
                column: "MealPlanId");

            migrationBuilder.CreateIndex(
                name: "IX_MealPlanRecipes_RecipeId",
                table: "MealPlanRecipes",
                column: "RecipeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MealPlanRecipes");

            migrationBuilder.DropTable(
                name: "MealPlans");

            migrationBuilder.DropTable(
                name: "Recipes");

            migrationBuilder.DropColumn(
                name: "Preferences_Allergies",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Preferences_CalorieTarget",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Preferences_DietaryRestrictions_Id",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Preferences_DietaryRestrictions_IsGlutenFree",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Preferences_DietaryRestrictions_IsPescatarian",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Preferences_DietaryRestrictions_IsVegan",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Preferences_DietaryRestrictions_IsVegetarian",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Preferences_DislikedFoods",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Preferences_FavoriteCuisines",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Preferences_Id",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Preferences_NutritionalGoals_HighProtein",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Preferences_NutritionalGoals_Id",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Preferences_NutritionalGoals_LowCarb",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Preferences_NutritionalGoals_LowFat",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Preferences_TotalMealsPerWeek",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "Preferences_UserId",
                table: "Users",
                newName: "PreferencesId");

            migrationBuilder.CreateTable(
                name: "DietaryRestrictions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IsGlutenFree = table.Column<bool>(type: "bit", nullable: false),
                    IsPescatarian = table.Column<bool>(type: "bit", nullable: false),
                    IsVegan = table.Column<bool>(type: "bit", nullable: false),
                    IsVegetarian = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DietaryRestrictions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NutritionalGoals",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HighProtein = table.Column<bool>(type: "bit", nullable: false),
                    LowCarb = table.Column<bool>(type: "bit", nullable: false),
                    LowFat = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NutritionalGoals", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserPreferences",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DietaryRestrictionsId = table.Column<int>(type: "int", nullable: false),
                    NutritionalGoalsId = table.Column<int>(type: "int", nullable: false),
                    Allergies = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CalorieTarget = table.Column<int>(type: "int", nullable: false),
                    DislikedFoods = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FavoriteCuisines = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TotalMealsPerWeek = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserPreferences", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserPreferences_DietaryRestrictions_DietaryRestrictionsId",
                        column: x => x.DietaryRestrictionsId,
                        principalTable: "DietaryRestrictions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserPreferences_NutritionalGoals_NutritionalGoalsId",
                        column: x => x.NutritionalGoalsId,
                        principalTable: "NutritionalGoals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_PreferencesId",
                table: "Users",
                column: "PreferencesId");

            migrationBuilder.CreateIndex(
                name: "IX_UserPreferences_DietaryRestrictionsId",
                table: "UserPreferences",
                column: "DietaryRestrictionsId");

            migrationBuilder.CreateIndex(
                name: "IX_UserPreferences_NutritionalGoalsId",
                table: "UserPreferences",
                column: "NutritionalGoalsId");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_UserPreferences_PreferencesId",
                table: "Users",
                column: "PreferencesId",
                principalTable: "UserPreferences",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
