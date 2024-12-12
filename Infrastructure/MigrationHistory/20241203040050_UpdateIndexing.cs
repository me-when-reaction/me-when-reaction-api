using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MeWhenAPI.Infrastructure.MigrationHistory
{
    /// <inheritdoc />
    public partial class UpdateIndexing : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<List<string>>(
                name: "alias",
                table: "tr_tag",
                type: "text[]",
                nullable: false,
                defaultValue: new List<string>());

            migrationBuilder.CreateIndex(
                name: "IX_tr_tag_name",
                table: "tr_tag",
                column: "name")
                .Annotation("Npgsql:IndexInclude", new[] { "id", "age_rating", "alias" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_tr_tag_name",
                table: "tr_tag");

            migrationBuilder.DropColumn(
                name: "alias",
                table: "tr_tag");
        }
    }
}
