using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ASGBackend.Migrations
{
    /// <inheritdoc />
    public partial class RemoveDietaryRestrictionsFromUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NutritionalGoals_Users_UserId",
                table: "NutritionalGoals");

            migrationBuilder.DropTable(
                name: "DietaryRestrictionsUsers");

            migrationBuilder.DropIndex(
                name: "IX_NutritionalGoals_UserId",
                table: "NutritionalGoals");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "NutritionalGoals");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "NutritionalGoals",
                type: "nvarchar(450)",
                nullable: true);

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
                    table.ForeignKey(
                        name: "FK_DietaryRestrictionsUsers_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_NutritionalGoals_UserId",
                table: "NutritionalGoals",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_DietaryRestrictionsUsers_DietaryRestrictionsId",
                table: "DietaryRestrictionsUsers",
                column: "DietaryRestrictionsId");

            migrationBuilder.AddForeignKey(
                name: "FK_NutritionalGoals_Users_UserId",
                table: "NutritionalGoals",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
