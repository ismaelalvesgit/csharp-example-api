using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Example.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder mb)
        {
            var utcNow = DateTime.UtcNow;
            mb.InsertData(
                table: "Category",
                columns: new[] { "Name", "ImageUrl", "CreatedAt", "UpdatedAt" },
                values: new object[] { "Pessoa", "https://avatars.githubusercontent.com/u/28938084?v=4", utcNow, utcNow }
            );
            
            mb.InsertData(
                table: "Product",
                columns: new[] { "Name", "Quantity", "Price", "CategoryId", "CreatedAt", "UpdatedAt" },
                values: new object[] { "Ismael", 10F, 10m, "1", utcNow, utcNow }
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder mb)
        {
            mb.DeleteData(
                table: "Category",
                keyColumn: "Name",
                keyValue: "Pessoa"
            );

            mb.DeleteData(
                table: "Product",
                keyColumn: "Name",
                keyValue: "Ismael"
            );
        }
    }
}
