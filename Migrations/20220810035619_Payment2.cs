using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineStore.Migrations
{
    public partial class Payment2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "userID",
                table: "payment",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_payment_userID",
                table: "payment",
                column: "userID");

            migrationBuilder.AddForeignKey(
                name: "FK_payment_users_userID",
                table: "payment",
                column: "userID",
                principalTable: "users",
                principalColumn: "ID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_payment_users_userID",
                table: "payment");

            migrationBuilder.DropIndex(
                name: "IX_payment_userID",
                table: "payment");

            migrationBuilder.AlterColumn<string>(
                name: "userID",
                table: "payment",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);
        }
    }
}
