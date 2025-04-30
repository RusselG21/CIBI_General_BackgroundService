using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Talkpush_BackgroundService.Migrations
{
    /// <inheritdoc />
    public partial class Initialmigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CreatedTickets",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Candidate_Primary_Id = table.Column<int>(type: "int", nullable: false),
                    Candidate_Id = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TicketNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TicketCreated = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CreatedTickets", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CreatedTickets");
        }
    }
}
