using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HighLowGameApi.Migrations
{
    /// <inheritdoc />
    public partial class GameInitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GameSessions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StartCardValue = table.Column<int>(type: "int", nullable: false),
                    StartCardSuit = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CurrentCardValue = table.Column<int>(type: "int", nullable: false),
                    CurrentCardSuit = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GameStatus = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    InsertedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameSessions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GameRounds",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoundNumber = table.Column<int>(type: "int", nullable: false),
                    PreviousCardValue = table.Column<int>(type: "int", nullable: false),
                    PreviousCardSuit = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GuessType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BetAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NewCardValue = table.Column<int>(type: "int", nullable: false),
                    NewCardSuit = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsWin = table.Column<bool>(type: "bit", nullable: false),
                    WinAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    GameSessionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InsertedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameRounds", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GameRounds_GameSessions",
                        column: x => x.GameSessionId,
                        principalTable: "GameSessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GameRounds_GameSessionId",
                table: "GameRounds",
                column: "GameSessionId");

            migrationBuilder.CreateIndex(
                name: "IX_GameSessions_UserId",
                table: "GameSessions",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GameRounds");

            migrationBuilder.DropTable(
                name: "GameSessions");
        }
    }
}
