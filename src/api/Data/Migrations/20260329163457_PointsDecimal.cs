using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Quibble.Migrations;

/// <inheritdoc />
public partial class PointsDecimal : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterColumn<decimal>(
            name: "Points",
            table: "SubmittedAnswers",
            type: "numeric",
            nullable: true,
            oldClrType: typeof(int),
            oldType: "integer",
            oldNullable: true);

        migrationBuilder.AlterColumn<decimal>(
            name: "Points",
            table: "Questions",
            type: "numeric",
            nullable: false,
            oldClrType: typeof(int),
            oldType: "integer");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterColumn<int>(
            name: "Points",
            table: "SubmittedAnswers",
            type: "integer",
            nullable: true,
            oldClrType: typeof(decimal),
            oldType: "numeric",
            oldNullable: true);

        migrationBuilder.AlterColumn<int>(
            name: "Points",
            table: "Questions",
            type: "integer",
            nullable: false,
            oldClrType: typeof(decimal),
            oldType: "numeric");
    }
}
