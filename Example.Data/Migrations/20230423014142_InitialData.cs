using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Example.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var utcNow = DateTime.UtcNow;
            migrationBuilder.InsertData(
                table: "Category",
                columns: new[] { "Name", "ImageUrl", "CreatedAt", "UpdatedAt" },
                values: new object[] { "Pessoa", "https://avatars.githubusercontent.com/u/28938084?v=4", utcNow, utcNow }
            );

            migrationBuilder.InsertData(
                table: "Product",
                columns: new[] { "Name", "Quantity", "Price", "CategoryId", "CreatedAt", "UpdatedAt" },
                values: new object[] { "Ismael", 10F, 10m, "1", utcNow, utcNow }
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Category",
                keyColumn: "Name",
                keyValue: "Pessoa"
            );

            migrationBuilder.DeleteData(
                table: "Product",
                keyColumn: "Name",
                keyValue: "Ismael"
            );
        }
    }
}
