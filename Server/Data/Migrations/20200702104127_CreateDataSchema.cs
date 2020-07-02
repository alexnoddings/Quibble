using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Quibble.Server.Data.Migrations
{
    public partial class CreateDataSchema : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetUserSettings",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    UserId = table.Column<string>(nullable: false),
                    UseNightMode = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserSettings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserSettings_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Quizzes",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    OwnerId = table.Column<string>(nullable: false),
                    Title = table.Column<string>(nullable: false),
                    State = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Quizzes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Quizzes_AspNetUsers_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "QuizParticipants",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    UserId = table.Column<string>(nullable: true),
                    QuizId = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuizParticipants", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuizParticipants_Quizzes_QuizId",
                        column: x => x.QuizId,
                        principalTable: "Quizzes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_QuizParticipants_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "QuizRounds",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    QuizId = table.Column<Guid>(nullable: false),
                    Title = table.Column<string>(nullable: false),
                    State = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuizRounds", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuizRounds_Quizzes_QuizId",
                        column: x => x.QuizId,
                        principalTable: "Quizzes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "QuizQuestions",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    RoundId = table.Column<Guid>(nullable: false),
                    Body = table.Column<string>(nullable: false),
                    Answer = table.Column<string>(nullable: false),
                    State = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuizQuestions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuizQuestions_QuizRounds_RoundId",
                        column: x => x.RoundId,
                        principalTable: "QuizRounds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "QuizSubmittedAnswers",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    ParticipantId = table.Column<Guid>(nullable: false),
                    QuestionId = table.Column<Guid>(nullable: false),
                    State = table.Column<int>(nullable: false),
                    Value = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuizSubmittedAnswers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuizSubmittedAnswers_QuizParticipants_ParticipantId",
                        column: x => x.ParticipantId,
                        principalTable: "QuizParticipants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_QuizSubmittedAnswers_QuizQuestions_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "QuizQuestions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserSettings_UserId",
                table: "AspNetUserSettings",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_QuizParticipants_QuizId",
                table: "QuizParticipants",
                column: "QuizId");

            migrationBuilder.CreateIndex(
                name: "IX_QuizParticipants_UserId",
                table: "QuizParticipants",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_QuizQuestions_RoundId",
                table: "QuizQuestions",
                column: "RoundId");

            migrationBuilder.CreateIndex(
                name: "IX_QuizRounds_QuizId",
                table: "QuizRounds",
                column: "QuizId");

            migrationBuilder.CreateIndex(
                name: "IX_QuizSubmittedAnswers_ParticipantId",
                table: "QuizSubmittedAnswers",
                column: "ParticipantId");

            migrationBuilder.CreateIndex(
                name: "IX_QuizSubmittedAnswers_QuestionId",
                table: "QuizSubmittedAnswers",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_Quizzes_OwnerId",
                table: "Quizzes",
                column: "OwnerId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetUserSettings");

            migrationBuilder.DropTable(
                name: "QuizSubmittedAnswers");

            migrationBuilder.DropTable(
                name: "QuizParticipants");

            migrationBuilder.DropTable(
                name: "QuizQuestions");

            migrationBuilder.DropTable(
                name: "QuizRounds");

            migrationBuilder.DropTable(
                name: "Quizzes");
        }
    }
}
