using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Talkpush_BackgroundService.Migrations
{
    /// <inheritdoc />
    public partial class changedatatypeofIDandcandidateId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Candidate_Id",
                table: "CreatedTickets",
                type: "int",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Candidate_Id",
                table: "CreatedTickets",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }
    }
}
