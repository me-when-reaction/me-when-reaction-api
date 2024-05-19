using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MeWhenAPI.Infrastructure.MigrationHistory
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("SET TimeZone='UTC';");
            migrationBuilder.CreateTable(
                name: "ms_user",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    email = table.Column<string>(type: "text", nullable: false),
                    password = table.Column<string>(type: "text", nullable: false),
                    identity_id = table.Column<string>(type: "text", nullable: false),
                    identity_provider = table.Column<string>(type: "text", nullable: false),
                    date_in = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    date_up = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    date_del = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    user_in = table.Column<Guid>(type: "uuid", nullable: false),
                    user_up = table.Column<Guid>(type: "uuid", nullable: true),
                    user_del = table.Column<Guid>(type: "uuid", nullable: true),
                    deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ms_user", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "tr_image",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    link = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: false),
                    source = table.Column<string>(type: "text", nullable: false),
                    extension = table.Column<string>(type: "text", nullable: false),
                    age_rating = table.Column<int>(type: "integer", nullable: false),
                    date_in = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    date_up = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    date_del = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    user_in = table.Column<Guid>(type: "uuid", nullable: false),
                    user_up = table.Column<Guid>(type: "uuid", nullable: true),
                    user_del = table.Column<Guid>(type: "uuid", nullable: true),
                    deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tr_image", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "tr_tag",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    age_rating = table.Column<int>(type: "integer", nullable: false),
                    date_in = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    date_up = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    date_del = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    user_in = table.Column<Guid>(type: "uuid", nullable: false),
                    user_up = table.Column<Guid>(type: "uuid", nullable: true),
                    user_del = table.Column<Guid>(type: "uuid", nullable: true),
                    deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tr_tag", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "tr_image_tag",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    image_id = table.Column<Guid>(type: "uuid", nullable: false),
                    tag_id = table.Column<Guid>(type: "uuid", nullable: false),
                    date_in = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    date_up = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    date_del = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    user_in = table.Column<Guid>(type: "uuid", nullable: false),
                    user_up = table.Column<Guid>(type: "uuid", nullable: true),
                    user_del = table.Column<Guid>(type: "uuid", nullable: true),
                    deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tr_image_tag", x => x.id);
                    table.ForeignKey(
                        name: "FK_tr_image_tag_tr_image_image_id",
                        column: x => x.image_id,
                        principalTable: "tr_image",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_tr_image_tag_tr_tag_tag_id",
                        column: x => x.tag_id,
                        principalTable: "tr_tag",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_tr_image_tag_image_id",
                table: "tr_image_tag",
                column: "image_id");

            migrationBuilder.CreateIndex(
                name: "IX_tr_image_tag_tag_id",
                table: "tr_image_tag",
                column: "tag_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ms_user");

            migrationBuilder.DropTable(
                name: "tr_image_tag");

            migrationBuilder.DropTable(
                name: "tr_image");

            migrationBuilder.DropTable(
                name: "tr_tag");
        }
    }
}
