using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarrefourPolaire.Migrations
{
    /// <inheritdoc />
    public partial class RemoveConfirmedGroupNumberIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_RegistrationGroups_GroupNumber",
                table: "RegistrationGroups");

            migrationBuilder.DropColumn(
                name: "GroupNumber",
                table: "RegistrationGroups");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "GroupNumber",
                table: "RegistrationGroups",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_RegistrationGroups_GroupNumber",
                table: "RegistrationGroups",
                column: "GroupNumber",
                unique: true,
                filter: "\"Confirmed\" = TRUE");
        }
    }
}
