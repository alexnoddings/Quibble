using Microsoft.EntityFrameworkCore.Migrations;

namespace Quibble.Server.Data.Migrations;

public partial class AddRoundQuestionOrder : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<int>(
            name: "Order",
            table: "Rounds",
            type: "int",
            nullable: false,
            defaultValue: 0);

        migrationBuilder.AddColumn<int>(
            name: "Order",
            table: "Questions",
            type: "int",
            nullable: false,
            defaultValue: 0);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "Order",
            table: "Rounds");

        migrationBuilder.DropColumn(
            name: "Order",
            table: "Questions");
    }
}
