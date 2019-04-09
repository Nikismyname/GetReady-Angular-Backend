using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GetReady.Data.Migrations
{
    public partial class initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Username = table.Column<string>(nullable: true),
                    HashedPassword = table.Column<string>(nullable: true),
                    Salt = table.Column<string>(nullable: true),
                    FirstName = table.Column<string>(nullable: true),
                    LastName = table.Column<string>(nullable: true),
                    Role = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "QuestionSheets",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    Difficulty = table.Column<int>(nullable: true),
                    Importance = table.Column<int>(nullable: false),
                    Order = table.Column<int>(nullable: false),
                    QuestionSheetId = table.Column<int>(nullable: true),
                    UserId = table.Column<int>(nullable: true),
                    IsGlobal = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestionSheets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuestionSheets_QuestionSheets_QuestionSheetId",
                        column: x => x.QuestionSheetId,
                        principalTable: "QuestionSheets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_QuestionSheets_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "GlobalQuestionPackages",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    Question = table.Column<string>(nullable: true),
                    Answer = table.Column<string>(nullable: true),
                    Comment = table.Column<string>(nullable: true),
                    Difficulty = table.Column<int>(nullable: false),
                    Order = table.Column<int>(nullable: false),
                    Approved = table.Column<bool>(nullable: false),
                    DerivedFromId = table.Column<int>(nullable: true),
                    QuestionSheetId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GlobalQuestionPackages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GlobalQuestionPackages_QuestionSheets_QuestionSheetId",
                        column: x => x.QuestionSheetId,
                        principalTable: "QuestionSheets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PersonalQuestionPackages",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    Question = table.Column<string>(nullable: true),
                    Answer = table.Column<string>(nullable: true),
                    Comment = table.Column<string>(nullable: true),
                    Difficulty = table.Column<int>(nullable: false),
                    Order = table.Column<int>(nullable: false),
                    AnswerRate = table.Column<float>(nullable: false),
                    TimesBeingAnswered = table.Column<int>(nullable: false),
                    YourBestAnswer = table.Column<string>(nullable: true),
                    LatestScores = table.Column<string>(nullable: true),
                    DerivedFromId = table.Column<int>(nullable: true),
                    QuestionSheetId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonalQuestionPackages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PersonalQuestionPackages_QuestionSheets_QuestionSheetId",
                        column: x => x.QuestionSheetId,
                        principalTable: "QuestionSheets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GlobalQuestionPackages_QuestionSheetId",
                table: "GlobalQuestionPackages",
                column: "QuestionSheetId");

            migrationBuilder.CreateIndex(
                name: "IX_PersonalQuestionPackages_QuestionSheetId",
                table: "PersonalQuestionPackages",
                column: "QuestionSheetId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionSheets_QuestionSheetId",
                table: "QuestionSheets",
                column: "QuestionSheetId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionSheets_UserId",
                table: "QuestionSheets",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Username",
                table: "Users",
                column: "Username",
                unique: true,
                filter: "[Username] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GlobalQuestionPackages");

            migrationBuilder.DropTable(
                name: "PersonalQuestionPackages");

            migrationBuilder.DropTable(
                name: "QuestionSheets");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
