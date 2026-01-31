using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CasinoAppBackend.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateSequence(
                name: "TransactionNumberSequence",
                startValue: 100000L);

            migrationBuilder.CreateTable(
                name: "AppSettings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Key = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Value = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    InsertedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppSettings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Countries",
                columns: table => new
                {
                    CountryCode = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: false),
                    CountryName = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Countries", x => x.CountryCode);
                });

            migrationBuilder.CreateTable(
                name: "Games",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    IsEnabled = table.Column<bool>(type: "bit", nullable: false),
                    MinBet = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MaxBet = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Category = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    InsertedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Games", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Username = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Password = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false),
                    Firstname = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    Lastname = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    UserRole = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    InsertedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Admins",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InsertedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Admins", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Admins_Users",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GameSessions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ExternalSessionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GameStatus = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EndedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GameId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InsertedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameSessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GameSessions_Games",
                        column: x => x.GameId,
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GameSessions_Users",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Players",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BirthDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Gender = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StreetName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    StreetNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    PostalCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    City = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    IsAgeVerified = table.Column<bool>(type: "bit", nullable: false),
                    HasAcceptedTerms = table.Column<bool>(type: "bit", nullable: false),
                    IsKycVerified = table.Column<bool>(type: "bit", nullable: false),
                    IsSelfExcluded = table.Column<bool>(type: "bit", nullable: false),
                    SelfExclusionStart = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SelfExclusionEnd = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SelfExclusionPeriod = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CountryCode = table.Column<string>(type: "nvarchar(2)", nullable: false),
                    InsertedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Players", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Players_Countries",
                        column: x => x.CountryCode,
                        principalTable: "Countries",
                        principalColumn: "CountryCode",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Players_Users",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Accounts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Balance = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    PlayerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InsertedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Accounts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Accounts_Players",
                        column: x => x.PlayerId,
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "KycDocuments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DocumentType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DocumentNumber = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false),
                    ExpireDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    KycStatus = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    KycCheckDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    KycCheckedBy = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    PlayerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InsertedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KycDocuments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KycDocuments_Players",
                        column: x => x.PlayerId,
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PlayerBanAudits",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ChangedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ChangedByUsername = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IsBanned = table.Column<bool>(type: "bit", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    PlayerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InsertedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerBanAudits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlayerBanAudits_Players",
                        column: x => x.PlayerId,
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PlayerBanAudits_Users_ChangedByUserId",
                        column: x => x.ChangedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PlayerDetailsAudits",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ChangedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ChangedByUsername = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    FieldName = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    OldValue = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    NewValue = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    PlayerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InsertedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerDetailsAudits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlayerDetailsAudits_Players",
                        column: x => x.PlayerId,
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PlayerLimitAudits",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FieldName = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    OldLimit = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NewLimit = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PlayerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InsertedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerLimitAudits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlayerLimitAudits_Players",
                        column: x => x.PlayerId,
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PlayerLimits",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DepositDailyLimit = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PendingDepositDailyLimit = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    PendingDepositDailyLimitStart = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    DepositWeeklyLimit = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PendingDepositWeeklyLimit = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    PendingDepositWeeklyLimitStart = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    DepositMonthlyLimit = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PendingDepositMonthlyLimit = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    PendingDepositMonthlyLimitStart = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LossDailyLimit = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PendingLossDailyLimit = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    PendingLossDailyLimitStart = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LossWeeklyLimit = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PendingLossWeeklyLimit = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    PendingLossWeeklyLimitStart = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LossMonthlyLimit = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PendingLossMonthlyLimit = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    PendingLossMonthlyLimitStart = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    PlayerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InsertedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerLimits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlayerLimits_Players",
                        column: x => x.PlayerId,
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PlayerSelfExclusionAudits",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SelfExclusionStart = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    SelfExclusionEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    SelfExclusionPeriod = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PlayerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InsertedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerSelfExclusionAudits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlayerSelfExclusionAudits_Players",
                        column: x => x.PlayerId,
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Transactions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TransactionType = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    GameId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    GameRoundId = table.Column<int>(type: "int", nullable: true),
                    GameName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TransactionNumber = table.Column<long>(type: "bigint", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    TransactionStatus = table.Column<int>(type: "int", maxLength: 15, nullable: false),
                    OldBalance = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    NewBalance = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    AccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InsertedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Transactions_Accounts",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Transactions_Games",
                        column: x => x.GameId,
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Attachments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BlobName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ContentType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Extension = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    KycDocumentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InsertedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Attachments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Attachments_KycDocuments",
                        column: x => x.KycDocumentId,
                        principalTable: "KycDocuments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_PlayerId",
                table: "Accounts",
                column: "PlayerId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Admins_UserId",
                table: "Admins",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppSettings_Key",
                table: "AppSettings",
                column: "Key",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Attachments_KycDocumentId",
                table: "Attachments",
                column: "KycDocumentId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Games_Name",
                table: "Games",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_GameSessions_ExternalSessionId",
                table: "GameSessions",
                column: "ExternalSessionId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GameSessions_GameId",
                table: "GameSessions",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_GameSessions_UserId",
                table: "GameSessions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_KycDocuments_PlayerId",
                table: "KycDocuments",
                column: "PlayerId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PlayerBanAudits_ChangedByUserId",
                table: "PlayerBanAudits",
                column: "ChangedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerBanAudits_PlayerId",
                table: "PlayerBanAudits",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerDetailsAudits_PlayerId",
                table: "PlayerDetailsAudits",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerLimitAudits_PlayerId",
                table: "PlayerLimitAudits",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerLimits_PlayerId",
                table: "PlayerLimits",
                column: "PlayerId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Players_CountryCode",
                table: "Players",
                column: "CountryCode");

            migrationBuilder.CreateIndex(
                name: "IX_Players_UserId",
                table: "Players",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PlayerSelfExclusionAudits_PlayerId",
                table: "PlayerSelfExclusionAudits",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_AccountId_InsertedAt_TransactionType",
                table: "Transactions",
                columns: new[] { "AccountId", "InsertedAt", "TransactionType" });

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_GameId",
                table: "Transactions",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_GameRoundId",
                table: "Transactions",
                column: "GameRoundId");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_InsertedAt",
                table: "Transactions",
                column: "InsertedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_TransactionNumber",
                table: "Transactions",
                column: "TransactionNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_PhoneNumber",
                table: "Users",
                column: "PhoneNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Username",
                table: "Users",
                column: "Username",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Admins");

            migrationBuilder.DropTable(
                name: "AppSettings");

            migrationBuilder.DropTable(
                name: "Attachments");

            migrationBuilder.DropTable(
                name: "GameSessions");

            migrationBuilder.DropTable(
                name: "PlayerBanAudits");

            migrationBuilder.DropTable(
                name: "PlayerDetailsAudits");

            migrationBuilder.DropTable(
                name: "PlayerLimitAudits");

            migrationBuilder.DropTable(
                name: "PlayerLimits");

            migrationBuilder.DropTable(
                name: "PlayerSelfExclusionAudits");

            migrationBuilder.DropTable(
                name: "Transactions");

            migrationBuilder.DropTable(
                name: "KycDocuments");

            migrationBuilder.DropTable(
                name: "Accounts");

            migrationBuilder.DropTable(
                name: "Games");

            migrationBuilder.DropTable(
                name: "Players");

            migrationBuilder.DropTable(
                name: "Countries");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropSequence(
                name: "TransactionNumberSequence");
        }
    }
}
