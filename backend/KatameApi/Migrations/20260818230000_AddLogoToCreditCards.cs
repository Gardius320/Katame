using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KatameApi.Migrations
{
    /// <inheritdoc />
    public partial class AddLogoToCreditCards : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LogoDataUrl",
                table: "CreditCards",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LogoDataUrl",
                table: "CreditCards");
        }
    }
}
