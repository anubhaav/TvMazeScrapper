using Microsoft.EntityFrameworkCore.Migrations;

namespace TvMazeScrapper.Migrations
{
    public partial class FirstMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PageNumberDb",
                columns: table => new
                {
                    PageNumberModelId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PageNumber = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PageNumberDb", x => x.PageNumberModelId);
                });

            migrationBuilder.CreateTable(
                name: "ShowDb",
                columns: table => new
                {
                    ShowId = table.Column<long>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Id = table.Column<long>(nullable: false),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShowDb", x => x.ShowId);
                });

            migrationBuilder.CreateTable(
                name: "CastDb",
                columns: table => new
                {
                    CastId = table.Column<long>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Id = table.Column<long>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Birthday = table.Column<string>(nullable: true),
                    ShowModelId = table.Column<int>(nullable: false),
                    ShowModelShowId = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CastDb", x => x.CastId);
                    table.ForeignKey(
                        name: "FK_CastDb_ShowDb_ShowModelShowId",
                        column: x => x.ShowModelShowId,
                        principalTable: "ShowDb",
                        principalColumn: "ShowId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CastDb_Birthday",
                table: "CastDb",
                column: "Birthday");

            migrationBuilder.CreateIndex(
                name: "IX_CastDb_CastId",
                table: "CastDb",
                column: "CastId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CastDb_ShowModelShowId",
                table: "CastDb",
                column: "ShowModelShowId");

            migrationBuilder.CreateIndex(
                name: "IX_ShowDb_Id",
                table: "ShowDb",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_ShowDb_ShowId",
                table: "ShowDb",
                column: "ShowId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CastDb");

            migrationBuilder.DropTable(
                name: "PageNumberDb");

            migrationBuilder.DropTable(
                name: "ShowDb");
        }
    }
}
