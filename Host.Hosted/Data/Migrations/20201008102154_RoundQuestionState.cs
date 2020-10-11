using Microsoft.EntityFrameworkCore.Migrations;

namespace Quibble.Host.Hosted.Data.Migrations
{
    public partial class RoundQuestionState : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "State",
                table: "QuibbleRounds",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "State",
                table: "QuibbleQuestions",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "State",
                table: "QuibbleRounds");

            migrationBuilder.DropColumn(
                name: "State",
                table: "QuibbleQuestions");
        }
    }
}
