using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Kafee.Api.Migrations
{
    /// <inheritdoc />
    public partial class init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MenuItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Price = table.Column<decimal>(type: "numeric", nullable: false),
                    AmountInStock = table.Column<int>(type: "integer", nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MenuItems", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "MenuItems",
                columns: new[] { "Id", "AmountInStock", "Name", "Price" },
                values: new object[,]
                {
                    { new Guid("17c61e41-3953-42cd-8f88-d3f698869b35"), 2, "Picon", 5.50m },
                    { new Guid("a89f6cd7-4693-457b-9009-02205dbbfe45"), 20, "Cola", 2.80m },
                    { new Guid("ca79e9b3-312c-43d4-a6f7-27ad7ac842e3"), 0, "Gin-Tonic", 9.50m },
                    { new Guid("e4fa19bf-6981-4e50-a542-7c9b26e9ec31"), 14, "Jupiler", 3.00m }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MenuItems");
        }
    }
}
