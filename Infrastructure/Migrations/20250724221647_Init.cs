using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Results",
                columns: table => new
                {
                    FileName = table.Column<string>(type: "text", nullable: false),
                    DeltaDate = table.Column<int>(type: "integer", nullable: false),
                    MinDateAndTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    AvgExecutionTime = table.Column<decimal>(type: "numeric", nullable: false),
                    AvgValue = table.Column<decimal>(type: "numeric", nullable: false),
                    MedianValue = table.Column<decimal>(type: "numeric", nullable: false),
                    MaxValue = table.Column<double>(type: "double precision", nullable: false),
                    MinValue = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Results", x => x.FileName);
                });

            migrationBuilder.CreateTable(
                name: "Values",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FileName = table.Column<string>(type: "text", nullable: false),
                    Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ExecutionTime = table.Column<int>(type: "integer", nullable: false),
                    Value = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Values", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Results");

            migrationBuilder.DropTable(
                name: "Values");
        }
    }
}
