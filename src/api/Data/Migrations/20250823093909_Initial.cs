using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Quibble.Migrations;

/// <inheritdoc />
public partial class Initial : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Users",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                ObjectId = table.Column<Guid>(type: "uuid", nullable: false),
                DisplayName = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Users", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "Games",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                Slug = table.Column<string>(type: "character(6)", fixedLength: true, maxLength: 6, nullable: false),
                OwnerId = table.Column<Guid>(type: "uuid", nullable: false),
                State = table.Column<int>(type: "integer", nullable: false),
                Title = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Games", x => x.Id);
                table.ForeignKey(
                    name: "FK_Games_Users_OwnerId",
                    column: x => x.OwnerId,
                    principalTable: "Users",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "Participants",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                UserId = table.Column<Guid>(type: "uuid", nullable: false),
                GameId = table.Column<Guid>(type: "uuid", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Participants", x => x.Id);
                table.ForeignKey(
                    name: "FK_Participants_Games_GameId",
                    column: x => x.GameId,
                    principalTable: "Games",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_Participants_Users_UserId",
                    column: x => x.UserId,
                    principalTable: "Users",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "Rounds",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                GameId = table.Column<Guid>(type: "uuid", nullable: false),
                Order = table.Column<int>(type: "integer", nullable: false),
                State = table.Column<int>(type: "integer", nullable: false),
                Title = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                Description = table.Column<string>(type: "character varying(400)", maxLength: 400, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Rounds", x => x.Id);
                table.ForeignKey(
                    name: "FK_Rounds_Games_GameId",
                    column: x => x.GameId,
                    principalTable: "Games",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "Questions",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                RoundId = table.Column<Guid>(type: "uuid", nullable: false),
                Order = table.Column<int>(type: "integer", nullable: false),
                State = table.Column<int>(type: "integer", nullable: false),
                Points = table.Column<int>(type: "integer", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Questions", x => x.Id);
                table.ForeignKey(
                    name: "FK_Questions_Rounds_RoundId",
                    column: x => x.RoundId,
                    principalTable: "Rounds",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "QuestionAnswer",
            columns: table => new
            {
                QuestionId = table.Column<Guid>(type: "uuid", nullable: false),
                Answer = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_QuestionAnswer", x => x.QuestionId);
                table.ForeignKey(
                    name: "FK_QuestionAnswer_Questions_QuestionId",
                    column: x => x.QuestionId,
                    principalTable: "Questions",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "QuestionBody",
            columns: table => new
            {
                QuestionId = table.Column<Guid>(type: "uuid", nullable: false),
                Text = table.Column<string>(type: "character varying(400)", maxLength: 400, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_QuestionBody", x => x.QuestionId);
                table.ForeignKey(
                    name: "FK_QuestionBody_Questions_QuestionId",
                    column: x => x.QuestionId,
                    principalTable: "Questions",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "SubmittedAnswers",
            columns: table => new
            {
                QuestionAnswerId = table.Column<Guid>(type: "uuid", nullable: false),
                ParticipantId = table.Column<Guid>(type: "uuid", nullable: false),
                Points = table.Column<int>(type: "integer", nullable: true),
                Answer = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_SubmittedAnswers", x => new { x.QuestionAnswerId, x.ParticipantId });
                table.ForeignKey(
                    name: "FK_SubmittedAnswers_Participants_ParticipantId",
                    column: x => x.ParticipantId,
                    principalTable: "Participants",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_SubmittedAnswers_QuestionAnswer_QuestionAnswerId",
                    column: x => x.QuestionAnswerId,
                    principalTable: "QuestionAnswer",
                    principalColumn: "QuestionId",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_SubmittedAnswers_ParticipantId",
            table: "SubmittedAnswers",
            column: "ParticipantId");

        migrationBuilder.CreateIndex(
            name: "IX_Games_OwnerId",
            table: "Games",
            column: "OwnerId");

        migrationBuilder.CreateIndex(
            name: "IX_Games_Slug",
            table: "Games",
            column: "Slug",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_Participants_GameId",
            table: "Participants",
            column: "GameId");

        migrationBuilder.CreateIndex(
            name: "IX_Participants_UserId_GameId",
            table: "Participants",
            columns: ["UserId", "GameId"],
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_Questions_RoundId",
            table: "Questions",
            column: "RoundId");

        migrationBuilder.CreateIndex(
            name: "IX_Rounds_GameId",
            table: "Rounds",
            column: "GameId");

        migrationBuilder.CreateIndex(
            name: "IX_Users_ObjectId",
            table: "Users",
            column: "ObjectId",
            unique: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "Answer");

        migrationBuilder.DropTable(
            name: "QuestionBody");

        migrationBuilder.DropTable(
            name: "Participants");

        migrationBuilder.DropTable(
            name: "QuestionAnswer");

        migrationBuilder.DropTable(
            name: "Questions");

        migrationBuilder.DropTable(
            name: "Rounds");

        migrationBuilder.DropTable(
            name: "Games");

        migrationBuilder.DropTable(
            name: "Users");
    }
}
