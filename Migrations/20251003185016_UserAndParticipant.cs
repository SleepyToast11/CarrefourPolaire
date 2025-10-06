using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarrefourPolaire.Migrations
{
    /// <inheritdoc />
    public partial class UserAndParticipant : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ShareUuid",
                table: "RegistrationGroups");

            migrationBuilder.AlterColumn<string>(
                name: "OwnerName",
                table: "RegistrationGroups",
                type: "character varying(256)",
                maxLength: 256,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "OwnerEmail",
                table: "RegistrationGroups",
                type: "character varying(256)",
                maxLength: 256,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<bool>(
                name: "Confirmed",
                table: "RegistrationGroups",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "RegistrationGroups",
                type: "character varying(256)",
                maxLength: 256,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "GroupNumber",
                table: "Participants",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "RegistrationId",
                table: "EmailLoginTokens",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "GroupInviteTokens",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    GroupId = table.Column<Guid>(type: "uuid", nullable: false),
                    Token = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Active = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupInviteTokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GroupInviteTokens_RegistrationGroups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "RegistrationGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RegistrationGroups_GroupNumber",
                table: "RegistrationGroups",
                column: "GroupNumber",
                unique: true,
                filter: "\"Confirmed\" = TRUE");

            migrationBuilder.CreateIndex(
                name: "IX_EmailLoginTokens_RegistrationId",
                table: "EmailLoginTokens",
                column: "RegistrationId");

            migrationBuilder.CreateIndex(
                name: "IX_GroupInviteTokens_GroupId",
                table: "GroupInviteTokens",
                column: "GroupId");

            migrationBuilder.AddForeignKey(
                name: "FK_EmailLoginTokens_RegistrationGroups_RegistrationId",
                table: "EmailLoginTokens",
                column: "RegistrationId",
                principalTable: "RegistrationGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmailLoginTokens_RegistrationGroups_RegistrationId",
                table: "EmailLoginTokens");

            migrationBuilder.DropTable(
                name: "GroupInviteTokens");

            migrationBuilder.DropIndex(
                name: "IX_RegistrationGroups_GroupNumber",
                table: "RegistrationGroups");

            migrationBuilder.DropIndex(
                name: "IX_EmailLoginTokens_RegistrationId",
                table: "EmailLoginTokens");

            migrationBuilder.DropColumn(
                name: "Confirmed",
                table: "RegistrationGroups");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "RegistrationGroups");

            migrationBuilder.DropColumn(
                name: "GroupNumber",
                table: "Participants");

            migrationBuilder.DropColumn(
                name: "RegistrationId",
                table: "EmailLoginTokens");

            migrationBuilder.AlterColumn<string>(
                name: "OwnerName",
                table: "RegistrationGroups",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(256)",
                oldMaxLength: 256);

            migrationBuilder.AlterColumn<string>(
                name: "OwnerEmail",
                table: "RegistrationGroups",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(256)",
                oldMaxLength: 256);

            migrationBuilder.AddColumn<Guid>(
                name: "ShareUuid",
                table: "RegistrationGroups",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }
    }
}
