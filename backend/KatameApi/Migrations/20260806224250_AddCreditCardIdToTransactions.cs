using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KatameApi.Migrations
{
    /// <inheritdoc />
    public partial class AddCreditCardIdToTransactions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CreditCardId",
                table: "Transactions",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_CreditCardId",
                table: "Transactions",
                column: "CreditCardId");

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_CreditCards_CreditCardId",
                table: "Transactions",
                column: "CreditCardId",
                principalTable: "CreditCards",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_CreditCards_CreditCardId",
                table: "Transactions");

            migrationBuilder.DropIndex(
                name: "IX_Transactions_CreditCardId",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "CreditCardId",
                table: "Transactions");
        }
    }
}
