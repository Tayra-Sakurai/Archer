using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Caesar.Migrations
{
    /// <inheritdoc />
    public partial class UpdatePaymentMethod : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Remainder",
                table: "PaymentMethods",
                type: "REAL",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Remainder",
                table: "PaymentMethods");
        }
    }
}
