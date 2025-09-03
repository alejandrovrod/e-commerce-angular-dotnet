using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ECommerce.Product.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddBrandCountryAndFoundedYear : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Country",
                table: "Brands",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "FoundedYear",
                table: "Brands",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Country",
                table: "Brands");

            migrationBuilder.DropColumn(
                name: "FoundedYear",
                table: "Brands");
        }
    }
}
