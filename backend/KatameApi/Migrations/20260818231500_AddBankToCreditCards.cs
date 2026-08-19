using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KatameApi.Migrations
{
    /// <inheritdoc />
    public partial class AddBankToCreditCards : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Bank",
                table: "CreditCards",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Bank",
                table: "CreditCards");
        }
    }
}
