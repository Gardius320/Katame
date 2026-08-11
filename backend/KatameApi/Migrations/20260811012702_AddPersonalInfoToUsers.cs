using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KatameApi.Migrations
{
    /// <inheritdoc />
    public partial class AddPersonalInfoToUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DocumentId",
                table: "Users",
                type: "varchar(255)",
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "Users",
                type: "longtext",
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                table: "Users",
                type: "longtext",
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "Users",
                type: "longtext",
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "DocumentId", "FirstName", "LastName", "PhoneNumber" },
                values: new object[] { "ADMIN-0001", "Admin", "Katame", "0000000000" });

            // Backfill defensivo para cualquier otra fila que ya exista (no solo la fila
            // semilla de arriba): usa el Username como nombre y un placeholder único por Id
            // para la cédula, así el índice único de abajo no falla por duplicados vacíos.
            migrationBuilder.Sql("UPDATE `Users` SET `FirstName` = `Username` WHERE `FirstName` = '';");
            migrationBuilder.Sql("UPDATE `Users` SET `LastName` = 'Katame' WHERE `LastName` = '';");
            migrationBuilder.Sql("UPDATE `Users` SET `PhoneNumber` = '0000000000' WHERE `PhoneNumber` = '';");
            migrationBuilder.Sql("UPDATE `Users` SET `DocumentId` = CONCAT('SIN-CEDULA-', `Id`) WHERE `DocumentId` = '';");

            migrationBuilder.CreateIndex(
                name: "IX_Users_DocumentId",
                table: "Users",
                column: "DocumentId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Users_DocumentId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "DocumentId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "LastName",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "Users");
        }
    }
}
