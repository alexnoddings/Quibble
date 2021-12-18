using Microsoft.EntityFrameworkCore.Migrations;
using System;

#nullable disable

namespace Quibble.Server.Data.Migrations
{
	public partial class DropShadowProperty : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropForeignKey(
				name: "FK_SubmittedAnswers_AspNetUsers_AppUserId",
				table: "SubmittedAnswers");

			migrationBuilder.DropIndex(
				name: "IX_SubmittedAnswers_AppUserId",
				table: "SubmittedAnswers");

			migrationBuilder.DropColumn(
				name: "AppUserId",
				table: "SubmittedAnswers");
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.AddColumn<Guid>(
				name: "AppUserId",
				table: "SubmittedAnswers",
				type: "uniqueidentifier",
				nullable: true);

			migrationBuilder.CreateIndex(
				name: "IX_SubmittedAnswers_AppUserId",
				table: "SubmittedAnswers",
				column: "AppUserId");

			migrationBuilder.AddForeignKey(
				name: "FK_SubmittedAnswers_AspNetUsers_AppUserId",
				table: "SubmittedAnswers",
				column: "AppUserId",
				principalTable: "AspNetUsers",
				principalColumn: "Id");
		}
	}
}
