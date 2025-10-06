using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarrefourPolaire.Migrations
{
    /// <inheritdoc />
    public partial class AddGroupConfirmToParticipants : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "GroupConfirmed",
                table: "Participants",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GroupConfirmed",
                table: "Participants");
        }
    }
}
