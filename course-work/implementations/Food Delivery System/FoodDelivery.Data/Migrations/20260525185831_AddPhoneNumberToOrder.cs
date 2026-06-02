using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FoodDelivery.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddPhoneNumberToOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "Orders",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$11$ImObA9fktvmIn9VMBGUDp.ZUegQMGDZTfJkZbbmJgWbgmPGMH9giy");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "Orders");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$11$AiDHX8TcGkBJZORnJeEVRe5W44RKxL1CHvvcwkzE0uPO1zWql7asq");
        }
    }
}
