using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace FlowerShop.API.Migrations
{
    /// <inheritdoc />
    public partial class AddBusinessLogicModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DefaultVaseLifeDays",
                table: "Products",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "WikiNotes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Content = table.Column<string>(type: "text", nullable: false),
                    Category = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WikiNotes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProductWikiNote",
                columns: table => new
                {
                    ProductsId = table.Column<int>(type: "integer", nullable: false),
                    WikiNotesId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductWikiNote", x => new { x.ProductsId, x.WikiNotesId });
                    table.ForeignKey(
                        name: "FK_ProductWikiNote_Products_ProductsId",
                        column: x => x.ProductsId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductWikiNote_WikiNotes_WikiNotesId",
                        column: x => x.WikiNotesId,
                        principalTable: "WikiNotes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductWikiNote_WikiNotesId",
                table: "ProductWikiNote",
                column: "WikiNotesId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductWikiNote");

            migrationBuilder.DropTable(
                name: "WikiNotes");

            migrationBuilder.DropColumn(
                name: "DefaultVaseLifeDays",
                table: "Products");
        }
    }
}
