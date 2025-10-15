using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarrefourPolaire.Migrations
{
    /// <inheritdoc />
    public partial class AddUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //Delete all current token for new method
            migrationBuilder.Sql("""DELETE FROM "EmailVerificationTokens";""");
            
            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "EmailVerificationTokens",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Email = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    RegistrationGroupId = table.Column<Guid>(type: "uuid", nullable: true),
                    ParticipantId = table.Column<Guid>(type: "uuid", nullable: true),
                    UserRoles = table.Column<int[]>(type: "integer[]", nullable: false),
                    Confirmed = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_Participants_ParticipantId",
                        column: x => x.ParticipantId,
                        principalTable: "Participants",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Users_RegistrationGroups_RegistrationGroupId",
                        column: x => x.RegistrationGroupId,
                        principalTable: "RegistrationGroups",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_EmailVerificationTokens_UserId",
                table: "EmailVerificationTokens",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_ParticipantId",
                table: "Users",
                column: "ParticipantId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_RegistrationGroupId",
                table: "Users",
                column: "RegistrationGroupId");

            migrationBuilder.AddForeignKey(
                name: "FK_EmailVerificationTokens_Users_UserId",
                table: "EmailVerificationTokens",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmailVerificationTokens_Users_UserId",
                table: "EmailVerificationTokens");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropIndex(
                name: "IX_EmailVerificationTokens_UserId",
                table: "EmailVerificationTokens");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "EmailVerificationTokens");
        }
    }
}
