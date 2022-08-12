using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineStore.Migrations
{
    public partial class Payment3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "taxes",
                table: "payment",
                newName: "number");

            migrationBuilder.RenameColumn(
                name: "date",
                table: "payment",
                newName: "holderName");

            migrationBuilder.RenameColumn(
                name: "amount",
                table: "payment",
                newName: "expirationDate");

            migrationBuilder.CreateTable(
                name: "items",
                columns: table => new
                {
                    ID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    amount = table.Column<bool>(type: "bit", nullable: false),
                    description = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_items", x => x.ID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "items");

            migrationBuilder.RenameColumn(
                name: "number",
                table: "payment",
                newName: "taxes");

            migrationBuilder.RenameColumn(
                name: "holderName",
                table: "payment",
                newName: "date");

            migrationBuilder.RenameColumn(
                name: "expirationDate",
                table: "payment",
                newName: "amount");
        }
    }
}
