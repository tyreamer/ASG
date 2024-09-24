using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ASGBackend.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DietaryRestrictions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IsVegetarian = table.Column<bool>(type: "bit", nullable: false),
                    IsVegan = table.Column<bool>(type: "bit", nullable: false),
                    IsGlutenFree = table.Column<bool>(type: "bit", nullable: false),
                    IsPescatarian = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DietaryRestrictions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DietaryRestrictionsUsers",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    DietaryRestrictionsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DietaryRestrictionsUsers", x => new { x.UserId, x.DietaryRestrictionsId });
                    table.ForeignKey(
                        name: "FK_DietaryRestrictionsUsers_DietaryRestrictions_DietaryRestrictionsId",
                        column: x => x.DietaryRestrictionsId,
                        principalTable: "DietaryRestrictions",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "NutritionalGoals",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HighProtein = table.Column<bool>(type: "bit", nullable: false),
                    LowCarb = table.Column<bool>(type: "bit", nullable: false),
                    LowFat = table.Column<bool>(type: "bit", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true)
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
                    UserId = table.Column<int>(type: "int", nullable: false),
                    TotalMealsPerWeek = table.Column<int>(type: "int", nullable: false),
                    DietaryRestrictionsId = table.Column<int>(type: "int", nullable: false),
                    Allergies = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FavoriteCuisines = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DislikedFoods = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NutritionalGoalsId = table.Column<int>(type: "int", nullable: false),
                    CalorieTarget = table.Column<int>(type: "int", nullable: false)
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

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    IsAuthenticated = table.Column<bool>(type: "bit", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BudgetPerMeal_Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    BudgetPerMeal_Currency = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HouseholdSize = table.Column<int>(type: "int", nullable: false),
                    PreferencesId = table.Column<int>(type: "int", nullable: false),
                    CookingSkillLevel = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_UserPreferences_PreferencesId",
                        column: x => x.PreferencesId,
                        principalTable: "UserPreferences",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DietaryRestrictionsUsers_DietaryRestrictionsId",
                table: "DietaryRestrictionsUsers",
                column: "DietaryRestrictionsId");

            migrationBuilder.CreateIndex(
                name: "IX_NutritionalGoals_UserId",
                table: "NutritionalGoals",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserPreferences_DietaryRestrictionsId",
                table: "UserPreferences",
                column: "DietaryRestrictionsId");

            migrationBuilder.CreateIndex(
                name: "IX_UserPreferences_NutritionalGoalsId",
                table: "UserPreferences",
                column: "NutritionalGoalsId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_PreferencesId",
                table: "Users",
                column: "PreferencesId");

            migrationBuilder.AddForeignKey(
                name: "FK_DietaryRestrictionsUsers_Users_UserId",
                table: "DietaryRestrictionsUsers",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_NutritionalGoals_Users_UserId",
                table: "NutritionalGoals",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserPreferences_DietaryRestrictions_DietaryRestrictionsId",
                table: "UserPreferences");

            migrationBuilder.DropForeignKey(
                name: "FK_NutritionalGoals_Users_UserId",
                table: "NutritionalGoals");

            migrationBuilder.DropTable(
                name: "DietaryRestrictionsUsers");

            migrationBuilder.DropTable(
                name: "DietaryRestrictions");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "UserPreferences");

            migrationBuilder.DropTable(
                name: "NutritionalGoals");
        }
    }
}
