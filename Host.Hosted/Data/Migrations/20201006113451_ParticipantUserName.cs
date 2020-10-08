using Microsoft.EntityFrameworkCore.Migrations;

namespace Quibble.Host.Hosted.Data.Migrations
{
    public partial class ParticipantUserName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserName",
                table: "QuibbleParticipants",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserName",
                table: "QuibbleParticipants");
        }
    }
}
